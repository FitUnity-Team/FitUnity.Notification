using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Consumers;
using NotificationService.Data;
using NotificationService.Models.Events;
using NotificationService.Services.Implementations;
using NotificationService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "NotificationService API",
        Version = "v1",
        Description = "Microservice de notification pour la plateforme sportive"
    });
});

// MODIFICATION ICI : On utilise une version fixe de MySQL pour éviter le crash AutoDetect
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 40))));

var authServiceBaseUrl = builder.Configuration["ExternalServices:AuthServiceBaseUrl"]
    ?? throw new InvalidOperationException("ExternalServices:AuthServiceBaseUrl is not configured.");

builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(authServiceBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
var rabbitUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
var rabbitPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";
var rabbitVHost = builder.Configuration["RabbitMQ:VirtualHost"] ?? "/";

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentSucceededConsumer>();
    x.AddConsumer<EventBookingConfirmedConsumer>();
    x.AddConsumer<EventBookingCancelledConsumer>();
    x.AddConsumer<SessionReminderConsumer>();
    x.AddConsumer<RefundProcessedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitHost, rabbitVHost, h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        cfg.ReceiveEndpoint("payment-succeeded", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<PaymentSucceededConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("event-booking-confirmed", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<EventBookingConfirmedConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("event-booking-cancelled", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<EventBookingCancelledConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("session-reminder", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<SessionReminderConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("refund-processed", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<RefundProcessedConsumer>(ctx);
        });
    });
});

var app = builder.Build();

// Création de la base de données au démarrage
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    db.Database.EnsureCreated();
}

// MODIFICATION ICI : On active Swagger même en dehors du mode Développement pour Docker
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationService v1");
    c.RoutePrefix = string.Empty; // Swagger sera à la racine : http://localhost:8080/
});

app.MapControllers();

app.Run();
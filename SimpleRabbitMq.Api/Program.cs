using System.Text.Json.Serialization;
using SimpleRabbitMq.Core.Configuration;
using SimpleRabbitMq.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => { });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind RabbitMQ settings from configuration
var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqConnection>();

// Register RabbitMQ services using the extension method
builder.Services.AddRabbitMq(rabbitMqSettings);

var app = builder.Build();

//app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

var rabbitMqApi = app.MapGroup("/rabbit");
rabbitMqApi.MapPost(
    "/add-temp-handler/{listeners}",
    (int listeners, IListener listener) =>
    {
        listener.AddHandler<SimpleMessage>(
            (message) => Console.WriteLine($"Receivend message: {message}"),
            listeners
        );
    }
);

rabbitMqApi.MapPost(
    "/add-handler/{listeners}",
    (int listeners, IListener listener) =>
    {
        listener.AddHandler(new MessageReceivedHandler(), listeners);
    }
);

app.Run();

public record class SimpleMessage(string Message) : IDomainEvent;

public class MessageReceivedHandler : IHandler<SimpleMessage>
{
    public void Handle(SimpleMessage args)
    {
        //throw new NotImplementedException();
        Console.WriteLine($"Receivend message: {args.Message}");
    }
}

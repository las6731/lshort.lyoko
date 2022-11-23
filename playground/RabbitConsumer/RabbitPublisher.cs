using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.Abstractions.Services;
using RabbitConsumer.DataAccess;
using ILogger = Serilog.ILogger;

namespace RabbitConsumer;

public class RabbitPublisher : IHostedService
{
    private readonly EventRepository repository;
    private readonly IMessagingService messagingService;
    private readonly ILogger logger;

    public RabbitPublisher(IServiceProvider services, IMessagingService messagingService, ILogger logger)
    {
        this.repository = services.CreateScope().ServiceProvider.GetRequiredService<EventRepository>();
        this.messagingService = messagingService;
        this.logger = logger.ForContext<RabbitPublisher>();
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            var pendingMessages = await this.repository.GetAll();
            foreach (var e in pendingMessages) await this.PublishMessage(e);
            await this.repository.Watch(async e => await this.PublishMessage(e));
        }, cancellationToken);

        return Task.CompletedTask;
    }

    private async Task PublishMessage(EventMessage e)
    {
        var publishResult = await this.messagingService.Publish(e);
        if (publishResult)
        {
            this.logger.Information("Published message: {id}", e.Id);
            await this.repository.Delete(e.Id);
            await this.repository.CommitTransaction();
            this.logger.Information("Removed message from outbox: {id}", e.Id);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask; // nothing needed, I hope
    }
}
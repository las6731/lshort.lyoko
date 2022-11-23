using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitConsumer.DataAccess;
using ILogger = Serilog.ILogger;

namespace RabbitConsumer.Controllers;

[Route("api/v1/test")]
public class TestController : Controller
{
    private readonly ILogger logger;
    private readonly IMessagingService messagingService;
    private readonly EventRepository eventRepository;

    public TestController(ILogger logger, IMessagingService messagingService, EventRepository eventRepository)
    {
        this.logger = logger.ForContext<TestController>();
        this.messagingService = messagingService;
        this.eventRepository = eventRepository;
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<EventMessage>> PublishMessage([FromBody] EventMessage msg)
    {
        this.logger.Information("event: {msg}", msg.Name);
        // try
        // {
        //     for (int i = 0; i < 10; i++)
        //     {
        //         this.messagingService.Publish(msg);
        //     }
        // }
        // catch (Exception e)
        // {
        //     this.logger.Error(e, "Failed to publish message: {msg}", msg);
        //     return this.BadRequest();
        // }

        var stored = new StoredEventMessage()
        {
            Id = msg.Id,
            CorrelationId = msg.CorrelationId,
            Name = msg.Name,
            Payload = msg.Payload,
            AggregateVersion = msg.AggregateVersion,
            SequenceNumber = msg.SequenceNumber,
            SchemaVersion = msg.SchemaVersion,
            Timestamp = msg.Timestamp
        };
        await this.eventRepository.Insert(stored);
        
        this.logger.Information("Stored first message");

        var newMsg = new StoredEventMessage()
        {
            Id = new Guid(),
            CorrelationId = msg.CorrelationId,
            Name = msg.Name,
            Payload = "This is the second payload.",
            AggregateVersion = msg.AggregateVersion + 1,
            SequenceNumber = msg.SequenceNumber + 1,
            SchemaVersion = msg.SchemaVersion,
            Timestamp = msg.Timestamp
        };

        await this.eventRepository.Insert(newMsg);
        
        this.logger.Information("Stored second message");

        await this.eventRepository.CommitTransaction();

        return msg;
    }

    public async Task<ActionResult<List<EventMessage>>> Get()
    {
        var events = await this.eventRepository.GetAll();

        return events.Cast<EventMessage>().ToList();
    }
}
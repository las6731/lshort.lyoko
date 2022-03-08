using LShort.Lyoko.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace RabbitPublisher.Controllers;

[Route("api/v1/test")]
public class TestController : Controller
{
    private readonly ILogger logger;
    private readonly IMessagingService messagingService;

    public TestController(ILogger logger, IMessagingService messagingService)
    {
        this.logger = logger.ForContext<TestController>();
        this.messagingService = messagingService;
    }

    [HttpPost]
    [Route("publish")]
    public ActionResult<EventMessage> PublishMessage(EventMessage msg)
    {
        try
        {
            messagingService.Publish(msg);
        } catch (Exception e)
        {
            logger.Error(e, "Failed to publish message: {msg}", msg);
            return this.BadRequest();
        }

        return msg;
    }
}

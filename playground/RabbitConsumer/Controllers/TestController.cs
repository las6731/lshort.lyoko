﻿using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace RabbitConsumer.Controllers;

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
    [Route("")]
    public ActionResult<EventMessage> PublishMessage([FromBody] EventMessage msg)
    {
        this.logger.Information("event: {msg}", msg.Name);
        try
        {
            for (int i = 0; i < 10; i++)
            {
                this.messagingService.Publish(msg);
            }
        }
        catch (Exception e)
        {
            this.logger.Error(e, "Failed to publish message: {msg}", msg);
            return this.BadRequest();
        }

        return msg;
    }
}
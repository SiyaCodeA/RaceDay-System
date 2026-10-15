
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Models;
using RaceDay.API.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace RaceDay.Tests;

public class EventsControllerTests
{
   
private static RaceDayContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RaceDayContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings =>
                warnings.Ignore(
                    Microsoft.EntityFrameworkCore.Diagnostics
                        .CoreEventId.ManyServiceProvidersCreatedWarning))
            .Options;

        return new RaceDayContext(options);
    }


    [Fact]
    public async Task GetEvents_ReturnsOk()
    {
        using var context = CreateContext();

        var controller = new EventsController(context);

        var result = await controller.GetEvents();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetEvent_WhenEventDoesNotExist_ReturnsNotFound()
    {
        using var context = CreateContext();

        var controller = new EventsController(context);

        var result = await controller.GetEvent(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void CreateEvent_RequiresOrganiserRole()
    {
        var method = typeof(EventsController)
            .GetMethod(nameof(EventsController.CreateEvent));

        Assert.NotNull(method);

        var attribute = method!
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .Cast<AuthorizeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(attribute);
        Assert.Equal("Organiser", attribute!.Roles);
    }

    [Fact]
    public async Task GetEvent_WhenEventExists_ReturnsOk()
    {
        using var context = CreateContext();

        context.Events.Add(new Event
        {
            EventId = 1,
            AccountId = 1,
            EventName = "Test Event",
            EventDescription = "Testing event retrieval",
            EventDate = new DateOnly(2026, 11, 15),
            Location = "Cape Town",
            EventType = "Run",
            BannerImageUrl = null
        });

        await context.SaveChangesAsync();

        var controller = new EventsController(context);

        var result = await controller.GetEvent(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateEvent_WhenIdsDoNotMatch_ReturnsBadRequest()
    {
        using var context = CreateContext();

        var controller = new EventsController(context);

        var request = new Event
        {
            EventId = 2
        };

        var result = await controller.UpdateEvent(1, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateAndDelete_RequireOrganiserRole()
    {
        var updateMethod = typeof(EventsController)
            .GetMethod(nameof(EventsController.UpdateEvent));

        var deleteMethod = typeof(EventsController)
            .GetMethod(nameof(EventsController.DeleteEvent));

        Assert.NotNull(updateMethod);
        Assert.NotNull(deleteMethod);

        var updateAttribute = updateMethod!
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .Cast<AuthorizeAttribute>()
            .FirstOrDefault();

        var deleteAttribute = deleteMethod!
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .Cast<AuthorizeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(updateAttribute);
        Assert.NotNull(deleteAttribute);

        Assert.Equal("Organiser", updateAttribute!.Roles);
        Assert.Equal("Organiser", deleteAttribute!.Roles);
    }
}

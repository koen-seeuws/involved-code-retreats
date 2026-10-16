namespace TheSeveredRoute;

public class ExploreStepTests
{
    [Fact]
    public void ExploreStep_ReturnsPathWhenCurrentNodeIsDestination()
    {
        var solution = new Solution();
        var destination = new WaygateNode { Id = "destination" };

        var result = solution.ExploreStep(destination, "destination");

        Assert.Equal("PATH:destination", result);
        Assert.Equal(new[] { "destination" }, solution.Memory);
    }

    [Fact]
    public void ExploreStep_ReturnsFirstConnectionAndRecordsCurrentNode()
    {
        var solution = new Solution();
        var current = new WaygateNode
        {
            Id = "start",
            Connections =
            {
                new WaygateConnection { DestinationId = "first", Cost = 10 },
                new WaygateConnection { DestinationId = "second", Cost = 1 }
            }
        };

        var result = solution.ExploreStep(current, "goal");

        Assert.Equal("first", result);
        Assert.Equal(new[] { "start" }, solution.Memory);
    }

    [Fact]
    public void ExploreStep_SkipsConnectionsToPreviouslyVisitedNodes()
    {
        var solution = new Solution();
        solution.Memory.Add("visited");
        var current = new WaygateNode
        {
            Id = "current",
            Connections =
            {
                new WaygateConnection { DestinationId = "visited" },
                new WaygateConnection { DestinationId = "unvisited" }
            }
        };

        var result = solution.ExploreStep(current, "goal");

        Assert.Equal("unvisited", result);
        Assert.Equal(new[] { "visited", "current" }, solution.Memory);
    }
}
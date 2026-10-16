namespace TheSeveredRoute;

public class Solution
{
    public List<string> Memory { get; } = [];

    public string ExploreStep(WaygateNode currentNode, string destinationId)
    {
        Memory.Add(currentNode.Id);

        if(currentNode.Id == destinationId) return "PATH:" + string.Join('>', Memory);

        var nextDestination = currentNode.Connections.First();
        var nextDestinationIndex = 0;

        while(Memory.Contains(nextDestination.DestinationId)) {
            nextDestinationIndex++;
            nextDestination = currentNode.Connections.ElementAt(nextDestinationIndex);
        }
        
        return nextDestination.DestinationId;
    }
}
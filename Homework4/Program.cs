using System.Text;

internal enum TravelWays
{
    Water,
    Sky,
    Road
}

internal interface IPathFinder
{
    List<string> FindPath(string from, string to);
}

internal class RoadPathFinder : PathFinderBase
{
    public RoadPathFinder() : base(TravelWays.Road) { }
}

internal class SkyPathFinder : PathFinderBase
{
    public SkyPathFinder() : base(TravelWays.Sky) { }
}

internal class WaterPathFinder : PathFinderBase
{
    public WaterPathFinder() : base(TravelWays.Water) { }
}

internal abstract class PathFinderBase : IPathFinder
{
    private readonly TravelWays _way;

    protected PathFinderBase(TravelWays way)
    {
        _way = way;
    }

    public List<string> FindPath(string from, string to)
    {
        return FindPathRecursive(from, to, new List<string>());
    }

    private List<string> FindPathRecursive(string current, string destination, List<string> path)
    {
        if (current == destination)
        {
            path.Add(current);
            return path;
        }

        if (!Graph.Places.Contains(current) || !Graph.Places.Contains(destination))
        {
            return new List<string> { "You will not reach your destination." };
        }

        foreach (var neighbor in Graph.Map[current])
        {
            if (neighbor.Value == _way && !path.Contains(neighbor.Key))
            {
                path.Add(current);
                return FindPathRecursive(neighbor.Key, destination, path);
            }
        }

        return new List<string> { "You will not reach your destination." };
    }
}

internal class Navigator
{
    private readonly IPathFinder _pathFinder;

    public Navigator(IPathFinder pathFinder)
    {
        _pathFinder = pathFinder;
    }

    public List<string> FindPath(string from, string to)
    {
        return _pathFinder.FindPath(from, to);
    }
}

internal static class Graph
{
    public static List<string> Places { get; } = new()
    {
        "New York", "Los Angeles", "Chicago", "Houston", "Miami"
    };

    public static Dictionary<string, Dictionary<string, TravelWays>> Map { get; } = new();

    static Graph()
    {
        var rnd = new Random();

        foreach (var from in Places)
        {
            var connections = new Dictionary<string, TravelWays>();
            foreach (var to in Places)
            {
                if (from != to)
                {
                    connections[to] = (TravelWays)(rnd.Next() % 3);
                }
            }

            Map[from] = connections;
        }
    }

    public static string ToText()
    {
        var sb = new StringBuilder();
        foreach (var from in Map)
        {
            foreach (var to in from.Value)
            {
                sb.AppendLine($"From - {from.Key} to {to.Key} by {to.Value}");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}


internal class Program
{
    private static void Main()
    {
        Console.WriteLine(Graph.ToText());

        Navigator roadNavigator = new Navigator(new RoadPathFinder());
        PrintPath("road", roadNavigator.FindPath("New York", "Miami"));

        Navigator skyNavigator = new Navigator(new SkyPathFinder());
        PrintPath("sky", skyNavigator.FindPath("New York", "Miami"));

        Navigator waterNavigator = new Navigator(new WaterPathFinder());
        PrintPath("water", waterNavigator.FindPath("New York", "Miami"));

    }

    private static void PrintPath(string mode, List<string> path)
    {
        Console.WriteLine($"by {mode}");
        foreach (var location in path)
        {
            Console.Write(">" + location + " ");
        }
        Console.WriteLine("\n");
    }
}

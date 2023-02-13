namespace FlightsMetaSubscriber.App.Models;

public class IataObject
{
    private static readonly string Delimiter = "-";

    public IataObject(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; }
    public string Name { get; }

    public override string ToString()
    {
        return $"{Code} - {Name}";
    }

    public static IataObject FromString(string str)
    {
        var arr = str
            .Split(Delimiter)
            .Select(s => s.Trim())
            .ToArray();

        if (arr.Length < 2)
        {
            throw new ArgumentException("Incorrect string. String should be with one dash delimiter");
        }

        return new IataObject(arr[0], arr[1]);
    }
}

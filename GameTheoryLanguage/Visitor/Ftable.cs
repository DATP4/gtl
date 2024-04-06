static class Ftable
{
    public static Dictionary<string, string[][]> Table = new Dictionary<string, string[][]>();
    public static string[] StringArray = [];
    public static bool Contains(string key)
    {
        return Table.ContainsKey(key);
    }
    public static void Add(string key, string[][] value)
    {
        Table.Add(key, value);
    }
    public static string[][] Find(string key)
    {
        return Table[key];
    }
    public static void Print()
    {
        foreach (var pair in Table)
        {

            Console.WriteLine($"Key: {pair.Key}");
            Console.WriteLine($"Value: {pair.Value[0]}");
        }
    }
}

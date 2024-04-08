static class Objecttable
{
    public static Dictionary<string, string[][]> Table = new Dictionary<string, string[][]>();
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
}

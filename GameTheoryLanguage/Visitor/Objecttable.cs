class Objecttable
{
    public Dictionary<string, string[][]> Table = new Dictionary<string, string[][]>();
    public bool Contains(string key)
    {
        return Table.ContainsKey(key);
    }
    public void Add(string key, string[][] value)
    {
        Table.Add(key, value);
    }
    public string[][] Find(string key)
    {
        return Table[key];
    }
    public void Clear()
    {
        Table.Clear();
    }
}

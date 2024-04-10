public class Scope
{
    public Dictionary<string, string> Variables { get; } = new Dictionary<string, string>();
    public Dictionary<string, string[][]> Functions = new Dictionary<string, string[][]>();
    public void AddVariable(string name, string value)
    {
        Variables[name] = value;
    }
    public bool VtableContains(string key)
    {
        return Variables.ContainsKey(key);
    }
    public string VtableFind(string key)
    {
        return Variables[key];
    }
    public void AddFunction(string name, string[][] value)
    {
        Functions[name] = value;
    }
    public bool FtableContains(string key)
    {
        return Functions.ContainsKey(key);
    }
    public string[][] FtableFind(string key)
    {
        return Functions[key];
    }
}

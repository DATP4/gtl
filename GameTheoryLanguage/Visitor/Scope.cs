public class Scope
{
    public Dictionary<string, string> Variables { get; } = new Dictionary<string, string>();
    public void AddVariable(string name, string value)
    {
        Variables[name] = value;
    }

}

public class GtlDictionary
{
    static Dictionary<string, string> Types { get; } = new Dictionary<string, string>(){
        {"int", "int"},
        {"real", "double"},
        {"bool", "bool"}
    };

    static Dictionary<string, string> BooleanValues { get; } = new Dictionary<string, string>(){
       {"TRUE", "true"},
       {"FALSE", "false"}
    };

    public string TranslateType(string type)
    {
        return Types[type];
    }

    public string TranslateBoolean(string literal)
    {
        return BooleanValues[literal];
    }
}

class Types
{
    readonly Dictionary<string, string> _symboltypes = new Dictionary<string, string>(){
        {"int", "int"},
        {"real", "real"},
        {"bool", "bool"}
    };

    readonly Dictionary<string, string> _functiontypes = new Dictionary<string, string>(){
        {"int", "int"},
        {"real", "real"},
        {"bool", "bool"},
        {"void", "void"}
    };

    public string CheckSymbolType(string str)
    {
        if (!_symboltypes.ContainsKey(str))
        {
            throw new InvalidOperationException("No can dos ville baby doll");
        }
        return str;

    }

    public string CheckFuncType(string str)
    {
        if (!_functiontypes.ContainsKey(str))
        {
            throw new InvalidOperationException("No can dos ville baby doll");
        }
        return str;
    }

}

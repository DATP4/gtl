class Types
{
    readonly Dictionary<string, string> _types = new Dictionary<string, string>(){
        {"int", "int"},
        {"real", "real"},
        {"bool", "bool"}
    };


    public void CheckSymbolType(string str)
    {
        if (!_types.ContainsKey(str))
        {
            throw new InvalidOperationException("No can dos ville baby doll");
        }

        Type expectedType = str switch
        {
            "int" => typeof(int),
            "real" => typeof(double),
            "bool" => typeof(bool),
            _ => throw new NotSupportedException("Unsupported type: " + str),
        };
        Console.WriteLine("Type: " + expectedType);
    }

    public void CheckFuncType(string func_type, string stmt_type)
    {
        if (!_types.ContainsKey(func_type))
        {
            throw new InvalidOperationException("No can dos ville baby doll, func type");
        }

        if (!_types.ContainsKey(func_type))
        {
            throw new InvalidOperationException("No can dos ville baby doll, stmt in func type");
        }

        if (func_type != stmt_type)
        {
            throw new InvalidOperationException("No can dos ville baby doll, stmt and func type dont match");
        }
    }

    public Type GetDeclarationType(GtlParser.Variable_decContext ctx)
    {
        var typeContext = ctx.type();

        string typeText = typeContext.GetText();

        return typeText switch
        {
            "int" => typeof(int),
            "real" => typeof(float),
            "bool" => typeof(bool),
            // Add more cases for other types as needed
            _ => throw new NotSupportedException($"Type '{typeText}' not supported."),
        };
    }

    public Type GetFunctionType(GtlParser.FunctionContext ctx)
    {
        var typeContext = ctx.type();

        string typeText = typeContext.GetText();

        return typeText switch
        {
            "int" => typeof(int),
            "real" => typeof(float),
            "bool" => typeof(bool),
            // Add more cases for other types as needed
            _ => throw new NotSupportedException($"Type '{typeText}' not supported."),
        };
    }

    public Type GetArgType(GtlParser.Arg_defContext ctx)
    {
        _ = ctx.type();

        //TODO: do something here

        /*
        switch (typeText)
        {
            case "int":
                return typeof(int);
            case "real":
                return typeof(float);
            case "bool":
                return typeof(bool);
            // Add more cases for other types as needed
            default:
                throw new NotSupportedException($"Type '{typeText}' not supported.");
        }
        */
        return typeof(int);
    }
}

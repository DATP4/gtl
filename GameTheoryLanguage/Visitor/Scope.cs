using System.Data;

class Scope
{
    static public Dictionary<string, Scope> Scopes = new Dictionary<string, Scope>();

    public Dictionary<string, List<string>> VtableTypes = new Dictionary<string, List<string>>(){
        {"int", new List<string>()},
        {"real", new List<string>()},
        {"bool", new List<string>()}
    };

    public Dictionary<string, List<string>> FtableTypes = new Dictionary<string, List<string>>(){
        {"int", new List<string>()},
        {"real", new List<string>()},
        {"bool", new List<string>()}
    };

    public void AddVTableType(string key, string val)
    {
        if (!VtableTypes.ContainsKey(key))
        {
            throw new DataException("Invalid type in addvtabletype. " + key);

        }
        else if (VtableTypes[key].Contains(val))
        {
            throw new DataException("Value already in scope in vtable.");
        }
        VtableTypes[key].Add(val);
    }

    public void ClearVTable()
    {
        VtableTypes["int"].Clear();
        VtableTypes["real"].Clear();
        VtableTypes["bool"].Clear();
    }

    public Dictionary<string, List<string>> GetFtable()
    {
        return FtableTypes;
    }

    public Dictionary<string, List<string>> GetVtable()
    {
        return VtableTypes;
    }

    public void AddFTableType(string key, string fun)
    {
        if (!FtableTypes.ContainsKey(key))
        {
            throw new DataException("Invalid type in addftabletype.");

        }
        else if (FtableTypes[key].Contains(fun))
        {
            throw new DataException("Value already in scope in ftable.");
        }
        FtableTypes[key].Add(fun);
    }

    public Scope CreateFunScope(string funcID)
    {
        Scope newScope = new Scope();
        Scopes.Add(funcID, newScope);
        return newScope;
    }

    public void PopFunScope(string funcID)
    {
        _ = Scopes.Remove(funcID);
    }

}


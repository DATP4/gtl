public class GtlDictionary
{
    // Dictionary for each category that has different syntax from rust. Mapping GTL to Rust
    static Dictionary<string, Dictionary<string, string>> Dictionary { get; } = new Dictionary<string, Dictionary<string, string>>(){
       {"Boolean", new Dictionary<string, string>(){
            {"TRUE", "true"},
            {"FALSE", "false"}
       }},
       {"ArithmeticOperator", new Dictionary<string, string>(){
            {"MOD", "%"},
            {"*", "*"},
            {"/", "/"},
            {"+", "+"},
            {"-", "-"}
       }}
    };

    // Translate method to return the rust syntax.
    public string Translate(string section, string literal)
    {
        return Dictionary[section][literal];
    }
}

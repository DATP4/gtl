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
       }},
       {"BooleanOperator", new Dictionary<string, string>(){
            {"<", "<"},
            {">", ">"},
            {"<=", "<="},
            {">=", ">="},
            {"==", "=="},
            {"!=", "!="},
            {"&&", "&&"},
            {"||", "||"},
            {"^^", "!="}
       }},
       {"Type", new Dictionary<string, string>(){
            {"int", "i32"},
            {"real", "f32"},
            {"bool", "bool"}
       }},
       {"Functions", new Dictionary<string, string>(){
            {"last_move", "GameState::last_move"},
            {"move_at_turn", "GameState::move_at_turn"},
            {"player_score", "GameState::player_score"},
            {"turn", "gmst.turn"},
            {"run", "run"}
       }}
    };

    // Translate method to return the rust syntax.
    public string Translate(string section, string literal)
    {
        return Dictionary[section][literal];
    }
}

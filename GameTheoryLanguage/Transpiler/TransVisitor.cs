using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;

public class TransVisitor : GtlBaseVisitor<object>
{

    public readonly List<string> Outputfile = new List<string>();
    public readonly List<string> MovesList = new List<string>();
    private Stack<Scope> ScopeStack { get; } = new Stack<Scope>();
    private Stack<Scope> FunctionStack { get; } = new Stack<Scope>();
    GtlDictionary GtlDictionary { get; } = new GtlDictionary();

    public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
    {
        EnterScope(new Scope());
        string retString = null!;
        Outputfile.Add("#![allow(warnings)]");
        Outputfile.Add("mod library;");
        Outputfile.Add("use library::{Action, BoolExpression, Condition, Game, GameState, Moves, Payoff, Players, Strategy, Strategyspace};");
        Outputfile.Add("fn main()\n{");

        Outputfile.Add("let mut gamestate: GameState = GameState::new();");

        // Program consists of statements only, so we iterate them
        foreach (var stmt in context.statement())
        {
            retString += Visit(stmt);
            retString += "\n";
        }
        foreach (var game_stmt in context.game_variable_declaration())
        {
            retString += Visit(game_stmt);
            retString += "\n";
        }
        foreach (var game_fun in context.game_functions())
        {
            retString += Visit(game_fun);
            retString += "\n";
        }
        // Everything we visit but this, will return a string. We add it to our output rust file
        if (retString != null)
        {
            Outputfile.Add(retString);
        }
        Outputfile.Add("}");
        GtlCFile writer = new GtlCFile();
        writer.PrintFileToOutput(Outputfile);
        ExitScope();
        return null!;
    }



    public override object VisitStatement([NotNull] GtlParser.StatementContext context)
    {
        string retString = null!;
        // We need all the if statements to follow the semicolon convention
        if (context.declaration() != null)
        {
            retString += Visit(context.declaration());
        }
        if (context.function() != null)
        {
            retString += Visit(context.function());
        }
        if (context.print() != null)
        {
            retString += Visit(context.print());
        }
        return retString!;
    }

    public override object VisitLiteral([NotNull] GtlParser.LiteralContext context)
    {
        // Booleans needs extra handling
        if (context.boolean_literal() != null)
        {
            return base.VisitLiteral(context);
        }

        // Other literals than booleans matches the rust syntax
        return context.GetText();
    }

    public override object VisitBoolean_literal([NotNull] GtlParser.Boolean_literalContext context)
    {
        // Translates the boolean literal to the rust
        return GtlDictionary.Translate("Boolean", context.GetText());
    }

    public override object VisitIdExpr([NotNull] GtlParser.IdExprContext context)
    {
        if (CheckMovesList(context.ID().GetText() + ",\n"))
        {
            return $"Moves::{context.ID().GetText()}";
        }
        return IfFunctionArgumentDereference(context.ID().GetText());
    }

    public override object VisitBooleanExpr(GtlParser.BooleanExprContext context)
    {
        // Seperates the boolean expression, translates the operator, and combines them again.
        string left = (string)Visit(context.expr(0));
        string right = (string)Visit(context.expr(1));
        string op = GtlDictionary.Translate("BooleanOperator", context.op.Text);
        return $"{left} {op} {right}";
    }

    public override object VisitMemberExpr([NotNull] GtlParser.MemberExprContext context)
    {
        return $"{context.GetText()}";
    }

    public override object VisitBinaryExpr([NotNull] GtlParser.BinaryExprContext context)
    {
        // Seperates the binary expression, translates the operator, and combines them again.
        string left = (string)Visit(context.expr(0));
        string right = (string)Visit(context.expr(1));
        string op = GtlDictionary.Translate("ArithmeticOperator", context.op.Text);
        return $"{left} {op} {right}";
    }

    public override object VisitBlock([NotNull] GtlParser.BlockContext context)
    {
        string retBlockString = "";
        foreach (var dec in context.declaration())
        {
            retBlockString += (string)Visit(dec);
        }
        retBlockString += (string)Visit(context.expr());
        return retBlockString;
    }


    public override object VisitIfElse([NotNull] GtlParser.IfElseContext context)
    {
        // Beginning of if
        string retIfString = $"if {Visit(context.expr())} {'{'}\n";

        // Adds all in block to the return string
        retIfString += Visit(context.block());

        // Repeat above process for the else if blocks
        foreach (var elseIfBlock in context.elseif())
        {
            retIfString += $"\n{'}'} else if {Visit(elseIfBlock.expr())} {'{'}\n";
            retIfString += Visit(elseIfBlock.block());
        }

        // Repeats the process for the final else block
        retIfString += "\n} else {\n";
        retIfString += (string)Visit(context.@else().block()) + "}\n";
        return retIfString;
    }

    public override object VisitFunction([NotNull] GtlParser.FunctionContext context)
    {
        EnterFunctionScope(new Scope()); // Enters function scope to match it against outerscope
        EnterScope(new Scope()); // Meanwhile creates a new scope (useful if we were to enter nested scopes)

        string retFnString = $"fn {context.ID()}";

        // We only visit the arguments if there are any
        if (context.arg_def().ChildCount != 0)
        {
            retFnString += $"({VisitArg_def(context.arg_def())})";
        }
        else
        {
            retFnString += "()";
        }

        // Compare the function arguments against the outer scope and return missing variables from the outer scope
        Dictionary<string, string> missingVariables = CompareVTables();

        // Function return type
        retFnString += $" -> {GtlDictionary.Translate("Type", context.type().GetText())} {'{'}\n";

        //// Body of function
        // First we add all missing variables from the outer scope
        foreach (var v in missingVariables)
        {
            retFnString += "let " + v.Key + " = " + v.Value + ";\n";
        }

        // Adds all other statements of the body
        retFnString += Visit(context.block());

        retFnString += "\n}";
        ExitFunctionScope();
        ExitScope();
        return retFnString;
    }

    public override object VisitMethod_access([NotNull] GtlParser.Method_accessContext context)
    {
        string id = context.ID().GetText();
        string returnString = "";
        if (id.Equals("lastMove"))
        {
            returnString += GtlDictionary.Translate("Functions", "last_move");
            returnString += $"(&gamestate, &{context.arg_call().expr()[0].GetText()}.to_string())";
        }
        else if (id.Equals("moveAtTurn"))
        {
            returnString += GtlDictionary.Translate("Functions", "move_at_turn");
            returnString += $"(&gamestate, &{context.arg_call().expr()[0].GetText()}.to_string(), {context.arg_call().expr(1)})";
        }
        else if (id.Equals("playerScore"))
        {
            returnString += GtlDictionary.Translate("Functions", "player_score");
            returnString += $"(&gamestate, &{context.arg_call().expr()[0].GetText()}.to_string(), {context.arg_call().expr(1)})";
        }
        else if (id.Equals("turn"))
        {
            returnString += GtlDictionary.Translate("Functions", "turn");
        }
        return returnString;
    }

    public override object VisitArg_def([NotNull] GtlParser.Arg_defContext context)
    {
        // Creates arrays of all types and ids in the arguments
        GtlParser.TypeContext[] types = context.type();
        Antlr4.Runtime.Tree.ITerminalNode[] ids = context.ID();

        // We add the argument to the vtable of the function scope to match it against the outer scopes vtable
        GetCurrentFunctionScope().AddVariable(ids[0].GetText(), "");

        // We do not want a comma before the first argument, hence added outside loop
        string retArgDefString = $"{ids[0].GetText()}: &{GtlDictionary.Translate("Type", types[0].GetText())}";

        // Adds the remaining arguments
        for (int i = 1; i < types.Length; i++)
        {
            GetCurrentFunctionScope().AddVariable(ids[i].GetText(), "");
            retArgDefString += $", {ids[i].GetText()}: &{GtlDictionary.Translate("Type", types[i].GetText())}";
        }

        return retArgDefString;
    }

    public override object VisitArgCallExpr([NotNull] GtlParser.ArgCallExprContext context)
    {
        // Follows the format of how we call a function with arguments
        return $"{context.ID().GetText()}({Visit(context.arg_call())})";
    }

    public override object VisitGame_functions([NotNull] GtlParser.Game_functionsContext context)
    {
        string returnString = "";
        returnString += "Game::run(&mut ";
        returnString += $"{context.ID().GetText()}, ";
        string val = (string)Visit(context.expr());
        returnString += "&mut " + val + ")";
        returnString += ";";
        return returnString;
    }

    public override object VisitArg_call([NotNull] GtlParser.Arg_callContext context)
    {
        string retArgCalls = "";
        // Ensures that the arguments are expressed as addresses due to call by reference
        foreach (var expr in context.expr())
        {
            retArgCalls += "&(" + Visit(expr) + "), ";
        }

        // Removes the last comma and space
        if (retArgCalls.Length > 0)
        {
            retArgCalls = retArgCalls.Remove(retArgCalls.Length - 2, 2);
        }

        return retArgCalls;
    }

    public override object VisitParExpr([NotNull] GtlParser.ParExprContext context)
    {
        return $"({Visit(context.expr())})";
    }
    public override object VisitUnaryExpr([NotNull] GtlParser.UnaryExprContext context)
    {
        return $"-{Visit(context.expr())}";
    }
    public override object VisitLogicalNotExpr([NotNull] GtlParser.LogicalNotExprContext context)
    {
        return $"!{Visit(context.expr())}";
    }
    public override object VisitVariable_dec([NotNull] GtlParser.Variable_decContext context)
    {
        // Adds a the variable declared to the vtable of the current scope.
        GetCurrentScope().AddVariable(context.ID().GetText(), (string)Visit(context.expr()));

        // No need for type declaration in rust, due to our typechecking prior to transpiling.
        return $"let {context.ID().GetText()} = {Visit(context.expr())};\n";
    }
    public override object VisitGame_variable_declaration([NotNull] GtlParser.Game_variable_declarationContext context)
    {
        string returnString = "";
        if (context.game_type() != null)
        {
            if (context.game_type().GetText().Equals("Game"))
            {
                returnString += $"let mut {context.ID().GetText()}: {context.game_type().GetText()} = {context.game_type().GetText()}";
                returnString += "{\n";
                returnString += Visit(context.game_expr().game_tuple());
                returnString += "};\n";
                return returnString;
            }
            returnString += $"let {context.ID().GetText()}: {context.game_type().GetText()} = {context.game_type().GetText()}";
            returnString += "{\n";

            if (context.game_type().GetText().Equals("Strategyspace"))
            {
                returnString += VisitStrategySpace(context.game_expr().array());
            }
            if (context.game_type().GetText().Equals("Strategy"))
            {
                returnString += VisitStrategy(context.game_expr().array());
            }
            if (context.game_type().GetText().Equals("Players"))
            {
                returnString += VisitPlayers(context.game_expr().array());
            }
            if (context.game_type().GetText().Equals("Payoff"))
            {
                returnString += VisitPayoff(context.game_expr().array());
            }
            if (context.game_type().GetText().Equals("Action"))
            {
                returnString += Visit(context.game_expr());
            }
        }
        else if (context.T_MOVES().GetText().Equals("Moves"))
        {
            List<string> moves = new List<string>();
            moves.Add("#[derive(Copy, Clone, Debug, PartialEq)]\n");
            moves.Add("pub enum Moves {\n");
            foreach (var move in context.array().array_type())
            {
                MovesList.Add(move.GetText() + ",\n");
                moves.Add(move.GetText() + ",\n");
            }
            moves.Add("None,\n");
            moves.Add("}\n");
            WriteToMoves(moves);
            return null!;
        }
        returnString += "};\n";
        return returnString;
    }
    public override object VisitAction([NotNull] GtlParser.ActionContext context)
    {
        string returnString = "";
        returnString += "condition: Condition::Expression(BoolExpression {\n";
        returnString += "b_val: |gamestate: &GameState| ";
        if (context.expr() == null)
        {
            returnString += "true";
        }
        else
        {
            returnString += (string)Visit(context.expr());
        }
        returnString += "}),\n";
        returnString += $"act_move: Moves::{context.move().GetText()},\n";
        return returnString;
    }
    public override object VisitGame_tuple([NotNull] GtlParser.Game_tupleContext context)
    {
        string returnString = "";
        returnString += "game_state: &mut gamestate,\n";
        returnString += "strat_space: &" + context.ID()[0].GetText() + ",\n";
        returnString += "players: &" + context.ID()[1].GetText() + ",\n";
        returnString += "pay_matrix: &" + context.ID()[2].GetText() + ",\n";
        return returnString;

    }
    public override object VisitPrint([NotNull] GtlParser.PrintContext context)
    {
        string expr = context.expr().GetText();
        return "println!(\"{:?}\", " + expr + ");\n";
    }
    private object VisitStrategySpace([NotNull] GtlParser.ArrayContext context)
    {
        string returnString = "";
        returnString += "matrix: vec![\n";
        foreach (var type in context.array_type())
        {
            foreach (var move in type.tuple().ID())
            {
                returnString += $"Moves::{move}, ";
            }
            returnString += "\n";
        }
        returnString += "],\n";
        return returnString;
    }
    private object VisitStrategy([NotNull] GtlParser.ArrayContext context)
    {
        string returnString = "";
        returnString += "strat: vec![";
        foreach (var action in context.array_type())
        {
            returnString += action.GetText() + ".clone(), ";
        }
        returnString = returnString.Remove(returnString.Length - 2, 2);
        returnString += "],\n";

        return returnString;
    }
    private object VisitPlayers([NotNull] GtlParser.ArrayContext context)
    {
        string returnString = "";
        returnString += "pl_and_strat: vec![\n";
        foreach (var player in context.array_type())
        {
            returnString += $"(\"{player.player().ID(0)}\".to_string(), {player.player().ID(1)}.clone()),\n";
        }
        returnString += "],\n";

        return returnString;
    }
    private object VisitPayoff([NotNull] GtlParser.ArrayContext context)
    {
        string returnString = "";
        returnString += "matrix: vec![\n";
        foreach (var array in context.array_type())
        {
            returnString += "vec![";
            foreach (var payoff in array.game_utility_function().array().array_type())
            {
                returnString += payoff.GetText() + ", ";
            }
            returnString = returnString.Remove(returnString.Length - 2, 2);
            returnString += "],\n";
        }
        returnString += "],\n";

        return returnString;
    }
    private void EnterFunctionScope(Scope scope)
    {
        FunctionStack.Push(scope);
    }

    private void ExitFunctionScope()
    {
        _ = FunctionStack.Pop();
    }

    private Scope GetCurrentFunctionScope()
    {
        return FunctionStack.Peek();
    }

    // Checks the outer scope against a functions scope, to identify which outer scope variables are missing in the function
    private Dictionary<string, string> CompareVTables()
    {
        Dictionary<string, string> missingVariables = new Dictionary<string, string>();
        foreach (var v in GetCurrentScope().Variables)
        {
            if (!GetCurrentFunctionScope().VtableContains(v.Key))
            {
                missingVariables.Add(v.Key, v.Value);
            }
        }
        return missingVariables;
    }

    // Used in VisitIdExpr to check if the id should be dereference or not.
    private string IfFunctionArgumentDereference(string id)
    {
        // If the function stack is empty, we know that it shouldn't be dereferenced
        if (FunctionStack.Count != 0)
        {
            // The function scope is basically the function arguments, if the id is in there, it should be dereferenced
            if (GetCurrentFunctionScope().VtableContains(id))
            {
                return "*" + id;
            }
            else
            {
                return id;
            }
        }
        return id;
    }


    public void EnterScope(Scope scope)
    {
        if (ScopeStack.Count() != 0) // Ensuring that the stack isn't empty as getCurrentScope() would return an error
        {
            foreach (var variable in GetCurrentScope().Variables) // Copying the variables from the parent scope to the child scope
            {
                scope.AddVariable(variable.Key, variable.Value);
            }
            foreach (var function in GetCurrentScope().Functions) // Copying the functions from the parent scope to the child scope
            {
                scope.AddFunction(function.Key, function.Value);
            }
        }
        ScopeStack.Push(scope);
    }
    public void ExitScope()
    {
        _ = ScopeStack.Pop();
    }
    private Scope GetCurrentScope()
    {
        return ScopeStack.Peek();
    }

    public bool CheckMovesList(string move)
    {
        return MovesList.Contains(move);
    }
    public virtual void WriteToMoves(List<string> moves)
    {
        GtlCFile writer = new GtlCFile();
        writer.PrintMovesToFile(moves);
    }

}

using Antlr4.Runtime.Misc;

public class TransVisitor : GtlBaseVisitor<object>
{

    public readonly List<string> Outputfile = new List<string>();
    private Stack<Scope> ScopeStack { get; } = new Stack<Scope>();
    private Stack<Scope> FunctionStack { get; } = new Stack<Scope>();

    GtlDictionary GtlDictionary { get; } = new GtlDictionary();

    public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
    {
        EnterScope(new Scope());
        string retString = null!;
        Outputfile.Add("fn main()\n{");
        // Program consists of statements only, so we iterate them
        foreach (var stmt in context.statement())
        {
            retString += Visit(stmt);
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
        if (context.expr() != null)
        {
            retString += Visit(context.expr()) + ";";
        }
        if (context.declaration() != null)
        {
            retString += Visit(context.declaration()) + ";";
        }
        if (context.function() != null)
        {
            retString += Visit(context.function());
        }
        if (context.game_variable_declaration() != null)
        {
            retString += Visit(context.game_variable_declaration()) + ";";
        }
        if (context.game_functions() != null)
        {
            retString += Visit(context.game_functions()) + ";";
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
        return IfFunctionArgumentDereference(context.ID().GetText());
    }

    public override object VisitBooleanExpr(GtlParser.BooleanExprContext context)
    {
        // Seperates the boolean expression, translates the operator, and combines them again.
        string left = (string)Visit(context.expr(0));
        string right = (string)Visit(context.expr(1));
        string op = context.op.Text;
        return $"{left} {op} {right}";
    }

    public override object VisitBinaryExpr([NotNull] GtlParser.BinaryExprContext context)
    {
        // Seperates the binary expression, translates the operator, and combines them again.
        string left = (string)Visit(context.expr(0));
        string right = (string)Visit(context.expr(1));
        string op = GtlDictionary.Translate("ArithmeticOperator", context.op.Text);
        return $"{left} {op} {right}";
    }


    public override object VisitIfElse([NotNull] GtlParser.IfElseContext context)
    {
        // Placeholder to check if the last statement is a declaration
        GtlParser.StatementContext lastStmt = null!;
        // Beginning of if
        string retIfString = $"if {Visit(context.expr())} {'{'}\n";

        // Finds last non-function statement of the function
        foreach (var stmt in context.statement().Reverse())
        {
            if (stmt.function() == null)
            {
                lastStmt = stmt;
                break;
            }
        }
        // Appending all content of the if body
        foreach (var stmt in context.statement())
        {
            if (stmt == lastStmt && lastStmt.declaration() != null)
            {
                retIfString += Visit(stmt);
            }
            else if (stmt != lastStmt)
            {
                retIfString += Visit(stmt);
            }
        }
        // Ensures that the last statment is returnable
        // As rust can't return declarations, we return the id of the declaration on the next line
        if (lastStmt.declaration() != null)
        {
            retIfString += lastStmt.declaration().ID().GetText();
        }
        else
        {
            retIfString += Visit(lastStmt); // Ensures that the last non-function statement is returned
            retIfString = retIfString.Remove(retIfString.Length - 1, 1); // Removes the semicolon added from VisitStatement
        }

        // Repeat above process for the else if blocks
        foreach (var elseIfBlock in context.elseif())
        {
            retIfString += $"\n{'}'} else if {Visit(elseIfBlock.expr())} {'{'}\n";
            foreach (var stmt in elseIfBlock.statement().Reverse())
            {
                if (stmt.function() == null)
                {
                    lastStmt = stmt;
                    break;
                }
            }
            foreach (var stmt in elseIfBlock.statement())
            {
                if (stmt == lastStmt && lastStmt.declaration() != null)
                {
                    retIfString += Visit(stmt);
                }
                else if (stmt != lastStmt)
                {
                    retIfString += Visit(stmt);
                }
            }
            if (lastStmt.declaration() != null)
            {
                retIfString += lastStmt.declaration().ID().GetText();
            }
            else
            {
                retIfString += Visit(lastStmt);
                retIfString = retIfString.Remove(retIfString.Length - 1, 1);
            }
        }

        // Repeats the process for the final else block
        retIfString += "\n} else {\n";
        foreach (var stmt in context.@else().statement().Reverse())
        {
            if (stmt.function() == null)
            {
                lastStmt = stmt;
                break;
            }
        }
        foreach (var stmt in context.@else().statement())
        {
            if (stmt == lastStmt && lastStmt.declaration() != null)
            {
                retIfString += Visit(stmt);
            }
            else if (stmt != lastStmt)
            {
                retIfString += Visit(stmt);
            }
        }
        if (lastStmt.declaration() != null)
        {
            retIfString += lastStmt.declaration().ID().GetText();
        }
        else
        {
            retIfString += Visit(lastStmt);
            retIfString = retIfString.Remove(retIfString.Length - 1, 1);
        }
        retIfString += "\n}\n";
        return retIfString;
    }

    public override object VisitFunction([NotNull] GtlParser.FunctionContext context)
    {
        EnterFunctionScope(new Scope()); // Enters function scope to match it against outerscope
        EnterScope(new Scope()); // Meanwhile creates a new scope (useful if we were to enter nested scopes)

        string retFnString = $"fn {context.ID()}";
        GtlParser.StatementContext lastStmt = null!;

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

        // Finds last non-function statement of the function
        foreach (var stmt in context.statement().Reverse())
        {
            if (stmt.function() == null)
            {
                lastStmt = stmt;
                break;
            }
        }

        //// Body of function
        // First we add all missing variables from the outer scope
        foreach (var v in missingVariables)
        {
            retFnString += "let " + v.Key + "=" + v.Value + ";\n";
        }

        // Adds all other statements of the body
        foreach (var stmt in context.statement())
        {
            if (stmt == lastStmt && lastStmt.declaration() != null)
            {
                retFnString += Visit(stmt);
            }
            else if (stmt != lastStmt)
            {
                retFnString += Visit(stmt);
            }
        }

        // Ensures that the last statment is returnable
        // As rust can't return declarations, we return the id of the declaration on the next line
        if (lastStmt.declaration() != null)
        {
            retFnString += lastStmt.declaration().ID().GetText();
        }
        else
        {
            retFnString += Visit(lastStmt); // Ensures that the last non-function statement is returned
            retFnString = retFnString.Remove(retFnString.Length - 1, 1); // Removes the semicolon added from VisitStatement
        }

        retFnString += "\n}";
        ExitFunctionScope();
        ExitScope();
        return retFnString;
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

    public override object VisitArg_call([NotNull] GtlParser.Arg_callContext context)
    {
        string retArgCalls = "";
        // Ensures that the arguments are expressed as addresses due to call by reference
        foreach (var expr in context.expr())
        {
            retArgCalls += "&" + Visit(expr);
        }

        return retArgCalls;
    }

    public override object VisitDeclaration([NotNull] GtlParser.DeclarationContext context)
    {
        // Adds a the variable declared to the vtable of the current scope.
        GetCurrentScope().AddVariable(context.ID().GetText(), context.expr().GetText());

        // No need for type declaration in rust, due to our typechecking prior to transpiling.
        return $"let {context.ID().GetText()} = {Visit(context.expr())}\n";
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


    private void EnterScope(Scope scope)
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
    private void ExitScope()
    {
        _ = ScopeStack.Pop();
    }
    private Scope GetCurrentScope()
    {
        return ScopeStack.Peek();
    }
}

using System.Globalization;

using Antlr4.Runtime.Misc;

public class CustomGtlVisitor : GtlBaseVisitor<object>
{
    private Stack<Scope> ScopeStack { get; } = new Stack<Scope>();

    public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
    {
        EnterScope(new Scope());
        Console.WriteLine("Visiting program");
        _ = base.VisitProgram(context);
        ExitScope();
        return null!;
    }

    public override object VisitStatement([NotNull] GtlParser.StatementContext context)
    {
        Console.WriteLine("Visiting statement");
        _ = base.VisitStatement(context);
        return null!;
    }

    public override object VisitDeclaration([NotNull] GtlParser.DeclarationContext context)
    {
        Console.WriteLine("Visiting declaration");
        string type = context.type().GetText();
        string variable = context.ID().GetText();
        string valueType = (string)Visit(context.expr());
        if (!type.Equals(valueType))
        {
            throw new NotSupportedException($"Declaration expected type {type} but recieved " + valueType);
        }
        ScopeStack.Peek().AddVariable(variable, type);
        return null!;
    }

    public override object VisitFunction([NotNull] GtlParser.FunctionContext context)
    {
        //check ftable, if it exists check input type against expected type and return functions return type, else add to ftable
        Console.WriteLine("Visiting function");


        string functionId = context.ID().GetText();
        GtlParser.TypeContext[] typeContext = context.arg_def().type();
        string[] stringArray = [];
        foreach (var type in typeContext)
        {
            stringArray = stringArray.Append(type.GetText()).ToArray();
        }
        string[][] functionTypes = [stringArray, [context.type().GetText()]];
        Ftable.Add(functionId, functionTypes);
        EnterScope(new Scope());

        Scope funcVTable = new Scope();

        string func_type = context.type().GetText();

        foreach (var stmt in context.statement())
        {
            if (stmt.expr() != null)
            {
                funcVTable.Variables.Clear();
                string type = (string)Visit(stmt.expr());
                funcVTable.AddVariable(type, "expr");
            }
            if (stmt.declaration() != null)
            {
                ScopeStack.Peek().AddVariable(stmt.declaration().ID().GetText(), stmt.declaration().type().GetText());
                funcVTable.Variables.Clear();
                funcVTable.AddVariable(stmt.declaration().type().GetText(), stmt.declaration().ID().GetText());
                continue;
            }
            if (stmt.@if() != null)
            {
                string type = "";
                if (stmt.@if().@else() != null)
                {
                    funcVTable.Variables.Clear();
                    foreach (var elsestmt in stmt.@if().@else().statement())
                    {
                        type = GetTypeFromStatement(elsestmt);
                    }
                    funcVTable.AddVariable(type, "else");
                }
                if (stmt.@if().@elseif() != null)
                {
                    if (CheckElseInElseIf(stmt.@if().@elseif()))
                    {
                        funcVTable.Variables.Clear();
                    }
                    string iftype = "";
                    foreach (var ifstmt in stmt.@if().statement())
                    {
                        iftype = GetTypeFromStatement(ifstmt);
                    }
                    funcVTable.AddVariable(GetTypeFromElseIf(stmt.@if().elseif(), iftype), "elseif");

                }

                foreach (var ifstmt in stmt.@if().statement())
                {
                    type = GetTypeFromStatement(ifstmt);
                }

                funcVTable.AddVariable(type, "if");
            }

        }


        foreach (var e in funcVTable.Variables)
        {
            if (e.Key == func_type)
            {
                continue;
            }
            else
            {
                throw new FieldAccessException("Non return values has been saved and thats a no no");
            }
        }

        var _ = base.VisitFunction(context);
        ExitScope();

        return null!;
    }

    public override object VisitIf(GtlParser.IfContext context)
    {
        Console.WriteLine("Visiting if statement");
        EnterScope(new Scope());
        foreach (var stmt in context.statement())
        {
            _ = base.Visit(stmt);
        }
        ExitScope();
        if (context.elseif() != null)
        {
            _ = base.Visit(context.elseif());
        }
        else if (context.@else() != null)
        {
            _ = base.Visit(context.@else());
        }

        return null!;
    }

    public override object VisitElseif([NotNull] GtlParser.ElseifContext context)
    {
        Console.WriteLine("Visiting elseif statement");
        EnterScope(new Scope());
        foreach (var stmt in context.statement())
        {
            _ = base.Visit(stmt);
        }
        ExitScope();
        if (context.elseif() != null)
        {
            _ = base.Visit(context.elseif());
        }
        else if (context.@else() != null)
        {
            _ = base.Visit(context.@else());
        }
        return null!;
    }


    public override object VisitElse([NotNull] GtlParser.ElseContext context)
    {
        Console.WriteLine("Visiting else statement");
        EnterScope(new Scope());
        foreach (var stmt in context.statement())
        {
            _ = base.Visit(stmt);
        }
        ExitScope();
        return null!;
    }

    /*
    -----------------------------------------------------------------------------Expressions-----------------------------------------------------------------------------------------
    */
    public override object VisitLiteralExpr(GtlParser.LiteralExprContext context)
    {
        Console.WriteLine("Visiting literal expr");
        string val = context.GetText();


        if (int.TryParse(val, out _))
        {
            return "int";
        }

        if (double.TryParse(val, CultureInfo.InvariantCulture, out _))
        {
            return "real";
        }

        if (bool.TryParse(val, out _))
        {
            return "bool";
        }
        throw new NotSupportedException($"literal expression expected type but recieved " + val);
    }
    public override object VisitBinaryExpr(GtlParser.BinaryExprContext context)
    {
        Console.WriteLine("Visiting binary expr");
        object left = (string)Visit(context.expr(0));
        object right = (string)Visit(context.expr(1));
        if (left.Equals("int") && right.Equals("int"))
        {
            return "int";
        }
        if (left.Equals("real") && right.Equals("real"))
        {
            return "real";
        }
        throw new NotSupportedException($"binary expression expected type {left} but recieved type " + right);
    }
    public override object VisitBooleanExpr(GtlParser.BooleanExprContext context)
    {
        Console.WriteLine("Visiting boolean expr");
        string left = (string)Visit(context.expr(0));
        string right = (string)Visit(context.expr(1));
        string op = context.op.Text;
        if (left.Equals("bool") && right.Equals("bool") &&
        (op == "==" || op == "!=" || op == "&&" || op == "||" || op == "^^"))
        {
            return "bool";
        }
        if (left.Equals("int") && right.Equals("int") &&
        (op == "==" || op == "<" || op == ">" || op == "<=" || op == ">=" || op == "!="))
        {
            return "bool";
        }
        if (left.Equals("real") && right.Equals("real") &&
        (op == "==" || op == "<" || op == ">" || op == "<=" || op == ">=" || op == "!="))
        {
            return "bool";
        }
        throw new NotSupportedException($"boolean expression expected type {left} but recieved type " + right);
    }
    public override object VisitLogicalNotExpr(GtlParser.LogicalNotExprContext context)
    {
        Console.WriteLine("Visiting logical not expr");
        string expr = (string)Visit(context.expr());
        if (expr.Equals("bool"))
        {
            return "bool";
        }
        throw new NotSupportedException("Logical not expression expected type bool but recieved type " + expr);
    }
    /*
        public override object VisitChainArgCallExpr(GtlParser.ChainArgCallExprContext context)
        {
            Check each call against ftable and return the return type of last call
        }
        public override object VisitMemberExpr(GtlParser.MemberExprContext context)
        {
            return type of member from vtable?
        }
        */

    public override object VisitArgCallExpr(GtlParser.ArgCallExprContext context)
    {
        Console.WriteLine("Visitting arg call expression");
        string functionId = context.ID().GetText();
        if (!Ftable.Contains(functionId))
        {
            throw new NotSupportedException("Function not found in Function table");
        }
        string[] stringArray = [];
        foreach (GtlParser.ExprContext exprcontext in context.arg_call().expr())
        {
            stringArray = stringArray.Append(Visit(exprcontext).ToString()).ToArray()!;
        }
        string[][] functionTypes = Ftable.Find(functionId);
        if (stringArray.Length != functionTypes[0].Length)
        {
            throw new NotSupportedException("Not the correct amount of input parameters in function");
        }
        for (int i = 0; i < stringArray.Length; i++)
        {
            if (!stringArray[i].Equals(functionTypes[0][i]))
            {
                throw new NotSupportedException($"expected type {functionTypes[0][i]} in function call but recieved {stringArray[i]}");
            }
        }
        return Ftable.Find(functionId)[1][0];
    }
    /*
    public override object VisitParExpr(GtlParser.ParExprContext context)
    {
        visit expression and return its returntype
    }
    public override object VisitUtilExpr(GtlParser.UtilExprContext context)
    {
        ??????
    }
    public override object VisitTupleExpr(GtlParser.TupleExprContext context)
    {
        ?????
    }
    public override object VisitArrayExpr(GtlParser.ArrayExprContext context)
    {
        ?????
    }
    */
    public override object VisitIdExpr(GtlParser.IdExprContext context)
    {
        Console.WriteLine("Visiting ID expression");
        string id = context.ID().GetText();
        if (ScopeStack.Peek().Contains(id))
        {
            return ScopeStack.Peek().Find(id);
        }
        throw new NotSupportedException("Variable " + id + " not found");

    }
    public override object VisitArg_call([NotNull] GtlParser.Arg_callContext context)
    {
        Console.WriteLine("Visitting Arg call");
        GtlParser.ExprContext[] expr = context.expr();
        string[] stringArray = [];
        foreach (GtlParser.ExprContext expression in expr)
        {
            var _ = stringArray.Append(Visit(expression).ToString());
        }
        Ftable.StringArray = stringArray;
        return base.VisitArg_call(context);
    }
    public override object VisitArg_def([NotNull] GtlParser.Arg_defContext context)
    {
        Console.WriteLine("Visitting arg def");
        GtlParser.TypeContext[] types = context.type();
        Antlr4.Runtime.Tree.ITerminalNode[] ids = context.ID();
        for (int i = 0; i < types.Length; i++)
        {
            string type = types[i].GetText();
            string id = ids[i].GetText();
            ScopeStack.Peek().AddVariable(id, type);
        }
        return base.VisitArg_def(context);
    }
    // Methods to support scope handling
    private void EnterScope(Scope scope) // TODO: Add FTable 
    {
        if (ScopeStack.Count() != 0) // Ensuring that the stack isn't empty as getCurrentScope() would return an error
        {
            foreach (var variable in GetCurrentScope().Variables) // Copying the variables from the parent scope to the child scope
            {
                scope.AddVariable(variable.Key, variable.Value);
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

    public string GetTypeFromStatement(GtlParser.StatementContext ctx)
    {
        string type = "";

        if (ctx.expr() != null)
        {
            type = (string)Visit(ctx.expr());
            return type;
        }
        if (ctx.declaration() != null)
        {
            return ctx.declaration().type().GetText();
        }
        else if (ctx.@if() != null)
        {
            foreach (var stmt in ctx.@if().statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            return type;
        }
        else if (ctx.@if().@else() != null)
        {
            foreach (var stmt in ctx.@if().@else().statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            return type;
        }
        else if (ctx.@if().elseif() != null)
        {
            foreach (var stmt in ctx.@if().elseif().statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            return type;

        }
        else
        {
            throw new NotSupportedException("Dis statement is fcked");
        }
    }

    public string GetTypeFromElseIf(GtlParser.ElseifContext ctx, string tp)
    {
        string type = "";

        if (ctx.elseif() != null)
        {
            foreach (var stmt in ctx.@elseif().statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            if (type != tp)
            {
                throw new NotSupportedException("Type in elseif doesn't match expected return type of function");
            }
            return GetTypeFromElseIf(ctx.elseif(), tp);
        }
        else if (ctx.@else() != null)
        {
            foreach (var stmt in ctx.@else().statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            if (type != tp)
            {
                throw new NotSupportedException("Type in else doesn't match expected return type of function");
            }
            return type;
        }
        else if (ctx.statement() != null)
        {
            foreach (var stmt in ctx.statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            if (type != tp)
            {
                throw new NotSupportedException("Type in elseif statement doesn't match return type of function");
            }
            return type;
        }
        else
        {
            throw new NotSupportedException("Error in else if statement");
        }
    }

    public bool CheckElseInElseIf(GtlParser.ElseifContext ctx)
    {
        bool check = false;

        if (ctx.elseif() != null)
        {
            return CheckElseInElseIf(ctx.elseif());
        }
        else if (ctx.@else() != null)
        {
            check = true;
            return check;
        }
        else
        {
            return check;
        }
    }
}


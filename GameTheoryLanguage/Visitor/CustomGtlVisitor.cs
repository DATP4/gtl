using System.Globalization;

using Antlr4.Runtime.Misc;

public class CustomGtlVisitor : GtlBaseVisitor<object>
{
    static readonly Scope GlobalScope = new Scope();
    static readonly Types TypesCaller = new Types();

    public override object VisitProgram(GtlParser.ProgramContext context)
    {
        Console.WriteLine("Visiting program");
        return base.VisitProgram(context);
    }

    public override object VisitStatement(GtlParser.StatementContext context)
    {
        Console.WriteLine("Visiting statement");
        return base.VisitStatement(context);
    }

    public override object VisitDeclaration(GtlParser.DeclarationContext context)
    {
        Console.WriteLine("Visiting declaration");
        string type = context.type().GetText(); // TODO: Typechecking later
        string variable = context.ID().GetText();
        string value = (string)Visit(context.expr()); // TODO: Typechecking later + handle all sort of expressions
        Console.WriteLine("Val: " + value);
        Console.WriteLine($"Type: {type}, Variable: {variable}, Value: {value}"); // TODO: Add to symbol table
        return base.VisitDeclaration(context);

    }

    public override object VisitFunction([NotNull] GtlParser.FunctionContext context)
    {
        Console.WriteLine("Visiting function");

        string func_name = context.ID().GetText();
        Scope funcTables = GlobalScope.CreateFunScope(func_name);

        string func_type = context.type().GetText();
        funcTables.AddFTableType(func_type, func_name);

        foreach (var stmt in context.statement())
        {
            if (stmt.declaration() != null)
            {
                funcTables.ClearVTable();
                funcTables.AddVTableType(stmt.declaration().type().GetText(), stmt.declaration().ID().GetText());
                continue;
            }
            if (stmt.@if() != null)
            {
                string type = "";
                if (stmt.@if().@else() != null)
                {
                    funcTables.ClearVTable();
                    foreach (var elsestmt in stmt.@if().@else().statement())
                    {
                        type = GetTypeFromStatement(elsestmt);
                    }
                    funcTables.AddVTableType(type, "else");
                }
                if (stmt.@if().@elseif() != null)
                {
                    if (CheckElseInElseIf(stmt.@if().@elseif()))
                    {
                        funcTables.ClearVTable();
                    }
                    string iftype = "";
                    foreach (var ifstmt in stmt.@if().statement())
                    {
                        iftype = GetTypeFromStatement(ifstmt);
                    }
                    funcTables.AddVTableType(GetTypeFromElseIf(stmt.@if().elseif(), iftype), "elseif");

                }

                foreach (var ifstmt in stmt.@if().statement())
                {
                    type = GetTypeFromStatement(ifstmt);
                }

                funcTables.AddVTableType(type, "if");
            }
        }

        Dictionary<string, List<string>> dic = funcTables.GetVtable();

        foreach (var e in dic)
        {
            if (e.Key == func_type)
            {
                List<string> noget = e.Value;
                if (!noget.Any())
                {
                    continue;
                }
            }
            else
            {
                List<string> noget = e.Value;
                if (noget.Any())
                {
                    throw new FieldAccessException("Non return values has been saved and thats a no no");
                }
            }
        }

        return base.VisitFunction(context);
    }

    public override object VisitIf([NotNull] GtlParser.IfContext context)
    {

        return base.VisitIf(context);
    }


    public override object VisitBinaryExpr(GtlParser.BinaryExprContext context) // TODO: Include MOD
    {

        Console.WriteLine("Visiting binary expr");

        _ = context.op.Text;

        // Visit the left and right expressions
        object left = Visit(context.expr(0));
        object right = Visit(context.expr(1));

        // Check if both operands are integers
        if (left.ToString() == "int" && right.ToString() == "int")
        {
            // Perform addition or subtraction based on the operator
            return "int";
        }
        else if (left.ToString() == "real" && right.ToString() == "real")
        {
            // Perform addition or subtraction based on the operator
            return "real";
        }
        else
        {
            // Error: operands are not integers
            Console.WriteLine("Error: Values must be the same type for binary operations.");
        }

        return base.VisitBinaryExpr(context);
    }

    public override string VisitLiteralExpr(GtlParser.LiteralExprContext context)
    {
        Console.WriteLine("Visiting literal expr");
        string val = context.GetText();


        if (int.TryParse(val, out _))
        {
            return "int";
        }
        else if (double.TryParse(val, out _))
        {
            return "real";
        }
        else if (bool.TryParse(val, out _))
        {
            return "bool";
        }
        else
        {
            throw new TypeAccessException("Not a type in our stuff, at visitliteral");
        }

        //return base.VisitLiteralExpr(context);
    }

    public string GetTypeFromStatement(GtlParser.StatementContext ctx)
    {
        string type = "";

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
                throw new NotSupportedException("Fuck up in gettypefromelseif else");
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
                throw new NotSupportedException("Fuck up in gettypefromelseif else");
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
                throw new NotSupportedException("Fuck up in gettypefromelseif stmt");
            }
            return type;
        }
        else
        {
            throw new NotSupportedException("Fuck up in your elseif stmt");
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

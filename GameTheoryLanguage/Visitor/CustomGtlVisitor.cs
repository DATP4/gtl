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
        return null;
    }

    public override object VisitStatement([NotNull] GtlParser.StatementContext context)
    {
        Console.WriteLine("Visiting statement");
        return base.VisitStatement(context);
    }

    public override object VisitDeclaration([NotNull] GtlParser.DeclarationContext context)
    {
        Console.WriteLine("Visiting declaration");
        string type = context.type().GetText(); // TODO: Typechecking later
        string variable = context.ID().GetText();
        object value = Visit(context.expr()); // TODO: Typechecking later + handle all sort of expressions
        Console.WriteLine($"Type: {type}, Variable: {variable}, Value: {value}"); // TODO: Add to symbol table
        ScopeStack.Peek().AddVariable(variable, type);
        return base.VisitDeclaration(context);
    }

    public override object VisitFunction([NotNull] GtlParser.FunctionContext context)
    {
        Console.WriteLine("Visiting function");
        EnterScope(new Scope());
        _ = base.VisitFunction(context);


        Types type_caller = new Types();

        string func_type = context.type().GetText();


        int index = 0;

        foreach (var innerstmt in context.statement())
        {
            _ = Visit(context.statement(index));
            index++;
        }

        string wtf = context.statement(index - 1).declaration().type().GetText();

        Console.WriteLine("dom: " + func_type);
        Console.WriteLine("dom2: " + wtf);

        type_caller.CheckFuncType(func_type, wtf);

        ExitScope();

        return null;
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

        return null;
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
        return null;
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
        return null;
    }

    public override object VisitLiteralExpr(GtlParser.LiteralExprContext context)
    {
        Console.WriteLine("Visiting literal expr");
        string val = context.GetText();

        if (int.TryParse(val, out int ivar))
        {
            return ivar;
        }

        if (double.TryParse(val, CultureInfo.InvariantCulture, out double rvar))
        {
            return rvar;
        }

        if (bool.TryParse(val, out bool bvar))
        {
            return bvar;
        }
        // TODO: Throw error
        return base.VisitLiteralExpr(context);
    }

    public override object VisitBinaryExpr(GtlParser.BinaryExprContext context) // TODO: Include MOD
    {

        Console.WriteLine("Visiting binary expr");
        string op = context.op.Text;
        object result = null;

        // Visit the left and right expressions
        object left = Visit(context.expr(0));
        object right = Visit(context.expr(1));

        // Check if both operands are integers
        if (left is int leftInt && right is int rightInt)
        {
            // Perform addition or subtraction based on the operator
            if (op.Equals("+"))
            {
                result = leftInt + rightInt;
            }
            else if (op.Equals("-"))
            {
                result = leftInt - rightInt;
            }
            else if (op.Equals("*"))
            {
                result = leftInt * rightInt;
            }
            else if (op.Equals("/"))
            {
                result = leftInt / rightInt;
            }
        }
        else if (left is double leftDouble && right is double rightDouble)
        {
            // Perform addition or subtraction based on the operator
            if (op.Equals("+"))
            {
                result = leftDouble + rightDouble;
            }
            else if (op.Equals("-"))
            {
                result = leftDouble - rightDouble;
            }
            else if (op.Equals("*"))
            {
                result = leftDouble * rightDouble;
            }
            else if (op.Equals("/"))
            {
                result = leftDouble / rightDouble;
            }
        }
        else
        {
            // Error: operands are not integers
            Console.WriteLine("Error: Values must be the same type for binary operations.");
        }

        return result;
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
}

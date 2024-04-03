using System.Globalization;

using Antlr4.Runtime.Misc;

public class CustomGtlVisitor : GtlBaseVisitor<object>
{
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
        object value = Visit(context.expr()); // TODO: Typechecking later + handle all sort of expressions
        Console.WriteLine($"Type: {type}, Variable: {variable}, Value: {value}"); // TODO: Add to symbol table
        return base.VisitDeclaration(context);
    }

    public override object VisitFunction([NotNull] GtlParser.FunctionContext context)
    {
        Console.WriteLine("Visiting function");

        Types type_caller = new Types();

        string func_type = context.type().GetText();

        object stmt = null;

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

        return base.VisitFunction(context);
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


}

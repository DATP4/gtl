using Antlr4.Runtime.Misc;

public class TransVisitor : GtlBaseVisitor<object>
{

    readonly List<string> _outputFile = new List<string>();

    GtlDictionary GtlDictionary { get; } = new GtlDictionary();

    public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
    {
        string retString = null!;
        _outputFile.Add("fn main()\n{");
        // Program consists of statements only, so we iterate them
        foreach (var stmt in context.statement())
        {
            retString += Visit(stmt);
            retString += "\n";
        }
        // Everything we visit but this, will return a string. We add it to our output rust file
        if (retString != null)
        {
            _outputFile.Add(retString);
        }
        _outputFile.Add("}");
        GtlCFile writer = new GtlCFile();
        writer.PrintFileToOutput(_outputFile);
        return null!;
    }

    public override object VisitStatement([NotNull] GtlParser.StatementContext context)
    {
        string retString = null!;
        foreach (var children in context.children)
        {
            retString += Visit(children);
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
        return context.ID().GetText();
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
        // Finding the last statement
        foreach (var stmt in context.statement().Reverse())
        {
            lastStmt = stmt;
            break;
        }
        // Appending all content of the if body
        foreach (var stmt in context.statement())
        {
            retIfString += Visit(stmt);
        }
        // As rust can't return declarations, we return the id of the declaration on the next line
        if (lastStmt.declaration() != null)
        {
            retIfString += "\n" + lastStmt.declaration().ID().GetText();
        }

        // Repeat above process for the else if blocks
        foreach (var elseIfBlock in context.elseif())
        {
            retIfString += $"\n{'}'} else if {Visit(elseIfBlock.expr())} {'{'}\n";
            foreach (var stmt in elseIfBlock.statement().Reverse())
            {
                lastStmt = stmt;
                break;
            }
            foreach (var stmt in elseIfBlock.statement())
            {
                retIfString += Visit(stmt);
            }
            if (lastStmt.declaration() != null)
            {
                retIfString += "\n" + lastStmt.declaration().ID().GetText();
            }
        }

        // Repeats the process for the final else block
        retIfString += "\n} else {\n";
        foreach (var stmt in context.@else().statement().Reverse())
        {
            lastStmt = stmt;
            break;
        }
        foreach (var stmt in context.@else().statement())
        {
            retIfString += Visit(stmt);
        }
        if (lastStmt.declaration() != null)
        {
            retIfString += "\n" + lastStmt.declaration().ID().GetText();
        }
        retIfString += "\n};";
        return retIfString;
    }


    public override object VisitDeclaration([NotNull] GtlParser.DeclarationContext context)
    {
        // No need for type declaration in rust, due to our typechecking prior to transpiling.
        return $"let {context.ID().GetText()} = {Visit(context.expr())};";
    }
}

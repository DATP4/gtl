using Antlr4.Runtime.Misc;

public class TransVisitor : GtlBaseVisitor<object>
{

    readonly List<string> _outputFile = new List<string>();

    GtlDictionary GtlDictionary { get; } = new GtlDictionary();

    readonly int _ifCounter = 0;

    public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
    {
        _outputFile.Add("fn main() {println!(\"Hello, world!\");}");
        //_ = base.VisitProgram(context);
        GtlCFile writer = new GtlCFile();
        writer.PrintFileToOutput(_outputFile);
        return null!;
    }

    public override object VisitStatement([NotNull] GtlParser.StatementContext context)
    {
        string retString = null!; // null, if we encounter a statement that is already appended inside other contexts

        // Visits the matching context and appends to string
        if (context.expr() != null)
        {
            retString += Visit(context.expr());
        }
        else if (context.declaration() != null && context.declaration().Parent.Depth() == 2)
        {
            retString += Visit(context.declaration());
        }
        else if (context.function() != null)
        {
            retString += Visit(context.function());
        }
        else if (context.game_variable_declaration() != null)
        {
            retString += Visit(context.game_variable_declaration());
        }
        else if (context.game_functions() != null)
        {
            retString += Visit(context.game_functions());
        }

        // If any content, add to output file.
        if (retString != null)
        {
            _outputFile.Add(retString);
        }

        return null!;
    }

    public override object VisitLiteral([NotNull] GtlParser.LiteralContext context)
    {
        // Booleans needs extra handling
        if (context.boolean_literal() != null)
        {
            return base.VisitLiteral(context);
        }

        // Other literals than booleans matches the transpiled language
        return context.GetText();
    }

    public override object VisitBoolean_literal([NotNull] GtlParser.Boolean_literalContext context)
    {
        // Translates the boolean literal to the transpiled language
        return GtlDictionary.TranslateBoolean(context.GetText());
    }

    public override object VisitIdExpr([NotNull] GtlParser.IdExprContext context)
    {
        return context.ID().GetText();
    }

    public override object VisitBooleanExpr(GtlParser.BooleanExprContext context)
    {
        string left = (string)Visit(context.expr(0));
        string right = (string)Visit(context.expr(1));
        string op = context.op.Text;
        return $"{left} {op} {right}";
    }

    public override object VisitBinaryExpr([NotNull] GtlParser.BinaryExprContext context)
    {
        return context.GetText();
    }


    public override object VisitIfElse([NotNull] GtlParser.IfElseContext context)
    {
        // Translating if elses, but currently only works for declarations in the body
        string retIfString = $"void IfFunc{_ifCounter}(){'{'}";
        retIfString += $"if ({Visit(context.expr())}) {'{'}";
        foreach (var stmt in context.statement())
        {
            if (stmt.declaration() != null)
            { }
            {
                retIfString += (string)Visit(stmt.declaration());
                retIfString += $"return {stmt.declaration().ID().GetText()};";
            }
            retIfString += (string)Visit(stmt);
        }

        foreach (var elseIfBlock in context.elseif())
        {
            retIfString += $"{'}'} else if ({Visit(elseIfBlock.expr())}) {'{'}";
            foreach (var stmt in elseIfBlock.statement())
            {
                if (stmt.declaration() != null)
                {
                    retIfString += (string)Visit(stmt.declaration());
                    retIfString += $"return {stmt.declaration().ID().GetText()};";
                }
                retIfString += (string)Visit(stmt);
            }
        }

        retIfString += "} else {";
        foreach (var stmt in context.@else().statement())
        {
            if (stmt.declaration() != null)
            {
                retIfString += (string)Visit(stmt.declaration());
                retIfString += $"return {stmt.declaration().ID().GetText()};";
            }
            retIfString += (string)Visit(stmt);
        }
        retIfString += "}}\n";
        retIfString += $"IfFunc{_ifCounter}();";
        return retIfString;
    }


    public override object VisitDeclaration([NotNull] GtlParser.DeclarationContext context)
    {
        // Language translates the type (real -> double). The rest is handled in their respective visits
        return $"{GtlDictionary.TranslateType(context.type().GetText())} {context.ID().GetText()} = {Visit(context.expr())};";
    }
}

// // input 
// int x = 1;

// if (x < 10 * 5) then {
//     int z = 10 / 5;
// } else if(x == 10) then {
//     x * 2;
// } else {
//     int z = 10 * 5;
// };

// // expected output
// class program
// {
//     static public void Main(string[] args)
//     {
//         int x = 1;

//         object funcif1()
//         {
//             if (x < 10 * 5)
//             {
//                 int z = 10 / 5;
//                 return z;
//             }
//             else if (x == 10)
//             {
//                 return x * 2;
//             }
//             else
//             {
//                 int z = 10 * 5;
//                 return z;
//             }
//         }
//         funcif1();
//     }
// }


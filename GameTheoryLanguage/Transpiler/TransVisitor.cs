using System.Globalization;
using System.Security.Cryptography;

using Antlr4.Runtime.Misc;

public class TransVisitor : GtlBaseVisitor<object>
{

    readonly List<string> OutputFile = new List<string>();

    GtlDictionary GtlDictionary { get; } = new GtlDictionary();

    Dictionary<string, string> Types { get; } = new Dictionary<string, string>(){
        {"int", "int"},
        {"real", "double"},
        {"bool", "bool"}
    };

    public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
    {

        OutputFile.Add("class program { \n static public void Main(string[] args){");
        _ = base.VisitProgram(context);
        OutputFile.Add("}\n}");
        GtlCFile writer = new GtlCFile();
        writer.PrintFileToOutput(OutputFile);
        return null!;
    }

    public override object VisitStatement([NotNull] GtlParser.StatementContext context)
    {
        return base.VisitStatement(context);
    }

    public override object VisitLiteral([NotNull] GtlParser.LiteralContext context)
    {
        if (context.boolean_literal() != null)
        {
            return base.VisitLiteral(context);
        }

        return context.GetText();
    }

    public override object VisitBoolean_literal([NotNull] GtlParser.Boolean_literalContext context)
    {
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


    public override object VisitFunction([NotNull] GtlParser.FunctionContext context)
    {
        //List<string> OutputFile = new List<string>();
        OutputFile.Add($"{context.type().GetText()} {context.ID().GetText()}(");

        GtlParser.TypeContext[] types = context.arg_def().type();
        Antlr4.Runtime.Tree.ITerminalNode[] ids = context.arg_def().ID();
        if (types.Length != 0)
        {
            OutputFile.Add($"{GtlDictionary.TranslateType(types[0].GetText())} {ids[0].GetText()} ");
            for (int i = 1; i < types.Length; i++)
            {
                string type = GtlDictionary.TranslateType(types[i].GetText());
                string id = ids[i].GetText();
                OutputFile.Add($",{type} {id} ");
            }
        }

        OutputFile.Add("){");

        GtlParser.StatementContext returnStmt = null!;


        foreach (var stmt in context.statement().Reverse())
        {
            returnStmt = stmt;
            break;
        }

        foreach (var stmt in context.statement())
        {
            if (stmt.Equals(returnStmt))
            {
                if (stmt.expr() != null)
                {
                    OutputFile.Add($"return {Visit(stmt.expr())};"); //TODO: make sure expressions work
                }
                else if (stmt.declaration() != null)
                {
                    OutputFile.Add((string)Visit(stmt.declaration()));
                    OutputFile.Add($"return {stmt.declaration().ID().GetText()};");
                }
                else if (stmt.@if().@else() != null)
                {
                    OutputFile.Add($"if ({Visit(stmt.@if().expr())}) {'{'}");
                    GtlParser.StatementContext tempRetStmt = null!;
                    foreach (var ifretstmt in stmt.@if().statement().Reverse())
                    {
                        tempRetStmt = ifretstmt;
                        break;
                    }

                    foreach (var ifstmt in stmt.@if().statement())
                    {
                        if (ifstmt.Equals(tempRetStmt))
                        {
                            if (ifstmt.declaration() != null)
                            {
                                OutputFile.Add((string)Visit(ifstmt.declaration()));
                                OutputFile.Add($"return {ifstmt.declaration().ID().GetText()};");
                            }
                            else
                            {
                                OutputFile.Add($"return {Visit(ifstmt)};");
                            }
                        }
                        else
                        {
                            OutputFile.Add($"{Visit(ifstmt)}");
                        }
                    }
                    OutputFile.Add("} else {");

                    foreach (var ifretstmt in stmt.@if().@else().statement().Reverse())
                    {
                        tempRetStmt = ifretstmt;
                        break;
                    }

                    foreach (var ifstmt in stmt.@if().@else().statement())
                    {
                        if (ifstmt.Equals(tempRetStmt))
                        {
                            if (ifstmt.declaration() != null)
                            {
                                OutputFile.Add((string)Visit(ifstmt.declaration()));
                                OutputFile.Add($"return {ifstmt.declaration().ID().GetText()};");
                            }
                            else
                            {
                                OutputFile.Add($"return {Visit(ifstmt)};");
                            }
                        }
                        else
                        {
                            OutputFile.Add($"{Visit(ifstmt)};");
                        }
                    }
                    OutputFile.Add("}");
                }
            }
            else
            {
                OutputFile.Add($"{Visit(stmt)}");
            }
        }

        OutputFile.Add("}");
        return base.VisitFunction(context);
    }

    public override object VisitIf([NotNull] GtlParser.IfContext context)
    {
        string retIfString = $"if ({Visit(context.expr())}) {'{'}";
        foreach (var stmt in context.statement())
        {
            retIfString += (string)Visit(stmt);
        }
        retIfString += "} else {";
        foreach (var stmt in context.@else().statement())
        {
            retIfString += (string)Visit(stmt);
        }
        retIfString += "}";
        return retIfString;
    }


    public override object VisitDeclaration([NotNull] GtlParser.DeclarationContext context)
    {
        return $"{GtlDictionary.TranslateType(context.type().GetText())} {context.ID().GetText()} = {Visit(context.expr())};";
    }
}

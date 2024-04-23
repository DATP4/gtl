using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
namespace GameTheoryLanguageTests;

[TestClass]
public class TranspilerTests
{
    [TestMethod]
    public void AssignmentTest()
    {
        string input1 = "int x = 3;";
        string output1 = "fn main(){let x = 3;}";
        AssertTrue(input1, output1);
    }
    private void AssertTrue(string input, string expectedOutput)
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(input));
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream)!;

        parser.ErrorHandler = new ErrorStrategy();
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener());

        var parseTree = parser.program();
        CustomGtlVisitor visitor = new CustomGtlVisitor();
        _ = visitor.Visit(parseTree);
        TestVisitor transvisitor = new TestVisitor();
        _ = transvisitor.Visit(parseTree);
        string workingDirectory = Environment.CurrentDirectory;
        string gtlPath = Directory.GetParent(workingDirectory)!.Parent!.Parent!.Parent!.FullName;
        string path = gtlPath + "/GameTheoryLanguage/output/src/main.rs";
        string output = File.ReadAllText(path);
        output = output.Replace("\r", "");
        output = output.Replace("\n", "");
        Console.WriteLine("Expected: " + expectedOutput);
        Console.WriteLine("Recieved: " + output);
        Assert.IsTrue(output.Equals(expectedOutput));
    }
}
class TestVisitor : TransVisitor
{
    public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
    {
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
        TestGtlCFile writer = new TestGtlCFile();
        writer.PrintFileToOutput(Outputfile);
        return null!;
    }
}
class TestGtlCFile
{
    public void PrintFileToOutput(List<string> file)
    {
        string workingDirectory = Environment.CurrentDirectory;
        string gtlPath = Directory.GetParent(workingDirectory)!.Parent!.Parent!.Parent!.FullName;
        string path = gtlPath + "/GameTheoryLanguage/output/src/main.rs";
        List<string> authors = file;
        File.WriteAllText(path, "");
        File.WriteAllLines(path, authors);
        //File.Delete(filepath); 
    }
}

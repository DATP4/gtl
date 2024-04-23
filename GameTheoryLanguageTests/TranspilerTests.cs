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
        string input2 = "bool x = TRUE;";
        string input3 = "real x = 3.0;";

        string output1 = "let x = 3;";
        string output2 = "let x = true;";
        string output3 = "let x = 3.0;";

        AssertTrue(input1, output1);
        AssertTrue(input2, output2);
        AssertTrue(input3, output3);
    }
    [TestMethod]
    public void BooleanExpressionTest()
    {
        string input1 = "bool test = TRUE && FALSE;";
        string input2 = "bool test = 1 > 2;";
        string input3 = "bool test = 1 != 0;";
        string input4 = "bool test = 1 <= 2;";
        string input5 = "bool test = (TRUE && TRUE) || ((TRUE && TRUE) == TRUE);";

        string output1 = "let test = true && false;";
        string output2 = "let test = 1 > 2;";
        string output3 = "let test = 1 != 0;";
        string output4 = "let test = 1 <= 2;";
        string output5 = "let test = (true && true) || ((true && true) == true);";

        AssertTrue(input1, output1);
        AssertTrue(input2, output2);
        AssertTrue(input3, output3);
        AssertTrue(input4, output4);
        AssertTrue(input5, output5);
    }
    [TestMethod]
    public void BinaryExpressionTest()
    {
        string input1 = "int test = 1 + 1 * 7;";
        string input2 = "real test = 1.5 + 1.6 / 10.0;";
        string input3 = "int test = (-5 + ( 4 + 3 * ( 4 MOD 5 ) / 1 + 5 ) - 3);";
        string input4 = "5 MOD 3;";
        string output1 = "let test = 1 + 1 * 7;";
        string output2 = "let test = 1.5 + 1.6 / 10.0;";
        string output3 = "let test = (-5 + (4 + 3 * (4 % 5) / 1 + 5) - 3);";
        string output4 = "5 % 3;";
        AssertTrue(input1, output1);
        AssertTrue(input2, output2);
        AssertTrue(input3, output3);
        AssertTrue(input4, output4);
    }
    [TestMethod]
    public void IfElseTest()
    {
        string input1 = "if (TRUE) then {int x = 4; x;} else {int y = 5; y;};";
        string input2 = "if (TRUE) then {int x = 4; x;} else if (FALSE) then {int y = 5; y;} else {int z = 6; z;};";
        string input3 = "if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 4;} else {int z = 4;};";

        string output1 = "if true {let x = 4;x} else {let y = 5;y};";
        string output2 = "if true {let x = 4;x} else if false {let y = 5;y} else {let z = 6;z};";
        string output3 = "if true {let x = 4;x} else if false {let y = 4;y} else {let z = 4;z};";

        AssertTrue(input1, output1);
        AssertTrue(input2, output2);
        AssertTrue(input3, output3);
    }
    /*
    [TestMethod]
    public void TestFunctionDeclaration()
    {
        string input1 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;}";
        string input2 = "intFunction : (int x) -> int {if (TRUE) then {5;} else {6;};}";

        string output1 = "fn intFunction : (int x) -> int {let y = x + 10;x - 5}";
        string output2 = "fn intFunction : (int x) -> int {if true {5} else {6};";

        AssertTrue(input1, output1);
        AssertTrue(input2, output2);
    }
    */
    /*
    [TestMethod]
    public void TestFunctionCall()
    {
        string input1 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;} intFunction(5);";
        string input2 = "int a = 10; intFunction1 : (int x) -> int {int y = x + a * 5; x - 5;} intFunction1(5);";
        string input3 = "intFunction : (int x) -> int {intFunction2 : (int x) -> int {x + 5;} intFunction2(x);} intFunction(5);";

        string output1 = "fn intFunction : (int x) -> int {let y = x + 10 * 5;x - 5}intFunction(5);";
        string output2 = "let a = 10;fn intFunction : (int x) -> int {let y = x + a * 5; x - 5}intFunction(5);";
        string output3 = "fn intFunction : (int x) -> int {fn intFunction2 : (int x) -> int {x + 5}intFunction2(x)}intFunction(5);";

        AssertTrue(input1, output1);
        AssertTrue(input2, output2);
        AssertTrue(input3, output3);
    }
    */
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
        output = output.Remove(0, 10);
        output = output.Remove(output.Length - 1, 1);
        Console.WriteLine("Expected: " + expectedOutput);
        Console.WriteLine("Recieved: " + output);
        Assert.IsTrue(output.Equals(expectedOutput));
    }
}
class TestVisitor : TransVisitor
{
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
        TestGtlCFile writer = new TestGtlCFile();
        writer.PrintFileToOutput(Outputfile);
        ExitScope();
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

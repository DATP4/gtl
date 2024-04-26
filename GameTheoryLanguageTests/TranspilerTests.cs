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
        string input1 = "if (TRUE) then {int x = 4;} else {int y = 5;};";
        string input2 = "if (TRUE) then {int x = 4; x;} else if (FALSE) then {int y = 5; y;} else {int z = 6; z;};";
        string input3 = "if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 4;} else {int z = 4;};";
        string input4 = "int x = if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 3;} else {int z = 2;};";

        string output1 = "if true {let x = 4;x} else {let y = 5;y};";
        string output2 = "if true {let x = 4;x} else if false {let y = 5;y} else {let z = 6;z};";
        string output3 = "if true {let x = 4;x} else if false {let y = 4;y} else {let z = 4;z};";
        string output4 = "let x = if true {let x = 4;x} else if false {let y = 3;y} else {let z = 2;z};";

        AssertTrue(input1, output1); // Regular if else statement with assignment to expression return value
        AssertTrue(input2, output2); // If else if else with return value
        AssertTrue(input3, output3); // If else if else with with assignment to expression return value
        AssertTrue(input4, output4); // Variable assignment to if else if else statement
    }

    [TestMethod]
    public void TestFunctionDeclaration()
    {
        string input1 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;}";
        string input2 = "intFunction : (int x) -> int {if (TRUE) then {5;} else {6;};}";
        string input3 = "intFunction : (int x) -> int {if (TRUE) then {x;intFunctionNested : (int z) -> int {1;}} else {6;};}";
        string input4 = "int x = 5; intFunction : (int z) -> int {int q = 3; z; intFunctionNested : (int y) -> int {x + 1;}}";


        string output1 = "fn intFunction(x: &i32) -> i32 {let y = *x + 10 * 5;*x - 5}";
        string output2 = "fn intFunction(x: &i32) -> i32 {if true {5} else {6}}";
        string output3 = "fn intFunction(x: &i32) -> i32 {if true {fn intFunctionNested(z: &i32) -> i32 {1}*x} else {6}}";
        string output4 = "let x = 5;fn intFunction(z: &i32) -> i32 {let x = 5;let q = 3;fn intFunctionNested(y: &i32) -> i32 {let x = 5;let q = 3;x + 1}*z}";

        // Also tests conversion to call by reference
        AssertTrue(input1, output1); // Regular function test
        AssertTrue(input2, output2); // If else in function
        AssertTrue(input3, output3); // Nested function
        AssertTrue(input4, output4); // Nested function with outer scope variables inclusion
    }

    [TestMethod]
    public void TestFunctionCall()
    {
        string input1 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;} intFunction(5);";
        string input2 = "int a = 10; intFunction : (int x) -> int {int y = x + a * 5; x - 5;} intFunction(5);";
        string input3 = "intFunction : (int x) -> int {intFunctionNested : (int x) -> int {x + 1;} intFunctionNested(x);} intFunction(5);";
        string input4 = "intFunction : (int x) -> int {int y = x + 10 * 5;} int x = intFunction(5); ";

        string output1 = "fn intFunction(x: &i32) -> i32 {let y = *x + 10 * 5;*x - 5}intFunction(&5);";
        string output2 = "let a = 10;fn intFunction(x: &i32) -> i32 {let a = 10;let y = *x + a * 5;*x - 5}intFunction(&5);";
        string output3 = "fn intFunction(x: &i32) -> i32 {fn intFunctionNested(x: &i32) -> i32 {*x + 1}intFunctionNested(&*x)}intFunction(&5);";
        string output4 = "fn intFunction(x: &i32) -> i32 {let y = *x + 10 * 5;y}let x = intFunction(&5);";

        // Also tests conversion to call by reference
        AssertTrue(input1, output1); // Regular function call
        AssertTrue(input2, output2); // Function call with outer scope variable
        AssertTrue(input3, output3); // Function call with nested function
        AssertTrue(input4, output4); // Function call with assignment
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
        output = output.Remove(0, 10);
        output = output.Remove(output.Length - 1, 1);
        Console.WriteLine("Expected: " + expectedOutput);
        Console.WriteLine("Received: " + output);
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

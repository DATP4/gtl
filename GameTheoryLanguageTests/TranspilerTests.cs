using System.Security.Cryptography;

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
namespace GameTheoryLanguageTests;


[TestClass]
public class TranspilerTests
{
    public string WholeGameString = "Moves = [cooperate, defect]; Action testaction = () then cooperate; Strategy teststrategy = [testaction]; Strategyspace teststratspace = [(cooperate, cooperate)]; Payoff testpayoff = [\"p1\" -> [1]]; Players testplayers = [\"p1\" chooses teststrategy]; Game testgame = (teststratspace, testplayers, testpayoff); run(testgame, 4);";
    [TestMethod]
    public void AssignmentTest()
    {
        string input1 = "int x = 3;" + WholeGameString;
        string input2 = "int x = 3.0;" + WholeGameString;
        string input3 = "bool x = TRUE;" + WholeGameString;
        string input4 = "real x = 3.0;" + WholeGameString;

        string output1 = "let x = 3;";
        string output2 = "let x = 3.0;";
        string output3 = "let x = true;";
        string output4 = "let x = 3.0;";

        AssertIntegrationTrue(input1, output1);
        AssertIntegrationFalse(input2);
        AssertTrue(input2, output2);
        AssertTrue(input3, output3);
        AssertTrue(input4, output4);
    }
    [TestMethod]
    public void BooleanExpressionTest()
    {
        string input1 = "bool test = TRUE && FALSE;" + WholeGameString;
        string input2 = "bool test = 1 > 2;" + WholeGameString;
        string input3 = "bool test = 1 != 0;" + WholeGameString;
        string input4 = "bool test = 1 <= 2;" + WholeGameString;
        string input5 = "real test = (TRUE && TRUE) || ((TRUE && TRUE) == TRUE);" + WholeGameString;
        string input6 = "bool test = (TRUE && TRUE) || ((TRUE && TRUE) == TRUE);" + WholeGameString;

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
        AssertIntegrationFalse(input5);
        AssertIntegrationTrue(input6, output5);
    }
    [TestMethod]
    public void BinaryExpressionTest()
    {
        string input1 = "int test = 1 + 1 * 7;" + WholeGameString;
        string input2 = "real test = 1.5 + 1.6 / 10.0;" + WholeGameString;
        string input3 = "int test = (-5 + ( 4 + 3 * ( 4 MOD 5 ) / 1 + 5 ) - 3);" + WholeGameString;
        string input4 = "int test = 5 MOD 3;" + WholeGameString;
        string input5 = "int test = 1 + 1 * TRUE;" + WholeGameString;

        string output1 = "let test = 1 + 1 * 7;";
        string output2 = "let test = 1.5 + 1.6 / 10.0;";
        string output3 = "let test = (-5 + (4 + 3 * (4 % 5) / 1 + 5) - 3);";
        string output4 = "let test = 5 % 3;";
        string output5 = "let test = 1 + 1 * true;";

        AssertTrue(input1, output1);
        AssertIntegrationTrue(input2, output2);
        AssertTrue(input2, output2);
        AssertTrue(input3, output3);
        AssertTrue(input4, output4);
        AssertTrue(input5, output5);
        AssertIntegrationFalse(input5);
    }
    [TestMethod]
    public void IfElseTest()
    {
        string input1 = "int test = if (TRUE) then {int x = 4;x} else {int y = 5;y};" + WholeGameString;
        string input2 = "int test = if (TRUE) then {int x = 4;x} else if (FALSE) then {int y = 5;y} else {int z = 6;z};" + WholeGameString;
        string input3 = "int test = if (TRUE) then {int x = 4;x} else if (FALSE) then {int y = 4;y} else {int z = 4;z};" + WholeGameString;
        string input4 = "int test = if (TRUE) then {int x = 4;x} else if (FALSE) then {int y = 3;y} else {int z = 2;z};" + WholeGameString;
        string input5 = "real test = if (TRUE) then {int x = 4;x} else if (FALSE) then {int y = 3;y} else {int z = 2;z};" + WholeGameString;

        string output1 = "let test = if true {let x = 4;x} else {let y = 5;y};";
        string output2 = "let test = if true {let x = 4;x} else if false {let y = 5;y} else {let z = 6;z};";
        string output3 = "let test = if true {let x = 4;x} else if false {let y = 4;y} else {let z = 4;z};";
        string output4_5 = "let test = if true {let x = 4;x} else if false {let y = 3;y} else {let z = 2;z};";

        AssertTrue(input1, output1); // Regular if else statement with assignment to expression return value
        AssertTrue(input2, output2); // If else if else with return value
        AssertTrue(input3, output3); // If else if else with with assignment to expression return value
        AssertTrue(input4, output4_5); // Variable assignment to if else if else statement
        AssertIntegrationTrue(input4, output4_5); // Same as the above, but passess the type checker too
        AssertIntegrationFalse(input5); // If else statement return int to real

    }

    [TestMethod]
    public void TestFunctionDeclaration()
    {
        string input1 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5}" + WholeGameString;
        string input2 = "intFunction : (int x) -> int {if (TRUE) then {5} else {6}}" + WholeGameString;
        string input3 = "intFunction : (int x) -> int {if (TRUE) then {5.1} else {6}}" + WholeGameString;

        string output1 = "fn intFunction(x: &i32) -> i32 {let y = *x + 10 * 5;*x - 5}";
        string output2 = "fn intFunction(x: &i32) -> i32 {if true {5} else {6}}";

        // Also tests conversion to call by reference
        AssertTrue(input1, output1); // Regular function test
        AssertTrue(input2, output2); // If else in function
        AssertIntegrationFalse(input3);
    }

    [TestMethod]
    public void TestFunctionCall()
    {
        string input1 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5} int test = intFunction(5);" + WholeGameString;
        string input2 = "int a = 10; intFunction : (int x) -> int {int y = x + a * 5; x - 5} int test = intFunction(5);" + WholeGameString;
        string input3 = "intFunction : (int x) -> int {intFunctionNested : (int z) -> int {z + 1} intFunctionNested(x)} int test = intFunction(5);" + WholeGameString;
        string input4 = "intFunction : (int x) -> int {int y = x + 10 * 5; y} int x = intFunction(5);" + WholeGameString;
        string input5 = "intFunction : (int x, int z) -> int {z + x} int x = 1; int test = intFunction(x, x + 4);" + WholeGameString;
        string input6 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5} int test = intFunction(5.2);" + WholeGameString;


        string output1 = "fn intFunction(x: &i32) -> i32 {let y = *x + 10 * 5;*x - 5}let test = intFunction(&(5));";
        string output2 = "let a = 10;fn intFunction(x: &i32) -> i32 {let a = 10;let y = *x + a * 5;*x - 5}let test = intFunction(&(5));";
        string output3 = "fn intFunction(x: &i32) -> i32 {fn intFunctionNested(z: &i32) -> i32 {*z + 1}intFunctionNested(&(*x))}let test = intFunction(&(5));";
        string output4 = "fn intFunction(x: &i32) -> i32 {let y = *x + 10 * 5;y}let x = intFunction(&(5));";
        string output5 = "fn intFunction(x: &i32, z: &i32) -> i32 {*z + *x}let x = 1;let test = intFunction(&(x), &(x + 4));";
        string output6 = "fn intFunction(x: &i32) -> i32 {let y = *x + 10 * 5;*x - 5}let test = intFunction(&(5.2));";


        // Also tests conversion to call by reference
        AssertTrue(input1, output1); // Regular function call
        AssertTrue(input2, output2); // Function call with outer scope variable
        AssertTrue(input3, output3); // Function call with nested function
        AssertTrue(input4, output4); // Function call with assignment
        AssertTrue(input5, output5); // Function call with multiple arguments and expression to pointer
        AssertIntegrationTrue(input5, output5);
        AssertTrue(input6, output6);
        AssertIntegrationFalse(input6);
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
        TestVisitor transvisitor = new TestVisitor();
        _ = transvisitor.Visit(parseTree);
        string workingDirectory = Environment.CurrentDirectory;
        string gtlPath = Directory.GetParent(workingDirectory)!.Parent!.Parent!.Parent!.FullName;
        string path = gtlPath + "/GameTheoryLanguage/output/src/main.rs";
        string output = File.ReadAllText(path);
        output = output.Replace("\r", "");
        output = output.Replace("\n", "");
        output = output.Remove(0, 143);
        output = output.Remove(output.Length - 1, 1);
        output = output.Remove(expectedOutput.Length);
        Console.WriteLine("Expected: " + expectedOutput);
        Console.WriteLine("Received: " + output);
        Assert.IsTrue(output.Equals(expectedOutput));
    }

    private void AssertIntegrationTrue(string input, string expectedOutput)
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(input));
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new ErrorListener());
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
        output = output.Remove(0, 143);
        output = output.Remove(output.Length - 1, 1);
        output = output.Remove(expectedOutput.Length);
        Console.WriteLine("Expected: " + expectedOutput);
        Console.WriteLine("Received: " + output);
        Assert.IsTrue(output.Equals(expectedOutput));
    }
    private void AssertIntegrationFalse(string input)
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(input));
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new ErrorListener());
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream)!;

        parser.ErrorHandler = new ErrorStrategy();
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener());

        var parseTree = parser.program();
        CustomGtlVisitor visitor = new CustomGtlVisitor();
        TestVisitor transvisitor = new TestVisitor();
        try
        {
            // If both visitors succeeded without throwing an exception,
            // it indicates an integration fault.
            _ = visitor.Visit(parseTree);
            _ = transvisitor.Visit(parseTree);
        }
        catch (Exception ex)
        {
            // If any exception is caught, it indicates a failure, which we expect for this test
            Console.WriteLine("Exception caught: " + ex.Message);
            return;
        }
        Assert.Fail("Integration fault: Expected an exception during visitation.");
    }
}


class TestVisitor : TransVisitor
{
    public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
    {
        EnterScope(new Scope());
        string retString = null!;
        Outputfile.Add("mod library;");
        Outputfile.Add("use library::{Action, BoolExpression, Condition, Game, GameState, Moves, PayoffMatrix, Players, Strategy, StrategySpace};");
        Outputfile.Add("fn main()\n{");
        // Program consists of statements only, so we iterate them
        foreach (var stmt in context.statement())
        {
            retString += Visit(stmt);
            retString += "\n";
        }
        foreach (var game_stmt in context.game_variable_declaration())
        {
            retString += Visit(game_stmt);
            retString += "\n";
        }
        foreach (var game_fun in context.game_functions())
        {
            retString += Visit(game_fun);
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
    public override object VisitGame_functions([NotNull] GtlParser.Game_functionsContext context)
    {
        string returnString = "let finishedgame = ";
        returnString += "Game::run(&mut ";
        returnString += $"{context.ID().GetText()}, ";
        string val = (string)Visit(context.expr());
        returnString += "&mut " + val + ");";
        return returnString;
    }
    public override void WriteToMoves(List<string> moves)
    {
        TestGtlCFile writer = new TestGtlCFile();
        writer.PrintMovesToFile(moves);
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
    public void PrintMovesToFile(List<string> moves)
    {
        string workingDirectory = Environment.CurrentDirectory;
        string gtlPath = Directory.GetParent(workingDirectory)!.Parent!.Parent!.FullName;
        string filepath = gtlPath + "/EndToEndTests/src/library/movs.rs";
        List<string> authors = moves;
        File.WriteAllText(filepath, "");
        File.WriteAllLines(filepath, authors);
    }
}

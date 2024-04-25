using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace GameTheoryLanguageTests;


[TestClass]
public class EndToEndTests

{
    public static int Testcounter = 1;
    public static string Assertstring = "";
    public static string Testtype = "";
    public static string Previoustesttype = "";
    [TestMethod]
    public void SetupTests()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string gtlPath = Directory.GetParent(workingDirectory)!.Parent!.Parent!.FullName;
        string path = gtlPath + "/EndToEndTests/src/main.rs";
        File.WriteAllText(path, "#[cfg(test)]\nmod tests {\n");
        Createtests();
        string currentText = File.ReadAllText(path);
        File.WriteAllText(path, currentText + "}");
    }
    private void Createtests()
    {
        DeclarationTest();
        BinaryExpressionTest();
        BooleanExpressionTest();
        LogicalNotTest();
        UnaryExpressionTest();
        IfElseTest();
        FunctionTest();
        ActionDeclarationTest();
        StrategyDeclarationTest();
        PlayerDeclarationTest();
        PayoffDeclarationTest();
        StrategySpaceDeclarationTest();
        GameTupleTest();
    }
    private void DeclarationTest()
    {
        Createtest("int x = 5;", "assert_eq!(x, 5)", "declaration");
        Createtest("real x = 5.0;", "assert_eq!(x, 5.0)", "declaration");
        Createtest("bool x = TRUE;", "assert_eq!(x, true)", "declaration");
    }
    private void BinaryExpressionTest()
    {
        Createtest("int test = 1 + 1 * 7;", "assert_eq!(test, 8)", "binary_expression");
        Createtest("real test = 1.5 + 1.6 / 10.0;", "assert_eq!(test, 1.66)", "binary_expression");
        Createtest("int test = -5 + ( 4 + 3 * ( 4 MOD 5 ) / 1 + 5 ) - 3;", "assert_eq!(test, 13)", "binary_expression");
        Createtest("int test = 5 MOD 3;", "assert_eq!(test, 2)", "binary_expression");
    }
    private void BooleanExpressionTest()
    {
        Createtest("bool test1 = TRUE && FALSE;", "assert_eq!(test1, false)", "boolean_expression");
        Createtest("bool test1 = 1 > 2;", "assert_eq!(test1, false)", "boolean_expression");
        Createtest("bool test1 = 1 != 0;", "assert_eq!(test1, true)", "boolean_expression");
        Createtest("bool test1 = 1 <= 2;", "assert_eq!(test1, true)", "boolean_expression");
        Createtest("bool test1 = (TRUE && TRUE) || ((TRUE && TRUE) == TRUE);", "assert_eq!(test1, true)", "boolean_expression");
    }
    private void LogicalNotTest()
    {
        Createtest("bool test1 = !(TRUE && FALSE);", "assert_eq!(test1, true)", "logical_not");
        Createtest("bool test1 = !(1 > 2);", "assert_eq!(test1, true)", "logical_not");
        Createtest("bool test1 = !(1 != 0);", "assert_eq!(test1, false)", "logical_not");
        Createtest("bool test1 = !(1 <= 2);", "assert_eq!(test1, false)", "logical_not");
        Createtest("bool test1 = !(!(TRUE && TRUE) || ((TRUE && TRUE) == TRUE));", "assert_eq!(test1, false)", "logical_not");
    }
    private void UnaryExpressionTest()
    {
        Createtest("int x = -5;", "assert_eq!(x, -5)", "unary_expression");
        Createtest("real x = -5.0;", "assert_eq!(x, -5.0)", "unary_expression");
        Createtest("int x = -5 - -5;", "assert_eq!(x, 0)", "unary_expression");
        Createtest("int x = -5 + -5;", "assert_eq!(x, -10)", "unary_expression");
        Createtest("int x = -5; int y = -x;", "assert_eq!(y, 5)", "unary_expression");
    }
    private void IfElseTest()
    {
        Createtest("int y = if (TRUE) then {int x = 4;} else {int x = 5;};", "assert_eq!(y, 4)", "if_else");
        Createtest("int y = if (FALSE) then {int x = 4;} else {int x = 5;};", "assert_eq!(y, 5)", "if_else");
        Createtest("int y = if (TRUE) then {int x = 4;} else if (TRUE) then {int x = 5;} else {int x = 6;};", "assert_eq!(y, 4)", "if_else");
        Createtest("int y = if (FALSE) then {int x = 4;} else if (TRUE) then {int x = 5;} else {int x = 6;};", "assert_eq!(y, 5)", "if_else");
        Createtest("int y = if (FALSE) then {int x = 4;} else if (FALSE) then {int x = 5;} else {int x = 6;};", "assert_eq!(y, 6)", "if_else");
    }
    private void FunctionTest()
    {
        Createtest("int_function : (int x) -> int {int y = x + 10 * 5; y - 5;} int x = int_function(5);", "assert_eq!(x, 50)", "function");
        Createtest("int a = 10; int_function1 : (int x) -> int {int y = x + a * 5; y - 5;} int x = int_function1(5);", "assert_eq!(x, 50)", "function");
        Createtest("int_function : (int x) -> int {int_function2 : (int z) -> int {z + 5;} int y = int_function2(x);} int x = int_function(5);", "assert_eq!(x, 10)", "function");
        Createtest("int a = 10 + 5; int b = 5 + 13; int c = a * b; int_function : (int x) -> int {c + x;} int x = int_function(10);", "assert_eq!(x, 280)", "function");
    }
    private void ActionDeclarationTest()
    {

    }
    private void StrategyDeclarationTest()
    {

    }
    private void PlayerDeclarationTest()
    {

    }
    private void PayoffDeclarationTest()
    {

    }
    private void StrategySpaceDeclarationTest()
    {

    }
    private void GameTupleTest()
    {

    }
    private void Createtest(string input, string output, string testtype)
    {
        Assertstring = output;
        Testtype = testtype;
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
    }
    class TestVisitor : TransVisitor
    {
        public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
        {
            EnterScope(new Scope());
            string retString = null!;
            if (!Testtype.Equals(Previoustesttype))
            {
                Testcounter = 1;
            }
            Outputfile.Add($"#[test]\nfn {Testtype}_test_{Testcounter}()" + "{\n");
            Testcounter += 1;
            Previoustesttype = Testtype;
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
            Outputfile.Add(Assertstring);
            Outputfile.Add("\n}\n");
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
            string gtlPath = Directory.GetParent(workingDirectory)!.Parent!.Parent!.FullName;
            string path = gtlPath + "/EndToEndTests/src/main.rs";
            List<string> authors = file;
            string currentText = File.ReadAllText(path);
            foreach (string line in authors)
            {
                currentText += line;
            }
            File.WriteAllText(path, currentText);
            //File.Delete(filepath); 
        }
    }
}

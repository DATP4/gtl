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
        Createtest("real x = 5.0;", "assert_eq!(x, 5.0);", "declaration");
        Createtest("bool x = TRUE;", "assert_eq!(x, true);", "declaration");
    }
    private void BinaryExpressionTest()
    {

    }
    private void BooleanExpressionTest()
    {

    }
    private void LogicalNotTest()
    {

    }
    private void UnaryExpressionTest()
    {

    }
    private void IfElseTest()
    {

    }
    private void FunctionTest()
    {

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
            Outputfile.Add($"#[test]\nfn {Testtype}_test{Testcounter}()" + "{\n");
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

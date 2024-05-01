using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
namespace GameTheoryLanguageTests;


[TestClass]
public class EndToEndTests

{
    public static int Testcounter = 1;
    public static string Assertstring = "";
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
    }
    private void DeclarationTest()
    {
        Createtest("int x = 5;", "assert_eq!(x, 5)");
    }
    private void Createtest(string input, string output)
    {
        Assertstring = output;
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
            Outputfile.Add($"#[test]\nfn test{Testcounter}()" + "{\n");
            Testcounter += 1;
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
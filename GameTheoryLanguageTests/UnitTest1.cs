namespace GameTheoryLanguageTests;

[TestClass]
public class UnitTest1

{
    public string parse (string path) {
    // Read input from a file
    // Update the file path accordingly
    string input = File.ReadAllText(path);

    // Create a stream from the input string
    ICharStream stream = CharStreams.fromString(input);

    // Create a lexer
    ITokenSource lexer = new GtlLexer(stream);

    // Create a token stream
    // Change this to ITokenStream
    CommonTokenStream tokens = new(lexer);

    // Create a parser
    GtlParser parser = new(tokens);
    IParseTree tree = parser.program();

    return tree.ToStringTree(parser);
    }

    [TestMethod]
    public void TestMethod1() {
        string output = parse("./IfElseTest.gtl");
        
    }
}

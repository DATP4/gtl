using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

static void MyParseMethod()
{
    // Read input from a file
    string filePath = "parser/input.gtl"; // Update the file path accordingly
    string input = File.ReadAllText(filePath);

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

    Console.WriteLine(tree.ToStringTree(parser));
}

MyParseMethod();

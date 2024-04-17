using Antlr4.Runtime;

static void MyParseMethod()
{
    // Read input from a file
    string filePath = "Parser/input.gtl"; // Update the file path accordingly
    string input = File.ReadAllText(filePath);

    // Create a stream from the input string
    ICharStream stream = CharStreams.fromString(input);

    // Create a lexer
    GtlLexer lexer = new GtlLexer(stream);

    // Create a token stream
    CommonTokenStream tokens = new CommonTokenStream(lexer);

    // Create a parser
    GtlParser parser = new GtlParser(tokens)!;

    parser.ErrorHandler = new ErrorStrategy();
    parser.RemoveErrorListeners();
    parser.AddErrorListener(new ErrorListener());

    GtlParser.ProgramContext tree = parser.program(); // Assign parse tree to a variable

    // Create a visitor
    CustomGtlVisitor visitor = new CustomGtlVisitor();

    _ = visitor.VisitProgram(tree);

    TransVisitor transpiler = new TransVisitor();

    _ = transpiler.VisitProgram(tree);


    // Optionally print the parse tree for debugging
    Console.WriteLine(tree.ToStringTree(parser));
}

MyParseMethod();

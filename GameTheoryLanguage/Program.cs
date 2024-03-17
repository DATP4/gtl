using Antlr4.Runtime;

static void MyParseMethod()
{
    // Read input from a file
    string filePath = "GameTheoryLanguage/parser/input.gtl"; // Update the file path accordingly
    string input = File.ReadAllText(filePath);

    // Create a stream from the input string
    ICharStream stream = CharStreams.fromString(input);

    // Create a lexer
    ITokenSource lexer = new ExprLexer(stream);

    // Create a token stream
    CommonTokenStream tokens = new CommonTokenStream(lexer);

    // Print the different tokens
    tokens.Fill();
    foreach (var token in tokens.GetTokens())
    {
        Console.WriteLine("Token: " + token);
    }

    // Reset the token stream position to the beginning
    tokens.Reset();

    // Create a parser
    ExprParser parser = new ExprParser(tokens);
}

MyParseMethod();

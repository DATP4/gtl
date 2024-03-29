﻿using Antlr4.Runtime;

static void MyParseMethod()
{
    // Read input from a file
    string filePath = "parser/input.gtl"; // Update the file path accordingly
    string input = File.ReadAllText(filePath);

    // Create a stream from the input string
    ICharStream stream = CharStreams.fromString(input);

    // Create a lexer
    GtlLexer lexer = new GtlLexer(stream);

    // Create a token stream    
    CommonTokenStream tokens = new CommonTokenStream(lexer);

    // Create a parser
    GtlParser parser = new GtlParser(tokens);
    GtlParser.ProgramContext tree = parser.program(); // Assign parse tree to a variable

    // Create a visitor
    CustomGtlVisitor visitor = new CustomGtlVisitor();

    _ = visitor.VisitProgram(tree);

    // Optionally print the parse tree for debugging
    Console.WriteLine(tree.ToStringTree(parser));
}

MyParseMethod();

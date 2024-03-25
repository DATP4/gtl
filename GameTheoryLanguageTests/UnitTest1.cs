using Microsoft.VisualStudio.TestTools.UnitTesting;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
namespace GameTheoryLanguageTests;


[TestClass]
public class UnitTest1

{
    public static string Parse (string path) {
    // Read input from a file
    // Update the file path accordingly
    string filePath = path;
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

    return tree.ToStringTree(parser);
    }

    [TestMethod]
    public void IfElseTest() {
        string workingDirectory = Environment.CurrentDirectory;
        #pragma warning disable CS8602 // Removes dereferencing warning.
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "./TestPrograms/IfElseTest.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (if if ( (expr TRUE) ) then { } (else else { }))) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    [TestMethod]
    public void AssignmentTest() {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "./TestPrograms/AssignmentTest.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (declaration (type int) x = (expr 2) ;)) (statement (declaration (type int) y = (expr 3) ;)) (statement (declaration (type int) z = (expr (expr y) + (expr x)) ;)) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    /*
    Create after removal of return
    [TestMethod]
    public void FunctionTest() {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "./TestPrograms/FunctionTest.gtl";
        string output = Parse(path);
        string expectedoutput = "";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    */
    [TestMethod]
    public void PayoffTest1() {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "./TestPrograms/PayoffTest1.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (payoff Payoffs payoff ( ) = { p1 -> (array [ (expr 1) , (expr 4) , (expr 0) , (expr 2) ]) , p2 -> (array [ (expr 1) , (expr 0) , (expr 4) , (expr 2) ]) })) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    [TestMethod]
    public void PayoffTest2() {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "./TestPrograms/PayoffTest2.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (declaration (type int) x = (expr 2) ;)) (statement (declaration (type int) y = (expr 1) ;)) (statement (declaration (type int) z = (expr 0) ;)) (statement (payoff Payoffs payoff ( ) = { p1 -> (array [ (expr x) , (expr y) , (expr z) , (expr x) ]) , p2 -> (array [ (expr x) , (expr z) , (expr y) , (expr x) ]) })) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    /*
    Create after Strategy is changed
    [TestMethod]
    public void StrategyTest() {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "./TestPrograms/PayoffTest.gtl";
        string output = Parse(path);
        string expectedoutput = "";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    */
    /*
    Create after StrategySet is changed
    [TestMethod]
    public void StrategySetTest() {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "./TestPrograms/PayoffTest.gtl";
        string output = Parse(path);
        string expectedoutput = "";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    */
    [TestMethod]
    public void PDTest() {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "./TestPrograms/PDTest.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (action Action history = { (statement (if if ( (expr (expr (expr (expr gamestate ( arg_call )) . opponent ( arg_call )) . lastmove ( arg_call )) == (expr cooperate)) ) then { (statement (return return (expr deflect) ;)) } (elseif else if ( (expr (expr (expr (expr gamestate ( arg_call )) . opponent ( arg_call )) . lastmove ( arg_call )) == (expr deflect)) ) then { (statement (return return (expr cooperate) ;)) } (else else { (statement (return return (expr turn) ;)) })))) })) (statement (action Action turn = { (statement (if if ( (expr (expr (expr gamestate ( arg_call )) . turn ( arg_call )) == (expr 4)) ) then { (statement (return return (expr cooperate) ;)) })) })) (statement (strategy Strategy aStrat1 -> (arg_strategy cooperate , deflect) { history , turn ; })) (statement (strategy Strategy aStrat2 -> (arg_strategy deflect) { history ; })) (statement (strategy_set StrategySet stratset = (strategy_set_array [ (move_tuple ( cooperate , cooperate )) , (move_tuple ( cooperate , deflect )) , (move_tuple ( deflect , cooperate )) , (move_tuple ( deflect , deflect )) ]))) (statement (payoff Payoffs payoff ( ) = { p1 -> (array [ (expr 1) , (expr 4) , (expr 0) , (expr 2) ]) , p2 -> (array [ (expr 1) , (expr 0) , (expr 4) , (expr 2) ]) })) (statement (player Player aske ( aStrat2 ) ;)) (statement (player Player martin ( aStrat1 ) ;)) (statement (game Game prisoners ( (array [ (expr aske) , (expr martin) ]) , stratset , (expr payoff ( arg_call )) ) ;)) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
}

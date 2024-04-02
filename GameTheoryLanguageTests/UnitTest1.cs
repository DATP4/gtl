using System;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace GameTheoryLanguageTests;


[TestClass]
public class UnitTest1

{
    public static string Parse(string path)
    {
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
    public void IfElseTest()
    {
        string workingDirectory = Environment.CurrentDirectory;
#pragma warning disable CS8602 // Removes dereferencing warning.
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "/TestPrograms/IfElseTest.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (if if ( (expr (literal (boolean_literal TRUE))) ) then { } (else else { }))) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    [TestMethod]
    public void AssignmentTest()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "/TestPrograms/AssignmentTest.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (declaration (type int) x = (expr (literal 2)) ;)) (statement (declaration (type int) y = (expr (literal 3)) ;)) (statement (declaration (type int) z = (expr (expr y) + (expr x)) ;)) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    [TestMethod]
    public void BooleanTest()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "/TestPrograms/BooleanTest.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (expr (expr (literal (boolean_literal TRUE))) && (expr (literal (boolean_literal FALSE)))) ;) (statement (expr (expr (literal (boolean_literal TRUE))) || (expr (literal (boolean_literal FALSE)))) ;) (statement (expr (expr (literal (boolean_literal TRUE))) == (expr (literal (boolean_literal FALSE)))) ;) (statement (expr (expr (literal (boolean_literal TRUE))) ^^ (expr (literal (boolean_literal FALSE)))) ;) (statement (expr (expr (literal 1)) < (expr (literal 3))) ;) (statement (expr (expr (literal 1)) >= (expr (literal 3))) ;) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    [TestMethod]
    public void FunctionTest()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "/TestPrograms/FunctionTest.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (function testFunction : ( (arg_def (type int) a) ) -> (type int) { (statement (expr (expr a) + (expr (literal 1))) ;) })) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    [TestMethod]
    public void PayoffTest1()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "/TestPrograms/PayoffTest1.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (payoff Payoffs payoff = (expr (array [ (expr (util_function p1 -> (expr (array [ (expr (literal 1)) , (expr (literal 4)) , (expr (literal 0)) , (expr (literal 2)) ])))) , (expr (util_function p2 -> (expr (array [ (expr (literal 1)) , (expr (literal 0)) , (expr (literal 4)) , (expr (literal 2)) ])))) ]))) ;) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    [TestMethod]
    public void PayoffTest2()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "/TestPrograms/PayoffTest2.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (declaration (type int) x = (expr (literal 2)) ;)) (statement (declaration (type int) y = (expr (literal 1)) ;)) (statement (declaration (type int) z = (expr (literal 0)) ;)) (statement (payoff Payoffs payoff = (expr (array [ (expr (util_function p1 -> (expr (array [ (expr x) , (expr y) , (expr z) , (expr x) ])))) , (expr (util_function p2 -> (expr (array [ (expr x) , (expr z) , (expr y) , (expr x) ])))) ]))) ;) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    [TestMethod]
    public void StrategyTest()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "/TestPrograms/StrategyTest.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (action Action turn = ( (expr (expr (expr gamestate) (member_access . turn)) == (expr (literal 4))) ) then deflect ;)) (statement (strategy Strategy aStrat = (expr (array [ (expr turn) ]))) ;) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    [TestMethod]
    public void StrategySetTest()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "/TestPrograms/StrategySetTest.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (strategy_space Strategyspace stratspace = (expr (array [ (expr (tuple ( (expr cooperate) , (expr cooperate) ))) , (expr (tuple ( (expr deflect) , (expr cooperate) ))) , (expr (tuple ( (expr cooperate) , (expr deflect) ))) , (expr (tuple ( (expr deflect) , (expr deflect) ))) ]))) ;) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
    [TestMethod]
    public void ActionTest()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string currentPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string path = currentPath + "/TestPrograms/ActionTest.gtl";
        string output = Parse(path);
        string expectedoutput = "(program (statement (action Action oppCooperate = ( (expr (expr (expr gamestate) (member_access . opponent) (member_access . lastmove)) == (expr deflect)) ) then cooperate ;)) (statement (action Action turn = ( (expr (expr (expr gamestate) (member_access . turn)) == (expr (literal 4))) ) then deflect ;)) <EOF>)";
        Console.WriteLine(output);
        Console.WriteLine(expectedoutput);
        Assert.IsTrue(output.Equals(expectedoutput));
    }
}

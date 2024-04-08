using Antlr4.Runtime;
namespace GameTheoryLanguageTests;

[TestClass]
public class UnitTest_Excep

{
    [TestMethod]
    public void TestLiteral()
    {
        string inputBad = "bool test = true;";
        string inputBad2 = "FAlsE;";
        string inputBad3 = "d;";
        string inputGood = "TRUE;";
        string inputGood2 = "FALSE;";
        string inputGood3 = "3;";
        string inputGood4 = "3.0;";

        // Test if exception is thrown on wrong Literal
        TestException<VariableNotFoundException>(inputBad);
        TestException<VariableNotFoundException>(inputBad2);
        TestException<VariableNotFoundException>(inputBad3);
        // The VisitLiteralExpr should throw exception: NotSupportedException($"literal expression expected type but recieved " + val)
        //, but because of the way variables have been implemented it thinks everything that is not a literal is a variable.

        // Test on if the input is correct
        TestAccepted(inputGood);
        TestAccepted(inputGood2);
        TestAccepted(inputGood3);
        TestAccepted(inputGood4);
    }

    [TestMethod]
    public void TestDeclaration()
    {
        string input = "bool x = 1;";
        string input2 = "bool x = TRUE;";
        string input3 = "int x = 1.4;";
        string input4 = "int x = 4;";

        TestException<DeclarationException>(input);
        TestAccepted(input2);
        TestException<DeclarationException>(input3);
        TestAccepted(input4);
    }

    [TestMethod]
    public void TestBinaryExpr()
    {
        string input1 = "int test = 1 + 1.5;";
        string input2 = "real test2 = 1.5 + 1;";

        string input3 = "int test = 1 + 1 * 7;";
        string input4 = "real test2 = 1.5 + 1.6 / 10.0;";

        // Test throws exception
        TestException<BinaryExpressionException>(input1);
        TestException<BinaryExpressionException>(input2);

        // Test does not throw exception
        TestAccepted(input3);
        TestAccepted(input4);
    }

    [TestMethod]
    public void TestBoolExpr()
    {
        string input1 = "bool test1 = TRUE && 2.0;";
        string input2 = "bool test1 = FALSE && 1;";
        string input3 = "bool test1 = 1 > 0.8;";
        string input4 = "bool test1 = 1 && 2;";

        string input5 = "bool test1 = TRUE && FALSE;";
        string input6 = "bool test1 = 1 > 2;";
        string input7 = "bool test1 = 1 != 0;";
        string input8 = "bool test1 = 1 <= 2;";

        // Test throws exception
        TestException<BooleanExpressionException>(input1);
        TestException<BooleanExpressionException>(input2);
        TestException<BooleanExpressionException>(input3);
        TestException<BooleanExpressionException>(input4);

        // Test does not throw exception
        TestAccepted(input5);
        TestAccepted(input6);
        TestAccepted(input7);
        TestAccepted(input8);
    }

    [TestMethod]
    public void TestLogicalNot()
    {
        string input1 = "bool test = TRUE; !test";
        string input2 = "int test = 1 + 1; !test";

        // Test throws exception
        TestException<BooleanExpressionException>(input2);

        // Test does not throw exception
        TestAccepted(input1);
    }

    [TestMethod]
    public void TestID()
    {
        string input1 = "bool x = TRUE; test;";
        string input2 = "test;";
        string input3 = "int test = 1; int test2 = 1.0 + test;";
        string input4 = "int test = 1 + 1; int test2 = test + 1;";

        // Test throws exception
        TestException<VariableNotFoundException>(input1);
        TestException<VariableNotFoundException>(input2);
        TestException<BinaryExpressionException>(input3);

        // Test does not throw exception
        TestAccepted(input4);
    }

    [TestMethod]
    public void TestIf()
    {
        string input1 = "if (TRUE) then {int x = 4;} x;";
        string input2 = "int y = 5; if (TRUE) then {int x = y + 4;}";
        string input3 = "if (TRUE) then {int x = 4; int y = x;}";

        // Test throws exception
        TestException<VariableNotFoundException>(input1);

        // Test does not throw exception
        TestAccepted(input2);
        TestAccepted(input3);
    }

    [TestMethod]
    public void TestIfElse()
    {
        string input1 = "if (TRUE) then {int x = 4;} else {x;}";
        string input2 = "if (TRUE) then {int x = 4; x;} else {int y = 5; y;} x;";
        string input3 = "if (TRUE) then {int x = 4; x;} else {int y = 5; y;} y;";

        string input4 = "if (TRUE) then {int x = 4; x;} else {int y = 5; y;}";

        // Test throws exception
        TestException<VariableNotFoundException>(input1);
        TestException<VariableNotFoundException>(input2);
        TestException<VariableNotFoundException>(input3);

        // Test does not throw exception
        TestAccepted(input4);
    }

    [TestMethod]
    public void TestElseIf()
    {
        string input1 = "if (TRUE) then {int x = 4;} else if (FALSE) then {x;}";
        string input2 = "if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 4;} else {int z = 4;} x;";
        string input3 = "if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 4;} else {int z = 4;} y;";
        string input4 = "if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 4;} else {int z = 4;} z;";

        string input5 = "if (TRUE) then {int x = 4; x;} else if (FALSE) {int y = 5; y;}";
        string input6 = "if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 4;} else {int z = 4;}";

        // Test throws exception
        TestException<VariableNotFoundException>(input1);
        TestException<VariableNotFoundException>(input2);
        TestException<VariableNotFoundException>(input3);
        TestException<VariableNotFoundException>(input4);

        // Test does not throw exception
        TestAccepted(input5);
        TestAccepted(input6);
    }

    [TestMethod]
    public void TestFunction()
    {
        string input = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;} intFunction(5);"; // is good
        string input1 = "int a = 10; intFunction1 : (int x) -> int {int y = x + a * 5; x - 5;} intFunction1(5);"; // is good
        string input2 = "intFunction2 : (int x) -> int {int y = x + 10 * 5; x - 5;} x;"; // is bad
        string input3 = "int x = func(4,5);";
        string input4 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;} intFunction(5, 0);";
        string input5 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;} intFunction(TRUE);";

        // Test throws exception
        /* Here the 'HelperThrowsexception(input);' adds the intFunction to the Ftable 
        and 'TestException<NotSupportedException>(input, "Variable x not found");' also adds the intFunction to the Ftable.
        This result in an exception as the intFuntion is added twice, therefore the ClearFTable is used. */
        TestException<VariableNotFoundException>(input2);
        TestException<FunctionCallException>(input3);
        TestException<FunctionCallException>(input4);
        TestException<FunctionCallException>(input5);

        // Test does not throw exception
        TestAccepted(input);
        TestAccepted(input1);
    }
    [TestMethod]
    public void TestMemberAccess()
    {
        string input1 = "gamestate.opponent.lastmove;";
        string input2 = "gamestate.turn;";
        string input3 = "gamestate.lastmove;";
        string input4 = "int x = 5; x.turn;";
        string input5 = "gamestat.turn;";

        TestAccepted(input1);
        TestAccepted(input2);
        TestException<MemberAccessException>(input3);
        TestException<WrongTypeException>(input4);
        TestException<VariableNotFoundException>(input5);
    }

    private void TestException<T>(string toTest) where T : Exception
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(toTest));
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream);

        var parseTree = parser.program();
        CustomGtlVisitor visitor = new CustomGtlVisitor();

        var _ = Assert.ThrowsException<T>(() => visitor.Visit(parseTree));
    }

    private void TestAccepted(string toTest)
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(toTest));
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream);

        var parseTree = parser.program();
        CustomGtlVisitor visitor = new CustomGtlVisitor();

        Exception caughtException = null!;

        try
        {
            _ = visitor.Visit(parseTree);
        }
        catch (Exception ex)
        {
            caughtException = ex;
        }

        Assert.IsNull(caughtException, "Unexpected exception occurred.");
    }
}

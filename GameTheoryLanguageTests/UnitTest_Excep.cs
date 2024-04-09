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

        // Test if excepetion is thrown on wrong Literal
        HelperThrowsExcepetion(inputBad);
        HelperExcepetionMessage(inputBad, "Variable true not found");
        HelperThrowsExcepetion(inputBad2);
        HelperExcepetionMessage(inputBad2, "Variable FAlsE not found");
        HelperThrowsExcepetion(inputBad3);
        HelperExcepetionMessage(inputBad3, "Variable d not found");
        // The VisitLiteralExpr should throw exception: NotSupportedException($"literal expression expected type but recieved " + val)
        //, but because of the way variables have been implemented it thinks everything that is not a literal is a variable.

        // Test on if the input is correct
        HelperAcceptedTest(inputGood);
        HelperAcceptedTest(inputGood2);
    }

    [TestMethod]
    public void TestType()
    {
        string input = "bool x = 1;";
        string input2 = "bool x = TRUE;";
        string input3 = "int x = 1.4;";
        string input4 = "int x = 4;";

        HelperThrowsExcepetion(input);
        HelperExcepetionMessage(input, "Declaration expected type bool but recieved int");
        HelperAcceptedTest(input2);
        HelperThrowsExcepetion(input3);
        HelperExcepetionMessage(input3, "Declaration expected type int but recieved real");
        HelperAcceptedTest(input4);
    }

    [TestMethod]
    public void TestBinaryExpr()
    {
        string input1 = "int test = 1 + 1.5;";
        string input2 = "real test2 = 1.5 + 1;";

        string input3 = "int test = 1 + 1 * 7;";
        string input4 = "real test2 = 1.5 + 1.6 / 10.0;";

        // Test throws excepetion
        HelperThrowsExcepetion(input1);
        HelperExcepetionMessage(input1, "binary expression expected type int but recieved type real");
        HelperThrowsExcepetion(input2);
        HelperExcepetionMessage(input2, "binary expression expected type real but recieved type int");

        // Test does not throw excepetion
        HelperAcceptedTest(input3);
        HelperAcceptedTest(input4);
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

        // Test throws excepetion
        HelperThrowsExcepetion(input1);
        HelperExcepetionMessage(input1, "boolean expression expected type bool but recieved type real");
        HelperThrowsExcepetion(input2);
        HelperExcepetionMessage(input2, "boolean expression expected type bool but recieved type int");
        HelperThrowsExcepetion(input3);
        HelperExcepetionMessage(input3, "boolean expression expected type int but recieved type real");
        HelperThrowsExcepetion(input4);
        HelperExcepetionMessage(input4, "boolean expression expected type int but recieved type int"); // TODO: fix thrown exception (the message)

        // Test does not throw excepetion
        HelperAcceptedTest(input5);
        HelperAcceptedTest(input6);
        HelperAcceptedTest(input7);
        HelperAcceptedTest(input8);
    }

    [TestMethod]
    public void TestLogicalNot()
    {
        string input1 = "bool test = TRUE; !test";
        string input2 = "int test = 1 + 1; !test";

        // Test throws excepetion
        HelperThrowsExcepetion(input2);
        HelperExcepetionMessage(input2, "Logical not expression expected type bool but recieved type int");

        // Test does not throw excepetion
        HelperAcceptedTest(input1);
    }

    [TestMethod]
    public void TestID()
    {
        string input1 = "bool x = TRUE; test;";
        string input2 = "test;";
        string input3 = "int test = 1 + 1; test;";

        // Test throws excepetion
        HelperThrowsExcepetion(input1);
        HelperExcepetionMessage(input1, "Variable test not found");
        HelperThrowsExcepetion(input2);
        HelperExcepetionMessage(input2, "Variable test not found");

        // Test does not throw excepetion
        HelperAcceptedTest(input3);
    }

    [TestMethod]
    public void TestIf()
    {
        string input1 = "if (TRUE) then {int x = 4;} x;";
        string input2 = "int y = 5; if (TRUE) then {int x = y + 4;}";

        string input3 = "if (TRUE) then {int x = 4; x;}";

        // Test throws excepetion
        HelperThrowsExcepetion(input1);
        HelperExcepetionMessage(input1, "Variable x not found");

        // Test does not throw excepetion
        HelperAcceptedTest(input2);
        HelperAcceptedTest(input3);
    }

    [TestMethod]
    public void TestIfElse()
    {
        string input1 = "if (TRUE) then {int x = 4;} else {x;}";

        string input2 = "if (TRUE) then {int x = 4; x;} else {int y = 5; y;}";

        // Test throws excepetion
        HelperThrowsExcepetion(input1);
        HelperExcepetionMessage(input1, "Variable x not found");

        // Test does not throw excepetion
        HelperAcceptedTest(input2);
    }

    [TestMethod]
    public void TestElseIf()
    {
        string input1 = "if (TRUE) then {int x = 4;} else if (FALSE) then {x;}";

        string input2 = "if (TRUE) then {int x = 4; x;} else if (FALSE) {int y = 5; y;}";

        // Test throws excepetion
        HelperThrowsExcepetion(input1);
        HelperExcepetionMessage(input1, "Variable x not found");

        // Test does not throw excepetion
        HelperAcceptedTest(input2);
    }

    [TestMethod]
    public void TestFunction()
    {
        CustomGtlVisitor visitor = new CustomGtlVisitor();
        string input = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;} intFunction(5);"; // is good
        string input1 = "int a = 10; intFunction1 : (int x) -> int {int y = x + a * 5; x - 5;} intFunction1(5);"; // is good
        string input2 = "intFunction2 : (int x) -> int {int y = x + 10 * 5; x - 5;} x;"; // is bad

        // Test throws excepetion
        HelperThrowsExcepetion(input2);
        /* Here the 'HelperThrowsExcepetion(input);' adds the intFunction to the Ftable 
        and 'HelperExcepetionMessage(input, "Variable x not found");' also adds the intFunction to the Ftable.
        This result in an exception as the intFuntion is added twice, therefore the ClearFTable is used. */
        visitor.ClearFtable();
        HelperExcepetionMessage(input2, "Variable x not found");

        // Test does not throw excepetion
        HelperAcceptedTest(input);
        HelperAcceptedTest(input1);
    }

    private void HelperThrowsExcepetion(string toTest)
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(toTest));
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream);

        var parseTree = parser.program();
        CustomGtlVisitor visitor = new CustomGtlVisitor();

#pragma warning disable IDE0058 // Expression value is never used
        Assert.ThrowsException<NotSupportedException>(() => visitor.Visit(parseTree));
#pragma warning restore IDE0058 // Expression value is never used
    }

    private void HelperExcepetionMessage(string toTest, string messageToMatch)
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(toTest));
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream);

        var parseTree = parser.program();
        CustomGtlVisitor visitor = new CustomGtlVisitor();

        var exceptionMessage = Assert.ThrowsException<NotSupportedException>(() => visitor.Visit(parseTree));
        Assert.AreEqual(messageToMatch, exceptionMessage.Message);
    }

    private void HelperAcceptedTest(string toTest)
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(toTest));
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream);

        var parseTree = parser.program();
        CustomGtlVisitor visitor = new CustomGtlVisitor();

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        Exception caughtException = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

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

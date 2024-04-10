using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
namespace GameTheoryLanguageTests;

[TestClass]
public class TypecheckingTests

{
    [TestMethod]
    public void TestLiteral()
    {
        string input1 = "TRUE;";
        string input2 = "FALSE;";
        string input3 = "3;";
        string input4 = "3.0;";
        string input5 = "bool test = true;";
        string input6 = "FAlsE;";
        string input7 = "d;";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertTrue(input4);

        AssertFalse<VariableNotFoundException>(input5);
        AssertFalse<VariableNotFoundException>(input6);
        AssertFalse<VariableNotFoundException>(input7);
    }

    [TestMethod]
    public void TestDeclaration()
    {
        string input1 = "bool x = TRUE;";
        string input2 = "int x = 4;";
        string input3 = "bool x = 1;";
        string input4 = "int x = 1.4;";

        AssertTrue(input1);
        AssertTrue(input2);

        AssertFalse<DeclarationException>(input3);
        AssertFalse<DeclarationException>(input4);
    }

    [TestMethod]
    public void TestBinaryExpr()
    {
        string input1 = "int test = 1 + 1 * 7;";
        string input2 = "real test2 = 1.5 + 1.6 / 10.0;";
        string input3 = "int test = 1 + 1.5;";
        string input4 = "real test2 = 1.5 + 1;";

        AssertTrue(input1);
        AssertTrue(input2);

        AssertFalse<BinaryExpressionException>(input3);
        AssertFalse<BinaryExpressionException>(input4);
    }

    [TestMethod]
    public void TestBoolExpr()
    {
        string input1 = "bool test1 = TRUE && FALSE;";
        string input2 = "bool test1 = 1 > 2;";
        string input3 = "bool test1 = 1 != 0;";
        string input4 = "bool test1 = 1 <= 2;";
        string input5 = "bool test1 = TRUE && 2.0;";
        string input6 = "bool test1 = FALSE && 1;";
        string input7 = "bool test1 = 1 > 0.8;";
        string input8 = "bool test1 = 1 && 2;";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertTrue(input4);

        AssertFalse<BooleanExpressionException>(input5);
        AssertFalse<BooleanExpressionException>(input6);
        AssertFalse<BooleanExpressionException>(input7);
        AssertFalse<BooleanExpressionException>(input8);
    }

    [TestMethod]
    public void TestLogicalNot()
    {
        string input1 = "bool test = TRUE; !test;";
        string input2 = "int test = 1 + 1; !test;";

        AssertTrue(input1);

        AssertFalse<BooleanExpressionException>(input2);
    }

    [TestMethod]
    public void TestID()
    {
        string input1 = "int test = 1 + 1; int test2 = test + 1;";
        string input2 = "bool x = TRUE; test;";
        string input3 = "test;";
        string input4 = "int test = 1; int test2 = 1.0 + test;";

        AssertTrue(input1);

        AssertFalse<VariableNotFoundException>(input2);
        AssertFalse<VariableNotFoundException>(input3);
        AssertFalse<BinaryExpressionException>(input4);
    }
    [TestMethod]
    public void TestIfElse()
    {
        string input1 = "if (TRUE) then {int x = 4; x;} else {int y = 5; y;};";
        string input2 = "if (TRUE) then {int x = 4;} else {x;};";
        string input3 = "if (TRUE) then {int x = 4; x;} else {int y = 5; y;}; x;";
        string input4 = "if (TRUE) then {int x = 4; x;} else {int y = 5; y;}; y;";
        string input5 = "if (x) then {int x = 4;} else {int x = 4;};";
        string input6 = "int x = 5; if(x) then {int y = 4;} else {int y = 4;};";

        AssertTrue(input1);

        AssertFalse<VariableNotFoundException>(input2);
        AssertFalse<VariableNotFoundException>(input3);
        AssertFalse<VariableNotFoundException>(input4);
        AssertFalse<VariableNotFoundException>(input5);
        AssertFalse<WrongTypeException>(input6);
    }

    [TestMethod]
    public void TestElseIf()
    {
        string input1 = "if (TRUE) then {int x = 4; x;} else if (FALSE) then {int y = 5; y;} else {int z = 6; z;};";
        string input2 = "if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 4;} else {int z = 4;};";
        string input3 = "if (TRUE) then {int x = 4;} else if (FALSE) then {x;} else {x;};";
        string input4 = "if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 4;} else {int z = 4;}; x;";
        string input5 = "if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 4;} else {int z = 4;}; y;";
        string input6 = "if (TRUE) then {int x = 4;} else if (FALSE) then {int y = 4;} else {int z = 4;}; z;";
        string input7 = "int x = 5; if (TRUE) then {int y = 5;} else if (x) then {int z = 5;} else {int b = 4;};";

        AssertTrue(input1);
        AssertTrue(input2);

        AssertFalse<VariableNotFoundException>(input3);
        AssertFalse<VariableNotFoundException>(input4);
        AssertFalse<VariableNotFoundException>(input5);
        AssertFalse<VariableNotFoundException>(input6);
        AssertFalse<WrongTypeException>(input7);
    }

    [TestMethod]
    public void TestFunction()
    {
        string input1 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;} intFunction(5);";
        string input2 = "int a = 10; intFunction1 : (int x) -> int {int y = x + a * 5; x - 5;} intFunction1(5);";
        string input3 = "intFunction2 : (int x) -> int {int y = x + 10 * 5; x - 5;} x;";
        string input4 = "int x = func(4,5);";
        string input5 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;} intFunction(5, 0);";
        string input6 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5;} intFunction(TRUE);";

        AssertTrue(input1);
        AssertTrue(input2);

        AssertFalse<VariableNotFoundException>(input3);
        AssertFalse<FunctionCallException>(input4);
        AssertFalse<FunctionCallException>(input5);
        AssertFalse<FunctionCallException>(input6);
    }
    [TestMethod]
    public void TestMemberAccess()
    {
        string input1 = "gamestate.opponent.lastmove;";
        string input2 = "gamestate.turn;";
        string input3 = "gamestate.lastmove;";
        string input4 = "int x = 5; x.turn;";
        string input5 = "gamestat.turn;";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse<MemberAccessException>(input3);
        AssertFalse<WrongTypeException>(input4);
        AssertFalse<VariableNotFoundException>(input5);
    }
    [TestMethod]
    public void TestActionDeclaration()
    {
        string input1 = "Moves t = [move]; Action turn = (TRUE) then move;";
        string input2 = "Moves t = [move]; bool a = TRUE; Action turn = (a) then move;";
        string input3 = "Moves t = [move]; Action turn = [5, 4];";
        string input4 = "Moves t = [move]; Action turn = (move, move, move);";
        string input5 = "Action turn = (TRUE) then move;";


        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse<WrongTypeException>(input3);
        AssertFalse<WrongTypeException>(input4);
        AssertFalse<VariableNotFoundException>(input5);
    }
    [TestMethod]
    public void TestStrategyDeclaration()
    {
        string input1 = "Moves t = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn];";
        string input2 = "Moves t = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, 5];";
        string input3 = "Strategy strat = [x];";


        AssertTrue(input1);
        AssertFalse<WrongTypeException>(input2);
        AssertFalse<VariableNotFoundException>(input3);
    }
    [TestMethod]
    public void TestPlayerDeclaration()
    {
        string input1 = "Moves t = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses strat, p2 chooses strat];";
        string input2 = "Moves t = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses x, p2 chooses strat];";
        string input3 = "Players p = [p1 chooses strat, p2 chooses strat];";


        AssertTrue(input1);
        AssertFalse<VariableNotFoundException>(input2);
        AssertFalse<VariableNotFoundException>(input3);
    }
    [TestMethod]
    public void TestPayoffDeclaration()
    {
        string input1 = "Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];";
        string input2 = "int x = 5; Payoff payoff = [p1 -> [1,4,0,x],p2 -> [1,x,4,2]];";
        string input3 = "bool x = TRUE; Payoff payoff = [p1 -> [1,4,0,x],p2 -> [1,x,4,2]];";
        string input4 = "Payoff payoff = (move, move, move);";


        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse<WrongTypeException>(input3);
        AssertFalse<VariableNotFoundException>(input4);
    }
    [TestMethod]
    public void TestStrategySpaceDeclaration()
    {
        string input1 = "Moves t = [deflect, cooperate]; Strategyspace stratspace = [(cooperate, cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];";
        string input2 = "Strategyspace stratspace = [(cooperate, cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];";
        string input3 = "int x = 5; Moves t = [deflect, cooperate]; Strategyspace stratspace = [(x, cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];";

        AssertTrue(input1);
        AssertFalse<VariableNotFoundException>(input2);
        AssertFalse<WrongTypeException>(input3);
    }
    [TestMethod]
    public void TestGameTuple()
    {
        string input1 = "Moves t = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses strat, p2 chooses strat]; Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]; Strategyspace stratspace = [(move, move)]; Game g = (stratspace, p, payoff); g.run(10);";
        string input2 = "Game g = (stratspace, p, payoff);";
        string input3 = "Moves t = [move]; Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]; Strategyspace stratspace = [(move, move)]; Game g = (stratspace, p, payoff);";
        string input4 = "Moves t = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses strat, p2 chooses strat]; Strategyspace stratspace = [(move, move)]; Game g = (stratspace, p, payoff);";
        string input5 = "Moves t = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses strat, p2 chooses strat]; Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]; Game g = (stratspace, p, payoff);";
        string input6 = "Moves t = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses strat, p2 chooses strat]; Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]; Strategyspace stratspace = [(move, move)]; Game g = (stratspace, p, payoff); g.run(TRUE);";
        string input7 = "int x = 5; x.run(10);";


        AssertTrue(input1);
        AssertFalse<VariableNotFoundException>(input2);
        AssertFalse<VariableNotFoundException>(input3);
        AssertFalse<VariableNotFoundException>(input4);
        AssertFalse<VariableNotFoundException>(input5);
        AssertFalse<WrongTypeException>(input6);
        AssertFalse<WrongTypeException>(input7);
    }
    private void AssertFalse<T>(string toTest) where T : Exception
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(toTest));
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream)!;

        parser.ErrorHandler = new ErrorStrategy();
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener());

        var parseTree = parser.program();
        CustomGtlVisitor visitor = new CustomGtlVisitor();

        var _ = Assert.ThrowsException<T>(() => visitor.Visit(parseTree));
    }

    private void AssertTrue(string toTest)
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(toTest));
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream)!;

        parser.ErrorHandler = new ErrorStrategy();
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener());

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
    public class ErrorStrategy : DefaultErrorStrategy
    {
        public override void ReportError(Parser recognizer, RecognitionException e)
        {
            throw new ParserException();
        }
    }
    public class ErrorListener : BaseErrorListener
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            Console.WriteLine(msg + " in line " + line + " position " + charPositionInLine);
            throw new ParserException();
        }
    }
}

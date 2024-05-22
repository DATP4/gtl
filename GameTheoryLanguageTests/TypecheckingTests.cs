using System.Data;

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
namespace GameTheoryLanguageTests;

[TestClass]
public class TypecheckingTests

{
    public string WholeGameString = "Moves = [cooperate, defect]; Action testaction = () then cooperate; Strategy teststrategy = [testaction]; Strategyspace teststratspace = [(cooperate, cooperate)]; Payoff testpayoff = [p1 -> [1]]; Players testplayers = [p1 chooses teststrategy]; Game testgame = (teststratspace, testplayers, testpayoff); run(testgame, 4);";
    public string RunString = "run(id, 4);";
    [TestMethod]
    public void TestLiteral()
    {
        string input1 = "bool test = TRUE;" + WholeGameString;
        string input2 = "bool test = FALSE;" + WholeGameString;
        string input3 = "int test = 3;" + WholeGameString;
        string input4 = "real test = 3.0;" + WholeGameString;
        string input5 = "bool test = true;" + WholeGameString;
        string input6 = "bool test = FAlsE;" + WholeGameString;
        string input7 = "bool test = d;" + WholeGameString;

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
        string input1 = "bool x = TRUE;" + WholeGameString;
        string input2 = "int x = 4;" + WholeGameString;
        string input3 = "bool x = 1;" + WholeGameString;
        string input4 = "int x = 1.4;" + WholeGameString;
        string input5 = "int x = 5; int x = 5;" + WholeGameString;

        AssertTrue(input1);
        AssertTrue(input2);

        AssertFalse<DeclarationException>(input3);
        AssertFalse<DeclarationException>(input4);
        AssertFalse<DeclarationException>(input5);
    }

    [TestMethod]
    public void TestBinaryExpr()
    {
        string input1 = "int test = 1 + 1 * 7;" + WholeGameString;
        string input2 = "real test2 = 1.5 + 1.6 / 10.0;" + WholeGameString;
        string input3 = "int test = (-5 + ( 4 + 3 * ( 4 MOD 5 ) / 1 + 5 ) - 3);" + WholeGameString;
        string input4 = "int test = 5 MOD 3;" + WholeGameString;
        string input5 = "int test = 1 + 1.5;" + WholeGameString;
        string input6 = "real test2 = 1.5 + 1;" + WholeGameString;
        string input7 = "real test = 5.0 MOD 3.0;" + WholeGameString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertTrue(input4);

        AssertFalse<BinaryExpressionException>(input5);
        AssertFalse<BinaryExpressionException>(input6);
        AssertFalse<WrongTypeException>(input7);
    }

    [TestMethod]
    public void TestBoolExpr()
    {
        string input1 = "bool test1 = TRUE && FALSE;" + WholeGameString;
        string input2 = "bool test1 = 1 > 2;" + WholeGameString;
        string input3 = "bool test1 = 1 != 0;" + WholeGameString;
        string input4 = "bool test1 = 1 <= 2;" + WholeGameString;
        string input5 = "bool test1 = (TRUE && TRUE) || ((TRUE && TRUE) == TRUE);" + WholeGameString;
        string input6 = "bool test1 = TRUE && 2.0;" + WholeGameString;
        string input7 = "bool test1 = FALSE && 1;" + WholeGameString;
        string input8 = "bool test1 = 1 > 0.8;" + WholeGameString;
        string input9 = "bool test1 = 1 && 2;" + WholeGameString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertTrue(input4);
        AssertTrue(input5);

        AssertFalse<BooleanExpressionException>(input6);
        AssertFalse<BooleanExpressionException>(input7);
        AssertFalse<BooleanExpressionException>(input8);
        AssertFalse<BooleanExpressionException>(input9);
    }

    [TestMethod]
    public void TestLogicalNot()
    {
        string input1 = "bool test = TRUE; bool test2 = !test;" + WholeGameString;
        string input2 = "int test = 1 + 1; int test2 = !test;" + WholeGameString;

        AssertTrue(input1);

        AssertFalse<WrongTypeException>(input2);
    }
    [TestMethod]
    public void TestUnaryExpression()
    {
        string input1 = "int x = -5;" + WholeGameString;
        string input2 = "real x = -5.0;" + WholeGameString;
        string input3 = "int x = -5 - -5;" + WholeGameString;
        string input4 = "int x = -5 + -5;" + WholeGameString;
        string input5 = "int x = -5; int test = -x;" + WholeGameString;
        string input6 = "bool x = -TRUE;" + WholeGameString;
        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertTrue(input4);
        AssertTrue(input5);

        AssertFalse<WrongTypeException>(input6);

    }
    [TestMethod]
    public void TestID()
    {
        string input1 = "int test = 1 + 1; int test2 = test + 1;" + WholeGameString;
        string input2 = "bool x = TRUE; int test2 = test;" + WholeGameString;
        string input3 = "int test2 = test;" + WholeGameString;
        string input4 = "int test = 1; int test2 = 1.0 + test;" + WholeGameString;

        AssertTrue(input1);

        AssertFalse<VariableNotFoundException>(input2);
        AssertFalse<VariableNotFoundException>(input3);
        AssertFalse<BinaryExpressionException>(input4);
    }
    [TestMethod]
    public void TestIfElse()
    {
        string input1 = "int test = if (TRUE) then {int x = 4; x} else {int y = 5; y};" + WholeGameString;
        string input2 = "int test = if (TRUE) then {int x = 4; x} else {x};" + WholeGameString;
        string input3 = "int test = if (TRUE) then {int x = 4; x} else {int y = 5; y}; int test2 = x;" + WholeGameString;
        string input4 = "int test = if (TRUE) then {int x = 4; x} else {int y = 5; y}; int test2 = y;" + WholeGameString;
        string input5 = "int test = if (x) then {int x = 4; x} else {int x = 4; x};" + WholeGameString;
        string input6 = "int x = 5; int test = if (x) then {int y = 4; y} else {int y = 4; y};" + WholeGameString;
        string input7 = "int test = if (TRUE) then {int x = 4; x} else {real y = 5.0; y};" + WholeGameString;

        AssertTrue(input1);

        AssertFalse<VariableNotFoundException>(input2);
        AssertFalse<VariableNotFoundException>(input3);
        AssertFalse<VariableNotFoundException>(input4);
        AssertFalse<VariableNotFoundException>(input5);
        AssertFalse<WrongTypeException>(input6);
        AssertFalse<WrongTypeException>(input7);
    }

    [TestMethod]
    public void TestElseIf()
    {
        string input1 = "int test = if (TRUE) then {int x = 4; x} else if (FALSE) then {int y = 5; y} else {int z = 6; z};" + WholeGameString;
        string input2 = "int test = if (TRUE) then {int x = 4; x} else if (FALSE) then {int y = 4; y} else {int z = 4; z};" + WholeGameString;
        string input3 = "int test = if (TRUE) then {int x = 4; x} else if (FALSE) then {x} else {x};" + WholeGameString;
        string input4 = "int test = if (TRUE) then {int x = 4; x} else if (FALSE) then {int y = 4; y} else {int z = 4; z}; int test2 = x;" + WholeGameString;
        string input5 = "int test = if (TRUE) then {int x = 4; x} else if (FALSE) then {int y = 4; y} else {int z = 4; z}; int test2 = y;" + WholeGameString;
        string input6 = "int test = if (TRUE) then {int x = 4; x} else if (FALSE) then {int y = 4; y} else {int z = 4; z}; int test2 = z;" + WholeGameString;
        string input7 = "int x = 5; int test = if (TRUE) then {int y = 5; y} else if (x) then {int z = 5; z} else {int b = 4; b};" + WholeGameString;
        string input8 = "int test = if (TRUE) then {int x = 4; x} else if (FALSE) then {real y = 6.0; y} else {int z = 6; z};" + WholeGameString;

        AssertTrue(input1);
        AssertTrue(input2);

        AssertFalse<VariableNotFoundException>(input3);
        AssertFalse<VariableNotFoundException>(input4);
        AssertFalse<VariableNotFoundException>(input5);
        AssertFalse<VariableNotFoundException>(input6);
        AssertFalse<WrongTypeException>(input7);
        AssertFalse<WrongTypeException>(input8);
    }
    [TestMethod]
    public void PrintTest()
    {
        string input1 = "int x = 5; print(x);" + WholeGameString;
        string input2 = "real x = 5.0; print(x);" + WholeGameString;
        string input3 = "print(x);" + WholeGameString;
        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse<VariableNotFoundException>(input3);
    }
    [TestMethod]
    public void TestFunctionDeclaration()
    {
        string input1 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5}" + WholeGameString;
        string input2 = "intFunction : (int x) -> int {if (TRUE) then {5} else {6}}" + WholeGameString;
        string input3 = "intFunction : (int x) -> int {int y = x + 10 * 5; 5.0}" + WholeGameString;
        string input4 = "intFunction : (int x) -> int {if (TRUE) then {5.0} else {5.0}}" + WholeGameString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse<WrongTypeException>(input3);
        AssertFalse<WrongTypeException>(input4);
    }
    [TestMethod]
    public void TestFunctionCall()
    {
        string input1 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5} int x = intFunction(5);" + WholeGameString;
        string input2 = "int a = 10; intFunction1 : (int x) -> int {int y = x + a * 5; x - 5} int x = intFunction1(5);" + WholeGameString;
        string input3 = "intFunction : (int x) -> int {intFunction2 : (int y) -> int {y + 5} intFunction2(x)} int x = intFunction(5);" + WholeGameString;
        string input4 = "intFunction2 : (int x) -> int {int y = x + 10 * 5; x - 5} int y = x;" + WholeGameString;
        string input5 = "int x = func(4,5);" + WholeGameString;
        string input6 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5} int x = intFunction(5, 0);" + WholeGameString;
        string input7 = "intFunction : (int x) -> int {int y = x + 10 * 5; x - 5} int x = intFunction(TRUE);" + WholeGameString;
        string input8 = "intFunction : (int x) -> int {intFunction2 : (int x) -> int {x + 5} intFunction2(x)} int x = intFunction2(5);" + WholeGameString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);

        AssertFalse<VariableNotFoundException>(input4);
        AssertFalse<FunctionCallException>(input5);
        AssertFalse<FunctionCallException>(input6);
        AssertFalse<FunctionCallException>(input7);
        AssertFalse<DuplicateNameException>(input8);
    }
    /*
    [TestMethod]
    public void TestMemberAccess()
    {
        string input1 = "gamestate.lastMove(\"p2\");";
        string input2 = "gamestate.turn;";
        string input3 = "str x = \"p2\"; gamestate.lastMove(x);";
        string input4 = "int x = 5; x.turn;";
        string input5 = "gamestat.turn;";
        string input6 = "int x = 5; gamestate.lastMove(x);";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertFalse<WrongTypeException>(input4);
        AssertFalse<VariableNotFoundException>(input5);
        AssertFalse<WrongTypeException>(input6);
    }
    */
    [TestMethod]
    public void TestActionDeclaration()
    {
        string input1 = "Moves = [move]; Action turn = (TRUE) then move;" + WholeGameString;
        string input2 = "bool a = TRUE; Moves = [move]; Action turn = (a) then move;" + WholeGameString;
        string input3 = "Moves = [move]; Action turn = [5, 4];" + WholeGameString;
        string input4 = "Moves = [move]; Action turn = (move, move, move);" + WholeGameString;
        string input5 = "Action turn = (TRUE) then move;" + WholeGameString;


        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse<WrongTypeException>(input3);
        AssertFalse<WrongTypeException>(input4);
        AssertFalse<VariableNotFoundException>(input5);
    }
    [TestMethod]
    public void TestStrategyDeclaration()
    {
        string input1 = "Moves = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn];" + WholeGameString;
        string input2 = "Moves = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, 5];" + WholeGameString;
        string input3 = "Strategy strat = [x];" + WholeGameString;


        AssertTrue(input1);
        AssertFalse<WrongTypeException>(input2);
        AssertFalse<VariableNotFoundException>(input3);
    }
    [TestMethod]
    public void TestPlayerDeclaration()
    {
        string input1 = "Moves = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses strat, p2 chooses strat];" + WholeGameString;
        string input2 = "Moves = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses x, p2 chooses strat];" + WholeGameString;
        string input3 = "Players p = [p1 chooses strat, p2 chooses strat];" + WholeGameString;


        AssertTrue(input1);
        AssertFalse<VariableNotFoundException>(input2);
        AssertFalse<VariableNotFoundException>(input3);
    }
    [TestMethod]
    public void TestPayoffDeclaration()
    {
        string input1 = "Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];" + WholeGameString;
        string input2 = "int x = 5; Payoff payoff = [p1 -> [1,4,0,x],p2 -> [1,x,4,2]];" + WholeGameString;
        string input3 = "bool x = TRUE; Payoff payoff = [p1 -> [1,4,0,x],p2 -> [1,x,4,2]];" + WholeGameString;
        string input4 = "Payoff payoff = (move, move, move);" + WholeGameString;


        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse<WrongTypeException>(input3);
        AssertFalse<VariableNotFoundException>(input4);
    }
    [TestMethod]
    public void TestStrategySpaceDeclaration()
    {
        string input1 = "Moves = [defect, cooperate]; Strategyspace stratspace = [(cooperate, cooperate),(defect, cooperate),(cooperate, defect),(defect, defect)];" + WholeGameString;
        string input2 = "Strategyspace stratspace = [(cooperate, cooperate),(defect, cooperate),(cooperate, defect),(defect, defect)];" + WholeGameString;
        string input3 = "int x = 5; Moves = [defect, cooperate]; Strategyspace stratspace = [(x, cooperate),(defect, cooperate),(cooperate, defect),(defect, defect)];" + WholeGameString;

        AssertTrue(input1);
        AssertFalse<VariableNotFoundException>(input2);
        AssertFalse<WrongTypeException>(input3);
    }
    [TestMethod]
    public void TestGameTuple()
    {
        string input1 = "Moves = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses strat, p2 chooses strat]; Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]; Strategyspace stratspace = [(move, move)]; Game g = (stratspace, p, payoff); run(g, 10);";
        string input2 = "Game g = (stratspace, p, payoff);" + RunString;
        string input3 = "Moves = [move]; Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]; Strategyspace stratspace = [(move, move)]; Game g = (stratspace, p, payoff);" + RunString;
        string input4 = "Moves = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses strat, p2 chooses strat]; Strategyspace stratspace = [(move, move)]; Game g = (stratspace, p, payoff);" + RunString;
        string input5 = "Moves = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses strat, p2 chooses strat]; Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]; Game g = (stratspace, p, payoff);" + RunString;
        string input6 = "Moves = [move]; Action turn = (TRUE) then move; Strategy strat = [turn, turn, turn]; Players p = [p1 chooses strat, p2 chooses strat]; Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]; Strategyspace stratspace = [(move, move)]; Game g = (stratspace, p, payoff); run(g, TRUE);";
        string input7 = "int x = 5; Moves = [move]; Action a = () then move; run(x, 10);";


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
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new ErrorListener());
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
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new ErrorListener());
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream)!;

        parser.ErrorHandler = new ErrorStrategy();
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener());

        var parseTree = parser.program();
        CustomGtlVisitor visitor = new CustomGtlVisitor();

        Exception caughtException = null!;
        string errormessage = "";

        try
        {
            _ = visitor.Visit(parseTree);
        }
        catch (Exception ex)
        {
            caughtException = ex;
            errormessage = ex.Message;
        }

        Assert.IsNull(caughtException, errormessage);
    }
}

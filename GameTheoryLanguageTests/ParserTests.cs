using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
namespace GameTheoryLanguageTests;


[TestClass]
public class ParserTests
{
    public string ActionRunString = "Moves = [cooperate, deflect]; Action testaction = () then cooperate; run(id, 4);";
    public string RunString = "run(id, 4);";
    [TestMethod]
    public void DeclarationTest()
    {
        string input1 = "int x = 5;" + ActionRunString;
        string input2 = "bool z = TRUE;" + ActionRunString;
        string input3 = "real y = 5.0;" + ActionRunString;
        string input4 = "dsa z = 5;" + ActionRunString;
        string input5 = "int x = 5" + ActionRunString;
        string input6 = "int x = int y = 5;" + ActionRunString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertFalse(input4);
        AssertFalse(input5);
        AssertFalse(input6);
    }
    [TestMethod]
    public void BinaryexpressionTest()
    {
        string input1 = "int x = -5 + 5;" + ActionRunString;
        string input2 = "int x = (-5 + ( 4 + 3 * ( 4 MOD 5 ) / 1 + 5 ) - 3);" + ActionRunString;
        string input3 = "int x = ((()());" + ActionRunString;
        string input4 = "int x = 5 - + 5;" + ActionRunString;

        AssertTrue(input1);
        AssertTrue(input2);

        AssertFalse(input3);
        AssertFalse(input4);
    }
    [TestMethod]
    public void BooleanTest()
    {
        string input1 = "bool x = TRUE && FALSE;" + ActionRunString;
        string input2 = "bool x = TRUE || FALSE;" + ActionRunString;
        string input3 = "bool x = TRUE == FALSE;" + ActionRunString;
        string input4 = "bool x = TRUE ^^ FALSE;" + ActionRunString;
        string input5 = "bool x = 1 < 3;" + ActionRunString;
        string input6 = "bool x = (TRUE && TRUE) || ((TRUE && TRUE) == TRUE);" + ActionRunString;
        string input7 = "bool x = 1 => 3;" + ActionRunString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertTrue(input4);
        AssertTrue(input5);
        AssertTrue(input6);
        AssertFalse(input7);
    }
    [TestMethod]
    public void FunctionTest()
    {
        string input1 = "function : (int a) -> int {a + 1}" + ActionRunString;
        string input2 = "function : () -> int {5}" + ActionRunString;
        string input3 = "function : (int a, bool b, real c, int d, bool e, real f) -> bool {f}" + ActionRunString;
        string input4 = "function : ( -> real {4}" + ActionRunString;
        string input5 = "function : (int a, int b, int c, int d) -> das {}" + ActionRunString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertFalse(input4);
        AssertFalse(input5);
    }
    [TestMethod]
    public void IfElseTest()
    {
        string input1 = "int x = if (TRUE) then {a} else {a};" + ActionRunString;
        string input2 = "int x = if (TRUE) then {a} else if (TRUE) then {a} else {a};" + ActionRunString;
        string input3 = "int x = if (TRUE) then {a};" + ActionRunString;
        string input4 = "int x = if (TRUE) then {a} else if (TRUE) then {a};" + ActionRunString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse(input3);
        AssertFalse(input4);
    }
    [TestMethod]
    public void PrintTest()
    {
        string input1 = "print(x);" + ActionRunString;
        string input2 = "print(\"something\");" + ActionRunString;
        string input3 = "print(func(5));" + ActionRunString;
        string input4 = "print(int x = 5;);" + ActionRunString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertFalse(input4);
    }
    [TestMethod]
    public void ActionTest()
    {
        string input1 = "Action oppCooperate = (gamestate.lastMove(\"p2\") == deflect) then cooperate;" + RunString;
        string input2 = "Action turn = (gamestate.turn == 4) then deflect;" + RunString;
        string input3 = "Actio turn = (gamestate.turn == 4) then deflect;" + RunString;
        string input4 = "Action turn = (gamestate.turn == 4) deflect;" + RunString;
        string input5 = "int turn = (gamestate.turn == 4) deflect;" + RunString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse(input3);
        AssertFalse(input4);
        AssertFalse(input5);
    }
    [TestMethod]
    public void PayoffTest()
    {
        string input1 = "Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];" + RunString;
        string input2 = "Payoff payoff = [p1 -> [x,y,z,b]];" + RunString;
        string input3 = "Payoffs payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];" + RunString;
        string input4 = "Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2];" + RunString;
        string input5 = "Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]" + RunString;
        string input6 = "payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];" + RunString;
        string input7 = "payoff payoff = [[1,4,0,2], [1,4,0,2]];" + RunString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse(input3);
        AssertFalse(input4);
        AssertFalse(input5);
        AssertFalse(input6);
        AssertFalse(input7);
    }
    [TestMethod]
    public void StrategySpaceTest()
    {
        string input1 = "Strategyspace stratspace = [(cooperate, cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];" + RunString;
        string input2 = "strategyspace stratspace = [(cooperate, cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];" + RunString;
        string input3 = "Strategyspace stratspace = [(cooperate, cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)]" + RunString;
        string input4 = "Strategyspace stratspace = [(cooperate, cooperate)(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];" + RunString;
        string input5 = "Strategyspace stratspace = [(cooperate cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];" + RunString;
        string input6 = "int stratspace = [(cooperate cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];" + RunString;

        AssertTrue(input1);
        AssertFalse(input2);
        AssertFalse(input3);
        AssertFalse(input4);
        AssertFalse(input5);
        AssertFalse(input6);
    }
    [TestMethod]
    public void StrategyTest()
    {
        string input1 = "Strategy aStrat = [turn];" + RunString;
        string input2 = "Strategy a = [turn, turn, turn, turn];" + RunString;
        string input3 = "Strat a = [turn];" + RunString;
        string input4 = "Strategy aStrat = [turn]" + RunString;
        string input5 = "Strategy aStrat = (turn);" + RunString;

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse(input3);
        AssertFalse(input4);
        AssertFalse(input5);
    }
    [TestMethod]
    public void PlayersTest()
    {
        string input1 = "Players p = [p1 chooses a, p2 chooses b, p3 chooses c];" + RunString;
        string input2 = "players p = [p1 chooses a, p2 chooses b, p3 chooses c];" + RunString;
        string input3 = "int p = [p1 chooses a, p2 chooses b, p3 chooses c];" + RunString;
        string input4 = "Players p = 1" + RunString;

        AssertTrue(input1);
        AssertFalse(input2);
        AssertFalse(input3);
        AssertFalse(input4);
    }
    [TestMethod]
    public void GameTest()
    {
        string input1 = "Game p = (a, b, c);" + RunString;
        string input2 = "game p = (a, b, c);" + RunString;
        string input3 = "Game p = 1;" + RunString;

        AssertTrue(input1);
        AssertFalse(input2);
        AssertFalse(input3);
    }

    private void AssertFalse(string program)
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(program));
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new ErrorListener());
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream)!;
        parser.ErrorHandler = new ErrorStrategy();
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener());
        var _ = Assert.ThrowsException<ParserException>(() => parser.program().ToStringTree());
    }
    private void AssertTrue(string program)
    {
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(program));
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new ErrorListener());
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream)!;
        parser.ErrorHandler = new ErrorStrategy();
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener());
        Exception caughtException = null!;
        string errormessage = "";
        try
        {
            var _ = Assert.ThrowsException<ParserException>(() => parser.program());
            errormessage = _.Message;
        }
        catch (Exception e)
        {
            caughtException = e;
        }

        Assert.IsNotNull(caughtException, errormessage);
    }
}

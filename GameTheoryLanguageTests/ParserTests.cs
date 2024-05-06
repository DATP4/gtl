using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
namespace GameTheoryLanguageTests;


[TestClass]
public class ParserTests

{
    [TestMethod]
    public void DeclarationTest()
    {
        string input1 = "int x = 5;";
        string input2 = "bool z = TRUE;";
        string input3 = "real y = 5.0;";
        string input4 = "dsa z = 5;";
        string input5 = "int x = 5";
        string input6 = "int x = int y = 5;";

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
        string input1 = "-5 + 5;";
        string input2 = "(-5 + ( 4 + 3 * ( 4 MOD 5 ) / 1 + 5 ) - 3);";
        string input3 = "((()());";
        string input4 = "5 - + 5;";

        AssertTrue(input1);
        AssertTrue(input2);

        AssertFalse(input3);
        AssertFalse(input4);
    }
    [TestMethod]
    public void BooleanTest()
    {
        string input1 = "TRUE && FALSE;";
        string input2 = "TRUE || FALSE;";
        string input3 = "TRUE == FALSE;";
        string input4 = "TRUE ^^ FALSE;";
        string input5 = "1 < 3;";
        string input6 = "(TRUE && TRUE) || ((TRUE && TRUE) == TRUE);";
        string input7 = "1 => 3;";
        string input8 = "TRUE ^^ FALSE";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertTrue(input4);
        AssertTrue(input5);
        AssertTrue(input6);
        AssertFalse(input7);
        AssertFalse(input8);
    }
    [TestMethod]
    public void FunctionTest()
    {
        string input1 = "function : (int a) -> int {a + 1;}";
        string input2 = "function : () -> int {5;}";
        string input3 = "function : (int a, bool b, real c, int d, bool e, real f) -> bool {}";
        string input4 = "function : ( -> real {4;}";
        string input5 = "function : (int a, int b, int c, int d) -> das {}";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertFalse(input4);
        AssertFalse(input5);
    }
    [TestMethod]
    public void IfElseTest()
    {
        string input1 = "if (TRUE) then {a;} else {a;};";
        string input2 = "if (TRUE) then {a;} else if (TRUE) then {a;} else {a;};";
        string input3 = "if (TRUE) {a;} else {a;};";
        string input4 = "if (TRUE) then {a;}";
        string input5 = "if (TRUE) then {a;} else if (TRUE) then {a;}";
        string input6 = "if () then {a;} else {a;};";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse(input3);
        AssertFalse(input4);
        AssertFalse(input5);
        AssertFalse(input6);
    }
    [TestMethod]
    public void ActionTest()
    {
        string input1 = "Action oppCooperate = (gamestate.opponent.lastmove == deflect) then cooperate;";
        string input2 = "Action turn = (gamestate.turn == 4) then deflect;";
        string input3 = "Actio turn = (gamestate.turn == 4) then deflect;";
        string input4 = "Action turn = (gamestate.turn == 4) deflect;";
        string input5 = "int turn = (gamestate.turn == 4) deflect;";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse(input3);
        AssertFalse(input4);
        AssertFalse(input5);
    }
    [TestMethod]
    public void PayoffTest()
    {
        string input1 = "Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];";
        string input2 = "Payoff payoff = [p1 -> [x,y,z,b]];";
        string input3 = "Payoffs payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];";
        string input4 = "Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2];";
        string input5 = "Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]";
        string input6 = "payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];";
        string input7 = "payoff payoff = [[1,4,0,2], [1,4,0,2]];";

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
        string input1 = "Strategyspace stratspace = [(cooperate, cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];";
        string input2 = "strategyspace stratspace = [(cooperate, cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];";
        string input3 = "Strategyspace stratspace = [(cooperate, cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)]";
        string input4 = "Strategyspace stratspace = [(cooperate, cooperate)(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];";
        string input5 = "Strategyspace stratspace = [(cooperate cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];";
        string input6 = "int stratspace = [(cooperate cooperate),(deflect, cooperate),(cooperate, deflect),(deflect, deflect)];";

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
        string input1 = "Strategy aStrat = [turn];";
        string input2 = "Strategy a = [turn, turn, turn, turn];";
        string input3 = "Strat a = [turn];";
        string input4 = "Strategy aStrat = [turn]";
        string input5 = "Strategy aStrat = (turn);";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse(input3);
        AssertFalse(input4);
        AssertFalse(input5);
    }
    [TestMethod]
    public void PlayersTest()
    {
        string input1 = "Players p = [p1 chooses a, p2 chooses b, p3 chooses c];";
        string input2 = "players p = [p1 chooses a, p2 chooses b, p3 chooses c];";
        string input3 = "int p = [p1 chooses a, p2 chooses b, p3 chooses c];";
        string input4 = "Players p = 1";

        AssertTrue(input1);
        AssertFalse(input2);
        AssertFalse(input3);
        AssertFalse(input4);
    }
    [TestMethod]
    public void GameTest()
    {
        string input1 = "Game p = (a, b, c);";
        string input2 = "game p = (a, b, c);";
        string input3 = "Game p = 1;";

        AssertTrue(input1);
        AssertFalse(input2);
        AssertFalse(input3);
    }
    [TestMethod]
    public void RunTest()
    {
        string input1 = "run(game, 10);";
        string input2 = "run(game, x);";
        string input3 = "run(game, 10;";

        AssertTrue(input1);
        AssertTrue(input2);
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

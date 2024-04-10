using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
namespace GameTheoryLanguageTests;


[TestClass]
public class ParserTests

{
    [TestMethod]
    public void AssignmentTest()
    {
        string input1 = "int x = 5;";
        string input2 = "bool z = TRUE;";
        string input3 = "real y = 5.0;";
        string input4 = "dsa z = 5;";
        string input5 = "int x = 5";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertFalse(input4);
        AssertFalse(input5);
    }
    [TestMethod]
    public void BooleanTest()
    {
        string input1 = "TRUE && FALSE;";
        string input2 = "TRUE || FALSE;";
        string input3 = "TRUE == FALSE;";
        string input4 = "TRUE ^^ FALSE;";
        string input5 = "1 < 3;";
        string input6 = "1 => 3;";
        string input7 = "TRUE ^^ FALSE";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertTrue(input3);
        AssertTrue(input4);
        AssertTrue(input5);
        AssertFalse(input6);
        AssertFalse(input7);
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
        string input1 = "if (TRUE) then {} else {}";
        string input2 = "if (TRUE) then {} else if (TRUE) then {} else {}";
        string input3 = "if (TRUE) {} else {}";
        //string input4 = "if (TRUE) then {}";
        //string input5 = "if (TRUE) then {} else if (TRUE) then {}";
        string input6 = "if () then {} else {}";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse(input3);
        //AssertFalse(input4);
        //AssertFalse(input5);
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
        string input1 = "Payoffs payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];";
        string input2 = "Payoffs payoff = [p1 -> [x,y,z,b]];";
        string input3 = "Payoff payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];";
        string input4 = "Payoffs payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2];";
        string input5 = "Payoffs payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]]";
        string input6 = "payoffs payoff = [p1 -> [1,4,0,2],p2 -> [1,0,4,2]];";
        string input7 = "payoffs payoff = [[1,4,0,2], [1,4,0,2]];";

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
        //string input5 = "Strategy aStrat = (turn);";

        AssertTrue(input1);
        AssertTrue(input2);
        AssertFalse(input3);
        AssertFalse(input4);
        //AssertFalse(input5);
    }
    [TestMethod]
    public void PlayersTest()
    {
        //string input1 = "Players p = [p1 chooses a, p2 chooses b, p3 chooses c];";
        string input2 = "players p = [p1 chooses a, p2 chooses b, p3 chooses c];";
        string input3 = "int p = [p1 chooses a, p2 chooses b, p3 chooses c];";
        string input4 = "Players p = 1";

        //AssertTrue(input1);
        AssertFalse(input2);
        AssertFalse(input3);
        AssertFalse(input4);
    }
    [TestMethod]
    public void GameTest()
    {
        //string input1 = "Game p = (a, b, c);";
        string input2 = "game p = (a, b, c);";
        string input3 = "Game p = 1;";

        //AssertTrue(input1);
        AssertFalse(input2);
        AssertFalse(input3);
    }
    [TestMethod]
    public void ChainArgCallTest()
    {
        //string input1 = "Game.run(10);";
        //sstring input2 = "Game.run(x);";
        //string input3 = "x.a().b();";
        string input4 = "Game.run(10;";

        //AssertTrue(input1);
        //AssertTrue(input2);
        //AssertTrue(input3);
        AssertFalse(input4);
    }


    private void AssertFalse(string program)
    {
        ICharStream stream = CharStreams.fromString(program);

        // Create a lexer
        ITokenSource lexer = new GtlLexer(stream);

        // Create a token stream
        // Change this to ITokenStream
        CommonTokenStream tokens = new(lexer);

        // Create a parser
        GtlParser parser = new(tokens)!;
        parser.ErrorHandler = new ErrorStrategy();
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener());
        var _ = Assert.ThrowsException<ParseCanceledException>(() => parser.program().ToStringTree());
    }
    private void AssertTrue(string program)
    {
        ICharStream stream = CharStreams.fromString(program);

        // Create a lexer
        ITokenSource lexer = new GtlLexer(stream);

        // Create a token stream
        // Change this to ITokenStream
        CommonTokenStream tokens = new(lexer);

        // Create a parser
        GtlParser parser = new(tokens)!;
        parser.ErrorHandler = new ErrorStrategy();
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener());
        Exception caughtException = null!;
        try
        {
            var _ = Assert.ThrowsException<ParseCanceledException>(() => parser.program());
        }
        catch (Exception e)
        {
            caughtException = e;
        }

        Assert.IsNotNull(caughtException, "exception occurred.");
        if (caughtException == null)
        {
            Console.WriteLine(parser.program().ToStringTree());
        }
    }
    public class ErrorStrategy : DefaultErrorStrategy
    {
        public override void ReportError(Parser recognizer, RecognitionException e)
        {
            throw new ParseCanceledException();
        }
    }
    public class ErrorListener : BaseErrorListener
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            Console.WriteLine(msg + " in line " + line + " position " + charPositionInLine);
            throw new ParseCanceledException();
        }
    }
}

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace GameTheoryLanguageTests;


[TestClass]
public class EndToEndTests

{
    public string WholeGameString = "Moves = [a, b, cooperate, deflect]; Action testaction = () then cooperate; Strategy teststrategy = [testaction]; Strategyspace teststratspace = [(cooperate, cooperate)]; Payoff testpayoff = [p1 -> [1]]; Players testplayers = [p1 chooses teststrategy]; Game testgame = (teststratspace, testplayers, testpayoff); run(testgame, 4);";
    public string NoMoveString = "Action testaction = () then cooperate; Strategy teststrategy = [testaction]; Strategyspace teststratspace = [(cooperate, cooperate)]; Payoff testpayoff = [p1 -> [1]]; Players testplayers = [p1 chooses teststrategy]; Game testgame = (teststratspace, testplayers, testpayoff); run(testgame, 4);";
    public static int Testcounter = 1;
    public static string Assertstring = "";
    public static string Testtype = "";
    public static string Previoustesttype = "";
    [TestMethod]
    public void SetupTests()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string gtlPath = Directory.GetParent(workingDirectory)!.Parent!.Parent!.FullName;
        string path = gtlPath + "/EndToEndTests/src/main.rs";
        File.WriteAllText(path, "#![allow(warnings)]\n mod library;\nuse library::{Action, BoolExpression, Condition, Game, GameState, Moves, Payoff, Players, Strategy, Strategyspace};\n#[cfg(test)]\nmod tests {\nuse super::*;\n");
        Createtests();
        string currentText = File.ReadAllText(path);
        File.WriteAllText(path, currentText + "}");
    }
    private void Createtests()
    {
        DeclarationTest();
        BinaryExpressionTest();
        BooleanExpressionTest();
        LogicalNotTest();
        PrintTest();
        UnaryExpressionTest();
        IfElseTest();
        FunctionTest();
        ActionDeclarationTest();
        StrategyDeclarationTest();
        PlayerDeclarationTest();
        PayoffDeclarationTest();
        StrategySpaceDeclarationTest();
        ProgramTest();
    }
    private void DeclarationTest()
    {
        Createtest("int x = 5;" + WholeGameString, "assert_eq!(x, 5)", "declaration");
        Createtest("real x = 5.0;" + WholeGameString, "assert_eq!(x, 5.0)", "declaration");
        Createtest("bool x = TRUE;" + WholeGameString, "assert_eq!(x, true)", "declaration");
    }
    private void BinaryExpressionTest()
    {
        Createtest("int test = 1 + 1 * 7;" + WholeGameString, "assert_eq!(test, 8)", "binary_expression");
        Createtest("real test = 1.5 + 1.6 / 10.0;" + WholeGameString, "assert_eq!(test, 1.66)", "binary_expression");
        Createtest("int test = -5 + ( 4 + 3 * ( 4 MOD 5 ) / 1 + 5 ) - 3;" + WholeGameString, "assert_eq!(test, 13)", "binary_expression");
        Createtest("int test = 5 MOD 3;" + WholeGameString, "assert_eq!(test, 2)", "binary_expression");
    }
    private void BooleanExpressionTest()
    {
        Createtest("bool test1 = TRUE && FALSE;" + WholeGameString, "assert_eq!(test1, false)", "boolean_expression");
        Createtest("bool test1 = 1 > 2;" + WholeGameString, "assert_eq!(test1, false)", "boolean_expression");
        Createtest("bool test1 = 1 != 0;" + WholeGameString, "assert_eq!(test1, true)", "boolean_expression");
        Createtest("bool test1 = 1 <= 2;" + WholeGameString, "assert_eq!(test1, true)", "boolean_expression");
        Createtest("bool test1 = (TRUE && TRUE) || ((TRUE && TRUE) == TRUE);" + WholeGameString, "assert_eq!(test1, true)", "boolean_expression");
    }
    private void LogicalNotTest()
    {
        Createtest("bool test1 = !(TRUE && FALSE);" + WholeGameString, "assert_eq!(test1, true)", "logical_not");
        Createtest("bool test1 = !(1 > 2);" + WholeGameString, "assert_eq!(test1, true)", "logical_not");
        Createtest("bool test1 = !(1 != 0);" + WholeGameString, "assert_eq!(test1, false)", "logical_not");
        Createtest("bool test1 = !(1 <= 2);" + WholeGameString, "assert_eq!(test1, false)", "logical_not");
        Createtest("bool test1 = !(!(TRUE && TRUE) || ((TRUE && TRUE) == TRUE));" + WholeGameString, "assert_eq!(test1, false)", "logical_not");
    }
    private void PrintTest()
    {
        // This test just makes sure the file compiles with print statements
        Createtest("int x = 1; real y = 2.0; bool z = TRUE; print(x); print(y); print(z);" + WholeGameString, "assert_eq!(1, 1)", "print");
    }
    private void UnaryExpressionTest()
    {
        Createtest("int x = -5;" + WholeGameString, "assert_eq!(x, -5)", "unary_expression");
        Createtest("real x = -5.0;" + WholeGameString, "assert_eq!(x, -5.0)", "unary_expression");
        Createtest("int x = -5 - -5;" + WholeGameString, "assert_eq!(x, 0)", "unary_expression");
        Createtest("int x = -5 + -5;" + WholeGameString, "assert_eq!(x, -10)", "unary_expression");
        Createtest("int x = -5; int y = -x;" + WholeGameString, "assert_eq!(y, 5)", "unary_expression");
    }
    private void IfElseTest()
    {
        Createtest("int y = if (TRUE) then {int x = 4; x} else {int x = 5; x};" + WholeGameString, "assert_eq!(y, 4)", "if_else");
        Createtest("int y = if (FALSE) then {int x = 4; x} else {int x = 5;x };" + WholeGameString, "assert_eq!(y, 5)", "if_else");
        Createtest("int y = if (TRUE) then {int x = 4; x} else if (TRUE) then {int x = 5; x} else {int x = 6; x};" + WholeGameString, "assert_eq!(y, 4)", "if_else");
        Createtest("int y = if (FALSE) then {int x = 4; x} else if (TRUE) then {int x = 5; x} else {int x = 6; x};" + WholeGameString, "assert_eq!(y, 5)", "if_else");
        Createtest("int y = if (FALSE) then {int x = 4; x} else if (FALSE) then {int x = 5; x} else {int x = 6; x};" + WholeGameString, "assert_eq!(y, 6)", "if_else");
    }
    private void FunctionTest()
    {
        Createtest("int_function : (int x) -> int {int y = x + 10 * 5; y - 5} int x = int_function(5);" + WholeGameString, "assert_eq!(x, 50)", "function");
        Createtest("int a = 10; int_function1 : (int x) -> int {int y = x + a * 5; y - 5} int x = int_function1(5);" + WholeGameString, "assert_eq!(x, 50)", "function");
        Createtest("int_function : (int x) -> int {int_function2 : (int z) -> int {z + 5} int y = int_function2(x); y} int x = int_function(5);" + WholeGameString, "assert_eq!(x, 10)", "function");
        Createtest("int a = 10 + 5; int b = 5 + 13; int c = a * b; int_function : (int x) -> int {c + x} int x = int_function(10);" + WholeGameString, "assert_eq!(x, 280)", "function");
    }
    private void ActionDeclarationTest()
    {
        Createtest("Moves = [a, b, cooperate, deflect]; Action turn = (TRUE) then a;" + NoMoveString, "assert_eq!(turn.act_move, Moves::a);\nlet Condition::Expression(expr) = turn.condition;\nassert_eq!((expr.b_val)(&gamestate), true)", "action");
        Createtest("Moves = [a, b, cooperate, deflect]; Action turn = (gamestate.turn == 1) then a;" + NoMoveString, "assert_eq!(turn.act_move, Moves::a);\nlet Condition::Expression(expr) = turn.condition;\nassert_eq!((expr.b_val)(&gamestate), false);\ngamestate.turn = 1;\nassert_eq!((expr.b_val)(&gamestate), true);", "action");
    }
    private void StrategyDeclarationTest()
    {
        Createtest("Moves = [a, b, cooperate, deflect]; Action turn = (TRUE) then a; Strategy strat = [turn];" + NoMoveString, "assert_eq!(strat.strat[0].act_move, Moves::a)", "strategy");
    }
    private void PlayerDeclarationTest()
    {
        Createtest("Moves = [a, b, cooperate, deflect]; Action turn = (TRUE) then a; Strategy strat = [turn]; Players p = [p1 chooses strat];" + NoMoveString, "assert_eq!(p.pl_and_strat[0].1.strat[0].act_move, Moves::a)", "player");
    }
    private void PayoffDeclarationTest()
    {
        Createtest("Payoff p = [p1 -> [1, 2], p2 -> [2, 1]];" + WholeGameString, "assert_eq!(p.matrix[0][0], 1);\nassert_eq!(p.matrix[0][1], 2);\nassert_eq!(p.matrix[1][0], 2);\nassert_eq!(p.matrix[1][1], 1)", "payoff");
    }
    private void StrategySpaceDeclarationTest()
    {
        Createtest("Moves = [a, b, cooperate, deflect]; Strategyspace stratspace = [(a, b), (b, a), (a, a), (b, b)];" + NoMoveString, "assert_eq!(stratspace.matrix[0], Moves::a);\nassert_eq!(stratspace.matrix[1], Moves::b);\nassert_eq!(stratspace.matrix[2], Moves::b);\nassert_eq!(stratspace.matrix[3], Moves::a);\nassert_eq!(stratspace.matrix[4], Moves::a);\nassert_eq!(stratspace.matrix[5], Moves::a);\nassert_eq!(stratspace.matrix[6], Moves::b);\nassert_eq!(stratspace.matrix[7], Moves::b)", "strategyspace");
    }
    private void ProgramTest()
    {
        string test1 = Readtest("test1");
        Createtest(test1, "assert_eq!(finishedgame.turn, 6);\nassert_eq!(finishedgame.player_score(&\"p1\".to_string()), 17);\nassert_eq!(finishedgame.player_score(&\"p2\".to_string()), 1)", "program");
        string test2 = Readtest("test2");
        Createtest(test2 + WholeGameString, "assert_eq!(a, 5);\nassert_eq!(b, 1);\nassert_eq!(c, 17)", "program");
        string test3 = Readtest("test3");
        Createtest(test3, "assert_eq!(finishedgame.turn, 6);\nassert_eq!(finishedgame.player_score(&\"p1\".to_string()), 12);\nassert_eq!(finishedgame.player_score(&\"p2\".to_string()), 7);\nassert_eq!(finishedgame.player_score(&\"p3\".to_string()), 7)", "program");
    }
    private void Createtest(string input, string output, string testtype)
    {
        Assertstring = output;
        Testtype = testtype;
        GtlLexer lexer = new GtlLexer(CharStreams.fromString(input));
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new ErrorListener());
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        GtlParser parser = new GtlParser(tokenStream)!;

        parser.ErrorHandler = new ErrorStrategy();
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener());

        var parseTree = parser.program();
        CustomGtlVisitor visitor = new CustomGtlVisitor();
        _ = visitor.Visit(parseTree);
        TestVisitor transvisitor = new TestVisitor();
        _ = transvisitor.Visit(parseTree);
    }
    private string Readtest(string name)
    {
        string workingDirectory = Environment.CurrentDirectory;
        string path = Directory.GetParent(workingDirectory)!.Parent!.Parent!.FullName;
        return File.ReadAllText(path + "/EndToEndTests/programtests/" + name + ".gtl");
    }
    class TestVisitor : TransVisitor
    {
        public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
        {
            EnterScope(new Scope());
            string retString = null!;
            if (!Testtype.Equals(Previoustesttype))
            {
                Testcounter = 1;
            }
            Outputfile.Add($"#[test]\nfn {Testtype}_test_{Testcounter}()" + "{\n");
            Testcounter += 1;
            Outputfile.Add("let mut gamestate: GameState = GameState::new();\n");
            Previoustesttype = Testtype;
            // Program consists of statements only, so we iterate them
            foreach (var stmt in context.statement())
            {
                retString += Visit(stmt);
                retString += "\n";
            }
            foreach (var game_stmt in context.game_variable_declaration())
            {
                retString += Visit(game_stmt);
                retString += "\n";
            }
            foreach (var game_fun in context.game_functions())
            {
                retString += Visit(game_fun);
                retString += "\n";
            }
            // Everything we visit but this, will return a string. We add it to our output rust file
            if (retString != null)
            {
                Outputfile.Add(retString);
            }
            Outputfile.Add(Assertstring);
            Outputfile.Add("\n}\n");
            TestGtlCFile writer = new TestGtlCFile();
            writer.PrintFileToOutput(Outputfile);
            ExitScope();
            return null!;
        }
        public override object VisitGame_functions([NotNull] GtlParser.Game_functionsContext context)
        {
            string returnString = "let finishedgame = ";
            returnString += "Game::run(&mut ";
            returnString += $"{context.ID().GetText()}, ";
            string val = (string)Visit(context.expr());
            returnString += "&mut " + val + ");";
            return returnString;
        }
        public override void WriteToMoves(List<string> moves)
        {
            TestGtlCFile writer = new TestGtlCFile();
            writer.PrintMovesToFile(moves);
        }
    }
    class TestGtlCFile
    {
        public void PrintFileToOutput(List<string> file)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string gtlPath = Directory.GetParent(workingDirectory)!.Parent!.Parent!.FullName;
            string path = gtlPath + "/EndToEndTests/src/main.rs";
            List<string> authors = file;
            string currentText = File.ReadAllText(path);
            foreach (string line in authors)
            {
                currentText += line;
            }
            File.WriteAllText(path, currentText);
            //File.Delete(filepath); 
        }
        public void PrintMovesToFile(List<string> moves)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string gtlPath = Directory.GetParent(workingDirectory)!.Parent!.Parent!.FullName;
            string filepath = gtlPath + "/EndToEndTests/src/library/movs.rs";
            List<string> authors = moves;
            File.WriteAllText(filepath, "");
            File.WriteAllLines(filepath, authors);
        }
    }
}

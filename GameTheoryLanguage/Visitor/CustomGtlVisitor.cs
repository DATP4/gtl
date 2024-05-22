using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

using Antlr4.Runtime.Misc;

public class CustomGtlVisitor : GtlBaseVisitor<object>
{
    private Stack<Scope> ScopeStack { get; } = new Stack<Scope>();
    private readonly Objecttable _objecttable = new Objecttable();
    // This is the first function that is run when the typechecker is used, afterwards it follows a depth-first search algorithm to traverse the program
    public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
    {
        // Objecttable is cleared since it is not cleared automatically between tests
        _objecttable.Clear();
        // Adds the gamestate object and its children to the objecttable
        _objecttable.Add("gamestate", [["opponent", "turn"], ["object", "int"]]);
        _objecttable.Add("opponent", [["lastmove"], ["move"]]);
        EnterScope(new Scope());
        AddLastMove();
        // Adds the gamestate object to the vtable
        ScopeStack.Peek().AddVariable("gamestate", "object");
        _ = base.VisitProgram(context);
        ExitScope();
        return null!;
    }

    public override object VisitStatement([NotNull] GtlParser.StatementContext context)
    {
        // Visit each statement indiviually and type checks it
        _ = base.VisitStatement(context);
        return null!;
    }

    public override object VisitBlock([NotNull] GtlParser.BlockContext context)
    {
        EnterScope(new Scope());
        // Visits each function and declaration in the block and adds them to the vtable and ftable
        foreach (var dec in context.declaration())
        {
            _ = Visit(dec);
        }

        string type = (string)Visit(context.expr());
        ExitScope();
        return type;
    }

    public override object VisitVariable_dec([NotNull] GtlParser.Variable_decContext context)
    {
        // Checks if the type declaration and the type of the value matches
        // and also if the id declaration of the variable has already been used, if not the variable gets added to the vtable
        string type = context.type().GetText();
        string variableID = context.ID().GetText();
        string valueType = (string)Visit(context.expr());
        if (!type.Equals(valueType))
        {
            throw new DeclarationException(type, valueType);
        }
        if (ScopeStack.Peek().VtableContains(variableID))
        {
            throw new DeclarationException($"Variable {variableID} already exists");
        }
        ScopeStack.Peek().AddVariable(variableID, type);
        // This return the type of the variable being declared, as it is needed to typecheck functions and if/else expressions
        return type;
    }
    public override object VisitFunction([NotNull] GtlParser.FunctionContext context)
    {
        // First the function is added to the function table, this is done before typechecking the function as it is required for the function to be called recursively
        string functionId = context.ID().GetText();
        GtlParser.TypeContext[] typeContext = context.arg_def().type();
        string[] stringArray = [];
        foreach (var type in typeContext)
        {
            stringArray = stringArray.Append(type.GetText()).ToArray();
        }
        string[][] functionTypes = [stringArray, [context.type().GetText()]];
        ScopeStack.Peek().AddFunction(functionId, functionTypes);
        EnterScope(new Scope());

        // This visits the node in the parsetree where the functions arguments are defined, which adds them to the vtable
        _ = Visit(context.arg_def());

        // Visits the last expression to get the type of the returned value
        string lastExpressionType = (string)Visit(context.block());

        string func_type = context.type().GetText();

        if (!lastExpressionType.Equals(func_type))
        {
            throw new WrongTypeException("Function declaration", func_type, lastExpressionType);
        }
        ExitScope();

        return null!;
    }
    public override object VisitIfElseExpr([NotNull] GtlParser.IfElseExprContext context)
    {
        return Visit(context.ifElse());
    }
    public override object VisitIfElse(GtlParser.IfElseContext context)
    {
        // First checks if the expression used in the if statement is a boolean
        string exprtype = (string)Visit(context.expr());
        if (!exprtype.Equals("bool"))
        {
            throw new WrongTypeException("If statement", "bool", exprtype);
        }
        // Visits the functions and declarations to add to vtable and ftable 
        // and returns the type of the expression

        string ifLastStatement = (string)Visit(context.block());
        string ifElseLastStatement;

        // If there is 1 or more if else statements, they are all visitted, and it is checked if they have the same return type as the if statement
        if (context.elseif() != null)
        {
            foreach (GtlParser.ElseifContext elseifcontext in context.elseif())
            {
                ifElseLastStatement = (string)Visit(elseifcontext);
                if (ifLastStatement != ifElseLastStatement)
                {
                    throw new WrongTypeException("Elseif statement", ifLastStatement, ifElseLastStatement);
                }
            }

        }
        else
        { return (string)Visit(context.block().expr()); }
        // finally the else statement is visitted and we check whether or not it has the same return type as the if statement
        ifElseLastStatement = (string)Visit(context.@else());
        if (ifLastStatement != ifElseLastStatement)
        {
            throw new WrongTypeException("Else statement", ifLastStatement, ifElseLastStatement);
        }
        return ifLastStatement;
    }
    public override object VisitElseif([NotNull] GtlParser.ElseifContext context)
    {
        // Works in the same way as the if statement
        string exprtype = (string)Visit(context.expr());
        if (!exprtype.Equals("bool"))
        {
            throw new WrongTypeException("Elseif statement", "bool", exprtype);
        }

        return (string)Visit(context.block());
    }


    public override object VisitElse([NotNull] GtlParser.ElseContext context)
    {
        // Works in the same way as the if statement
        return (string)Visit(context.block());
    }

    /*
    -----------------------------------------------------------------------------Expressions-----------------------------------------------------------------------------------------
    */
    public override object VisitLiteralExpr(GtlParser.LiteralExprContext context)
    {
        string val = context.GetText();

        if (int.TryParse(val, out _))
        {
            return "int";
        }

        if (double.TryParse(val, CultureInfo.InvariantCulture, out _))
        {
            return "real";
        }

        if (bool.TryParse(val, out _))
        {
            return "bool";
        }
        if (val[0].Equals('"') && val[val.Length - 1].Equals('"'))
        {
            return "str";
        }
        throw new NotSupportedException($"literal expression expected type but received {val}");
    }
    public override object VisitBinaryExpr(GtlParser.BinaryExprContext context)
    {
        // Checks that both expressions used have the same type, and if they are both reals checks that the MOD operator isn't used
        // If any non-defined operator is used it would be caught in the parser so we can largely ignore operators here
        string left = (string)Visit(context.expr(0));
        string right = (string)Visit(context.expr(1));
        if (left.Equals("int") && right.Equals("int"))
        {
            return "int";
        }
        if (left.Equals("real") && right.Equals("real"))
        {
            if (context.op.Text.Equals("MOD"))
            {
                throw new WrongTypeException("Modulus", "int", "real");
            }
            return "real";
        }
        throw new BinaryExpressionException(left, right);
    }
    public override object VisitBooleanExpr(GtlParser.BooleanExprContext context)
    {
        // Cheks that both expressions used have the same type and the operator used is defined for that type
        string left = (string)Visit(context.expr(0));
        string right = (string)Visit(context.expr(1));
        string op = context.op.Text;
        if (left.Equals("bool") && right.Equals("bool") &&
        (op == "==" || op == "!=" || op == "&&" || op == "||" || op == "^^"))
        {
            return "bool";
        }
        if (left.Equals("int") && right.Equals("int") &&
        (op == "==" || op == "<" || op == ">" || op == "<=" || op == ">=" || op == "!="))
        {
            return "bool";
        }
        if (left.Equals("real") && right.Equals("real") &&
        (op == "==" || op == "<" || op == ">" || op == "<=" || op == ">=" || op == "!="))
        {
            return "bool";
        }
        if (left.Equals("move") && right.Equals("move") && op == "==")
        {
            return "bool";
        }
        throw new BooleanExpressionException(left, right);
    }
    public override object VisitLogicalNotExpr(GtlParser.LogicalNotExprContext context)
    {
        string expr = (string)Visit(context.expr());
        if (expr.Equals("bool"))
        {
            return "bool";
        }
        throw new WrongTypeException("Logical not expression", "bool", expr);
    }
    public override object VisitGame_functions([NotNull] GtlParser.Game_functionsContext context)
    {
        string gametype = (string)VisitId(context.ID().GetText());
        if (!gametype.Equals("Game"))
        {
            throw new WrongTypeException("Game function", "Game", gametype);
        }
        string turnsType = (string)Visit(context.expr());
        if (!turnsType.Equals("int"))
        {
            throw new WrongTypeException("Game function", "int", turnsType);
        }
        return null!;
    }
    public override object VisitMemberExpr(GtlParser.MemberExprContext context)
    {
        // first we check if the member we are trying to access is an object and it is in the objecttable
        string id = context.ID().GetText();
        string type = (string)VisitId(id);
        if (!type.Equals("object"))
        {
            throw new WrongTypeException($"member expression expected type object from {id} but received {type}");
        }
        if (!_objecttable.Contains(id))
        {
            throw new MemberAccessException("Object not found");
        }
        GtlParser.Member_accessContext[] membercontexts = context.member_access();
        // checks if the member that is being accessed is a part of the object in the objecttable
        if (membercontexts.Length > 1)
        {
            string nextid = membercontexts[0].ID().GetText();
            string[][] objarray = _objecttable.Find(id);
            int k = -1;
            for (int i = 0; i < objarray[0].Length; i++)
            {
                if (objarray[0][i].Equals(nextid))
                {
                    k = i;
                }
            }
            if (k == -1)
            {
                throw new MemberAccessException($"couldnt find entry {nextid} in objecttable");
            }
            if (!objarray[1][k].Equals("object"))
            {
                throw new WrongTypeException($"Expected type object from member {objarray[0][k]} but received {objarray[1][k]}");
            }
        }
        // checks that each member being accesses is a part of the previous members object table
        for (int i = 0; i < membercontexts.Length - 2; i++)
        {
            string memberid = membercontexts[i].ID().GetText();
            if (!_objecttable.Contains(memberid))
            {
                throw new MemberAccessException($"couldnt find entry {memberid} in objecttable");
            }
        }
        string[][] objectarray;
        if (membercontexts.Length == 1)
        {
            objectarray = _objecttable.Find(id);
        }
        else
        {
            objectarray = _objecttable.Find(membercontexts[membercontexts.Length - 2].ID().GetText());
        }
        int j = -1;
        for (int i = 0; i < objectarray[0].Length; i++)
        {
            if (objectarray[0][i].Equals(membercontexts[membercontexts.Length - 1].ID().GetText()))
            {
                j = i;
            }
        }
        if (j == -1)
        {
            throw new MemberAccessException($"couldnt find entry {membercontexts[membercontexts.Length - 1].ID().GetText()} in objecttable");
        }
        return objectarray[1][j];
    }
    public override object VisitMember_access([NotNull] GtlParser.Member_accessContext context)
    {
        return base.VisitMember_access(context);
    }

    public override object VisitArgCallExpr(GtlParser.ArgCallExprContext context)
    {
        // Firstly checks that the function being called has been declared in the current scope
        string functionId = context.ID().GetText();
        if (!ScopeStack.Peek().FtableContains(functionId))
        {
            throw new FunctionCallException($"{functionId} not found in Function table");
        }
        string[] stringArray = [];
        // finds the types of all the input parameters used
        foreach (GtlParser.ExprContext exprcontext in context.arg_call().expr())
        {
            stringArray = stringArray.Append(Visit(exprcontext).ToString()).ToArray()!;
        }
        // finds the input types the function expects and the functions return type in the ftable
        string[][] functionTypes = ScopeStack.Peek().FtableFind(functionId);
        // Checks that the function is given the correct amount of inputs
        if (stringArray.Length != functionTypes[0].Length)
        {
            throw new FunctionCallException("Not the correct amount of input parameters in function");
        }
        // checks that each input given has the correct type
        for (int i = 0; i < stringArray.Length; i++)
        {
            if (!stringArray[i].Equals(functionTypes[0][i]))
            {
                throw new FunctionCallException(functionTypes[0][i], stringArray[i]);
            }
        }
        // returns the functions returntype
        return ScopeStack.Peek().FtableFind(functionId)[1][0];
    }
    public override object VisitParExpr(GtlParser.ParExprContext context)
    {
        return (string)Visit(context.expr());
    }
    public override object VisitGame_utility_function([NotNull] GtlParser.Game_utility_functionContext context)
    {
        // Checks that the input given is an array of ints, if so returns the type utilarray
        string arraytype = (string)Visit(context.array());
        if (!arraytype.Equals("intarray"))
        {
            throw new WrongTypeException("Utility function", "intarray", arraytype);
        }
        return "utilarray";
    }
    public override object VisitIdExpr(GtlParser.IdExprContext context)
    {
        string id = context.ID().GetText();
        return VisitId(id);
    }
    public override object VisitArg_call([NotNull] GtlParser.Arg_callContext context)
    {
        GtlParser.ExprContext[] expr = context.expr();
        string[] stringArray = [];
        foreach (GtlParser.ExprContext expression in expr)
        {
            var _ = stringArray.Append(Visit(expression).ToString());
        }
        return base.VisitArg_call(context);
    }
    public override object VisitArg_def([NotNull] GtlParser.Arg_defContext context)
    {
        GtlParser.TypeContext[] types = context.type();
        Antlr4.Runtime.Tree.ITerminalNode[] ids = context.ID();
        // adds each input the the vtable of the functions scope as long as the id has not already been used
        for (int i = 0; i < types.Length; i++)
        {
            string type = types[i].GetText();
            string id = ids[i].GetText();
            if (ScopeStack.Peek().VtableContains(id))
            {
                throw new DuplicateNameException($"{id} is already declared");
            }
            ScopeStack.Peek().AddVariable(id, type);
        }
        return null!;
    }

    public override object VisitMethod_access([NotNull] GtlParser.Method_accessContext context)
    {
        string id = context.ID().GetText();
        if (id.Equals("lastMove"))
        {
            string inputtype = (string)Visit(context.arg_call());
            if (inputtype.Equals("str"))
            {
                return "move";
            }
            else
            {
                throw new WrongTypeException("method access", "str", inputtype);
            }
        }

        return id switch
        {
            "turn" => "int",
            "moveAtTurn" => "move",
            "playerScore" => "int",
            _ => throw new NotSupportedException($"Method {id} not found"),
        };
    }

    public override object VisitPlayer([NotNull] GtlParser.PlayerContext context)
    {
        // checks that the player is given a strategy and then the player is added to the vtable
        string id = context.ID(0).GetText();
        string type = (string)VisitId(context.ID(1).GetText());
        if (!type.Equals("Strategy"))
        {
            throw new DeclarationException("Strategy", type);
        }
        ScopeStack.Peek().AddVariable(id, "player");
        return "player";
    }
    public override object VisitGame_variable_declaration([NotNull] GtlParser.Game_variable_declarationContext context)
    {
        // firstly checks if the declaration declares moves, if so said moves are added to the vtable
        string gametype = "";
        string movesType = "";
        if (context.game_type() != null)
        {
            gametype = context.game_type().GetText();
        }
        if (context.T_MOVES() != null)
        {
            movesType = context.T_MOVES().GetText();
        }

        if (movesType != "" && movesType.Equals("Moves"))
        {
            GtlParser.Array_typeContext[] moves = context.array().array_type();
            foreach (GtlParser.Array_typeContext move in moves)
            {
                ScopeStack.Peek().AddVariable(move.GetText(), "move");
            }
            return null!;
        }
        // if the declaration is not a move declaration, the type of the value given and the declaration type are checked, if they are equal the id is added to the vtable
        string valuetype = (string)Visit(context.game_expr());
        if (!gametype.Equals(valuetype))
        {
            throw new WrongTypeException("Game declaration", gametype, valuetype);
        }
        ScopeStack.Peek().AddVariable(context.ID().GetText(), gametype);
        return null!;
    }
    public override object VisitGame_expr([NotNull] GtlParser.Game_exprContext context)
    {
        if (context.array() != null)
        {
            return Visit(context.array());
        }
        if (context.game_tuple() != null)
        {
            return Visit(context.game_tuple());
        }
        if (context.action() != null)
        {
            return Visit(context.action());
        }
        //this codepath should never be reached, as the parser would have already thrown an error
        return null!;
    }
    public override object VisitAction([NotNull] GtlParser.ActionContext context)
    {
        // firstly checks that the expressions given is a boolean, afterwards checks that the action evaluates to a move
        string expressiontype;
        if (context.expr() == null)
        {
            expressiontype = "bool";
        }
        else
        {
            expressiontype = (string)Visit(context.expr());
        }
        if (!expressiontype.Equals("bool"))
        {
            throw new WrongTypeException("Action", "bool", expressiontype);
        }
        string movetype = (string)Visit(context.move());
        if (!movetype.Equals("move"))
        {
            throw new WrongTypeException("Action", "bool", movetype);
        }
        return "Action";
    }
    public override object VisitArray([NotNull] GtlParser.ArrayContext context)
    {
        // firstly checks that all entrys in the array have the same type
        string arraytype = (string)Visit(context.array_type(0));
        for (int i = 1; i < context.array_type().Length; i++)
        {
            string valuetype = (string)Visit(context.array_type(i));
            if (!arraytype.Equals(valuetype))
            {
                throw new WrongTypeException("Array", arraytype, valuetype);
            }
        }
        // lastly returns the correct type based on what type of entrys the array consists of
        if (arraytype.Equals("utilarray"))
        {
            return "Payoff";
        }
        if (arraytype.Equals("player"))
        {
            return "Players";
        }
        if (arraytype.Equals("Action"))
        {
            return "Strategy";
        }
        if (arraytype.Equals("tuple"))
        {
            return "Strategyspace";
        }
        return arraytype + "array";
    }
    public override object VisitTuple([NotNull] GtlParser.TupleContext context)
    {
        // checks that each entry in the tuple is a move
        foreach (Antlr4.Runtime.Tree.ITerminalNode move in context.ID())
        {
            string type = (string)VisitId(move.GetText());
            if (!type.Equals("move"))
            {
                throw new WrongTypeException("Tuple", "move", type);
            }
        }
        return "tuple";
    }
    public override object VisitMove([NotNull] GtlParser.MoveContext context)
    {
        return VisitId(context.ID().GetText());
    }
    public override object VisitGame_tuple([NotNull] GtlParser.Game_tupleContext context)
    {
        // checks that the inputs in the game tuple are a strategyspace, a player array and a payoff-matrix
        string stratspacetype = (string)VisitId(context.ID(0).GetText());
        string playerlisttype = (string)VisitId(context.ID(1).GetText());
        string payofftype = (string)VisitId(context.ID(2).GetText());
        if (!stratspacetype.Equals("Strategyspace"))
        {
            throw new WrongTypeException("Game tuple", "Strategyspace", stratspacetype);
        }
        if (!playerlisttype.Equals("Players"))
        {
            throw new WrongTypeException("Game tuple", "Players", playerlisttype);
        }
        if (!payofftype.Equals("Payoff"))
        {
            throw new WrongTypeException("Game tuple", "Payoff", payofftype);
        }
        return "Game";
    }
    public override object VisitUnaryExpr([NotNull] GtlParser.UnaryExprContext context)
    {
        // checks that the expression is of type int or real
        string exprtype = (string)Visit(context.expr());
        if (!exprtype.Equals("int") && !exprtype.Equals("real"))
        {
            throw new WrongTypeException("Unary expression", "int or type real", exprtype);
        }
        return exprtype;
    }
    public override object VisitPrint([NotNull] GtlParser.PrintContext context)
    {
        _ = Visit(context.expr());
        return null!;
    }
    private object VisitId(string id)
    {
        // checks the vtable for said id and returns its type, throws error if the id is not found
        if (ScopeStack.Peek().VtableContains(id))
        {
            return ScopeStack.Peek().VtableFind(id);
        }
        throw new VariableNotFoundException($"Variable {id} not found");
    }
    // Methods to support scope handling
    private void EnterScope(Scope scope) // TODO: Add FTable
    {
        if (ScopeStack.Count() != 0) // Ensuring that the stack isn't empty as getCurrentScope() would return an error
        {
            foreach (var variable in GetCurrentScope().Variables) // Copying the variables from the parent scope to the child scope
            {
                scope.AddVariable(variable.Key, variable.Value);
            }
            foreach (var function in GetCurrentScope().Functions) // Copying the variables from the parent scope to the child scope
            {
                scope.AddFunction(function.Key, function.Value);
            }
        }
        ScopeStack.Push(scope);
    }
    private void ExitScope()
    {
        _ = ScopeStack.Pop();
    }
    private Scope GetCurrentScope()
    {
        return ScopeStack.Peek();
    }
    private void AddLastMove()
    {
        string[] input = ["str"];
        string[] output = ["move"];
        ScopeStack.Peek().AddFunction("lastMove", [input, output]);
    }
    /*
    public string GetTypeFromStatement(GtlParser.StatementContext ctx)
    {
        string type = "";

        if (ctx.expr() != null)
        {
            type = (string)Visit(ctx.expr());
            return type;
        }
        if (ctx.declaration() != null)
        {
            return ctx.declaration().type().GetText();
        }
        else if (ctx.@if() != null)
        {
            foreach (var stmt in ctx.@if().statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            return type;
        }
        else if (ctx.@if().@else() != null)
        {
            foreach (var stmt in ctx.@if().@else().statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            return type;
        }
        else if (ctx.@if().elseif() != null)
        {
            foreach (var stmt in ctx.@if().elseif().statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            return type;

        }
        else
        {
            throw new NotSupportedException("Dis statement is fcked");
        }
    }

    public string GetTypeFromElseIf(GtlParser.ElseifContext ctx, string tp)
    {
        string type = "";

        if (ctx.elseif() != null)
        {
            foreach (var stmt in ctx.@elseif().statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            if (type != tp)
            {
                throw new NotSupportedException("Type in elseif doesn't match expected return type of function");
            }
            return GetTypeFromElseIf(ctx.elseif(), tp);
        }
        else if (ctx.@else() != null)
        {
            foreach (var stmt in ctx.@else().statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            if (type != tp)
            {
                throw new NotSupportedException("Type in else doesn't match expected return type of function");
            }
            return type;
        }
        else if (ctx.statement() != null)
        {
            foreach (var stmt in ctx.statement())
            {
                type = GetTypeFromStatement(stmt);
            }
            if (type != tp)
            {
                throw new NotSupportedException("Type in elseif statement doesn't match return type of function");
            }
            return type;
        }
        else
        {
            throw new NotSupportedException("Error in else if statement");
        }
    }

    public bool CheckElseInElseIf(GtlParser.ElseifContext ctx)
    {
        bool check = false;

        if (ctx.elseif() != null)
        {
            return CheckElseInElseIf(ctx.elseif());
        }
        else if (ctx.@else() != null)
        {
            check = true;
            return check;
        }
        else
        {
            return check;
        }
    }
    */
}


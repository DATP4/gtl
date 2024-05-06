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

    public override object VisitProgram([NotNull] GtlParser.ProgramContext context)
    {
        _objecttable.Clear();
        _objecttable.Add("gamestate", [["opponent", "turn"], ["object", "int"]]);
        _objecttable.Add("opponent", [["lastmove"], ["move"]]);
        EnterScope(new Scope());
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

    public override object VisitDeclaration([NotNull] GtlParser.DeclarationContext context)
    {
        // Check if the type declaration and the type of the value matches
        // and if the variable is already declared, otherwise adding it to the variable table
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
        return type;
    }
    public override object VisitFunction([NotNull] GtlParser.FunctionContext context)
    {
        // 
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

        _ = Visit(context.arg_def());

        string lastExpressionType = "";

        string func_type = context.type().GetText();
        foreach (var stmt in context.statement())
        {
            if (stmt.expr() != null)
            {
                lastExpressionType = (string)Visit(stmt.expr());
            }
            else if (stmt.declaration() != null)
            {
                lastExpressionType = (string)Visit(stmt.declaration());
                continue;
            }
            else if (stmt.function() != null)
            {
                _ = Visit(stmt.function());
                continue;
            }
            else if (stmt.game_variable_declaration() != null)
            {
                _ = Visit(stmt.game_variable_declaration());
                continue;
            }
            else if (stmt.game_functions() != null)
            {
                _ = Visit(stmt.game_functions());
                continue;
            }
        }

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
        string exprtype = (string)Visit(context.expr());
        if (!exprtype.Equals("bool"))
        {
            throw new WrongTypeException("If statement", "bool", exprtype);
        }
        EnterScope(new Scope());
        string ifLastStatement = "";
        string ifElseLastStatement = "";
        foreach (var stmt in context.statement())
        {
            if (stmt.expr() != null)
            {
                ifLastStatement = (string)Visit(stmt.expr());
            }
            else if (stmt.declaration() != null)
            {
                ifLastStatement = (string)Visit(stmt.declaration());
            }
            else if (stmt.function() != null)
            {
                _ = Visit(stmt.function());
            }
            else if (stmt.game_variable_declaration != null)
            {
                _ = Visit(stmt.game_variable_declaration());
            }
            else if (stmt.game_functions() != null)
            {
                _ = Visit(stmt.game_functions());
            }
        }
        ExitScope();
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
        ifElseLastStatement = (string)Visit(context.@else());
        if (ifLastStatement != ifElseLastStatement)
        {
            throw new WrongTypeException("Else statement", ifLastStatement, ifElseLastStatement);
        }
        return ifLastStatement;
    }
    public override object VisitElseif([NotNull] GtlParser.ElseifContext context)
    {
        string exprtype = (string)Visit(context.expr());
        if (!exprtype.Equals("bool"))
        {
            throw new WrongTypeException("Elseif statement", "bool", exprtype);
        }
        EnterScope(new Scope());
        string ifLastStatement = "";
        foreach (var stmt in context.statement())
        {
            if (stmt.expr() != null)
            {
                ifLastStatement = (string)Visit(stmt.expr());
            }
            else if (stmt.declaration() != null)
            {
                ifLastStatement = (string)Visit(stmt.declaration());
            }
            else if (stmt.function() != null)
            {
                _ = Visit(stmt.function());
            }
            else if (stmt.game_variable_declaration != null)
            {
                _ = Visit(stmt.game_variable_declaration());
            }
            else if (stmt.game_functions() != null)
            {
                _ = Visit(stmt.game_functions());
            }
        }
        ExitScope();
        return ifLastStatement;
    }


    public override object VisitElse([NotNull] GtlParser.ElseContext context)
    {
        EnterScope(new Scope());
        string ifLastStatement = "";
        foreach (var stmt in context.statement())
        {
            if (stmt.expr() != null)
            {
                ifLastStatement = (string)Visit(stmt.expr());
            }
            else if (stmt.declaration() != null)
            {
                ifLastStatement = (string)Visit(stmt.declaration());
            }
            else if (stmt.function() != null)
            {
                _ = Visit(stmt.function());
            }
            else if (stmt.game_variable_declaration != null)
            {
                _ = Visit(stmt.game_variable_declaration());
            }
            else if (stmt.game_functions() != null)
            {
                _ = Visit(stmt.game_functions());
            }
        }
        ExitScope();
        return ifLastStatement;
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
        throw new NotSupportedException($"literal expression expected type but received {val}");
    }
    public override object VisitBinaryExpr(GtlParser.BinaryExprContext context)
    {
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
        string functionId = context.ID().GetText();
        if (!ScopeStack.Peek().FtableContains(functionId))
        {
            throw new FunctionCallException($"{functionId} not found in Function table");
        }
        string[] stringArray = [];
        foreach (GtlParser.ExprContext exprcontext in context.arg_call().expr())
        {
            stringArray = stringArray.Append(Visit(exprcontext).ToString()).ToArray()!;
        }
        string[][] functionTypes = ScopeStack.Peek().FtableFind(functionId);
        if (stringArray.Length != functionTypes[0].Length)
        {
            throw new FunctionCallException("Not the correct amount of input parameters in function");
        }
        for (int i = 0; i < stringArray.Length; i++)
        {
            if (!stringArray[i].Equals(functionTypes[0][i]))
            {
                throw new FunctionCallException(functionTypes[0][i], stringArray[i]);
            }
        }
        return ScopeStack.Peek().FtableFind(functionId)[1][0];
    }
    public override object VisitParExpr(GtlParser.ParExprContext context)
    {
        return (string)Visit(context.expr());
    }
    public override object VisitGame_utility_function([NotNull] GtlParser.Game_utility_functionContext context)
    {
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
        return id switch
        {
            "lastMove" => "move",
            "turn" => "int",
            "moveAtTurn" => "move",
            "playerScore" => "int",
            _ => throw new NotSupportedException($"Method {id} not found"),
        };
    }

    public override object VisitPlayer([NotNull] GtlParser.PlayerContext context)
    {
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
        string gametype = context.game_type().GetText();
        if (gametype.Equals("Moves"))
        {
            GtlParser.Array_typeContext[] moves = context.game_expr().array().array_type();
            foreach (GtlParser.Array_typeContext move in moves)
            {
                ScopeStack.Peek().AddVariable(move.GetText(), "move");
            }
            return null!;
        }
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
        string arraytype = (string)Visit(context.array_type(0));
        for (int i = 1; i < context.array_type().Length; i++)
        {
            string valuetype = (string)Visit(context.array_type(i));
            if (!arraytype.Equals(valuetype))
            {
                throw new WrongTypeException("Array", arraytype, valuetype);
            }
        }
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
        string exprtype = (string)Visit(context.expr());
        if (!exprtype.Equals("int") && !exprtype.Equals("real"))
        {
            throw new WrongTypeException("Unary expression", "int or type real", exprtype);
        }
        return exprtype;
    }
    private object VisitId(string id)
    {
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


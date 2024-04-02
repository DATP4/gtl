// words starting without capital letter are parser rules
// words starting with capital letter are lexer rules (All
// caps for readability)

grammar Gtl;

// Start of the program
program
    : (statement)+ EOF
    ;

// Statements
statement
    : expr ';'
    | declaration
    | function
    | if
    | game
    | payoff ';'
    | player
    | strategy ';'
    | strategy_space ';'
    | action
    ;

// Expressions
expr
    : literal                           # LiteralExpr
    | ID                                # IdExpr
    | array                             # ArrayExpr
    | tuple                             # TupleExpr
    | util_function                     # UtilExpr
    | '(' expr ')'                      # ParExpr
    | ID '(' arg_call ')'               # ArgCallExpr
    | expr (member_access)+             # MemberExpr
    | expr '.' ID '(' arg_call ')'      # ChainArgCallExpr
    | expr op=('*' | '/' | MOD) expr    # BinaryExpr
    | expr op=('+' | '-') expr          # BinaryExpr
    | expr op='=' expr                  # BooleanExpr
    | expr op='>' expr                  # BooleanExpr
    | expr op='<' expr                  # BooleanExpr
    | expr op='<=' expr                 # BooleanExpr
    | expr op='>=' expr                 # BooleanExpr
    | expr op='!=' expr                 # BooleanExpr
    | expr op='&&' expr                 # BooleanExpr
    | expr op='||' expr                 # BooleanExpr
    | expr op='^^' expr                 # BooleanExpr
    | '!' expr                          # LogicalNotExpr
    ;

// Declarations
declaration
    : type ID '=' expr ';'
    ;

// Functions
function
    : ID ':' '(' arg_def ')' '->' type '{' (statement)* '}'
    ;

// Literals
literal
    : INT | REAL | boolean_literal
    ;

boolean_literal
    : TRUE | FALSE
    ;

// Control structures
if
    : IF '(' expr ')' THEN '{' (statement)* '}' (elseif | else)?
    ;

elseif
    : ELSE IF '(' expr ')' THEN '{' (statement)* '}' (elseif | else)?
    ;

else
    : ELSE '{' (statement)* '}'
    ;

// Argument definitions
arg_def
    : ((type ID) (',' type ID)*)?
    ;

arg_call
    : (expr (',' expr)*)?
    ;

// Arrays
array
    : '[' expr (',' expr)* ']'
    ;

// Member access
member_access
    : '.' ID
    ;

// Game theory specific grammar
tuple
    : '(' expr (',' expr)+ ')'
    ;

game
    : GAME ID '=' '('
        PLAYERS ':' player_list
        STRATEGYSPACE ':' ID
        PAYOFFS ':' ID
    ')' ';'
    ;

player
    : ID CHOOSES ID
    ;

player_list
    : (player) (',' player)*
    ;

payoff
    : PAYOFFS ID '=' expr
    ;

util_function
    : ID '->' expr
    ;

strategy
    : STRATEGY ID '=' expr
    ;

strategy_space
    : STRATEGYSPACE ID '=' expr
    ;

action
    : ACTION ID '=' '(' expr ')' THEN ID ';'
    ;

// Types
type
    : T_INT
    | T_REAL
    | T_BOOL
    ;

// Lexer rules
T_INT   : 'int';
T_REAL  : 'real';
T_BOOL  : 'bool';

GAME    : 'Game';
ACTION  : 'Action';
STRATEGY: 'Strategy';
PLAYERS : 'Players';
PAYOFFS : 'Payoffs';
STRATEGYSPACE: 'Strategyspace';

IF      : 'if';
THEN    : 'then';
ELSE    : 'else';

ASSIGN  : '=';
R_ARROW : '->';
CHOOSES : 'chooses';

LPAR    : '(';
RPAR    : ')';
LCURL   : '{';
RCURL   : '}';
LSQUARE : '[';
RSQUARE : ']';
COLON   : ':';
SEMICOLON: ';';
COMMA   : ',';
DOT     : '.';

PLUS    : '+';
MINUS   : '-';
MUL     : '*';
DIV     : '/';
MOD     : 'MOD';

EQUALS : '==';
GREATER : '>';
LESS    : '<';
LESSEQ  : '<=';
GREATEREQ: '>=';
NOTEQ   : '!=';
AND     : '&&';
OR      : '||';
XOR     : '^^';
NOT     : '!';

TRUE    : 'TRUE';
FALSE   : 'FALSE';

INT     : [0-9]+;
REAL    : [0-9]+ ('.' [0-9]+)?;
ID      : [a-zA-Z_][a-zA-Z_0-9]*;

WS      : [ \t\r\n]+ -> skip;
COMMENT : '//' ~[\r\n]* -> skip;

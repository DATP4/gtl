// words starting without capital letter are parser rules
// words starting with capital letter are lexer rules (All
// caps for readability)

grammar Gtl;

// Start of the program
program
    : list_of_statements EOF
    ;

// Statements
list_of_statements
    : statement*
    ;

statement
    : declaration
    | assignment
    | function // ?????
    | if
    | return
    | game
    | payoff
    | player
    | strategy
    | strategy_set
    | action
    ;

// Control structures
return
    : RETURN expr? SEMICOLON
    ;

if
    : IF LPAR b_expr RPAR THEN LCURL list_of_statements RCURL (elseif | else)?
    ;

elseif
    : ELSE IF LPAR b_expr RPAR THEN LCURL list_of_statements RCURL (elseif | else)?
    ;

else
    : ELSE LCURL list_of_statements RCURL
    ;

// Declarations
declaration
    : type ID ASSIGN expr SEMICOLON
    ;

// Assignment
assignment
    : ID ASSIGN expr SEMICOLON
    ;

// Functions
function
    : ID COLON LPAR arg_def RPAR R_ARROW type LCURL statement RCURL
    ;

// Argument definitions
arg_def
    : ((type ID) (COMMA type ID)*)?
    ;

arg_strategy
    : ID (COMMA ID)*
    ;

arg_call
    : (expr (COMMA expr)*)?
    ;

arg_chain
    : DOT arg_call
    ;

// Expressions
expr
    : LPAR expr RPAR
    | ID LPAR arg_call RPAR arg_chain*
    | expr (MUL | DIV) expr
    | expr (PLUS | MINUS) expr
    | expr MOD expr
    | array
    | INT
    | BOOL
    | move
    ;

b_expr
    : expr BEQUALS expr
    | expr GREATER expr
    | expr LESS expr
    | expr LESSEQ expr
    | expr GREATEREQ expr
    | expr NOTEQ expr
    | b_expr AND b_expr
    | b_expr OR b_expr
    | b_expr XOR b_expr
    | NOT b_expr
    ;

// Types
type
    : T_INT
    | T_REAL
    | T_BOOL
    ;

// Arrays
array
    : LSQUARE expr(COMMA expr)* RSQUARE
    ;

strategy_set_array
    : LSQUARE move_tuple (COMMA move_tuple)* RSQUARE
    ;

// Game theory specific grammar
move
    : ID
    ;

move_tuple
    : LPAR move (COMMA move)* RPAR
    ;

game
    : GAME ID LPAR array COMMA ID COMMA expr RPAR SEMICOLON
    ;

payoff
    : PAYOFFS ID LPAR RPAR ASSIGN LCURL ID R_ARROW array (COMMA ID R_ARROW array)* RCURL
    ;

player
    : PLAYER ID LPAR ID RPAR SEMICOLON
    ;

strategy
    : STRATEGY ID R_ARROW arg_strategy LCURL ID (COMMA ID)* SEMICOLON RCURL
    ;

strategy_set
    : STRATEGYSET ID ASSIGN strategy_set_array
    ;

action
    : ACTION ID ASSIGN LCURL list_of_statements RCURL
    ;

// Lexer rules
T_INT   : 'int';
T_REAL  : 'real';
T_BOOL  : 'bool';

GAME    : 'Game';
ACTION  : 'Action';
STRATEGY: 'Strategy';
PLAYER  : 'Player';
PAYOFFS : 'Payoffs';
STRATEGYSET: 'StrategySet';

IF      : 'if';
THEN    : 'then';
ELSE    : 'else';
IS      : 'is';
BREAK   : 'break';
RETURN  : 'return';

ASSIGN  : '=';
R_ARROW : '->';

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
MOD     : 'mod';

BEQUALS : '==';
GREATER : '>';
LESS    : '<';
LESSEQ  : '<=';
GREATEREQ: '>=';
NOTEQ   : '!=';
AND     : '&&';
OR      : '||';
XOR     : '^^';
NOT     : '!';

LITERAL : INT | REAL;
ID      : [a-zA-Z_][a-zA-Z_0-9]*;
INT     : [0-9]+;
REAL    : [0-9]+ ('.' [0-9]+)?;
BOOL    : 'TRUE' | 'FALSE';

WS      : [ \t\r\n]+ -> skip;
COMMENT : '//' ~[\r\n]* -> skip;

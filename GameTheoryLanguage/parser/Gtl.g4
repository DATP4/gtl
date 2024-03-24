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
    : expr SEMICOLON
    | declaration
    | function
    | return
    | if
    | break
    | game
    | payoff
    | player
    | strategy
    | strategy_set
    | action
    ;

// Expressions
expr
    : LITERAL                           # LiteralExpr
    | ID                                # IdExpr
    | array                             # ArrayExpr
    | LPAR expr RPAR                    # ParExpr
    | ID LPAR arg_call RPAR             # ArgCallExpr
    | expr DOT ID LPAR arg_call RPAR    # ChainArgCallExpr
    | expr op=(MUL | DIV) expr          # BinaryExpr
    | expr op=(PLUS | MINUS) expr       # BinaryExpr
    | expr op=MOD expr                  # BooleanExpr
    | expr op=EQUALS expr               # BooleanExpr
    | expr op=GREATER expr              # BooleanExpr
    | expr op=LESS expr                 # BooleanExpr
    | expr op=LESSEQ expr               # BooleanExpr
    | expr op=GREATEREQ expr            # BooleanExpr
    | expr op=NOTEQ expr                # BooleanExpr
    | expr op=AND expr                  # BooleanExpr
    | expr op=OR expr                   # BooleanExpr
    | expr op=XOR expr                  # BooleanExpr
    | NOT expr                          # LogicalNotExpr
    ;

// Declarations
declaration
    : type ID ASSIGN expr SEMICOLON
    ;

// Functions
function
    : ID COLON LPAR arg_def RPAR R_ARROW type LCURL (statement)* RCURL
    ;

return
    : RETURN expr? SEMICOLON
    ;

// Control structures
if
    : IF LPAR expr RPAR THEN LCURL (statement)* RCURL (elseif | else)?
    ;

elseif
    : ELSE IF LPAR expr RPAR THEN LCURL (statement)* RCURL (elseif | else)?
    ;

else
    : ELSE LCURL (statement)* RCURL
    ;

break
    : BREAK SEMICOLON
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

// Arrays
array
    : LSQUARE expr (COMMA expr)* RSQUARE
    ;

strategy_set_array
    : LSQUARE move_tuple (COMMA move_tuple)* RSQUARE
    ;

// Game theory specific grammar
move_tuple
    : LPAR ID (COMMA ID)* RPAR
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
    : ACTION ID ASSIGN LCURL (statement)+ RCURL
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
PLAYER  : 'Player';
PAYOFFS : 'Payoffs';
STRATEGYSET: 'StrategySet';

RETURN  : 'return';
IF      : 'if';
THEN    : 'then';
ELSE    : 'else';
BREAK   : 'break';

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

LITERAL : INT | REAL | BOOL;
ID      : [a-zA-Z_][a-zA-Z_0-9]*;
INT     : [0-9]+;
REAL    : [0-9]+ ('.' [0-9]+)?;
BOOL    : 'TRUE' | 'FALSE';

WS      : [ \t\r\n]+ -> skip;
COMMENT : '//' ~[\r\n]* -> skip;

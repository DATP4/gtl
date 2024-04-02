// words starting without capital letter are parser rules
// words starting with capital letter are lexer rules (All
// caps for readability)

// move_tuple are now just tuples
// strategy space takes an array and should be verified to contain tuples
// made boolean literals and changed literal to parser rule
// reworked game theory
// removed chain args and added member access instead

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
    | if
    | break
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
    | LPAR expr RPAR                    # ParExpr
    | ID LPAR arg_call RPAR             # ArgCallExpr
    | expr (member_access)+             # MemberExpr
    | expr DOT ID LPAR arg_call RPAR    # ChainArgCallExpr
    | expr op=(MUL | DIV | MOD) expr    # BinaryExpr
    | expr op=(PLUS | MINUS) expr       # BinaryExpr
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

// Literals
literal
    : INT | REAL | boolean_literal
    ;

boolean_literal
    : TRUE | FALSE
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

arg_call
    : (expr (COMMA expr)*)?
    ;

// Arrays
array
    : LSQUARE expr (COMMA expr)* RSQUARE
    ;

// Member access
member_access
    : '.' ID
    ;

// Game theory specific grammar
tuple
    : LPAR expr (COMMA expr)+ RPAR
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
BREAK   : 'break';

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

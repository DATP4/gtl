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
    | declaration ';'
    | function
    | game_variable_declaration ';'
    | game_functions ';'
    ;

// Expressions
expr
    : '(' expr ')'                      # ParExpr
    | literal                           # LiteralExpr
    | ID                                # IdExpr
    | ID '(' arg_call ')'               # ArgCallExpr
    | ID (method_access)+               # MethodCallExpr
    | ID (member_access)+               # MemberExpr
    | ifElse                            # IfElseExpr
    | expr op=('*' | '/' | MOD) expr    # BinaryExpr
    | expr op=('+' | '-') expr          # BinaryExpr
    | expr op='==' expr                 # BooleanExpr
    | expr op='>' expr                  # BooleanExpr
    | expr op='<' expr                  # BooleanExpr
    | expr op='<=' expr                 # BooleanExpr
    | expr op='>=' expr                 # BooleanExpr
    | expr op='!=' expr                 # BooleanExpr
    | expr op='&&' expr                 # BooleanExpr
    | expr op='||' expr                 # BooleanExpr
    | expr op='^^' expr                 # BooleanExpr
    | '!' expr                          # LogicalNotExpr
    | '-' expr                          # UnaryExpr
    ;

// Declarations
declaration
    : type ID '=' expr
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
ifElse
    : IF '(' expr ')' THEN '{' (statement)+ '}' elseif* else
    ;

elseif
    : ELSE IF '(' expr ')' THEN '{' (statement)+ '}'
    ;

else
    : ELSE '{' (statement)* '}'
    ;

// Argument definitions
arg_def
    : (type ID (',' type ID)*)?
    ;

arg_call
    : (expr (',' expr)*)?
    ;

array
    : '[' array_type (',' array_type)* ']'
    ;

array_type
    : expr
    | player
    | game_utility_function
    | tuple
    ;

tuple
    : '(' ID (',' ID)* ')'
    ;

member_access
    : '.' ID
    ;

method_access
    : '.' ID '(' arg_def ')'
    ;

// Game theory specific grammar

game_variable_declaration
    : game_type ID '=' game_expr
    ;

game_expr
    : array
    | action
    | game_tuple
    ;

action
    : '('expr?')' THEN move
    ;

game_utility_function
    : ID '->' array
    ;

game_tuple
    : '(' ID ',' ID ',' ID ')'
    ;

player
    : ID CHOOSES ID
    ;

move
    : ID
    ;

game_functions
    : RUN '(' ID',' expr ')'
    ;

// Types
type
    : T_INT
    | T_REAL
    | T_BOOL
    | T_STRING
    ;

game_type
    : T_GAME
    | T_PLAYERS
    | T_MOVES
    | T_PAYOFF
    | T_STRATEGY
    | T_STRATEGYSPACE
    | T_ACTION
    ;

// Lexer rules
T_INT   : 'int';
T_REAL  : 'real';
T_BOOL  : 'bool';
T_STRING: 'str';

T_GAME    : 'Game';
T_MOVES   : 'Moves';
T_ACTION  : 'Action';
T_STRATEGY: 'Strategy';
T_PLAYERS : 'Players';
T_PAYOFF  : 'Payoff';
T_STRATEGYSPACE: 'Strategyspace';
RUN     : 'run';

IF      : 'if';
THEN    : 'then';
ELSE    : 'else';

LET     : 'let';
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
REAL    : [0-9]+ '.' [0-9]+;
ID      : [a-zA-Z_][a-zA-Z_0-9]*;

WS      : [ \t\r\n]+ -> skip;
COMMENT : '//' ~[\r\n]* -> skip;

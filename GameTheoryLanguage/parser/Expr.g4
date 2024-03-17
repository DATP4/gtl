grammar Expr;		

prog    : strat EOF ;

gam     : GAME name LPAR arr COMMA name COMMA name RPAR ;

pay     : PAYOFFS name EQUALS LCURL name ARROW arr (COMMA name ARROW arr)* RCURL ;

pl      : PLAYER pl_id LPAR name RPAR ;

strat   : STRATEGY name ARROW arg_x LCURL name (COMMA name)* RCURL ;

strat_s : STRATEGYSET name EQUALS strat_arr ;

act     : ACTION name EQUALS LCURL state RCURL ;


state   : state SEMICOLON state
        | IF LPAR b_expr RPAR THEN LCURL state RCURL
        | IF LPAR b_expr RPAR THEN LCURL state RCURL ELSE LCURL state RCURL
        | IS x LPAR (s_c)+ RPAR
        | BREAK
        | RETURN x
        | d_f
        | d_v
        | x EQUALS expr
        ;

b_expr  : expr BEQUALS expr
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


expr    : LPAR expr RPAR 
        | expr (MUL | DIV) expr
        | expr (PLUS | MINUS) expr 
        | expr MOD expr
        | INT
        | string
        | BOOL
        | d_v
        | LPAR expr RPAR
        ;

act_t   : CUSTOM | DEFAULT ;

act_c   : INT | (pl_id) (COMMA pl_id)* |  ;

pl_id   : name ;

s_c     : x COLON state | OTHER COLON state ;

d_f     : x COLON LPAR (arg_v) RPAR ARROW t LCURL state RCURL;

arg_v   : (t x) (COMMA t x)* ; 
arg_x   : x (COMMA x)* ;

d_v     : t x EQUALS x;


x       : INT 
        | REAL 
        | CHAR 
        | string 
        | BOOL 
        ;
        
t       : T_INT
        | T_REAL 
        | T_CHAR
        | T_STRING 
        | T_BOOL 
        ;

arr     : LCURL element RCURL ;
strat_arr: LCURL LPAR element RPAR (COMMA LPAR element RPAR)* RCURL ;

element : name (COMMA name)* ;

GAME    : 'Game' ;
ACTION  : 'Action' ;
STRATEGY: 'Strategy' ;
PLAYER  : 'Player' ;
PAYOFFS : 'Payoffs' ;
STRATEGYSET: 'StrategySet' ;

CUSTOM  : 'custom' ;
DEFAULT : 'default' ;
OTHER   : 'other' ;

IF      : 'if' ;
THEN    : 'then' ;
ELSE    : 'else' ;
IS      : 'is' ;
BREAK   : 'break' ;
RETURN  : 'return' ;

LPAR    : '(' ;
RPAR    : ')' ;
LCURL   : '{' ;
RCURL   : '}' ;
COLON   : ':' ;
SEMICOLON: ';' ;
COMMA   : ',' ;

T_INT   : 'int' ;
T_REAL  : 'real' ;
T_CHAR  : 'char' ;
T_STRING: 'String' ;
T_BOOL  : 'bool' ;

PLUS    : '+';
MINUS   : '-';
MUL     : '*';
DIV     : '/';
MOD     : 'mod';

EQUALS  : '=' ;
ARROW   : '->' ;

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

name    : (CHAR | INT)+ ;
string  : '"'(CHAR)+'"' ;
INT     : [0-9] ;
REAL    : [0-9]+ ('.' [0-9]+)?;
CHAR    : [a-zA-Z] ; 
BOOL    : 'TRUE' | 'FALSE' ;

WS      : [ \t\r\n]+ -> skip;
CM      : '//' ~[\r\n]* -> skip;
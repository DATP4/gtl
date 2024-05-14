#![allow(warnings)]
 mod library;
use library::{Action, BoolExpression, Condition, Game, GameState, Moves, Payoff, Players, Strategy, Strategyspace};
#[cfg(test)]
mod tests {
use super::*;
#[test]
fn declaration_test_1(){
let mut gamestate: GameState = GameState::new();
let x = 5
;
assert_eq!(x, 5)
}
#[test]
fn declaration_test_2(){
let mut gamestate: GameState = GameState::new();
let x = 5.0
;
assert_eq!(x, 5.0)
}
#[test]
fn declaration_test_3(){
let mut gamestate: GameState = GameState::new();
let x = true
;
assert_eq!(x, true)
}
#[test]
fn binary_expression_test_1(){
let mut gamestate: GameState = GameState::new();
let test = 1 + 1 * 7
;
assert_eq!(test, 8)
}
#[test]
fn binary_expression_test_2(){
let mut gamestate: GameState = GameState::new();
let test = 1.5 + 1.6 / 10.0
;
assert_eq!(test, 1.66)
}
#[test]
fn binary_expression_test_3(){
let mut gamestate: GameState = GameState::new();
let test = -5 + (4 + 3 * (4 % 5) / 1 + 5) - 3
;
assert_eq!(test, 13)
}
#[test]
fn binary_expression_test_4(){
let mut gamestate: GameState = GameState::new();
let test = 5 % 3
;
assert_eq!(test, 2)
}
#[test]
fn boolean_expression_test_1(){
let mut gamestate: GameState = GameState::new();
let test1 = true && false
;
assert_eq!(test1, false)
}
#[test]
fn boolean_expression_test_2(){
let mut gamestate: GameState = GameState::new();
let test1 = 1 > 2
;
assert_eq!(test1, false)
}
#[test]
fn boolean_expression_test_3(){
let mut gamestate: GameState = GameState::new();
let test1 = 1 != 0
;
assert_eq!(test1, true)
}
#[test]
fn boolean_expression_test_4(){
let mut gamestate: GameState = GameState::new();
let test1 = 1 <= 2
;
assert_eq!(test1, true)
}
#[test]
fn boolean_expression_test_5(){
let mut gamestate: GameState = GameState::new();
let test1 = (true && true) || ((true && true) == true)
;
assert_eq!(test1, true)
}
#[test]
fn logical_not_test_1(){
let mut gamestate: GameState = GameState::new();
let test1 = !(true && false)
;
assert_eq!(test1, true)
}
#[test]
fn logical_not_test_2(){
let mut gamestate: GameState = GameState::new();
let test1 = !(1 > 2)
;
assert_eq!(test1, true)
}
#[test]
fn logical_not_test_3(){
let mut gamestate: GameState = GameState::new();
let test1 = !(1 != 0)
;
assert_eq!(test1, false)
}
#[test]
fn logical_not_test_4(){
let mut gamestate: GameState = GameState::new();
let test1 = !(1 <= 2)
;
assert_eq!(test1, false)
}
#[test]
fn logical_not_test_5(){
let mut gamestate: GameState = GameState::new();
let test1 = !(!(true && true) || ((true && true) == true))
;
assert_eq!(test1, false)
}
#[test]
fn print_test_1(){
let mut gamestate: GameState = GameState::new();
let x = 1
;
let y = 2.0
;
let z = true
;
println!("{:?}", x)
;
println!("{:?}", y)
;
println!("{:?}", z)
;
assert_eq!(1, 1);
}
#[test]
fn unary_expression_test_1(){
let mut gamestate: GameState = GameState::new();
let x = -5
;
assert_eq!(x, -5)
}
#[test]
fn unary_expression_test_2(){
let mut gamestate: GameState = GameState::new();
let x = -5.0
;
assert_eq!(x, -5.0)
}
#[test]
fn unary_expression_test_3(){
let mut gamestate: GameState = GameState::new();
let x = -5 - -5
;
assert_eq!(x, 0)
}
#[test]
fn unary_expression_test_4(){
let mut gamestate: GameState = GameState::new();
let x = -5 + -5
;
assert_eq!(x, -10)
}
#[test]
fn unary_expression_test_5(){
let mut gamestate: GameState = GameState::new();
let x = -5
;
let y = -x
;
assert_eq!(y, 5)
}
#[test]
fn if_else_test_1(){
let mut gamestate: GameState = GameState::new();
let y = if true {
let x = 4
;x
} else {
let x = 5
;x
}

;
assert_eq!(y, 4)
}
#[test]
fn if_else_test_2(){
let mut gamestate: GameState = GameState::new();
let y = if false {
let x = 4
;x
} else {
let x = 5
;x
}

;
assert_eq!(y, 5)
}
#[test]
fn if_else_test_3(){
let mut gamestate: GameState = GameState::new();
let y = if true {
let x = 4
;x
} else if true {
let x = 5
;x
} else {
let x = 6
;x
}

;
assert_eq!(y, 4)
}
#[test]
fn if_else_test_4(){
let mut gamestate: GameState = GameState::new();
let y = if false {
let x = 4
;x
} else if true {
let x = 5
;x
} else {
let x = 6
;x
}

;
assert_eq!(y, 5)
}
#[test]
fn if_else_test_5(){
let mut gamestate: GameState = GameState::new();
let y = if false {
let x = 4
;x
} else if false {
let x = 5
;x
} else {
let x = 6
;x
}

;
assert_eq!(y, 6)
}
#[test]
fn function_test_1(){
let mut gamestate: GameState = GameState::new();
fn int_function(x: &i32) -> i32 {
let y = *x + 10 * 5
;y - 5
}
let x = int_function(&(5))
;
assert_eq!(x, 50)
}
#[test]
fn function_test_2(){
let mut gamestate: GameState = GameState::new();
let a = 10
;
fn int_function1(x: &i32) -> i32 {
let a = 10;
let y = *x + a * 5
;y - 5
}
let x = int_function1(&(5))
;
assert_eq!(x, 50)
}
#[test]
fn function_test_3(){
let mut gamestate: GameState = GameState::new();
fn int_function(x: &i32) -> i32 {
fn int_function2(z: &i32) -> i32 {
*z + 5
}let y = int_function2(&(*x))
;y
}
let x = int_function(&(5))
;
assert_eq!(x, 10)
}
#[test]
fn function_test_4(){
let mut gamestate: GameState = GameState::new();
let a = 10 + 5
;
let b = 5 + 13
;
let c = a * b
;
fn int_function(x: &i32) -> i32 {
let a = 10+5;
let b = 5+13;
let c = a*b;
c + *x
}
let x = int_function(&(10))
;
assert_eq!(x, 280)
}
#[test]
fn action_test_1(){
let mut gamestate: GameState = GameState::new();
;
let turn: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| true}),
act_move: Moves::a,
}
;
assert_eq!(turn.act_move, Moves::a);
let Condition::Expression(expr) = turn.condition;
assert_eq!((expr.b_val)(&gamestate), true)
}
#[test]
fn action_test_2(){
let mut gamestate: GameState = GameState::new();
;
let turn: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| gamestate.turn == 1}),
act_move: Moves::a,
}
;
assert_eq!(turn.act_move, Moves::a);
let Condition::Expression(expr) = turn.condition;
assert_eq!((expr.b_val)(&gamestate), true);
gamestate.turn += 1;
assert_eq!((expr.b_val)(&gamestate), false);
}
#[test]
fn strategy_test_1(){
let mut gamestate: GameState = GameState::new();
;
let turn: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| true}),
act_move: Moves::a,
}
;
let strat: Strategy = Strategy{
strat: vec![turn.clone()],
}
;
assert_eq!(strat.strat[0].act_move, Moves::a)
}
#[test]
fn player_test_1(){
let mut gamestate: GameState = GameState::new();
;
let turn: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| true}),
act_move: Moves::a,
}
;
let strat: Strategy = Strategy{
strat: vec![turn.clone()],
}
;
let p: Players = Players{
pl_and_strat: vec![
("p1".to_string(), strat.clone()),
],
}
;
assert_eq!(p.pl_and_strat[0].1.strat[0].act_move, Moves::a)
}
#[test]
fn payoff_test_1(){
let mut gamestate: GameState = GameState::new();
let p: Payoff = Payoff{
matrix: vec![
vec![1, 2],
vec![2, 1],
],
}
;
assert_eq!(p.matrix[0][0], 1);
assert_eq!(p.matrix[0][1], 2);
assert_eq!(p.matrix[1][0], 2);
assert_eq!(p.matrix[1][1], 1);
}
#[test]
fn strategyspace_test_1(){
let mut gamestate: GameState = GameState::new();
;
let stratspace: Strategyspace = Strategyspace{
matrix: vec![
Moves::a, Moves::b, 
Moves::b, Moves::a, 
Moves::a, Moves::a, 
Moves::b, Moves::b, 
],
}
;
assert_eq!(stratspace.matrix[0], Moves::a);
assert_eq!(stratspace.matrix[1], Moves::b);
assert_eq!(stratspace.matrix[2], Moves::b);
assert_eq!(stratspace.matrix[3], Moves::a);
assert_eq!(stratspace.matrix[4], Moves::a);
assert_eq!(stratspace.matrix[5], Moves::a);
assert_eq!(stratspace.matrix[6], Moves::b);
assert_eq!(stratspace.matrix[7], Moves::b);
}
#[test]
fn program_test_1(){
let mut gamestate: GameState = GameState::new();
;
let turn1Coop: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| gamestate.turn == 1}),
act_move: Moves::cooperate,
}
;
let oppDeflect: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| GameState::last_move(&gamestate, &"p2".to_string()) == Moves::cooperate}),
act_move: Moves::deflect,
}
;
let oppCooperate: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| GameState::last_move(&gamestate, &"p2".to_string()) == Moves::deflect}),
act_move: Moves::cooperate,
}
;
let turn: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| gamestate.turn == 4}),
act_move: Moves::deflect,
}
;
let coop: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| true}),
act_move: Moves::cooperate,
}
;
let aStrat1: Strategy = Strategy{
strat: vec![turn1Coop.clone(), oppDeflect.clone(), oppCooperate.clone()],
}
;
let aStrat2: Strategy = Strategy{
strat: vec![coop.clone()],
}
;
let stratspace: Strategyspace = Strategyspace{
matrix: vec![
Moves::cooperate, Moves::cooperate, 
Moves::deflect, Moves::cooperate, 
Moves::cooperate, Moves::deflect, 
Moves::deflect, Moves::deflect, 
],
}
;
let payoff: Payoff = Payoff{
matrix: vec![
vec![1, 4, 0, 2],
vec![1, 0, 4, 2],
],
}
;
let p: Players = Players{
pl_and_strat: vec![
("p1".to_string(), aStrat1.clone()),
("p2".to_string(), aStrat2.clone()),
],
}
;
let mut prisoners: Game = Game{
game_state: &mut gamestate,
strat_space: &stratspace,
players: &p,
pay_matrix: &payoff,
}
;
let finishedgame = Game::run(&mut prisoners, &mut 5);
assert_eq!(finishedgame.turn, 6);
assert_eq!(finishedgame.player_score(&"p1".to_string()), 17);
assert_eq!(finishedgame.player_score(&"p2".to_string()), 1);
}
#[test]
fn program_test_2(){
let mut gamestate: GameState = GameState::new();
fn gcd(a: &i32, b: &i32) -> i32 {
if *b == 0 {
*a
} else {
gcd(&(*b), &(*a % *b))
}

}
let a = gcd(&(10), &(5))
;
let b = gcd(&(32131), &(3241))
;
let c = gcd(&(1088), &(17))
;
assert_eq!(a, 5);
assert_eq!(b, 1);
assert_eq!(c, 17);
}
#[test]
fn program_test_3(){
let mut gamestate: GameState = GameState::new();
;
let a1: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| GameState::last_move(&gamestate, &"p2".to_string()) == GameState::last_move(&gamestate, &"p3".to_string())}),
act_move: Moves::a,
}
;
let a2: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| true}),
act_move: Moves::a,
}
;
let a3: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| gamestate.turn == 1}),
act_move: Moves::b,
}
;
let s1: Strategy = Strategy{
strat: vec![a3.clone(), a1.clone()],
}
;
let s2: Strategy = Strategy{
strat: vec![a2.clone()],
}
;
let stratspace: Strategyspace = Strategyspace{
matrix: vec![
Moves::a, Moves::a, Moves::a, 
Moves::a, Moves::a, Moves::b, 
Moves::a, Moves::b, Moves::a, 
Moves::b, Moves::a, Moves::a, 
Moves::a, Moves::b, Moves::b, 
Moves::b, Moves::a, Moves::b, 
Moves::b, Moves::b, Moves::a, 
Moves::b, Moves::b, Moves::b, 
],
}
;
let payoff: Payoff = Payoff{
matrix: vec![
vec![3, 4, 0, 0, 0, 0, 0, 0],
vec![2, 0, 4, -1, 0, 0, 0, 0],
vec![1, 2, 4, 3, 0, 0, 0, 0],
],
}
;
let p: Players = Players{
pl_and_strat: vec![
("p1".to_string(), s1.clone()),
("p2".to_string(), s2.clone()),
("p3".to_string(), s2.clone()),
],
}
;
let mut prisoners: Game = Game{
game_state: &mut gamestate,
strat_space: &stratspace,
players: &p,
pay_matrix: &payoff,
}
;
let finishedgame = Game::run(&mut prisoners, &mut 5);
assert_eq!(finishedgame.turn, 6);
assert_eq!(finishedgame.player_score(&"p1".to_string()), 12);
assert_eq!(finishedgame.player_score(&"p2".to_string()), 7);
assert_eq!(finishedgame.player_score(&"p3".to_string()), 7);
}
}
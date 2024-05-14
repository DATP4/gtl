#![allow(warnings)]
mod library;
use library::{Action, BoolExpression, Condition, Game, GameState, Moves, Payoff, Players, Strategy, Strategyspace};
fn main()
{
let mut gamestate: GameState = GameState::new();
let someVar = 3
;
let x = 5
;
let dasdsa = "dsadas"
;
println!("{:?}", dasdsa)
;
fn intFunction(aVar: &i32) -> i32 {
let someVar = 3;
let x = 5;
let dasdsa = "dsadas";
let y = x + 10 * 5
;if x < 10 * 5 {
let z = 10 / 5
;z
} else if x == 10 {
x % 2
} else {
let z = 10 * 5
;z
}
;x - 5
}
intFunction(&(someVar));
fn boolFunction() -> bool {
let someVar = 3;
let x = 5;
let dasdsa = "dsadas";
true
}
let a = boolFunction()
;
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
b_val: |gamestate: &GameState| gamestate.turn == 1}),
act_move: Moves::cooperate,
}
;
let otherwise: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| true}),
act_move: Moves::deflect,
}
;
let aStrat1: Strategy = Strategy{
strat: vec![oppDeflect.clone(), oppCooperate.clone(), turn.clone()],
}
;
let aStrat2: Strategy = Strategy{
strat: vec![turn.clone(), otherwise.clone()],
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
vec![2, 4, 0, 1],
vec![2, 0, 4, 1],
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
Game::run(&mut prisoners, &mut 5);

}

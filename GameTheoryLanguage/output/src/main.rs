mod library;
use library::{Action, BoolExpression, Condition, Game, GameState, Moves, Payoff, Players, Strategy, Strategyspace};
fn main()
{
let gamestate: GameState = GameState {
turn: 1,
players: Vec::new(),
moves_and_points: Vec::new(), 
};
;
let a1: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| GameState::last_move(&gamestate, "p2".to_string()) == GameState::last_move(&gamestate, "p3".to_string())}),
act_move: Moves::a,
}
;
let a2: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| true}),
act_move: Moves::b,
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
vec![1, 4, 0, 2, 0, 0, 0, 0],
vec![1, 0, 4, 2, 0, 0, 0, 0],
vec![1, 2, 4, 0, 0, 0, 0, 0],
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
let prisoners: Game = Game{
game_state: gamestate,
strat_space: stratspace,
players: p,
pay_matrix: payoff,
}
;
;

}

mod library;
use library::{Action, BoolExpression, Condition, Game, GameState, Moves, PayoffMatrix, Players, Strategy, StrategySpace};
fn main()
{
fn intFunction(x: &i32) -> i32 {
let y = *x + 10 * 5;
*x - 5
}
let test = intFunction(&(5.2));


let testaction: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| true}),
act_move: Moves::cooperate,
};

let teststrategy: Strategy = Strategy{
strat: vec![testaction.clone()],
};

let teststratspace: Strategyspace = Strategyspace{
matrix: vec![
Moves::cooperate, Moves::cooperate, 
],
};

let testpayoff: Payoff = Payoff{
matrix: vec![
vec![1],
],
};

let testplayers: Players = Players{
pl_and_strat: vec![
("p1".to_string(), teststrategy.clone()),
],
};

let mut testgame: Game = Game{
game_state: &mut gamestate,
strat_space: &teststratspace,
players: &testplayers,
pay_matrix: &testpayoff,
};

let finishedgame = Game::run(&mut testgame, &mut 4);

}

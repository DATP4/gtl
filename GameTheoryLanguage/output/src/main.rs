mod library;
use library::{Action, BoolExpression, Condition, Game, GameState, Moves, PayoffMatrix, Players, Strategy, StrategySpace};
fn main()
{

let turn: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| true}),
act_move: Moves::move,
};

let strat: Strategy = Strategy{
strat: vec![turn.clone(), TRUE.clone()],
};


let testaction: Action = Action{
condition: Condition::Expression(BoolExpression {
b_val: |gamestate: &GameState| true}),
act_move: Moves::cooperate,
};
use library::{
    Action, BoolExpression, Condition, Game, GameState, Moves, Payoff, Players, Strategy,
    Strategyspace,
};
fn main() {
    let mut gamestate: GameState = GameState::new();
    let someVar = 3;

    let x = 5;

    let dasdsa = "dsadas";

    println!("{:?}", dasdsa);

    let test23 = true != true;

    fn intFunction(aVar: &i32) -> i32 {
        let someVar = 3;
        let x = 5;
        let dasdsa = "dsadas";
        let test23 = true != true;
        let y = x + 10 * 5;
        if x < 10 * 5 {
            let z = 10 / 5;
            z
        } else if x == 10 {
            x % 2
        } else {
            let z = 10 * 5;
            z
        }
    }
    let funcTest = intFunction(&(someVar));

    fn boolFunction() -> bool {
        let someVar = 3;
        let x = 5;
        let dasdsa = "dsadas";
        let test23 = true != true;
        let funcTest = intFunction(&(someVar));
        true
    }
    let a = boolFunction();

    let oppDefect: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gamestate: &GameState| {
                GameState::last_move(&gamestate, &"p2".to_string()) == Moves::cooperate
            },
        }),
        act_move: Moves::cooperate,
    };

    let oppCooperate: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gamestate: &GameState| {
                GameState::last_move(&gamestate, &"p2".to_string()) == Moves::defect
            },
        }),
        act_move: Moves::defect,
    };

    let turn: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gamestate: &GameState| gamestate.turn == 1,
        }),
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

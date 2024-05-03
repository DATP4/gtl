mod library;
use library::{
    Action, BoolExpression, Condition, Game, GameState, Moves, Payoff, Players, Strategy,
    Strategyspace,
};
fn main() {
    let gmst: GameState = GameState {
        turn: 1,
        players: Vec::new(),
        moves_and_points: Vec::new(),
    };
    let someVar = 3;
    let x = 5;
    fn intFunction(aVar: &i32) -> i32 {
        let someVar = 3;
        let x = 5;
        let y = x + 10 * 5;
        if x < 10 * 5 {
            let z = 10 / 5;
            z
        } else if x == 10 {
            x % 2
        } else {
            let z = 10 * 5;
            z
        };
        x - 5
    }
    intFunction(&(someVar));
    fn boolFunction() -> bool {
        let someVar = 3;
        let x = 5;
        true
    }
    let a = boolFunction();
    let oppDeflect: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| GameState::last_move(p2.to_string()) == cooperate,
        }),
        act_move: Moves::deflect,
    };
    let oppCooperate: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| GameState::last_move(p2.to_string()) == deflect,
        }),
        act_move: Moves::cooperate,
    };
    let turn: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| gamestate.turn == 4,
        }),
        act_move: Moves::deflect,
    };
    let aStrat1: Strategy = Strategy {
        strat: vec![oppDeflect.clone(), oppCooperate.clone(), turn.clone()],
    };
    let aStrat2: Strategy = Strategy {
        strat: vec![turn.clone()],
    };
    let stratspace: Strategyspace = Strategyspace {
        matrix: vec![
            Moves::cooperate,
            Moves::cooperate,
            Moves::deflect,
            Moves::cooperate,
            Moves::cooperate,
            Moves::deflect,
            Moves::deflect,
            Moves::deflect,
        ],
    };
    let payoff: Payoff = Payoff {
        matrix: vec![vec![1, 4, 0, 2], vec![1, 0, 4, 2]],
    };
    let p: Players = Players {
        pl_and_strat: vec![
            ("p1".to_string(), aStrat1.clone()),
            ("p2".to_string(), aStrat2.clone()),
        ],
    };
    let prisoners: Game = Game {
        game_state: gmst,
        strat_space: stratspace,
        players: p,
        pay_matrix: payoff,
    };
}

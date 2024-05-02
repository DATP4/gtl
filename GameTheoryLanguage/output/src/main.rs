mod library;
use library::{Action, BoolExpression, Condition, Game, GameState, Moves, PayoffMatrix, Players, Strategy, StrategySpace};


fn main() {
    // ----------------- Game setup -----------------
    let gmst: GameState = GameState {
        turn: 1,
        players: Vec::new(),
        moves_and_points: Vec::new(),
    };

    let turn_1: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| gmst.turn == 1,
        }),
        act_move: Moves::Cooperate,
    };

    let opp_deflect: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| { GameState::last_move(gmst, "player3".to_string()) == Moves::Deflect
            },
        }),
        act_move: Moves::Deflect,
    };

    let opp_coop: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| { GameState::last_move(gmst, "player3".to_string()) == Moves::Cooperate
            },
        }),
        act_move: Moves::Cooperate,
    };

    let player1_strat: Strategy = Strategy {
        strat: vec![turn_1, opp_deflect, opp_coop],
    };

    let even_turn_deflect: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| gmst.turn % 2 != 0,
        }),
        act_move: Moves::Deflect,
    };

    let odd_turn_coop: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| gmst.turn % 2 == 0,
        }),
        act_move: Moves::Cooperate,
    };

    let player2_strat: Strategy = Strategy {
        strat: vec![even_turn_deflect, odd_turn_coop],
    };

    let turn_less_5_coop: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| gmst.turn < 5,
        }),
        act_move: Moves::Cooperate,
    };

    let turn_over_5_deflect: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| gmst.turn >= 5,
        }),
        act_move: Moves::Deflect,
    };

    let player3_strat: Strategy = Strategy {
        strat: vec![turn_less_5_coop, turn_over_5_deflect],
    };

    let opp_turn_5_deflect_deflect: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |gmst: &GameState| { GameState::move_at_turn(gmst, "player3".to_string(), 5)  == Moves::Deflect
            },
        }),
        act_move: Moves::Deflect,
    };

    let always_true_coop: Action = Action {
        condition: Condition::Expression(BoolExpression {
            b_val: |_gmst: &GameState| true,
        }),
        act_move: Moves::Cooperate,
    };

    let player4_strat: Strategy = Strategy {
        strat: vec![opp_turn_5_deflect_deflect, always_true_coop],
    };

    let payoff_matrix: PayoffMatrix = PayoffMatrix {
        matrix: vec![
            vec![2, 0, 0, 0, 0, 0, 0, 0, 1, 3, 3, 4, 3, 4, 4, 8],
            vec![2, 0, 0, 0, 8, 4, 4, 3, 1, 3, 3, 4, 0, 0, 0, 0],
            vec![2, 0, 8, 4, 0, 0, 4, 3, 1, 3, 0, 0, 3, 4, 0, 0],
            vec![2, 8, 0, 4, 0, 4, 0, 3, 1, 0, 3, 0, 3, 0, 4, 0],
        ],
    };

    let strat_space: StrategySpace = StrategySpace {
        matrix: vec![
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Deflect,
            Moves::Deflect,
            Moves::Cooperate,
            Moves::Cooperate,
            Moves::Cooperate,
        ],
    };

    let player_array: Players = Players {
        pl_and_strat: vec![
            ("player1".to_string(), player1_strat.clone()),
            ("player2".to_string(), player2_strat.clone()),
            ("player3".to_string(), player3_strat.clone()),
            ("player4".to_string(), player4_strat.clone()),
        ],
    };

    

    let game: Game = Game {
        players: player_array,
        game_state: gmst,
        strat_space: strat_space,
        pay_matrix: payoff_matrix,
    };

    game.run(10);
}

mod movs;
pub use movs::Moves;

// Struct that holds the state of the game at a given time
#[derive(Clone, Debug)]
pub struct GameState {
    pub turn: i32,
    pub players: Vec<String>,
    pub moves_and_points: Vec<(Vec<Moves>, Vec<i32>)>,
}

// Implementations of structs are the functions that a struct can use
impl GameState {
    fn get_player_index(&self, player: String) -> usize {
        // Initialize arbitrary index
        let mut index: usize = self.players.len();
        for i in 0..self.players.len() {
            if self.players[i] == player {
                index = i;
                break;
            }
        }
        if index == self.players.len() {
            panic!("Player not found in game state");
        }
        index
    }

    pub fn add_players(&mut self, plays: Players) {
        for pl in plays.pl_and_strat.iter() {
            self.players.push(pl.0.clone());
        }
    }

    pub fn add_points_and_moves_to_player(&mut self, player_points: Vec<(String, Moves, i32)>) {
        for pl in player_points.iter() {
            if self.turn == 1 {
                self.moves_and_points.push((vec![pl.1], vec![pl.2]));
            } else {
                let index: usize = self.get_player_index(pl.0.clone());
                let last_score: i32 = *self.moves_and_points[index].1.last().unwrap();
                self.moves_and_points[index].0.push(pl.1);
                self.moves_and_points[index].1.push(pl.2 + last_score);
            }
        }
    }

    pub fn last_move(&self, player: String) -> Moves {
        let index: usize = self.get_player_index(player);
        *self.moves_and_points[index].0.last().unwrap()
    }

    pub fn move_at_turn(&self, player: String, turn: i32) -> Moves {
        let index: usize = self.get_player_index(player);
        let i_turn: usize = turn as usize;
        if i_turn >= self.turn as usize {
            Moves::None
        } else {
            self.moves_and_points[index].0[i_turn - 1]
        }
    }

    pub fn player_score(&self, player: String) -> i32 {
        let index = self.get_player_index(player);
        *self.moves_and_points[index].1.last().unwrap()
    }
}

// Structs that define a custom action
#[derive(Clone, Debug)]
pub struct Action {
    pub condition: Condition,
    pub act_move: Moves,
}

#[derive(Clone, Debug)]
pub enum Condition {
    Expression(BoolExpression),
}

#[derive(Copy, Clone, Debug)]
pub struct BoolExpression {
    pub b_val: fn(&GameState) -> bool,
}

// Structs that define the players and their strategies
#[derive(Clone, Debug)]
pub struct Players {
    pub pl_and_strat: Vec<(String, Strategy)>,
}

#[derive(Clone, Debug)]
pub struct Strategy {
    pub strat: Vec<Action>,
}

// Strategy space that is a one-dimensional vector of moves
#[derive(Clone, Debug)]
pub struct Strategyspace {
    pub matrix: Vec<Moves>,
}

// Payoff matrix that is a two-dimensional vector of integers
#[derive(Clone, Debug)]
pub struct Payoff {
    pub matrix: Vec<Vec<i32>>,
}

// Functions for calculating points and finding the index of a strategy in the strategy space
impl Payoff {
    pub fn calc_points(
        &self,
        stratsp: &Strategyspace,
        player_moves: Vec<(String, Moves)>,
    ) -> Vec<(String, Moves, i32)> {
        let index: usize = Payoff::find_strat_index(&stratsp, player_moves.clone());
        let mut player_points: Vec<(String, Moves, i32)> = Vec::new();
        for i in 0..self.matrix.len() {
            player_points.push((
                player_moves[i].0.clone(),
                player_moves[i].1.clone(),
                self.matrix[i][index],
            ));
        }
        player_points
    }

    pub fn find_strat_index(stratsp: &Strategyspace, player_moves: Vec<(String, Moves)>) -> usize {
        let numb_of_players: usize = player_moves.len();

        for i in (0..stratsp.matrix.len() - 1).step_by(numb_of_players) {
            for j in 0..numb_of_players {
                if stratsp.matrix[j + i] != player_moves[j].1 {
                    break;
                }
                if j == numb_of_players - 1 {
                    return i / numb_of_players;
                }
            }
        }
        Err(String::from("No index corresponding to player moves found")).unwrap()
    }

    pub fn get_player_moves(
        player_moves: Vec<(String, Vec<(Moves, i32)>)>,
    ) -> Vec<(String, Moves)> {
        let mut player_moves_vec: Vec<(String, Moves)> = Vec::new();
        for pl in player_moves.iter() {
            player_moves_vec.push((pl.0.clone(), pl.1.last().unwrap().0));
        }
        player_moves_vec
    }
}

// Struct that holds the game state and the players, strategies, strategy space and payoff matrix
#[derive(Clone, Debug)]
pub struct Game {
    pub players: Players,
    pub game_state: GameState,
    pub strat_space: Strategyspace,
    pub pay_matrix: Payoff,
}

// Implementation of the .run() method for the game, which works for the prisoners dilemma.
// Dunno if it works with other games
impl Game {
    pub fn run(mut self, turns: i32) -> Game {
        // Add players
        self.game_state.add_players(self.players.clone());
        while self.game_state.turn <= turns {

            let mut choices: Vec<(String, Vec<(Moves, i32)>)> = Vec::new();

            for player in self.players.pl_and_strat.iter() {
                for action in player.1.strat.iter() {
                    // check if the player already has a move in choices
                    let mut player_has_move: bool = false;
                    for pl in choices.iter() {
                        if pl.0.to_string() == player.0.to_string() {
                            player_has_move = true;
                            break;
                        }
                    }
                    if player_has_move {
                        break;
                    }

                    let Condition::Expression(expr) = action.condition;
                    if (expr.b_val)(&self.game_state) {
                        let move_and_turn = vec![(action.act_move, self.game_state.turn)];
                        choices.push((player.0.to_string(), move_and_turn));
                        break;
                    }
                }
            }

            // Check stratspace and player moves to get payoff
            let number_of_players = self.players.pl_and_strat.len();
            for i in (0..self.strat_space.matrix.len()).step_by(number_of_players) {
                for pl_i in 0..number_of_players {
                    if i + pl_i >= self.strat_space.matrix.len() - 1 {
                        let non_turn_choices: Vec<(String, Moves)> =
                            Payoff::get_player_moves(choices.clone());
                        let player_points: Vec<(String, Moves, i32)> = self
                            .pay_matrix
                            .calc_points(&self.strat_space, non_turn_choices);
                        self.game_state
                            .add_points_and_moves_to_player(player_points);
                        break;
                    }
                    let player_move = choices[pl_i].1.last().unwrap().0;
                    let strat_space_move = self.strat_space.matrix[i + pl_i];
                    if player_move == strat_space_move {
                        continue;
                    }
                }
            }

            self.game_state.turn += 1;
        }
        // print final scores
        for pl in self.game_state.players.iter() {
            println!(
                "Player: {}, Score: {}",
                pl,
                self.game_state.player_score(pl.to_string())
            );
        }
        return self;
    }
}

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
    pub fn new() -> Self {
        GameState {
            turn: 1,
            players: Vec::new(),
            moves_and_points: Vec::new(),
        }
    }

    fn get_player_index(&self, player: String, index: &usize) -> usize {
        if self.players[*index] == player {
            return *index;
        }
        if *index == self.players.len() {
            panic!("Player not found in game state");
        }

        self.get_player_index(player, &(index + 1))
    }

    pub fn add_players(&mut self, plays: Players, index: &usize) {
        if *index == plays.pl_and_strat.len() {
            return;
        }
        self.players.push(plays.pl_and_strat[*index].0.clone());
        self.add_players(plays, &(index + 1));
    }

    pub fn add_points_and_moves_to_player(
        &mut self,
        player_points: Vec<(String, Moves, i32)>,
        index: &usize,
    ) {
        if player_points.len() == *index {
            return;
        }

        if self.turn == 1 {
            self.moves_and_points
                .push((vec![player_points[*index].1], vec![player_points[*index].2]));
        } else {
            let player_index: usize =
                self.get_player_index(player_points[*index].0.clone(), &mut 0);
            let last_score: i32 = *self.moves_and_points[*index].1.last().unwrap();
            self.moves_and_points[player_index]
                .0
                .push(player_points[*index].1);
            self.moves_and_points[player_index]
                .1
                .push(player_points[*index].2 + last_score);
        }

        self.add_points_and_moves_to_player(player_points, &(index + 1));
    }

    pub fn last_move(&self, player: String) -> Moves {
        if self.turn == 1 {
            return Moves::None;
        }
        let index: usize = self.get_player_index(player, &mut 0);
        if self.moves_and_points.len() == 0 {
            return Moves::None;
        }
        *self.moves_and_points[index].0.last().unwrap()
    }

    pub fn move_at_turn(&self, player: String, turn: i32) -> Moves {
        let index: usize = self.get_player_index(player, &mut 0);
        let i_turn: usize = turn as usize;
        if i_turn >= self.turn as usize {
            Moves::None
        } else {
            self.moves_and_points[index].0[i_turn - 1]
        }
    }

    pub fn player_score(&self, player: String) -> i32 {
        let index = self.get_player_index(player, &mut 0);
        if self.moves_and_points.len() == 0 {
            return 0;
        }
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
        player_points: &mut Vec<(String, Moves, i32)>,
        index: &usize,
    ) {
        if self.matrix.len() == *index {
            return;
        }
        let strat_index: usize =
            Payoff::find_strat_index(&self, &stratsp, player_moves.clone(), &mut 0);
        player_points.push((
            (player_moves[*index].0.clone()).to_string(),
            (player_moves[*index].1.clone()),
            self.matrix[*index][strat_index],
        ));
        self.calc_points(&stratsp, player_moves, player_points, &(index + 1));
    }

    pub fn find_strat_index(
        &self,
        stratsp: &Strategyspace,
        player_moves: Vec<(String, Moves)>,
        index: &usize,
    ) -> usize {
        if stratsp.matrix.len() == *index {
            panic!("No index corresponding to player moves found, in find_strat_index()");
        }

        let numb_of_players: usize = player_moves.len();

        let inner_index: usize =
            Self::find_strat_index_helper(&self, stratsp, player_moves.clone(), index, &0);
        if inner_index == numb_of_players {
            return *index / numb_of_players;
        }
        Self::find_strat_index(&self, stratsp, player_moves, &(index + numb_of_players))
    }

    pub fn find_strat_index_helper(
        &self,
        stratsp: &Strategyspace,
        player_moves: Vec<(String, Moves)>,
        outer_index: &usize,
        inner_index: &usize,
    ) -> usize {
        let numb_of_players: usize = player_moves.len();
        if numb_of_players == *inner_index {
            return *inner_index;
        }
        if stratsp.matrix[*outer_index + *inner_index] != player_moves[*inner_index].1 {
            return 0;
        }
        self.find_strat_index_helper(stratsp, player_moves, outer_index, &(inner_index + 1))
    }

    pub fn get_player_moves(
        player_moves: Vec<(String, Vec<(Moves, i32)>)>,
        return_moves: &mut Vec<(String, Moves)>,
        index: &usize,
    ) {
        if player_moves.len() == *index {
            return;
        }
        return_moves.push((
            player_moves[*index].0.clone(),
            player_moves[*index].1.last().unwrap().0,
        ));

        Self::get_player_moves(player_moves, return_moves, &(index + 1));
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
    pub fn run(gm: &mut Game, turns: &i32) {
        // Add players
        gm.game_state.add_players(gm.players.clone(), &mut 0);

        gm.run_game(&turns);

        gm.printS_scores(&mut 0);
    }

    pub fn run_game(&mut self, turns: &i32) {
        if *turns == 0 {
            return;
        }

        let mut choices: Vec<(String, Vec<(Moves, i32)>)> = Vec::new();

        self.find_moves(self, &mut choices, &mut 0);

        // Check stratspace and player moves to get payoff
        let number_of_players = self.players.pl_and_strat.len();
        let mut player_moves: Vec<(String, Moves)> = Vec::new();
        Payoff::get_player_moves(choices.clone(), &mut player_moves, &mut 0);
        let mut player_points: Vec<(String, Moves, i32)> = Vec::new();
        self.pay_matrix
            .calc_points(&self.strat_space, player_moves, &mut player_points, &mut 0);
        self.game_state
            .add_points_and_moves_to_player(player_points, &mut 0);

        self.game_state.turn += 1;

        self.run_game(&(turns - 1));
    }

    pub fn find_moves(
        &self,
        gm: &Game,
        choices: &mut Vec<(String, Vec<(Moves, i32)>)>,
        index: &usize,
    ) {
        if gm.players.pl_and_strat.len() == *index {
            return;
        }

        self.find_moves_helper(gm, choices, index, &0);

        self.find_moves(gm, choices, &(index + 1));
    }

    pub fn find_moves_helper(
        &self,
        gm: &Game,
        choices: &mut Vec<(String, Vec<(Moves, i32)>)>,
        outer_index: &usize,
        inner_index: &usize,
    ) {
        // check if the player already has a move in choices
        if self.check_made_move(gm, choices, outer_index, &mut 0) {
            return;
        }

        let player = &self.players.pl_and_strat[*outer_index];
        let action = &player.1.strat[*inner_index];
        let Condition::Expression(expr) = action.condition;
        if (expr.b_val)(&gm.game_state) {
            let move_and_turn = vec![(action.act_move, gm.game_state.turn)];
            choices.push((player.0.to_string(), move_and_turn));
        }

        self.find_moves_helper(gm, choices, outer_index, &(inner_index + 1));
    }

    pub fn check_made_move(
        &self,
        gm: &Game,
        choices: &Vec<(String, Vec<(Moves, i32)>)>,
        outer_index: &usize,
        inner_index: &usize,
    ) -> bool {
        if choices.len() == *inner_index {
            return false;
        }
        if choices[*inner_index].0.to_string()
            == self.players.pl_and_strat[*outer_index].0.to_string()
        {
            return true;
        }

        self.check_made_move(gm, choices, outer_index, &(inner_index + 1))
    }

    // Prints the players and their scores in self.game_state.players using recursion
    pub fn printS_scores(&self, index: &usize) {
        if *index == self.game_state.players.len() {
            return;
        }

        println!(
            "Player: {} has score: {}",
            self.game_state.players[*index],
            self.game_state
                .player_score(self.game_state.players[*index].clone())
        );

        self.printS_scores(&(index + 1));
    }
}

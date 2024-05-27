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
        // Constructer for the GameState struct
        // This could be updated such that constructer takes parameters
        // This would allow the gamestate to be immutable and the gamestate to be updated
        // without modifying the original gamestate
        GameState {
            turn: 1,
            players: Vec::new(),
            moves_and_points: Vec::new(),
        }
    }

    // Function that returns the index of a player in the game state
    fn get_player_index(&self, player: &String, index: &usize) -> usize {
        // if the condition is true, it returns the index of the corresponding player
        if self.players[*index] == *player {
            return *index;
        }
        // Base case to stop at the end of the players vector
        if *index == self.players.len() {
            panic!("Player not found in game state");
        }

        // Recursively call get_player_index with the next index
        self.get_player_index(player, &(index + 1))
    }

    // Function to begin the game and add the players to the game state
    pub fn add_players(&mut self, plays: &Players, index: &usize) {
        // Base case condition to return when the number of players is equal to the index
        if *index == plays.pl_and_strat.len() {
            return;
        }

        // Add the players to the game state
        self.players
            .push((*plays.pl_and_strat[*index].0).to_string());

        // Recursively call add_players with the next index counting upwards
        self.add_players(plays, &(index + 1));
    }

    pub fn add_points_and_moves_to_player(
        &mut self,
        player_points: &Vec<(String, Moves, i32)>,
        index: &usize,
    ) {
        // Base case condition to stop at the end of the player points vector
        if player_points.len() == *index {
            return;
        }

        // If the turn is 1, add the points and moves to the player
        if self.turn == 1 {
            self.moves_and_points
                .push((vec![player_points[*index].1], vec![player_points[*index].2]));
        // If the turn is not 1, add the points and moves to the player
        // and add the last score to the current score
        } else {
            let player_index: usize = self.get_player_index(&player_points[*index].0, &mut 0);
            let last_score: i32 = *self.moves_and_points[*index].1.last().unwrap();
            self.moves_and_points[player_index]
                .0
                .push(player_points[*index].1);
            self.moves_and_points[player_index]
                .1
                .push(player_points[*index].2 + last_score);
        }

        // Recursively call add_points_and_moves_to_player with the next index counting upwards
        self.add_points_and_moves_to_player(player_points, &(index + 1));
    }

    // Reserved function for GTL which finds the last move of a specific player
    pub fn last_move(&self, player: &String) -> Moves {
        // If the turn is 1, return Moves::None
        // since there are no prior moves at turn 1
        if self.turn == 1 {
            return Moves::None;
        }

        // If there are no moves and points, return Moves::None
        if self.moves_and_points.len() == 0 {
            return Moves::None;
        }

        // Get the index of the player to use in the recursive call
        let index: usize = self.get_player_index(player, &mut 0);

        // Return the last move of the player
        *self.moves_and_points[index].0.last().unwrap()
    }

    // Function to find the move of a player at a specific turn
    pub fn move_at_turn(&self, player: &String, turn: i32) -> Moves {
        // Get the index of the player in the game state
        let index: usize = self.get_player_index(player, &mut 0);

        // Turn has to be usize when using it to access the vector in the else condition
        let i_turn: usize = turn as usize;

        // If the specified turn is greater than the current turn, return Moves::None
        // since it makes no sense. Else return the move of the player at the specified turn
        if i_turn >= self.turn as usize {
            Moves::None
        } else {
            self.moves_and_points[index].0[i_turn - 1]
        }
    }

    // Function to find the score of a player
    pub fn player_score(&self, player: &String) -> i32 {
        // Get the index of the player in the game state
        let index = self.get_player_index(player, &mut 0);

        // If there are no moves and points, return 0
        if self.moves_and_points.len() == 0 {
            return 0;
        }

        // Else return the score of the player
        *self.moves_and_points[index].1.last().unwrap()
    }
}

// Structs that define a custom action
#[derive(Clone, Debug)]
pub struct Action {
    // Action with its condition and the move it will make if the condition is true
    pub condition: Condition,
    pub act_move: Moves,
}

#[derive(Clone, Debug)]
pub enum Condition {
    // Enum that has a function with a boolean expression as its field
    Expression(BoolExpression),
}

#[derive(Copy, Clone, Debug)]
pub struct BoolExpression {
    // Function which will be defined by the expression in GTL
    // This will return a boolean of whether the condition is true or false
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
        player_moves: &Vec<(String, Moves)>,
        player_points: &mut Vec<(String, Moves, i32)>,
        index: &usize,
    ) {
        // Base case condition to stop at the end of the matrix
        if self.matrix.len() == *index {
            return;
        }

        // Helper function to find the index of the strategy in the strategy space
        let strat_index: usize = Payoff::find_strat_index(&self, &stratsp, player_moves, &mut 0);

        // Calculate the points of the player and add them to the address of the player_points vector
        let player_name: String = (*player_moves[*index].0).to_string();
        player_points.push((
            player_name,
            (player_moves[*index].1),
            self.matrix[*index][strat_index],
        ));

        // Recursively call calc_points with the next index
        self.calc_points(&stratsp, player_moves, player_points, &(index + 1));
    }

    // Returns the index of the strategy in the strategy space
    pub fn find_strat_index(
        &self,
        stratsp: &Strategyspace,
        player_moves: &Vec<(String, Moves)>,
        index: &usize,
    ) -> usize {
        // Base case condition to stop at the end of the matrix
        // If none of the indexes correspond to the player moves, panic
        if stratsp.matrix.len() == *index {
            panic!("No index corresponding to player moves found, in find_strat_index()");
        }

        // Get the number of players
        let numb_of_players: usize = player_moves.len();

        // Helper function to iterate through the strategy space
        let inner_index: usize =
            Self::find_strat_index_helper(&self, stratsp, player_moves, index, &0);
        // If the inner index is equal to the number of players, return the index divided by the number of players
        // Because it means that every move corresponded to the index of the strategy space
        if inner_index == numb_of_players {
            return *index / numb_of_players;
        }

        // Recursively call find_strat_index with the next index
        Self::find_strat_index(&self, stratsp, player_moves, &(index + numb_of_players))
    }

    pub fn find_strat_index_helper(
        &self,
        stratsp: &Strategyspace,
        player_moves: &Vec<(String, Moves)>,
        outer_index: &usize,
        inner_index: &usize,
    ) -> usize {
        // Get number of players
        let numb_of_players: usize = player_moves.len();

        // Base case condition to stop at the number of players and return the index of the strategy space
        if numb_of_players == *inner_index {
            return *inner_index;
        }

        // If the strategy space does not correspond to the player moves, return 0
        // and end the ideration
        if stratsp.matrix[*outer_index + *inner_index] != player_moves[*inner_index].1 {
            return 0;
        }

        // recursively call find_strat_index_helper with the next index
        self.find_strat_index_helper(stratsp, player_moves, outer_index, &(inner_index + 1))
    }

    pub fn get_player_moves(
        player_moves: &Vec<(String, Vec<(Moves, i32)>)>,
        return_moves: &mut Vec<(String, Moves)>,
        index: &usize,
    ) {
        // Base case condition to stop at the end of the player moves vector
        if player_moves.len() == *index {
            return;
        }

        // Modify the address of return_moves vector with the moves of the players
        return_moves.push((
            (*player_moves[*index].0).to_string(),
            player_moves[*index].1.last().unwrap().0,
        ));

        // recursively iterate through the player moves vector
        Self::get_player_moves(player_moves, return_moves, &(index + 1));
    }
}

// Struct that holds the game state and the players, strategies, strategy space and payoff matrix
#[derive(Debug)]
pub struct Game<'a> {
    // The fields of a game with their lifetime set to 'a
    // Lifetime is a Rust concept that ensures that references are valid for a certain scope
    // Since no lifetime is specified as 'b, the liftime 'a lives through the entire program
    pub players: &'a Players,
    pub game_state: &'a mut GameState,
    pub strat_space: &'a Strategyspace,
    pub pay_matrix: &'a Payoff,
}

// Implementation of the .run() method for the game, which works for the prisoners dilemma.
// Dunno if it works with other games
impl<'a> Game<'_> {
    pub fn run(gm: &'a mut Game, turns: &i32) -> &'a mut GameState {
        // Add players to gamestate
        gm.game_state.add_players(gm.players, &mut 0);

        // Run the game and update the gamestate
        gm.run_game(&turns);

        // Print the scores of the players
        gm.printS_scores(&mut 0);

        // Return the gamestate to be used for testing
        return gm.game_state;
    }

    pub fn run_game(&mut self, turns: &i32) {
        // returns the recursive calls to run_game when the turn is 0
        if *turns == 0 {
            return;
        }

        // Initialize the choices vector
        let mut choices: Vec<(String, Vec<(Moves, i32)>)> = Vec::new();
        // Find the previous moves of the players
        self.find_moves(&mut choices, &mut 0);

        // Init player moves and call get_player_moves
        // to modify player_moves with the moves of the players
        let mut player_moves: Vec<(String, Moves)> = Vec::new();
        Payoff::get_player_moves(&choices, &mut player_moves, &mut 0);

        // Init player_points to calc the points of the players
        // by calling calc_points with the player_moves and player_points
        let mut player_points: Vec<(String, Moves, i32)> = Vec::new();
        self.pay_matrix
            .calc_points(&self.strat_space, &player_moves, &mut player_points, &mut 0);
        // Add the points and moves to the players
        self.game_state
            .add_points_and_moves_to_player(&player_points, &mut 0);

        // Increment the turn for the gamestate
        self.game_state.turn += 1;

        // recursively call run_game with the turns - 1
        self.run_game(&(turns - 1));
    }

    // recursively modifies the choices vector with the moves of the players
    pub fn find_moves(&self, choices: &mut Vec<(String, Vec<(Moves, i32)>)>, index: &usize) {
        // Base case condition to stop at the number of players
        if self.players.pl_and_strat.len() == *index {
            return;
        }

        // Call find_moves_helper to iterate the moves of the players
        self.find_moves_helper(choices, index, &0);

        // recursively call find_moves with the next player
        self.find_moves(choices, &(index + 1));
    }

    pub fn find_moves_helper(
        &self,
        choices: &mut Vec<(String, Vec<(Moves, i32)>)>,
        outer_index: &usize,
        inner_index: &usize,
    ) {
        // check if the player already has a move in choices
        if self.check_made_move(choices, outer_index, &mut 0) {
            return;
        }

        // Find the player and the action of the player
        // Outer_index will not be modified as it is the player index
        let player = &self.players.pl_and_strat[*outer_index];

        // Inner_index will be modified to iterate the actions of the player
        let action = &player.1.strat[*inner_index];

        // Convert the condition to an expression and check if the condition is true
        let Condition::Expression(expr) = action.condition;
        if (expr.b_val)(&self.game_state) {
            let move_and_turn = vec![(action.act_move, self.game_state.turn)];
            choices.push((player.0.to_string(), move_and_turn));
        }

        // recursively call find_moves_helper with the next action
        // until an action has been chosen
        self.find_moves_helper(choices, outer_index, &(inner_index + 1));
    }

    // Returns a bool if a player has already made a move
    pub fn check_made_move(
        &self,
        choices: &Vec<(String, Vec<(Moves, i32)>)>,
        outer_index: &usize,
        inner_index: &usize,
    ) -> bool {
        // Base case condition to stop if the inner index is equal to the number of choices
        // meaning that the player has not made a move
        if choices.len() == *inner_index {
            return false;
        }
        // If this is true then the player has made a move
        if choices[*inner_index].0.to_string()
            == self.players.pl_and_strat[*outer_index].0.to_string()
        {
            return true;
        }

        // recursively call check_made_move to check the next index
        self.check_made_move(choices, outer_index, &(inner_index + 1))
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
                .player_score(&self.game_state.players[*index]),
        );

        self.printS_scores(&(index + 1));
    }
}

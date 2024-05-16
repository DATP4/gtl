#![allow(warnings)]
 mod library;
use library::{Action, BoolExpression, Condition, Game, GameState, Moves, Payoff, Players, Strategy, Strategyspace};
#[cfg(test)]
mod tests {
use super::*;

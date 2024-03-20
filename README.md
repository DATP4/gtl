# Game Theory Language (GTL)

A programming language for developing programs related to game theory, a mathematical field concerned with the behavior of rational agents, which emerged with the paper "On the Theory of Games of Strategy" by John von Neumann in 1928.

## Structure

The two main projects are found in the folders GameTheoryLanguage and GameTheoryLanguageTests.

`GameTheoryLanguage`: The compiler

`GameTheoryLanguageTests`: The testing suite for the compiler

## Installation

Following is the dependencies for installing and running the project, as well as instructions on how to do it.

### Dependencies

- `dotnet` v8.0+
- `antlr4`

### Using the project

The following commands generate the antlr4 files required by the project:

```
cd GameTheoryLanguage/parser
antlr4 -Dlanguage=CSharp Gtl.g4 -o out
```

Running the project:

```
cd GameTheoryLanguage
dotnet run
```

Running the tests:

```
cd GameTheoryLanguageTests
dotnet test
```

Optionally, on Windows, use the provided .bat scripts in the root folder to build and run the project.

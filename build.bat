@echo off
cd .\GameTheoryLanguage\Parser
antlr4 -Dlanguage=CSharp -visitor -no-listener -o out Gtl.g4
cd..
cd..
dotnet test --filter "Classname=GameTheoryLanguageTests.ParserTests"
dotnet test --filter "Classname=GameTheoryLanguageTests.TypecheckingTests"
dotnet test --filter "Classname=GameTheoryLanguageTests.TranspilerTests"
dotnet test --filter "FullyQualifiedName=GameTheoryLanguageTests.EndToEndTests.SetupTests"
cd GameTheoryLanguageTests\EndToEndTests
cargo test
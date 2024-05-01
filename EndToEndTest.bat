@echo off
dotnet test --filter "FullyQualifiedName=GameTheoryLanguageTests.EndToEndTests.SetupTests"
cd GameTheoryLanguageTests\EndToEndTests
cargo test
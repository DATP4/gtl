@echo off
cd .\GameTheoryLanguage\Parser
antlr4 -Dlanguage=CSharp -visitor -no-listener -o out Gtl.g4
cd..
cd..
dotnet test

@echo off
cd .\GameTheoryLanguage\parser
antlr4 -Dlanguage=CSharp -visitor -no-listener -o out Gtl.g4
cd..
cd..
dotnet test
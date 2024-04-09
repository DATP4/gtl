@echo off
cd .\Build
antlr4 -Dlanguage=CSharp -visitor -no-listener -o ../GameTheoryLanguage/out Gtl.g4
cd..
dotnet tes

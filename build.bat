@echo off
cd .\GameTheoryLanguage\parser
antlr4 -Dlanguage=CSharp -o out Expr.g4
cd..
cd..
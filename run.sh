cd ./Build
antlr4 -Dlanguage=CSharp -o ../GameTheoryLanguage/out Gtl.g4
cd ../GameTheoryLanguage
dotnet run


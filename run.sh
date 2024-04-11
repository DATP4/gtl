cd ./GameTheoryLanguage/Parser
antlr4 -Dlanguage=CSharp -o ../out Gtl.g4
cd ..
dotnet run

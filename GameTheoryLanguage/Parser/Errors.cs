using Antlr4.Runtime;
public class ErrorStrategy : DefaultErrorStrategy
{
    public override void ReportError(Parser recognizer, RecognitionException e)
    {
        throw new ParserException();
    }
}
public class ErrorListener : BaseErrorListener
{
    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new ParserException(msg + " in line " + line + " position " + charPositionInLine);
    }
}

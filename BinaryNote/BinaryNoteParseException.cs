namespace BinaryNote;

public class BinaryNoteParseException : Exception
{
    public BinaryNoteParseException(string message, int line, int position)
        : base(message)
    {
        Line = line;
        Position = position;
    }

    public int Line { get; }
    public int Position { get; }
}
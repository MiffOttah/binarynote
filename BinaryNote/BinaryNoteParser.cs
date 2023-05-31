using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryNote;
public sealed class BinaryNoteParser
{
    readonly string _Input;
    readonly Stream _Output;
    int _Line = 1;
    int _Position = 0;

    private BinaryNoteParser(string input, Stream output)
    {
        _Input = input;
        _Output = output;
    }

    // this method ostensibly returns char so it can
    // be used as a result in NextChar and
    // the switch statement used to parse
    // backslash escapes in ReadQuoteString
    private char Throw(string message)
    {
        throw new BinaryNoteParseException(
            $"{message} on line {_Line}",
            _Line,
            _Position - 1
        );
    }

    private char NextChar()
    {
        if (_Position < _Input.Length)
        {
            char r = _Input[_Position++];
            if (r == '\n') _Line++;
            return r;
        }
        else
        {
            return Throw("Unexpected end of input");
        }
    }

    private void Parse()
    {
        while (_Position < _Input.Length)
        {
            char ch = NextChar();
            if (CheckHexChar(ch, out int hex))
            {
                ch = NextChar();
                if (CheckHexChar(ch, out int hex2))
                {
                    _Output.WriteByte((byte)((hex << 4) | hex2));
                }
                else
                {
                    Throw($"Expected a second hex character (got '{ch}')");
                }
            }
            else if (ch == '\'')
            {
                ReadQuoteString();
            }
            else if (ch == '#')
            {
                while (_Position < _Input.Length && NextChar() != '\n')
                {
                }
            }
            else if (!char.IsWhiteSpace(ch))
            {
                // Not whitepsace. Unepxected character
                Throw($"Unexpected input (got '{ch}')");
            }
        }
    }

    private void ReadQuoteString()
    {
        char ch;
        var outputUtf8 = new StringBuilder();

        while ((ch = NextChar()) != '\'')
        {
            if (ch == '\\')
            {
                ch = NextChar();
                outputUtf8.Append(ch switch
                {
                    '\\' => '\\',
                    '\'' => '\'',
                    'r' => '\r',
                    'n' => '\n',
                    't' => '\t',
                    _ => Throw($"Unknown backslash escape: '\\{ch}'")
                });
            }
            else if (ch == '\n')
            {
                // strings cannot contain newlines
                Throw("Unexpected newline in quoted string");
            }
            else
            {
                outputUtf8.Append(ch);
            }
        }

        // convert output to utf8
        string output = outputUtf8.ToString();
        int len = Encoding.UTF8.GetByteCount(output);
        Span<byte> buffer = stackalloc byte[len];
        int len2 = Encoding.UTF8.GetBytes(output, buffer);
        Debug.Assert(len == len2); // make sure that the amount of bytes written match the expected
        _Output.Write(buffer);
    }

    private static bool CheckHexChar(char ch, out int hex)
    {
        const string HEX_DIGITS = "0123456789abcdefABCDEF";
        hex = HEX_DIGITS.IndexOf(ch);
        if (hex >= 16) hex -= 6;
        return hex >= 0;
    }

    public static void Parse(string input, Stream output)
    {
        var parser = new BinaryNoteParser(input, output);
        parser.Parse();
    }

    public static byte[] Parse(string input)
    {
        var ms = new MemoryStream();
        var parser = new BinaryNoteParser(input, ms);
        parser.Parse();
        return ms.ToArray();
    }

    //enum ParseState
    //{
    //    Default,
    //    HexDigit,
    //    Comment,
    //    SingleQuotes,
    //    Backslash
    //}
}

# BinaryNote

BinaryNote is a way to easily generate specific binary files from human readable source.

## Syntax

Hex digits are copied directly into the output. Hex digits for each byte may not have anything else between them.

Any text in `'single quotes'` is copied verbatim, encoded in utf8. Quote strings may not expand across lines.

Single quotes allow the following backslash escapes: `\\`, `\r`, `\n`, `\t`, `\'`

Any # outside of single quotes starts a comment until the end of the line.

Any other non-whitespace character is a syntax error.

## Command line

    # Windows
    BinaryNote.exe INPUT_FILE OUTPUT_FILE

    # Linux
    dotnet BinaryNote.dll INPUT_FILE OUTPUT_FILE

If there are no parameters given, will read from stdin and write to stdout.

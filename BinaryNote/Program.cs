using BinaryNote;

try
{
    if (args.Length == 0)
    {
        using var stdout = Console.OpenStandardOutput();
        BinaryNoteParser.Parse(Console.In.ReadToEnd(), stdout);
    }
    else if (args.Length == 2)
    {
        string input = File.ReadAllText(args[0]);
        using var output = File.Open(args[1], FileMode.Create);
        BinaryNoteParser.Parse(input, output);
    }
    else if (args[0] == "/?" || args[0] == "--help")
    {
        Console.Out.WriteLine(@"BinaryNote - A way to easily generate specific binary files from human readable source.
Copyright 2023 Miff

Command line: BinaryNote [INPUT_FILE OUTPUT_FILE]
If files are omitted, will read from stdin and write to stdout.

View https://github.com/miffottah/binarynote for full info.

===

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.");
    }
    else
    {
        Console.Error.WriteLine("Incorrect number of command line arguments.");
        return 3;
    }

    return 0;
}
catch (IOException ex)
{
    Console.Error.WriteLine(ex.Message);
    return 2;
}
catch (BinaryNoteParseException ex)
{
    Console.Error.WriteLine($"Parse error: {ex.Message}");
    return 1;
}
using BinaryNote;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace BinaryNoteTest;

[TestClass]
public class BinaryNoteParserTests
{
    // https://base64.guru/converter/encode/hex is a good resource
    // to generate the base64 expected data

    [TestMethod]
    [DataRow("01234567", "ASNFZw==")]
    [DataRow("00abcdef01ABCDEF", "AKvN7wGrze8=")]
    public void TestSimpleBinaryParse(string input, string expectedBase64)
    {
        var o = BinaryNoteParser.Parse(input);
        Assert.AreEqual(expectedBase64, Convert.ToBase64String(o));
    }

    [TestMethod]
    public void TestEmptyInput()
    {
        var o = BinaryNoteParser.Parse("");
        Assert.AreEqual(0, o.Length);
    }

    [TestMethod]
    [DataRow("01 02 03", "AQID", DisplayName = "Spaces")]
    [DataRow("ab\r\ncd\r\nef", "q83v", DisplayName = "Newlines")]
    [DataRow("A1\tB2\tC3", "obLD", DisplayName = "Tabs")]
    public void TestWhitespace(string input, string expectedBase64)
    {
        var o = BinaryNoteParser.Parse(input);
        Assert.AreEqual(expectedBase64, Convert.ToBase64String(o));
    }

    [TestMethod]
    [DataRow("01 02 03 #comment\n", "AQID", DisplayName = "Comment at end with newline")]
    [DataRow("01 02 03 #comment", "AQID", DisplayName = "Comment at end, no newline")]
    [DataRow("01 02#comment\n03", "AQID", DisplayName = "Comment with data afterwards")]
    [DataRow("01 02#\n03", "AQID", DisplayName = "Line ends with comment, data afterwards")]
    [DataRow("01 02 03#", "AQID", DisplayName = "Last character is #")]
    public void TestComment(string input, string expectedBase64)
    {
        var o = BinaryNoteParser.Parse(input);
        Assert.AreEqual(expectedBase64, Convert.ToBase64String(o));
    }

    [TestMethod]
    [DataRow("'Test!'", "Test!", DisplayName = "Entire note is quote string")]
    [DataRow("5B 'Test' 5D", "[Test]", DisplayName = "Mix of hex and quote string")]
    [DataRow("5B 'æ™¡' 5D", "[æ™¡]", DisplayName = "Non-ASCII characters")]
    [DataRow("'\\\\'", "\\", DisplayName = "Backslash escape - \\\\")]
    [DataRow("'\\''", "'", DisplayName = "Backslash escape - \\'")]
    [DataRow("'\\r'", "\r", DisplayName = "Backslash escape - \\r")]
    [DataRow("'\\n'", "\n", DisplayName = "Backslash escape - \\n")]
    [DataRow("'\\t'", "\t", DisplayName = "Backslash escape - \\t")]
    public void TestQuotes(string input, string expected)
    {
        var o = BinaryNoteParser.Parse(input);
        string actual = Encoding.UTF8.GetString(o);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow("?", DisplayName = "Invalid character")]
    [DataRow("A,B", DisplayName = "Interrupted hex byte")]
    [DataRow("A", DisplayName = "Unfinished hex byte")]
    [DataRow("'\\?''", DisplayName = "Quote string with unknown backslash escape")]
    [DataRow("'\n'", DisplayName = "Quote string with newline")]
    public void TestErrors(string errorInput)
    {
        Assert.ThrowsException<BinaryNoteParseException>(() => BinaryNoteParser.Parse(errorInput));
    }
}

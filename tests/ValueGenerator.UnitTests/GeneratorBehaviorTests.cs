// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ValueGenerator.Generators.Ulid;
using ValueGenerator.Helpers;

namespace ValueGenerator.UnitTests;

[TestClass]
public sealed class GeneratorBehaviorTests
{
    [TestMethod]
    [DataRow("md5", "900150983CD24FB0D6963F7D28E17F72")]
    [DataRow("sha1", "A9993E364706816ABA3E25717850C26C9CD0D89D")]
    [DataRow("sha256", "BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD")]
    [DataRow("sha384", "CB00753F45A35E8BB5A03D699AC65007272C32AB0EDED1631A8B605A43FF5BED8086072BA1E7CC2358BAECA134C825A7")]
    [DataRow("sha512", "DDAF35A193617ABACC417349AE20413112E6FA4E89A97EA20A9EEEE64B55D39A2192992A274FC1A836BA3C23A3FEEBBD454D4423643CE80E2A9AC94FA54CA49F")]
    public void HashesMatchKnownVectors(string command, string expected)
    {
        Assert.AreEqual(expected, Compute(command + " abc"));
    }

    [TestMethod]
    [DataRow("base64", "base64d")]
    [DataRow("url", "urld")]
    [DataRow("esc:data", "uesc:data")]
    public void EncodingRoundTripPreservesUnicodeAndInternalSpaces(string encode, string decode)
    {
        const string text = "Olá  世界 + /?&= 😀";
        var encoded = Compute(encode + " " + text);
        Assert.AreEqual(text, Compute(decode + " " + encoded));
    }

    [TestMethod]
    public void UrlEncodingRetainsOriginalPlusAndPercentSemantics()
    {
        Assert.AreEqual("a+b%2bc%2f", Compute("url a b+c/"));
        Assert.AreEqual("a b+c/", Compute("urld a+b%2bc%2f"));
        Assert.AreEqual("a%20b%2Bc%2F", Compute("esc:data a b+c/"));
        Assert.AreEqual("%7A", Compute("esc:hex z"));
        Assert.AreEqual("z", Compute("uesc:hex %7A"));
    }

    [TestMethod]
    [DataRow("base64d %%%")]
    [DataRow("uesc:hex z44")]
    [DataRow("uesc:hex")]
    public void InvalidEncodedInputFailsWithoutThrowingDuringCompute(string query)
    {
        var request = new InputParser().ParseInput(query);
        Assert.IsFalse(request.Compute());
        Assert.IsFalse(string.IsNullOrWhiteSpace(request.ErrorMessage));
    }

    [TestMethod]
    public void ParserTrimsBordersAndPreservesInternalWhitespace()
    {
        var encoded = Compute(" \t BaSe64  one  two\tthree \r\n");
        Assert.AreEqual("one  two\tthree", Encoding.UTF8.GetString(Convert.FromBase64String(encoded)));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" \t\r\n")]
    [DataRow("sha111 abc")]
    [DataRow("anything text")]
    public void ParserRejectsInvalidQueries(string query)
    {
        Assert.ThrowsExactly<FormatException>(() => new InputParser().ParseInput(query));
    }

    [TestMethod]
    [DataRow("guid2")]
    [DataRow("guid6")]
    [DataRow("guid3 ns:DNS")]
    [DataRow("guid5 ns:DNS two words")]
    [DataRow("esc:hex 世界")]
    public void ParserRejectsInvalidArguments(string query)
    {
        Assert.Throws<ArgumentException>(() => new InputParser().ParseInput(query));
    }

    [TestMethod]
    [DataRow("guid")]
    [DataRow("uuid")]
    public void DefaultGuidIsVersion4(string query)
    {
        Assert.AreEqual(4, Guid.Parse(Compute(query)).Version);
    }

    [TestMethod]
    [DataRow("md5")]
    [DataRow("sha1")]
    [DataRow("sha256")]
    [DataRow("sha384")]
    [DataRow("sha512")]
    public void MissingHashContentReadsClipboardOnceAndPreservesItsBorders(string command)
    {
        const string clipboardText = "  abc\t\r\n";
        var reads = 0;
        var parser = new InputParser(() => { reads++; return clipboardText; });
        var request = parser.ParseInput(command + " ");
        Assert.IsTrue(request.Compute());
        Assert.AreEqual(1, reads);
        Assert.AreEqual(command.ToUpperInvariant() + "(" + clipboardText + ")", request.Description);
        Assert.AreNotEqual(Compute(command + " abc"), request.ResultToString());
    }

    [TestMethod]
    [DataRow("sha256 abc")]
    [DataRow("guid7")]
    [DataRow("ulid")]
    [DataRow("base64")]
    [DataRow("url abc")]
    public void ExplicitContentAndOtherGeneratorsDoNotReadClipboard(string query)
    {
        var reads = 0;
        var request = new InputParser(() => { reads++; return "clipboard"; }).ParseInput(query);
        Assert.IsTrue(request.Compute());
        Assert.AreEqual(0, reads);
    }

    [TestMethod]
    public void EmptyOrUnavailableClipboardRejectsMissingHashContent()
    {
        Assert.Throws<FormatException>(() => new InputParser(() => string.Empty).ParseInput("sha256"));
        Assert.Throws<FormatException>(() => new InputParser().ParseInput("md5"));
    }

    [TestMethod]
    public void HashRequestsAreSafeWhenEvaluatedConcurrently()
    {
        Parallel.For(0, 100, _ =>
            Assert.AreEqual("900150983CD24FB0D6963F7D28E17F72", Compute("md5 abc")));
    }

    [TestMethod]
    [DataRow("GUID7", true)]
    [DataRow("\tSHA256 abc", true)]
    [DataRow("uuidv5 ns:URL abc", true)]
    [DataRow("guid3", true)]
    [DataRow("ulid", true)]
    [DataRow("esc:hex", true)]
    [DataRow("guidance", false)]
    [DataRow("guid2", false)]
    [DataRow("guid77", false)]
    [DataRow("sha256sum abc", false)]
    [DataRow("base64dmore", false)]
    [DataRow("open guid", false)]
    [DataRow("", false)]
    public void FallbackMatchesOnlyCompleteSupportedCommandTokens(string query, bool expected)
    {
        Assert.AreEqual(expected, CommandCatalog.IsKnownQuery(query));
    }

    [TestMethod]
    public void SuggestionsAreRecognizedByFallback()
    {
        foreach (var generator in CommandCatalog.GeneratorDataList)
        {
            Assert.IsTrue(CommandCatalog.IsKnownQuery(generator.Keyword), generator.Keyword);
            Assert.IsFalse(string.IsNullOrWhiteSpace(generator.Example));
        }
    }

    [TestMethod]
    [DataRow("00000000000000000000000000000000", "00000000000000000000000000")]
    [DataRow("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", "7ZZZZZZZZZZZZZZZZZZZZZZZZZ")]
    [DataRow("01563E3AB5D3D6764C61EFB99302BD5B", "01ARZ3NDEKTSV4RRFFQ69G5FAV")]
    public void UlidMatchesKnownEncodingVectors(string hex, string expected)
    {
        var request = new UlidRequest { Result = Convert.FromHexString(hex), IsSuccessful = true };
        Assert.AreEqual(expected, request.ResultToString());
    }

    [TestMethod]
    public void UlidContainsCurrentTimestampAndRandomness()
    {
        var before = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var request = new UlidRequest();
        Assert.IsTrue(request.Compute());
        var timestamp = (long)(BinaryPrimitives.ReadUInt64BigEndian(request.Result) >> 16);
        Assert.IsTrue(timestamp >= before && timestamp <= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        Assert.AreEqual(26, request.ResultToString().Length);
        Assert.IsTrue(request.ResultToString().All(character => "0123456789ABCDEFGHJKMNPQRSTVWXYZ".Contains(character)));
        Assert.AreNotEqual(request.ResultToString(), Compute("ulid"));
    }

    private static string Compute(string query)
    {
        var request = new InputParser().ParseInput(query);
        Assert.IsTrue(request.Compute(), request.ErrorMessage);
        return request.ResultToString();
    }
}

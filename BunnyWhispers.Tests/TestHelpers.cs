using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BunnyWhispers.Tests;

internal static class TestHelpers
{
    public static void AssertEqualBytes(byte[] expected, byte[] actual)
    {
        Assert.AreEqual(BitConverter.ToString(expected), BitConverter.ToString(actual));
    }
}

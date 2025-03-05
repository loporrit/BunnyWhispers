using BunnyWhispers.Crypto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace BunnyWhispers.Tests;

[TestClass]
public class XorCipherTest
{
    [TestMethod]
    public void XorCipherTransform()
    {
        var input = Encoding.UTF8.GetBytes("Hello, world!");
        var otp = MockUtil.GetRandomBytes(input.Length);
        var ciphertext = XorCipher.Transform(input, otp);
        var plaintext = XorCipher.Transform(ciphertext, otp);
        TestHelpers.AssertEqualBytes(input, plaintext);
    }
}

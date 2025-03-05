using System.Runtime.CompilerServices;

namespace BunnyWhispers.Tests;

internal static class MockUtil
{
    private static string _lastCaller = "";
    private static uint _rngState = 0x12345678;

    public static byte[] GetRandomBytes(int length, [CallerMemberName] string callerName = "")
    {
        if (!_lastCaller.Equals(callerName, StringComparison.Ordinal))
        {
            _rngState = 0x12345678;
            _lastCaller = callerName;
        }

        var result = new byte[length];

        for (int i = 0; i < length; ++i)
        {
            _rngState = (uint)(_rngState * 48271UL % 0x7FFFFFFFUL);
            result[i] = (byte)((_rngState >> 24) & 0xFF);
        }

        return result;
    }
}

namespace BunnyWhispers.Crypto;

public static class XorCipher
{
    public static byte[] Transform(byte[] input, byte[] otp)
    {
        if (input.Length != otp.Length) throw new CryptoException("plaintext.Length != OTP.Length");
        byte[] result = new byte[input.Length];
        for (int i = 0; i < input.Length; ++i)
            result[i] = (byte)(input[i] ^ otp[i]);
        return result;
    }
}

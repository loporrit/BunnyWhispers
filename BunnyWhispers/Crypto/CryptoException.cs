namespace BunnyWhispers.Crypto;

public class CryptoException : Exception
{
    public CryptoException()
        : base("An error occured in the cryptographic library")
    { }

    public CryptoException(string msg)
        : base($"An error occured in the cryptographic library: {msg}")
    { }

    public CryptoException(Exception innerException)
        : base("An error occured in the cryptographic library", innerException)
    { }

    public CryptoException(string msg, Exception innerException)
        : base($"An error occured in the cryptographic library: {msg}", innerException)
    { }
}

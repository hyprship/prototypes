using System;

namespace Bearz.Crypto
{
    public interface IEncryptionProvider
    {
        byte[] Encrypt(byte[] data);

        ReadOnlySpan<byte> Encrypt(ReadOnlySpan<byte> data);

        byte[] Decrypt(byte[] encryptedData);

        ReadOnlySpan<byte> Decrypt(ReadOnlySpan<byte> data);
    }
}
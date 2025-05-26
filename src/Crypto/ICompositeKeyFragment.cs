using System;

namespace Bearz.Crypto
{
    public interface ICompositeKeyFragment : IDisposable
    {
        ReadOnlySpan<byte> ToReadOnlySpan();
    }
}
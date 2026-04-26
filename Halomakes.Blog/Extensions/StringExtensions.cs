using System.Security.Cryptography;
using System.Text;
using ByteAether.Ulid;

namespace Halomakes.Blog.Extensions;

public static class StringExtensions
{
    public static string ToUlid(this ReadOnlySpan<char> input)
    {
        var encoding = Encoding.UTF8;
        var inputByteCount = encoding.GetByteCount(input);
        using var md5 = MD5.Create();

        var bytes = inputByteCount < 1024
            ? stackalloc byte[inputByteCount]
            : new byte[inputByteCount];
        Span<byte> destination = stackalloc byte[md5.HashSize / 8];

        encoding.GetBytes(input, bytes);

        md5.TryComputeHash(bytes, destination, out _);

        return Ulid.New(destination[..16]);
    }
}
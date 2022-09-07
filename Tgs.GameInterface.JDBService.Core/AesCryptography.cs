using System.Security.Cryptography;
using System.Text;

namespace Tgs.GameInterface.JDBService.Core;

public class AesCryptography : IAesCryptography
{
    public async ValueTask<string> GetAesEncryptAsync(
        string data,
        string secretKey,
        string secretIV,
        CancellationToken cancellationToken = default)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
        byte[] ivBytes = Encoding.UTF8.GetBytes(secretIV);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = keyBytes;
        aes.IV = ivBytes;

        using var cryptoTransform = aes.CreateEncryptor();
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
        await cryptoStream
                .WriteAsync(
                    buffer: dataBytes,
                    offset: 0,
                    count: dataBytes.Length,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        await cryptoStream
                .FlushFinalBlockAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        return Convert.ToBase64String(memoryStream.ToArray());
    }
}

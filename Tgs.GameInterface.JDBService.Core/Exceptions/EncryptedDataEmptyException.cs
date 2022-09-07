namespace Tgs.GameInterface.JDBService.Core.Exceptions;

public class EncryptedDataEmptyException : Exception
{
    public EncryptedDataEmptyException() : base("Encrypted data is empty")
    {
    }
}

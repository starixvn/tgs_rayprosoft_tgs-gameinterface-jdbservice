namespace Tgs.GameInterface.JDBService.Core.Exceptions;

public class JsonNodeEmptyException : Exception
{
    public JsonNodeEmptyException() : base("JsonNode is empty")
    {
    }
}

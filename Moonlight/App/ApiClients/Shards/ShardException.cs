using System.Runtime.Serialization;

namespace Moonlight.App.ApiClients.Shards;

[Serializable]
public class ShardException : Exception
{
    public int StatusCode { get; set; }

    public ShardException()
    {
    }

    public ShardException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public ShardException(string message) : base(message)
    {
    }

    public ShardException(string message, Exception inner) : base(message, inner)
    {
    }

    protected ShardException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
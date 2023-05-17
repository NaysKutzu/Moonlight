using System.Runtime.Serialization;

namespace Moonlight.App.ApiClients.ShardProxies;

[Serializable]
public class ShardProxyException : Exception
{
    public int StatusCode { get; set; }

    public ShardProxyException()
    {
    }

    public ShardProxyException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public ShardProxyException(string message) : base(message)
    {
    }

    public ShardProxyException(string message, Exception inner) : base(message, inner)
    {
    }

    protected ShardProxyException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
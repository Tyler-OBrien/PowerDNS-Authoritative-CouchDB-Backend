namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

public class CouchDBException : Exception
{
    public CouchDBException()
    {
    }

    public CouchDBException(string content) : base(content)
    {
        Reason = content;
    }

    public CouchDBException(string content, Exception innerException) : base(content, innerException)
    {
        Reason = content;
    }

    public string? Reason { get; }
}
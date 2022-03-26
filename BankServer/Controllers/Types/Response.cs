namespace BankServer.Controllers.Types;

public class Response
{
    public ResponseStatus? Status { get; set; }
    public string? Message { get; set; }

    public Response(ResponseStatus status, string? message = null)
    {
        Status = status;
        Message = message;
    }
}
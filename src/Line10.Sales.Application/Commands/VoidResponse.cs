namespace Line10.Sales.Application.Commands;

public sealed record VoidResponse : BaseResponse
{
    public static readonly VoidResponse Success = new VoidResponse();
}
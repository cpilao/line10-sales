using Line10.Sales.Core;

namespace Line10.Sales.Application;

public abstract record BaseResponse
{
    public bool IsSuccess => !Errors.Any();
    public IEnumerable<Error> Errors { get; init; } = [];
    
    public static T GetErrorResponse<T>(params Error[] errors)
        where T: BaseResponse, new()
    {
        var response = new T
        {
            Errors = errors
        };
        return response;
    }
}
namespace Line10.Sales.Core;

public class Result
{
    public static readonly Result Success = new ();

    private readonly List<Error> _list = new();
    public IEnumerable<Error> Errors => _list;

    public bool HasErrors => Errors.Any();
    public bool IsSuccess => !HasErrors;

    public void Add(Error error)
    {
        _list.Add(error);
    }
    
    public static Result Create(params Error[] errors)
    {
        var notification = new Result();
        errors.ToList().ForEach(e => notification.Add(e));
        return notification;
    }
    
    public static Result<T> Create<T>(T content)
    {
        ArgumentNullException.ThrowIfNull(content);
        var notification = new Result<T>(content);
        return notification;
    }

    public static Result<T> Create<T>(params Error[] errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        if (errors.Length == 0)
        {
            throw new ArgumentException("Error list is empty", nameof(errors));
        }
        
        var notification = new Result<T>();
        errors.ToList().ForEach(e => notification.Add(e));
        return notification;
    }
}

public class Result<T>: Result
{
    public T? Content { get; }
    
    internal Result(T? content = default)
    {
        Content = content;
    }
}

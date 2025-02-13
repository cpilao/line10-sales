using System.Linq.Expressions;

namespace Line10.Sales.Core;

public static class ExpressionHelper
{
    public static Expression<Func<T, object>> GetExpression<T>(string fieldName)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression body = parameter;

        foreach (var member in fieldName.Split('.'))
        {
            body = Expression.PropertyOrField(body, member);
        }

        var convertedBody = Expression.Convert(body, typeof(object));
        return Expression.Lambda<Func<T, object>>(convertedBody, parameter);
    }
}
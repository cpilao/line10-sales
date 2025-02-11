using System.Linq.Expressions;

namespace Line10.Sales.Core;

public static class ExpressionHelper
{
    public static Expression<Func<T, object>> GetExpression<T>(string fieldName)
    {
        // Get the property info
        var propertyInfo = typeof(T).GetProperty(fieldName);
        if (propertyInfo == null)
        {
            throw new ArgumentException($"'{fieldName}' is not a valid property of type '{typeof(T).Name}'");
        }

        // Create the parameter expression
        var parameter = Expression.Parameter(typeof(T), "o");

        // Create the property expression
        var property = Expression.Property(parameter, propertyInfo);

        // Convert the property to object type
        var convert = Expression.Convert(property, typeof(object));

        // Create the lambda expression
        var lambda = Expression.Lambda<Func<T, object>>(convert, parameter);

        return lambda;
    }
}
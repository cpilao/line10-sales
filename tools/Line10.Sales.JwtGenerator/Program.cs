// See https://aka.ms/new-console-template for more information

using Line10.Sales.Core.Security;
using TextCopy;

var token = JwtUtils.GetToken([
    "orders.read",
    "orders.write",
    "customers.read",
    "customers.write",
    "products.read",
    "products.write"
]);

ClipboardService.SetText(token);
Console.WriteLine("Token copied to clipboard:");
Console.WriteLine(token);
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// app.MapPost("/user", () =>
//     new { Name = "Eberton Sobreira", Idade = 41 });

// app.MapGet("/AddHeader", (HttpResponse response) =>
// {
//     response.Headers.Add("Teste", "Teste Sobreira");
//     return new { Name = "Eberton Sobreira", Idade = 41 };
// });

app.MapPost("/products", (Product product) => ProductRepository.Add(product));

// app.MapGet("/getProduct", ([FromQuery] string dateStart, [FromQuery] string dateEnd)
//     => dateStart + " - " + dateEnd);

app.MapGet("/products/{code}", ([FromRoute] string code)
    => ProductRepository.GetByCode(code));

// app.MapPut("/products", (Product product) =>
// {
//     var productSaved = ProductRepository.GetByCode(product.Code);
//     productSaved.Name = product.Name;
// });

app.MapGet("/get-all-products", () => ProductRepository.GetAll());

app.MapPut("/products/{code}", ([FromRoute] string code, Product product) =>
{
    var productSaved = ProductRepository.GetByCode(code);
    productSaved.Name = product.Name;
});

app.MapDelete("/products/{code}", ([FromRoute] string code) => ProductRepository.Delete(code));

// app.MapGet("/get-product-by-header", (HttpRequest request)
//     => request.Headers["product-code"].ToString());

app.Run();

public static class ProductRepository
{
    public static List<Product> Products { get; set; }
        = new List<Product>();

    public static void Add(Product product)
    {
        Products.Add(product);
    }

    public static Product GetByCode(string code)
    {
        return Products.FirstOrDefault(x => x.Code == code);
    }

    public static void Delete(string code)
    {
        Products.Remove(Products.First(x => x.Code == code));
    }

    public static List<Product> GetAll() => Products.ToList();
}

public class Product
{
    public string Code { get; set; }
    public string Name { get; set; }
}
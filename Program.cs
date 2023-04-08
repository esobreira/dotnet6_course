using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var c = new ConfigurationBuilder();
//var configuration = c.AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapGet("/", () => "Hello World!");

if (app.Environment.IsDevelopment())
{
    app.MapGet("/configuration/database", () => Results.Ok($"{configuration["Database::Connection"]}/{configuration["Database::Port"]}"));
}

app.MapPost("/user", () =>
    new { Name = "Eberton Sobreira", Idade = 41 });

// app.MapGet("/AddHeader", (HttpResponse response) =>
// {
//     response.Headers.Add("Teste", "Teste Sobreira");
//     return new { Name = "Eberton Sobreira", Idade = 41 };
// });

app.MapGet("/products", () => ProductRepository.GetAll());

app.MapGet("/products/{code}", ([FromRoute] string code)
    =>
{
    var product = ProductRepository.GetByCode(code);
    if (product is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(product);
});

app.MapPost("/products", (Product product) =>
{
    ProductRepository.Add(product);
    return Results.Created("/products/" + product.Code, product);
});

app.MapPut("/products/{code}", ([FromRoute] string code, Product product) =>
{
    var productSaved = ProductRepository.GetByCode(code);
    if (productSaved is null)
    {
        return Results.NotFound();
    }
    productSaved.Name = product.Name;
    return Results.Ok();
});

app.MapDelete("/products/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.GetByCode(code);
    if (product is null)
    {
        return Results.NotFound();
    }
    ProductRepository.Delete(code);
    return Results.Ok();
});

app.Run();

public static class ProductRepository
{
    public static List<Product> Products { get; set; }
        = new List<Product>();

    public static void Init(IConfiguration configuration)
    {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

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
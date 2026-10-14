using OrderService.Clients;
using OrderService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<OrderRepository>();
builder.Services.AddHttpClient<CatalogClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:Catalog"]!));
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.Run();
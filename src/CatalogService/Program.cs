using Asp.Versioning;
using CatalogService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<ClientRepository>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2");

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = false;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}.json");
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
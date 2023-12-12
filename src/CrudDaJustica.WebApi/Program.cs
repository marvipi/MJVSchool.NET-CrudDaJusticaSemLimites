var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("VirtualRepository", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["HeroRepoServiceUrls:VirtualRepository"]!);
});

builder.Services.AddHttpClient("JsonRepository", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["HeroRepoServiceUrls:JsonRepository"]!);

});

builder.Services.AddHttpClient("SqlServerRepository", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["HeroRepoServiceUrls:SqlServerRepository"]!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

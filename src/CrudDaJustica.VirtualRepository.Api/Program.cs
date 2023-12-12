using CrudDaJustica.Data.Lib.Repositories;
using CrudDaJustica.Data.Lib.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<PagingService>();
builder.Services.AddSingleton<HeroRepository, VirtualHeroRepository>(serviceProvider =>
{
    var pagingService = serviceProvider.GetRequiredService<PagingService>();
    const int INITIAL_SIZE = 10;
    return new(pagingService, INITIAL_SIZE);
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

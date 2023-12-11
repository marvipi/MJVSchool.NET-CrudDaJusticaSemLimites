using CrudDaJustica.Data.Lib.Repositories;
using CrudDaJustica.Data.Lib.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
var heroDataDir = builder.Configuration["HeroDataDir"];
var heroDataFile = builder.Configuration["HeroDataFile"];
string heroDataFilePath = Path.Combine(appDataDir, heroDataDir!, heroDataFile!);

builder.Services.AddScoped<PagingService>();
builder.Services.AddScoped<HeroRepository, JsonHeroRepository>(serviceProvider =>
{
	var pagingService = serviceProvider.GetRequiredService<PagingService>();
	return new(pagingService, heroDataFilePath);
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

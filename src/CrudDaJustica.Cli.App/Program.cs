using CrudDaJustica.Cli.App.Controllers;
using CrudDaJustica.Cli.App.Views;
using CrudDaJustica.Data.Lib.Repositories;
using CrudDaJustica.Data.Lib.Services;

var sqlServerUsername = Environment.GetEnvironmentVariable("MJVSCHOOLDB_USERNAME");
var sqlServerPassword = Environment.GetEnvironmentVariable("MJVSCHOOLDB_PASSWORD");
var connectionString = string.Format(
    "Server=DESKTOP-GI663U1\\SQLEXPPERSONAL;Database=CrudDaJusticaDev;User Id={0};Password={1};TrustServerCertificate=true;",
    sqlServerUsername,
    sqlServerPassword);

var pagingService = new PagingService();
var sqlServerHeroDal = new SqlServerHeroDapper(connectionString);
var heroRepository = new SqlServerHeroRepository(pagingService, sqlServerHeroDal);
var heroController = new HeroController(heroRepository);

var cli = new CLI(heroController);
cli.Start();


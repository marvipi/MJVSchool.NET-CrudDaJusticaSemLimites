using CrudDaJustica.Cli.App.Controllers;
using CrudDaJustica.Cli.App.Views;
using CrudDaJustica.Data.Lib.Repositories;
using CrudDaJustica.Data.Lib.Services;

var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
string heroDataFilePath = Path.Combine(appDataDir, "CRUD da Justica Dev", "heroData.json");

var pagingService = new PagingService();
var heroRepository = new JsonHeroRepository(pagingService, heroDataFilePath);
var heroController = new HeroController(heroRepository);

var cli = new CLI(heroController);
cli.Start();


# MJV School .NET - CRUD da Justiça
O CRUD da Justiça é uma aplicação simples que lida com informações sobre super-heróis. Programas desse tipo são maçantes por natureza, e por isso, escolhi um tema leve para tentar torná-lo menos entediante.
Ele possui duas interfaces de usuário: uma web implementada com o ASP.NET MVC e uma de console. Você pode vê-las na seção de imagens deste readme.

### Sumário
- [Melhorias](#melhorias)
- [Design patterns e padrões de projeto](#design-patterns-e-padr%C3%B5es-de-projeto)
- [Diagramas UML](#diagramas-uml)
- [Imagens](#imagens)

### Melhorias
- Manipulação dos dados através de Stored Procedures.
- Acesso aos dados protegido pelas Views do SQL Server.
- Microsserviço de paginação desacoplado dos controladores e repositórios.

### Design patterns e padrões de projeto
Estes conceitos não foram abordados durante o bootcamp, porém apliquei alguns deles para assegurar a qualidade da implementação.
- MVC: o website é estruturado neste padrão.
- MVP e Mediator: a comunicação entre a interface de linha de comando e o backend é mediada pelos controladores.
- MVVM: ambas as interfaces de usuário seguem este padrão.
- Repository ou Façade: os controladores não sabem com quais repositórios eles se comunicam.
- Decorator: a interface de linha de comando é implementada neste padrão.

### Diagramas UML
#### Arquitetura do projeto
![Arquitetura da solução](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/CrudDaJustica.Arquitetura.png)
#### Website
![Design do website](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/CrudDaJustica.Website.png)
#### WebApi
![Design do WebApi](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/CrudDaJustica.WebApi.png)
#### Camada de dados
![Design da camada de dados](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/CrudDaJustica.Data.Lib.png)
#### Aplicativo de linha de comando
![Design do aplicativo de linha de comando](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/CrudDaJustica.Cli.App.png)
#### Interface de linha de comando
![Design da interface de linha de comando](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/CrudDaJustica.Cli.Lib.png)

### Imagens
#### REST API
![Endpoints](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/web-api.png)

#### Website
![Listagem no website](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/web-heroes.png)
![Prompt de confirmação](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/web-confirmation-prompt.png)
![Validação nos formulários de criação de hérois](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/web-createhero-validation.png)
![Validação nos formulários de atualização de hérois](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/web-updatehero-validation.png)

#### Linha de comando
![Listagens na linha de comando](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/cli.png)
![Formulários na linha de comando](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/cli-form.png)
![Validação de formulários na linha de comando](https://github.com/marvipi/MJVSchool.NET-CrudDaJusticaSemLimites/blob/stable/res/cli-form-validation.png)

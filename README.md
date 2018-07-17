# TempCertificados

Projeto voltado para testes e estudo. A ideia é gerar um certificado por meio de uma aplicação web e enviar para as respectivas pessoas por e-mail.

## Configuração inicial

Este projeto foi feito utilizando sistema operacional macOS High sierra versão - 10.13.4.
Foi utilizado o [Visual Studio for mac](https://visualstudio.microsoft.com/vs/mac/) mantendo suas configugações de instalação. Iniciando um projeto ASP.NET Core MVC (ver [aqui](https://docs.microsoft.com/pt-br/aspnet/core/tutorials/first-mvc-app-mac/start-mvc?view=aspnetcore-2.1) para mais detalhes)

### Instalando e rodando

Apos seguir os passos para configuração inicial é necessário adicionar pacotes NuGet no projeto (veja [aqui] (https://docs.microsoft.com/pt-br/visualstudio/mac/nuget-walkthrough)).

Para este projeto foram utilizados os seguintes:

* iTextSharp (para criação de pdfs)
* Sendgrid (para envio dos e-mails, API [aqui](https://sendgrid.com/docs/API_Reference/index.html))

Antes de iniciar o projeto no arquivo appsettings.json e colocar a sua API_KEY onde necessário lá no lugar de "PUT_YOUR_API_KEY_HERE" por sua chave "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"

```
"SENDGRID_API_KEY": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" 
```

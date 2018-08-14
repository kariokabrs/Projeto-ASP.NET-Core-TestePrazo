
# Projet-ASP.NET-Core-TestePrazo #

## Arquitetura e info técnicas ##

* Uso do Visual Studio 2017;
* Versão CORE 2.0
* MVC;
* Separation of Concerns;
* Pattern Async Task;
* Strongly Typed;
* DTO´s;
* Uso do Identity 3.0.
* Linguagem C#;
* Dependency Injection pattern;
* Uso de Bind como parâmetro dos métodos do Controller para evitar Over-posting;
* Uso de Html Helpers;
* Uso de Tag Helpers;
* Uso do Code First / EF Core / Migrations;
* Uso de Bando de dados SQL Server;
* Classe de validação dos modelos personalizada;
* Uso de BootStrap template do Visual Studio;
* CSS , HTML e JS;
* JQuery/Ajax;
* Página de Erro personalizada;
* Uso de configurações de Front-end personalizada;
* Uso de Partial Viewss com Razor Views;
* Expressões Lamba com LINQ;
* Interpolação de String: $.
* URL QueryString criptogrfada: Não ( não requerida no teste )

## Sobre o Identity - Registrar usuário ##

* Pode ter a partir de cinco letras para usuario podendo ser minúsculo e/ou maiúsculas;
* Pode ter a partir de cinco letras para senha;
* Já existe um usuario 'admin' senha 'admin' cadastrado conforme requerido no teste e o único Adminstrador do Sistema a incluir outros usuários, mas caso esteja instalado em outro sistema, por ser code first, no midlleware um novo usuario admin será criado. 
* A política da senha está configurada para aceitar qualquer tipo de senha sem necessidade de letras maiúsculas, dígitos e caracteres especias;
* Para logar não é necessário, nessa configuração, que seja o email obirgatório. Foi definido o nome do usuário;
* A confirmação de email não foi ativa por não constar no teste;
* O login de thrid parties, como facebook, não foi ativado por não constar no teste;
* O usuário clicando em Oi 'usuario' pode alterar seus dados como nome, telefone e email;
* O usuário pode trocar sua senha também na mesma tela clicando em Oi 'usuario' e depois em Senha no menu à esquerda;
* A senha é hashed no Banco de Dados na Tabela de AspNetUsers;
* O id do usuário é GUID.
* Usuário inativo por 10 minutos é redirecionado para Login novamente;
* Após 3 tentativas o usuário é bloqueado e deve aguardar 1 minuto para nova 3 tentativas de logar. 

## Roles ##

* Admin;
* UsuarioBasico.

## Telas ##

* Home ( visto por todos usuários logados ou não );
* Tarefas ( visto pelos usuários logados );
* Login ( visto por todos usuários não logados );
* Alterar Dados da Conta ( visto por todos usuários logados );
* Registrar ( visto pelo usuario 'admin' - conforme explicado no teste onde o usuário básico verá apenas a tela de Tarefas ).

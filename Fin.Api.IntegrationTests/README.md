# Testes de Integração com TestContainers

Este projeto usa TestContainers para executar testes de integração com um banco de dados PostgreSQL real em um container Docker.

## Pré-requisitos

- Docker instalado e em execução
- .NET 8 SDK

## Como funciona

### TestContainers

Os testes de integração usam a biblioteca `Testcontainers.PostgreSql` para:

1. **Criar automaticamente um container PostgreSQL** para cada execução de teste
2. **Isolar os testes** - cada teste roda em um ambiente limpo
3. **Limpar automaticamente** - os containers são removidos após os testes

### Estrutura

- **`IntegrationTestWebApplicationFactory`**: Configura o WebApplicationFactory com TestContainer
- **`IntegrationTestBase`**: Classe base que todos os testes de integração herdam
- **Testes específicos**: Herdam de `IntegrationTestBase` e focam em cenários específicos

### Configuração do Container

```csharp
private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
    .WithImage("postgres:15-alpine")
    .WithDatabase("fintest")
    .WithUsername("postgres")
    .WithPassword("postgres")
    .WithCleanUp(true)
    .Build();
```

### Seed de Dados

O `IntegrationTestWebApplicationFactory` automaticamente:

1. Cria o banco de dados
2. Executa migrations (via `EnsureCreated()`)
3. Insere dados de teste necessários para os cenários

## Executando os Testes

### Via dotnet CLI

```bash
# Executar todos os testes de integração
dotnet test Fin.Api.IntegrationTests/

# Executar um teste específico
dotnet test Fin.Api.IntegrationTests/ --filter "UpdateTransferUseCaseIntegrationTests"

# Executar com logs detalhados
dotnet test Fin.Api.IntegrationTests/ --logger:console --verbosity:detailed
```

### Via IDE

Os testes podem ser executados diretamente no Visual Studio, Rider ou VS Code com a extensão C#.

## Vantagens dos TestContainers

### ✅ Antes (sem TestContainers)
- Dependia de um banco de dados externo
- Dados compartilhados entre testes
- Conflitos de dados
- Setup manual do ambiente

### ✅ Depois (com TestContainers)
- **Isolamento completo**: Cada teste roda em seu próprio banco
- **Sem configuração manual**: Docker faz tudo automaticamente
- **Dados limpos**: Sempre começa com dados conhecidos
- **CI/CD friendly**: Funciona em qualquer ambiente com Docker

## Estrutura dos Testes

### Classe Base
```csharp
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected HttpClient Client;
    protected IntegrationTestWebApplicationFactory Factory;
    
    // Setup e teardown automáticos
    // Autenticação helper
}
```

### Teste Exemplo
```csharp
public class UpdateTransferUseCaseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task GivenATransfer_WhenUpdateTransfer_ThenShouldUpdateTransfer()
    {
        // Arrange
        await SetAccessTokenAsync(); // Autenticação automática
        
        // Act & Assert
        var response = await Client.PutAsJsonAsync("/api/transfers/51", request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
```

## Dados de Teste

Os seguintes dados são automaticamente criados para cada teste:

- **Usuário**: `user@email.com` / `12345678`
- **Bancos**: Test Bank 1, Test Bank 2
- **Contas**: Test Account 16, Test Account 62
- **Pagamentos e Transferências**: Dados específicos para cenários de teste

## Configuração de Ambiente

As seguintes variáveis de ambiente são configuradas automaticamente:

- `FIN2025_JWT_SECRET_KEY`: Chave secreta para JWT
- `FIN2025_DATABASE_CONNECTION`: String de conexão do container PostgreSQL
- `JwtSettings`: Configurações do JWT para testes

## Troubleshooting

### Container não inicia
```bash
# Verificar se Docker está rodando
docker info

# Verificar imagens PostgreSQL disponíveis
docker images postgres
```

### Testes falham com erro de conexão
- Verificar se Docker está instalado e rodando
- Verificar se não há conflitos de porta
- Verificar logs do teste para detalhes específicos

### Performance lenta
- TestContainers pode ser mais lento na primeira execução
- Considerar usar imagem PostgreSQL menor (`postgres:15-alpine`)
- Docker cache melhora execuções subsequentes

## Próximos Passos

1. **Adicionar mais cenários de teste**
2. **Implementar factory de dados** para criar objetos de teste mais facilmente
3. **Configurar pipeline CI/CD** para executar testes automaticamente
4. **Adicionar testes de performance** usando a mesma infraestrutura

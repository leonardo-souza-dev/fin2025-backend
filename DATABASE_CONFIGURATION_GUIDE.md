# 🗄️ Guia de Configuração do Banco de Dados - Fin2025

## 📋 Configuração Segura da String de Conexão

### 🚨 **IMPORTANTE: NUNCA commite strings de conexão com senhas no Git!**

## 🔧 Configuração por Ambiente

### **Desenvolvimento Local**

#### Opção 1: User Secrets (Recomendado)
```bash
# No diretório do projeto
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=fin_db.db"
```

#### Opção 2: Variável de Ambiente
```bash
# Windows (PowerShell)
$env:FIN2025_DATABASE_CONNECTION="Data Source=fin_db.db"

# Windows (CMD)
set FIN2025_DATABASE_CONNECTION=Data Source=fin_db.db

# Linux/Mac
export FIN2025_DATABASE_CONNECTION="Data Source=fin_db.db"
```

#### Opção 3: Script Automatizado
```powershell
# Configurar via User Secrets
.\setup-database-connection.ps1 -UseUserSecrets

# Configurar via variável de ambiente
.\setup-database-connection.ps1 -UseEnvironmentVariable
```

### **Produção**

#### Azure App Service
```bash
# Via Azure CLI
az webapp config appsettings set --name "sua-app" --resource-group "seu-rg" --settings FIN2025_DATABASE_CONNECTION="Data Source=/home/site/wwwroot/fin_db.db"

# Via Portal Azure
# Settings > Configuration > Application settings > New application setting
```

#### Docker
```dockerfile
# Dockerfile
ENV FIN2025_DATABASE_CONNECTION="Data Source=/app/fin_db.db"

# docker-compose.yml
environment:
  - FIN2025_DATABASE_CONNECTION=Data Source=/app/fin_db.db
```

#### Kubernetes
```yaml
# deployment.yaml
env:
- name: FIN2025_DATABASE_CONNECTION
  value: "Data Source=/app/fin_db.db"
```

## 🗂️ Estrutura de Arquivos

### **Desenvolvimento**
```
Fin.Api/
├── fin_db.db          # Banco SQLite local
├── appsettings.json   # Configuração base
└── appsettings.Development.json
```

### **Produção**
```
/app/
├── fin_db.db          # Banco SQLite da aplicação
├── appsettings.json   # Configuração base
└── appsettings.Production.json
```

## 🔄 Migrações e Atualizações

### **Criar Migração**
```bash
# No diretório Fin.Api
dotnet ef migrations add NomeDaMigracao
```

### **Aplicar Migrações**
```bash
# Desenvolvimento
dotnet ef database update

# Produção (via código)
# As migrações são aplicadas automaticamente na inicialização
```

### **Reverter Migração**
```bash
dotnet ef database update NomeDaMigracaoAnterior
```

## 🛡️ Validações Implementadas

### **No FinDbContext**
- ✅ Verificação de string de conexão não nula
- ✅ Prioridade: Variável de ambiente > Configuração > Padrão
- ✅ Fallback para desenvolvimento local
- ✅ Criação automática de diretórios
- ✅ Tratamento de erro com mensagem clara

### **No Program.cs**
- ✅ Validação na inicialização
- ✅ Configuração via DI
- ✅ Falha rápida se não configurado
- ✅ Suporte a diferentes ambientes

## 📊 Tipos de Banco Suportados

### **SQLite (Atual)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=fin_db.db"
  }
}
```

### **SQL Server (Futuro)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Fin2025;Trusted_Connection=true;"
  }
}
```

### **PostgreSQL (Futuro)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=fin2025;Username=user;Password=pass"
  }
}
```

## 🔍 Troubleshooting

### **Erro: "String de conexão não configurada"**
1. Verificar se variável de ambiente está definida
2. Verificar se User Secrets está configurado
3. Verificar se arquivo `appsettings.json` existe
4. Reiniciar aplicação após configuração

### **Erro: "Database file not found"**
1. Verificar se o arquivo `fin_db.db` existe
2. Verificar permissões de acesso ao arquivo
3. Verificar se o caminho está correto
4. Criar banco se não existir

### **Erro: "Database is locked"**
1. Verificar se outra instância está usando o banco
2. Verificar permissões de escrita
3. Reiniciar aplicação
4. Verificar se há transações pendentes

## 📝 Checklist de Configuração

- [ ] String de conexão removida do código
- [ ] Variável de ambiente configurada
- [ ] User Secrets configurado (desenvolvimento)
- [ ] Arquivo de banco no local correto
- [ ] Permissões de acesso configuradas
- [ ] Backup do banco criado
- [ ] Documentação atualizada
- [ ] Testes de conexão implementados

## 🚨 Monitoramento

### **Health Checks**
```csharp
// Verificar se banco está acessível
public class DatabaseHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(...)
    {
        try
        {
            // Testar conexão
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}
```

### **Logs de Conexão**
```csharp
// Adicionar logs para auditoria
_logger.LogInformation("Conectando ao banco: {ConnectionString}", maskedConnectionString);
_logger.LogWarning("Falha na conexão com banco: {Error}", ex.Message);
```

## 🔄 Backup e Recuperação

### **Backup Automático**
```bash
# Script de backup
sqlite3 fin_db.db ".backup backup_$(date +%Y%m%d_%H%M%S).db"
```

### **Restauração**
```bash
# Restaurar backup
sqlite3 fin_db.db ".restore backup_20241201_120000.db"
```

## 📈 Performance

### **Configurações Recomendadas**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=fin_db.db;Cache=Shared;Mode=ReadWrite;"
  }
}
```

### **Índices Importantes**
```sql
-- Índices para melhor performance
CREATE INDEX IF NOT EXISTS idx_transactions_date ON transactions(date);
CREATE INDEX IF NOT EXISTS idx_transactions_user_id ON transactions(user_id);
CREATE INDEX IF NOT EXISTS idx_accounts_user_id ON accounts(user_id);
``` 
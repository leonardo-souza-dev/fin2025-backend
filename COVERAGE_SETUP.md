# Configuração de Cobertura de Código - SonarQube Cloud

## 📊 Problema Resolvido

O SonarQube Cloud não estava mostrando cobertura porque:

1. **Formato incorreto**: Estava usando JSON em vez de OpenCover
2. **Falta de dependências**: Não tinha `coverlet.msbuild`
3. **Configuração de CI inadequada**: Paths e parâmetros incorretos

## ✅ Soluções Implementadas

### 1. **Dependências Adicionadas**
Adicionado `coverlet.msbuild` em todos os projetos de teste:
```xml
<PackageReference Include="coverlet.msbuild" Version="6.0.0" />
```

### 2. **Configuração SonarQube**
Criado `sonar-project.properties` com configurações corretas:
```properties
sonar.cs.opencover.reportsPaths=**/coverage.opencover.xml
sonar.cs.vstest.reportsPaths=**/TestResults.xml
```

### 3. **CI/CD Corrigido**
Atualizado `.github/workflows/ci.yaml` para gerar relatórios OpenCover:
```yaml
--collect:"XPlat Code Coverage"
-- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
```

## 🚀 Como Usar

### **Localmente - Gerar Cobertura**
```bash
./generate-coverage.sh
```

### **Localmente - Executar SonarQube**
```bash
export SONAR_TOKEN=seu_token_aqui
./run-sonar-local.sh
```

### **CI/CD**
A cobertura será enviada automaticamente no push/PR para `develop`.

## 📁 Arquivos Criados/Modificados

| Arquivo | Mudança |
|---------|---------|
| `sonar-project.properties` | ✨ Novo - Configuração SonarQube |
| `generate-coverage.sh` | ✨ Novo - Script para gerar cobertura local |
| `run-sonar-local.sh` | ✨ Novo - Script SonarQube local |
| `.github/workflows/ci.yaml` | 🔧 Corrigido - Formato OpenCover |
| `*.Tests.csproj` | 🔧 Adicionado - coverlet.msbuild |

## 🔍 Verificação

### **1. Verificar cobertura local**
```bash
./generate-coverage.sh
find . -name "coverage.opencover.xml"
```

### **2. Verificar no SonarQube Cloud**
- Faça push para `develop`
- Aguarde CI concluir
- Verifique em: https://sonarcloud.io/project/overview?id=leonardo-souza-dev_fin2025-backend

## 📋 Configurações de Exclusão

Arquivos/pastas excluídos da cobertura:
- `Program.cs` (ponto de entrada)
- `Migrations/` (gerado automaticamente)
- `bin/`, `obj/` (artefatos de build)

## 🛠️ Resolução de Problemas

### **Cobertura não aparece**
1. Verifique se `coverage.opencover.xml` está sendo gerado
2. Confirme que o CI está executando sem erros
3. Verifique logs do SonarQube Scanner

### **Erro no formato**
- Use sempre `opencover` format, não `json` ou `cobertura`
- Confirme que está usando `XPlat Code Coverage`

### **CI falhando**
- Verifique se `SONAR_TOKEN` está configurado nos secrets
- Confirme que todos os testes passam antes do SonarQube

## 🎯 Próximos Passos

1. **Execute** `./generate-coverage.sh` para testar localmente
2. **Faça push** para `develop` para testar CI
3. **Verifique** SonarQube Cloud para confirmar cobertura
4. **Ajuste exclusões** conforme necessário

✅ **A cobertura agora deve aparecer no SonarQube Cloud!**

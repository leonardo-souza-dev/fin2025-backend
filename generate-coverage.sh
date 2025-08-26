#!/bin/bash

# Script para gerar relatórios de cobertura localmente para SonarQube
# Execute este script para testar se a cobertura está sendo gerada corretamente

echo "🧪 Gerando relatórios de cobertura de código..."

# Limpar artefatos anteriores
echo "🧹 Limpando artefatos anteriores..."
find . -name "TestResults" -type d -exec rm -rf {} + 2>/dev/null || true
find . -name "coverage*.xml" -exec rm -f {} + 2>/dev/null || true
find . -name "*.trx" -exec rm -f {} + 2>/dev/null || true

# Restore dependencies
echo "📦 Restaurando dependências..."
dotnet restore

# Build solution
echo "🔨 Compilando solução..."
dotnet build --configuration Release --no-restore

# Run tests with coverage (Unit Tests)
echo "🧪 Executando testes unitários com cobertura..."
dotnet test Fin.Application.Tests/ \
    --configuration Release \
    --no-build \
    --logger "trx;LogFileName=TestResults.xml" \
    --collect:"XPlat Code Coverage" \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

dotnet test Fin.Domain.Tests/ \
    --configuration Release \
    --no-build \
    --logger "trx;LogFileName=TestResults.xml" \
    --collect:"XPlat Code Coverage" \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

# Check if Docker is running for integration tests
if docker info > /dev/null 2>&1; then
    echo "🐳 Docker detectado - executando testes de integração..."
    dotnet test Fin.Api.IntegrationTests/ \
        --configuration Release \
        --no-build \
        --logger "trx;LogFileName=TestResults.xml" \
        --collect:"XPlat Code Coverage" \
        -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
else
    echo "⚠️  Docker não está rodando - pulando testes de integração"
fi

# Find and list coverage files
echo "📊 Arquivos de cobertura gerados:"
find . -name "coverage.opencover.xml" -type f | head -10

echo "📋 Arquivos de resultado de teste gerados:"
find . -name "TestResults.xml" -type f | head -10

echo ""
echo "✅ Relatórios de cobertura gerados!"
echo "📁 Você pode visualizar os arquivos coverage.opencover.xml gerados"
echo "🔍 Para executar SonarQube localmente, use:"
echo "   dotnet sonarscanner begin /k:\"seu-projeto\" /d:sonar.cs.opencover.reportsPaths=\"**/coverage.opencover.xml\""
echo "   dotnet build"
echo "   dotnet sonarscanner end"

#!/bin/bash

# Script simplificado para testar cobertura
echo "🧪 Testando cobertura de código..."

# Clean previous results
echo "🧹 Limpando resultados anteriores..."
find . -name "TestResults" -type d -exec rm -rf {} + 2>/dev/null || true

# Restore and build
echo "📦 Preparando projeto..."
dotnet restore --quiet
dotnet build --configuration Release --no-restore --quiet

# Test with coverage
echo "🔍 Executando teste com cobertura..."
dotnet test Fin.Domain.Tests/ \
    --configuration Release \
    --no-build \
    --quiet \
    --collect:"XPlat Code Coverage" \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

# Show results
echo "📊 Arquivos de cobertura gerados:"
find . -name "coverage.opencover.xml" -type f

echo "✅ Teste de cobertura concluído!"
echo "📁 Os arquivos coverage.opencover.xml estão prontos para o SonarQube"

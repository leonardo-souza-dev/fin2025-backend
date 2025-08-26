#!/bin/bash

# Script para executar testes de integração com TestContainers
# Certifique-se de que o Docker está rodando antes de executar

echo "🐳 Verificando se Docker está rodando..."
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker não está rodando. Por favor, inicie o Docker primeiro."
    exit 1
fi

echo "✅ Docker está rodando!"

echo "🧪 Executando testes de integração..."

# Executar testes com logs detalhados
dotnet test Fin.Api.IntegrationTests/ \
    --logger:console \
    --verbosity:normal \
    --configuration:Release

echo "🏁 Testes concluídos!"

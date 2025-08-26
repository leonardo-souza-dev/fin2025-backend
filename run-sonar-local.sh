#!/bin/bash

# Script para executar análise SonarQube localmente
# Requer SONAR_TOKEN como variável de ambiente

if [ -z "$SONAR_TOKEN" ]; then
    echo "❌ SONAR_TOKEN não definido. Execute:"
    echo "   export SONAR_TOKEN=seu_token_aqui"
    echo "   ./run-sonar-local.sh"
    exit 1
fi

echo "🔍 Executando análise SonarQube localmente..."

# Install SonarScanner if not installed
if ! command -v dotnet-sonarscanner &> /dev/null; then
    echo "📦 Instalando SonarScanner..."
    dotnet tool install --global dotnet-sonarscanner
fi

# Generate coverage first
echo "📊 Gerando cobertura..."
./generate-coverage.sh

echo "🚀 Iniciando análise SonarQube..."

# Begin SonarQube analysis
dotnet sonarscanner begin \
    /k:"leonardo-souza-dev_fin2025-backend" \
    /o:"ltreze" \
    /d:sonar.login="$SONAR_TOKEN" \
    /d:sonar.host.url="https://sonarcloud.io" \
    /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" \
    /d:sonar.cs.vstest.reportsPaths="**/TestResults.xml" \
    /d:sonar.coverage.exclusions="**/Program.cs,**/Migrations/**" \
    /d:sonar.exclusions="**/bin/**,**/obj/**,**/Migrations/**"

# Build
echo "🔨 Compilando..."
dotnet build --configuration Release

# End SonarQube analysis
echo "📤 Enviando para SonarQube..."
dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"

echo "✅ Análise SonarQube concluída!"
echo "🌐 Verifique os resultados em: https://sonarcloud.io/project/overview?id=leonardo-souza-dev_fin2025-backend"

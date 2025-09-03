#!/bin/bash

# Caminho do projeto de testes (ajuste se necessário)
TEST_PROJECT="./Fin.Api.IntegrationTests"

# Verifica se o projeto de testes existe
if [ ! -d "$TEST_PROJECT" ]; then
  echo "❌ Projeto de testes não encontrado: $TEST_PROJECT"
  exit 1
fi

echo "🧪 Executando testes com cobertura..."
# Roda os testes com cobertura
dotnet test $TEST_PROJECT --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Verifica se os testes foram executados com sucesso
if [ $? -ne 0 ]; then
  echo "❌ Falha na execução dos testes."
  exit 1
fi

echo "📊 Localizando arquivo de cobertura..."
# Localiza o arquivo de cobertura gerado (busca em TestResults também)
COVERAGE_FILE=$(find ./TestResults $TEST_PROJECT -type f -name "coverage.cobertura.xml" | head -n 1)

# Verifica se o arquivo existe
if [ -z "$COVERAGE_FILE" ]; then
  echo "❌ Arquivo de cobertura não encontrado."
  echo "💡 Verifique se o pacote coverlet.collector está instalado no projeto de testes."
  exit 1
fi

echo "✅ Arquivo de cobertura encontrado: $COVERAGE_FILE"

# Verifica se o reportgenerator está disponível
if ! command -v reportgenerator &> /dev/null; then
  echo "❌ ReportGenerator não encontrado. Instale com:"
  echo "dotnet tool install -g dotnet-reportgenerator-globaltool"
  exit 1
fi

echo "📈 Gerando relatório HTML..."
# Gera o relatório em HTML
reportgenerator -reports:"$COVERAGE_FILE" -targetdir:coveragereport -reporttypes:Html

# Verifica se o relatório foi gerado com sucesso
if [ $? -ne 0 ]; then
  echo "❌ Falha na geração do relatório."
  exit 1
fi

echo "✅ Relatório gerado com sucesso!"

# Verifica se o arquivo HTML existe antes de tentar abrir
if [ -f "coveragereport/index.html" ]; then
  echo "🌐 Abrindo relatório no navegador..."
  
  # Tenta abrir com Microsoft Edge
  if command -v microsoft-edge &> /dev/null; then
    microsoft-edge "file://$(pwd)/coveragereport/index.html"
  elif command -v microsoft-edge-stable &> /dev/null; then
    microsoft-edge-stable "file://$(pwd)/coveragereport/index.html"
  elif command -v edge &> /dev/null; then
    edge "file://$(pwd)/coveragereport/index.html"
  else
    echo "⚠️  Microsoft Edge não encontrado. Usando navegador padrão..."
    xdg-open coveragereport/index.html
  fi
else
  echo "❌ Arquivo index.html não encontrado em coveragereport/"
  exit 1
fi
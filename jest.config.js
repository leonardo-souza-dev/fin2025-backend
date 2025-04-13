// jest.config.js
module.exports = {
    preset: 'ts-jest', // Usar ts-jest para compilar arquivos TypeScript
    testEnvironment: 'node', // Ambiente de teste
    roots: ['<rootDir>/src', '<rootDir>/tests'], // Define onde os testes estão localizados
    moduleFileExtensions: ['ts', 'js'], // Extensões de arquivo suportadas
    testMatch: ['**/*.test.ts'], // Nome dos arquivos de teste
    verbose: true // Exibir detalhes dos testes
  };
  
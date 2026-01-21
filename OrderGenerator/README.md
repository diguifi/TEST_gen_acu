# OrderGenerator

Aplicação WebAPI em .NET 8 que recebe ordens via WebSocket, estrutura e repassa para um Accumulator usando protocolo FIX.  

**Executar o projeto OrderAccumulator antes**  
_(não foram implementadas estratégias de retry na conexão)_

## Requisitos

- .NET 8.0 ou superior

## Como Executar

### 1. Restaurar dependências
```bash
dotnet restore
```

### 2. Compilar o projeto
```bash
dotnet build
```

### 3. Executar a aplicação
```bash
dotnet run --project OrderGenerator.WebApi
```

A aplicação iniciará em `http://localhost:5283`.

## Executar Testes

```bash
dotnet test
```

## Estrutura do Projeto

- `OrderGenerator.WebApi` - Aplicação principal com API REST e WebSocket
- `OrderGenerator.Tests` - Testes unitários

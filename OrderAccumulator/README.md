# OrderAccumulator

Aplicação WebAPI em .NET 8 que recebe ordens via FIX, valida e calcula exposição, então retorna um ExecutionReport para um Initiator usando protocolo FIX.  

**Deve ser o primeiro projeto executado**  

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
cd OrderAccumulator
dotnet run
```

## Executar Testes

```bash
dotnet test
```

## Estrutura do Projeto
- `OrderAccumulator/` - Main application
- `OrderAccumulator.Tests/` - Unit tests

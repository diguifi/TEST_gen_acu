# OrderGenerator Front

Front simples que realiza ordens de compra ou venda.

## Requisitos

Para executar apenas a build:
- Um servidor estático de arquivos web (como o pacote http-server do node)

Para executar em modo desenvolvimento:
- Node ^20.19.0 || >=22.12.0

## Executar o Build Compilado

1. Caso não tenha uma aplicação de servidor web, instale o http-server globalmente:
```bash
npm install -g http-server
```

2. Navegue até a pasta build e inicie o servidor:
```bash
cd build
http-server -p 8080
```

3. Acesse http://localhost:8080 no navegador.

## Executar em Modo Desenvolvedor

1. Instale as dependências:
```bash
npm install
```

2. Inicie o servidor de desenvolvimento:
```bash
npm run dev
```

3. A aplicação será executada em http://localhost:5173 (ou a porta indicada no terminal).

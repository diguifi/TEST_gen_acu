# AVALIAÇÃO TÉCNICA DESENVOLVIMENTO

> Por [Diego Penha](https://www.linkedin.com/in/diego-penha-54a833148/).  
> - [Github](https://github.com/diguifi)  
> - [Itch](https://diguifi.itch.io/)

Este projeto implementa as partes necessárias para o funcionamento pleno do desafio proposto no processo seletivo.

## Partes

O projeto é dividido em 3 partes:
- OrderAccumulator: age como um **acceptor** na comunicação FIX; Recebe NewOrderSingles e retorna ExecutionReports (além de outras regras de negócio)
- OrderGenerator: age como um **initiator** na comunicação FIX; Recebe dados do front, estrutura e envia NewOrderSingles, recebe e devolve para o front ExecutionReports
- OrderGenerator_Front: um client simples em vue que funciona como porta de entrada do fluxo.

OrderAccumulator e OrderGenerator devem ser executados nessa respectiva ordem, mais detalhes no README do OrderGenerator.  
Cada projeto é um repositório git, caso queiram acompanhar as iterações.

## Log de atividades?

É um arquivo extra contendo toda linha de raciocínio do desenvolvimento, a motivação da criação desse arquivo está descrita no final do mesmo.


Grato!
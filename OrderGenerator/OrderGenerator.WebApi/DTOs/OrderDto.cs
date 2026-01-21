namespace OrderGenerator.WebApi.DTOs;

// Deixei nomes curtos para economizar banda na transmissão via WebSocket
// Sei que enviar JSON stringifyado não é eficiente, mas quis manter a simplicidade do exemplo
// Em um cenário real, usaria Protobuf ou MsgPack
public record OrderDto(string Sy, string Si, decimal Qt, decimal Pr);
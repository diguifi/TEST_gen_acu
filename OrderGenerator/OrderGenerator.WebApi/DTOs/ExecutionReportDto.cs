namespace OrderGenerator.WebApi.DTOs;

// deixei nomes curtos para economizar banda na transmissão via WebSocket
// sei que enviar JSON stringifyado não é eficiente, mas quis manter a simplicidade do exemplo
// em um cenário real, usaria Protobuf ou MsgPack
public class ExecutionReportDto
{
    public string? CId { get; set; }
    public string? OId { get; set; }
    public string? EId { get; set; }

    public string? Sy { get; set; }
    public string? Si { get; set; }
    public string? Sts { get; set; }
    public string? ExTy { get; set; }

    public decimal Qt { get; set; }
    public decimal CQt { get; set; }
    public decimal LvQt { get; set; }

    public decimal LQt { get; set; }
    public decimal LPx { get; set; }
    public decimal AvPx { get; set; }

    public string? Txt { get; set; } = "";
}

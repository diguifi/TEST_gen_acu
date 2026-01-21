using OrderGenerator.WebApi.DTOs;
using QuickFix.Fields;

namespace OrderGenerator.WebApi.Mappers;

public static class ExecutionReportMapper
{
    public static ExecutionReportDto ToExecutionReportDto(QuickFix.Message message)
    {
        var dto = new ExecutionReportDto();

        if (message.IsSetField(Tags.ClOrdID))
            dto.CId = message.GetString(Tags.ClOrdID);

        if (message.IsSetField(Tags.OrderID))
            dto.OId = message.GetString(Tags.OrderID);

        dto.EId = message.GetString(Tags.ExecID);
        dto.Sy = message.GetString(Tags.Symbol);
        dto.Si = MapSide(message.GetChar(Tags.Side));

        dto.Sts = MapOrdStatus(message.GetChar(Tags.OrdStatus));
        dto.ExTy = MapExecType(message.GetChar(Tags.ExecType));

        dto.Qt = message.GetDecimal(Tags.OrderQty);

        dto.CQt = message.IsSetField(Tags.CumQty)
            ? message.GetDecimal(Tags.CumQty)
            : 0;

        dto.LvQt = message.IsSetField(Tags.LeavesQty)
            ? message.GetDecimal(Tags.LeavesQty)
            : 0;

        dto.AvPx = message.IsSetField(Tags.AvgPx)
            ? message.GetDecimal(Tags.AvgPx)
            : 0;

        dto.LQt = message.IsSetField(Tags.LastQty)
            ? message.GetDecimal(Tags.LastQty)
            : 0;

        dto.LPx = message.IsSetField(Tags.LastPx)
            ? message.GetDecimal(Tags.LastPx)
            : 0;

        if (message.IsSetField(Tags.Text))
            dto.Txt = message.GetString(Tags.Text);

        return dto;
    }

    private static string MapSide(char side) =>
        side switch
        {
            '1' => "Buy",
            '2' => "Sell",
            '5' => "SellShort",
            _   => "Unknown"
        };
    private static string MapOrdStatus(char ordStatus) =>
        ordStatus switch
        {
            '0' => "New",
            '1' => "PartiallyFilled",
            '2' => "Filled",
            '4' => "Canceled",
            '8' => "Rejected",
            'A' => "PendingNew",
            '6' => "PendingCancel",
            'E' => "PendingReplace",
            'C' => "Expired",
            _   => "Unknown"
        };

    private static string MapExecType(char execType) =>
        execType switch
        {
            '0' => "New",
            '1' => "PartiallyFilled",
            '2' => "Filled",
            '4' => "Canceled",
            '8' => "Rejected",
            'F' => "Trade",
            '5' => "Replaced",
            'C' => "Expired",
            _   => "Unknown"
        };
}
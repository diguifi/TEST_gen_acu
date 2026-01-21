using OrderGenerator.WebApi.Mappers;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderGenerator.Tests.UnitTests;

public class ExecutionReportMapperTests
{
    private static ExecutionReport CreateMockExecutionReport(
        string clOrdId = "CLIENT123",
        string orderId = "ORDER123",
        string symbol = "PETR4",
        char side = '1',
        decimal qty = 100,
        char ordStatus = '2',
        char execType = '2')
    {
        var msg = new ExecutionReport(
            new OrderID(orderId),
            new ExecID("EXEC123"),
            new ExecType(execType),
            new OrdStatus(ordStatus),
            new Symbol(symbol),
            new Side(side),
            new LeavesQty(qty),
            new CumQty(qty),
            new AvgPx(25.50m)
        );

        msg.Set(new ClOrdID(clOrdId));
        msg.Set(new OrderQty(qty));
        msg.Set(new LastPx(25.50m));
        msg.Set(new LastQty(qty));
        msg.Set(new Text("Order filled"));

        return msg;
    }

    [Fact]
    public void ToExecutionReportDto_WithValidMessage_ShouldMapAllFields()
    {
        // Arrange
        var executionReport = CreateMockExecutionReport(
            clOrdId: "CLI001",
            orderId: "ORD001",
            symbol: "VALE3",
            side: '1',
            qty: 50,
            ordStatus: '2',
            execType: '2'
        );

        // Act
        var result = ExecutionReportMapper.ToExecutionReportDto(executionReport);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CLI001", result.CId);
        Assert.Equal("ORD001", result.OId);
        Assert.Equal("VALE3", result.Sy);
        Assert.Equal("Buy", result.Si);
        Assert.Equal("Filled", result.Sts);
        Assert.Equal("Filled", result.ExTy);
        Assert.Equal(50, result.Qt);
        Assert.Equal(50, result.CQt);
        Assert.Equal(25.50m, result.AvPx);
        Assert.Equal("Order filled", result.Txt);
    }

    [Theory]
    [InlineData('1', "Buy")]
    [InlineData('2', "Sell")]
    public void ToExecutionReportDto_MapSideBuy_ShouldReturnBuy(char side, string expectedSide)
    {
        // Arrange
        var msg = CreateMockExecutionReport(side: side);

        // Act
        var result = ExecutionReportMapper.ToExecutionReportDto(msg);

        // Assert
        Assert.Equal(expectedSide, result.Si);
    }

    [Theory]
    [InlineData('0', "New")]
    [InlineData('8', "Rejected")]
    public void ToExecutionReportDto_MapOrdStatusNew_ShouldReturnNew(char ordStatus, string expectedStatus)
    {
        // Arrange
        var msg = CreateMockExecutionReport(ordStatus: ordStatus);

        // Act
        var result = ExecutionReportMapper.ToExecutionReportDto(msg);

        // Assert
        Assert.Equal(expectedStatus, result.Sts);
    }

    [Fact]
    public void ToExecutionReportDto_MapExecTypeNew_ShouldReturnNew()
    {
        // Arrange
        var msg = CreateMockExecutionReport(execType: '0'); // New

        // Act
        var result = ExecutionReportMapper.ToExecutionReportDto(msg);

        // Assert
        Assert.Equal("New", result.ExTy);
    }

    [Fact]
    public void ToExecutionReportDto_WithDifferentQuantities_ShouldMapCorrectly()
    {
        // Arrange
        var msg = CreateMockExecutionReport(qty: 250);

        // Act
        var result = ExecutionReportMapper.ToExecutionReportDto(msg);

        // Assert
        Assert.Equal(250, result.Qt);
        Assert.Equal(250, result.CQt);
        Assert.Equal(250, result.LQt);
    }

    [Fact]
    public void ToExecutionReportDto_WithDifferentPrices_ShouldMapCorrectly()
    {
        // Arrange
        var executionReport = CreateMockExecutionReport();
        executionReport.Set(new AvgPx(26.75m));
        executionReport.Set(new LastPx(26.75m));

        // Act
        var result = ExecutionReportMapper.ToExecutionReportDto(executionReport);

        // Assert
        Assert.Equal(26.75m, result.AvPx);
        Assert.Equal(26.75m, result.LPx);
    }
}
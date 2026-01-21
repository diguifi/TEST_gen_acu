using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using OrderAccumulator.Acceptor;
using System.Reflection;
using System.Collections.Concurrent;

namespace OrderAccumulator.Tests.UnitTests;

public class FixApplicationTests
{
    private readonly FixApplication _fixApplication;
    private readonly SessionID _sessionID;

    public FixApplicationTests()
    {
        _fixApplication = new FixApplication();
        _sessionID = new SessionID("FIX.4.4", "ACCUMULATOR", "GENERATOR");
    }

    [Fact]
    public void OnBuyOrder_ShouldIncrementExposure()
    {
        // Arrange
        var order = CriarNewOrderSingle("PETR4", Side.BUY, 1000, 50m);

        // Act
        _fixApplication.FromApp(order, _sessionID);

        // Assert
        var exposicao = ObterExposicao(_fixApplication, "PETR4");
        Assert.Equal(50000m, exposicao);
    }

    [Fact]
    public void OnSellOrder_ShouldDecrementExposure()
    {
        // Arrange
        var order = CriarNewOrderSingle("VALE5", Side.SELL, 500, 100m);

        // Act
        _fixApplication.FromApp(order, _sessionID);

        // Assert
        var exposicao = ObterExposicao(_fixApplication, "VALE5");
        Assert.Equal(-50000m, exposicao);
    }

    [Fact]
    public void WithMultipleOrdersSameSymbol_ShouldAccumulateExposure()
    {
        // Arrange
        var order1 = CriarNewOrderSingle("ITUB4", Side.BUY, 1000, 25m);
        var order2 = CriarNewOrderSingle("ITUB4", Side.BUY, 500, 25m);

        // Act
        _fixApplication.FromApp(order1, _sessionID);
        _fixApplication.FromApp(order2, _sessionID);

        // Assert
        var exposicao = ObterExposicao(_fixApplication, "ITUB4");
        Assert.Equal(37500m, exposicao);
    }

    [Fact]
    public void WithBuyAndSellOrders_ShouldCompensateExposure()
    {
        // Arrange
        var orderCompra = CriarNewOrderSingle("BBDC4", Side.BUY, 1000, 20m);
        var orderVenda = CriarNewOrderSingle("BBDC4", Side.SELL, 500, 20m);

        // Act
        _fixApplication.FromApp(orderCompra, _sessionID);
        _fixApplication.FromApp(orderVenda, _sessionID);

        // Assert
        var exposicao = ObterExposicao(_fixApplication, "BBDC4");
        Assert.Equal(10000m, exposicao);
    }

    [Fact]
    public void OnBuyOrderExceedingPositiveLimit_ShouldRejectOrder()
    {
        // Arrange
        var order1 = CriarNewOrderSingle("WEGE3", Side.BUY, 1000000, 100m);
        var order2 = CriarNewOrderSingle("WEGE3", Side.BUY, 1000, 100m);

        // Act
        _fixApplication.FromApp(order1, _sessionID);
        _fixApplication.FromApp(order2, _sessionID);

        // Assert
        var exposicao = ObterExposicao(_fixApplication, "WEGE3");
        Assert.Equal(100000000m, exposicao);
    }

    [Fact]
    public void OnSellOrderExceedingNegativeLimit_ShouldRejectOrder()
    {
        // Arrange
        var order1 = CriarNewOrderSingle("GOLL4", Side.SELL, 1000000, 100m);
        var order2 = CriarNewOrderSingle("GOLL4", Side.SELL, 1000, 100m);

        // Act
        _fixApplication.FromApp(order1, _sessionID);
        _fixApplication.FromApp(order2, _sessionID);

        // Assert
        var exposicao = ObterExposicao(_fixApplication, "GOLL4");
        Assert.Equal(-100000000m, exposicao);
    }

    [Fact]
    public void OnOrderAtExactLimit_ShouldAcceptOrder()
    {
        // Arrange
        var order = CriarNewOrderSingle("MGLU3", Side.BUY, 1000000, 100m);

        // Act
        _fixApplication.FromApp(order, _sessionID);

        // Assert
        var exposicao = ObterExposicao(_fixApplication, "MGLU3");
        Assert.Equal(100000000m, exposicao);
    }

    [Fact]
    public void OnNonExistentSymbol_ShouldReturnZeroExposure()
    {
        // Act & Assert
        var exposicao = ObterExposicao(_fixApplication, "INEXISTENTE");
        Assert.Equal(0m, exposicao);
    }

    [Fact]
    public void WithMultipleSymbols_ShouldMaintainSeparateExposure()
    {
        // Arrange
        var orderPetr = CriarNewOrderSingle("PETR4", Side.BUY, 1000, 50m);
        var orderVale = CriarNewOrderSingle("VALE5", Side.BUY, 500, 100m);
        var orderItau = CriarNewOrderSingle("ITUB4", Side.SELL, 200, 25m);

        // Act
        _fixApplication.FromApp(orderPetr, _sessionID);
        _fixApplication.FromApp(orderVale, _sessionID);
        _fixApplication.FromApp(orderItau, _sessionID);

        // Assert
        Assert.Equal(50000m, ObterExposicao(_fixApplication, "PETR4"));
        Assert.Equal(50000m, ObterExposicao(_fixApplication, "VALE5"));
        Assert.Equal(-5000m, ObterExposicao(_fixApplication, "ITUB4"));
    }

    #region Helpers
    private static NewOrderSingle CriarNewOrderSingle(
        string symbol,
        char side,
        decimal qty,
        decimal price)
    {
        var order = new NewOrderSingle();
        order.Set(new ClOrdID(Guid.NewGuid().ToString()));
        order.Set(new Symbol(symbol));
        order.Set(new Side(side));
        order.Set(new OrderQty(qty));
        order.Set(new Price(price));
        order.Set(new OrdType(OrdType.LIMIT));
        order.Set(new TransactTime(DateTime.UtcNow));

        return order;
    }

    private static decimal ObterExposicao(FixApplication fixApplication, string symbol)
    {
        var field = typeof(FixApplication).GetField(
            "_exposicao",
            BindingFlags.NonPublic | BindingFlags.Instance);

        var exposicaoDictionary = field?.GetValue(fixApplication) as ConcurrentDictionary<string, decimal>;
        return exposicaoDictionary?.GetValueOrDefault(symbol, 0m) ?? 0m;
    }
    #endregion
}
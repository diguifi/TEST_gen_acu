using OrderGenerator.WebApi.DTOs;
using OrderGenerator.WebApi.Initiator;
using QuickFix;
using QuickFix.Fields;

namespace OrderGenerator.Tests.UnitTests;

public class FixClientTests
{
    [Fact]
    public void OnLogon_ShouldSetSessionID()
    {
        // Arrange
        var fixClient = new FixClient();
        var testSessionID = new SessionID("FIX.4.4", "SENDER", "TARGET");

        // Act
        fixClient.OnLogon(testSessionID);

        // Assert
        Assert.NotNull(fixClient.SessionID);
        Assert.Equal(testSessionID, fixClient.SessionID);
    }

    [Fact]
    public void OnExecutionReport_ShouldInvokeCallback()
    {
        // Arrange
        var fixClient = new FixClient();
        var callbackInvoked = false;
        ExecutionReportDto? capturedReport = null;

        fixClient.OnExecutionReport = (report) =>
        {
            callbackInvoked = true;
            capturedReport = report;
        };

        var mockReport = new ExecutionReportDto
        {
            OId = "ORDER123",
            CId = "CLIENT123",
            Sy = "PETR4",
            Si = "BUY",
            Qt = 100,
            LPx = 25.50m,
            CQt = 100,
            AvPx = 25.50m,
            Sts = "2",
            Txt = "Order filled"
        };

        // Act
        fixClient.OnExecutionReport?.Invoke(mockReport);

        // Assert
        Assert.True(callbackInvoked);
        Assert.NotNull(capturedReport);
        Assert.Equal("ORDER123", capturedReport.OId);
        Assert.Equal("PETR4", capturedReport.Sy);
        Assert.Equal(100, capturedReport.CQt);
    }

    [Fact]
    public void SendOrder_WithValidParameters_ShouldNotThrow()
    {
        // Arrange
        var fixClient = new FixClient();
        var sessionID = new SessionID("FIX.4.4", "SENDER", "TARGET");
        fixClient.OnLogon(sessionID);

        // Act
        var exception = Record.Exception(() =>
            fixClient.SendOrder("PETR4", Side.BUY, 100, 25.50m)
        );

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void SendOrder_WithoutSessionID_ShouldHandleGracefully()
    {
        // Arrange
        var fixClient = new FixClient();

        // Act
        var exception = Record.Exception(() =>
            fixClient.SendOrder("PETR4", Side.SELL, 50, 26.00m)
        );

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void OnExecutionReport_WithNullCallback_ShouldNotThrow()
    {
        // Arrange
        var fixClient = new FixClient();
        fixClient.OnExecutionReport = null;

        var mockReport = new ExecutionReportDto
        {
            OId = "ORDER456",
            CId = "CLIENT456",
            Sy = "VALE3",
            Si = "SELL",
            Qt = 200,
            LPx = 15.75m,
            CQt = 200,
            AvPx = 15.75m,
            Sts = "2",
            Txt = "Order filled"
        };

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            if (fixClient.OnExecutionReport != null)
            {
                fixClient.OnExecutionReport(mockReport);
            }
        });

        Assert.Null(exception);
    }
}
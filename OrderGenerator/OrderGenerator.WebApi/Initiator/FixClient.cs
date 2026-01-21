using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using OrderGenerator.WebApi.DTOs;
using OrderGenerator.WebApi.Mappers;

namespace OrderGenerator.WebApi.Initiator;
public class FixClient : IApplication
{
    public SessionID? SessionID { get; private set; }
    public Action<ExecutionReportDto>? OnExecutionReport;

    public void OnLogon(SessionID sessionID)
    {
        SessionID = sessionID;
    }

    public void FromApp(QuickFix.Message message, SessionID sessionID)
    {
        if (message.Header.GetString(Tags.MsgType) == MsgType.EXECUTION_REPORT)
        {
             OnExecutionReport?.Invoke(ExecutionReportMapper.ToExecutionReportDto(message));
        }
    }

    public void SendOrder(string symbol, char side, decimal qty, decimal price)
    {
        // imagino que um guid não seja o ideal, por ter muitos bytes, mas pro exemplo vou deixar assim
        // não conheço práticas melhores para geração de ids no FIX
        var order = new NewOrderSingle(
            new ClOrdID(Guid.NewGuid().ToString()),
            new Symbol(symbol),
            new Side(side),
            new TransactTime(DateTime.UtcNow),
            new OrdType(OrdType.LIMIT)
        );

        order.Set(new Symbol(symbol));
        order.Set(new OrderQty(qty));
        order.Set(new Price(price));
        
        try
        {
            Session.SendToTarget(order, SessionID!);
        }
        catch (SessionNotFound ex)
        {
            Console.WriteLine("==session not found exception!==");
            Console.WriteLine(ex.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void OnCreate(SessionID sessionID) { }
    public void OnLogout(SessionID sessionID) { }
    public void ToAdmin(QuickFix.Message message, SessionID sessionID) { }
    public void FromAdmin(QuickFix.Message message, SessionID sessionID) { }
    public void ToApp(QuickFix.Message message, SessionID sessionID) { }
}

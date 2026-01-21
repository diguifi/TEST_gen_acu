using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using System.Collections.Concurrent;

namespace OrderAccumulator.Acceptor;
public class FixApplication : IApplication
{
    private const decimal LIMITE = 100_000_000m;

    // entendo que essa classe funciona como um singleton e isso poderia gerar um calculo global de exposição
    // juntando todos os clients, mas como o desafio não especificava nada sobre múltiplos clients, mantive assim pela simplicidade
    private readonly ConcurrentDictionary<string, decimal> _exposicao = new();

    public void FromApp(QuickFix.Message message, SessionID sessionID)
    {
        if (message is NewOrderSingle order)
        {
            ProcessarOrdem(order, sessionID);
        }
    }

    private void ProcessarOrdem(NewOrderSingle order, SessionID sessionID)
    {
        string symbol = order.Symbol.Value;
        decimal qty = order.OrderQty.Value;
        decimal price = order.Price.Value;
        char side = order.Side.Value;

        decimal valor = qty * price;
        if (side == Side.SELL)
            valor *= -1;

        decimal exposicaoAtual = _exposicao.GetOrAdd(symbol, 0m);
        decimal novaExposicao = exposicaoAtual + valor;

        if (Math.Abs(novaExposicao) > LIMITE)
        {
            EnviarExecutionReport(order, sessionID, ExecType.REJECTED, OrdStatus.REJECTED);
            return;
        }

        _exposicao[symbol] = novaExposicao;
        // o desafio não especificava exatamente o que fazer com a exposição, então só vou logar.
        Console.WriteLine($"Ordem aceita. Nova exposição para {symbol}: {novaExposicao}");

        EnviarExecutionReport(order, sessionID, ExecType.NEW, OrdStatus.NEW);
    }

    private static void EnviarExecutionReport(
        NewOrderSingle order,
        SessionID sessionID,
        char execType,
        char ordStatus)
    {
        // imagino que guids não sejam o ideal, por ter muitos bytes, mas pro exemplo vou deixar assim
        // não conheço práticas melhores para geração de ids no FIX
        var exReport = new ExecutionReport(
            new OrderID(Guid.NewGuid().ToString()),
            new ExecID(Guid.NewGuid().ToString()),
            new ExecType(execType),
            new OrdStatus(ordStatus),
            order.Symbol,
            order.Side,
            new LeavesQty(0),
            new CumQty(order.OrderQty.Value),
            new AvgPx(order.Price.Value));

        exReport.Set(order.ClOrdID);
        exReport.Set(order.Symbol);
        exReport.Set(order.OrderQty);
        exReport.Set(new LastQty(order.OrderQty.Value));
        exReport.Set(new LastPx(order.Price.Value));

        if (order.IsSetAccount())
            exReport.SetField(order.Account);

        try
        {
            Session.SendToTarget(exReport, sessionID);
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
    public void OnLogon(SessionID sessionID) { }
    public void OnLogout(SessionID sessionID) { }
    public void ToAdmin(QuickFix.Message message, SessionID sessionID) { }
    public void FromAdmin(QuickFix.Message message, SessionID sessionID) { }
    public void ToApp(QuickFix.Message message, SessionID sessionID) {}
}

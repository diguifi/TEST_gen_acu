using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using QuickFix.Fields;
using OrderGenerator.WebApi.Initiator;
using OrderGenerator.WebApi.DTOs;
using OrderGenerator.WebApi.Enums;

namespace OrderGenerator.WebApi.Extensions;

public static class WebSocketExtensions
{
    public static void MapWebSocketRoutes(this WebApplication app, FixClient fixApp)
    {
        app.Map("/ws", async context =>
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                return;
            }

            using var socket = await context.WebSockets.AcceptWebSocketAsync();

            fixApp.OnExecutionReport = async msg =>
            {
                var json = JsonSerializer.Serialize(msg);
                var bytes = Encoding.UTF8.GetBytes(json);
                await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            };

            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                if (!string.IsNullOrEmpty(json))
                {
                    // repare que ja recebo mensagens e considero que são newordersingles
                    // em uma aplicação real, teria que fazer parsing da mensagem para decidir o que fazer
                    // mas mantive o exemplo simples devido ao tempo
                    var order = JsonSerializer.Deserialize<OrderDto>(json)!;

                    fixApp.SendOrder(
                        order.Sy,
                        order.Si == SideTypes.Compra.ToString() ? Side.BUY : Side.SELL,
                        order.Qt,
                        order.Pr
                    );
                }
            }
        });
    }
}

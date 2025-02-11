using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs;

public static class SellOrderHubExtensions
{
    public static async Task PublishAStatusChangeNotification(this IHubContext<SellOrderHub> sellOrderHub, string guid, string newStatus)
    {
        await sellOrderHub.Clients.Group(guid).SendAsync("StatusChangedNotificationHandler", newStatus);
    }
}

public class SellOrderHub : Hub
{
    public async Task SubscribeToStatusChangeNotification(string guid)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, guid);
    }
    
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"[{DateTime.Now}] Client {Context.ConnectionId} connected.");
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"[{DateTime.Now}] Client {Context.ConnectionId} disconnected. Message: {exception?.Message ?? "null"}.");
        
        await base.OnDisconnectedAsync(exception);
    }
}
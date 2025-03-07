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
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
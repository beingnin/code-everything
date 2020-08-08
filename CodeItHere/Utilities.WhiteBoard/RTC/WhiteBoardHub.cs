using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utilities.WhiteBoard.RTC
{
    public class WhiteBoardHub : Hub
    {
        private static readonly object _locker = new object();
        public static List<Buffer> _buffers = new List<Buffer>();


        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        public async Task Change(Buffer data)
        {
            //_buffers.Add(buffer);
            await this.Clients.GroupExcept(data.Board,this.Context.ConnectionId).SendAsync("change", data);
        }
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
    public class Buffer
    {
        public int Id { get; set; }
        public string Array { get; set; }
        public string Board { get; set; }
        public string Command { get; set; }
    }
}

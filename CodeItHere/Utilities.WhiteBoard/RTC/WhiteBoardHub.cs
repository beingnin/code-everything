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
        public async Task Add(Buffer data)
        {
            //_buffers.Add(buffer);
            await this.Clients.Others.SendAsync("change", data);
        }
    }
    public class Buffer
    {
        public int Id { get; set; }
        public string Array { get; set; }
    }
}

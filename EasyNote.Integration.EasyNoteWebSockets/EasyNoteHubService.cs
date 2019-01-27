using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyNote.Integration.EasyNoteWebSockets
{
    public class EasyNoteHubService
    {
        public readonly HubConnection connection;

        public EasyNoteHubService()
        {
            connection = new HubConnectionBuilder()
               //.WithUrl(ConfigurationManager.AppSettings["HubURL"])
               .WithUrl("http://easynotebbj.azurewebsites.net/easynotehub")
               .Build();
        }

        public async Task<bool> Connect()
        {
            await connection.StartAsync();

            while (connection.State != HubConnectionState.Connected) { }

            return true;
        }

        public void LockFile(string content)
        {
            if (connection.State == HubConnectionState.Disconnected)
            {
                connection.StartAsync().ContinueWith(z => { connection.InvokeAsync("LockFile", content); });
            }
            else
                connection.InvokeAsync("LockFile", content);

        }

        public void UnlockFile(string content)
        {
            if (connection.State == HubConnectionState.Disconnected)
            {
                connection.StartAsync().ContinueWith(z => { connection.InvokeAsync("UnlockFile", content); });
            }
            else
                connection.InvokeAsync("UnlockFile", content);
        }

        public void ForbidUnlockingFile(string fileId, string name)
        {
            if (connection.State == HubConnectionState.Disconnected)
            {
                connection.StartAsync().ContinueWith(z => { connection.InvokeAsync("ForbidUnlockingFile", fileId, name); });
            }
            else
                connection.InvokeAsync("ForbidUnlockingFile", fileId, name);
        }

        public void RequestUnlockingFile(string fileId, string name)
        {
            if (connection.State == HubConnectionState.Disconnected)
            {
                connection.StartAsync().ContinueWith(z => { connection.InvokeAsync("RequestUnlockingFile", fileId, name); });
            }
            else
                connection.InvokeAsync("RequestUnlockingFile", fileId, name);
        }

    }
}

using EasyNote.Integration.EasyNoteAPI.Gateway;
using EasyNote.Integration.EasyNoteAPI.Model;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace EasyNote.Integration.EasyNoteAPI
{
    public interface IEasyNoteService
    {
        void Add(CreateFileCommand newNote);
        IEnumerable<FileQueryResponse> Get();
        FileQueryResponse Get(int id);
        void Update(UpdateFileCommand command);
        void Delete(int id);
        LogonInfo Login(UserInfo userInfo);
    }

    internal class EasyNoteService : IEasyNoteService
    {
        private readonly IAPIGateway gateway;
        private LogonInfo logonInfo;

        public EasyNoteService(IAPIGateway gateway)
        {
            this.gateway = gateway;
        }

        public IEnumerable<FileQueryResponse> Get()
        {
            return gateway.ExecuteApiQuery<IEnumerable<FileQueryResponse>>("files/list",
                Method.GET, HttpStatusCode.OK, logonInfo.Auth_token);
        }
        public FileQueryResponse Get(int id)
        {
            return gateway.ExecuteApiQuery<FileQueryResponse>($"files/get/{id}",
                Method.GET, HttpStatusCode.OK, logonInfo.Auth_token);
        }

        public void Add(CreateFileCommand command)
        {
            gateway.ExecuteAPICommand("files/create",
                Method.POST, command, HttpStatusCode.Created, logonInfo.Auth_token);
        }

        public void Update(UpdateFileCommand command)
        {
            gateway.ExecuteAPICommand("files/update",
                Method.PUT, command, HttpStatusCode.OK, logonInfo.Auth_token);
        }

        public void Delete(int id)
        {
            gateway.ExecuteApiQuery($"files/delete/{id}",
                Method.GET, HttpStatusCode.OK, logonInfo.Auth_token);
        }

        public LogonInfo Login(UserInfo userInfo)
        {
            var logonResult
                = gateway.ExecuteAPICommand<LogonInfo>("accounts/login",
                    Method.POST, userInfo, HttpStatusCode.OK);

            this.logonInfo = logonResult;
            return logonResult;
        }
    }
}

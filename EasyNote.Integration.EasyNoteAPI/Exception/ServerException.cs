using System.Net;

namespace EasyNote.Integration.EasyNoteAPI.Exceptions
{
    public class ServerResponseException : System.Exception
    {
        private readonly HttpStatusCode code;

        public ServerResponseException(HttpStatusCode code)
        {
            this.code = code;
        }

        public override string Message
            => $"Error. Server responsed with status {(int)code} - {code}";
    }
}

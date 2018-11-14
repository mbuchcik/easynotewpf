using System;

namespace EasyNote.Integration.EasyNoteAPI.Model
{
    public class LogonInfo
    {
        public string Id { get; set; }
        public string Auth_token { get; set; }
        public int Expires_in { get; set; }
    }
}

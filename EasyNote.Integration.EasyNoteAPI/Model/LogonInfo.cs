using System;

namespace EasyNote.Integration.EasyNoteAPI.Model
{
    public class LogonInfo
    {
        public string id { get; set; }
        public string userName { get; set; }
        public string authToken { get; set; }
        public int expiresIn { get; set; }
    }
}

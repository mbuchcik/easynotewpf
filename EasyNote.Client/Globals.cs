using EasyNote.Integration.EasyNoteAPI.Model;

namespace EasyNote.Client
{
    public static class Globals
    {
        public static FileQueryResponse CurrentlyOpenedFile { get; set; }
        public static UserInfo Credentials { get; internal set; }
        public static bool IsReadOnly { get; internal set; }
    }
}

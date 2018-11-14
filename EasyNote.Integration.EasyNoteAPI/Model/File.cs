namespace EasyNote.Integration.EasyNoteAPI.Model
{

    public abstract class BaseFile
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
    }

    public class FileQueryResponse : BaseFile
    {
        public int Id { get; set; }
        public bool IsLocked { get; set; }
    }

    public class CreateFileCommand : BaseFile { }

    public class UpdateFileCommand : BaseFile
    {
        public int Id { get; set; }
    }


}

using static iDocsTestProject.Models.Enums;
namespace iDocsTestProject.Models.Documents
{
    public class DocumentViewModel
    {
        public Guid CreatedUserId { get; set; }

        public Guid ReceiverUserId { get; set; }

        public DocumentTypes Type { get; set; }

        public string Name { get; set; }

        public byte[] Content { get; set; }

        public string MimeType { get; set; }
        public string Extension { get; set; }
    }
}

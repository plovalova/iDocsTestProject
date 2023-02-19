using static iDocsTestProject.Models.Enums;

namespace iDocsTestProject.Models.Documents
{
    public class UserDocumentsModel
    {
        public Guid Number { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid CreatedUserId { get; set; }

        public Guid ReceiverUserId { get; set; }

        public DocumentTypes Type { get; set; }

        public string Name { get; set; }
    }
}

using static iDocsTestProject.Models.Enums;

namespace iDocsTestProject.Models.Documents
{
    public class UpdateDocumentRequest
    {
        public DocumentTypes? Type { get; set; }
        public string? Name { get; set; }
        public IFormFile? Content { get; set; }
    }
}

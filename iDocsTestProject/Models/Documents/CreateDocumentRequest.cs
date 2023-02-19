using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using static iDocsTestProject.Models.Enums;

namespace iDocsTestProject.Models.Documents
{
    public class CreateDocumentRequest
    {
        [Required]
        public Guid CreatedUserId { get; set; }
        [Required]
        public Guid ReceiverUserId { get; set; }
        [Required]
        public DocumentTypes Type { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public IFormFile Content { get; set; }

    }
}

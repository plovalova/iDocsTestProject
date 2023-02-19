using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static iDocsTestProject.Models.Enums;

namespace iDocsTestProject.Entities
{
    [Index(nameof(CreatedDate), nameof(CreatedUserId))]
    [Index(nameof(CreatedDate), nameof(ReceiverUserId))]
    public class Document
    {

        [Key]
        public Guid Number { get; set; }
        [Required]
        public DocumentTypes Type { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public Guid CreatedUserId { get; set; }
        [Required]
        public string Name { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string ContentId { get; set; }
        [Required]
        public Guid ReceiverUserId { get; set; }

    }
}

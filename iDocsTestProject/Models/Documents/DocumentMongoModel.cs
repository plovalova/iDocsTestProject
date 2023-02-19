using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace iDocsTestProject.Models.Documents
{
    public class DocumentMongoModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId ObjectId { get; set; }
        [BsonRepresentation(BsonType.Binary)]
        public byte[] Content { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string MimeType { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string Extension { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string FileName { get; set; }
    }
}

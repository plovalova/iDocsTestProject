using iDocsTestProject.Configurations;
using iDocsTestProject.Models.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.IO;
using System.Net.Sockets;

namespace iDocsTestProject.Services.Repositories
{
    public class ContentRepository
    {
        private readonly MongoDbConfig mongoDbConfig;
        private readonly IGridFSBucket gridFS;

        public ContentRepository(IOptions<MongoDbConfig> props)
        {
            mongoDbConfig = props.Value;
            var mongoClient = new MongoClient(mongoDbConfig.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbConfig.DatabaseName);
            gridFS = new GridFSBucket(mongoDatabase);
        }

        public async Task<string> UploadContentAsync(IFormFile content)
        {
            var objectId = await gridFS.UploadFromStreamAsync(content.FileName, content.OpenReadStream(), new GridFSUploadOptions
            {
                Metadata = new BsonDocument()
                    .Add("ContentType", content.ContentType)
                    .Add("Extension", Path.GetExtension(content.FileName))
            }); ;

            return objectId.ToString();
        }


        public async Task<DocumentMongoModel> GetContentAsync(string contentId)
        {
            var objectId = ObjectId.Parse(contentId);

            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", objectId);

            using (var cursor = await gridFS.FindAsync(filter))
            {
                var fileInfo = (await cursor.ToListAsync()).FirstOrDefault();

                var mongoModel = new DocumentMongoModel
                {
                    ObjectId = fileInfo.Id,
                    FileName = fileInfo.Filename,
                    Extension = fileInfo.Metadata.GetValue("Extension").ToString(),
                    MimeType = fileInfo.Metadata.GetValue("ContentType").ToString(),
                };

                var content = await gridFS.DownloadAsBytesAsync(objectId);

                mongoModel.Content = content;
                return mongoModel;
            }

        }

        public async Task DeleteContentAsync(ObjectId contentId)
        {
            await gridFS.DeleteAsync(contentId);
        }


    }
}

using iDocsTestProject.Entities;
using iDocsTestProject.Models;
using iDocsTestProject.Models.Documents;
using iDocsTestProject.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace iDocsTestProject.Services.Business
{
    public class DocumentsService
    {
        private readonly AppDbContext appDbContext;
        private readonly ContentRepository contentRepository;

        public DocumentsService(AppDbContext appDbContext, ContentRepository contentRepository)
        {
            this.appDbContext = appDbContext;
            this.contentRepository = contentRepository;
        }

        public async Task<Guid> CreateDocumentAsync(CreateDocumentRequest request, UserModel currentUser)
        {

            var contentId = await contentRepository.UploadContentAsync(request.Content);

            var newDocument = new Document
            {
                Name = request.Name,
                ContentId = contentId,
                Type = request.Type,
                CreatedUserId = Guid.Parse(currentUser.Id),
                ReceiverUserId = request.ReceiverUserId,
                CreatedDate = DateTime.Now
            };

            await appDbContext.Documents.AddAsync(newDocument);
            await appDbContext.SaveChangesAsync();

            return newDocument.Number;

        }

        public async Task<(Document document, DocumentMongoModel mongoModel)> GetDocumentAsync(Guid docNum)
        {
            var existingDocument = await appDbContext.Documents.FirstOrDefaultAsync(d => d.Number == docNum);

            if (existingDocument is null)
                throw new NullReferenceException(); 

            var mongoModel = await contentRepository.GetContentAsync(existingDocument.ContentId);

            return (existingDocument, mongoModel);
        }

        public async Task DeleteDocumentAsync((Document document, DocumentMongoModel mongoModel) resultDocument)
        {
            await contentRepository.DeleteContentAsync(resultDocument.mongoModel.ObjectId);
            appDbContext.Documents.Remove(resultDocument.document);
            await appDbContext.SaveChangesAsync();
        }

        public async Task UpdateDocumentAsync((Document document, DocumentMongoModel mongoModel) resultDocument, UpdateDocumentRequest request)
        {

            if (request.Content is not null)
            {
                await contentRepository.DeleteContentAsync(resultDocument.mongoModel.ObjectId);
                var newContentId = await contentRepository.UploadContentAsync(request.Content);
                resultDocument.document.ContentId = newContentId;
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
                resultDocument.document.Name = request.Name;
            if (request.Type.HasValue)
                resultDocument.document.Type = request.Type.Value;

            await appDbContext.SaveChangesAsync();

        }

        public async Task<IList<UserDocumentsModel>> GetUserDocumentsAsync(DateTime from, DateTime to, UserModel currentUser, int? pageSize, int? pageNumber)
        {

            var take = 15;
            var skip = 0;

            if (pageSize is not null)
                take = pageSize.Value;

            if (pageNumber is not null)
                skip = pageNumber.Value;


            var existingDocuments = await appDbContext.Documents
                .AsNoTracking()
                .Where(d => d.CreatedDate >= from &&
                d.CreatedDate <= to &&
                (d.CreatedUserId == Guid.Parse(currentUser.Id) || d.ReceiverUserId == Guid.Parse(currentUser.Id)))
                .Skip(skip)
                .Take(take).ToListAsync();

            var userDocuments = new List<UserDocumentsModel>();

            existingDocuments.ForEach(d => userDocuments.Add(
                new UserDocumentsModel
                {
                    Number = d.Number,
                    CreateDate = d.CreatedDate,
                    CreatedUserId = d.CreatedUserId,
                    ReceiverUserId = d.ReceiverUserId,
                    Name = d.Name,
                    Type = d.Type
                }));

            return userDocuments;

        }

        public async Task<(byte[] file, string fileName)> GetExcelReportAsync(IList<UserDocumentsModel> userDocuments, UserModel currentUser)
        {
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            { 
                var inSheet = package.Workbook.Worksheets.Add("Входящие");
                var outSheet = package.Workbook.Worksheets.Add("Исходящие");
                inSheet.Cells.LoadFromCollection(userDocuments.Where(d => d.CreatedUserId != Guid.Parse(currentUser.Id)), true);
                outSheet.Cells.LoadFromCollection(userDocuments.Where(d => d.CreatedUserId == Guid.Parse(currentUser.Id)), true);
                package.Save();
            }

            stream.Position = 0;
            string excelName = $"Отчет-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            return (stream.ToArray(), excelName);

        }
        
    }
}

using iDocsTestProject.Entities;
using iDocsTestProject.Helpers;
using iDocsTestProject.Models;
using iDocsTestProject.Models.Documents;
using iDocsTestProject.Services.Business;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.DataValidation;
using System.IO;
using System.Net;

namespace iDocsTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentsService documentsService;

        public DocumentsController(DocumentsService documentsService)
        {
            this.documentsService = documentsService;
        }

        [HttpPost]
        [Route(nameof(CreateDocument))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<GenericResponse>> CreateDocument([FromForm] CreateDocumentRequest request)
        {
            var docNum = await documentsService.CreateDocumentAsync(request);

            return Ok(new GenericResponse
            {
                Data = docNum,
                Result = true
            });

        }

        [HttpGet]
        [Route("{docNum:Guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<GenericResponse>> GetDocument(Guid docNum)
        {
            try
            {
                var resultDocument = await documentsService.GetDocumentAsync(docNum);

                return Ok(new GenericResponse
                {
                    Data = new DocumentViewModel
                    {
                        Content = resultDocument.mongoModel.Content,
                        CreatedUserId = resultDocument.document.CreatedUserId,
                        Extension = resultDocument.mongoModel.Extension,
                        MimeType = resultDocument.mongoModel.MimeType,
                        Name = resultDocument.document.Name,
                        ReceiverUserId = resultDocument.document.ReceiverUserId,
                        Type = resultDocument.document.Type,
                    },
                    Result = true
                });
            }
            catch (NullReferenceException)
            {
                return NotFound(new GenericResponse
                {
                    Result = false,
                    ErrorMessage = "Document not found!"
                });
            }
        }

        [HttpDelete]
        [Route("{docNum:Guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<GenericResponse>> DeleteDocument(Guid docNum)
        {
            try
            {
                var resultDocument = await documentsService.GetDocumentAsync(docNum);

                var currentUser = User.GetCurrentUser();

                if (resultDocument.document.CreatedUserId != Guid.Parse(currentUser.Id))
                {
                    return Unauthorized(new GenericResponse
                    {
                        Result = false,
                        ErrorMessage = "Not enough privileges!"
                    });
                }

                await documentsService.DeleteDocumentAsync(resultDocument);

                return Ok(new GenericResponse
                {
                    Result = true
                });

            }
            catch (NullReferenceException)
            {
                return NotFound(new GenericResponse
                {
                    Result = false,
                    ErrorMessage = "Document not found!"
                });
            }

        }

        [HttpPut]
        [Route("{docNum:Guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<GenericResponse>> UpdateDocument([FromForm] UpdateDocumentRequest request, Guid docNum)
        {
            try
            {
                var resultDocument = await documentsService.GetDocumentAsync(docNum);

                var currentUser = User.GetCurrentUser();

                if (resultDocument.document.CreatedUserId != Guid.Parse(currentUser.Id))
                {
                    return Unauthorized(new GenericResponse
                    {
                        Result = false,
                        ErrorMessage = "Not enough privileges!"
                    });
                }

                await documentsService.UpdateDocumentAsync(resultDocument, request);
                return Ok(new GenericResponse
                {
                    Result = true
                });
            }
            catch (NullReferenceException)
            {
                return NotFound(new GenericResponse
                {
                    Result = false,
                    ErrorMessage = "Document not found!"
                });
            }

        }

        [HttpGet]
        [Route(nameof(GetUserDocuments))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<GenericResponse>> GetUserDocuments([FromQuery] DateTime from, DateTime to, int? pageSize, int? pageNumber)
        {
            var currentUser = User.GetCurrentUser();

            var userDocuments = await documentsService.GetUserDocumentsAsync(from, to, currentUser, pageSize, pageNumber);

            return (Ok(new GenericResponse
            {
                Data = userDocuments,
                Result = true
            }));

        }

        [HttpGet]
        [Route(nameof(DownloadExcelReport))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<FileContentResult> DownloadExcelReport([FromQuery] DateTime from, DateTime to, int? skip, int? top)
        {
            var currentUser = User.GetCurrentUser();

            var userDocuments = await documentsService.GetUserDocumentsAsync(from, to, currentUser, skip, top);

            var report = await documentsService.GetExcelReportAsync(userDocuments, currentUser);

            return File(report.file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", report.fileName);
        }

    }
}

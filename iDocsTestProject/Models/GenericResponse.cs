using Amazon.Auth.AccessControlPolicy;
using System.Text.Json.Serialization;

namespace iDocsTestProject.Models
{
    public class GenericResponse

    {
        public object Data { get; set; }
        public bool Result { get; set; }
        public string ErrorMessage { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bitventure_Accounts.Shared.Commands
{
    public class UploadFileCommand
    {
        [Required]
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        [Required]
        [JsonPropertyName("fileContent")]
        public byte[] FileContent { get; set; }
    }
}

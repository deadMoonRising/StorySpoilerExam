using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StorySpoilerExam.Models
{
    internal class ApiResponseDTO
    {
        [JsonPropertyName("msg")]
        public string msg { get; set; }

        [JsonPropertyName("storyid")]
        public string storyId { get; set; }
    }
}

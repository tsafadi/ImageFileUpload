using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FileUpload.DTOs
{
    public class AuthorizationDTO
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
    }
}

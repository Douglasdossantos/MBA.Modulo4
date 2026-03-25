using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Models
{
    public class ContentCustomResult: ContentResult
    {
        public string AuthHeader { get; set; }
    }
}

using Microsoft.AspNetCore.DataProtection.Repositories;

namespace MBA.WebApp.MVC.Models
{
    public class ResponseResult
    {
        public string Title { get; set; }
        public int Status { get; set; }
        public ResponseErrorMessages Errors{ get; set; }
    }
}

using System.Net;

namespace MBA.Conteudo.Api.ViewModels
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse()
        {
            Errors = new List<string>();
        }
    }
}

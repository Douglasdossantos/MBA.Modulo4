using MBA.WebApp.MVC.Extensions;

namespace MBA.WebApp.MVC.Services
{
    public abstract class Service
    {
        protected bool TratarErrrosResponse(HttpResponseMessage response)
        {
           
            switch ((int)response.StatusCode)
            {
                case 401: 
                case 403: 
                case 404: 
                case 500:
                    throw new CustomHttpRequestException(response.StatusCode);

                case 400:
                    return false;
            }

            response.EnsureSuccessStatusCode();
            return true;

        }
    }
}

namespace MBA.WebApp.MVC.Models
{
    public class UsuarioToken
    {
        public string id { get; set; }
        public string Email { get; set; }
        public IEnumerable<UsuarioClaim> Claims { get; set; }
    }
}

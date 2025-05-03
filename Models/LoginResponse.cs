namespace API_Usuarios.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string Nombre { get; set; }
        public int JefeId { get; set; }
    }
}

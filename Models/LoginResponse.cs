namespace API_Usuarios.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public Users Usuario { get; set; }
    }
}

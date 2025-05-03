using System.ComponentModel.DataAnnotations;

namespace API_Usuarios.Models
{
    public class LoginRequest
    {
        public string Nombre { get; set; }
        public string NumeroTarjeta { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace API_Usuarios.Models
{
    public class LoginRequest
    {
        //Clase para recibir los datos del login.
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }
    }
}

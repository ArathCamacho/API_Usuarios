using System.ComponentModel.DataAnnotations;

namespace API_Usuarios.Models
{
    public class LoginRequest
    {
        //Clase para recibir los datos del login.
        [Required(ErrorMessage = "El usuario o correo es obligatorio")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }
    }
}

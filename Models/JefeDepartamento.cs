using System.ComponentModel.DataAnnotations;

namespace API_Usuarios.Models
{

    public class JefeDepartamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string NumeroTarjeta { get; set; } // Encriptado
    }

    // DTO para actualizar un usuario existente

}

using Microsoft.AspNetCore.Mvc;
using apiRESTCheckBdFull.Models;
using System.Collections.Generic;

namespace apiRESTCheckBdFull.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private static readonly List<clsUsuario> usuarios = new()
        {
            new clsUsuario { Id = 1, Nombre = "Luis", Apellido = "Angeles", Rol = "Admin" },
            new clsUsuario { Id = 2, Nombre = "Ana", Apellido = "Garcia", Rol = "Usuario" }
        };

        // GET: api/Usuarios
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(usuarios);
        }
    }
}

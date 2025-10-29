using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlConnector;                 // Usar MySqlConnector (no MySql.Data)
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using apiRestCenso.Models;

namespace apiRestCenso.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly string _connStr;
        public UsuariosController(IConfiguration config)
        {
            _connStr = config.GetConnectionString("MySqlConn");
        }

        // POST api/usuarios  -> spInsUsuario
        [HttpPost]
        public async Task<ActionResult<ApiResult<object>>> Post([FromBody] UsuarioCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiResult<object> { ok = false, msg = "Body vacío" });

            string code = null;

            await using (var con = new MySqlConnection(_connStr))
            await using (var cmd = new MySqlCommand("spInsUsuario", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("nombre", dto.nombre);
                cmd.Parameters.AddWithValue("paterno", dto.paterno);
                cmd.Parameters.AddWithValue("materno", dto.materno);
                cmd.Parameters.AddWithValue("usuario", dto.usuario);
                cmd.Parameters.AddWithValue("contrasena", dto.contrasena);
                cmd.Parameters.AddWithValue("ruta", dto.ruta);
                cmd.Parameters.AddWithValue("tipo", dto.tipo);

                await con.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    code = reader.GetString(0);
            }

            // '0' = OK, '1' = duplicado nombre completo, '2' = usuario existe, '3' = tipo no existe
            return code switch
            {
                "0" => Ok(new ApiResult<object> { ok = true, msg = "Usuario insertado" }),
                "1" => Conflict(new ApiResult<object> { ok = false, msg = "Ya existe una persona con ese nombre completo" }),
                "2" => Conflict(new ApiResult<object> { ok = false, msg = "USU_USUARIO ya existe" }),
                "3" => BadRequest(new ApiResult<object> { ok = false, msg = "TIPO de usuario no existe" }),
                _ => StatusCode(500, new ApiResult<object> { ok = false, msg = $"Código desconocido del SP: {code}" })
            };
        }

        // PUT api/usuarios/{id} -> spUpdUsuario
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResult<object>>> Put(int id, [FromBody] UsuarioCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new ApiResult<object> { ok = false, msg = "Body vacío" });

            string code = null;

            await using (var con = new MySqlConnection(_connStr))
            await using (var cmd = new MySqlCommand("spUpdUsuario", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("clave", id);
                cmd.Parameters.AddWithValue("nombre", dto.nombre);
                cmd.Parameters.AddWithValue("paterno", dto.paterno);
                cmd.Parameters.AddWithValue("materno", dto.materno);
                cmd.Parameters.AddWithValue("usuario", dto.usuario);
                cmd.Parameters.AddWithValue("pwd", dto.contrasena);
                cmd.Parameters.AddWithValue("ruta", dto.ruta);
                cmd.Parameters.AddWithValue("tipo", dto.tipo);

                await con.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    code = reader.GetString(0);
            }

            // '0'=OK, '1'=no existe clave, '2'=ya existe persona (nombre completo),
            // '3'=USUARIO ya existe, '4'=tipo no existe
            return code switch
            {
                "0" => Ok(new ApiResult<object> { ok = true, msg = "Usuario actualizado" }),
                "1" => NotFound(new ApiResult<object> { ok = false, msg = "No existe el usuario (clave)" }),
                "2" => Conflict(new ApiResult<object> { ok = false, msg = "Ya existe una persona con ese nombre completo" }),
                "3" => Conflict(new ApiResult<object> { ok = false, msg = "USU_USUARIO ya existe" }),
                "4" => BadRequest(new ApiResult<object> { ok = false, msg = "TIPO de usuario no existe" }),
                _ => StatusCode(500, new ApiResult<object> { ok = false, msg = $"Código desconocido del SP: {code}" })
            };
        }

        // DELETE api/usuarios/{id} -> spDelUsuario
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResult<object>>> Delete(int id)
        {
            string code = null;

            await using (var con = new MySqlConnection(_connStr))
            await using (var cmd = new MySqlCommand("spDelUsuario", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("clave", id);

                await con.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    code = reader.GetString(0);
            }

            // '0' = OK borrado, '1' = no existe
            return code switch
            {
                "0" => Ok(new ApiResult<object> { ok = true, msg = "Usuario eliminado" }),
                "1" => NotFound(new ApiResult<object> { ok = false, msg = "No existe el usuario (clave)" }),
                _ => StatusCode(500, new ApiResult<object> { ok = false, msg = $"Código desconocido del SP: {code}" })
            };
        }

        // GET api/usuarios  -> vista vwRptUsuario (reporte)
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<UsuarioRptDto>>>> GetAll()
        {
            var list = new List<UsuarioRptDto>();

            await using (var con = new MySqlConnection(_connStr))
            await using (var cmd = new MySqlCommand(
                "SELECT Clave, Nombre, Usuario, Foto, Rol FROM vwRptUsuario", con))
            {
                await con.OpenAsync();
                await using var rd = await cmd.ExecuteReaderAsync();
                while (await rd.ReadAsync())
                {
                    list.Add(new UsuarioRptDto
                    {
                        Clave = rd.GetInt32("Clave"),
                        Nombre = rd.GetString("Nombre"),
                        Usuario = rd.GetString("Usuario"),
                        Foto = rd.GetString("Foto"),
                        Rol = rd.GetString("Rol")
                    });
                }
            }

            return Ok(new ApiResult<List<UsuarioRptDto>> { ok = true, msg = "OK", data = list });
        }

        // GET api/usuarios/tipos -> vista vwTipoUsuario (combo)
        [HttpGet("tipos")]
        public async Task<ActionResult<ApiResult<List<TipoUsuarioDto>>>> GetTipos()
        {
            var list = new List<TipoUsuarioDto>();

            await using (var con = new MySqlConnection(_connStr))
            await using (var cmd = new MySqlCommand(
                "SELECT clave, descripcion FROM vwTipoUsuario", con))
            {
                await con.OpenAsync();
                await using var rd = await cmd.ExecuteReaderAsync();
                while (await rd.ReadAsync())
                {
                    list.Add(new TipoUsuarioDto
                    {
                        clave = rd.GetInt32("clave"),
                        descripcion = rd.GetString("descripcion")
                    });
                }
            }

            return Ok(new ApiResult<List<TipoUsuarioDto>> { ok = true, msg = "OK", data = list });
        }
    }
}

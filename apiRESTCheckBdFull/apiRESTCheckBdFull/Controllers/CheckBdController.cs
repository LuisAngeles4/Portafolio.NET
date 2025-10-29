using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using apiRESTCheckBdFull.Models; 
using Newtonsoft.Json.Linq;

namespace apiRESTCheckBdFull.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckBdController : ControllerBase
    {
        private readonly string _connStr;

        public CheckBdController(IConfiguration config)
        {
            // Lee la connection string de appsettings.json
            _connStr = config.GetConnectionString("MySqlDefault") 
                       ?? throw new InvalidOperationException("ConnectionString 'MySqlDefault' no definida.");
        }

        /// <summary>
        /// GET: /api/CheckBd/pingdb
        /// Prueba de conexión a MySQL. Devuelve clsApiStatus.
        /// </summary>
        [HttpGet("pingdb")]
        public async Task<IActionResult> PingDb()
        {
            var resp = new clsApiStatus();
            try
            {
                await using var conn = new MySqlConnection(_connStr);
                await conn.OpenAsync();

                // Consulta simple para validar lectura
                await using var cmd = new MySqlCommand("SELECT 1 AS ok;", conn);
                var val = await cmd.ExecuteScalarAsync();

                resp.statusExec = true;
                resp.ban = 1; //(1=OK, 0=Error)
                resp.msg = "Conexión a MySQL exitosa";
                resp.datos = JObject.FromObject(new { ok = val, serverVersion = conn.ServerVersion });
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.statusExec = false;
                resp.ban = 0;
                resp.msg = "Error de conexión a MySQL";
                resp.datos = JObject.FromObject(new { error = ex.Message });
                // 500 si falló la conexión
                return StatusCode(500, resp);
            }
        }
    }
}

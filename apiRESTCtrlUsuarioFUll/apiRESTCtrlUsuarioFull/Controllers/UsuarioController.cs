using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Linq; // Para LINQ sobre DataTable
using apiRESTCtrlUsuarioFull.Models;

namespace apiRESTCtrlUsuarioFull.Controllers
{
    [ApiController]
    [Route("full/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly string _connStr;

        public UsuarioController(IConfiguration cfg)
        {
            _connStr = cfg.GetConnectionString("bdControlAcceso")
                       ?? throw new InvalidOperationException("ConnString 'bdControlAcceso' no definida.");
        }

        // --------------------------------------------------------------------
        // POST: /full/usuario/spinsusuario
        // Body: clsUsuario (nombre, apellidoPaterno, apellidoMaterno, usuario, contrasena, ruta, tipo)
        // --------------------------------------------------------------------
        [HttpPost("spinsusuario")]
        public async Task<clsApiStatus> SpInsUsuario([FromBody] clsUsuario modelo)
        {
            var resp = new clsApiStatus();
            try
            {
                if (modelo is null)
                {
                    resp.statusExec = false; resp.ban = 0; resp.msg = "Body vacío";
                    resp.datos = new JObject { ["msgData"] = "Sin contenido" };
                    return resp;
                }

                var code = await modelo.SpInsUsuarioAsync(_connStr);

                (resp.statusExec, resp.ban, resp.msg) = code switch
                {
                    "0" => (true, 1, "Usuario registrado exitosamente!"),
                    "1" => (false, 0, "Ya existe un usuario con el mismo Nombre/Ap. Paterno/Ap. Materno."),
                    "2" => (false, 0, "El USU_USUARIO ya existe."),
                    "3" => (false, 0, "El tipo de usuario no existe."),
                    _   => (false,-1, $"Código inesperado del SP: '{code}'")
                };

                resp.datos = JObject.FromObject(new { code });
                return resp;
            }
            catch (Exception ex)
            {
                resp.statusExec = false;
                resp.ban = -1;
                resp.msg = "Usuario NO registrado...";
                resp.datos = JObject.FromObject(new { error = ex.ToString() });
                return resp;
            }
        }

        // --------------------------------------------------------------------
        // POST: /full/usuario/spvalidaracceso
        // Body: { "usuario":"...", "contrasena":"..." }
        // --------------------------------------------------------------------
        [HttpPost("spvalidaracceso")]
        public async Task<clsApiStatus> SpValidarAcceso([FromBody] clsUsuario modelo)
        {
            var resp = new clsApiStatus();
            try
            {
                if (modelo is null || string.IsNullOrWhiteSpace(modelo.usuario) || string.IsNullOrWhiteSpace(modelo.contrasena))
                {
                    resp.statusExec = false; resp.ban = 0; resp.msg = "Usuario y contraseña son requeridos";
                    resp.datos = new JObject();
                    return resp;
                }

                var (ok, nombreCompleto, ruta, usuario, rol) = await modelo.SpValidarAccesoDtoAsync(_connStr);

                if (ok)
                {
                    resp.statusExec = true;
                    resp.ban = 1;
                    resp.msg = "Acceso correcto";
                    resp.datos = JObject.FromObject(new
                    {
                        usu_nombre_completo = nombreCompleto,
                        usu_ruta = ruta,
                        usu_usuario = usuario,
                        tip_descripcion = rol
                    });
                }
                else
                {
                    resp.statusExec = false;
                    resp.ban = 0;
                    resp.msg = "Acceso denegado";
                    resp.datos = new JObject();
                }

                return resp;
            }
            catch (Exception ex)
            {
                resp.statusExec = false;
                resp.ban = -1;
                resp.msg = "Error al validar acceso";
                resp.datos = JObject.FromObject(new { error = ex.ToString() });
                return resp;
            }
        }

        // --------------------------------------------------------------------
        // GET: /full/usuario/vwrptusuario?filtro=texto
        // - Llama a clsUsuario.VwRptUsuarioAsync(_connStr) para traer la vista
        // - Aplica filtro en memoria (Nombre, Usuario)
        // - Devuelve JSON con la tabla usando el nombre de la DataTable
        // --------------------------------------------------------------------
        [HttpGet("vwrptusuario")]
        public async Task<clsApiStatus> VwRptUsuario([FromQuery] string filtro = "")
        {
            var objRespuesta = new clsApiStatus();
            var jsonResp = new JObject();

            try
            {
                var objUsuario = new clsUsuario();
                DataSet ds = await objUsuario.VwRptUsuarioAsync(_connStr); // trae todos los usuarios

                if (ds.Tables.Count == 0)
                {
                    objRespuesta.statusExec = true;
                    objRespuesta.msg = "No hay registros en la vista.";
                    objRespuesta.ban = 0;
                    objRespuesta.datos = JObject.FromObject(new { rows = new JArray() });
                    return objRespuesta;
                }

                DataTable dt = ds.Tables[0];

                // Si llega filtro, aplicamos búsqueda en columnas Nombre o Usuario
                if (!string.IsNullOrEmpty(filtro))
                {
                    var rows = dt.AsEnumerable()
                                 .Where(r =>
                                    (r.Field<string>("Nombre") ?? "")
                                        .IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    (r.Field<string>("Usuario") ?? "")
                                        .IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0);

                    dt = rows.Any() ? rows.CopyToDataTable() : dt.Clone(); // vacío si no hay match
                }

                objRespuesta.statusExec = true;
                objRespuesta.msg = "Reporte de usuarios generado!";
                objRespuesta.ban = dt.Rows.Count;

                string jsonString = JsonConvert.SerializeObject(dt, Formatting.Indented);
                // La clave del objeto será el nombre de la tabla, p.ej. "vwRptUsuario"
                var key = string.IsNullOrWhiteSpace(dt.TableName) ? "vwRptUsuario" : dt.TableName;
                jsonResp = JObject.Parse($"{{\"{key}\": {jsonString}}}");
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = "Fallo en el reporte de usuarios ...";
                objRespuesta.ban = -1;
                jsonResp.Add("msgData", ex.Message);
            }

            objRespuesta.datos = jsonResp;
            return objRespuesta;
        }

        // --------------------------------------------------------------------
        // GET: /full/usuario/vwtipousuario
        // - Llama a clsUsuario.VwTipoUsuarioAsync(_connStr)
        // - Devuelve rows en datos.rows (lista)
        // --------------------------------------------------------------------
        [HttpGet("vwtipousuario")]
        public async Task<clsApiStatus> VwTipoUsuario()
        {
            var resp = new clsApiStatus();
            try
            {
                var dal = new clsUsuario();
                var ds = await dal.VwTipoUsuarioAsync(_connStr);

                var list = new JArray();
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        var obj = new JObject
                        {
                            ["clave"] = JToken.FromObject(r["clave"]),
                            ["descripcion"] = JToken.FromObject(r["descripcion"])
                        };
                        list.Add(obj);
                    }
                }

                resp.statusExec = true;
                resp.ban = list.Count;
                resp.msg = list.Count > 0 ? "OK" : "Sin resultados";
                resp.datos = new JObject { ["rows"] = list };
                return resp;
            }
            catch (Exception ex)
            {
                resp.statusExec = false;
                resp.ban = -1;
                resp.msg = "Error al consultar vwtipousuario";
                resp.datos = JObject.FromObject(new { error = ex.ToString() });
                return resp;
            }
        }

        // --------------------------------------------------------------------
        // PUT: /full/usuario/spupdusuario?clave=1
        // Body: clsUsuario (usa las propiedades del modelo)
        // --------------------------------------------------------------------
        [HttpPut("spupdusuario")]
        public async Task<clsApiStatus> SpUpdUsuario([FromQuery] int clave, [FromBody] clsUsuario modelo)
        {
            var resp = new clsApiStatus();
            try
            {
                if (clave <= 0)
                {
                    resp.statusExec = false; resp.ban = 0; resp.msg = "Clave inválida";
                    resp.datos = new JObject();
                    return resp;
                }
                if (modelo is null)
                {
                    resp.statusExec = false; resp.ban = 0; resp.msg = "Body vacío";
                    resp.datos = new JObject();
                    return resp;
                }

                var code = await modelo.SpUpdUsuarioAsync(_connStr, clave);

                (resp.statusExec, resp.ban, resp.msg) = code switch
                {
                    "0" => (true, 1, "¡Usuario actualizado con éxito!"),
                    "1" => (false, 0, "La clave no existe."),
                    "2" => (false, 0, "El nombre completo ya existe en otro registro."),
                    "3" => (false, 0, "El usuario (login) ya existe en otro registro."),
                    "4" => (false, 0, "El tipo de usuario no existe."),
                    _   => (false,-1, $"Código inesperado del SP: '{code}'")
                };

                resp.datos = JObject.FromObject(new { code });
                return resp;
            }
            catch (Exception ex)
            {
                resp.statusExec = false;
                resp.ban = -1;
                resp.msg = "Error al actualizar usuario";
                resp.datos = JObject.FromObject(new { error = ex.ToString() });
                return resp;
            }
        }

        // --------------------------------------------------------------------
        // DELETE: /full/usuario/spdelusuario/123
        // --------------------------------------------------------------------
        [HttpDelete("spdelusuario/{clave:int}")]
        public async Task<clsApiStatus> SpDelUsuario([FromRoute] int clave)
        {
            var resp = new clsApiStatus();
            try
            {
                if (clave <= 0)
                {
                    resp.statusExec = false; resp.ban = 0; resp.msg = "Clave inválida";
                    resp.datos = new JObject();
                    return resp;
                }

                var dal = new clsUsuario();
                var code = await dal.SpDelUsuarioAsync(_connStr, clave);

                (resp.statusExec, resp.ban, resp.msg) = code switch
                {
                    "0" => (true, 1, "Usuario eliminado correctamente."),
                    "1" => (false, 0, "La clave no existe."),
                    _   => (false,-1, $"Código inesperado del SP: '{code}'")
                };

                resp.datos = JObject.FromObject(new { code });
                return resp;
            }
            catch (Exception ex)
            {
                resp.statusExec = false;
                resp.ban = -1;
                resp.msg = "Error al eliminar usuario";
                resp.datos = JObject.FromObject(new { error = ex.ToString() });
                return resp;
            }
        }
    }

    // Si ya tienes esta clase en otro archivo, elimina esta definición para evitar duplicados.
    public class clsApiStatus
    {
        public bool statusExec { get; set; }
        public int ban { get; set; }
        public string msg { get; set; } = "";
        public JObject datos { get; set; } = new JObject();
    }
}

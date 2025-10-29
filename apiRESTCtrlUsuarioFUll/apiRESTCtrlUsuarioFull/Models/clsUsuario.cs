using System.Data;
using MySqlConnector;

namespace apiRESTCtrlUsuarioFull.Models
{
    public class clsUsuario
    {
        // -----------------------------------------------------
        // Propiedades
        // -----------------------------------------------------
        public string cve { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string apellidoPaterno { get; set; } = string.Empty;
        public string apellidoMaterno { get; set; } = string.Empty;
        public string usuario { get; set; } = string.Empty;
        public string contrasena { get; set; } = string.Empty;
        public string ruta { get; set; } = string.Empty;
        public int tipo { get; set; }

        // -----------------------------------------------------
        // Inserción con spInsUsuario
        // -----------------------------------------------------
        public async Task<string> SpInsUsuarioAsync(string cs)
        {
            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();

            const string sql = @"CALL spInsUsuario(@nombre, @paterno, @materno, @usuario, @contrasena, @ruta, @tipo);";
            await using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@paterno", apellidoPaterno);
            cmd.Parameters.AddWithValue("@materno", apellidoMaterno);
            cmd.Parameters.AddWithValue("@usuario", this.usuario);
            cmd.Parameters.AddWithValue("@contrasena", contrasena);
            cmd.Parameters.AddWithValue("@ruta", ruta);
            cmd.Parameters.AddWithValue("@tipo", tipo);

            var obj = await cmd.ExecuteScalarAsync();
            return Convert.ToString(obj) ?? "";
        }

        // -----------------------------------------------------
        // Validación de acceso con spValidarAcceso (DataTable)
        // -----------------------------------------------------
        public async Task<DataTable> SpValidarAccesoAsync(string cs)
        {
            var dt = new DataTable();

            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();

            const string sql = @"CALL spValidarAcceso(@usuario, @contrasena);";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@usuario", this.usuario);
            cmd.Parameters.AddWithValue("@contrasena", this.contrasena);

            await using var rdr = await cmd.ExecuteReaderAsync();
            dt.Load(rdr);

            return dt;
        }

        // -----------------------------------------------------
        // Validación de acceso con spValidarAcceso (DTO amigable)
        // -----------------------------------------------------
        public async Task<(bool ok, string nombreCompleto, string ruta, string usuario, string rol)> SpValidarAccesoDtoAsync(string cs)
        {
            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();

            const string sql = @"CALL spValidarAcceso(@usuario, @contrasena);";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@usuario", this.usuario);
            cmd.Parameters.AddWithValue("@contrasena", this.contrasena);

            await using var rdr = await cmd.ExecuteReaderAsync();

            if (await rdr.ReadAsync())
            {
                var ban = Convert.ToString(rdr.GetValue(0)) ?? "0";
                if (ban == "1")
                {
                    string Safe(string col)
                    {
                        var i = rdr.GetOrdinal(col);
                        return rdr.IsDBNull(i) ? "" : rdr.GetString(i);
                    }

                    return (true,
                        nombreCompleto: Safe("usu_nombre_completo"),
                        ruta: Safe("usu_ruta"),
                        usuario: Safe("usu_usuario"),
                        rol: Safe("tip_descripcion"));
                }
            }
            return (false, "", "", "", "");
        }

        // -----------------------------------------------------
        // Vista: vwRptUsuario -> DataSet
        // -----------------------------------------------------
        public async Task<DataSet> VwRptUsuarioAsync(string cs)
        {
            var ds = new DataSet();
            var dt = new DataTable("vwRptUsuario");

            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();

            const string sql = "SELECT * FROM vwRptUsuario;";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var rdr = await cmd.ExecuteReaderAsync();
            dt.Load(rdr);

            ds.Tables.Add(dt);
            return ds;
        }

        // -----------------------------------------------------
        // Vista: vwTipoUsuario -> DataSet
        // -----------------------------------------------------
        public async Task<DataSet> VwTipoUsuarioAsync(string cs)
        {
            var ds = new DataSet();
            var dt = new DataTable("vwTipoUsuario");

            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();

            const string sql = "SELECT * FROM vwTipoUsuario;";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var rdr = await cmd.ExecuteReaderAsync();
            dt.Load(rdr);

            ds.Tables.Add(dt);
            return ds;
        }

        // -----------------------------------------------------
        // Actualización con spUpdUsuario
        // -----------------------------------------------------
        public async Task<string> SpUpdUsuarioAsync(string cs, int clave)
        {
            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();

            const string sql = @"CALL spUpdUsuario(@clave,@nombre,@paterno,@materno,@usuario,@pwd,@ruta,@tipo);";
            await using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@clave", clave);
            cmd.Parameters.AddWithValue("@nombre", this.nombre);
            cmd.Parameters.AddWithValue("@paterno", this.apellidoPaterno);
            cmd.Parameters.AddWithValue("@materno", this.apellidoMaterno);
            cmd.Parameters.AddWithValue("@usuario", this.usuario);
            cmd.Parameters.AddWithValue("@pwd", this.contrasena);
            cmd.Parameters.AddWithValue("@ruta", this.ruta);
            cmd.Parameters.AddWithValue("@tipo", this.tipo);

            var obj = await cmd.ExecuteScalarAsync();
            return Convert.ToString(obj) ?? "";
        }

        // -----------------------------------------------------
        // Eliminación con spDelUsuario
        // -----------------------------------------------------
        public async Task<string> SpDelUsuarioAsync(string cs, int clave)
        {
            await using var conn = new MySqlConnection(cs);
            await conn.OpenAsync();

            const string sql = @"CALL spDelUsuario(@clave);";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@clave", clave);

            var obj = await cmd.ExecuteScalarAsync();
            return Convert.ToString(obj) ?? "";
        }
    }
}

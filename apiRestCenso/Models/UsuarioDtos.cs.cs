namespace apiRestCenso.Models
{
    public class UsuarioCreateDto
    {
        public string nombre { get; set; }
        public string paterno { get; set; }
        public string materno { get; set; }
        public string usuario { get; set; }
        public string contrasena { get; set; }
        public string ruta { get; set; }          // 'imagenes/fotos/<ID>.jpg' (tu lógica actual)
        public int tipo { get; set; }             // TIP_CVE_TIPOUSUARIO
    }

    public class UsuarioUpdateDto : UsuarioCreateDto
    {
        public int clave { get; set; }            // USU_CVE_USUARIO
    }

    public class UsuarioRptDto
    {
        public int Clave { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Foto { get; set; }
        public string Rol { get; set; }
    }

    public class TipoUsuarioDto
    {
        public int clave { get; set; }
        public string descripcion { get; set; }
    }

    public class ApiResult<T>
    {
        public bool ok { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }
}

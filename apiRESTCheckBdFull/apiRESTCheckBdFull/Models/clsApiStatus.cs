// Models/clsApiStatus.cs
using Newtonsoft.Json.Linq;

namespace apiRESTCheckBdFull.Models
{
    public class clsApiStatus
    {
        // Estado de ejecución del endpoint (método)
        public bool statusExec { get; set; } = false;

        // Descripción del resultado
        public string msg { get; set; } = string.Empty;

        // Código de ejecución del endpoint (1=OK, 0=Error, etc.)
        public int ban { get; set; } = 0;

        // Objeto Json para envío de datos
        public JObject? datos { get; set; } = null;
    }
}


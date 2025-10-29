using Newtonsoft.Json.Linq;

namespace apiRESTCtrlUsuarioFull.Models
{
    public class clsApiStatus
    {
        public bool statusExec { get; set; } = false;
        public string msg { get; set; } = string.Empty;
        public int ban { get; set; } = 0;
        public JObject datos { get; set; } = new JObject();
    }
}

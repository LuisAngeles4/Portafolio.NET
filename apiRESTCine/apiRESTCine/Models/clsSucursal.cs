namespace apiRESTCine.Models;


public class clsSucursal
{
    // Atributos (todos string)
    public string Clave { get; set; } = string.Empty;     // mapea SUC_CVE_SUCURSAL
    public string Nombre { get; set; } = string.Empty;    // SUC_NOMBRE
    public string Direccion { get; set; } = string.Empty; // SUC_DIRECCION
    public string Url { get; set; } = string.Empty;       // SUC_HOMEWEB
    public string Logo { get; set; } = string.Empty;      // SUC_LOGO

    // Constructor sin parámetros (sin código dentro)
    public clsSucursal() { }

    // Constructor con todos los parámetros
    public clsSucursal(string clave, string nombre, string direccion, string url, string logo)
    {
        Clave = clave;
        Nombre = nombre;
        Direccion = direccion;
        Url = url;
        Logo = logo;
    }

    // Métodos get/set ya están dados por las propiedades (get; set;)
}

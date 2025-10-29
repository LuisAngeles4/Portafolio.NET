namespace apiRESTCine.Models;

public class clsApiStatus
{
    public int Code { get; set; } = 0;           // 0 = OK, otros = error/estado
    public string Message { get; set; } = "OK";  // texto explicativo
    public object? Data { get; set; }            // payload opcional (DataSet, objeto, lista, etc.)

    public clsApiStatus() { }

    public clsApiStatus(int code, string message, object? data = null)
    {
        Code = code;
        Message = message;
        Data = data;
    }
}

namespace Taxify.WebApi.Models;

public class Responce
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}
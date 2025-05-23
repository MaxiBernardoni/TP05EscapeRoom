using System.ComponentModel;
using Newtonsoft.Json;

class SalaEscape
{
    public string nombreUsuario {get; set;}
    public string nombreDirector {get; set;}
    public List<string> inventario{get; set;}
    
    public static string ObjectToString<T>(T? obj)
    {
    return JsonConvert.SerializeObject(obj);
    }

}
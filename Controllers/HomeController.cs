using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TP05_EscapeRoom.Controllers;

public class HomeController : Controller
{
   public IActionResult Index()
{
    return RedirectToAction("Intro");
}


    public IActionResult StartGame()
    {
        HttpContext.Session.SetInt32("Habitacion", 1);
        HttpContext.Session.SetString("inventario", "");
        return RedirectToAction("Habitacion1");
    }

    public IActionResult Habitacion1()
    {
        if (HttpContext.Session.GetInt32("Habitacion") != 1)
            return RedirectToAction("Index");

        ViewBag.Inventario = HttpContext.Session.GetString("inventario");
        return View();
    }

    [HttpPost]
    public IActionResult Agarrar(string objeto)
    {
        var inventario = HttpContext.Session.GetString("inventario") ?? "";
        if (!inventario.Contains(objeto))
        {
            inventario += objeto + ";";
            HttpContext.Session.SetString("inventario", inventario);
        }

        if (inventario.Contains("llaves") && inventario.Contains("desayuno") && inventario.Contains("sube"))
        {
            HttpContext.Session.SetInt32("Habitacion", 2);
            return RedirectToAction("Habitacion2");
        }

        TempData["Mensaje"] = $"Agarraste: {objeto}";
        return RedirectToAction("Habitacion1");
    }

    public IActionResult Habitacion2()
    {
        return View();
    }
    [HttpPost]
public IActionResult ValidarColectivo(string colectivo)
{
    if (colectivo == "152")
    {
        HttpContext.Session.SetInt32("Habitacion", 3);
        return RedirectToAction("Habitacion3");
    }

    TempData["Error"] = "Ese colectivo no te lleva a destino. Intentá con otro.";
    return RedirectToAction("Habitacion2");
}
[HttpPost]
public IActionResult ValidarCalle(string calle)
{
    if (calle.Trim().ToLower() == "bulnes")
    {
        HttpContext.Session.SetInt32("Habitacion", 4);
        return RedirectToAction("Habitacion4");
    }

    TempData["Error"] = "Esa calle no es la correcta. Te perdiste.";
    return RedirectToAction("Habitacion3");
}
[HttpPost]
public IActionResult IniciarSesion(string nombre)
{
    if (!string.IsNullOrWhiteSpace(nombre))
    {
        HttpContext.Session.SetString("usuario", nombre);
        return RedirectToAction("Intro");
    }

    TempData["Error"] = "Por favor, ingresá tu nombre.";
    return RedirectToAction("Index");
}

public IActionResult Intro()
{
    return View();
}
public IActionResult GuardarNombre(string nombre)
{
    HttpContext.Session.SetString("usuario", nombre);
    return Ok();
}

public IActionResult IntroAlarma()
{
    return View();
}
public IActionResult SetHabitacion(int id)
{
    HttpContext.Session.SetInt32("Habitacion", id);
    return RedirectToAction("Habitacion1");
}

}

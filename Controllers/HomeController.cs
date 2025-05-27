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
        HttpContext.Session.Remove("habitacion1_objetos");
        HttpContext.Session.Remove("habitacion1_inventario");
        return RedirectToAction("Habitacion1");
    }

    public IActionResult Habitacion1()
    {
        if (HttpContext.Session.GetInt32("Habitacion") != 1)
            return RedirectToAction("Index");

        if (HttpContext.Session.GetString("habitacion1_objetos") == null)
        {
            var objetos = new List<string> { "llaves", "sube", "cafe", "tostadas", "ropa" };
            var random = new Random();
            var posiciones = Enumerable.Range(0, 9).OrderBy(x => random.Next()).Take(objetos.Count).ToList();
            var mapa = new string[9];
            for (int i = 0; i < mapa.Length; i++) mapa[i] = "";
            for (int i = 0; i < objetos.Count; i++) mapa[posiciones[i]] = objetos[i];
            HttpContext.Session.SetString("habitacion1_objetos", string.Join(";", mapa));
            HttpContext.Session.SetString("habitacion1_inventario", "");
        }

        ViewBag.Mapa = HttpContext.Session.GetString("habitacion1_objetos")?.Split(';') ?? new string[9];
        ViewBag.Inventario = HttpContext.Session.GetString("habitacion1_inventario") ?? "";

        return View();
    }

    [HttpPost]
    public IActionResult Recolectar(int index)
    {
        var mapa = HttpContext.Session.GetString("habitacion1_objetos")?.Split(';') ?? new string[9];
        var inventario = HttpContext.Session.GetString("habitacion1_inventario") ?? "";

        if (index >= 0 && index < mapa.Length && mapa[index] != "")
        {
            inventario += mapa[index] + ";";
            mapa[index] = "";
        }

        HttpContext.Session.SetString("habitacion1_objetos", string.Join(";", mapa));
        HttpContext.Session.SetString("habitacion1_inventario", inventario);

        if (inventario.Contains("llaves") && inventario.Contains("sube") &&
            (inventario.Contains("cafe") || inventario.Contains("tostadas")))
        {
            HttpContext.Session.SetInt32("Habitacion", 2);
            return RedirectToAction("Habitacion2");
        }

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


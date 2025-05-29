using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TuProyecto.Models;
using System.Text.Json;

namespace TuProyecto.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Intro()
        {
            int seconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            ViewBag.GameSeconds = seconds;
            ViewBag.GameClock = new GameClock(seconds);
            return View();
        }

        [HttpGet]
        public IActionResult IntroAlarma()
        {
            int seconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            ViewBag.GameSeconds = seconds;
            ViewBag.GameClock = new GameClock(seconds);
            return View();
        }

        [HttpPost]
        public IActionResult StartGame()
        {
            if (HttpContext.Session.GetInt32("game_seconds") == null)
                HttpContext.Session.SetInt32("game_seconds", 0);

            var modelo = new Habitacion1Model();
            var modeloJson = JsonSerializer.Serialize(modelo);
            HttpContext.Session.SetString("habitacion1", modeloJson);

            return RedirectToAction("Habitacion1");
        }

        [HttpGet]
        public IActionResult Habitacion1()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion1");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new Habitacion1Model()
                : JsonSerializer.Deserialize<Habitacion1Model>(modeloJson);

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult SeleccionarObjeto(string objeto)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion1");
            if (modeloJson != null)
            {
                var modelo = JsonSerializer.Deserialize<Habitacion1Model>(modeloJson);
                modelo.SeleccionarObjeto(objeto);
                HttpContext.Session.SetString("habitacion1", JsonSerializer.Serialize(modelo));
            }
            return RedirectToAction("Habitacion1");
        }

        [HttpPost]
        public IActionResult VerificarPuerta()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion1");
            if (modeloJson != null)
            {
                var modelo = JsonSerializer.Deserialize<Habitacion1Model>(modeloJson);
                if (modelo.PuedeSalir())
                    return RedirectToAction("Habitacion2");

                TempData["Error"] = "Te falta algo para poder salir.";
            }
            return RedirectToAction("Habitacion1");
        }

        [HttpGet]
        public IActionResult Habitacion2()
        {
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            ViewBag.Modelo = new Habitacion2Model();
            return View();
        }

        [HttpPost]
        public IActionResult FalloRuta()
        {
            int seconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            seconds += 120;
            HttpContext.Session.SetInt32("game_seconds", seconds);
            TempData["Error"] = "Elegiste el colectivo incorrecto. Se restaron 2 minutos.";
            return RedirectToAction("Habitacion2");
        }

        [HttpPost]
        public IActionResult AciertoRuta() => RedirectToAction("TransicionColectivo");

[HttpGet]
public IActionResult TransicionColectivo()
{
    ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
    return View();
}

        [HttpGet]
        public IActionResult JuegoColectivo()
        {
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult ResultadoColectivo(string eleccion)
        {
            var modelo = new JuegoColectivoModel(eleccion);
            int segundos = HttpContext.Session.GetInt32("game_seconds") ?? 0;

            if (modelo.Resultado == "Perdiste") segundos += 60;
            if (modelo.Resultado == "Ganaste") segundos = Math.Max(0, segundos - 30);

            HttpContext.Session.SetInt32("game_seconds", segundos);
            ViewBag.GameSeconds = segundos;
            ViewBag.Modelo = modelo;
            return View("ResultadoColectivo");
        }

        [HttpGet]
        public IActionResult Habitacion3()
        {
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpGet]
        public IActionResult UpdateClock(int seconds)
        {
            HttpContext.Session.SetInt32("game_seconds", seconds);
            return Ok();
        }
    }
}


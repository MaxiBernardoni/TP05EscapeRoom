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
            return View();
        }

        [HttpGet]
        public IActionResult TransicionColectivo()
        {
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpGet]
        public IActionResult JuegoColectivo()
        {
            var modelo = new JuegoColectivoModel();
            HttpContext.Session.SetString("juego_colectivo", JsonSerializer.Serialize(modelo));
            HttpContext.Session.SetInt32("game_seconds", HttpContext.Session.GetInt32("game_seconds") ?? 0);
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds");
            return View(modelo);
        }

        [HttpPost]
        public IActionResult ResultadoColectivo(string eleccion)
        {
            var juegoJson = HttpContext.Session.GetString("juego_colectivo");
            var juegoAnterior = juegoJson != null 
                ? JsonSerializer.Deserialize<JuegoColectivoModel>(juegoJson) 
                : new JuegoColectivoModel();

            var modelo = new JuegoColectivoModel(
                eleccion, 
                juegoAnterior.RondaActual,
                juegoAnterior.VictoriasJugador,
                juegoAnterior.VictoriasConductor
            );

            HttpContext.Session.SetString("juego_colectivo", JsonSerializer.Serialize(modelo));
            var segundos = HttpContext.Session.GetInt32("game_seconds") ?? 0;

            if (modelo.JuegoTerminado)
            {
                if (modelo.GanadorFinal == "Jugador")
                    segundos = Math.Max(0, segundos - 30);
                else
                    segundos += 60;

                HttpContext.Session.SetInt32("game_seconds", segundos);
                ViewBag.GameSeconds = segundos;
                return View("ResultadoFinal", modelo);
            }

            ViewBag.GameSeconds = segundos;
            return View("ResultadoRonda", modelo);
        }

        [HttpGet]
        public IActionResult SiguienteRonda()
        {
            var juegoJson = HttpContext.Session.GetString("juego_colectivo");
            if (juegoJson == null)
            {
                return RedirectToAction("JuegoColectivo");
            }

            var juegoAnterior = JsonSerializer.Deserialize<JuegoColectivoModel>(juegoJson);
            var modelo = new JuegoColectivoModel
            {
                RondaActual = juegoAnterior.RondaActual + 1,
                VictoriasJugador = juegoAnterior.VictoriasJugador,
                VictoriasConductor = juegoAnterior.VictoriasConductor
            };

            HttpContext.Session.SetString("juego_colectivo", JsonSerializer.Serialize(modelo));
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View("JuegoColectivo", modelo);
        }

        [HttpGet]
        public IActionResult UpdateClock(int seconds)
        {
            HttpContext.Session.SetInt32("game_seconds", seconds);
            return Ok();
        }

        [HttpGet]
        public IActionResult Habitacion3()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new Habitacion3Model()
                : JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult IniciarHabitacion3()
        {
            var modelo = new Habitacion3Model();
            var modeloJson = JsonSerializer.Serialize(modelo);
            HttpContext.Session.SetString("habitacion3", modeloJson);
            return RedirectToAction("Habitacion3");
        }
    }
}


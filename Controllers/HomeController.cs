using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TuProyecto.Models;
using System.Text.Json;
using System.Collections.Generic;

namespace TuProyecto.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            int seconds = 0;
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
            var inventario = InventarioGlobal.CargarDeSession(HttpContext.Session);
            
            if (inventario.TieneObjeto(objeto))
                inventario.QuitarObjeto(objeto);
            else if (inventario.PuedeAgregarObjeto())
                inventario.AgregarObjeto(objeto);
            
            inventario.GuardarEnSession(HttpContext.Session);
            return RedirectToAction("Habitacion1");
        }

        [HttpPost]
        public IActionResult VerificarPuerta()
        {
            var inventario = InventarioGlobal.CargarDeSession(HttpContext.Session);
            bool tieneLlaves = inventario.TieneObjeto("🔑 Llaves");
            bool tieneSube = inventario.TieneObjeto("🚌 SUBE");
            bool tieneDesayuno = inventario.TieneObjeto("☕ Café") || inventario.TieneObjeto("🍞 Tostadas");

            if (tieneLlaves && tieneSube && tieneDesayuno)
                return RedirectToAction("Habitacion2");

            TempData["Error"] = "Te falta algo para poder salir.";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReiniciarJuego()
        {
            // Limpiar la sesión
            HttpContext.Session.Clear();
            // Redirigir al index
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Habitacion3()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            if (string.IsNullOrEmpty(modeloJson))
                HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpGet]
        public IActionResult DialogoPunga()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            // Si ya resolvió el punga, mostrar mensaje de que ya no está
            if (modelo.PungaResuelto)
            {
                TempData["MensajePunga"] = "El punga ya se fue. Podés seguir avanzando.";
                return RedirectToAction("Habitacion3");
            }

            // Si perdió, mostrar mensaje de que debe reiniciar
            if (!modelo.TienePlata)
            {
                TempData["MensajePunga"] = "El punga te robó todo. Tenés que reiniciar la sala.";
                return RedirectToAction("Habitacion3");
            }

            // Si el juego está en curso, ir al minijuego
            if (modelo.JuegoPungaEnCurso)
            {
                return RedirectToAction("MinijuegoPunga");
            }

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult AceptarDesafioPunga()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            modelo.ReiniciarJuegoPunga();
            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("MinijuegoPunga");
        }

        [HttpGet]
        public IActionResult MinijuegoPunga()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            // Si el juego no está en curso y no hay resultado, volver al diálogo
            if (!modelo.JuegoPungaEnCurso && modelo.VasoSeleccionado < 0)
                return RedirectToAction("DialogoPunga");

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult ResolverPunga(int vasoElegido, string ordenVasos)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            modelo.VasoSeleccionado = vasoElegido;
            if (!string.IsNullOrEmpty(ordenVasos))
            {
                try { modelo.OrdenVasos = System.Text.Json.JsonSerializer.Deserialize<List<int>>(ordenVasos); }
                catch { modelo.OrdenVasos = new List<int> { 0, 1, 2 }; }
            }
            bool gano = (vasoElegido == modelo.VasoCorrecto);
            modelo.JuegoPungaEnCurso = false;

            if (gano)
            {
                modelo.PungaResuelto = true;
            }
            else
            {
                modelo.TienePlata = false;
            }

            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("MinijuegoPunga");
        }

        [HttpGet]
        public IActionResult ResultadoPunga()
        {
            ViewBag.Resultado = TempData["ResultadoPunga"];
            return View();
        }

        [HttpGet]
        public IActionResult MenuProgreso()
        {
            bool hab1 = false, hab2 = false, hab3 = false;
            var hab1Json = HttpContext.Session.GetString("habitacion1");
            if (!string.IsNullOrEmpty(hab1Json))
            {
                var modelo1 = JsonSerializer.Deserialize<Habitacion1Model>(hab1Json);
                hab1 = modelo1.PuedeSalir();
            }
            var hab2Json = HttpContext.Session.GetString("habitacion2");
            if (!string.IsNullOrEmpty(hab2Json))
            {
                var modelo2 = JsonSerializer.Deserialize<Habitacion2Model>(hab2Json);
                hab2 = modelo2.JuegoGanado;
            }
            var hab3Json = HttpContext.Session.GetString("habitacion3");
            if (!string.IsNullOrEmpty(hab3Json))
            {
                hab3 = true;
            }
            return Json(new { hab1, hab2, hab3 });
        }

        [HttpPost]
        public IActionResult VolverHabitacion(int habitacion)
        {
            // Borra el progreso posterior a la habitación elegida
            if (habitacion == 1)
            {
                HttpContext.Session.Remove("habitacion2");
                HttpContext.Session.Remove("habitacion3");
                HttpContext.Session.Remove("juego_colectivo");
            }
            else if (habitacion == 2)
            {
                HttpContext.Session.Remove("habitacion3");
            }
            // Redirige a la habitación correspondiente
            switch (habitacion)
            {
                case 1: return RedirectToAction("Habitacion1");
                case 2: return RedirectToAction("Habitacion2");
                case 3: return RedirectToAction("Habitacion3");
                default: return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult DialogoIgnorado()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult FinalizarDialogoIgnorado()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            modelo.IgnoradoInteractuado = true;
            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("Habitacion3");
        }

        [HttpGet]
        public IActionResult DialogoAyudar()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult FinalizarDialogoAyudar()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            modelo.AyudaResuelto = true;
            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("Habitacion3");
        }

        [HttpGet]
        public IActionResult DialogoInsistente()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult AceptarInsistente()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            modelo.CompraRealizada = true;
            modelo.InsistenteResuelto = true;
            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("DialogoInsistente");
        }

        [HttpPost]
        public IActionResult RechazarInsistente()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            if (modelo.IntentosRechazo == null) modelo.IntentosRechazo = 0;
            modelo.IntentosRechazo++;
            if (modelo.IntentosRechazo >= 2)
                modelo.InsistenteResuelto = true;
            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("DialogoInsistente");
        }

        [HttpPost]
        public IActionResult FinalizarDialogoInsistente()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            modelo.InsistenteResuelto = true;
            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("Habitacion3");
        }

        [HttpPost]
        public IActionResult ReiniciarNivel()
        {
            // Detectar la habitación actual por el referer o por TempData/ViewBag si se implementa
            string referer = Request.Headers["Referer"].ToString().ToLower();
            if (referer.Contains("habitacion1"))
            {
                HttpContext.Session.Remove("habitacion1");
                return RedirectToAction("Habitacion1");
            }
            if (referer.Contains("habitacion2"))
            {
                HttpContext.Session.Remove("habitacion2");
                return RedirectToAction("Habitacion2");
            }
            // Por defecto, o si es un diálogo de la 3, reinicia la 3
            HttpContext.Session.Remove("habitacion3");
            return RedirectToAction("Habitacion3");
        }

        [HttpGet]
        public IActionResult DialogoAyudar2()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            // Inicializar estado del minijuego si es la primera vez
            if (modelo.Ayudar2BalasUsuario == 0 && modelo.Ayudar2BalasNPC == 0 && !modelo.Ayudar2Resuelto && !modelo.Ayudar2Perdio)
            {
                modelo.Ayudar2BalasUsuario = 0;
                modelo.Ayudar2BalasNPC = 0;
                modelo.Ayudar2EscudoConsecutivo = 0;
                modelo.Ayudar2UltimaAccionUsuario = null;
                modelo.Ayudar2UltimaAccionNPC = null;
                modelo.Ayudar2Dialogo = 0;
            }

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TuProyecto.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

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
                return View("FinalColectivo", modelo);
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
                HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
                return RedirectToAction("MinijuegoPunga");
            }
            else
            {
                // Perdió contra el punga - pierde 10 minutos y vuelve a Habitación 3
                modelo.TienePlata = false;
                
                // Obtener el tiempo actual y restar 10 minutos (600 segundos)
                int tiempoActual = HttpContext.Session.GetInt32("game_seconds") ?? 0;
                int nuevoTiempo = tiempoActual + 600; // Sumar 10 minutos como penalización
                HttpContext.Session.SetInt32("game_seconds", nuevoTiempo);
                
                // Actualizar el modelo
                HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
                
                // Mostrar mensaje de penalización
                TempData["MensajePunga"] = "¡Perdiste contra el punga! Te robó todo el dinero y perdiste 10 minutos. Vuelves a la Habitación 3.";
                
                return RedirectToAction("Habitacion3");
            }
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
            bool hab1 = false, hab2 = false, hab3 = false, hab4 = false, hab5 = false;
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
            var hab4Json = HttpContext.Session.GetString("habitacion4_completada");
            if (!string.IsNullOrEmpty(hab4Json))
            {
                hab4 = true;
            }
            var hab5Json = HttpContext.Session.GetString("habitacion5_completada");
            if (!string.IsNullOrEmpty(hab5Json))
            {
                hab5 = true;
            }
            return Json(new { hab1, hab2, hab3, hab4, hab5 });
        }

        [HttpPost]
        public IActionResult VolverHabitacion(int habitacion)
        {
            // Borra el progreso posterior a la habitación elegida
            if (habitacion == 1)
            {
                HttpContext.Session.Remove("habitacion2");
                HttpContext.Session.Remove("habitacion3");
                HttpContext.Session.Remove("habitacion4_completada");
                HttpContext.Session.Remove("habitacion5_completada");
                HttpContext.Session.Remove("juego_colectivo");
            }
            else if (habitacion == 2)
            {
                HttpContext.Session.Remove("habitacion3");
                HttpContext.Session.Remove("habitacion4_completada");
                HttpContext.Session.Remove("habitacion5_completada");
            }
            else if (habitacion == 3)
            {
                HttpContext.Session.Remove("habitacion4_completada");
                HttpContext.Session.Remove("habitacion5_completada");
            }
            else if (habitacion == 4)
            {
                HttpContext.Session.Remove("habitacion5_completada");
            }
            // Redirige a la habitación correspondiente
            switch (habitacion)
            {
                case 1: return RedirectToAction("Habitacion1");
                case 2: return RedirectToAction("Habitacion2");
                case 3: return RedirectToAction("Habitacion3");
                case 4: return RedirectToAction("Habitacion4");
                case 5: return RedirectToAction("Habitacion5");
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
            // Detectar la habitación actual por el referer, contemplando todas las rutas posibles
            string referer = Request.Headers["Referer"].ToString().ToLower();
            
            // Habitación 1 y sus rutas
            if (referer.Contains("habitacion1") || referer.Contains("seleccionarobjeto") || referer.Contains("verificarpuerta"))
            {
                HttpContext.Session.Remove("habitacion1");
                return RedirectToAction("Habitacion1");
            }
            
            // Habitación 2 y sus rutas
            if (referer.Contains("habitacion2") || referer.Contains("transicioncolectivo") || 
                referer.Contains("juegocolectivo") || referer.Contains("resultadocolectivo") || 
                referer.Contains("siguienteronda"))
            {
                HttpContext.Session.Remove("habitacion2");
                HttpContext.Session.Remove("juego_colectivo");
                return RedirectToAction("Habitacion2");
            }
            
            // Habitación 4 y todas sus rutas
            if (referer.Contains("habitacion4") || referer.Contains("codigoportero") || 
                referer.Contains("verificarcodigoportero") || referer.Contains("seleccioncaminoedificio") ||
                referer.Contains("tuberialaberinto") || referer.Contains("tuberialaser") ||
                referer.Contains("escalera") || referer.Contains("ascensor") ||
                referer.Contains("encuentrobinker") || referer.Contains("finalhabitacion4"))
            {
                // La Habitación 4 no tiene estado persistente en session, solo redirigir
                return RedirectToAction("Habitacion4");
            }
            
            // Habitación 5 y todas sus rutas
            if (referer.Contains("habitacion5") || referer.Contains("juegonucleo") || 
                referer.Contains("completarhabitacion5") || referer.Contains("firmacontrato") ||
                referer.Contains("finalizarjuego"))
            {
                // La Habitación 5 no tiene estado persistente en session, solo redirigir
                return RedirectToAction("Habitacion5");
            }
            
            // Habitación 3 y todas sus rutas (por defecto)
            // Incluye: diálogos, minijuegos, resultados, etc.
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

        [HttpGet]
        public IActionResult DialogoNervioso()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            // Verificar si el usuario tiene el espejo
            var inventario = InventarioGlobal.CargarDeSession(HttpContext.Session);
            if (!inventario.TieneObjeto("🪞 Espejo"))
            {
                TempData["MensajeNervioso"] = "Necesitás el espejo para poder hablar con el nervioso. Volvé a la Habitación 1 y agarrá el espejo.";
                return RedirectToAction("Habitacion3");
            }

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult SalvarEspejo()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            modelo.EspejoSalvado = true;
            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalizarDialogoNervioso()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            modelo.NerviosoResuelto = true;
            modelo.CodigoObtenido = true;
            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("Habitacion3");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AvanzarCuadra()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            // Avanzar cuadra
            modelo.CuadraActual++;
            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));

            // Redirigir al minijuego correspondiente
            if (modelo.CuadraActual == 1)
                return RedirectToAction("Minijuego1");
            else if (modelo.CuadraActual == 2)
                return RedirectToAction("Minijuego2");
            else if (modelo.CuadraActual == 3)
                return RedirectToAction("Minijuego3");
            else
                return RedirectToAction("Habitacion3"); // O final del juego
        }

        [HttpGet]
        public IActionResult Minijuego1()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            // Si la ronda no está completada, generar y guardar el emoji correcto
            if (modelo.RondaActualEmoji <= 10)
            {
                var random = new System.Random();
                modelo.EmojiCorrectoActual = random.Next(50);
                HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            }

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpGet]
        public IActionResult Minijuego2()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            // Si la secuencia no está generada para la ronda actual, generarla
            if (modelo.SecuenciaMemoria == null || modelo.SecuenciaMemoria.Count == 0 || modelo.RondaActualMemoria == 1)
            {
                var random = new System.Random();
                modelo.SecuenciaMemoria = new List<int>();
                var indices = Enumerable.Range(0, 49).OrderBy(_ => random.Next()).Take(5).ToList();
                modelo.SecuenciaMemoria.AddRange(indices);
                HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            }

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpGet]
        public IActionResult Minijuego3()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            // Inicializar el número secreto si no existe
            if (string.IsNullOrEmpty(modelo.NumeroSecretoCofre))
            {
                var random = new System.Random();
                var digitos = Enumerable.Range(0, 10).OrderBy(_ => random.Next()).Take(4).ToArray();
                modelo.NumeroSecretoCofre = string.Join("", digitos);
                modelo.IntentosCofre = new List<string>();
                modelo.RespuestasCofre = new List<string>();
                modelo.IntentosRestantesCofre = 8;
                modelo.CofreResuelto = false;
                HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            }

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult ResolverMinijuego1(int emojiSeleccionado)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            // Usar el emoji correcto guardado
            int emojiCorrecto = modelo.EmojiCorrectoActual;

            // Si el usuario acierta, avanza de ronda
            if (emojiSeleccionado == emojiCorrecto)
            {
                modelo.RondaActualEmoji++;
                if (modelo.RondaActualEmoji > 10)
                {
                    modelo.Minijuego1Completado = true;
                }
            }
            // No modificar el emojiCorrecto aquí

            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("Minijuego1");
        }

        [HttpPost]
        public IActionResult ResolverMinijuego2(string secuencia)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            // Convertir la secuencia recibida a lista de int
            var seleccionUsuario = new List<int>();
            if (!string.IsNullOrEmpty(secuencia))
            {
                seleccionUsuario = secuencia.Split(',').Select(s => int.Parse(s)).ToList();
            }

            // Verificar si la secuencia es correcta
            bool acerto = seleccionUsuario.Count == modelo.SecuenciaMemoria.Count &&
                seleccionUsuario.All(idx => modelo.SecuenciaMemoria.Contains(idx)) &&
                modelo.SecuenciaMemoria.All(idx => seleccionUsuario.Contains(idx));

            if (acerto)
            {
                modelo.RondaActualMemoria++;
                if (modelo.RondaActualMemoria > 7)
                {
                    modelo.Minijuego2Completado = true;
                }
                else
                {
                    // Generar nueva secuencia para la siguiente ronda
                    var random = new System.Random();
                    modelo.SecuenciaMemoria = new List<int>();
                    var indices = Enumerable.Range(0, 49).OrderBy(_ => random.Next()).Take(5).ToList();
                    modelo.SecuenciaMemoria.AddRange(indices);
                }
            }
            // Si no acierta, no avanza de ronda, puede reintentar

            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("Minijuego2");
        }

        [HttpPost]
        public IActionResult ResolverMinijuego3(string intento)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            if (modelo.CofreResuelto || modelo.IntentosRestantesCofre <= 0)
                return RedirectToAction("Minijuego3");

            // Validar intento
            if (string.IsNullOrEmpty(intento) || intento.Length != 4 || intento.Distinct().Count() != 4 || !intento.All(char.IsDigit))
            {
                TempData["ErrorCofre"] = "Error: Debes ingresar exactamente 4 dígitos diferentes (ej: 1234, 5678).";
                return RedirectToAction("Minijuego3");
            }

            // Calcular exactos y desordenados
            int exactos = 0, desordenados = 0;
            for (int i = 0; i < 4; i++)
            {
                if (intento[i] == modelo.NumeroSecretoCofre[i]) exactos++;
                else if (modelo.NumeroSecretoCofre.Contains(intento[i])) desordenados++;
            }

            modelo.IntentosCofre.Add(intento);
            modelo.RespuestasCofre.Add($"{exactos}E-{desordenados}D");
            modelo.IntentosRestantesCofre--;

            if (exactos == 4)
            {
                modelo.CofreResuelto = true;
                modelo.Minijuego3Completado = true;
                HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
                return RedirectToAction("ContinuarDespuesCofre");
            }

            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("Minijuego3");
        }

        [HttpPost]
        public IActionResult ReiniciarMinijuego3()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new TuProyecto.Models.Habitacion3Model()
                : System.Text.Json.JsonSerializer.Deserialize<TuProyecto.Models.Habitacion3Model>(modeloJson);

            modelo.NumeroSecretoCofre = null;
            modelo.IntentosCofre = new List<string>();
            modelo.RespuestasCofre = new List<string>();
            modelo.IntentosRestantesCofre = 8;
            modelo.CofreResuelto = false;
            HttpContext.Session.SetString("habitacion3", System.Text.Json.JsonSerializer.Serialize(modelo));
            return RedirectToAction("Minijuego3");
        }

        [HttpGet]
        public IActionResult ContinuarDespuesCofre()
        {
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContinuarAHabitacion4()
        {
            return RedirectToAction("Habitacion4");
        }

        [HttpGet]
        public IActionResult Habitacion4()
        {
            // Entrada principal del edificio
            return View();
        }


        [HttpPost]
        public IActionResult VerificarCodigoPortero(string codigo)
        {
            if (!string.IsNullOrEmpty(codigo) && codigo.Trim() == "hello world")
            {
                // Código correcto, redirigir a la selección de escalera/ascensor
                return RedirectToAction("SeleccionCaminoEdificio");
            }
            else
            {
                TempData["ErrorCodigo"] = "Código incorrecto. Intenta nuevamente.";
                return RedirectToAction("CodigoPortero");
            }
        }

        [HttpGet]
        public IActionResult SeleccionCaminoEdificio()
        {
            // Vista para elegir escalera o ascensor
            return View();
        }

        [HttpGet]
        public IActionResult TuberiaLaberinto()
        {
            // Minijuego de laberinto simple
            return View();
        }

        [HttpGet]
        public IActionResult TuberiaLaser()
        {
            // Minijuego de tubería avanzada (láseres y dash)
            return View();
        }

        [HttpGet]
        public IActionResult Escalera()
        {
            // Diálogo y resultado de la escalera
            return View();
        }

        [HttpGet]
        public IActionResult Ascensor()
        {
            // Diálogo y resultado del ascensor
            return View();
        }

        [HttpGet]
        public IActionResult EncuentroBinker()
        {
            // Diálogo con Binker
            return View();
        }

        [HttpGet]
        public IActionResult FinalHabitacion4()
        {
            // Final de la habitación 4
            return View();
        }

        [HttpGet]
        public IActionResult DialogoPortero()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Habitacion5()
        {
            // Habitación 5 - El Despacho de Auraman
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpGet]
        public IActionResult JuegoNucleo()
        {
            // Inicializar el juego del núcleo
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult CompletarHabitacion5()
        {
            // Marcar la habitación 5 como completada
            HttpContext.Session.SetString("habitacion5_completada", "true");
            return RedirectToAction("FirmaContrato");
        }

        [HttpGet]
        public IActionResult FirmaContrato()
        {
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult FinalizarJuego()
        {
            // Marcar la habitación 4 como completada si no está marcada
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("habitacion4_completada")))
            {
                HttpContext.Session.SetString("habitacion4_completada", "true");
            }
            // Final del juego completo
            return RedirectToAction("ResultadoFinal");
        }

        [HttpGet]
        public IActionResult ResultadoFinal()
        {
            return View();
        }
    }
}


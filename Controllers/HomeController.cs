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

        [HttpGet]
        public IActionResult Habitacion3()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson)
                ? new Habitacion3Model()
                : JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);

            // Obtener los objetos del inventario global
            var inventario = InventarioGlobal.CargarDeSession(HttpContext.Session);
            modelo.TieneEspejo = inventario.TieneObjeto("🪞 Espejo");
            modelo.TieneLentes = inventario.TieneObjeto("👓 Lentes");
            modelo.TienePlata = inventario.TieneObjeto("💵 Dinero");

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

        [HttpPost]
        public IActionResult InteractuarNPC(string npc)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            if (modeloJson == null) 
            {
                var nuevoModelo = new Habitacion3Model();
                HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(nuevoModelo));
                return RedirectToAction("Habitacion3");
            }

            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);
            var inventario = InventarioGlobal.CargarDeSession(HttpContext.Session);
            
            // Verificar requisitos antes de permitir la interacción
            switch (npc)
            {
                case "punga":
                    if (inventario.TieneObjeto("💵 Dinero"))
                    {
                        modelo.TienePlata = true;
                        modelo.JuegoPungaEnCurso = false;
                        modelo.PungaResuelto = false;
                        HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
                        return RedirectToAction("DialogoPunga");
                    }
                    modelo.TienePlata = false;
                    break;
                case "ignorado":
                    modelo.IgnoradoInteractuado = true;
                    break;
                case "confundido":
                    if (!modelo.TieneLentes) return RedirectToAction("Habitacion3");
                    modelo.ConfundidoInteractuado = true;
                    break;
                case "insistente":
                    if (!modelo.TienePlata) return RedirectToAction("Habitacion3");
                    if (!modelo.InsistenteResuelto && modelo.TienePlata)
                        return RedirectToAction("DialogoInsistente");
                    break;
                case "ayuda":
                    if (modelo.PuedeJugarPistolita())
                        modelo.ReiniciarJuegoPistolita();
                    break;
                case "nervioso":
                    if (!modelo.TieneEspejo) return RedirectToAction("Habitacion3");
                    if (!modelo.NerviosoResuelto && modelo.TieneEspejo)
                        modelo.NerviosoResuelto = true;
                    break;
            }

            HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
            return RedirectToAction("Habitacion3");
        }

        [HttpPost]
        public IActionResult JugarVasos(int vaso)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            if (modeloJson == null) return RedirectToAction("Habitacion3");

            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);
            
            if (!modelo.JuegoPungaEnCurso) return RedirectToAction("Habitacion3");

            modelo.VasoSeleccionado = vaso;
            if (vaso == modelo.VasoCorrecto)
            {
                modelo.PungaResuelto = true;
                modelo.JuegoPungaEnCurso = false;
            }
            else
            {
                modelo.TienePlata = false;
                modelo.JuegoPungaEnCurso = false;
            }

            HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
            return RedirectToAction("Habitacion3");
        }

        [HttpPost]
        public IActionResult JugarPistolita(string accion)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            if (modeloJson == null) return RedirectToAction("Habitacion3");

            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);
            
            if (!modelo.JuegoPistolitaEnCurso) return RedirectToAction("Habitacion3");

            modelo.AccionJugador = accion;
            var accionOponente = GenerarAccionOponente(modelo);

            ResolverTurnoPistolita(modelo, accion, accionOponente);

            HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
            return RedirectToAction("Habitacion3");
        }

        private string GenerarAccionOponente(Habitacion3Model modelo)
        {
            var random = new Random();
            if (modelo.BalasOponente == 0)
                return "recargar";
            
            var acciones = new[] { "disparar", "escudo", "recargar" };
            return acciones[random.Next(acciones.Length)];
        }

        private void ResolverTurnoPistolita(Habitacion3Model modelo, string accionJugador, string accionOponente)
        {
            // Procesar recargas
            if (accionJugador == "recargar") modelo.BalasJugador++;
            if (accionOponente == "recargar") modelo.BalasOponente++;

            // Procesar escudos
            bool jugadorProtegido = accionJugador == "escudo";
            bool oponenteProtegido = accionOponente == "escudo";

            if (jugadorProtegido) modelo.EscudosUsados++;

            // Procesar disparos
            if (accionJugador == "disparar" && modelo.BalasJugador > 0)
            {
                modelo.BalasJugador--;
                if (!oponenteProtegido)
                {
                    modelo.AyudaResuelto = true;
                    modelo.JuegoPistolitaEnCurso = false;
                }
            }

            if (accionOponente == "disparar" && modelo.BalasOponente > 0)
            {
                modelo.BalasOponente--;
                if (!jugadorProtegido)
                {
                    modelo.JuegoPistolitaEnCurso = false;
                }
            }

            // Verificar límite de escudos
            if (modelo.EscudosUsados >= 3)
            {
                modelo.JuegoPistolitaEnCurso = false;
            }
        }

        [HttpPost]
        public IActionResult DialogoInsistente(string accion)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            if (modeloJson == null) return RedirectToAction("Habitacion3");

            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);

            if (accion == "comprar")
            {
                modelo.TienePlata = false;
                modelo.CompraRealizada = true;
                modelo.InsistenteResuelto = true;
            }
            else if (accion == "rechazar")
            {
                modelo.IntentosRechazo++;
                if (modelo.IntentosRechazo >= 3)
                {
                    modelo.InsistenteRechazado = true;
                    modelo.InsistenteResuelto = true;
                }
            }

            HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
            return RedirectToAction("Habitacion3");
        }

        [HttpPost]
        public IActionResult IniciarMinijuego(int cuadra)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            if (modeloJson == null) return RedirectToAction("Habitacion3");

            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);
            modelo.CuadraActual = cuadra;

            HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
            return RedirectToAction($"Minijuego{cuadra}");
        }

        [HttpGet]
        public IActionResult Minijuego1()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);
            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult ResolverMinijuego1(int emojiSeleccionado)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);

            if (emojiSeleccionado == GenerarEmojiCorrecto(modelo.RondaActualEmoji))
            {
                modelo.RondaActualEmoji++;
                if (modelo.RondaActualEmoji > 10)
                {
                    modelo.Minijuego1Completado = true;
                    return RedirectToAction("Habitacion3");
                }
            }
            else
            {
                var segundos = HttpContext.Session.GetInt32("game_seconds") ?? 0;
                HttpContext.Session.SetInt32("game_seconds", segundos + 30);
            }

            HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
            return RedirectToAction("Minijuego1");
        }

        private int GenerarEmojiCorrecto(int ronda)
        {
            // Lógica para generar el emoji correcto según la ronda
            return new Random().Next(50);
        }

        [HttpGet]
        public IActionResult Minijuego2()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);
            
            if (modelo.SecuenciaMemoria.Count == 0)
            {
                GenerarSecuenciaMemoria(modelo);
            }

            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        private void GenerarSecuenciaMemoria(Habitacion3Model modelo)
        {
            var random = new Random();
            int cantidadCuadrados = modelo.RondaActualMemoria + 2;
            modelo.SecuenciaMemoria.Clear();
            
            for (int i = 0; i < cantidadCuadrados; i++)
            {
                modelo.SecuenciaMemoria.Add(random.Next(49)); // 7x7 grid = 49 cuadrados
            }
        }

        [HttpPost]
        public IActionResult ResolverMinijuego2(string secuencia)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);

            var secuenciaJugador = secuencia.Split(',').Select(int.Parse).ToList();
            bool secuenciaCorrecta = modelo.SecuenciaMemoria.SequenceEqual(secuenciaJugador);

            if (secuenciaCorrecta)
            {
                modelo.RondaActualMemoria++;
                if (modelo.RondaActualMemoria > 7)
                {
                    modelo.Minijuego2Completado = true;
                    return RedirectToAction("Habitacion3");
                }
                GenerarSecuenciaMemoria(modelo);
            }
            else
            {
                modelo.RondaActualMemoria = 1;
                GenerarSecuenciaMemoria(modelo);
            }

            HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
            return RedirectToAction("Minijuego2");
        }

        [HttpGet]
        public IActionResult Minijuego3()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);
            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult ResolverMinijuego3(int puzzleId, string solucion)
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);

            bool puzzleResuelto = false;
            switch (puzzleId)
            {
                case 1: // Huevos
                    puzzleResuelto = solucion == "huevo2"; // El huevo correcto
                    break;
                case 2: // Caja del tesoro
                    puzzleResuelto = solucion == "estrella"; // La forma correcta
                    break;
                case 3: // Ventana empañada
                    puzzleResuelto = int.Parse(solucion) >= 80; // 80% limpio
                    break;
            }

            if (puzzleResuelto)
            {
                modelo.PuzzleActual++;
                if (modelo.PuzzleActual > 3)
                {
                    modelo.Minijuego3Completado = true;
                    return RedirectToAction("Habitacion3");
                }
            }

            HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
            return RedirectToAction("Minijuego3");
        }

        [HttpPost]
        public IActionResult AvanzarHabitacion3()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            if (modeloJson == null) return RedirectToAction("Habitacion3");

            var modelo = JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);
            
            if (modelo.VerificarAvance())
            {
                // Aquí iría la lógica para avanzar a la siguiente habitación
                return RedirectToAction("Habitacion4");
            }

            return RedirectToAction("Habitacion3");
        }

        [HttpGet]
        public IActionResult DialogoPunga()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson) 
                ? new Habitacion3Model() 
                : JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);
            
            var inventario = InventarioGlobal.CargarDeSession(HttpContext.Session);

            // Verificar si tiene dinero antes de mostrar el diálogo
            if (!inventario.TieneObjeto("💵 Dinero"))
            {
                modelo.TienePlata = false;
                HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
                return RedirectToAction("Habitacion3");
            }

            // Inicializar el estado del diálogo
            modelo.TienePlata = true;
            modelo.JuegoPungaEnCurso = false;
            if (!modelo.PungaResuelto)
            {
                modelo.VasoSeleccionado = -1;
                modelo.VasoCorrecto = new Random().Next(3);
            }

            HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
            
            ViewBag.Modelo = modelo;
            ViewBag.GameSeconds = HttpContext.Session.GetInt32("game_seconds") ?? 0;
            return View();
        }

        [HttpPost]
        public IActionResult IniciarJuegoPunga()
        {
            var modeloJson = HttpContext.Session.GetString("habitacion3");
            var modelo = string.IsNullOrEmpty(modeloJson) 
                ? new Habitacion3Model() 
                : JsonSerializer.Deserialize<Habitacion3Model>(modeloJson);
            
            var inventario = InventarioGlobal.CargarDeSession(HttpContext.Session);
            if (!inventario.TieneObjeto("💵 Dinero"))
            {
                return RedirectToAction("Habitacion3");
            }

            modelo.JuegoPungaEnCurso = true;
            modelo.VasoSeleccionado = -1;
            modelo.VasoCorrecto = new Random().Next(3);
            modelo.TienePlata = true;
            
            HttpContext.Session.SetString("habitacion3", JsonSerializer.Serialize(modelo));
            return RedirectToAction("DialogoPunga");
        }

        [HttpPost]
        public IActionResult ReiniciarJuego()
        {
            // Limpiar la sesión
            HttpContext.Session.Clear();
            
            // Reiniciar el juego desde el principio
            return RedirectToAction("StartGame");
        }
    }
}


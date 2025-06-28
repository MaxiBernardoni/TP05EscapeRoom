using System.Collections.Generic;

namespace TuProyecto.Models
{
    public class Habitacion3Model
    {
        // Estado de cada NPC
        public bool PungaResuelto { get; set; } = false;
        public bool IgnoradoInteractuado { get; set; } = false;
        public bool ConfundidoInteractuado { get; set; } = false;
        public bool InsistenteResuelto { get; set; } = false;
        public bool AyudaResuelto { get; set; } = false;
        public bool NerviosoResuelto { get; set; } = false;
        public bool EspejoSalvado { get; set; } = false;

        // Inventario
        public bool TienePlata { get; set; } = true; // Comienza con dinero
        public bool TieneEspejo { get; set; } = false;
        public bool TieneLentes { get; set; } = false;

        // Estado de avance
        public bool PuedeAvanzar { get; set; } = false;
        public int CuadraActual { get; set; } = 0;
        public bool CodigoObtenido { get; set; } = false;

        // Comentarios mostrados por el usuario al pasar el mouse
        public Dictionary<string, bool> ComentariosMostrados { get; set; } = new Dictionary<string, bool>();

        // Minijuego del Punga (Vasos)
        public int IntentosPunga { get; set; } = 0;
        public bool JuegoPungaEnCurso { get; set; } = false;
        public int VasoSeleccionado { get; set; } = -1;
        public int VasoCorrecto { get; set; } = -1;
        public List<int> OrdenVasos { get; set; } = new List<int> { 0, 1, 2 };

        // Minijuego de Pistolita
        public int IntentosPistolita { get; set; } = 0;
        public bool JuegoPistolitaEnCurso { get; set; } = false;
        public string AccionJugador { get; set; } = "";
        public int BalasJugador { get; set; } = 0;
        public int BalasOponente { get; set; } = 0;
        public int EscudosUsados { get; set; } = 0;

        // Minijuegos por cuadra
        public bool Minijuego1Completado { get; set; } = false;
        public int RondaActualEmoji { get; set; } = 1;
        public bool Minijuego2Completado { get; set; } = false;
        public int RondaActualMemoria { get; set; } = 1;
        public List<int> SecuenciaMemoria { get; set; } = new List<int>();
        public bool Minijuego3Completado { get; set; } = false;
        public int PuzzleActual { get; set; } = 1;

        // Estado del juego del Insistente
        public bool CompraRealizada { get; set; } = false;
        public bool InsistenteRechazado { get; set; } = false;
        public int IntentosRechazo { get; set; } = 0;

        // Estado y minijuego de Ayudar2 (pistolita)
        public bool Ayudar2Resuelto { get; set; } = false;
        public bool Ayudar2Perdio { get; set; } = false;
        public int Ayudar2BalasUsuario { get; set; } = 0;
        public int Ayudar2BalasNPC { get; set; } = 0;
        public int Ayudar2EscudoConsecutivo { get; set; } = 0;
        public string Ayudar2UltimaAccionUsuario { get; set; } = null;
        public string Ayudar2UltimaAccionNPC { get; set; } = null;
        public int Ayudar2Dialogo { get; set; } = 0;
        public bool Ayudar2EnCurso { get; set; } = false;
        public List<string> Ayudar2Historial { get; set; } = new List<string>();

        // Minijuego 3: Cofre Numérico
        public string NumeroSecretoCofre { get; set; } = null;
        public List<string> IntentosCofre { get; set; } = new List<string>();
        public List<string> RespuestasCofre { get; set; } = new List<string>(); // formato: "2E-1D" (2 exactos, 1 desordenado)
        public int IntentosRestantesCofre { get; set; } = 6;
        public bool CofreResuelto { get; set; } = false;

        // Métodos de ayuda
        public bool TodosLosMinijuegosCompletados()
        {
            return Minijuego1Completado && Minijuego2Completado && Minijuego3Completado;
        }

        public bool PuedeJugarPistolita()
        {
            return !AyudaResuelto && !JuegoPistolitaEnCurso;
        }

        public bool PuedeJugarPunga()
        {
            return !PungaResuelto && !JuegoPungaEnCurso;
        }

        public void ReiniciarJuegoPistolita()
        {
            JuegoPistolitaEnCurso = true;
            BalasJugador = 0;
            BalasOponente = 0;
            EscudosUsados = 0;
            AccionJugador = "";
        }

        public void ReiniciarJuegoPunga()
        {
            JuegoPungaEnCurso = true;
            VasoSeleccionado = -1;
            VasoCorrecto = new System.Random().Next(3);
        }

        public bool VerificarAvance()
        {
            return TodosLosMinijuegosCompletados() && CodigoObtenido;
        }

        public int EmojiCorrectoActual { get; set; } = 0;
    }
} 
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
    }
} 
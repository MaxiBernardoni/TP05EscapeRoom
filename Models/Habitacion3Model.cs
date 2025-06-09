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
        public bool TienePlata { get; set; } = false;
        public bool TieneEspejo { get; set; } = false;
        public bool TieneLentes { get; set; } = false;

        // Estado de avance
        public bool PuedeAvanzar { get; set; } = false;

        // Comentarios mostrados por el usuario al pasar el mouse
        public Dictionary<string, bool> ComentariosMostrados { get; set; } = new Dictionary<string, bool>();

        // Para minijuegos y lógica adicional
        public int IntentosPunga { get; set; } = 0;
        public int IntentosPistolita { get; set; } = 0;
        public bool JuegoPungaEnCurso { get; set; } = false;
        public bool JuegoPistolitaEnCurso { get; set; } = false;
    }
} 
namespace TuProyecto.Models
{
    public class Habitacion2Model
    {
        public int IntentosFallidos { get; set; } = 0;
        public bool JuegoGanado { get; set; } = false;

        public void RegistrarFallo()
        {
            IntentosFallidos++;
        }

        public bool AlcanzadoLimiteErrores()
        {
            return IntentosFallidos >= 3;
        }

        public int PenalizacionTiempoTotal()
        {
            return IntentosFallidos * 120; // 2 minutos por error
        }
    }
}

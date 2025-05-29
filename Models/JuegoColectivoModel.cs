using System;

namespace TuProyecto.Models
{
    public class JuegoColectivoModel
    {
        public string EleccionJugador { get; set; }
        public string EleccionMaquina { get; set; }
        public string Resultado { get; set; }

        private static readonly string[] Opciones = { "piedra", "papel", "tijera" };

        public JuegoColectivoModel(string eleccionJugador)
        {
            EleccionJugador = eleccionJugador.ToLower();
            EleccionMaquina = ObtenerOpcionAleatoria();
            Resultado = CalcularResultado();
        }

        private string ObtenerOpcionAleatoria()
        {
            var random = new Random();
            return Opciones[random.Next(Opciones.Length)];
        }

        private string CalcularResultado()
        {
            if (EleccionJugador == EleccionMaquina)
                return "Empate";

            if ((EleccionJugador == "piedra" && EleccionMaquina == "tijera") ||
                (EleccionJugador == "tijera" && EleccionMaquina == "papel") ||
                (EleccionJugador == "papel" && EleccionMaquina == "piedra"))
                return "Ganaste";

            return "Perdiste";
        }
    }
}

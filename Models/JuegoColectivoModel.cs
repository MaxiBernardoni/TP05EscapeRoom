using System;
using System.Collections.Generic;

namespace TuProyecto.Models
{
    public class JuegoColectivoModel
    {
        public string EleccionJugador { get; set; }
        public string EleccionConductor { get; set; }
        public string Resultado { get; set; }
        public int TiempoPenalizacion { get; set; }
        public string MensajeConductor { get; set; }
        public string AnimacionConductor { get; set; }
        public int RondaActual { get; set; }
        public int VictoriasJugador { get; set; }
        public int VictoriasConductor { get; set; }
        public bool JuegoTerminado => VictoriasJugador == 2 || VictoriasConductor == 2;
        public string GanadorFinal => VictoriasJugador > VictoriasConductor ? "Jugador" : "Conductor";

        private static readonly Dictionary<string, string> OpcionesEmoji = new()
        {
            { "piedra", "🪨" },
            { "papel", "📄" },
            { "tijera", "✂️" }
        };

        private static readonly Dictionary<string, List<string>> MensajesConductor = new()
        {
            { "Ganaste", new List<string> {
                "¡Bien jugado! Una victoria más y te doy un atajo.",
                "¡Me ganaste esta ronda! Veamos la siguiente.",
                "¡Excelente elección! Pero aún no termina."
            }},
            { "Perdiste", new List<string> {
                "¡Ja! Una victoria más y tomaremos la ruta más larga.",
                "¡Te gané esta! ¿Podrás recuperarte?",
                "¡Perdiste esta ronda! El tráfico está de mi lado."
            }},
            { "Empate", new List<string> {
                "¡Empate! Sigamos jugando.",
                "¡Mentes brillantes piensan igual! Siguiente ronda.",
                "¡Empate! Esto se pone interesante."
            }},
            { "VictoriaFinal", new List<string> {
                "¡Increíble! Has ganado el desafío. Tomaré todos los atajos posibles.",
                "¡Eres un maestro! Mereces llegar más rápido.",
                "¡Victoria merecida! Aceleraré el viaje."
            }},
            { "DerrotaFinal", new List<string> {
                "¡Ja ja! He ganado. Prepárate para el tráfico.",
                "¡Victoria para el conductor! Ruta escénica activada.",
                "¡Perdiste el desafío! Hora de tomar el camino largo."
            }}
        };

        public JuegoColectivoModel()
        {
            RondaActual = 1;
            VictoriasJugador = 0;
            VictoriasConductor = 0;
        }

        public JuegoColectivoModel(string eleccionJugador, int rondaActual = 1, int victoriasJugador = 0, int victoriasConductor = 0)
        {
            EleccionJugador = eleccionJugador.ToLower();
            EleccionConductor = ObtenerEleccionConductor();
            Resultado = CalcularResultado();
            RondaActual = rondaActual;
            VictoriasJugador = victoriasJugador;
            VictoriasConductor = victoriasConductor;

            // Actualizar victorias basado en el resultado
            if (Resultado == "Ganaste") VictoriasJugador++;
            else if (Resultado == "Perdiste") VictoriasConductor++;

            TiempoPenalizacion = CalcularPenalizacion();
            MensajeConductor = ObtenerMensajeConductor();
            AnimacionConductor = DeterminarAnimacion();
        }

        public string ObtenerEmoji(string eleccion) => OpcionesEmoji[eleccion];

        private string ObtenerEleccionConductor()
        {
            var opciones = new List<string>(OpcionesEmoji.Keys);
            return opciones[new Random().Next(opciones.Count)];
        }

        private string CalcularResultado()
        {
            if (EleccionJugador == EleccionConductor)
                return "Empate";

            return (EleccionJugador, EleccionConductor) switch
            {
                ("piedra", "tijera") => "Ganaste",
                ("tijera", "papel") => "Ganaste",
                ("papel", "piedra") => "Ganaste",
                _ => "Perdiste"
            };
        }

        private int CalcularPenalizacion()
        {
            if (!JuegoTerminado) return 0;

            return GanadorFinal == "Jugador" ? -30 : 60;
        }

        private string ObtenerMensajeConductor()
        {
            var mensajes = JuegoTerminado
                ? MensajesConductor[GanadorFinal == "Jugador" ? "VictoriaFinal" : "DerrotaFinal"]
                : MensajesConductor[Resultado];

            return mensajes[new Random().Next(mensajes.Count)];
        }

        private string DeterminarAnimacion()
        {
            if (JuegoTerminado)
            {
                return GanadorFinal == "Jugador" ? "conductor-feliz" : "conductor-risa";
            }

            return Resultado switch
            {
                "Ganaste" => "conductor-feliz",
                "Perdiste" => "conductor-risa",
                _ => "conductor-normal"
            };
        }
    }
} 
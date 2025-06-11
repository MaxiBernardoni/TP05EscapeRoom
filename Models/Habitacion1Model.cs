using System;
using System.Collections.Generic;
using System.Linq;

namespace TuProyecto.Models
{
    public class Habitacion1Model
    {
        public List<string> ObjetosDisponibles { get; set; }
        public List<string> ObjetosSeleccionados { get; set; } = new List<string>();
        public static string[] ObjetosFijos = new string[]
        {
            "🔑 Llaves",
            "🚌 SUBE",
            "☕ Café",
            "🍞 Tostadas",
            "🪞 Espejo",
            "👓 Lentes",
            "💵 Dinero",
            "🎒 Mochila"
        };

        public Habitacion1Model()
        {
            var random = new Random();
            ObjetosDisponibles = ObjetosFijos.OrderBy(_ => random.Next()).ToList();
        }

        public bool PuedeSalir()
        {
            bool tieneLlaves = ObjetosSeleccionados.Contains("🔑 Llaves");
            bool tieneSube = ObjetosSeleccionados.Contains("🚌 SUBE");
            bool tieneDesayuno = ObjetosSeleccionados.Contains("☕ Café") || ObjetosSeleccionados.Contains("🍞 Tostadas");

            return tieneLlaves && tieneSube && tieneDesayuno;
        }

        public void SeleccionarObjeto(string nombre)
        {
            if (ObjetosSeleccionados.Contains(nombre))
                ObjetosSeleccionados.Remove(nombre);
            else if (ObjetosSeleccionados.Count < 4)
                ObjetosSeleccionados.Add(nombre);
        }
    }
}

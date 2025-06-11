using System.Collections.Generic;
using System.Text.Json;

namespace TuProyecto.Models
{
    public class InventarioGlobal
    {
        public List<string> ObjetosSeleccionados { get; set; } = new List<string>();
        public const int CAPACIDAD_MAXIMA = 4;

        public bool TieneObjeto(string objeto)
        {
            return ObjetosSeleccionados.Contains(objeto);
        }

        public bool PuedeAgregarObjeto()
        {
            return ObjetosSeleccionados.Count < CAPACIDAD_MAXIMA;
        }

        public bool AgregarObjeto(string objeto)
        {
            if (!PuedeAgregarObjeto()) return false;
            if (TieneObjeto(objeto)) return false;
            
            ObjetosSeleccionados.Add(objeto);
            return true;
        }

        public bool QuitarObjeto(string objeto)
        {
            return ObjetosSeleccionados.Remove(objeto);
        }

        public static InventarioGlobal CargarDeSession(Microsoft.AspNetCore.Http.ISession session)
        {
            var json = session.GetString("inventario_global");
            return string.IsNullOrEmpty(json) 
                ? new InventarioGlobal() 
                : JsonSerializer.Deserialize<InventarioGlobal>(json);
        }

        public void GuardarEnSession(Microsoft.AspNetCore.Http.ISession session)
        {
            session.SetString("inventario_global", JsonSerializer.Serialize(this));
        }
    }
} 
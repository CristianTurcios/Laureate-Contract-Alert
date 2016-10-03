using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class Type_Provider
    {
        public List<string> ReturnTypeProvider()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<string> Tipo_Proveedores = (from type_prov in Entidad.SP_selectTypeProvider() select type_prov).ToList();
            return Tipo_Proveedores;
        }
    }
}
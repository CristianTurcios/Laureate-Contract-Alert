using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class Profile
    {
        public List<string> DevolverPerfiles()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<string> perfiles = (from perf in Entidad.SP_selectProfiles() select perf).ToList();
            return perfiles;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class SelectPass
    {
        public List<SP_selectPass_Result> ReturnPass(string email)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectPass_Result> Pass = (from pass in Entidad.SP_selectPass(email) select pass).ToList();
            return Pass;
        }
    }
}
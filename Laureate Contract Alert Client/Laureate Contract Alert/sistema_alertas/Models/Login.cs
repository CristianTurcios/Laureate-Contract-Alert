using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class Login
    {
       
        public List<Nullable<int>> ReturnUserId(string user, string pass)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<Nullable<int>> UserId = (from login in Entidad.SP_selectUserId(user, pass) select login).ToList();
            return UserId;
        }
    }
}
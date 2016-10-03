using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllUser
    {
        public List<SP_selectAllUser_Result> ReturnAllUser()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAllUser_Result> AllUser = (from user in Entidad.SP_selectAllUser() select user).ToList();
            return AllUser;
        }

    }
}
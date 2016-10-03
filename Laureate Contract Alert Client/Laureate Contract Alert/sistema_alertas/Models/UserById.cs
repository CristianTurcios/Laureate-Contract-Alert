using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class UserById
    {
        public List<SP_selectUserById_Result> ReturnUserById(int UserId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectUserById_Result> User = (from user in Entidad.SP_selectUserById(UserId) select user).ToList();
            return User;
        }
    }
}
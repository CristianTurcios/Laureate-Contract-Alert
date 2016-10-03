using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace sistema_alertas.Models
{
    public class AdminUserPermissions
    {
        public List<SP_selectAdminUserPermissions_Result> ReturnAdminUserPermissions(int CodeUser)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAdminUserPermissions_Result> AdminUserPermissions = (from Admin_User_Permissions in Entidad.SP_selectAdminUserPermissions(CodeUser) select Admin_User_Permissions).ToList();

          
            return AdminUserPermissions;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AdministratorContract
    {
        public List<SP_selectAdministrator_Result> ReturnAdministratorContract()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAdministrator_Result> AdministratorContract = (from Administrator in Entidad.SP_selectAdministrator() select Administrator).ToList();
            return AdministratorContract;
        }
    }
}
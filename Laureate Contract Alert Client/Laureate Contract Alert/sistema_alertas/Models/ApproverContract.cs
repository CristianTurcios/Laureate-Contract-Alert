using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class ApproverContract
    {
        public List<SP_selectApprover_Result> ReturnApproverContract()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();
            List<SP_selectApprover_Result> ApproverContract = (from Approver in Entidad.SP_selectApprover() select Approver).ToList();
            return ApproverContract;
        }
    }
}
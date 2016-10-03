using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllDetailsContractAboutToFinishForPDF
    {          
        public List<AboutToFinishContractForReportPDF_Result> ReturnAllDetailsContract()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<AboutToFinishContractForReportPDF_Result> AllDetailsContract = (from ContractDetails in Entidad.AboutToFinishContractForReportPDF() select ContractDetails).ToList();
            return AllDetailsContract;   
        }

        public List<AboutToFinishContractByAreaForReportPDF_Result> ReturnAllDetailsContractByArea(int Area)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<AboutToFinishContractByAreaForReportPDF_Result> AllDetailsContract = (from ContractDetails in Entidad.AboutToFinishContractByAreaForReportPDF(Area) select ContractDetails).ToList();
            return AllDetailsContract;
        }
    }
}
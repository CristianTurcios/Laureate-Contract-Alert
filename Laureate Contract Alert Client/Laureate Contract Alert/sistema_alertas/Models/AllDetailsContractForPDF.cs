using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllDetailsContractForPDF
    {
        public List<AllDetailsContractForReportPDF_Result> ReturnAllDetailsContract()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<AllDetailsContractForReportPDF_Result> AllDetailsContractForReportPDF = (from ContractDetails in Entidad.AllDetailsContractForReportPDF() select ContractDetails).ToList();
            return AllDetailsContractForReportPDF;
        }

        public List<AllDetailsContractByAreaForReportPDF_Result> ReturnAllDetailsContractByArea(int Area)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<AllDetailsContractByAreaForReportPDF_Result> AllDetailsContractForReportPDF = (from ContractDetails in Entidad.AllDetailsContractByAreaForReportPDF(Area) select ContractDetails).ToList();
            return AllDetailsContractForReportPDF;
        }
    }
}
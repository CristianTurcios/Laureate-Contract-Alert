using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllDetailsActiveContractForPDF
    {
       public List<ActiveContractForReportPDF_Result> ReturnAllDetailsContract()
       {
          sistema_alertasEntities Entidad = new sistema_alertasEntities();

          List<ActiveContractForReportPDF_Result> AllDetailsContract = (from ContractDetails in Entidad.ActiveContractForReportPDF() select ContractDetails).ToList();
          return AllDetailsContract;
       }

       public List<ActiveContractByAreaForReportPDF_Result> ReturnAllDetailsContractByArea(int Area)
       {
           sistema_alertasEntities Entidad = new sistema_alertasEntities();

           List<ActiveContractByAreaForReportPDF_Result> AllDetailsContract = (from ContractDetails in Entidad.ActiveContractByAreaForReportPDF(Area) select ContractDetails).ToList();
           return AllDetailsContract;
       }


    }
}
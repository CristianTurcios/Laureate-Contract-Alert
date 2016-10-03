using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllMethodPayment
    {
        public List<SP_selectAllMethodPayment_Result> ReturnAllMethodPayment()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAllMethodPayment_Result> AllmethodPayment = (from methodPayment in Entidad.SP_selectAllMethodPayment() select methodPayment).ToList();
            return AllmethodPayment;
        }
    }
}
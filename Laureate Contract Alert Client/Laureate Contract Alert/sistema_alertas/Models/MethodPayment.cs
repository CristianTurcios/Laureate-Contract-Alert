using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class MethodPayment
    {
        public List<SP_selectMethodPayment_Result> ReturnMethodPayment()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectMethodPayment_Result> MethodPayment = (from methodPayment in Entidad.SP_selectMethodPayment() select methodPayment).ToList();
            return MethodPayment;
        }
    }
}
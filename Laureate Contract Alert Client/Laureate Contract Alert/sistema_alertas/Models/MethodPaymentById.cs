using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class MethodPaymentById
    {
        public List<SP_selectMethodPaymentById_Result> ReturnMethodPayment(int MetodoPagoId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectMethodPaymentById_Result> MethodPayment = (from methodPayment in Entidad.SP_selectMethodPaymentById(MetodoPagoId) select methodPayment).ToList();
            return MethodPayment;
        }
    }
}
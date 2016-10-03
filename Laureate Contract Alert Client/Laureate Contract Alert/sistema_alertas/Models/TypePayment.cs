using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class TypePayment
    {
        public List<SP_selectTypePayment_Result> ReturnTypePayment()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectTypePayment_Result> TypePayment = (from typePayment in Entidad.SP_selectTypePayment() select typePayment).ToList();
            return TypePayment;
        }
    }
}
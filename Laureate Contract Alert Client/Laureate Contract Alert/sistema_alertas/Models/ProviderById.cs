using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class ProviderById
    {
        public List<SP_selectProviderById_Result> ReturnProviderById(int ProviderId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectProviderById_Result> Provider = (from provider in Entidad.SP_selectProviderById(ProviderId) select provider).ToList();
            return Provider;
        }
    }
}
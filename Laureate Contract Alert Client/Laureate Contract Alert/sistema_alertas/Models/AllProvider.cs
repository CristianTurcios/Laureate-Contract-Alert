using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllProvider
    {
        public List<SP_selectAllProvider_Result> ReturnAllProvider()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAllProvider_Result> AllProvider = (from provider in Entidad.SP_selectAllProvider() select provider).ToList();
            return AllProvider;
        }
    }
}
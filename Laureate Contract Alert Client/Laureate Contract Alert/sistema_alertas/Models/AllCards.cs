using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllCards
    {
        public List<SP_selectAllCards_Result> ReturnAllCards()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAllCards_Result> AllCards = (from Cards in Entidad.SP_selectAllCards() select Cards).ToList();
            return AllCards;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class Cards
    {
        public List<SP_selectCard_Result> ReturnCards()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectCard_Result> Cards = (from card in Entidad.SP_selectCard() select card).ToList();
            return Cards;
        }
    }
}
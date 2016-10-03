using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class Type_Cards
    {
        public List<string> ReturnTypeCards()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<string> typeCards = (from type_card in Entidad.SP_selectTypeCard() select type_card).ToList();
            return typeCards;
        }
    }
}
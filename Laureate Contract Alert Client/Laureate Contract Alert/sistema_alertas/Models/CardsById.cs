using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class CardsById
    {
        public List<SP_selectTarjetasById_Result> ReturnCards(int TarjetaId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectTarjetasById_Result> Cards = (from card in Entidad.SP_selectTarjetasById(TarjetaId) select card).ToList();
            return Cards;
        }
    }
}
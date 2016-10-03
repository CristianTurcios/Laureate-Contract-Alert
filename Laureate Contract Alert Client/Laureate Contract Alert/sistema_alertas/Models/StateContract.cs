using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class StateContract
    {
        public List<SP_selectState_Result> ReturnStateContract()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectState_Result> StateContract = (from stateContract in Entidad.SP_selectState() select stateContract).ToList();
            return StateContract;
        }
    }
}
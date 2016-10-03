using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class WeatherAlerts
    {
        public List<string> ReturnWeatherAlerts()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<string> WeatherAlerts = (from weatherAlerts in Entidad.SP_selectWeatherAlerts() select weatherAlerts).ToList();
            return WeatherAlerts;
        }
    }
}
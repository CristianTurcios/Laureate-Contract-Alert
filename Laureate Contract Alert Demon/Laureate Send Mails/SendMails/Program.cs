using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMails
{
    class Program
    {
        static void Main(string[] args)
        {
            string Server="172.20.33.13";
            string DataBaseName="sistema_alertas";
            
            Connection conexion = new Connection(Server,DataBaseName);

            conexion.OpenConecction();
            conexion.contractExpirationDateAlerts();
            Console.WriteLine("/*------------------------------------------------------------------------------*/");
            Console.WriteLine("/*------------------------------------------------------------------------------*/");
            conexion.alertsRenewalPaymentDate();
            Console.WriteLine("/*------------------------------------------------------------------------------*/");
            Console.WriteLine("/*------------------------------------------------------------------------------*/");
            conexion.warrantyExpirationDateAlert();
            Console.WriteLine("/*------------------------------------------------------------------------------*/");
            Console.WriteLine("/*------------------------------------------------------------------------------*/");
            conexion.alertCardExpirationDate();
            conexion.CloseConecction();                     
        }
    }
}

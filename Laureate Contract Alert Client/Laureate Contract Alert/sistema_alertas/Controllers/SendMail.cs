using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using sistema_alertas.Models;

namespace sistema_alertas.Controllers
{
    public class SendMail
    {
        public bool enviamail(string email, string username ,string pass)
        {
            //Estos ya son para el cuerpo, la direccion que tendra el correo etc, para el envio de los correos
            string To = email;
            string Subject = "Password recovery";
            string Body = "Your username and password for the Contract Laureate Alert system is: \n \n"+
            "Username: "+ username +"\n Password: "+ pass+
            "\n Link to the platform:  http://laur.cc/contractmanagement" +

            "\n \n This message was sent by a computer system, please do not reply to this message";

            MailMessage mail = new MailMessage("no-reply@laureate.net", To, Subject, Body);

            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.mandrillapp.com";
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("raul.rivera@laureate.net", "uuQ-6gYsZG5Yzz7Hv-a3yA");
                smtp.EnableSsl = true;
                smtp.Send(mail);

                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("No se ha podido enviar el correo");
                return false;
            }
        }

        public void SendMailWhenContractIsUpload(string contractName, string contractNumber,int idApprover,int idAdministrator, DateTime? DateEnd)
        {
            //Estos ya son para el cuerpo, la direccion que tendra el correo etc, para el envio de los correos
            string To = selectMailByUserId(idApprover)+","+selectMailByUserId(idAdministrator);
            string Subject = "New Contract is upload";
           
            string Body = "New Contract is upload in the platform 'Laureate Contract Alert' \n \n"+
                          "Name Contract: "+ contractName+ "\n"+
                          "Contract Number: "+ contractNumber+ "\n"+
                          "Date Expiration Contract: " + DateEnd.ToString()+ "\n \n"+
                          "Link to the platform:  http://laur.cc/contractmanagement";

            MailMessage mail = new MailMessage("no-reply@laureate.net", To, Subject, Body);

            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.mandrillapp.com";
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("raul.rivera@laureate.net", "uuQ-6gYsZG5Yzz7Hv-a3yA");
                smtp.EnableSsl = true;
                smtp.Send(mail);             
            }
            catch (Exception)
            {       
            }
        }


        public string selectMailByUserId(int userId)
        {
            sistema_alertasEntities entidad = new sistema_alertasEntities();
            
            List<string> Email = entidad.SP_selectEmailByUSerId(userId).ToList();

            return Email.ElementAt(0);
        }
    }
}
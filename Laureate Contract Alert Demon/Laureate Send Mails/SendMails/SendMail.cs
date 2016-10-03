using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace SendMails
{
    class SendMail
    {
        /*Con este bloque de codigo lo que se hace es que se envia un email para indicar que alguna de las fechas de un contrato esta proxima a acercarse y se debe de renovar
         * El correo, se envia a la persona administradora y aprobadora del contrato*/

       /* public bool enviamail(string nombreContrato,string numeroContrato,string motivoCorreo,string emailAprobador, string emailAdministrador)
        {
          //Estos dos son estrictamente  para  las credenciales de google, para el envio de los correos
          string From = "patito@gmail.com";
          string pass = "1234";
          //Estos dos son estrictamente  para  las credenciales de google, para el envio de los correos

          //Estos ya son para el cuerpo, la direccion que tendra el correo etc, para el envio de los correos
          string To = emailAprobador+","+emailAdministrador;
          string Subject = motivoCorreo;
          string Body = "El contrato numero: "+numeroContrato+ " con nombre " + nombreContrato + " se encuentra a pocos dias de expirar" + "\n" +
          "\n \n Este mensaje ha sido enviado por un sistema de computo, porfavor no responda este mensaje";
            
          MailMessage mail = new MailMessage(From, To, Subject, Body);
        
          try
          {
             SmtpClient smtp = new SmtpClient();
             smtp.Host = "smtp.gmail.com";
             smtp.Port = 587;
             smtp.Credentials = new NetworkCredential(From, pass); // password for connection smtp if u dont have have then pass blank

             smtp.EnableSsl = true;
             smtp.Send(mail);
             return true;
          }
          catch (Exception)
          {
             Console.WriteLine("No se ha podido enviar el correo");
             return false;
          }
        }*/
  /*Fin bloque de codigo lo que se hace es que se envia un email para indicar que alguna de las fechas de un contrato esta proxima a acercarse y se debe de renovar
  * El correo, se envia a la persona administradora y aprobadora del contrato*/

 /*Con este bloque de codigo lo que se hace es que se envia un email para indicar que alguna de las fechas de un contrato esta proxima a acercarse y se debe de renovar
 * El correo, se envia a la persona administradora y aprobadora del contrato*/

        public bool enviamail(int contratoId, string nombreContrato, string numeroContrato, string motivoCorreo, string emailAprobador, string emailAdministrador,int TiempoRecordatorios)
        {           
            //Estos ya son para el cuerpo, la direccion que tendra el correo etc, para el envio de los correos
            string To = emailAprobador + "," + emailAdministrador;          
            string Subject = motivoCorreo;
            string Body = "";

            if (motivoCorreo.Equals("Contract Close to Expiration"))
            {
                Body = "The contract called: " + nombreContrato + "\n Number: " + numeroContrato + "\n It is close to expiring" + "\n \n" +
                "Link to the contract in the platform: http://190.5.107.139/Main/DetallesContrato?ContractId=" + contratoId+
                "\n \n This message was sent by a computer system, please do not reply to this message \n \n";                            
            }
            else
                if (motivoCorreo.Equals("Date Payment Contract Renewal Close to Expiration"))
             {
                 Body = "The renewal date of payment of the contract with name: " + nombreContrato +"\n and number: " + numeroContrato + "\n It is close to expiring, please make payment of this contract" + "\n \n" +
                 "Link to the contract in the platform: http://190.5.107.139/Main/DetallesContrato?ContractId=" + contratoId +
                 "\n \n This message was sent by a computer system, please do not reply to this message";
             }
             else
                 if (motivoCorreo.Equals("Contract Expiration Date Guarantee Close to Expiration"))
              {
                  Body = "The expiration date of the contract guarantee with name: " + nombreContrato + "\n and number: " + numeroContrato + "\n It is close to expiring \n \n" +
                  "Link to the contract in the platform: http://190.5.107.139/Main/DetallesContrato?ContractId=" + contratoId +
                  "\n \n This message was sent by a computer system, please do not reply to this message";
              }
                      
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

/*Fin bloque de codigo lo que se hace es que se envia un email para indicar que alguna de las fechas de un contrato esta proxima a acercarse y se debe de renovar
* El correo, se envia a la persona administradora y aprobadora del contrato*/


 /*Con este bloque de codigo lo que se hace es que se envia un email para indicar que la fecha de vencimiento de una tarjeta esta proxima a acercarse (Unicamente envia un correo para la fecha de vencimiento de las tarjetas)
  * y se debe de renovar. El correo, se envia a la persona administradora y aprobadora del contrato
  Nota: Pude simplemente utilizar el bloque de codigo anterior para que recibiera los parametros que debe llevar en especifico el mail de tarjetas, pero tenia pereza de cambiar el codigo que ya tenia hecho ya que esto se debe cambiar en la clase
  connection tambien, asi que como tengo pereza mejor solo creo otro bloque de codigo y cheque :)*/

        public bool enviamailTarjetas(int contratoId,string nombreContrato, string numeroContrato, string motivoCorreo, string NombrePersonaAsignadaTarjeta, string TipoTarjeta, string UltimosDigitosTarjeta, string emailAprobador, string emailAdministrador,int TiempoRecordatorios)
        {          
            //Estos ya son para el cuerpo, la direccion que tendra el correo etc, para el envio de los correos
            string To = emailAprobador+ "," + emailAdministrador;
            string Subject = motivoCorreo;
            string Body = "The card is associated with the contract with name: " + nombreContrato + "\n Number: " + numeroContrato+ "\n"
                        + " and card type: " + TipoTarjeta + "-" + UltimosDigitosTarjeta + "\n Where the person assigned to the card is: " + NombrePersonaAsignadaTarjeta +
                          "\n It is close to expiring" + "\n \n" +
                          "Link to the contract in the platform: http://190.5.107.139/Main/DetallesContrato?ContractId=" + contratoId +
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

/*Fin bloque de codigo lo que se hace es que se envia un email para indicar que la fecha de vencimiento de una tarjeta esta proxima a acercarse (Unicamente envia un correo para la fecha de vencimiento de las tarjetas)
* y se debe de renovar. El correo, se envia a la persona administradora y aprobadora del contrato
Nota: Pude simplemente utilizar el bloque de codigo anterior para que recibiera los parametros que debe llevar en especifico el mail de tarjetas, pero tenia pereza de cambiar el codigo que ya tenia hecho ya que esto se debe cambiar en la clase
connection tambien, asi que como tengo pereza mejor solo creo otro bloque de codigo y cheque :)*/

    }
}
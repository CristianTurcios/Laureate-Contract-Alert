using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace SendMails
{
    class Connection
    {
        private string datosConexion="";
        private SqlConnection conexion;
        SendMail envioCorreos = new SendMail();
      
        public Connection(string server, string databaseName)
        {
            this.datosConexion = "Data Source=" + server + ";Initial Catalog=" + databaseName + ";Integrated Security=true;";
            InitialConecction(this.datosConexion);
        }

        public void InitialConecction(string datosConexion)
        {
            conexion = new SqlConnection();
            conexion.ConnectionString = datosConexion;
        }

        public void OpenConecction()
        {
            conexion.Open();
            Console.WriteLine("La Conexión se ha realizado de forma exitosa");
        }

        public void CloseConecction()
        {
            conexion.Close();
            Console.WriteLine("La Conexión se ha cerrado de forma exitosa");
        }

 /*Este bloque de codigo realiza una consulta a la base de datos para saber la fecha de vencimiento de contrato, si esta fecha es igual o mayor a la de fecha de alertas, entonces
 * generara un correo avisando al administrador y al usuario del contrato que este ya se encuentra proximo a vencer para que puedan tomar alguna accion ya sea renovarlo o cancelarlo definitivamente*/
        public void contractExpirationDateAlerts()
        {
          string motivoCorreo = "Contract Close to Expiration";

          string Consulta = "SELECT dbo.contratos.ContratoId, dbo.contratos.Nombre_Contrato, dbo.contratos.Numero_Contrato, dbo.contratos.Fecha_Finalizacion,"+ 
                              " dbo.estados.Descripcion, dbo.contratos.Usuario_AprobadorId, dbo.contratos.Usuario_AdministradorId,dbo.recordatorios.Descripcion AS Expr1"+
                              " FROM dbo.contratos INNER JOIN dbo.estados ON dbo.contratos.EstadoId = dbo.estados.EstadoId INNER JOIN"+
                              " dbo.recordatorios ON dbo.contratos.RecordatorioId = dbo.recordatorios.RecordatorioId";

          SqlCommand cmd = new SqlCommand(Consulta, conexion);
         
          try
          {
            DataTable tabla = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(tabla);
           
            DateTime Fecha_Actual = DateTime.Now;
             
            for(int i = 0; i < tabla.Rows.Count; i++)
            {          
               string Descripcion = tabla.Rows[i]["Descripcion"].ToString();

               if (!Descripcion.Equals("Expired") )
               {                    
                  int TiempoRecordatorios = Convert.ToInt32(tabla.Rows[i]["Expr1"].ToString());
                  DateTime Fecha_Finalizacion_Contrato = Convert.ToDateTime(tabla.Rows[i]["Fecha_Finalizacion"]);
              
                  if (DateTime.Compare(Fecha_Finalizacion_Contrato.AddDays(-TiempoRecordatorios), Fecha_Actual) <= 0)
                  {
                     
                     int ContratoId =(int)tabla.Rows[i]["ContratoId"];
                     int idAprobador = (int)tabla.Rows[i]["Usuario_AprobadorId"];
                     int idAdministradorContrato = (int)tabla.Rows[i]["Usuario_AdministradorId"];
                     string Nombre_Contrato = tabla.Rows[i]["Nombre_Contrato"].ToString();
                     string Numero_Contrato = tabla.Rows[i]["Numero_Contrato"].ToString();
                     
                     envioCorreos.enviamail(ContratoId,Nombre_Contrato, Numero_Contrato,motivoCorreo,SelectUser(idAprobador),SelectUser(idAdministradorContrato),TiempoRecordatorios);
                        
                     if (Descripcion.Equals("Active"))
                     {
                        cmd = new SqlCommand("Update Contratos set EstadoId=2 where ContratoId="+ContratoId, conexion);
                        cmd.ExecuteNonQuery();
                     }

                     if (Fecha_Finalizacion_Contrato.ToString("d").Equals(Fecha_Actual.ToString("d")))
                     {                             
                         cmd = new SqlCommand("Update Contratos set EstadoId=3 where ContratoId=" + ContratoId, conexion);
                         cmd.ExecuteNonQuery();
                     }
                                         
                     Console.WriteLine("Correo para el contrato: " + Nombre_Contrato + " Con numero de contrato: " + Numero_Contrato +" Con motivo de: "+motivoCorreo +" Enviado Con exito");                      
                  } 
               }                          
            }      
          }
          catch (SqlException e)
          {
             Console.WriteLine(e.Message);
          }      
        }
  
  /*Fin bloque de codigo realiza una consulta a la base de datos para saber la fecha de vencimiento de contrato, si esta fecha es igual o mayor a la de fecha de alertas, entonces
  * generara un correo avisando al administrador y al usuario del contrato que este ya se encuentra proximo a vencer para que puedan tomar alguna accion ya sea renovarlo o cancelarlo definitivamente*/

  /*Este bloque de codigo realiza una consulta a la base de datos para saber la fecha de renovacion de pago del contrato, si esta fecha es igual o mayor a la de fecha de alertas, entonces
  * generara un correo avisando al administrador y al usuario del contrato que este ya se encuentra proximo a vencer para que puedan tomar alguna accion ya sea renovarlo o cancelarlo definitivamente*/
        public void alertsRenewalPaymentDate()
        {
            string motivoCorreo = "Date Payment Contract Renewal Close to Expiration";

            string Consulta = "SELECT dbo.contratos.ContratoId,dbo.contratos.Nombre_Contrato, dbo.contratos.Numero_Contrato, dbo.estados.Descripcion," +
            " dbo.recordatorios.Descripcion AS Expr1," +
            " dbo.contratos.Fecha_Renovacion_Pago,dbo.tipo_pagos.Tipo_PagoId,dbo.contratos.Usuario_AdministradorId,dbo.contratos.Usuario_AprobadorId" +
            " FROM dbo.contratos INNER JOIN" +
            " dbo.tipo_pagos ON dbo.contratos.Tipo_PagoId = dbo.tipo_pagos.Tipo_PagoId INNER JOIN" +
            " dbo.estados ON dbo.contratos.EstadoId = dbo.estados.EstadoId INNER JOIN" +
            " dbo.recordatorios ON dbo.contratos.RecordatorioId = dbo.recordatorios.RecordatorioId";

            SqlCommand cmd = new SqlCommand(Consulta, conexion);
           
            try
            {
                DataTable tabla = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tabla);

                DateTime Fecha_Actual = DateTime.Now;
               
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    string Descripcion = tabla.Rows[i]["Descripcion"].ToString();

                    if (!Descripcion.Equals("Expired"))
                    {                
                        int TiempoRecordatorios = Convert.ToInt32(tabla.Rows[i]["Expr1"].ToString());
                        DateTime Fecha_Renovacion_Pago = Convert.ToDateTime(tabla.Rows[i]["Fecha_Renovacion_Pago"]);
                     
                        if (DateTime.Compare(Fecha_Renovacion_Pago.AddDays(-TiempoRecordatorios), Fecha_Actual) <= 0)
                        {
                            int ContratoId = (int)tabla.Rows[i]["ContratoId"];
                            string Nombre_Contrato = tabla.Rows[i]["Nombre_Contrato"].ToString();
                            string Numero_Contrato = tabla.Rows[i]["Numero_Contrato"].ToString();
                            int TipoPago = (int)(tabla.Rows[i]["Tipo_PagoId"]);
                            int idAprobador = (int)tabla.Rows[i]["Usuario_AprobadorId"];
                            int idAdministradorContrato = (int)tabla.Rows[i]["Usuario_AdministradorId"];

                            envioCorreos.enviamail(ContratoId,Nombre_Contrato, Numero_Contrato, motivoCorreo, SelectUser(idAprobador), SelectUser(idAdministradorContrato),TiempoRecordatorios);

                            if (TipoPago == 1)
                            {                              
                                Fecha_Renovacion_Pago = Fecha_Renovacion_Pago.AddYears(1);
                                string consulta = "Update Contratos set Fecha_Renovacion_Pago= " + "'" + Fecha_Renovacion_Pago + "'" + " where ContratoId=" + ContratoId;                               

                                cmd = new SqlCommand(consulta, conexion);
                                cmd.ExecuteNonQuery();
                            }
                            else      
                              if (TipoPago == 2)
                              {                                                                
                                 Fecha_Renovacion_Pago = Fecha_Renovacion_Pago.AddMonths(4);
                                 string consulta = "Update Contratos set Fecha_Renovacion_Pago= " +"'"+Fecha_Renovacion_Pago+"'"+ " where ContratoId=" + ContratoId;
                                 
                                 cmd = new SqlCommand(consulta, conexion);
                                 cmd.ExecuteNonQuery();
                              }
                            else
                              if (TipoPago == 3)
                              {
                                 Fecha_Renovacion_Pago = Fecha_Renovacion_Pago.AddMonths(3);
                                 string consulta = "Update Contratos set Fecha_Renovacion_Pago= " + "'" + Fecha_Renovacion_Pago + "'" + " where ContratoId=" + ContratoId;                               

                                 cmd = new SqlCommand(consulta, conexion);
                                 cmd.ExecuteNonQuery();
                              }
                            else
                              if (TipoPago == 4)
                              {
                                 Fecha_Renovacion_Pago = Fecha_Renovacion_Pago.AddMonths(1);
                                 string consulta = "Update Contratos set Fecha_Renovacion_Pago= " + "'" + Fecha_Renovacion_Pago + "'" + " where ContratoId=" + ContratoId;
                                
                                 cmd = new SqlCommand(consulta, conexion);
                                 cmd.ExecuteNonQuery();
                              }     
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }
 /*Fin bloque de codigo realiza una consulta a la base de datos para saber la fecha de renovacion de pago del contrato, si esta fecha es igual o mayor a la de fecha de alertas, entonces
 * generara un correo avisando al administrador y al usuario del contrato que este ya se encuentra proximo a vencer para que puedan tomar alguna accion ya sea renovarlo o cancelarlo definitivamente*/

/*Este bloque de codigo realiza una consulta a la base de datos para saber la fecha de vencimiento de la garantia del contrato, si esta fecha es igual o mayor a la de fecha de alertas, entonces
* generara un correo avisando al administrador y al usuario del contrato que este ya se encuentra proximo a vencer para que puedan tomar alguna accion ya sea renovarlo o cancelarlo definitivamente*/
       
        public void warrantyExpirationDateAlert()
        {
            string motivoCorreo = "Contract Expiration Date Guarantee Close to Expiration";

            string Consulta = "SELECT dbo.contratos.ContratoId,dbo.contratos.Nombre_Contrato, dbo.contratos.Numero_Contrato, dbo.estados.Descripcion, dbo.recordatorios.Descripcion AS Expr1," +
           " dbo.contratos.Fecha_Vencimiento_Garantia, dbo.contratos.Usuario_AprobadorId, dbo.contratos.Usuario_AdministradorId" +
           " FROM dbo.contratos INNER JOIN dbo.estados ON dbo.contratos.EstadoId = dbo.estados.EstadoId INNER JOIN" +
           " dbo.recordatorios ON dbo.contratos.RecordatorioId = dbo.recordatorios.RecordatorioId" +
           " where Fecha_Vencimiento_Garantia != '1901-01-01'";
  
           SqlCommand cmd = new SqlCommand(Consulta, conexion);

           try
           {
             DataTable tabla = new DataTable();
             SqlDataAdapter da = new SqlDataAdapter(cmd);
             da.Fill(tabla);

             DateTime Fecha_Actual = DateTime.Now;

             for (int i = 0; i < tabla.Rows.Count; i++)
             {
               string Descripcion = tabla.Rows[i]["Descripcion"].ToString();

               if (!Descripcion.Equals("Expired"))
               {
                  DateTime Fecha_Vencimiento_Garantia = Convert.ToDateTime(tabla.Rows[i]["Fecha_Vencimiento_Garantia"]);
                  if(!Fecha_Vencimiento_Garantia.ToString("d").Equals(""))
                  {                     
                     int TiempoRecordatorios = Convert.ToInt32(tabla.Rows[i]["Expr1"].ToString());
                                       
                     if (DateTime.Compare(Fecha_Vencimiento_Garantia.AddDays(-TiempoRecordatorios), Fecha_Actual) <= 0)
                     {
                        int ContratoId = (int)tabla.Rows[i]["ContratoId"];
                        string Nombre_Contrato = tabla.Rows[i]["Nombre_Contrato"].ToString();
                        string Numero_Contrato = tabla.Rows[i]["Numero_Contrato"].ToString();
                        int idAprobador = (int)tabla.Rows[i]["Usuario_AprobadorId"];
                        int idAdministradorContrato = (int)tabla.Rows[i]["Usuario_AdministradorId"];

                        bool bandera = envioCorreos.enviamail(ContratoId,Nombre_Contrato, Numero_Contrato, motivoCorreo, SelectUser(idAprobador), SelectUser(idAdministradorContrato),TiempoRecordatorios);

                        if (bandera)
                             Console.WriteLine("Correo para el contrato: " + Nombre_Contrato + " Con numero de contrato: " + Numero_Contrato + " Con motivo de: " + motivoCorreo + " Enviado Con exito");
                        else
                             Console.WriteLine("Correo para el contrato: " + Nombre_Contrato + " Con numero de contrato: " + Numero_Contrato + " Con motivo de: " + motivoCorreo + " No se ha podido enviar");
                     }
                  }                                              
               }
             }
           }
           catch (SqlException e)
           {
             Console.WriteLine(e.Message);
           }
        }
 /*Fin bloque de codigo realiza una consulta a la base de datos para saber la fecha de vencimiento de la garantia del contrato, si esta fecha es igual o mayor a la de fecha de alertas, entonces
 * generara un correo avisando al administrador y al usuario del contrato que este ya se encuentra proximo a vencer para que puedan tomar alguna accion ya sea renovarlo o cancelarlo definitivamente*/

/*Este bloque de codigo realiza una consulta a la base de datos para saber la fecha de vencimiento de la garantia del contrato, si esta fecha es igual o mayor a la de fecha de alertas, entonces
* generara un correo avisando al administrador y al usuario del contrato que este ya se encuentra proximo a vencer para que puedan tomar alguna accion ya sea renovarlo o cancelarlo definitivamente*/

        public void alertCardExpirationDate()
        {
            string motivoCorreo = "Card Expiration Date";

            string Consulta = "SELECT  dbo.contratos.ContratoId,dbo.contratos.Nombre_Contrato, dbo.contratos.Numero_Contrato, dbo.estados.Descripcion, dbo.recordatorios.Descripcion AS Expr1," +
            " dbo.tarjetas.Nombre_Persona_Asignada_A_Tarjeta, dbo.tipo_tarjetas.Nombre_Tarjeta, dbo.tarjetas.Fecha_Vencimiento_Tarjeta,dbo.tarjetas.Ultimos_Digitos_Tarjeta," +
            " dbo.contratos.Usuario_AprobadorId,dbo.contratos.Usuario_AdministradorId" +
            " FROM dbo.contratos INNER JOIN" +
            " dbo.estados ON dbo.contratos.EstadoId = dbo.estados.EstadoId INNER JOIN" +
            " dbo.recordatorios ON dbo.contratos.RecordatorioId = dbo.recordatorios.RecordatorioId INNER JOIN" +
            " dbo.tarjetas ON dbo.contratos.TarjetaId = dbo.tarjetas.TarjetaId INNER JOIN" +
            " dbo.tipo_tarjetas ON dbo.tarjetas.Tipo_TarjetaId = dbo.tipo_tarjetas.Tipo_TarjetaId"+
            " where dbo.tarjetas.TarjetaId  !=0";

            SqlCommand cmd = new SqlCommand(Consulta, conexion);

            try
            {
                DataTable tabla = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(tabla);

                DateTime Fecha_Actual = DateTime.Now;

                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    string Descripcion = tabla.Rows[i]["Descripcion"].ToString();

                    if (!Descripcion.Equals("Expired"))
                    {
                        DateTime Fecha_Vencimiento_Tarjeta = Convert.ToDateTime(tabla.Rows[i]["Fecha_Vencimiento_Tarjeta"]);

                        if (!Fecha_Vencimiento_Tarjeta.ToString("d").Equals(""))
                        {
                            int TiempoRecordatorios = Convert.ToInt32(tabla.Rows[i]["Expr1"].ToString());
                         
                            if (DateTime.Compare(Fecha_Vencimiento_Tarjeta.AddDays(-TiempoRecordatorios), Fecha_Actual) <= 0)
                            {
                                int ContratoId = (int)tabla.Rows[i]["ContratoId"];
                                string Nombre_Contrato = tabla.Rows[i]["Nombre_Contrato"].ToString();
                                string Numero_Contrato = tabla.Rows[i]["Numero_Contrato"].ToString();
                                string Nombre_Persona_Asignada_A_Tarjeta = tabla.Rows[i]["Nombre_Persona_Asignada_A_Tarjeta"].ToString();
                                string Nombre_Tarjeta = tabla.Rows[i]["Nombre_Tarjeta"].ToString();
                                string Ultimos_Digitos_Tarjeta = tabla.Rows[i]["Ultimos_Digitos_Tarjeta"].ToString();
                                int idAprobador = (int)tabla.Rows[i]["Usuario_AprobadorId"];
                                int idAdministradorContrato = (int)tabla.Rows[i]["Usuario_AdministradorId"];

                                bool bandera = envioCorreos.enviamailTarjetas(ContratoId,Nombre_Contrato, Numero_Contrato, motivoCorreo, Nombre_Persona_Asignada_A_Tarjeta, Nombre_Tarjeta, Ultimos_Digitos_Tarjeta, SelectUser(idAprobador), SelectUser(idAdministradorContrato),TiempoRecordatorios);

                                if (bandera)
                                    Console.WriteLine("Correo para el contrato: " + Nombre_Contrato + " Con numero de contrato: " + Numero_Contrato + " Con motivo de: " + motivoCorreo + " Enviado Con exito");
                                else
                                    Console.WriteLine("Correo para el contrato: " + Nombre_Contrato + " Con numero de contrato: " + Numero_Contrato + " Con motivo de: " + motivoCorreo + " No se ha podido enviar");
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }
 /*Fin bloque de codigo realiza una consulta a la base de datos para saber la fecha de vencimiento de la garantia del contrato, si esta fecha es igual o mayor a la de fecha de alertas, entonces
 * generara un correo avisando al administrador y al usuario del contrato que este ya se encuentra proximo a vencer para que puedan tomar alguna accion ya sea renovarlo o cancelarlo definitivamente*/

/*Este bloque de codigo lo unico que hace es hacer una consulta a la base de datos y traer el email de un usuario de acuerdo a su id en la base 
 * Esto va a servir para enviarle un correo a los administradores y aprobadores especificos de un contrato y no para enviar un correo a las personas que no le interesen los contratos que no son suyos
 * por ejemplo que la gente de Main Office solo reciban las alertas contratos que a ellos le interesan y que a la gente de IT solo reciban las alertas de contratos que les corresponden*/

        public string SelectUser(int UserId)
        {
            string Consulta = "select email from usuarios where UsuarioId ="+UserId;
            SqlCommand cmd = new SqlCommand(Consulta, conexion);

            DataTable tabla = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(tabla);
            string emailUsuario = tabla.Rows[0]["email"].ToString();
            return emailUsuario;
        }

/*Fin bloque de codigo lo unico que hace es hacer una consulta a la base de datos y traer el email de un usuario de acuerdo a su id en la base 
* Esto va a servir para enviarle un correo a los administradores y aprobadores especificos de un contrato y no para enviar un correo a las personas que no le interesen los contratos que no son suyos
* por ejemplo que la gente de Main Office solo reciban las alertas contratos que a ellos le interesan y que a la gente de IT solo reciban las alertas de contratos que les corresponden*/ 
    }
}

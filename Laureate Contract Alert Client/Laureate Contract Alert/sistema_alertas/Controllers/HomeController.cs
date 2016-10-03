using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sistema_alertas.Models;

namespace sistema_alertas.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [HttpGet]
        public ActionResult Index(int nivelMensaje=0)
        {
            //Cuando el usuario sale del sistema (asumiendo que ya estaba logueado) se libera la key que el usuario tenia en ese momento                                        
            Session.Remove("UserKey");
            ViewData["NivelMensaje"] = nivelMensaje;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string user, string pass)
        {
            Login login = new Login();
            List<Nullable<int>>  comprobarConsulta = login.ReturnUserId(user,pass);
           
            //Si el usuario se encuentra logueado en el sistema, a fuerza el tamaño de la lista sera distinto de 0 y asi es como se hace la comprobacion que el usuario existe en el sistema
            
            if( comprobarConsulta.Count!=0)
            {
                //Se hace una nueva consulta a la base para determinar el cual es el id del usuario (se manda el elemento 0 de la lista porque ya se que siempre viene 1 unicamente entonces no ocupo recorrerla, tomando el primero basta)
                Session["UserId"] = Convert.ToInt32(comprobarConsulta.ElementAt(0));
                DateTime fechaActual = DateTime.Now;
               
                //Se asigna una llave al usuario para poder entrar al sistema y que se ira comprabando en cada accion que este realice para comprobar que es un usuario legitimamente logueado en el sistema
                Session["UserKey"] = "1udhuasgd17yk akjss;asd-as.a,s"+fechaActual.ToString();

                return RedirectToAction("main", "Main");
            }
            else
            {
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index",
                    NivelMensaje = 3,
                    Mensaje = "Usuario y contraseña no figuran en la base de datos"
                });
            }
        }

        [HttpGet]
        public ActionResult RecuperarContraseña()
        {                      
            return View();
        }

        [HttpPost]
        public ActionResult RecuperarContraseña(string email)
        {           
            SelectPass pass = new SelectPass();
            List<SP_selectPass_Result> passUser = pass.ReturnPass(email);

            int nivelMensaje = 2;

            try
            {                              
                SendMail mail = new SendMail();
            
                foreach(SP_selectPass_Result user in passUser)
                {
                   mail.enviamail(email, user.username, user.pass);
                   nivelMensaje = 1;
                }                
            }
            catch(Exception)
            {               
                nivelMensaje = 2;
            }

            return RedirectToRoute(new
            {
                controller = "Home",
                action = "Index",
                NivelMensaje = nivelMensaje,               
            });
        }
    }
}

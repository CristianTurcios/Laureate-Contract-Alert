using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices;
using sistema_alertas.Models;

namespace sistema_alertas.Controllers
{
    public class BackEndController : Controller
    {  
        sistema_alertasEntities Entidad = new sistema_alertasEntities();
           
        public ActionResult MainBackEnd()
        {
            string UserKey = "";
            int permisos = 0;

            try
            {
                UserKey = Session["UserKey"].ToString();
                permisos = (int)Session["UserPermission"];
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }


            //Para acceder a la parte de la administracion de los contratos se debe hacer una comprobacion del nivel que tenga el usuario para asi restringir las acciones que puede realizar
            //un nivel de perfil 1 significa que es un usuario super admin y puede realizar todas las acciones en el sistema

           if(permisos == 1)
           {
              return View();
           }

           // un nivel de perfil 2 y 3 significa que estos usuarios son aministradores y que pueden editar y crear algunos elementos pero no todos como por ejemplo crear usuarios
           else
               if (permisos == 2 || permisos == 3 || permisos == 4)
             {
                 return View("../BackEnd/MainBackEnd2");
             }

           //Un nivel de perfil 5 y 6 significa que estos solo son usuarios ya sea de Main office y usuarios de IT y que solo tienen permiso a agregar cosas al sistema, mas no tienen permiso de editar o eliminar
           else
             if (permisos == 6 || permisos == 7 || permisos == 8)
             {
                 
                 return RedirectToAction("MainBackEnd3", "BackEnd");   
             }
            // Y si el usuario no cuenta con alguno de los anteriores permisos significa que no puede agregar, ni editar alguna informacion en el sistema, este caso solo se da en los usuarios aprobadores del sistema
           else
           {
               int nivelMensaje = 3;
               string mensaje = "No Cuenta con los permisos necesarios para acceder a esta parte del sitio web";
               return RedirectToRoute(new
               {
                   controller = "Main",
                   action = "main",
                   NivelMensaje = nivelMensaje,
                   Mensaje = mensaje
               });
           }    
        }

        public ActionResult MainBackEnd2()
        {
            string UserKey = "";
           
            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        public ActionResult MainBackEnd3()
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


// Codigo para agregar usuario tanto el metodo cuando se recibe una peticion Get de la pagina asi como el envio de datos por POST

        [HttpGet]
        public ActionResult agregarUsuario(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";
            int permisos = 0;
           
            try
            {
                UserKey = Session["UserKey"].ToString();
                permisos = (int)Session["UserPermission"];
                ViewData["permisos"] = permisos;
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            Profile perfil = new Profile();
            ViewData["perfiles"] = perfil.DevolverPerfiles();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;
            return View();
        }


        [HttpPost]
        public ActionResult agregarUsuario(string Name, string last_name, string email, string password, string phone, string Code_User, string username, HttpPostedFileBase imagen, int[] charge_within_the_system)
        {
            DateTime fecha_creacion = DateTime.Now;
            string mensaje = "";
            int nivelMensaje = 0;
     
            if (charge_within_the_system == null)
            {
                nivelMensaje = 5;
            }
            if (charge_within_the_system.Length >= 4)
            {
                nivelMensaje = 6;
            }
            
            else
              if (charge_within_the_system.Length == 1)
              {
                try
                {
                  if (imagen == null)
                  {
                     string imagen2 = Convert.ToString(imagen);
                     Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, imagen2, charge_within_the_system[0]);
                     mensaje = "Data inserted successfully";
                     nivelMensaje = 2;
                  }
                  else
                    if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png")
                    {
                      mensaje = "Error debe insertar una imagen";
                      nivelMensaje = 4;
                    }
                    else
                    {
                        // Codigo del programa para reemplazar caracteres no permitidos 
                        //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                        string nombreArchivo = imagen.FileName.Replace("\\", "");
                        nombreArchivo = nombreArchivo.Replace("//", "");
                        nombreArchivo = nombreArchivo.Replace(":", "");
                        nombreArchivo = nombreArchivo.Replace("*", "");
                        nombreArchivo = nombreArchivo.Replace("?", "");
                        nombreArchivo = nombreArchivo.Replace("<", "");
                        nombreArchivo = nombreArchivo.Replace(">", "");
                        nombreArchivo = nombreArchivo.Replace("|", "");
                        //Fin Codigo del programa para reemplazar caracteres no permitidos 
                        //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                       string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                       imagen.SaveAs("C:\\Uploads\\" + archivo);

                        Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, archivo, charge_within_the_system[0]);
                        mensaje = "Data inserted successfully";
                        nivelMensaje = 2;
                     }
                 }
                catch (Exception)
                {
                    mensaje = "Error";
                    nivelMensaje = 3;
                }                
              }

              else
                  if (charge_within_the_system.Length == 2)
                  {
                      try
                      {
                          if (imagen == null)
                          {
                              string imagen2 = Convert.ToString(imagen);
                              Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, imagen2, charge_within_the_system[0]);
                              Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, imagen2, charge_within_the_system[1]);
                              mensaje = "Data inserted successfully";
                              nivelMensaje = 2;
                          }
                          else
                              if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png")
                              {
                                  mensaje = "Error debe insertar una imagen";
                                  nivelMensaje = 4;
                              }
                              else
                              {
                                  // Codigo del programa para reemplazar caracteres no permitidos 
                                  //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                                  string nombreArchivo = imagen.FileName.Replace("\\", "");
                                  nombreArchivo = nombreArchivo.Replace("//", "");
                                  nombreArchivo = nombreArchivo.Replace(":", "");
                                  nombreArchivo = nombreArchivo.Replace("*", "");
                                  nombreArchivo = nombreArchivo.Replace("?", "");
                                  nombreArchivo = nombreArchivo.Replace("<", "");
                                  nombreArchivo = nombreArchivo.Replace(">", "");
                                  nombreArchivo = nombreArchivo.Replace("|", "");
                                  //Fin Codigo del programa para reemplazar caracteres no permitidos 
                                  //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                                  string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                                  imagen.SaveAs("C:\\Uploads\\" + archivo);

                                  Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, archivo, charge_within_the_system[0]);
                                  Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, archivo, charge_within_the_system[1]);
                                  mensaje = "Data inserted successfully";
                                  nivelMensaje = 2;
                              }
                      }
                      catch (Exception)
                      {
                          mensaje = "Error";
                          nivelMensaje = 3;
                      }          
                  }

            else
              if (charge_within_the_system.Length == 3)
               {
                   try
                   {
                       if (imagen == null)
                       {
                           string imagen2 = Convert.ToString(imagen);
                           Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, imagen2, charge_within_the_system[0]);
                           Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, imagen2, charge_within_the_system[1]);
                           Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, imagen2, charge_within_the_system[2]);
                           mensaje = "Data inserted successfully";
                           nivelMensaje = 2;
                       }
                       else
                           if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png")
                           {
                               mensaje = "Error debe insertar una imagen";
                               nivelMensaje = 4;
                           }
                           else
                           {
                               // Codigo del programa para reemplazar caracteres no permitidos 
                               //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                               string nombreArchivo = imagen.FileName.Replace("\\", "");
                               nombreArchivo = nombreArchivo.Replace("//", "");
                               nombreArchivo = nombreArchivo.Replace(":", "");
                               nombreArchivo = nombreArchivo.Replace("*", "");
                               nombreArchivo = nombreArchivo.Replace("?", "");
                               nombreArchivo = nombreArchivo.Replace("<", "");
                               nombreArchivo = nombreArchivo.Replace(">", "");
                               nombreArchivo = nombreArchivo.Replace("|", "");
                               //Fin Codigo del programa para reemplazar caracteres no permitidos 
                               //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                               string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                               imagen.SaveAs("C:\\Uploads\\" + archivo);

                               Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, archivo, charge_within_the_system[0]);
                               Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, archivo, charge_within_the_system[1]);
                               Entidad.SP_insertarUsuarios(Name, last_name, email, password, phone, Code_User, username, fecha_creacion, archivo, charge_within_the_system[2]);
                               mensaje = "Data inserted successfully";
                               nivelMensaje = 2;
                           }
                   }
                   catch (Exception)
                   {
                       mensaje = "Error";
                       nivelMensaje = 3;
                   }          
               }

            Profile perfil = new Profile();
            ViewData["perfiles"] = perfil.DevolverPerfiles();
            
            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "agregarUsuario",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

// Fin Codigo para agregar usuario tanto el metodo cuando se recibe una peticion Get de la pagina asi como el envio de datos por POST

//Codigo que listara todos los usuarios registrados en el sistema

        [HttpGet]
        public ActionResult ModificarUsuario(int nivelMensaje = 1, string mensaje = "")
        {
            int permisos =0;
            int idUsuario = 0;
            try
            {
                permisos = (int)Session["UserPermission"];
              
                if (permisos == 1)
                {
                    AllUser user = new AllUser();
                    ViewData["AllUser"] = user.ReturnAllUser();
                    ViewData["Decision"] = 1;
                }
                else
                {
                    idUsuario = (int)Session["UserId"];
                    UserById userbyid = new UserById();
                    ViewData["UserById"] = userbyid.ReturnUserById(idUsuario);
                    ViewData["Decision"] = 2;
                }
            }
            catch(Exception e)
            {

            }
            
          
            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;
            return View();
        }

//Fin Codigo que listara todos los usuarios registrados en el sistema
        
        [HttpGet]
        public ActionResult EditarUsuario(int UsuarioId)
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            Profile perfil = new Profile();
            UserById user = new UserById();

            ViewData["perfiles"] = perfil.DevolverPerfiles();
            ViewData["UserById"] = user.ReturnUserById(UsuarioId);
        
            return View();
        }

        [HttpPost]
        public ActionResult EditarUsuario(int UserId, string Name, string last_name, string email, string password, string phone, string Code_User, string username, HttpPostedFileBase imagen, int charge_within_the_system, string UploadOculto="")
        {         
            string mensaje = "";
            int nivelMensaje = 0;

            try
            {
              if (imagen == null)
              {            
                 Entidad.SP_EditUser(UserId, Name, last_name, email, password, phone, Code_User, username,UploadOculto, charge_within_the_system);
                 mensaje = "Data inserted successfully";
                 nivelMensaje = 2; 
              }
              else
                if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png")
                {
                   mensaje = "Error debe insertar una imagen";
                   nivelMensaje = 4;
                }
              else
              {
                  // Codigo del programa para reemplazar caracteres no permitidos 
                  //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                  string nombreArchivo = imagen.FileName.Replace("\\", "");
                  nombreArchivo = nombreArchivo.Replace("//", "");
                  nombreArchivo = nombreArchivo.Replace(":", "");
                  nombreArchivo = nombreArchivo.Replace("*", "");
                  nombreArchivo = nombreArchivo.Replace("?", "");
                  nombreArchivo = nombreArchivo.Replace("<", "");
                  nombreArchivo = nombreArchivo.Replace(">", "");
                  nombreArchivo = nombreArchivo.Replace("|", "");
                  //Fin Codigo del programa para reemplazar caracteres no permitidos 
                  //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                  string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                  imagen.SaveAs("C:\\Uploads\\" + archivo);
                  Entidad.SP_EditUser(UserId, Name, last_name, email, password, phone, Code_User, username, archivo, charge_within_the_system);
                  mensaje = "Data inserted successfully";
                  nivelMensaje = 2; 
              }                                       
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            AllUser user = new AllUser();
            ViewData["AllUser"] = user.ReturnAllUser();

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarUsuario",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult EliminarUsuario(int UsuarioId)
        {
            string mensaje = "";
            int nivelMensaje = 0;

            try
            {
                Entidad.SP_DeleteUser(UsuarioId);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;   
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }
          
            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarUsuario",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });  
        }

 //Codigo para agregar Proveedor tanto el metodo cuando se recibe una peticion Get de la pagina asi como el envio de datos por POST

        [HttpGet]
        public ActionResult agregarProveedor(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            Type_Provider typeProvider = new Type_Provider();
            ViewData["TypeProvider"] = typeProvider.ReturnTypeProvider();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;

            return View();
        }    

        [HttpPost]
        public ActionResult agregarProveedor(string Name_Provider, string email_Provider, int[] Provider, string Name_Contact, string Phone_Contact, string Description, string Description_Provider)
        {
            string mensaje = "";
            int nivelMensaje = 0;

            if (Provider == null)
            {
                nivelMensaje = 3;
            }
            
            else
             if(Provider.Length ==1)
             {
                 try
                 {
                    Entidad.SP_insertarProveedores(Name_Provider,email_Provider,Name_Contact,Phone_Contact,Description,Description_Provider,Provider[0] );
                    mensaje = "Data inserted successfully";
                    nivelMensaje = 2;
                 }
                 catch (Exception)
                 {
                   mensaje = "Error";
                   nivelMensaje = 3;
                 }
             }
            else
               if (Provider.Length == 2)
               {
                   try
                   {
                       Entidad.SP_insertarProveedores(Name_Provider, email_Provider, Name_Contact, Phone_Contact, Description, Description_Provider, Provider[0]);
                       Entidad.SP_insertarProveedores(Name_Provider, email_Provider, Name_Contact, Phone_Contact, Description, Description_Provider, Provider[1]);
                       mensaje = "Data inserted successfully";
                       nivelMensaje = 2;
                   }
                   catch (Exception)
                   {
                       mensaje = "Error";
                       nivelMensaje = 3;
                   }
               }
                 
            Type_Provider typeProvider = new Type_Provider();
            ViewData["TypeProvider"] = typeProvider.ReturnTypeProvider();

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "agregarProveedor",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });       
        }

 //Fin Codigo para agregar Proveedor tanto el metodo cuando se recibe una peticion Get de la pagina asi como el envio de datos por POST

        [HttpGet]
        public ActionResult ModificarProveedor(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            AllProvider provider = new AllProvider();
            ViewData["AllProvider"] = provider.ReturnAllProvider();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;

            return View();
        }

        [HttpGet]
        public ActionResult EditarProveedor(int ProveedorId)
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            Type_Provider typeProvider = new Type_Provider();
            ViewData["TypeProvider"] = typeProvider.ReturnTypeProvider();

            ProviderById provider = new ProviderById();
            ViewData["Providers"] = provider.ReturnProviderById(ProveedorId);
        
            return View();
        }

        [HttpPost]
        public ActionResult EditarProveedor(int ProviderId, string Name_Provider, string email_Provider, int Provider, string Name_Contact, string Phone_Contact, string Description, string Description_Provider)
        {
            string mensaje = "";
            int nivelMensaje = 0;

            
                  try
                  {
                      Entidad.SP_EditProvider(ProviderId, Name_Provider, email_Provider, Name_Contact, Phone_Contact, Description, Description_Provider, Provider);
                      mensaje = "Data inserted successfully";
                      nivelMensaje = 2;
                  }
                  catch (Exception)
                  {
                      mensaje = "Error";
                      nivelMensaje = 3;
                  }
              
            AllProvider provider = new AllProvider();
            ViewData["AllProvider"] = provider.ReturnAllProvider();

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarProveedor",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }


        [HttpGet]
        public ActionResult EliminarProveedor(int ProveedorId)
        {
            string mensaje = "";
            int nivelMensaje = 0;

            try
            {
                Entidad.SP_DeleteProvider(ProveedorId);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
               nivelMensaje = 3;
            }

            AllProvider provider = new AllProvider();
            ViewData["AllProvider"] = provider.ReturnAllProvider();

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarProveedor",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

 //Codigo para agregar Producto tanto el metodo cuando se recibe una peticion Get de la pagina asi como el envio de datos por POST

        [HttpGet]
        public ActionResult agregarNuevoProducto(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            Provider_Manufacturing  providerManufacturing= new Provider_Manufacturing();
            ViewData["providerManufacturing"] = providerManufacturing.ReturnProviderManufacturing();

            ProviderVendor providervendor = new ProviderVendor();
            ViewData["providervendor"] = providervendor.ReturnProviderVendor();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;

            return View();
        }

        [HttpPost]
        public ActionResult agregarNuevoProducto(string Name_Product, decimal Price, string Description, HttpPostedFileBase imagen, int Provider_Manufacturing, int Provider_Vendor)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                if (imagen == null)
                {
                    string imagen2 = Convert.ToString(imagen);
                    Entidad.SP_insertarProductos(Name_Product, Price, Description, imagen2, Provider_Manufacturing, Provider_Vendor);
                    mensaje = "Data inserted successfully";
                    nivelMensaje = 2;
                }

                else
                   if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png")
                   {                      
                       mensaje = "Error debe insertar una imagen";
                       nivelMensaje = 4;
                   }

               else
               {
                   // Codigo del programa para reemplazar caracteres no permitidos 
                   //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                   string nombreArchivo = imagen.FileName.Replace("\\", "");
                   nombreArchivo = nombreArchivo.Replace("//", "");
                   nombreArchivo = nombreArchivo.Replace(":", "");
                   nombreArchivo = nombreArchivo.Replace("*", "");
                   nombreArchivo = nombreArchivo.Replace("?", "");
                   nombreArchivo = nombreArchivo.Replace("<", "");
                   nombreArchivo = nombreArchivo.Replace(">", "");
                   nombreArchivo = nombreArchivo.Replace("|", "");
                   //Fin Codigo del programa para reemplazar caracteres no permitidos 
                   //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                   string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                   imagen.SaveAs("C:\\Uploads\\" + archivo);

                    Entidad.SP_insertarProductos(Name_Product, Price, Description, archivo, Provider_Manufacturing, Provider_Vendor);
                    mensaje = "Data inserted successfully";
                    nivelMensaje = 2;
               }
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            Provider_Manufacturing providerManufacturing = new Provider_Manufacturing();
            ViewData["providerManufacturing"] = providerManufacturing.ReturnProviderManufacturing();

            ProviderVendor providervendor = new ProviderVendor();
            ViewData["providervendor"] = providervendor.ReturnProviderVendor();

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "agregarNuevoProducto",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
           
        }
//Fin Codigo para agregar Producto tanto el metodo cuando se recibe una peticion Get de la pagina asi como el envio de datos por POST

        [HttpGet]
        public ActionResult ModificarProducto(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            AllProduct product = new AllProduct();
            ViewData["AllProducts"] = product.ReturnAllProduct();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;

            return View();
        }

        [HttpGet]
        public ActionResult EditarProducto(int ProductoId)
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            Provider_Manufacturing providerManufacturing = new Provider_Manufacturing();
            ViewData["providerManufacturing"] = providerManufacturing.ReturnProviderManufacturing();

            ProviderVendor providervendor = new ProviderVendor();
            ViewData["providervendor"] = providervendor.ReturnProviderVendor();

            ProductById product = new ProductById();
            ViewData["ProductById"] = product.ReturnProduct(ProductoId);

            return View();
        }

        [HttpPost]
        public ActionResult EditarProducto(int ProductId, string Name_Product, decimal Price, string Description, HttpPostedFileBase imagen, int Provider_Manufacturing, int Provider_Vendor,string UploadOculto="")
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {                              
                if (imagen == null)
                {                   
                    Entidad.SP_EditProduct(ProductId, Name_Product, Price, Description,UploadOculto, Provider_Manufacturing, Provider_Vendor);
                    mensaje = "Data inserted successfully";
                    nivelMensaje = 2;
                }

                else
                    if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png")
                    {
                        mensaje = "Error debe insertar una imagen";
                        nivelMensaje = 4;
                    }

                    else
                    {
                        // Codigo del programa para reemplazar caracteres no permitidos 
                        //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                        string nombreArchivo = imagen.FileName.Replace("\\", "");
                        nombreArchivo = nombreArchivo.Replace("//", "");
                        nombreArchivo = nombreArchivo.Replace(":", "");
                        nombreArchivo = nombreArchivo.Replace("*", "");
                        nombreArchivo = nombreArchivo.Replace("?", "");
                        nombreArchivo = nombreArchivo.Replace("<", "");
                        nombreArchivo = nombreArchivo.Replace(">", "");
                        nombreArchivo = nombreArchivo.Replace("|", "");
                        //Fin Codigo del programa para reemplazar caracteres no permitidos 
                        //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                        string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                        imagen.SaveAs("C:\\Uploads\\" + archivo);

                        Entidad.SP_EditProduct(ProductId, Name_Product, Price, Description,archivo, Provider_Manufacturing, Provider_Vendor);
                        mensaje = "Data inserted successfully";
                        nivelMensaje = 2;
                    }
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            AllProduct product = new AllProduct();
            ViewData["AllProducts"] = product.ReturnAllProduct();

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarProducto",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });

        }

        [HttpGet]
        public ActionResult EliminarProducto(int ProductoId)
        {
            string mensaje = "";
            int nivelMensaje = 0;

            try
            {
                Entidad.SP_DeleteProduct(ProductoId);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            AllProduct product = new AllProduct();
            ViewData["AllProducts"] = product.ReturnAllProduct();

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarProducto",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }


        [HttpGet]
        public ActionResult agregarNuevoTipoContrato(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;
            return View();
        }

        [HttpPost]
        public ActionResult agregarNuevoTipoContrato(string Type_Contract, string Description)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                Entidad.SP_insertarTipoContratos(Type_Contract,Description);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }
            
            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "agregarNuevoTipoContrato",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });         
        }

        [HttpGet]
        public ActionResult ModificarTipoContrato(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";
            
            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            AllTypeContract typeContract = new AllTypeContract();
            ViewData["AllTypeContract"] = typeContract.ReturnAllTypeContract();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;
            
            return View();
        }

        [HttpGet]
        public ActionResult EditarTipoContrato(int TipoContratoId)
        {   
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
           
            TypeContractById typecontract = new TypeContractById();
            ViewData["typecontractid"] = typecontract.ReturnTypeContract(TipoContratoId);
               
            return View();
        }

        [HttpPost]
        public ActionResult EditarTipoContrato(int TypeContractId, string Type_Contract, string Description)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                Entidad.SP_EditTipoContratos(TypeContractId, Type_Contract, Description);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarTipoContrato",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });     
        }

        [HttpGet]
        public ActionResult EliminarTipoContrato(int TipoContratoId)
        {
            string mensaje = "";
            int nivelMensaje = 0;

            try
            {
                Entidad.SP_DeleteTipoContratos(TipoContratoId);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            AllTypeContract typeContract = new AllTypeContract();
            ViewData["AllTypeContract"] = typeContract.ReturnAllTypeContract();

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarTipoContrato",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult agregarNuevoMetodoPago(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;
            return View();
        }

        [HttpPost]
        public ActionResult agregarNuevoMetodoPago(string Method_Pay)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                Entidad.SP_insertarMetodoPagos(Method_Pay);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "agregarNuevoMetodoPago",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult ModificarMetodoPago(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            AllMethodPayment methodPayment = new AllMethodPayment();
            ViewData["AllMethodPayment"] = methodPayment.ReturnAllMethodPayment();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;
       
            return View();
        }

        [HttpGet]
        public ActionResult EditarMetodoPago(int MetodoPagoId)
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
                MethodPaymentById metodoPago = new MethodPaymentById();
                ViewData["metodoPago"] = metodoPago.ReturnMethodPayment(MetodoPagoId); 
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditarMetodoPago(int Method_PayId, string Method_Pay)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                Entidad.SP_EditMethodPaymentById(Method_PayId,Method_Pay);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarMetodoPago",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult EliminarMetodoPago(int metodoPagoId)
        {
            string UserKey = "";
            string mensaje = "";
            int nivelMensaje = 0;

            try
            {
                UserKey = Session["UserKey"].ToString();
                Entidad.SP_DeleteMethodPaymentById(metodoPagoId);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarMetodoPago",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult agregarNuevaAreaContrato(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;
            return View();
        }

        [HttpPost]
        public ActionResult agregarNuevaAreaContrato(string Area_Name)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                Entidad.SP_insertarAreaContratos(Area_Name);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "agregarNuevaAreaContrato",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult ModificarAreaContrato(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            AllContractArea contractArea = new AllContractArea();
            ViewData["ContractArea"] = contractArea.ReturnAllContractArea();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;
            return View();
        }

        [HttpGet]
        public ActionResult EditarAreaContrato(int AreaContratoId)
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
                ContractAreaById contractArea = new ContractAreaById();
                ViewData["ContractArea"] = contractArea.ReturnContractArea(AreaContratoId);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult EditarAreaContrato(int AreaContratoId,string Area_Name)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                Entidad.SP_EditAreaContratos(AreaContratoId, Area_Name);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarAreaContrato",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult EliminarAreaContrato(int AreaContratoId)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                Entidad.SP_DeleteAreaContratos(AreaContratoId);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarAreaContrato",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult agregarNuevaTarjetas(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            Type_Cards typecards = new Type_Cards();
            ViewData["TypeCards"] = typecards.ReturnTypeCards();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;
            return View();
        }

        [HttpPost]
        public ActionResult agregarNuevaTarjetas(string Last_Digits_Card, int Type_Card, string Name_Person_Assigned_To_Card, DateTime End_Date_Card)
        {
            string mensaje = "";
            int nivelMensaje = 0;

            try
            {
                Entidad.SP_insertarTarjetas(Last_Digits_Card, End_Date_Card, Name_Person_Assigned_To_Card, Type_Card);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;  
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;  
            }

            Type_Cards typecards = new Type_Cards();
            ViewData["TypeCards"] = typecards.ReturnTypeCards();

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "agregarNuevaTarjetas",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
           
        }

        [HttpGet]
        public ActionResult ModificarTarjetas(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            AllCards cards = new AllCards();
            ViewData["AllCards"] = cards.ReturnAllCards();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;

            return View();
        }

        [HttpGet]
        public ActionResult EditarTarjetas(int TarjetaId)
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
                CardsById cards = new CardsById();
                ViewData["CardsById"] = cards.ReturnCards(TarjetaId);

                Type_Cards typecards = new Type_Cards();
                ViewData["TypeCards"] = typecards.ReturnTypeCards();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult EditarTarjetas(int TarjetaId,string Last_Digits_Card, int Type_Card, string Name_Person_Assigned_To_Card, DateTime End_Date_Card)
        {
            string mensaje = "";
            int nivelMensaje = 0;

            try
            {
                Entidad.SP_EditTarjetas (TarjetaId,Last_Digits_Card, End_Date_Card, Name_Person_Assigned_To_Card, Type_Card);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            Type_Cards typecards = new Type_Cards();
            ViewData["TypeCards"] = typecards.ReturnTypeCards();

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarTarjetas",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });

        }

        [HttpGet]
        public ActionResult EliminarTarjetas(int TarjetaId)
        {         
            string mensaje = "";
            int nivelMensaje = 0;

            try
            {
                Entidad.SP_DeleteTarjetas(TarjetaId);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarTarjetas",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult agregarNuevaUnidadNegocio(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;

            return View();
        }

        [HttpPost]
        public ActionResult agregarNuevaUnidadNegocio(string Business_Unit_Number, string Business_Unit_Name, string Previous_Business_Unit_Name)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                Entidad.SP_insertarUnidadNegocios(Business_Unit_Number,Business_Unit_Name,Previous_Business_Unit_Name);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;  
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;  
            }
            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "agregarNuevaUnidadNegocio",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult ModificarUnidadNegocio(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
                AllBusinessUnit businessUnit = new AllBusinessUnit();
                ViewData["businessUnit"] = businessUnit.ReturnAllBusinessUnit();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }       
            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;

            return View();
        }

        [HttpGet]
        public ActionResult EditarUnidadNegocio(int UnidadNegocioId)
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
                BusinessUnitById businessUnit = new BusinessUnitById();
                ViewData["businessunit"] = businessUnit.ReturnBusinessUnit(UnidadNegocioId);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

           
            return View();
        }

        [HttpPost]
        public ActionResult EditarUnidadNegocio(int UnidadNegocioId, string Business_Unit_Number, string Business_Unit_Name, string Previous_Business_Unit_Name)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                Entidad.SP_EditUnidadNegociosById(UnidadNegocioId,Business_Unit_Number, Business_Unit_Name, Previous_Business_Unit_Name);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }
            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarUnidadNegocio",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult EliminarUnidadNegocio(int UnidadNegocioId)
        {
            string mensaje = "";
            int nivelMensaje = 0;

            try
            {
                Entidad.SP_DeleteUnidadNegocios(UnidadNegocioId);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }
            catch (Exception)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }
            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarUnidadNegocio",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult agregarNuevaOrdenCompra(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;

            return View();
        }

        [HttpPost]
        public ActionResult agregarNuevaOrdenCompra(string Budget_Code, string Previous_Value, DateTime Last_Purchase_Order_Date, string Last_Invoice_Number,HttpPostedFileBase imagen)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                if (imagen == null)
                {
                    string imagen2 = Convert.ToString(imagen);
                    Entidad.SP_insertarOrdenCompras(Last_Purchase_Order_Date, Last_Invoice_Number, Budget_Code, Previous_Value, imagen2);
                    mensaje = "Data inserted successfully";
                    nivelMensaje = 2;     
                }

                else
                    if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png" && imagen.ContentType != "application/msword" && imagen.ContentType != "application/pdf"
                       && imagen.ContentType != "application/vnd.ms-excel" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" &&
                       imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.template" && imagen.ContentType != "application/vnd.ms-word.document.macroEnabled.12" &&
                       imagen.ContentType != "application/vnd.ms-word.template.macroEnabled.12" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
                       imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.template" && imagen.ContentType != "application/vnd.ms-excel.sheet.macroEnabled.12" &&
                       imagen.ContentType != "application/vnd.ms-excel.template.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.addin.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.sheet.binary.macroEnabled.12"
                       && imagen.ContentType != "multipart/x-zip" && imagen.ContentType != "application/x-tar" && imagen.ContentType != "application/zip" && imagen.ContentType != "application/x-rar")
                   {
                       mensaje = "Data inserted successfully";
                       nivelMensaje = 4;     
                   }

                else
                {
                    // Codigo del programa para reemplazar caracteres no permitidos 
                    //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                    string nombreArchivo = imagen.FileName.Replace("\\", "");
                    nombreArchivo = nombreArchivo.Replace("//", "");
                    nombreArchivo = nombreArchivo.Replace(":", "");
                    nombreArchivo = nombreArchivo.Replace("*", "");
                    nombreArchivo = nombreArchivo.Replace("?", "");
                    nombreArchivo = nombreArchivo.Replace("<", "");
                    nombreArchivo = nombreArchivo.Replace(">", "");
                    nombreArchivo = nombreArchivo.Replace("|", "");
                    //Fin Codigo del programa para reemplazar caracteres no permitidos 
                    //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                    string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                    imagen.SaveAs("C:\\Uploads\\" + archivo);

                    Entidad.SP_insertarOrdenCompras(Last_Purchase_Order_Date, Last_Invoice_Number, Budget_Code, Previous_Value, archivo);
                    mensaje = "Data inserted successfully";
                    nivelMensaje = 2;     
                }

            }
            catch (Exception)
            {
                mensaje = "Data inserted successfully";
                nivelMensaje = 3;     
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "agregarNuevaOrdenCompra",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult ModificarOrdenCompra(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
                AllPurchaseOrder purchaseOrder = new AllPurchaseOrder();
                ViewData["AllPurchaseOrder"] = purchaseOrder.ReturnAllPurchaseOrder();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
                
            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;

            return View();
        }

        [HttpGet]
        public ActionResult EditarOrdenCompra(int OrdenCompraId)
        {
            string UserKey = "";

            try
            {
                UserKey = Session["UserKey"].ToString();
                PurchaseOrderById purchaseOrder = new PurchaseOrderById();
                ViewData["purchaseOrder"] = purchaseOrder.ReturnPurchaseOrder(OrdenCompraId);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult EditarOrdenCompra(int OrdenCompraId, string Budget_Code, string Previous_Value, DateTime Last_Purchase_Order_Date, string Last_Invoice_Number, HttpPostedFileBase imagen, string UploadOculto="")
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
               if (imagen == null)
               {                   
                   Entidad.SP_EditOrdenCompras(OrdenCompraId,Last_Purchase_Order_Date,Last_Invoice_Number,Budget_Code,Previous_Value,UploadOculto);
                   mensaje = "Data inserted successfully";
                   nivelMensaje = 2;
               }

               else
                   if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png" && imagen.ContentType != "application/msword" && imagen.ContentType != "application/pdf"
                      && imagen.ContentType != "application/vnd.ms-excel" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" &&
                      imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.template" && imagen.ContentType != "application/vnd.ms-word.document.macroEnabled.12" &&
                      imagen.ContentType != "application/vnd.ms-word.template.macroEnabled.12" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
                      imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.template" && imagen.ContentType != "application/vnd.ms-excel.sheet.macroEnabled.12" &&
                      imagen.ContentType != "application/vnd.ms-excel.template.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.addin.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.sheet.binary.macroEnabled.12"
                      && imagen.ContentType != "multipart/x-zip" && imagen.ContentType != "application/x-tar" && imagen.ContentType != "application/zip" && imagen.ContentType != "application/x-rar")
                   {
                       mensaje = "Data inserted successfully";
                       nivelMensaje = 4;
                   }

                   else
                   {
                       // Codigo del programa para reemplazar caracteres no permitidos 
                       //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                       string nombreArchivo = imagen.FileName.Replace("\\", "");
                       nombreArchivo = nombreArchivo.Replace("//", "");
                       nombreArchivo = nombreArchivo.Replace(":", "");
                       nombreArchivo = nombreArchivo.Replace("*", "");
                       nombreArchivo = nombreArchivo.Replace("?", "");
                       nombreArchivo = nombreArchivo.Replace("<", "");
                       nombreArchivo = nombreArchivo.Replace(">", "");
                       nombreArchivo = nombreArchivo.Replace("|", "");
                       //Fin Codigo del programa para reemplazar caracteres no permitidos 
                       //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                       string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                       imagen.SaveAs("C:\\Uploads\\" + archivo);

                       Entidad.SP_EditOrdenCompras(OrdenCompraId, Last_Purchase_Order_Date, Last_Invoice_Number, Budget_Code, Previous_Value, archivo);
                       mensaje = "Data inserted successfully";
                       nivelMensaje = 2;
                   }
            }
              
            catch (Exception)
            {
                mensaje = "Data inserted successfully";
                nivelMensaje = 3;
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarOrdenCompra",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public ActionResult EliminarOrdenCompra(int OrdenCompraId)
        {
            string mensaje = "";
            int nivelMensaje = 0;
            try
            {
                Entidad.SP_DeleteOrdenCompras(OrdenCompraId);
                mensaje = "Data inserted successfully";
                nivelMensaje = 2;
            }

            catch (Exception)
            {
                mensaje = "Data inserted successfully";
                nivelMensaje = 3;
            }

            return RedirectToRoute(new
            {
                controller = "BackEnd",
                action = "ModificarOrdenCompra",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
        }

        [HttpGet]
        public string selectProductAccordingSuppliers(int ProviderManufacturing, int ProviderVendor)
        {
            List<SP_selectProductsAccordingSuppliers_Result> lista = Entidad.SP_selectProductsAccordingSuppliers(ProviderManufacturing, ProviderVendor).ToList();
            string json = "";
            string json2 = "";

            foreach (SP_selectProductsAccordingSuppliers_Result list in lista)
            {
                json += list.ProductoId + ",";
                json2 += list.Nombre_Producto + "/";
            }
            string json3 = json + "|" + json2;
            return json3;
        }

        [HttpGet]
        public ActionResult AgregarNuevoContrato(int nivelMensaje=1, string mensaje="")
        {
            string UserKey = "";
            int permisos = 0;

            try
            {
                UserKey = Session["UserKey"].ToString();
                permisos = (int)Session["UserPermission"];

                if (permisos == 5)
                {
                    return RedirectToRoute(new
                    {
                        controller = "Main",
                        action = "main",
                        NivelMensaje = 3,
                        Mensaje = ""
                    });
                }

                ViewData["permisos"] = permisos;
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            TypeContract typecontract = new TypeContract();
            ViewData["typecontract"] = typecontract.ReturnTypeContract();

            StateContract statecontract = new StateContract();
            ViewData["statecontract"] = statecontract.ReturnStateContract();

            ApproverContract approver = new ApproverContract();
            ViewData["approver"] = approver.ReturnApproverContract();

            AdministratorContract administrator = new AdministratorContract();
            ViewData["administrator"] = administrator.ReturnAdministratorContract();

            Provider_Manufacturing providerManufacturing = new Provider_Manufacturing();
            ViewData["providerManufacturing"] = providerManufacturing.ReturnProviderManufacturing();

            ProviderVendor providervendor = new ProviderVendor();
            ViewData["providervendor"] = providervendor.ReturnProviderVendor();

            PurchaseOrder purchaseorder = new PurchaseOrder();
            ViewData["purchaseorder"] = purchaseorder.ReturnPurchaseOrder();

            MethodPayment methodPayment = new MethodPayment();
            ViewData["methodPayment"] = methodPayment.ReturnMethodPayment();

            Cards cards = new Cards();
            ViewData["cards"] = cards.ReturnCards();

            TypePayment typepayment = new TypePayment();
            ViewData["typepayment"] = typepayment.ReturnTypePayment();

            AllBusinessUnit businessUnit = new AllBusinessUnit();
            ViewData["businessUnit"] = businessUnit.ReturnAllBusinessUnit();

            ContractArea contractarea  = new ContractArea();
            ViewData["contractarea"] = contractarea.ReturnContractArea();

            WeatherAlerts weatherAlerts = new WeatherAlerts();
            ViewData["weatherAlerts"] = weatherAlerts.ReturnWeatherAlerts();

            AllUser user = new AllUser();
            ViewData["AllUser"] = user.ReturnAllUser();

            ViewData["Mensaje"] = mensaje;
            ViewData["NivelMensaje"] = nivelMensaje;

            return View();
        }

        [HttpPost]
        public ActionResult AgregarNuevoContrato(int Draft2,string expenses, int? Type_Of_Contract, int? State, string[] Product2,
             int? Method_Pay, decimal? Cost, int? Type_Pay, decimal? Cost_Per_Payment, DateTime? Date_Renewal, int? Area_Contract, DateTime? Date_Start,
             DateTime? Date_End, string Description, int? Notify_Before, DateTime? Date_Warranty_End, HttpPostedFileBase imagen, int Bussines_Unit_Name = 0,
             int Provider_Manufacturing = 0, int Provider_Vendor = 0, int Approver = 1, int administrator = 0, int Quantity = 0, string Warranty = "", string UploadOculto="",
             int Card_Type = 0, int Purchase_Order_Code = 0, string Number_Of_Contract = "N/A", string Number_Service_Contract = "N/A", int Product = 0, string Contract_Name="", string direccion="")
        {
            string mensaje = "";
            int nivelMensaje = 0;
            SendMail enviarMail = new SendMail();

            if (Date_Start == null)
            {
                Date_Start = Convert.ToDateTime("01/01/1901");
            }

            if (Date_End == null)
            {
                Date_End = Convert.ToDateTime("01/01/1901");
            }

            if (Date_Renewal == null)
            {
                Date_Renewal = Convert.ToDateTime("01/01/1901");
            }

            if (Date_Warranty_End == null)
            {
                Date_Warranty_End = Convert.ToDateTime("01/01/1901");
            }

            if (Draft2 == 1)
            {
               DateTime fecha_creacion = DateTime.Now;
               string productos = "";

               if (Product2 == null)
               {
                  if (imagen == null)
                  {                  
                     Entidad.SP_insertarBorradorContratos(Contract_Name, Number_Of_Contract, Number_Service_Contract,expenses ,Date_Start, Date_End, Cost, Cost_Per_Payment, Date_Renewal, Quantity, Warranty, Date_Warranty_End,
                     fecha_creacion, Description, UploadOculto, Provider_Manufacturing, Provider_Vendor, Approver, administrator, Type_Of_Contract, Method_Pay, Bussines_Unit_Name, Type_Pay, Purchase_Order_Code, Card_Type, Area_Contract, Product
                     ,Notify_Before, State, productos);

                      mensaje = "Data inserted successfully";
                      nivelMensaje = 5;
                  }
                  else
                    if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png" && imagen.ContentType != "application/msword" && imagen.ContentType != "application/pdf"
                        && imagen.ContentType != "application/vnd.ms-excel" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" &&
                        imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.template" && imagen.ContentType != "application/vnd.ms-word.document.macroEnabled.12" &&
                        imagen.ContentType != "application/vnd.ms-word.template.macroEnabled.12" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
                        imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.template" && imagen.ContentType != "application/vnd.ms-excel.sheet.macroEnabled.12" &&
                        imagen.ContentType != "application/vnd.ms-excel.template.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.addin.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.sheet.binary.macroEnabled.12"
                        && imagen.ContentType != "multipart/x-zip" && imagen.ContentType != "application/x-tar" && imagen.ContentType != "application/zip" && imagen.ContentType != "application/x-rar")
                        {
                           mensaje = "Extension de archivo no permitida";
                           nivelMensaje = 4;                          
                        }

                  else
                  {
                      // Codigo del programa para reemplazar caracteres no permitidos 
                      //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                      string nombreArchivo = imagen.FileName.Replace("\\", "");
                      nombreArchivo = nombreArchivo.Replace("//", "");
                      nombreArchivo = nombreArchivo.Replace(":", "");
                      nombreArchivo = nombreArchivo.Replace("*", "");
                      nombreArchivo = nombreArchivo.Replace("?", "");
                      nombreArchivo = nombreArchivo.Replace("<", "");
                      nombreArchivo = nombreArchivo.Replace(">", "");
                      nombreArchivo = nombreArchivo.Replace("|", "");
                      //Fin Codigo del programa para reemplazar caracteres no permitidos 
                      //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                        string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                        imagen.SaveAs("C:\\Uploads\\" + archivo);

                        Entidad.SP_insertarBorradorContratos(Contract_Name, Number_Of_Contract, Number_Service_Contract,expenses ,Date_Start, Date_End, Cost, Cost_Per_Payment, Date_Renewal, Quantity, Warranty, Date_Warranty_End,
                        fecha_creacion, Description, archivo, Provider_Manufacturing, Provider_Vendor, Approver, administrator, Type_Of_Contract, Method_Pay, Bussines_Unit_Name, Type_Pay, Purchase_Order_Code, Card_Type, Area_Contract, Product
                        ,Notify_Before, State, productos);
                        mensaje = "Data inserted successfully";
                        nivelMensaje = 5;
                  }
                    //}
                    //catch (Exception)
                    // {
                    // mensaje = "Error";
                    // nivelMensaje = 3;              
                    //}
                }
                
                else
                {
                    for (int i = 0; i < Product2.Length; i++)
                    {
                        productos += Product2[i] + "¡";
                    }              

                    // try
                    //{
                    if (imagen == null)
                    {                      
                        Entidad.SP_insertarBorradorContratos(Contract_Name, Number_Of_Contract, Number_Service_Contract,expenses, Date_Start, Date_End, Cost, Cost_Per_Payment, Date_Renewal, Quantity, Warranty, Date_Warranty_End,
                            fecha_creacion, Description, UploadOculto, Provider_Manufacturing, Provider_Vendor, Approver, administrator, Type_Of_Contract, Method_Pay, Bussines_Unit_Name, Type_Pay, Purchase_Order_Code, Card_Type, Area_Contract, Product
                            , Notify_Before, State, productos);

                        mensaje = "Data inserted successfully";
                        nivelMensaje = 5;
                    }

                    else
                        if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png" && imagen.ContentType != "application/msword" && imagen.ContentType != "application/pdf"
                            && imagen.ContentType != "application/vnd.ms-excel" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" &&
                            imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.template" && imagen.ContentType != "application/vnd.ms-word.document.macroEnabled.12" &&
                            imagen.ContentType != "application/vnd.ms-word.template.macroEnabled.12" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
                            imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.template" && imagen.ContentType != "application/vnd.ms-excel.sheet.macroEnabled.12" &&
                            imagen.ContentType != "application/vnd.ms-excel.template.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.addin.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.sheet.binary.macroEnabled.12"
                            && imagen.ContentType != "multipart/x-zip" && imagen.ContentType != "application/x-tar" && imagen.ContentType != "application/zip" && imagen.ContentType != "application/x-rar")
                        {
                            mensaje = "Extension de archivo no permitida";
                            nivelMensaje = 4;
                        }

                        else
                        {
                            // Codigo del programa para reemplazar caracteres no permitidos 
                            //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                            string nombreArchivo = imagen.FileName.Replace("\\", "");
                            nombreArchivo = nombreArchivo.Replace("//", "");
                            nombreArchivo = nombreArchivo.Replace(":", "");
                            nombreArchivo = nombreArchivo.Replace("*", "");
                            nombreArchivo = nombreArchivo.Replace("?", "");
                            nombreArchivo = nombreArchivo.Replace("<", "");
                            nombreArchivo = nombreArchivo.Replace(">", "");
                            nombreArchivo = nombreArchivo.Replace("|", "");
                            //Fin Codigo del programa para reemplazar caracteres no permitidos 
                            //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                            string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                            imagen.SaveAs("C:\\Uploads\\" + archivo);

                            Entidad.SP_insertarBorradorContratos(Contract_Name, Number_Of_Contract, Number_Service_Contract,expenses ,Date_Start, Date_End, Cost, Cost_Per_Payment, Date_Renewal, Quantity, Warranty, Date_Warranty_End,
                               fecha_creacion, Description, archivo, Provider_Manufacturing, Provider_Vendor, Approver, administrator, Type_Of_Contract, Method_Pay, Bussines_Unit_Name, Type_Pay, Purchase_Order_Code, Card_Type, Area_Contract, Product
                               , Notify_Before, State, productos);
                            mensaje = "Data inserted successfully";
                            nivelMensaje = 5;
                        }
                    //}
                    //catch (Exception)
                    // {
                    // mensaje = "Error";
                    // nivelMensaje = 3;              
                    //}
                }
            }

            else
            {
                DateTime fecha_creacion = DateTime.Now;
                
                string productos = "";

                if (Product2 == null)
                {
                    mensaje = "Error";
                    nivelMensaje = 3;
                }
                else
                {
                    for (int i = 0; i < Product2.Length; i++)
                    {
                        productos += Product2[i] + "¡";
                    }

                    // try
                    //{
                    if (imagen == null)
                    {                       
                        Entidad.SP_insertarContratos(Contract_Name, Number_Of_Contract, Number_Service_Contract,expenses ,Date_Start, Date_End, Cost, Cost_Per_Payment, Date_Renewal, Quantity, Warranty, Date_Warranty_End,
                            fecha_creacion, Description, UploadOculto, Provider_Manufacturing, Provider_Vendor, Approver, administrator, Type_Of_Contract, Method_Pay, Bussines_Unit_Name, Type_Pay, Purchase_Order_Code, Card_Type, Area_Contract, Product
                            , Notify_Before, State, productos);

                        mensaje = "Data inserted successfully";
                        nivelMensaje = 2;
                        enviarMail.SendMailWhenContractIsUpload(Contract_Name, Number_Of_Contract, Approver, administrator,Date_End);

                    }

                    else
                        if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png" && imagen.ContentType != "application/msword" && imagen.ContentType != "application/pdf"
                            && imagen.ContentType != "application/vnd.ms-excel" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" &&
                            imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.template" && imagen.ContentType != "application/vnd.ms-word.document.macroEnabled.12" &&
                            imagen.ContentType != "application/vnd.ms-word.template.macroEnabled.12" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
                            imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.template" && imagen.ContentType != "application/vnd.ms-excel.sheet.macroEnabled.12" &&
                            imagen.ContentType != "application/vnd.ms-excel.template.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.addin.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.sheet.binary.macroEnabled.12"
                            && imagen.ContentType != "multipart/x-zip" && imagen.ContentType != "application/x-tar" && imagen.ContentType != "application/zip" && imagen.ContentType != "application/x-rar")
                        {
                            mensaje = "Extension de archivo no permitida";
                            nivelMensaje = 4;
                        }

                        else
                        {
                            // Codigo del programa para reemplazar caracteres no permitidos 
                            //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                            string nombreArchivo = imagen.FileName.Replace("\\", "");
                            nombreArchivo = nombreArchivo.Replace("//", "");
                            nombreArchivo = nombreArchivo.Replace(":", "");
                            nombreArchivo = nombreArchivo.Replace("*", "");
                            nombreArchivo = nombreArchivo.Replace("?", "");
                            nombreArchivo = nombreArchivo.Replace("<", "");
                            nombreArchivo = nombreArchivo.Replace(">", "");
                            nombreArchivo = nombreArchivo.Replace("|", "");
                            //Fin Codigo del programa para reemplazar caracteres no permitidos 
                            //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                            string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                            imagen.SaveAs("C:\\Uploads\\" + archivo);

                            Entidad.SP_insertarContratos(Contract_Name, Number_Of_Contract, Number_Service_Contract,expenses ,Date_Start, Date_End, Cost, Cost_Per_Payment, Date_Renewal, Quantity, Warranty, Date_Warranty_End,
                               fecha_creacion, Description, archivo, Provider_Manufacturing, Provider_Vendor, Approver, administrator, Type_Of_Contract, Method_Pay, Bussines_Unit_Name, Type_Pay, Purchase_Order_Code, Card_Type, Area_Contract, Product
                               , Notify_Before, State, productos);
                            mensaje = "Data inserted successfully";
                            nivelMensaje = 2;
                            enviarMail.SendMailWhenContractIsUpload(Contract_Name, Number_Of_Contract, Approver, administrator,Date_End);

                        }
                    //}
                    //catch (Exception)
                    // {
                    // mensaje = "Error";
                    // nivelMensaje = 3;              
                    //}
                }
            }
      
            TypeContract typecontract = new TypeContract();
            ViewData["typecontract"] = typecontract.ReturnTypeContract();

            StateContract statecontract = new StateContract();
            ViewData["statecontract"] = statecontract.ReturnStateContract();

            ApproverContract approver = new ApproverContract();
            ViewData["approver"] = approver.ReturnApproverContract();

            AdministratorContract administratorcontract = new AdministratorContract();
            ViewData["administrator"] = administratorcontract.ReturnAdministratorContract();

            Provider_Manufacturing providerManufacturing = new Provider_Manufacturing();
            ViewData["providerManufacturing"] = providerManufacturing.ReturnProviderManufacturing();

            ProviderVendor providervendor = new ProviderVendor();
            ViewData["providervendor"] = providervendor.ReturnProviderVendor();

            PurchaseOrder purchaseorder = new PurchaseOrder();
            ViewData["purchaseorder"] = purchaseorder.ReturnPurchaseOrder();

            MethodPayment methodPayment = new MethodPayment();
            ViewData["methodPayment"] = methodPayment.ReturnMethodPayment();

            Cards cards = new Cards();
            ViewData["cards"] = cards.ReturnCards();

            TypePayment typepayment = new TypePayment();
            ViewData["typepayment"] = typepayment.ReturnTypePayment();

            AllBusinessUnit businessUnit = new AllBusinessUnit();
            ViewData["businessUnit"] = businessUnit.ReturnAllBusinessUnit();

            ContractArea contractarea = new ContractArea();
            ViewData["contractarea"] = contractarea.ReturnContractArea();

            WeatherAlerts weatherAlerts = new WeatherAlerts();
            ViewData["weatherAlerts"] = weatherAlerts.ReturnWeatherAlerts();

            AllUser user = new AllUser();
            ViewData["AllUser"] = user.ReturnAllUser();


            if (direccion.Length != 0)
            {
                return Redirect(direccion);
            }

            else
            {
                return RedirectToRoute(new
                {
                    controller = "BackEnd",
                    action = "AgregarNuevoContrato",
                    NivelMensaje = nivelMensaje,
                    Mensaje = mensaje
                });
            }        
        }

        [HttpGet]
        public ActionResult ModificarContrato(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";
            int permisos = 0;

            try
            {
                UserKey = Session["UserKey"].ToString();
                permisos = (int)Session["UserPermission"];
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            if (permisos == 1)
            {
                Contracts contratos = new Contracts();
                ViewData["Decision"] = 1;
                ViewData["Contratos"] = contratos.ReturnContracts();
            }

            else
                if (permisos == 2)
                {
                    Contracts contratos = new Contracts();
                    ViewData["Decision"] = 2;
                    ViewData["Contratos2"] = contratos.ReturnContractsByArea(2);
                }

            else
                if (permisos == 3)
                {
                    Contracts contratos = new Contracts();
                    ViewData["Decision"] = 2;
                    ViewData["Contratos2"] = contratos.ReturnContractsByArea(1);
                }

                else
                    if (permisos == 4)
                    {
                        Contracts contratos = new Contracts();
                        ViewData["Decision"] = 2;
                        ViewData["Contratos2"] = contratos.ReturnContractsByArea(3);
                    }
          
            return View();
        }

        [HttpGet]
        public ActionResult ModificarBorrador(int nivelMensaje = 1, string mensaje = "")
        {
            string UserKey = "";
            int permisos = 0;

            try
            {
                UserKey = Session["UserKey"].ToString();
                permisos = (int)Session["UserPermission"];
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            if (permisos == 1)
            {
                Contracts contratos = new Contracts();
                ViewData["Decision"] = 1;
                ViewData["Contratos"] = contratos.ReturnDraftContracts();
                return View();
            }

            else
                if (permisos == 2 || permisos == 6)
                {
                    Contracts contratos = new Contracts();
                    ViewData["Decision"] = 2;
                    ViewData["Contratos2"] = contratos.ReturnDraftContractsByArea(2);
                    return View();
                }

                else
                    if (permisos == 3 || permisos == 7)
                    {
                        Contracts contratos = new Contracts();
                        ViewData["Decision"] = 2;
                        ViewData["Contratos2"] = contratos.ReturnDraftContractsByArea(1);
                        return View();
                    }

                    else
                        if (permisos == 4 || permisos == 8)
                        {
                            Contracts contratos = new Contracts();
                            ViewData["Decision"] = 2;
                            ViewData["Contratos2"] = contratos.ReturnDraftContractsByArea(3);
                            return View();
                        }
                        else
                        {
                            return RedirectToRoute(new
                            {
                                controller = "Main",
                                action = "main",                                
                            });
                        }
           
        }

        [HttpGet]
        public ActionResult EditarBorradorContrato(int ContractId = 0)
        {
            string UserKey = "";
            int permisos = 0;

            try
            {
                UserKey = Session["UserKey"].ToString();
                permisos = (int)Session["UserPermission"];
                ViewData["permisos"] = permisos;
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            if (permisos == 1 || permisos == 2 || permisos == 3 || permisos == 4 || permisos == 6 || permisos == 7 || permisos == 8)
            {
                TypeContract typecontract = new TypeContract();
                ViewData["typecontract"] = typecontract.ReturnTypeContract();

                StateContract statecontract = new StateContract();
                ViewData["statecontract"] = statecontract.ReturnStateContract();

                ApproverContract approver = new ApproverContract();
                ViewData["approver"] = approver.ReturnApproverContract();

                AdministratorContract administrator = new AdministratorContract();
                ViewData["administrator"] = administrator.ReturnAdministratorContract();

                Provider_Manufacturing providerManufacturing = new Provider_Manufacturing();
                ViewData["providerManufacturing"] = providerManufacturing.ReturnProviderManufacturing();

                ProviderVendor providervendor = new ProviderVendor();
                ViewData["providervendor"] = providervendor.ReturnProviderVendor();

                PurchaseOrder purchaseorder = new PurchaseOrder();
                ViewData["purchaseorder"] = purchaseorder.ReturnPurchaseOrder();

                MethodPayment methodPayment = new MethodPayment();
                ViewData["methodPayment"] = methodPayment.ReturnMethodPayment();

                Cards cards = new Cards();
                ViewData["cards"] = cards.ReturnCards();

                TypePayment typepayment = new TypePayment();
                ViewData["typepayment"] = typepayment.ReturnTypePayment();

                AllBusinessUnit businessUnit = new AllBusinessUnit();
                ViewData["businessUnit"] = businessUnit.ReturnAllBusinessUnit();

                ContractArea contractarea = new ContractArea();
                ViewData["contractarea"] = contractarea.ReturnContractArea();

                WeatherAlerts weatherAlerts = new WeatherAlerts();
                ViewData["weatherAlerts"] = weatherAlerts.ReturnWeatherAlerts();

                EditContracts editarContratos = new EditContracts();
                ViewData["ContractForEdit"] = editarContratos.ReturnDraftContractsForEdit(ContractId);

                return View();
            }
            else
            {
                int nivelMensaje = 3;
                string mensaje = "No Cuenta con los permisos necesarios para acceder a esta parte del sitio web";
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = nivelMensaje,
                    Mensaje = mensaje
                });
            }
        }


 //Codigo para editar un contrato ya existente en la base de datos

        [HttpGet]
        public ActionResult EditarContrato(int ContractId = 0)
        {
            string UserKey = "";
            int permisos = 0;

            try
            {
                UserKey = Session["UserKey"].ToString();
                permisos = (int)Session["UserPermission"];
                ViewData["permisos"] = permisos;
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            if (permisos == 1 || permisos == 2 || permisos == 3 || permisos== 4)
            {
                TypeContract typecontract = new TypeContract();
                ViewData["typecontract"] = typecontract.ReturnTypeContract();

                StateContract statecontract = new StateContract();
                ViewData["statecontract"] = statecontract.ReturnStateContract();

                ApproverContract approver = new ApproverContract();
                ViewData["approver"] = approver.ReturnApproverContract();

                AdministratorContract administrator = new AdministratorContract();
                ViewData["administrator"] = administrator.ReturnAdministratorContract();

                Provider_Manufacturing providerManufacturing = new Provider_Manufacturing();
                ViewData["providerManufacturing"] = providerManufacturing.ReturnProviderManufacturing();

                ProviderVendor providervendor = new ProviderVendor();
                ViewData["providervendor"] = providervendor.ReturnProviderVendor();

                PurchaseOrder purchaseorder = new PurchaseOrder();
                ViewData["purchaseorder"] = purchaseorder.ReturnPurchaseOrder();

                MethodPayment methodPayment = new MethodPayment();
                ViewData["methodPayment"] = methodPayment.ReturnMethodPayment();

                Cards cards = new Cards();
                ViewData["cards"] = cards.ReturnCards();

                TypePayment typepayment = new TypePayment();
                ViewData["typepayment"] = typepayment.ReturnTypePayment();

                AllBusinessUnit businessUnit = new AllBusinessUnit();
                ViewData["businessUnit"] = businessUnit.ReturnAllBusinessUnit();

                ContractArea contractarea = new ContractArea();
                ViewData["contractarea"] = contractarea.ReturnContractArea();

                WeatherAlerts weatherAlerts = new WeatherAlerts();
                ViewData["weatherAlerts"] = weatherAlerts.ReturnWeatherAlerts();

                EditContracts editarContratos = new EditContracts();
                ViewData["ContractForEdit"] = editarContratos.ReturnContractsForEdit(ContractId);
                      
                return View();
            }
            else
            {              
                int nivelMensaje = 3;
                string mensaje = "No Cuenta con los permisos necesarios para acceder a esta parte del sitio web";
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = nivelMensaje,
                    Mensaje = mensaje
                });
            }          
        }

        [HttpPost]
        public ActionResult EditarContrato(int Contract_Id, string Contract_Name, int Type_Of_Contract,string expenses ,int State, int Approver, int administrator
            , int Provider_Manufacturing, int Provider_Vendor, string[] Product2, int Method_Pay, decimal Cost, int Type_Pay, decimal Cost_Per_Payment, DateTime Date_Renewal
            ,  DateTime? Date_Warranty_End, int Bussines_Unit_Name, int Area_Contract, DateTime Date_Start, DateTime Date_End, string Description, int Notify_Before, HttpPostedFileBase imagen,
            int Quantity = 0, string Warranty = "", int Card_Type = 0, int Purchase_Order_Code = 0, string Number_Of_Contract = "N/A", string Number_Service_Contract = "N/A", int Product = 0, string UploadOculto="")
        {
           
            DateTime fecha_creacion = DateTime.Now;
            int nivelMensaje = 0;
            string mensaje = "";
            string productos = "";

            if (Product2 == null)
            {
                mensaje = "Error";
                nivelMensaje = 3;
            }
            else
            {
                for (int i = 0; i < Product2.Length; i++)
                {
                    productos += Product2[i] + "¡";
                }

                if (Date_Warranty_End == null)
                {
                    Date_Warranty_End = Convert.ToDateTime("01/01/1901");
                }

                try
                {
                    if (imagen == null)
                    {                      
                        Entidad.SP_EditContratos(Contract_Id, Contract_Name, Number_Of_Contract, Number_Service_Contract,expenses, Date_Start, Date_End, Cost, Cost_Per_Payment, Date_Renewal, Quantity, Warranty, Date_Warranty_End,
                            fecha_creacion, Description,UploadOculto,Provider_Manufacturing, Provider_Vendor, Approver, administrator, Type_Of_Contract, Method_Pay, Bussines_Unit_Name, Type_Pay, Purchase_Order_Code, Card_Type, Area_Contract, Product
                            , Notify_Before, State,productos);

                        mensaje = "Data inserted succesfully";
                        nivelMensaje = 2;
                    }

                    else
                        if (imagen.ContentType != "image/jpeg" && imagen.ContentType != "image/png" && imagen.ContentType != "application/msword" && imagen.ContentType != "application/pdf"
                          && imagen.ContentType != "application/vnd.ms-excel" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" &&
                          imagen.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.template" && imagen.ContentType != "application/vnd.ms-word.document.macroEnabled.12" &&
                          imagen.ContentType != "application/vnd.ms-word.template.macroEnabled.12" && imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
                          imagen.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.template" && imagen.ContentType != "application/vnd.ms-excel.sheet.macroEnabled.12" &&
                          imagen.ContentType != "application/vnd.ms-excel.template.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.addin.macroEnabled.12" && imagen.ContentType != "application/vnd.ms-excel.sheet.binary.macroEnabled.12"
                          && imagen.ContentType != "multipart/x-zip" && imagen.ContentType != "application/x-tar" && imagen.ContentType != "application/zip" && imagen.ContentType != "application/x-rar")
                        {
                            mensaje = "Extension de archivo no permitida";
                            nivelMensaje = 4;
                        }

                        else
                        {
                            // Codigo del programa para reemplazar caracteres no permitidos 
                            //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera
                            string nombreArchivo = imagen.FileName.Replace("\\", "");
                            nombreArchivo = nombreArchivo.Replace("//", "");
                            nombreArchivo = nombreArchivo.Replace(":", "");
                            nombreArchivo = nombreArchivo.Replace("*", "");
                            nombreArchivo = nombreArchivo.Replace("?", "");
                            nombreArchivo = nombreArchivo.Replace("<", "");
                            nombreArchivo = nombreArchivo.Replace(">", "");
                            nombreArchivo = nombreArchivo.Replace("|", "");
                            //Fin Codigo del programa para reemplazar caracteres no permitidos 
                            //Nota: Solucion hecha 1 hora antes de presentar el proyecto, osea a la carrera

                            string archivo = (DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + nombreArchivo).ToLower();
                            imagen.SaveAs("C:\\Uploads\\" + archivo);

                            Entidad.SP_EditContratos(Contract_Id, Contract_Name, Number_Of_Contract, Number_Service_Contract,expenses, Date_Start, Date_End, Cost, Cost_Per_Payment, Date_Renewal, Quantity, Warranty, Date_Warranty_End,
                               fecha_creacion, Description,archivo,Provider_Manufacturing, Provider_Vendor, Approver, administrator, Type_Of_Contract, Method_Pay, Bussines_Unit_Name, Type_Pay, Purchase_Order_Code, Card_Type, Area_Contract, Product
                               , Notify_Before, State,productos);

                            mensaje = "Data inserted succesfully";
                            nivelMensaje = 2;

                        }
                }
                catch (Exception)
                {
                    mensaje = "Extension de archivo no permitida";
                    nivelMensaje = 3;
                }
            }
 
            TypeContract typecontract = new TypeContract();
            ViewData["typecontract"] = typecontract.ReturnTypeContract();

            StateContract statecontract = new StateContract();
            ViewData["statecontract"] = statecontract.ReturnStateContract();

            ApproverContract approver = new ApproverContract();
            ViewData["approver"] = approver.ReturnApproverContract();

            AdministratorContract administratorcontract = new AdministratorContract();
            ViewData["administrator"] = administratorcontract.ReturnAdministratorContract();

            Provider_Manufacturing providerManufacturing = new Provider_Manufacturing();
            ViewData["providerManufacturing"] = providerManufacturing.ReturnProviderManufacturing();

            ProviderVendor providervendor = new ProviderVendor();
            ViewData["providervendor"] = providervendor.ReturnProviderVendor();

            PurchaseOrder purchaseorder = new PurchaseOrder();
            ViewData["purchaseorder"] = purchaseorder.ReturnPurchaseOrder();

            MethodPayment methodPayment = new MethodPayment();
            ViewData["methodPayment"] = methodPayment.ReturnMethodPayment();

            Cards cards = new Cards();
            ViewData["cards"] = cards.ReturnCards();

            TypePayment typepayment = new TypePayment();
            ViewData["typepayment"] = typepayment.ReturnTypePayment();

            AllBusinessUnit businessUnit = new AllBusinessUnit();
            ViewData["businessUnit"] = businessUnit.ReturnAllBusinessUnit();

            ContractArea contractarea = new ContractArea();
            ViewData["contractarea"] = contractarea.ReturnContractArea();

            WeatherAlerts weatherAlerts = new WeatherAlerts();
            ViewData["weatherAlerts"] = weatherAlerts.ReturnWeatherAlerts();

            EditContracts editarContratos = new EditContracts();
            ViewData["ContractForEdit"] = editarContratos.ReturnContractsForEdit();
           
            
            return RedirectToRoute(new
            {
                controller = "Main",
                action = "main",
                NivelMensaje = nivelMensaje,
                Mensaje = mensaje
            });
            
        }
 //Codigo para eliminar un contrato ya existente en la base de dato
        [HttpGet]
        public ActionResult EliminarContrato(int ContractId = 0)
        {
            string UserKey = "";
            int permisos = 0;

            try
            {
                UserKey = Session["UserKey"].ToString();
                permisos = (int)Session["UserPermission"];
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            if (permisos == 1 || permisos == 2 || permisos == 3 || permisos == 4)
            {
                Entidad.SP_DeleteContract(ContractId);
                return RedirectToAction("main", "Main",1);
               
            }
            else
            {
                int nivelMensaje = 3;
                string mensaje = "No Cuenta con los permisos necesarios para acceder a esta parte del sitio web";
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = nivelMensaje,
                    Mensaje = mensaje
                });
            }
        }

        //Codigo para eliminar un contrato ya existente en la base de dato
        [HttpGet]
        public ActionResult EliminarBorradorContrato(int ContractId = 0)
        {
            string UserKey = "";
            int permisos = 0;

            try
            {
                UserKey = Session["UserKey"].ToString();
                permisos = (int)Session["UserPermission"];
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            if (permisos == 1 || permisos == 2 || permisos == 3 || permisos == 4)
            {
                Entidad.SP_DeleteDraftContract(ContractId);
                return RedirectToAction("ModificarBorrador", "BackEnd", 2);
            }
            else
            {
                int nivelMensaje = 3;
                string mensaje = "No Cuenta con los permisos necesarios para acceder a esta parte del sitio web";
                return RedirectToRoute(new
                {
                    controller = "BackEnd",
                    action = "ModificarBorrador",
                    NivelMensaje = nivelMensaje,
                    Mensaje = mensaje
                });
            }
        }
    }
}

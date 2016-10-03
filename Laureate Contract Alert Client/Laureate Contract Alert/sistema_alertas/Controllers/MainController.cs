using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sistema_alertas.Models;
//Para el manejo de Archivos
using System.IO;
//Clases necesarias de iTextSharp
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
//Para la creacion de un excel
using DocumentFormat.OpenXml;
using ClosedXML.Excel;

namespace sistema_alertas.Controllers
{
    public class MainController : Controller
    {
       //Accion de la vista main, del controlador Main, esta accion es fundamental, ya que aqui es donde se determina que es lo que el usuario podra ver y hacer en el sistema
        [HttpGet]
        public ActionResult main(int NivelMensaje =1, string Mensaje="")
        {
            //Lo primero es determinar si el usuario se encuentra previamente logueado en el sistema, al momento que el usuario se loguea, se le asigna una llave por asi decirlo,
            //si esa llave esta vacia en la variable de session "UserKey" significa que el usuario no paso por el proceso de logueo y no esta siguiendo el orden normal del sistema
            //Y por tanto se debe redirigir al sistema de login para que haga su correcto ingreso al sistema
            string UserKey = "";
            int PermisoUsuario = 0;

            try
            {
              UserKey = Session["UserKey"].ToString();
              PermisoUsuario = (int)Session["UserId"];

            }catch(Exception )
            {
               return RedirectToAction("Index", "Home");
            }
          
            Contracts contract = new Contracts();
            AdminUserPermissions role = new AdminUserPermissions();
            List<SP_selectAdminUserPermissions_Result> AdminUserPermissionsss = role.ReturnAdminUserPermissions(PermisoUsuario);

            /*Aqui es donde se determina el nivel que tiene el usuario si el usuario tiene un nivel 1 significa que es un super administrador y que puede realizar cualquier 
            accion en el sistema. 2 significa que es un administradir IT y por tanto solo podra ver los contratos registrados de IT ,3 administrador de Main Office, solo podra
           *ver los contratos de MO, 4 es un usuario aprobador , podra ver todos los contratos pero no podra editar ni eliminar algun contrato, tampoco entrar a la pantalla de administracion del sistema
           *5 y 6 usuario de IT y Usuario de Main Office respectivamente, estos usuarios podran ver los contratos de sus respectivas areas, , podran agregar contratos, productos, y ciertas cosas mas al 
           *sistema, pero no podran editar ni eliminar algun elemento dentro del sistema
           */

            foreach (SP_selectAdminUserPermissions_Result permisos in AdminUserPermissionsss)
            {
               if (permisos.PerfilId == 1 || permisos.PerfilId == 5)
               {
                   //Este viewData[Decision] se utiliza porque si la decision es igual a 1, significa que el usuario es un administrador o es un aprobador , por lo tanto se seleccionen todos los contratos
                   //para que estos los puedan ver  y en los siguientes casos que es igual a 2 significa que se debe mostrar solo los contratos de cada area correspondiente y se debe hacer una consulta enviandole
                   //el numero de area que le corresponda a cada quien , un Area 2 corresponde al area de IT y un Area 1 corresponde al area de Main Office
                   ViewData["Decision"] = 1;
                   Session["UserPermission"] = permisos.PerfilId;
                   ViewData["contract"] = contract.ReturnContracts();
                   ViewData["NivelMensaje"] = NivelMensaje;
                   ViewData["Mensaje"] = Mensaje;
                   break;
               }
               else
                 if (permisos.PerfilId == 2 || permisos.PerfilId == 6)
                 {
                     ViewData["Decision"] = 2;
                     Session["UserPermission"] = permisos.PerfilId;
                     ViewData["contract2"] = contract.ReturnContractsByArea(2);
                     ViewData["NivelMensaje"] = NivelMensaje;
                     ViewData["Mensaje"] = Mensaje;
                    
                     break;
                 }
                 else
                  if (permisos.PerfilId == 3 || permisos.PerfilId == 7)
                  {
                     ViewData["Decision"] = 2;
                     Session["UserPermission"] = permisos.PerfilId;
                     ViewData["contract2"] = contract.ReturnContractsByArea(1);
                     ViewData["NivelMensaje"] = NivelMensaje;
                     ViewData["Mensaje"] = Mensaje;
                     break;
                  }
                  else
                      if (permisos.PerfilId == 4 || permisos.PerfilId == 8)
                      {
                          ViewData["Decision"] = 2;
                          Session["UserPermission"] = permisos.PerfilId;
                          ViewData["contract2"] = contract.ReturnContractsByArea(3);
                          ViewData["NivelMensaje"] = NivelMensaje;
                          ViewData["Mensaje"] = Mensaje;
                          break;
                      }
            }
            return View();  
        }

        //En esta seccion se administra la vista Detalles de contrato, recibe como parametro un contrato id ya que debe mostrar solo los detalles del contrato que se ha seleccionado
        //y jalar todos los detalles de este contrato desde la base de datos
        [HttpGet]
        public ActionResult DetallesContrato(int ContractId=0)
        {
            string UserKey = "";

            if (ContractId == 0)
                return RedirectToAction("Index", "Home");

            else
            {
                try
                {
                    UserKey = Session["UserKey"].ToString();

                }
                catch (Exception )
                {
                    return RedirectToAction("Index", "Home");
                }

                AllDetailsContract detailsContract = new AllDetailsContract();
                ViewData["detailsContract"] = detailsContract.ReturnAllDetailsContract(ContractId);

                List<SP_selectAllDetailsContract_Result> list =   detailsContract.ReturnAllDetailsContract(ContractId);

                //Ya que en la base de datos existe una relacion doble de usuarios : usuarios administradores y usuarios aprobadores se debe seleccionar esos usuarios en otra consulta
                //haciendo uso del id para traer los detalles de esos usuarios, lo mismo pasa con los proveedores manufactureros y los proveedores distribuidores
                int userApproverId = 0;
                int userAdministratorId = 0;
                int ProviderManufacturingId = 0;
                int ProviderVendorId = 0;

                foreach (SP_selectAllDetailsContract_Result lista in list)
                {
                    userApproverId=lista.Usuario_AprobadorId;
                    userAdministratorId = lista.Usuario_AdministradorId;
                    ProviderManufacturingId = lista.Proveedor_ManufactureroId;
                    ProviderVendorId = lista.Proveedor_DistribuidorId;
                    break;
                }
            
                ProviderById proveedor = new ProviderById();
                UserById user = new UserById();

                ViewData["ProviderManufacturingContract"] = proveedor.ReturnProviderById(ProviderManufacturingId);
                ViewData["ProviderVendorContract"] = proveedor.ReturnProviderById(ProviderVendorId);
                ViewData["UserApproverContract"] = user.ReturnUserById(userApproverId);
                ViewData["UserAdministratorContract"] = user.ReturnUserById(userAdministratorId);
                            
                return View();
            }
        }

        [HttpGet]
        public ActionResult DetallesUsuario(int UserId = 0)
        {
            string UserKey = "";

            if (UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
              try
              {
                UserKey = Session["UserKey"].ToString();
              }
              catch (Exception)
              {
                return RedirectToAction("Index", "Home");
              }
              UserById user = new UserById();
              ViewData["UserDetails"] = user.ReturnUserById(UserId);

              return View();
            }
        }

        [HttpGet]
        public void SomeImage(string imageName, int id)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();

            if (imageName != null)
            {            
                Response.ContentType = "image/jpeg";
                Response.Flush();
                try
                {
                   Response.TransmitFile("C:\\Uploads\\"+imageName);

                }catch(Exception)
                {
                   if(id==1)
                     Response.TransmitFile("C:\\Uploads\\User.png");
                    
                   else
                    Response.TransmitFile("C:\\Uploads\\Producto.png");
                }            
            }
            else
             if (id==1)
             {              
                Response.ContentType = "image/jpeg";
                Response.Flush();
                Response.TransmitFile("C:\\Uploads\\User.png");
             }

            else
             if (id == 2)
             {               
                Response.ContentType = "image/png";
                Response.Flush();
                Response.TransmitFile("C:\\Uploads\\Producto.png");
             }
        }

        [HttpGet]
        public ActionResult DetallesProducto(int ProductId = 0)
        {
            string UserKey = "";

            if (ProductId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                try
                {
                    UserKey = Session["UserKey"].ToString();
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }

                ProductById product = new ProductById();
                ViewData["ProductDetails"] = product.ReturnProduct(ProductId);
                List<SP_selectProductById_Result> list = product.ReturnProduct(ProductId);

                int ProviderManufacturingId = 0;
                int ProviderVendorId = 0;

                foreach (SP_selectProductById_Result lista in list)
                {
                    ProviderManufacturingId = lista.Proveedor_ManufactureroId;
                    ProviderVendorId = lista.Proveedor_DistribuidorId;
                    break;
                }
           
                ProviderById proveedor = new ProviderById();
                UserById user = new UserById();

                ViewData["ProviderManufacturingContract"] = proveedor.ReturnProviderById(ProviderManufacturingId);
                ViewData["ProviderVendorContract"] = proveedor.ReturnProviderById(ProviderVendorId);

                return View();
            }
        }

        [HttpGet]
        public ActionResult DetallesProducto2(string ProductName = "")
        {
            string UserKey = "";

            if (ProductName == "")
                return RedirectToAction("Index", "Home");
            else
            {
                try
                {
                    UserKey = Session["UserKey"].ToString();
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }

                ProductById product = new ProductById();
                ViewData["ProductDetails"] = product.ReturnProductByName(ProductName);
                List<SP_selectProductByName_Result> list = product.ReturnProductByName(ProductName);

                int ProviderManufacturingId = 0;
                int ProviderVendorId = 0;

                foreach (SP_selectProductByName_Result lista in list)
                {
                    ProviderManufacturingId = lista.Proveedor_ManufactureroId;
                    ProviderVendorId = lista.Proveedor_DistribuidorId;
                    break;
                }

                ProviderById proveedor = new ProviderById();
                UserById user = new UserById();

                ViewData["ProviderManufacturingContract"] = proveedor.ReturnProviderById(ProviderManufacturingId);
                ViewData["ProviderVendorContract"] = proveedor.ReturnProviderById(ProviderVendorId);

                return View();
            }
        }

        [HttpGet]
        public ActionResult DetallesProveedor(int ProveedorId = 0)
        {
            string UserKey = "";

            if (ProveedorId == 0)
            {             
                return RedirectToAction("Index", "Home");
            }
               
            else
            {
                try
                {
                    UserKey = Session["UserKey"].ToString();
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }
                ProviderById proveedor = new ProviderById();
                ViewData["Provider"] = proveedor.ReturnProviderById(ProveedorId);

                ProductByProviderId product = new ProductByProviderId();
                ViewData["ProductByProviderId"] = product.ReturnProductByProviderId(ProveedorId);

                return View();
            }
        }

        //En esta seccion se empiezan a generar los PDF , solo comentare una accion ya que las demas son exactamente igual y lo unico que cambia es la consulta que se hace a la base de datos

        public ActionResult GenerarPDFTodosContratos()
        {
            bool bandera = false;

            //Creamos un tipo de archivo que solo se cargará en la memoria principal
            Document documento = new Document();
            
            //Obtenemos la fecha actual incluyendo horas, minutos y segundos, esto se agregara al path del contrato, esto se hace asi para poder tener varios reportes de contratos guardados con el mismo nombre
            //y que lo que diferencie a cada reporte de contrato sea la fecha y hora en que se han generado
            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            //Se crea el path (direccion y nombre con el cual se guardara el reporte generado )
            // string path = "C:\\Users\\cturcios\\Desktop\\sistema_alertas\\sistema_alertas\\Content\\Reportes\\ReportesTotalContratos\\ReportesTotalContratos"+fechaActual+".pdf";
            string path = "C:\\Reportes\\ReportesTotalContratos\\ReportesTotalContratos"+fechaActual+".pdf";
              
            //Creamos la instancia para generar el archivo PDF
            //Le pasamos el documento creado arriba y con capacidad para abrir o Crear y de nombre Mi_Primer_PDF
            PdfWriter.GetInstance(documento, new FileStream(path, FileMode.OpenOrCreate));

            //Rotamos el documento para que quede horizontal y pueda caber mas informacion para ser visualizada por el usuario
            documento.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            //Abrimos el documento
            documento.Open();
                  
            //Le decimos que nuestro documento actualmente tendra 9 celdas (posible cambio en futuro)
            iTextSharp.text.pdf.PdfPTable aTable = new iTextSharp.text.pdf.PdfPTable(9);           
            //Se hace una instancia de la clase que es la encargada de hacer la consulta a la base de datos para retornar los contratos
            AllDetailsContractForPDF detailsContract = new AllDetailsContractForPDF();
            
            //Se crea una nueva celda y se le agrega un titulo
            PdfPCell cell = new PdfPCell(new Phrase("List All Contacts Stored On The System"));
            
            //Estas son solamente opciones de personalizacion y diseño de la celda          
            cell.HorizontalAlignment = 1;
            cell.UseVariableBorders = true;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new iTextSharp.text.BaseColor(245, 92, 24);
            cell.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cell.Colspan = 9;
           
            //Se agrega la celda que se creo al documento
            aTable.AddCell(cell);

            //Se agregan los titulos de las celdas al documento

            PdfPCell headers = new PdfPCell(new Phrase("Name Contract"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Contract number"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Number Service"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Start date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("End date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Payment type"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Total cost"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Cost Of Payment"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Renewal Payment Date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

           
            //aTable.AddCell("Cantidad Licencias");
            //aTable.AddCell("Garantia");
           // aTable.AddCell("Fecha Vencimiento Garantia");
           // aTable.AddCell("Nombre Unidad De Negocio");
            //aTable.AddCell("Estado Contrato");


            //Nuevamente se hace una restriccion dependiendo el usuario en los reportes, los super usuarios y aprobadores podran ver todos los contratos, los administradores y usuarios de IT
            //solo podran ver los contratos de su area y los administradores y usuarios de MO solo podran ver los contratos de su area tambien Nota: posiblemente en el futuro el sistema tenga mas roles creados
            // pero la logica que se usara para estos roles sera la misma
            //Ya que se debe pensar en el usuario que esta generando la solicitud del reporte segun su nivel en el sistema, se deben hacer dos consultas a la base de datos una para traer todos los contratos y mostrarlos
            // si el usuario resultara ser un super admin o aprobador o una consulta y traer los contratos segun el area, dependiente de que usuario sea el que genere la solicitud en el sistema

            if ((int)Session["UserPermission"] == 1 || (int)Session["UserPermission"] == 5)
            {               
                List<AllDetailsContractForReportPDF_Result> lista = detailsContract.ReturnAllDetailsContract().ToList();

                foreach (AllDetailsContractForReportPDF_Result list in lista)
                {
                    aTable.AddCell(list.Nombre_Contrato);
                    aTable.AddCell(list.Numero_Contrato);
                    aTable.AddCell(list.Numero_Servicio_Contrato);
                    aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                    aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                    aTable.AddCell(list.Nombre_Tipo_Pago);
                    aTable.AddCell(list.Costo_Total.ToString());
                    aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                    aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                    /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                    aTable.AddCell(list.Garantia);*/
                    //   aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                    //  aTable.AddCell(list.Nombre_Unidad_Negocio);
                    //aTable.AddCell(list.Estado_Contrato);
                }
            }

            else
                if ((int)Session["UserPermission"] == 2 || (int)Session["UserPermission"] == 6)
                {                   
                    List<AllDetailsContractByAreaForReportPDF_Result> lista = detailsContract.ReturnAllDetailsContractByArea(2).ToList();

                    foreach (AllDetailsContractByAreaForReportPDF_Result list in lista)
                    {
                        aTable.AddCell(list.Nombre_Contrato);
                        aTable.AddCell(list.Numero_Contrato);
                        aTable.AddCell(list.Numero_Servicio_Contrato);
                        aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                        aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                        aTable.AddCell(list.Nombre_Tipo_Pago);
                        aTable.AddCell(list.Costo_Total.ToString());
                        aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                        aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                        /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                        aTable.AddCell(list.Garantia);*/
                        //   aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                        //  aTable.AddCell(list.Nombre_Unidad_Negocio);
                        //aTable.AddCell(list.Estado_Contrato);
                    }
                }
             else
               if ((int)Session["UserPermission"] == 3 || (int)Session["UserPermission"] == 7)
               {
                   List<AllDetailsContractByAreaForReportPDF_Result> lista = detailsContract.ReturnAllDetailsContractByArea(1).ToList();

                   foreach (AllDetailsContractByAreaForReportPDF_Result list in lista)
                   {
                       aTable.AddCell(list.Nombre_Contrato);
                       aTable.AddCell(list.Numero_Contrato);
                       aTable.AddCell(list.Numero_Servicio_Contrato);
                       aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                       aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                       aTable.AddCell(list.Nombre_Tipo_Pago);
                       aTable.AddCell(list.Costo_Total.ToString());
                       aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                       aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                       /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                       aTable.AddCell(list.Garantia);*/
                       //   aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                       //  aTable.AddCell(list.Nombre_Unidad_Negocio);
                       //aTable.AddCell(list.Estado_Contrato);
                   }
               }

               else
                   if ((int)Session["UserPermission"] == 4 || (int)Session["UserPermission"] == 8)
                   {
                       List<AllDetailsContractByAreaForReportPDF_Result> lista = detailsContract.ReturnAllDetailsContractByArea(3).ToList();

                       foreach (AllDetailsContractByAreaForReportPDF_Result list in lista)
                       {
                           aTable.AddCell(list.Nombre_Contrato);
                           aTable.AddCell(list.Numero_Contrato);
                           aTable.AddCell(list.Numero_Servicio_Contrato);
                           aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                           aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                           aTable.AddCell(list.Nombre_Tipo_Pago);
                           aTable.AddCell(list.Costo_Total.ToString());
                           aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                           aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                           /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                           aTable.AddCell(list.Garantia);*/
                           //   aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                           //  aTable.AddCell(list.Nombre_Unidad_Negocio);
                           //aTable.AddCell(list.Estado_Contrato);
                       }
                   } 

            //Se agrega toda la informacion traida desde la base de datos al documento
            documento.Add(aTable);
            //Se cierra el documento para que pueda ser visualizado por el usuario
            documento.Close();
            bandera = true;
            
            downloadAdjuntos(path);

            //Si la bandera es true quiere decir que el reporte fue generado de forma exitosa y se puede mostrar un mensaje de exito al usuario
            if (bandera)
            {              
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = 2,
                    Mensaje = "Reporte generado correctamente"
                });
            }
            //Sino se mostrara un mensaje de error al usuario 
            else 
            {            
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = 3,
                    Mensaje = "El reporte no ha podido ser generado"
                });
            }                                                  
        }

        public ActionResult GenerarPDFContratosActivos()
        {
            bool bandera = false;
            //Creamos un tipo de archivo que solo se cargará en la memoria principal
            Document documento = new Document();
            //Creamos la instancia para generar el archivo PDF
            //Le pasamos el documento creado arriba y con capacidad para abrir o Crear y de nombre Mi_Primer_PDF

            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string path = "C:\\Reportes\\ReportesContratosActivos\\ReportesContratosActivos"+fechaActual+".pdf";
          
            PdfWriter.GetInstance(documento, new FileStream(path, FileMode.OpenOrCreate));

            //Rotamos el documento para que quede horizontal
            documento.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            //Abrimos el documento
            documento.Open();
                  
            iTextSharp.text.pdf.PdfPTable aTable = new iTextSharp.text.pdf.PdfPTable(9);

            AllDetailsActiveContractForPDF detailsContractActive = new AllDetailsActiveContractForPDF();
            PdfPCell cell = new PdfPCell(new Phrase("Assets Contracts list stored in the system"));
            cell.HorizontalAlignment = 1;
            cell.UseVariableBorders = true;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new iTextSharp.text.BaseColor(245, 92, 24);
            cell.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cell.Colspan = 9;
            //Se agrega la celda que se creo al documento
            aTable.AddCell(cell);

            //Se agregan los titulos de las celdas al documento

            PdfPCell headers = new PdfPCell(new Phrase("Name Contract"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Contract number"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Number Service"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Start date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("End date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Payment type"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Total cost"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Cost Of Payment"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Renewal Payment Date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);
            //aTable.AddCell("Cantidad Licencias");
            //aTable.AddCell("Garantia");
           // aTable.AddCell("Fecha Vencimiento Garantia");
           // aTable.AddCell("Nombre Unidad De Negocio");
            //aTable.AddCell("Estado Contrato");

            if ((int)Session["UserPermission"] == 1 || (int)Session["UserPermission"] == 5)
            {       
                List<ActiveContractForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContract().ToList();
               
                foreach (ActiveContractForReportPDF_Result list in lista)
                {               
                    aTable.AddCell(list.Nombre_Contrato);               
                    aTable.AddCell(list.Numero_Contrato);               
                    aTable.AddCell(list.Numero_Servicio_Contrato);               
                    aTable.AddCell(list.Fecha_Inicio.ToString("d"));               
                    aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));               
                    aTable.AddCell(list.Nombre_Tipo_Pago);              
                    aTable.AddCell(list.Costo_Total.ToString());                
                    aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());               
                    aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                  /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                    aTable.AddCell(list.Garantia);*/
                  //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                  //aTable.AddCell(list.Nombre_Unidad_Negocio);
                  //aTable.AddCell(list.Estado_Contrato);
               }
            }
            else
              if ((int)Session["UserPermission"] == 2 || (int)Session["UserPermission"] == 6)
              {

                  List<ActiveContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(2).ToList();
                  foreach (ActiveContractByAreaForReportPDF_Result list in lista)
                  {
                      aTable.AddCell(list.Nombre_Contrato);
                      aTable.AddCell(list.Numero_Contrato);
                      aTable.AddCell(list.Numero_Servicio_Contrato);
                      aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                      aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                      aTable.AddCell(list.Nombre_Tipo_Pago);
                      aTable.AddCell(list.Costo_Total.ToString());
                      aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                      aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                      /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                        aTable.AddCell(list.Garantia);*/
                      //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                      //aTable.AddCell(list.Nombre_Unidad_Negocio);
                      //aTable.AddCell(list.Estado_Contrato);
                  }
              }
            else
              if ((int)Session["UserPermission"] == 3 || (int)Session["UserPermission"] == 7)
              {
                  List<ActiveContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(1).ToList();
                  foreach (ActiveContractByAreaForReportPDF_Result list in lista)
                  {
                      aTable.AddCell(list.Nombre_Contrato);
                      aTable.AddCell(list.Numero_Contrato);
                      aTable.AddCell(list.Numero_Servicio_Contrato);
                      aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                      aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                      aTable.AddCell(list.Nombre_Tipo_Pago);
                      aTable.AddCell(list.Costo_Total.ToString());
                      aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                      aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                      /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                        aTable.AddCell(list.Garantia);*/
                      //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                      //aTable.AddCell(list.Nombre_Unidad_Negocio);
                      //aTable.AddCell(list.Estado_Contrato);
                  }
              }

              else
                  if ((int)Session["UserPermission"] == 4 || (int)Session["UserPermission"] == 8)
                  {
                      List<ActiveContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(3).ToList();
                      foreach (ActiveContractByAreaForReportPDF_Result list in lista)
                      {
                          aTable.AddCell(list.Nombre_Contrato);
                          aTable.AddCell(list.Numero_Contrato);
                          aTable.AddCell(list.Numero_Servicio_Contrato);
                          aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                          aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                          aTable.AddCell(list.Nombre_Tipo_Pago);
                          aTable.AddCell(list.Costo_Total.ToString());
                          aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                          aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                          /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                            aTable.AddCell(list.Garantia);*/
                          //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                          //aTable.AddCell(list.Nombre_Unidad_Negocio);
                          //aTable.AddCell(list.Estado_Contrato);
                      }
                  }

            documento.Add(aTable);
            documento.Close();
            bandera = true;
            downloadAdjuntos(path);

            if (bandera)
            {              
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = 2,
                    Mensaje = "Reporte generado correctamente"
                });
            }
            else 
            {            
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = 3,
                    Mensaje = "El reporte no ha podido ser generado"
                });
            }                                                  
        }

        public ActionResult GenerarPDFContratosProximosAVencer()
        {
            bool bandera = false;
            //Creamos un tipo de archivo que solo se cargará en la memoria principal
            Document documento = new Document();
            //Creamos la instancia para generar el archivo PDF
            //Le pasamos el documento creado arriba y con capacidad para abrir o Crear y de nombre Mi_Primer_PDF

            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            //string path = "C:/Users/cturcios/Desktop/sistema_alertas/sistema_alertas/Content/Reportes/ReportesContratosProximosAVencer/ReportesContratosProximosAVencer" + fechaActual + ".pdf";
            string path = "C:\\Reportes\\ReportesContratosProximosAVencer\\ReportesContratosProximosAVencer"+fechaActual+".pdf";
                    
            PdfWriter.GetInstance(documento, new FileStream(path, FileMode.OpenOrCreate));

            //Rotamos el documento para que quede horizontal
            documento.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            //Abrimos el documento
            documento.Open();

            iTextSharp.text.pdf.PdfPTable aTable = new iTextSharp.text.pdf.PdfPTable(9);

            AllDetailsContractAboutToFinishForPDF detailsContractActive = new AllDetailsContractAboutToFinishForPDF();

            PdfPCell cell = new PdfPCell(new Phrase("Contracts List Next A Win stored in the system"));
            cell.HorizontalAlignment = 1;
            cell.UseVariableBorders = true;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new iTextSharp.text.BaseColor(245, 92, 24);
            cell.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cell.Colspan = 9;
            //Se agrega la celda que se creo al documento
            aTable.AddCell(cell);

            //Se agregan los titulos de las celdas al documento

            PdfPCell headers = new PdfPCell(new Phrase("Name Contract"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Contract number"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Number Service"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Start date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("End date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Payment type"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Total cost"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Cost Of Payment"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Renewal Payment Date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);
            //aTable.AddCell("Cantidad Licencias");
            //aTable.AddCell("Garantia");
            // aTable.AddCell("Fecha Vencimiento Garantia");
            // aTable.AddCell("Nombre Unidad De Negocio");
            //aTable.AddCell("Estado Contrato");

            if ((int)Session["UserPermission"] == 1 || (int)Session["UserPermission"] == 5)
            {
                List<AboutToFinishContractForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContract().ToList();

                foreach (AboutToFinishContractForReportPDF_Result list in lista)
                {
                    aTable.AddCell(list.Nombre_Contrato);
                    aTable.AddCell(list.Numero_Contrato);
                    aTable.AddCell(list.Numero_Servicio_Contrato);
                    aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                    aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                    aTable.AddCell(list.Nombre_Tipo_Pago);
                    aTable.AddCell(list.Costo_Total.ToString());
                    aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                    aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                    /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                      aTable.AddCell(list.Garantia);*/
                    //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                    //aTable.AddCell(list.Nombre_Unidad_Negocio);
                    //aTable.AddCell(list.Estado_Contrato);
                }
            }

            else
             if ((int)Session["UserPermission"] == 2 || (int)Session["UserPermission"] == 6)
             {
                List<AboutToFinishContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(2).ToList();

                foreach (AboutToFinishContractByAreaForReportPDF_Result list in lista)
               {
                  aTable.AddCell(list.Nombre_Contrato);
                  aTable.AddCell(list.Numero_Contrato);
                  aTable.AddCell(list.Numero_Servicio_Contrato);
                  aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                  aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                  aTable.AddCell(list.Nombre_Tipo_Pago);
                  aTable.AddCell(list.Costo_Total.ToString());
                  aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                  aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                      aTable.AddCell(list.Garantia);*/
                    //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                    //aTable.AddCell(list.Nombre_Unidad_Negocio);
                    //aTable.AddCell(list.Estado_Contrato);
                }
            }
             else
               if ((int)Session["UserPermission"] == 3 || (int)Session["UserPermission"] == 7)
               {
                     List<AboutToFinishContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(1).ToList();

                     foreach (AboutToFinishContractByAreaForReportPDF_Result list in lista)
                     {
                         aTable.AddCell(list.Nombre_Contrato);
                         aTable.AddCell(list.Numero_Contrato);
                         aTable.AddCell(list.Numero_Servicio_Contrato);
                         aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                         aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                         aTable.AddCell(list.Nombre_Tipo_Pago);
                         aTable.AddCell(list.Costo_Total.ToString());
                         aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                         aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                         /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                               aTable.AddCell(list.Garantia);*/
                         //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                         //aTable.AddCell(list.Nombre_Unidad_Negocio);
                         //aTable.AddCell(list.Estado_Contrato);
                     }
                 }
               else
                   if ((int)Session["UserPermission"] == 4 || (int)Session["UserPermission"] == 8)
                   {
                       List<AboutToFinishContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(3).ToList();

                       foreach (AboutToFinishContractByAreaForReportPDF_Result list in lista)
                       {
                           aTable.AddCell(list.Nombre_Contrato);
                           aTable.AddCell(list.Numero_Contrato);
                           aTable.AddCell(list.Numero_Servicio_Contrato);
                           aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                           aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                           aTable.AddCell(list.Nombre_Tipo_Pago);
                           aTable.AddCell(list.Costo_Total.ToString());
                           aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                           aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                           /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                             aTable.AddCell(list.Garantia);*/
                           //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                           //aTable.AddCell(list.Nombre_Unidad_Negocio);
                           //aTable.AddCell(list.Estado_Contrato);
                       }
                   }

            documento.Add(aTable);
            documento.Close();
            bandera = true;
            downloadAdjuntos(path);

            if (bandera)
            {
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = 2,
                    Mensaje = "Reporte generado correctamente , se encuentra en la Carpeta Content del directorio raiz"
                });
            }
            else
            {
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = 3,
                    Mensaje = "El reporte no ha podido ser generado"
                });
            }
        }

        public ActionResult GenerarPDFContratosVencidos()
        {
            bool bandera = false;
            //Creamos un tipo de archivo que solo se cargará en la memoria principal
            Document documento = new Document();
            //Creamos la instancia para generar el archivo PDF
            //Le pasamos el documento creado arriba y con capacidad para abrir o Crear y de nombre Mi_Primer_PDF

            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            //string path = "C:/Users/cturcios/Desktop/sistema_alertas/sistema_alertas/Content/Reportes/ReportesContratosProximosAVencer/ReportesContratosProximosAVencer" + fechaActual + ".pdf";
            string path = "C:\\Reportes\\ReportesContratosVencidos\\ReportesContratosVencidos" + fechaActual + ".pdf";

            PdfWriter.GetInstance(documento, new FileStream(path, FileMode.OpenOrCreate));

            //Rotamos el documento para que quede horizontal
            documento.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            //Abrimos el documento
            documento.Open();

            iTextSharp.text.pdf.PdfPTable aTable = new iTextSharp.text.pdf.PdfPTable(9);

            AllDetailsContractDefeatedForPDF detailsContractDefeated = new AllDetailsContractDefeatedForPDF();

            PdfPCell cell = new PdfPCell(new Phrase("Expired Contracts list stored in the system"));
            cell.HorizontalAlignment = 1;
            cell.UseVariableBorders = true;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new iTextSharp.text.BaseColor(245, 92, 24);
            cell.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            cell.Colspan = 9;
            //Se agrega la celda que se creo al documento
            aTable.AddCell(cell);

            PdfPCell headers = new PdfPCell(new Phrase("Name Contract"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Contract number"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Number Service"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Start date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("End date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Payment type"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Total cost"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Cost Of Payment"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);

            headers = new PdfPCell(new Phrase("Renewal Payment Date"));
            headers.BorderWidthRight = 1;
            headers.HorizontalAlignment = 1;
            headers.BackgroundColor = new iTextSharp.text.BaseColor(247, 150, 70);
            headers.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
            headers.Colspan = 1;
            aTable.AddCell(headers);
            //aTable.AddCell("Cantidad Licencias");
            //aTable.AddCell("Garantia");
            // aTable.AddCell("Fecha Vencimiento Garantia");
            // aTable.AddCell("Nombre Unidad De Negocio");
            //aTable.AddCell("Estado Contrato");

            if ((int)Session["UserPermission"] == 1 || (int)Session["UserPermission"] == 5)
            {             
                List<defeatedContractForReportPDF_Result> lista = detailsContractDefeated.ReturnAllDetailsContract().ToList();

                foreach (defeatedContractForReportPDF_Result list in lista)
                {
                    aTable.AddCell(list.Nombre_Contrato);
                    aTable.AddCell(list.Numero_Contrato);
                    aTable.AddCell(list.Numero_Servicio_Contrato);
                    aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                    aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                    aTable.AddCell(list.Nombre_Tipo_Pago);
                    aTable.AddCell(list.Costo_Total.ToString());
                    aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                    aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                    /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                      aTable.AddCell(list.Garantia);*/
                    //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                    //aTable.AddCell(list.Nombre_Unidad_Negocio);
                    //aTable.AddCell(list.Estado_Contrato);
                }
            }

            else
                if ((int)Session["UserPermission"] == 2 || (int)Session["UserPermission"] == 6)
                {                  
                    List<defeatedContractByAreaForReportPDF_Result> lista = detailsContractDefeated.ReturnAllDetailsContractByArea(2).ToList();

                    foreach (defeatedContractByAreaForReportPDF_Result list in lista)
                    {
                        aTable.AddCell(list.Nombre_Contrato);
                        aTable.AddCell(list.Numero_Contrato);
                        aTable.AddCell(list.Numero_Servicio_Contrato);
                        aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                        aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                        aTable.AddCell(list.Nombre_Tipo_Pago);
                        aTable.AddCell(list.Costo_Total.ToString());
                        aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                        aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                        /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                              aTable.AddCell(list.Garantia);*/
                        //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                        //aTable.AddCell(list.Nombre_Unidad_Negocio);
                        //aTable.AddCell(list.Estado_Contrato);
                    }
                }
                else
                    if ((int)Session["UserPermission"] == 3 || (int)Session["UserPermission"] == 7)
                    {
                        List<defeatedContractByAreaForReportPDF_Result> lista = detailsContractDefeated.ReturnAllDetailsContractByArea(1).ToList();

                        foreach (defeatedContractByAreaForReportPDF_Result list in lista)
                        {
                            aTable.AddCell(list.Nombre_Contrato);
                            aTable.AddCell(list.Numero_Contrato);
                            aTable.AddCell(list.Numero_Servicio_Contrato);
                            aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                            aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                            aTable.AddCell(list.Nombre_Tipo_Pago);
                            aTable.AddCell(list.Costo_Total.ToString());
                            aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                            aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                            /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                                  aTable.AddCell(list.Garantia);*/
                            //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                            //aTable.AddCell(list.Nombre_Unidad_Negocio);
                            //aTable.AddCell(list.Estado_Contrato);
                        }
                    }
                    else
                        if ((int)Session["UserPermission"] == 4 || (int)Session["UserPermission"] == 8)
                        {
                            List<defeatedContractByAreaForReportPDF_Result> lista = detailsContractDefeated.ReturnAllDetailsContractByArea(3).ToList();

                            foreach (defeatedContractByAreaForReportPDF_Result list in lista)
                            {
                                aTable.AddCell(list.Nombre_Contrato);
                                aTable.AddCell(list.Numero_Contrato);
                                aTable.AddCell(list.Numero_Servicio_Contrato);
                                aTable.AddCell(list.Fecha_Inicio.ToString("d"));
                                aTable.AddCell(list.Fecha_Finalizacion.ToString("d"));
                                aTable.AddCell(list.Nombre_Tipo_Pago);
                                aTable.AddCell(list.Costo_Total.ToString());
                                aTable.AddCell(list.Costo_Por_Cada_Pago.ToString());
                                aTable.AddCell(list.Fecha_Renovacion_Pago.ToString("d"));
                                /*aTable.AddCell(list.Cantidad_Licencias.ToString());
                                      aTable.AddCell(list.Garantia);*/
                                //aTable.AddCell(list.Fecha_Vencimiento_Garantia.ToString("d"));
                                //aTable.AddCell(list.Nombre_Unidad_Negocio);
                                //aTable.AddCell(list.Estado_Contrato);
                            }
                        }

            documento.Add(aTable);
            documento.Close();
            bandera = true;
            downloadAdjuntos(path);

            if (bandera)
            {
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = 2,
                    Mensaje = "Reporte generado correctamente , se encuentra en la Carpeta Content del directorio raiz"
                });
            }
            else
            {
                return RedirectToRoute(new
                {
                    controller = "Main",
                    action = "main",
                    NivelMensaje = 3,
                    Mensaje = "El reporte no ha podido ser generado"
                });
            }
        }




        public ActionResult GenerarExcelTodosContratos()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Todos los contratos");

            worksheet.Cell("A2").Value = "Name Contract";
            worksheet.Cell("B2").Value = "Contract number";
            worksheet.Cell("C2").Value = "Service Contract Number";
            worksheet.Cell("D2").Value = "Start date";
            worksheet.Cell("E2").Value = "End date";
            worksheet.Cell("F2").Value = "Payment type";
            worksheet.Cell("G2").Value = "State Contract";
            worksheet.Cell("H2").Value = "Total cost";
            worksheet.Cell("I2").Value = "Cost for each payment";
            worksheet.Cell("J2").Value = "Renewal Payment Date";
            worksheet.Cell("K2").Value = "Number Licenses";
            worksheet.Cell("L2").Value = "Warranty";
            worksheet.Cell("M2").Value = "Guarantee Maturity Date";
            worksheet.Cell("N2").Value = "Business Unit Name";
            worksheet.Cell("O2").Value = "Payment method";
            worksheet.Cell("P2").Value = "Assigned Person Card";
            worksheet.Cell("Q2").Value = "Last Digit Card";
            worksheet.Cell("R2").Value = "Card Expiration Date";           
            worksheet.Cell("S2").Value = "Contract type";
            worksheet.Cell("T2").Value = "Type Expense";
            worksheet.Cell("U2").Value = "Product name";
          
            AllDetailsContractForPDF detailsContract = new AllDetailsContractForPDF();
          

            if ((int)Session["UserPermission"] == 1 || (int)Session["UserPermission"] == 5)
            {
                List<AllDetailsContractForReportPDF_Result> lista = detailsContract.ReturnAllDetailsContract().ToList();
                               
                int contador = 4;
                foreach (AllDetailsContractForReportPDF_Result list in lista)
                {
                    worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                    worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                    worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                    worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                    worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                    worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                    worksheet.Cell("G" + contador).Value = list.Estado_Contrato;                   
                    worksheet.Cell("H" + contador).Value = list.Costo_Total.ToString();
                    worksheet.Cell("I" + contador).Value = list.Costo_Por_Cada_Pago;
                    worksheet.Cell("J" + contador).Value = list.Fecha_Renovacion_Pago;
                    worksheet.Cell("K" + contador).Value = list.Cantidad_Licencias.ToString();
                    worksheet.Cell("L" + contador).Value = list.Garantia;
                    worksheet.Cell("M" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                    worksheet.Cell("N" + contador).Value = list.Nombre_Unidad_Negocio;
                    worksheet.Cell("O" + contador).Value = list.Nombre_Metodo_Pago;
                    worksheet.Cell("P" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                    worksheet.Cell("Q" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                    worksheet.Cell("R" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                    worksheet.Cell("S" + contador).Value = list.Nombre_Tipo_Contrato;
                    worksheet.Cell("T" + contador).Value = list.Gastos;
                    worksheet.Cell("U" + contador).Value = list.Detalles_Producto;
                 
                    contador++;
                }

                contador = 0;
            }

            else
                if ((int)Session["UserPermission"] == 2 || (int)Session["UserPermission"] == 6)
                {
                    List<AllDetailsContractByAreaForReportPDF_Result> lista = detailsContract.ReturnAllDetailsContractByArea(2).ToList();                   

                    int contador = 4;
                    foreach (AllDetailsContractByAreaForReportPDF_Result list in lista)
                    {
                        worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                        worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                        worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                        worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                        worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                        worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                        worksheet.Cell("G" + contador).Value = list.Estado_Contrato;                    
                        worksheet.Cell("H" + contador).Value = list.Costo_Total.ToString();
                        worksheet.Cell("I" + contador).Value = list.Costo_Por_Cada_Pago;
                        worksheet.Cell("J" + contador).Value = list.Fecha_Renovacion_Pago;
                        worksheet.Cell("K" + contador).Value = list.Cantidad_Licencias.ToString();
                        worksheet.Cell("L" + contador).Value = list.Garantia;
                        worksheet.Cell("M" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                        worksheet.Cell("N" + contador).Value = list.Nombre_Unidad_Negocio;
                        worksheet.Cell("O" + contador).Value = list.Nombre_Metodo_Pago;
                        worksheet.Cell("P" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                        worksheet.Cell("Q" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                        worksheet.Cell("R" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                        worksheet.Cell("S" + contador).Value = list.Nombre_Tipo_Contrato;
                        worksheet.Cell("T" + contador).Value = list.Gastos;
                        worksheet.Cell("U" + contador).Value = list.Detalles_Producto;
                       

                        contador++;
                    }

                    contador = 0;
                }
                else
                    if ((int)Session["UserPermission"] == 3 || (int)Session["UserPermission"] == 7)
                    {
                        List<AllDetailsContractByAreaForReportPDF_Result> lista = detailsContract.ReturnAllDetailsContractByArea(1).ToList();

                        int contador = 4;
                        foreach (AllDetailsContractByAreaForReportPDF_Result list in lista)
                        {
                            worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                            worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                            worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                            worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                            worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                            worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                            worksheet.Cell("G" + contador).Value = list.Estado_Contrato;                          
                            worksheet.Cell("H" + contador).Value = list.Costo_Total.ToString();
                            worksheet.Cell("I" + contador).Value = list.Costo_Por_Cada_Pago;
                            worksheet.Cell("J" + contador).Value = list.Fecha_Renovacion_Pago;
                            worksheet.Cell("K" + contador).Value = list.Cantidad_Licencias.ToString();
                            worksheet.Cell("L" + contador).Value = list.Garantia;
                            worksheet.Cell("M" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                            worksheet.Cell("N" + contador).Value = list.Nombre_Unidad_Negocio;
                            worksheet.Cell("O" + contador).Value = list.Nombre_Metodo_Pago;
                            worksheet.Cell("P" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                            worksheet.Cell("Q" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                            worksheet.Cell("R" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                            worksheet.Cell("S" + contador).Value = list.Nombre_Tipo_Contrato;
                            worksheet.Cell("T" + contador).Value = list.Gastos;
                            worksheet.Cell("U" + contador).Value = list.Detalles_Producto;
                            

                            contador++;
                        }
                        contador = 0;
                    }

                    else
                        if ((int)Session["UserPermission"] == 4 || (int)Session["UserPermission"] == 8)
                        {
                            List<AllDetailsContractByAreaForReportPDF_Result> lista = detailsContract.ReturnAllDetailsContractByArea(3).ToList();

                            int contador = 4;
                            foreach (AllDetailsContractByAreaForReportPDF_Result list in lista)
                            {
                                worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                                worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                                worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                                worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                                worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                                worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                                worksheet.Cell("G" + contador).Value = list.Estado_Contrato;                          
                                worksheet.Cell("H" + contador).Value = list.Costo_Total.ToString();
                                worksheet.Cell("I" + contador).Value = list.Costo_Por_Cada_Pago;
                                worksheet.Cell("J" + contador).Value = list.Fecha_Renovacion_Pago;
                                worksheet.Cell("K" + contador).Value = list.Cantidad_Licencias.ToString();
                                worksheet.Cell("L" + contador).Value = list.Garantia;
                                worksheet.Cell("M" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                                worksheet.Cell("N" + contador).Value = list.Nombre_Unidad_Negocio;
                                worksheet.Cell("O" + contador).Value = list.Nombre_Metodo_Pago;
                                worksheet.Cell("P" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                                worksheet.Cell("Q" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                                worksheet.Cell("R" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                                worksheet.Cell("S" + contador).Value = list.Nombre_Tipo_Contrato;
                                worksheet.Cell("T" + contador).Value = list.Gastos;
                                worksheet.Cell("U" + contador).Value = list.Detalles_Producto;
                               

                                contador++;
                            }
                            contador = 0;
                        }
            worksheet.Columns().AdjustToContents();

            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string path = "C:\\Reportes\\ReportesTotalContratos\\Excel\\ReporteTotalContratos"+fechaActual+".xlsx";
            
            workbook.SaveAs(path);

            downloadExcel(path);
            return RedirectToRoute(new
            {
                controller = "Main",
                action = "main",

            });
        }

        public ActionResult GenerarExcelContratosActivos()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Contratos Activos");

            worksheet.Cell("A2").Value = "Name Contract";
            worksheet.Cell("B2").Value = "Contract number";
            worksheet.Cell("C2").Value = "Service Contract Number";
            worksheet.Cell("D2").Value = "Start date";
            worksheet.Cell("E2").Value = "End date";
            worksheet.Cell("F2").Value = "Payment type";
            worksheet.Cell("G2").Value = "Total cost";
            worksheet.Cell("H2").Value = "Cost for each payment";
            worksheet.Cell("I2").Value = "Renewal Payment Date";
            worksheet.Cell("J2").Value = "Number Licenses";
            worksheet.Cell("K2").Value = "Warranty";
            worksheet.Cell("L2").Value = "Guarantee Maturity Date";
            worksheet.Cell("M2").Value = "Business Unit Name";
            worksheet.Cell("N2").Value = "Payment method";
            worksheet.Cell("O2").Value = "Assigned Person Card";
            worksheet.Cell("P2").Value = "Last Digit Card";
            worksheet.Cell("Q2").Value = "Card Expiration Date";
            worksheet.Cell("R2").Value = "Contract type";
            worksheet.Cell("S2").Value = "Type Expense";
            worksheet.Cell("T2").Value = "Product name";
         
         
            AllDetailsActiveContractForPDF detailsContractActive = new AllDetailsActiveContractForPDF();
                    
            if ((int)Session["UserPermission"] == 1 || (int)Session["UserPermission"] == 5)
            {                               
                List<ActiveContractForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContract().ToList();
                
                int contador = 4;
                foreach (ActiveContractForReportPDF_Result list in lista)
                {
                    worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                    worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                    worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                    worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                    worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                    worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                    worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                    worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                    worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                    worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                    worksheet.Cell("K" + contador).Value = list.Garantia;
                    worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                    worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                    worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                    worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                    worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                    worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                    worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                    worksheet.Cell("S" + contador).Value = list.Gastos;
                    worksheet.Cell("T" + contador).Value = list.Detalles_Producto;
                    
                    contador++;
                }

                contador = 0;
            }

            else
                if ((int)Session["UserPermission"] == 2 || (int)Session["UserPermission"] == 6)
                {                    
                    List<ActiveContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(2).ToList();

                    int contador = 4;
                    foreach (ActiveContractByAreaForReportPDF_Result list in lista)
                    {
                        worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                        worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                        worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                        worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                        worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                        worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                        worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                        worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                        worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                        worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                        worksheet.Cell("K" + contador).Value = list.Garantia;
                        worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                        worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                        worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                        worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                        worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                        worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                        worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                        worksheet.Cell("S" + contador).Value = list.Gastos;
                        worksheet.Cell("T" + contador).Value = list.Detalles_Producto;

                        contador++;
                    }

                    contador = 0;
                }
                else
                    if ((int)Session["UserPermission"] == 3 || (int)Session["UserPermission"] == 7)
                    {                       
                        List<ActiveContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(1).ToList();

                        int contador = 4;
                        foreach (ActiveContractByAreaForReportPDF_Result list in lista)
                        {
                            worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                            worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                            worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                            worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                            worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                            worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                            worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                            worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                            worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                            worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                            worksheet.Cell("K" + contador).Value = list.Garantia;
                            worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                            worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                            worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                            worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                            worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                            worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                            worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                            worksheet.Cell("S" + contador).Value = list.Gastos;
                            worksheet.Cell("T" + contador).Value = list.Detalles_Producto;
                         

                            contador++;
                        }
                        contador = 0;
                    }
                    else
                        if ((int)Session["UserPermission"] == 4 || (int)Session["UserPermission"] == 8)
                        {
                            List<ActiveContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(3).ToList();

                            int contador = 4;
                            foreach (ActiveContractByAreaForReportPDF_Result list in lista)
                            {
                                worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                                worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                                worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                                worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                                worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                                worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                                worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                                worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                                worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                                worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                                worksheet.Cell("K" + contador).Value = list.Garantia;
                                worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                                worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                                worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                                worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                                worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                                worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                                worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                                worksheet.Cell("S" + contador).Value = list.Gastos;
                                worksheet.Cell("T" + contador).Value = list.Detalles_Producto;
                               

                                contador++;
                            }
                            contador = 0;
                        }
            worksheet.Columns().AdjustToContents();
            
            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string path = "C:\\Reportes\\ReportesContratosActivos\\Excel\\ReporteContratosActivos" + fechaActual + ".xlsx";          
            
            workbook.SaveAs(path);

            downloadExcel(path);
            return RedirectToRoute(new
            {
                controller = "Main",
                action = "main",

            });
        }

        public ActionResult GenerarExcelContratosProximosAVencer()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Contratos Proximos a Vencer");

            worksheet.Cell("A2").Value = "Name Contract";
            worksheet.Cell("B2").Value = "Contract number";
            worksheet.Cell("C2").Value = "Service Contract Number";
            worksheet.Cell("D2").Value = "Start date";
            worksheet.Cell("E2").Value = "End date";
            worksheet.Cell("F2").Value = "Payment type";
            worksheet.Cell("G2").Value = "Total cost";
            worksheet.Cell("H2").Value = "Cost for each payment";
            worksheet.Cell("I2").Value = "Renewal Payment Date";
            worksheet.Cell("J2").Value = "Number Licenses";
            worksheet.Cell("K2").Value = "Warranty";
            worksheet.Cell("L2").Value = "Guarantee Maturity Date";
            worksheet.Cell("M2").Value = "Business Unit Name";
            worksheet.Cell("N2").Value = "Payment method";
            worksheet.Cell("O2").Value = "Assigned Person Card";
            worksheet.Cell("P2").Value = "Last Digit Card";
            worksheet.Cell("Q2").Value = "Card Expiration Date";
            worksheet.Cell("R2").Value = "Contract type";
            worksheet.Cell("S2").Value = "Type Expense";
            worksheet.Cell("T2").Value = "Product name";
            

            AllDetailsContractAboutToFinishForPDF detailsContractActive = new AllDetailsContractAboutToFinishForPDF();

            if ((int)Session["UserPermission"] == 1 || (int)Session["UserPermission"] == 5)
            {
                List<AboutToFinishContractForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContract().ToList();              

                int contador = 4;
                foreach (AboutToFinishContractForReportPDF_Result list in lista)
                {
                    worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                    worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                    worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                    worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                    worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                    worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                    worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                    worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                    worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                    worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                    worksheet.Cell("K" + contador).Value = list.Garantia;
                    worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                    worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                    worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                    worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                    worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                    worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;                    
                    worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                    worksheet.Cell("S" + contador).Value = list.Gastos;
                    worksheet.Cell("T" + contador).Value = list.Detalles_Producto;

                    contador++;
                }

                contador = 0;
            }

            else
                if ((int)Session["UserPermission"] == 2 || (int)Session["UserPermission"] == 6)
                {
                    List<AboutToFinishContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(2).ToList();

                    int contador = 4;
                    foreach (AboutToFinishContractByAreaForReportPDF_Result list in lista)
                    {
                        worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                        worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                        worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                        worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                        worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                        worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                        worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                        worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                        worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                        worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                        worksheet.Cell("K" + contador).Value = list.Garantia;
                        worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                        worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                        worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                        worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                        worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                        worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                        worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                        worksheet.Cell("S" + contador).Value = list.Gastos;
                        worksheet.Cell("T" + contador).Value = list.Detalles_Producto;
                       

                        contador++;
                    }

                    contador = 0;
                }
                else
                    if ((int)Session["UserPermission"] == 3 || (int)Session["UserPermission"] == 7)
                    {
                        List<AboutToFinishContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(1).ToList();

                        int contador = 4;
                        foreach (AboutToFinishContractByAreaForReportPDF_Result list in lista)
                        {
                            worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                            worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                            worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                            worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                            worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                            worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                            worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                            worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                            worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                            worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                            worksheet.Cell("K" + contador).Value = list.Garantia;
                            worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                            worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                            worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                            worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                            worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                            worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                            worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                            worksheet.Cell("S" + contador).Value = list.Gastos;
                            worksheet.Cell("T" + contador).Value = list.Detalles_Producto;
                           

                            contador++;
                        }
                        contador = 0;
                    }
                    else
                        if ((int)Session["UserPermission"] == 4 || (int)Session["UserPermission"] == 8)
                        {
                            List<AboutToFinishContractByAreaForReportPDF_Result> lista = detailsContractActive.ReturnAllDetailsContractByArea(3).ToList();

                            int contador = 4;
                            foreach (AboutToFinishContractByAreaForReportPDF_Result list in lista)
                            {
                                worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                                worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                                worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                                worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                                worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                                worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                                worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                                worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                                worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                                worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                                worksheet.Cell("K" + contador).Value = list.Garantia;
                                worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                                worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                                worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                                worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                                worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                                worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                                worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                                worksheet.Cell("S" + contador).Value = list.Gastos;
                                worksheet.Cell("T" + contador).Value = list.Detalles_Producto;
                                

                                contador++;
                            }
                            contador = 0;
                        }
            worksheet.Columns().AdjustToContents();
                     
            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string path = "C:\\Reportes\\ReportesContratosProximosAVencer\\Excel\\ReporteContratosProximosAVencer" + fechaActual + ".xlsx";          
            
            workbook.SaveAs(path);

            downloadExcel(path);
            return RedirectToRoute(new
            {
                controller = "Main",
                action = "main",

            });
           
        }

        public ActionResult GenerarExcelContratosVencidos()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Contratos Vencidos");

            worksheet.Cell("A2").Value = "Name Contract";
            worksheet.Cell("B2").Value = "Contract number";
            worksheet.Cell("C2").Value = "Service Contract Number";
            worksheet.Cell("D2").Value = "Start date";
            worksheet.Cell("E2").Value = "End date";
            worksheet.Cell("F2").Value = "Payment type";
            worksheet.Cell("G2").Value = "Total cost";
            worksheet.Cell("H2").Value = "Cost for each payment";
            worksheet.Cell("I2").Value = "Renewal Payment Date";
            worksheet.Cell("J2").Value = "Number Licenses";
            worksheet.Cell("K2").Value = "Warranty";
            worksheet.Cell("L2").Value = "Guarantee Maturity Date";
            worksheet.Cell("M2").Value = "Business Unit Name";
            worksheet.Cell("N2").Value = "Payment method";
            worksheet.Cell("O2").Value = "Assigned Person Card";
            worksheet.Cell("P2").Value = "Last Digit Card";
            worksheet.Cell("Q2").Value = "Card Expiration Date";           
            worksheet.Cell("R2").Value = "Contract type";
            worksheet.Cell("S2").Value = "Type Expense";
            worksheet.Cell("T2").Value = "Product name";

            AllDetailsContractDefeatedForPDF detailsContractDefeated = new AllDetailsContractDefeatedForPDF();

            if ((int)Session["UserPermission"] == 1 || (int)Session["UserPermission"] == 5)
            {
                List<defeatedContractForReportPDF_Result> lista = detailsContractDefeated.ReturnAllDetailsContract().ToList();

                int contador = 4;
                foreach (defeatedContractForReportPDF_Result list in lista)
                {
                    worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                    worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                    worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                    worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                    worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                    worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                    worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                    worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                    worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                    worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                    worksheet.Cell("K" + contador).Value = list.Garantia;
                    worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                    worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                    worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                    worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                    worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                    worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                    worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                    worksheet.Cell("S" + contador).Value = list.Gastos;
                    worksheet.Cell("T" + contador).Value = list.Detalles_Producto;
                    

                    contador++;
                }

                contador = 0;
            }

            else
                if ((int)Session["UserPermission"] == 2 || (int)Session["UserPermission"] == 6)
                {
                    List<defeatedContractByAreaForReportPDF_Result> lista = detailsContractDefeated.ReturnAllDetailsContractByArea(2).ToList();

                    int contador = 4;
                    foreach (defeatedContractByAreaForReportPDF_Result list in lista)
                    {
                        worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                        worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                        worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                        worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                        worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                        worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                        worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                        worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                        worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                        worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                        worksheet.Cell("K" + contador).Value = list.Garantia;
                        worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                        worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                        worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                        worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                        worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                        worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                        worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                        worksheet.Cell("S" + contador).Value = list.Gastos;
                        worksheet.Cell("T" + contador).Value = list.Detalles_Producto;
                        

                        contador++;
                    }

                    contador = 0;
                }
                else
                    if ((int)Session["UserPermission"] == 3 || (int)Session["UserPermission"] == 7)
                    {
                        List<defeatedContractByAreaForReportPDF_Result> lista = detailsContractDefeated.ReturnAllDetailsContractByArea(1).ToList();
                        
                        int contador = 4;
                        foreach (defeatedContractByAreaForReportPDF_Result list in lista)
                        {
                            worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                            worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                            worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                            worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                            worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                            worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                            worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                            worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                            worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                            worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                            worksheet.Cell("K" + contador).Value = list.Garantia;
                            worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                            worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                            worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                            worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                            worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                            worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                            worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                            worksheet.Cell("S" + contador).Value = list.Gastos;
                            worksheet.Cell("T" + contador).Value = list.Detalles_Producto;
                            

                            contador++;
                        }
                        contador = 0;
                    }
                    else
                        if ((int)Session["UserPermission"] == 4 || (int)Session["UserPermission"] == 8)
                        {
                            List<defeatedContractByAreaForReportPDF_Result> lista = detailsContractDefeated.ReturnAllDetailsContractByArea(3).ToList();

                            int contador = 4;
                            foreach (defeatedContractByAreaForReportPDF_Result list in lista)
                            {
                                worksheet.Cell("A" + contador).Value = list.Nombre_Contrato;
                                worksheet.Cell("B" + contador).Value = list.Numero_Contrato;
                                worksheet.Cell("C" + contador).Value = list.Numero_Servicio_Contrato;
                                worksheet.Cell("D" + contador).Value = list.Fecha_Inicio.ToString("d");
                                worksheet.Cell("E" + contador).Value = list.Fecha_Finalizacion.ToString("d");
                                worksheet.Cell("F" + contador).Value = list.Nombre_Tipo_Pago;
                                worksheet.Cell("G" + contador).Value = list.Costo_Total.ToString();
                                worksheet.Cell("H" + contador).Value = list.Costo_Por_Cada_Pago;
                                worksheet.Cell("I" + contador).Value = list.Fecha_Renovacion_Pago;
                                worksheet.Cell("J" + contador).Value = list.Cantidad_Licencias.ToString();
                                worksheet.Cell("K" + contador).Value = list.Garantia;
                                worksheet.Cell("L" + contador).Value = list.Fecha_Vencimiento_Garantia.ToString("d");
                                worksheet.Cell("M" + contador).Value = list.Nombre_Unidad_Negocio;
                                worksheet.Cell("N" + contador).Value = list.Nombre_Metodo_Pago;
                                worksheet.Cell("O" + contador).Value = list.Nombre_Persona_Asignada_A_Tarjeta;
                                worksheet.Cell("P" + contador).Value = list.Ultimos_Digitos_Tarjeta;
                                worksheet.Cell("Q" + contador).Value = list.Fecha_Vencimiento_Tarjeta;
                                worksheet.Cell("R" + contador).Value = list.Nombre_Tipo_Contrato;
                                worksheet.Cell("S" + contador).Value = list.Gastos;
                                worksheet.Cell("T" + contador).Value = list.Detalles_Producto;
                                

                                contador++;
                            }
                            contador = 0;
                        }
            worksheet.Columns().AdjustToContents();

            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string path = "C:\\Reportes\\ReportesContratosVencidos\\Excel\\ReporteContratosVencidos" + fechaActual + ".xlsx";          
            
            workbook.SaveAs(path);
            
            downloadExcel(path);
            return RedirectToRoute(new
            {
                controller = "Main",
                action = "main",
               
            });          
        }

        [HttpGet]
        public void downloadAdjuntos(string path)
        {
            string[] mime = path.Split('.');
            string mimeFinal="";

            if(mime[1] == "pdf"){ mimeFinal = "application/pdf";}

            else
             if(mime[1] == "doc"){mimeFinal = "application/msword";}

            else
             if (mime[1] == "docx") { mimeFinal = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
            
            else
             if(mime[1] == "xls"){mimeFinal = "application/vnd.ms-excel";}

            else
             if (mime[1] == "xlsx") { mimeFinal = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }

            else
             if (mime[1] == "jpg" ||mime[1] =="jpeg" ||mime[1] =="jpe" ) { mimeFinal = "image/jpeg"; }

            else
             if (mime[1] == "png") { mimeFinal = "image/png"; }

            else
             if (mime[1] == "zip") { mimeFinal = "multipart/x-zip"; }

            else
             if (mime[1] == "tar") { mimeFinal = "application/x-tar"; }

            else
             if (mime[1] == "rar") { mimeFinal = "application/x-rar"; }
        
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = mimeFinal;
            Response.Flush();
            Response.TransmitFile(path);
            Response.End();            
        }


        private void downloadExcel(string path)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Flush();
            Response.TransmitFile(path);
            Response.End();
            
        }

        private void download(string path)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.Flush();
            Response.TransmitFile(path);
            Response.End();
             
        }
    }
}

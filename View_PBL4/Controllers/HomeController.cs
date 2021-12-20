using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Mvc;
using View_PBL4.BLL;
using View_PBL4.Models;

namespace View_PBL4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(BLL_Controller.Instance.LoadFolder());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult User(string id, string name)
        {
            System.Diagnostics.Debug.WriteLine(id);
            name = Request["ten"];
            System.Diagnostics.Debug.WriteLine(name);
            Server_Socket.Server_Socket.AcceptRequestFromWeb(name,id);
            /*if (id == "shudown")
            { 
                
            
            }
            if (id == "reset")
            { 
            
            
            }
            if(id == "sendimage")
            { 
            
            
            }
            if(id == "null")
            {
            
            
            }*/
            return View(BLL_Controller.Instance.LoadUser());
        }
        public ActionResult Data_Mode()
        {
            return View(BLL_Controller.Instance.LoadUser());
        }
        public ActionResult Image_View(string id)
        {
            System.Diagnostics.Debug.WriteLine(id);
            return View(BLL_Controller.Instance.Load_Img_Txt(id));
        }
        public ActionResult Text_View(string id)
        {
            return View(BLL_Controller.Instance.Load_Img_Txt(id));
        }
        public ActionResult Change_Folder(string id)
        {
            return View(BLL_Controller.Instance.Cheange_Img(id));
        }
        public ActionResult Change_Folder_txt(string id)
        {
            return View(BLL_Controller.Instance.Cheange_txt(id));
        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Feedback.Models;


namespace Feedback.Controllers
{
    public class HomeController : Controller
    {
        feedbackSystemDxcEntities entities = new feedbackSystemDxcEntities();

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Chapters chapters)
        {
            Session["username"] = chapters.Cname;
            Session["cid"] = chapters.Cid;
            List<Chapters> chap = entities.Chapters.ToList();
            Chapters temp = chap.FirstOrDefault(x => x.Cname == chapters.Cname);
            //int cid = temp.Cid;
            if (temp != null)
            {
                if (temp.Cpassword.Equals(chapters.Cpassword))
                {
                    FormsAuthentication.SetAuthCookie(chapters.Cname, false);
                    @ViewBag.result = "Login Successful";
                    return RedirectToAction("Userpage",new { cid=temp.Cid});

                }
                else
                {
                    @ViewBag.result = "Wrong Password";
                    return View();

                }
            }
            else
            {
                @ViewBag.result = "Wrong Username and Password";
                return View();

            }
        }
        public ActionResult Logout()
        {
            
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult UserPage(int cid,string searchVal,string Sortby)
        {
            List<Feedbacks> fb = entities.Feedbacks.ToList();
            IEnumerable<Feedbacks> fb1 = fb.Where(x => x.Products.Cid == cid);
            if (!String.IsNullOrEmpty(searchVal))
            {
                fb1 = fb1.Where(x => x.Customers.Organization.ToLower().Contains(searchVal.ToLower()) | x.Products.Pname.ToLower().Contains(searchVal.ToLower()));
            }

            if (!String.IsNullOrEmpty(Sortby))
            {
                switch (int.Parse(Sortby))
                {
                    case 0: fb1 = fb1.OrderByDescending(x => x.Customers.Dov);break;
                    case 1: fb1 = fb1.OrderBy(x => x.Customers.Dov); break;
                    case 2: fb1 = fb1.OrderByDescending(x => x.avg); break;
                    case 3: fb1 = fb1.OrderBy(x => x.avg); break;
                    default: break;
                }
            }
            return View(fb1);
        }

        public ActionResult Details(int id) {
            Feedbacks fb = entities.Feedbacks.FirstOrDefault(x => x.Fid == id);
            Chapters temp = entities.Chapters.FirstOrDefault(x => x.Cid == fb.Products.Cid);
            return View(fb);
        }

    }
}

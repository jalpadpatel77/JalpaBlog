 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using JalpaBlog.Models;
using PagedList;
using PagedList.Mvc;


namespace JalpaBlog.Controllers
{
   
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //[Authorize(Roles = "Admin")]

        //[Authorize(Roles = "Admin,Moderator")]
        public ActionResult Index(int? page)
        {
            int pageSize = 4;
            int pageNumber = page ?? 1;
            var publishedBlogPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize);
            return View(publishedBlogPosts);
        }
       
        //public ActionResult unpublishedIndex()
        //{
         //   var publishedBlogPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(b => b.Created).ToList();
         //   return View("Index",publishedBlogPosts);
       // }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();
            return View(model);
           // ViewBag.Message = "Your contact page.";
           // return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p >";
                   var from = $"{model.FromEmail}<jalpadpatel77@gmail.com>";
                    //model.Body = "This is a message from your portfolio site.  The name and the email of the contacting person is above.";

                    var email = new MailMessage(from, WebConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = model.Subject,
                        Body = model.Body,
                        IsBodyHtml = true
                    };
                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }


    }
}
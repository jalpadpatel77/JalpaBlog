using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JalpaBlog.Models;
using JalpaBlog.Utilities;
using PagedList;
using PagedList.Mvc;

namespace JalpaBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    //[Authorize(Roles = "Admin,Moderator")]
    [RequireHttps]
    public class BlogPostsController : Controller
    {
       
        private ApplicationDbContext db = new ApplicationDbContext();
       //private object image;
        
        public ActionResult AdminIndex()
        {
            return View("Index", db.BlogPosts.ToList());
        }
        //Inside the controller we create  some methods
        //every public method of the controller is action method

        // GET: BlogPosts
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);
            int pageSize = 3;
            int pageNumber = page ?? 1;
            
//var publishedBlogPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(b => b.Created).ToList();
            return View(blogList.ToPagedList(pageNumber, pageSize));
        }

     // private object IndexSearch(string searchStr)
      //  {
       //    throw new NotImplementedException();
       // }

        //post: Iquearble/ Index

        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                                           p.Body.Contains(searchStr) ||
                                           p.Comments.Any(c => c.Body.Contains(searchStr) ||
                                           c.Author.FirstName.Contains(searchStr) ||
                                           c.Author.LastName.Contains(searchStr) ||
                                           c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }
            return result.OrderByDescending(p => p.Created);
        }
                   
        
        // GET: BlogPosts/Details/
        [AllowAnonymous]
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))  //if nothing for slug
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);//
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create

       //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //whatever is in the bind goes into the post- bind is like exclutionary prpocess, if it's not in bind it's not coming in
              public ActionResult Create([Bind(Include = "Title,Abstract,MediaURL,Body,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaUrl = "/Uploads/" + fileName;
                }

                var Slug = StringUtilities.SlugMaker(blogPost.Title);   //create slug
                                
                //No guarantee that we can use slug bcuz it is empty
                if (string.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);

                }

                //If the slug is already pressent in the db. its bad
                if (db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique"); // "Title" name of property it is targeting
                    return View(blogPost);                // passing back into create view
                }
   
                //otherwise slug is good
                blogPost.Slug = Slug;            // sets slug the property value to slug
                blogPost.Created = DateTimeOffset.Now; //sets the time automatically
                                             
                db.BlogPosts.Add(blogPost); // data we created is added 
                db.SaveChanges(); // save changes
                return RedirectToAction("Index"); // takes user to index action-blogpost controller
            }

            return View(blogPost);  //If anything bad return to blogPost
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Abstract,Slug,Body,MediaUrl,Published,Created,Updated")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaUrl = "/Uploads/" + fileName;
                }

                var newSlug = StringUtilities.SlugMaker(blogPost.Title);
                if (newSlug != blogPost.Slug)
                {

                    if (String.IsNullOrWhiteSpace(newSlug))
                    {
                        ModelState.AddModelError("Title", "Invalid title");
                        return View(blogPost);

                    }

                    //If the slug is already pressent in the db. its bad
                    if (db.BlogPosts.Any(p => p.Slug == newSlug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique");
                        return View(blogPost);
                    }

                   

                    blogPost.Slug = newSlug;
                    
                }
               
                //db.Entry(blogPost).State = EntityState.Modified;
                blogPost.Updated= DateTimeOffset.Now;
                db.BlogPosts.Add(blogPost);
                    db.SaveChanges();
                                
                return RedirectToAction("Index");
             
            }
             
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

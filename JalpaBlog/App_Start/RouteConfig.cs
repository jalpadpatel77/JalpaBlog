using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JalpaBlog
{
    public class RouteConfig        
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //to Navigate post
            routes.MapRoute(
                name: "NewSlug",
                url: "Blog/{slug}",
                defaults: new
                {
                    controller = "BlogPosts",
                    action = "Details",
                    slug = UrlParameter.Optional
                });
            routes.MapRoute(       // maping for the incoming  url
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

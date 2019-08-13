using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JalpaBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int  BlogPostId { get; set; }
        public string AuthorId { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string UpdateReason { get; set; }
            //Virtual Navigation Section
        public virtual BlogPost BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }



    }
}
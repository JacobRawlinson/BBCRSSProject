using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace RSSProject.Models
{
    public class Feed
    {
        public String Title { get; set; }
        public String Link { get; set; } 
        public String Description { get; set; }
        public String Date { get; set; }
        public String ImageURL { get; set; }
    }
}
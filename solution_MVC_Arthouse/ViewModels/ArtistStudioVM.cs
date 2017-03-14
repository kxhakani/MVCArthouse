using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace solution_MVC_Arthouse.ViewModels
{
    public class ArtistStudioVM
    {
        public int StudioID { get; set; }
        public string StudioName { get; set; }
        public bool Assigned { get; set; }
    }
}
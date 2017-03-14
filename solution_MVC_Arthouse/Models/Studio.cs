using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace solution_MVC_Arthouse.Models
{
    public class Studio
    {
        public int ID { get; set; }

        [Display(Name = "Studio")]
        [Required(ErrorMessage = "You cannot leave the name of the Studio blank.")]
        [StringLength(10, ErrorMessage = "Only 10 Characters Allowed")]
        [Index("IX_Unique_Studio",IsUnique =true)]
        public string StudioName { get; set; }

        public virtual ICollection<Artist> Artists { get; set; }
    }
}
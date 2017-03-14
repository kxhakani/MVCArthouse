using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace solution_MVC_Arthouse.Models
{
    public class ArtType
    {
        public ArtType()
        {
            this.Artworks = new HashSet<Artwork>();
        }
        public int ID { get; set; }

        [Display(Name = "Art Type")]
        [Required(ErrorMessage = "You cannot leave the art type name black.")]
        [StringLength(25, ErrorMessage = "Art type cannot be more than 25 characters long.")]
        public string Type { get; set; }

        public virtual ICollection<Artwork> Artworks { get; set; }
    }
}
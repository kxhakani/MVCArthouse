using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace solution_MVC_Arthouse.Models
{
    public class UploadedFiles
    { 
        public int id { get; set; }

        [Required]
        public byte[] FileContent { get; set; }

        [Required]
        [StringLength(256)]
        public string MimeType { get; set; }

        [Required(ErrorMessage = "You cannot leave the nameof the file blank")]
        [StringLength(255, ErrorMessage = "The name of the file cannot be more than 255 characters")]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [StringLength(100, ErrorMessage = "The file description cannot be more than 100 characters")]
        [Display(Name = "Description")]
        public string FileDescription { get; set; }

        public int ArtworkID { get; set; }

        public virtual Artwork Artwork { get; set; }

    }
}
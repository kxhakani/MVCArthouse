using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace solution_MVC_Arthouse.Models
{
    public class Artwork : Auditable
    {

        public Artwork()
        {
            this.UploadedFiles = new HashSet<UploadedFiles>();
        }


        public int ID { get; set; }

        [Display(Name="Artwork (Completed)")]
        public string Summary
        {
            get
            {
                return Name + 
                    (Finished.HasValue ? (" - " + Finished.GetValueOrDefault().ToShortDateString()) :
                        " - Unfinished");
            }
        }

        [Display(Name = "Name or Title")]
        [Required(ErrorMessage = "You cannot leave the artwork's name blank.")]
        [StringLength(255, ErrorMessage = "First name cannot be more than 255 characters long.")]
        [Index("IX_Unique_Artwork", IsUnique=true, Order=1)]
        public string Name { get; set; }

        [Required(ErrorMessage = "You cannot leave the start date blank.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Started")]
        [Index("IX_Unique_Artwork", IsUnique = true, Order = 2)]
        public DateTime Started { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true, NullDisplayText = "Unfinished")]
        [Display(Name = "Date Finished")]
        public DateTime? Finished { get; set; }

        [Required(ErrorMessage = "You cannot leave the description of the artwork blank.")]
        [StringLength(511,MinimumLength =20, ErrorMessage ="Description must be a least 20 characters and no more than 511.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Estimated Value")]
        [Required(ErrorMessage = "You cannot leave the estimated value of the artwork blank.")]
        [Range(1.00, 999000.00, ErrorMessage ="Estimated value must be between one and 999 thousand dollars.")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = false)]
        public decimal Value { get; set; }

        [ScaffoldColumn (false)]
        public byte[] imageContent { get; set; }

        [ScaffoldColumn (false)]
        [StringLength(256)]
        public string imageMimeType { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(100, ErrorMessage = "The name of the file cannot be more than 100 characters!")]
        [Display(Name = "File Name")]
        public string imageFileName { get; set; }

        // foreign key for Artist table
        [Required(ErrorMessage ="You must select the Artist.")]
        public int ArtistID { get; set; }
        public virtual Artist Artist { get; set; }

        // foreign key for Art Type table
        [Required(ErrorMessage = "Please identify the Type of art.")]
        public int ArtTypeID { get; set; }
        public virtual ArtType ArtType { get; set; }

        public ICollection<UploadedFiles> UploadedFiles { get; set; }


    }
}
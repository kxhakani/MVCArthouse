using solution_MVC_Arthouse.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace solution_MVC_Arthouse.DAL.AHEntities
{
    //USE THIS COMMAND TO ENABLE MIGRAITONS in the Package Manager Console
    //Enable-Migrations -ContextTypeName ArthouseEntities -MigrationsDirectory DAL\AHEntities\Migrations
     
    public class ArthouseEntities : DbContext
    {
        public ArthouseEntities() : base("name=ArthouseEntities")
        {
            // SHOW ME THE MONEY... (Or at least the SQL)
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<ArtType> ArtTypes { get; set; }
        public DbSet<Studio> Studios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //This option keeps table names in singular form, my personal preference.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
        public override int SaveChanges()
        {
            //Get Audit Values if not supplied
            string auditUser = "Unknown";
            try //Need to try becuase HttpContext might not exist
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                    auditUser = HttpContext.Current.User.Identity.Name;
            }
            catch (Exception)
            { }

            DateTime auditDate = DateTime.UtcNow;
            foreach (DbEntityEntry<IAuditable> entry in ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = auditDate;
                    entry.Entity.CreatedBy = auditUser;
                    entry.Entity.UpdatedOn = auditDate;
                    entry.Entity.UpdatedBy = auditUser;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedOn = auditDate;
                    entry.Entity.UpdatedBy = auditUser;
                }
            }
            return base.SaveChanges();
        }

        public System.Data.Entity.DbSet<solution_MVC_Arthouse.Models.UploadedFiles> UploadedFiles { get; set; }
    }
}
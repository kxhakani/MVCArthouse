namespace solution_MVC_Arthouse.DAL.AHEntities.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<solution_MVC_Arthouse.DAL.AHEntities.ArthouseEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DAL\AHEntities\Migrations";
        }

        /// Wrapper for SaveChanges adding the Validation Messages to the generated exception
        /// </summary>
        /// <param name="context">The context.</param>
        /// Just replace calls to context.SaveChanges() with SaveChanges(context) in your seed method.
        private void SaveChanges(DbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
            catch (Exception e)
            {
                throw new Exception(
                     "Seed Failed - errors follow:\n" +
                     e.InnerException.InnerException.Message.ToString(), e
                 ); // Add the original exception as the innerException
            }
        }

        protected override void Seed(solution_MVC_Arthouse.DAL.AHEntities.ArthouseEntities context)
        {
            //  REMEMBER REFERENTIAL INTEGRITY RULES... THE ORDER MATTERS!

            var artists = new List<Artist>
            {
                new Artist { FirstName = "Gregory", MiddleName = "A",  LastName = "House",
                    Phone =9055551212, DOB=DateTime.Parse("1955-09-01"), Rate=40},
                new Artist { FirstName = "Victor", LastName = "Frankenstein",
                    Phone =9055551234, DOB=DateTime.Parse("1945-05-05"), Rate=10,
                 Studios =new List<Studio> {
                    new Models.Studio { StudioName="Studio A"},
                    new Models.Studio { StudioName="Studio B"},
                    new Models.Studio { StudioName="Studio C"}}},
                new Artist { FirstName = "Fred", MiddleName = "A",  LastName = "Flintstone",
                    Phone =2895554321, DOB=DateTime.Parse("1995-12-01"), Rate=40,
                    Studios =new List<Studio> {
                    new Models.Studio { StudioName="Studio D"},
                    new Models.Studio { StudioName="Studio E"}} }
            };
            artists.ForEach(a => context.Artists.AddOrUpdate(n => n.LastName, a));
            SaveChanges(context);

            var arttypes = new List<ArtType>
            {
                new ArtType { Type="Painting"},
                new ArtType { Type="Drawing"},
                new ArtType { Type="Sculpture"},
                new ArtType { Type="Plastic Art"},
                new ArtType { Type="Decorative Art"},
                new ArtType { Type="Visual Art"}
            };
            arttypes.ForEach(a => context.ArtTypes.AddOrUpdate(n => n.Type, a));
            SaveChanges(context);

            var artworks = new List<Artwork>
            {
                new Artwork { Name="Red Dot", Value=12000m,
                    Started=DateTime.Parse("1999-12-01"),
                    Finished=DateTime.Parse("2002-06-06"),
                    Description="Painting of a large Red Dot on a white backgraound.",
                    ArtTypeID =(context.ArtTypes.Where(d=>d.Type=="Painting").SingleOrDefault().ID),
                    ArtistID=(context.Artists.Where(a=>a.LastName=="Flintstone").SingleOrDefault().ID) },
                new Artwork { Name="Rossini Regal", Value=99000m,
                    Started=DateTime.Parse("2009-12-01"),
                    Finished=DateTime.Parse("2009-12-06"),
                    Description="Photograph of a regal horse.",
                    ArtTypeID =(context.ArtTypes.Where(d=>d.Type=="Visual Art").SingleOrDefault().ID),
                    ArtistID=(context.Artists.Where(a=>a.LastName=="House").SingleOrDefault().ID) },
                new Artwork { Name="Love Sublime", Value=19.99m,
                    Started=DateTime.Parse("2014-07-11"),
                    Finished=DateTime.Parse("2015-09-21"),
                    Description="Soapstone Sculpture of woman's face gazing at an unknown figure.",
                    ArtTypeID =(context.ArtTypes.Where(d=>d.Type=="Sculpture").SingleOrDefault().ID),
                    ArtistID = (context.Artists.Where(a => a.LastName == "Flintstone").SingleOrDefault().ID) },
                new Artwork { Name="Igor Smashes", Value=750000.50m,
                    Started=DateTime.Parse("1976-07-11"),
                    Description="Abstract concept of smashed emotion done in crumpled paper.",
                    ArtTypeID =(context.ArtTypes.Where(d=>d.Type=="Plastic Art").SingleOrDefault().ID),
                    ArtistID = (context.Artists.Where(a => a.LastName == "Frankenstein").SingleOrDefault().ID) }
            };
            artworks.ForEach(a => context.Artworks.AddOrUpdate(n => n.Name, a));
            SaveChanges(context);
        }
    }
}

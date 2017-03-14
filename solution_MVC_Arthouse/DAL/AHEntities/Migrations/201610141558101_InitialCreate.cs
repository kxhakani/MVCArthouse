namespace solution_MVC_Arthouse.DAL.AHEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Artist",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        MiddleName = c.String(maxLength: 30),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Phone = c.Long(nullable: false),
                        DOB = c.DateTime(nullable: false),
                        Rate = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Artwork",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Started = c.DateTime(nullable: false),
                        Finished = c.DateTime(),
                        Description = c.String(nullable: false, maxLength: 511),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ArtistID = c.Int(nullable: false),
                        ArtTypeID = c.Int(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Artist", t => t.ArtistID)
                .ForeignKey("dbo.ArtType", t => t.ArtTypeID)
                .Index(t => new { t.Name, t.Started }, unique: true, name: "IX_Unique_Artwork")
                .Index(t => t.ArtistID)
                .Index(t => t.ArtTypeID);
            
            CreateTable(
                "dbo.ArtType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false, maxLength: 25),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Artwork", "ArtTypeID", "dbo.ArtType");
            DropForeignKey("dbo.Artwork", "ArtistID", "dbo.Artist");
            DropIndex("dbo.Artwork", new[] { "ArtTypeID" });
            DropIndex("dbo.Artwork", new[] { "ArtistID" });
            DropIndex("dbo.Artwork", "IX_Unique_Artwork");
            DropTable("dbo.ArtType");
            DropTable("dbo.Artwork");
            DropTable("dbo.Artist");
        }
    }
}

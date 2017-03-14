namespace solution_MVC_Arthouse.DAL.AHEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArtworkFiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UploadedFiles",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        FileContent = c.Binary(nullable: false),
                        MimeType = c.String(nullable: false, maxLength: 256),
                        FileName = c.String(nullable: false, maxLength: 255),
                        FileDescription = c.String(maxLength: 100),
                        ArtworkID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Artwork", t => t.ArtworkID)
                .Index(t => t.ArtworkID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UploadedFiles", "ArtworkID", "dbo.Artwork");
            DropIndex("dbo.UploadedFiles", new[] { "ArtworkID" });
            DropTable("dbo.UploadedFiles");
        }
    }
}

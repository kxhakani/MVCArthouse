namespace solution_MVC_Arthouse.DAL.AHEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MtoMStudio : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Studio",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StudioName = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.StudioName, unique: true, name: "IX_Unique_Studio");
            
            CreateTable(
                "dbo.StudioArtist",
                c => new
                    {
                        Studio_ID = c.Int(nullable: false),
                        Artist_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Studio_ID, t.Artist_ID })
                .ForeignKey("dbo.Studio", t => t.Studio_ID, cascadeDelete: true)
                .ForeignKey("dbo.Artist", t => t.Artist_ID, cascadeDelete: true)
                .Index(t => t.Studio_ID)
                .Index(t => t.Artist_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudioArtist", "Artist_ID", "dbo.Artist");
            DropForeignKey("dbo.StudioArtist", "Studio_ID", "dbo.Studio");
            DropIndex("dbo.StudioArtist", new[] { "Artist_ID" });
            DropIndex("dbo.StudioArtist", new[] { "Studio_ID" });
            DropIndex("dbo.Studio", "IX_Unique_Studio");
            DropTable("dbo.StudioArtist");
            DropTable("dbo.Studio");
        }
    }
}

namespace solution_MVC_Arthouse.DAL.AHEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArtworkPicture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Artwork", "imageContent", c => c.Binary());
            AddColumn("dbo.Artwork", "imageMimeType", c => c.String(maxLength: 256));
            AddColumn("dbo.Artwork", "imageFileName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Artist", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.Artist", "UpdatedOn", c => c.DateTime());
            AlterColumn("dbo.Artwork", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.Artwork", "UpdatedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Artwork", "UpdatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Artwork", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Artist", "UpdatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Artist", "CreatedOn", c => c.DateTime(nullable: false));
            DropColumn("dbo.Artwork", "imageFileName");
            DropColumn("dbo.Artwork", "imageMimeType");
            DropColumn("dbo.Artwork", "imageContent");
        }
    }
}

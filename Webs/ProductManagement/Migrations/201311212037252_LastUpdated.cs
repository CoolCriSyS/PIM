namespace ProductManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pattern", "LastUpdated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Style", "LastUpdated", c => c.DateTime(nullable: false));
            DropColumn("dbo.Pattern", "Timestamp");
            DropColumn("dbo.Style", "Timestamp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Style", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Pattern", "Timestamp", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            DropColumn("dbo.Style", "LastUpdated");
            DropColumn("dbo.Pattern", "LastUpdated");
        }
    }
}

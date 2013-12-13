namespace ProductManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Brand",
                c => new
                    {
                        BrandId = c.Int(nullable: false, identity: true),
                        BrandName = c.String(nullable: false),
                        SalesOrg = c.String(nullable: false, maxLength: 4),
                        DistChan = c.String(nullable: false, maxLength: 2),
                    })
                .PrimaryKey(t => t.BrandId);
            
            CreateTable(
                "dbo.Pattern",
                c => new
                    {
                        PatternId = c.Int(nullable: false, identity: true),
                        PatternName = c.String(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        BrandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PatternId);
            
            CreateTable(
                "dbo.Style",
                c => new
                    {
                        StyleId = c.Int(nullable: false, identity: true),
                        StockNumber = c.String(nullable: false),
                        MarketingDescription = c.String(maxLength: 4000),
                        TechBullets = c.String(maxLength: 4000),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        PatternId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StyleId)
                .ForeignKey("dbo.Pattern", t => t.PatternId, cascadeDelete: true)
                .Index(t => t.PatternId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Style", new[] { "PatternId" });
            DropForeignKey("dbo.Style", "PatternId", "dbo.Pattern");
            DropTable("dbo.Style");
            DropTable("dbo.Pattern");
            DropTable("dbo.Brand");
            DropTable("dbo.UserProfile");
        }
    }
}

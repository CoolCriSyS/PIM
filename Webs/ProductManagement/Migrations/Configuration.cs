namespace ProductManagement.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<ProductManagement.Models.PIMContext>
    {
        private readonly bool _pendingMigrations;

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            var migrator = new DbMigrator(this);
            _pendingMigrations = migrator.GetPendingMigrations().Any();
        }

        protected override void Seed(ProductManagement.Models.PIMContext context)
        {
            //  This method will be called after migrating to the latest version.

            if (!_pendingMigrations) return;
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            WebSecurity.InitializeDatabaseConnection(
                "PIMdb",
                "UserProfile",
                "UserId",
                "UserName", autoCreateTables: true);

            if (!Roles.RoleExists("Administrator"))
                Roles.CreateRole("Administrator");

            if (!WebSecurity.UserExists("westovma"))
                WebSecurity.CreateUserAndAccount(
                    "westovma",
                    "$ecure21",
                    new { Email = "westovma@wwwinc.com" });

            if (!Roles.GetRolesForUser("westovma").Contains("Administrator"))
                Roles.AddUsersToRoles(new[] { "westovma" }, new[] { "Administrator" });

            if (context.Brands.Count() == 0)
                context.Brands.AddOrUpdate(
                    new ProductManagement.Models.Brand { BrandName = "Bates", SalesOrg = "1000", DistChan = "09" },
                    new ProductManagement.Models.Brand { BrandName = "Cat Footwear", SalesOrg = "1000", DistChan = "10" },
                    new ProductManagement.Models.Brand { BrandName = "Chaco", SalesOrg = "3000", DistChan = "38" },
                    new ProductManagement.Models.Brand { BrandName = "Cushe", SalesOrg = "1000", DistChan = "18" },
                    new ProductManagement.Models.Brand { BrandName = "Harley-Davidson", SalesOrg = "1000", DistChan = "15" },
                    new ProductManagement.Models.Brand { BrandName = "Hush Puppies", SalesOrg = "1000", DistChan = "02" },
                    new ProductManagement.Models.Brand { BrandName = "HyTest", SalesOrg = "1000", DistChan = "20" },
                    new ProductManagement.Models.Brand { BrandName = "Merrell Apparel", SalesOrg = "3000", DistChan = "AA" },
                    new ProductManagement.Models.Brand { BrandName = "Merrell Footwear", SalesOrg = "3000", DistChan = "30" },
                    new ProductManagement.Models.Brand { BrandName = "Patagonia", SalesOrg = "3000", DistChan = "85" },
                    new ProductManagement.Models.Brand { BrandName = "Sebago Apparel", SalesOrg = "1700", DistChan = "AG" },
                    new ProductManagement.Models.Brand { BrandName = "Sebago Footwear", SalesOrg = "1700", DistChan = "80" },
                    new ProductManagement.Models.Brand { BrandName = "Wolverine Apparel", SalesOrg = "1000", DistChan = "AD" },
                    new ProductManagement.Models.Brand { BrandName = "Wolverine Footwear", SalesOrg = "1000", DistChan = "06" });
        }
    }
}

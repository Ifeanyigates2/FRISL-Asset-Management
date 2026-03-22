using FrislEams.Web.Models;

namespace FrislEams.Web.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (!db.AssetCategories.Any())
        {
            db.AssetCategories.AddRange(
                new AssetCategory { Name = "Laptop", Code = "LAP", UsefulLifeYears = 4 },
                new AssetCategory { Name = "Furniture", Code = "FUR", UsefulLifeYears = 8 },
                new AssetCategory { Name = "Vehicle", Code = "VEH", UsefulLifeYears = 6 }
            );
        }

        if (!db.Locations.Any())
        {
            db.Locations.AddRange(
                new Location { Name = "Admin Storage", Code = "STORAGE" },
                new Location { Name = "HQ Abuja", Code = "ABJ" },
                new Location { Name = "Ibadan Branch", Code = "IBA" }
            );
        }

        if (!db.Departments.Any())
        {
            db.Departments.AddRange(
                new Department { Name = "Information Technology", Code = "IT" },
                new Department { Name = "Finance", Code = "FIN" },
                new Department { Name = "Operations", Code = "OPS" }
            );
        }

        db.SaveChanges();

        if (!db.Staff.Any())
        {
            var it = db.Departments.First(d => d.Code == "IT").Id;
            var fin = db.Departments.First(d => d.Code == "FIN").Id;
            db.Staff.AddRange(
                new Staff { FullName = "Admin User", Email = "admin@frisl.local", DepartmentId = it },
                new Staff { FullName = "Aisha Bello", Email = "aisha@frisl.local", DepartmentId = fin },
                new Staff { FullName = "David Musa", Email = "david@frisl.local", DepartmentId = it }
            );
        }

        if (!db.Suppliers.Any())
        {
            db.Suppliers.AddRange(
                new Supplier { Name = "Prime Tech", Code = "SUP-PT" },
                new Supplier { Name = "Office Mart", Code = "SUP-OM" }
            );
        }

        db.SaveChanges();
    }
}

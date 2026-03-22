using System.Text;
using FrislEams.Web.Data;
using FrislEams.Web.Domain;
using FrislEams.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FrislEams.Web.Services;

public class ReportingService(AppDbContext db)
{
    public async Task<List<Asset>> GetAssetsReportAsync(ReportFilterVm filter)
    {
        var query = db.Assets
            .Include(a => a.AssetCategory)
            .Include(a => a.CurrentDepartment)
            .Include(a => a.CurrentLocation)
            .AsQueryable();

        if (filter.FromDate.HasValue)
        {
            query = query.Where(a => a.PurchaseDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(a => a.PurchaseDate <= filter.ToDate.Value);
        }

        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(a => a.CurrentDepartmentId == filter.DepartmentId.Value);
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(a => a.AssetCategoryId == filter.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<AssetStatus>(filter.Status, out var status))
        {
            query = query.Where(a => a.CurrentStatus == status);
        }

        return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    public string BuildAssetsCsv(List<Asset> assets)
    {
        var sb = new StringBuilder();
        sb.AppendLine("TagCode,AssetName,Category,Department,Location,Status,PurchaseDate,PurchaseCost");

        foreach (var a in assets)
        {
            var line = string.Join(",",
                Csv(a.TagCode),
                Csv(a.AssetName),
                Csv(a.AssetCategory?.Name),
                Csv(a.CurrentDepartment?.Name),
                Csv(a.CurrentLocation?.Name),
                Csv(a.CurrentStatus.ToString()),
                Csv(a.PurchaseDate?.ToString("yyyy-MM-dd")),
                Csv(a.PurchaseCost?.ToString("0.00"))
            );
            sb.AppendLine(line);
        }

        return sb.ToString();
    }

    public async Task<Dictionary<string, int>> GetAssetCountByCategoryAsync()
    {
        return await db.Assets
            .Include(a => a.AssetCategory)
            .GroupBy(a => a.AssetCategory!.Name)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Count);
    }

    public async Task<List<DepreciationRowVm>> GetDepreciationReportAsync()
    {
        var assets = await db.Assets
            .Include(a => a.AssetCategory)
            .Where(a => a.PurchaseDate.HasValue && a.PurchaseCost.HasValue && a.AssetCategory != null)
            .ToListAsync();

        var today = DateTime.UtcNow.Date;
        var report = new List<DepreciationRowVm>(assets.Count);

        foreach (var a in assets)
        {
            var usefulLife = Math.Max(1, a.AssetCategory!.UsefulLifeYears);
            var annualDep = a.PurchaseCost!.Value / usefulLife;
            var ageYears = Math.Max(0, (today - a.PurchaseDate!.Value.Date).Days / 365m);
            var accumulated = Math.Min(a.PurchaseCost.Value, annualDep * ageYears);
            var nbv = Math.Max(0, a.PurchaseCost.Value - accumulated);

            report.Add(new DepreciationRowVm
            {
                AssetId = a.Id,
                TagCode = a.TagCode,
                AssetName = a.AssetName,
                Category = a.AssetCategory.Name,
                PurchaseCost = a.PurchaseCost.Value,
                UsefulLifeYears = usefulLife,
                AnnualDepreciation = Decimal.Round(annualDep, 2),
                AccumulatedDepreciation = Decimal.Round(accumulated, 2),
                NetBookValue = Decimal.Round(nbv, 2)
            });
        }

        return report.OrderBy(r => r.TagCode).ToList();
    }

    public string BuildDepreciationCsv(List<DepreciationRowVm> report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("AssetId,TagCode,AssetName,Category,PurchaseCost,UsefulLifeYears,AnnualDepreciation,AccumulatedDepreciation,NetBookValue");
        foreach (var row in report)
        {
            sb.AppendLine(string.Join(",",
                row.AssetId,
                Csv(row.TagCode),
                Csv(row.AssetName),
                Csv(row.Category),
                row.PurchaseCost.ToString("0.00"),
                row.UsefulLifeYears,
                row.AnnualDepreciation.ToString("0.00"),
                row.AccumulatedDepreciation.ToString("0.00"),
                row.NetBookValue.ToString("0.00")
            ));
        }

        return sb.ToString();
    }

    private static string Csv(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}

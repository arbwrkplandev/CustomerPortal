using WrkPlan.CustomerPortal.Application.Interfaces;
using WrkPlan.CustomerPortal.Shared.Dtos;

namespace WrkPlan.CustomerPortal.Application.Services;

public class OnboardingEngineService : IOnboardingEngineService
{
    public bool CanCompleteStep(IReadOnlyCollection<OnboardingStepDto> steps, Guid stepId)
    {
        var step = steps.Single(s => s.Id == stepId);
        if (step.DependsOnStepId is null)
        {
            return true;
        }

        var dependency = steps.SingleOrDefault(s => s.Id == step.DependsOnStepId.Value);
        return dependency?.IsCompleted == true;
    }

    public decimal CalculateProgressPercent(IReadOnlyCollection<OnboardingStepDto> steps)
    {
        if (steps.Count == 0)
        {
            return 0m;
        }

        var completed = steps.Count(s => s.IsCompleted);
        return Math.Round((decimal)completed / steps.Count * 100m, 2);
    }
}

public class RenewalService : IRenewalService
{
    public DateTime CalculateNextRenewal(DateTime startDateUtc, string cycle)
    {
        return cycle.ToLowerInvariant() switch
        {
            "monthly" => startDateUtc.AddMonths(1),
            "quarterly" => startDateUtc.AddMonths(3),
            "annual" or "yearly" => startDateUtc.AddYears(1),
            _ => throw new ArgumentOutOfRangeException(nameof(cycle), "Unsupported billing cycle.")
        };
    }
}

public class QuoteTransitionService(IRenewalService renewalService) : IQuoteTransitionService
{
    public (DateTime StartDateUtc, DateTime EndDateUtc, DateTime RenewalDateUtc) BuildSubscriptionTimeline(DateTime acceptedAtUtc, string cycle)
    {
        var start = acceptedAtUtc;
        var renewal = renewalService.CalculateNextRenewal(start, cycle);
        var end = cycle.ToLowerInvariant() switch
        {
            "monthly" => start.AddYears(1),
            "quarterly" => start.AddYears(1),
            "annual" or "yearly" => start.AddYears(3),
            _ => throw new ArgumentOutOfRangeException(nameof(cycle), "Unsupported billing cycle.")
        };

        return (start, end, renewal);
    }
}

public class CsvTransformService : ICsvTransformService
{
    public Task<(bool IsValid, List<string> Errors, string TransformedCsv)> ValidateAndTransformAsync(string csvPayload, CancellationToken ct = default)
    {
        var errors = new List<string>();
        var lines = csvPayload.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lines.Length < 2)
        {
            errors.Add("CSV must include header and at least one data row.");
            return Task.FromResult((false, errors, string.Empty));
        }

        var headers = lines[0].Split(',').Select(h => h.Trim()).ToArray();
        var required = new[] { "EmployeeId", "EmployeeName", "Department" };
        foreach (var col in required)
        {
            if (!headers.Contains(col, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"Missing required column: {col}");
            }
        }

        if (errors.Count > 0)
        {
            return Task.FromResult((false, errors, string.Empty));
        }

        var transformed = "ERPEmployeeCode,ERPEmployeeName,ERPDepartment\n" + string.Join('\n', lines.Skip(1));
        return Task.FromResult((true, errors, transformed));
    }
}

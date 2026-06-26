using Microsoft.EntityFrameworkCore;
using WrkPlan.CustomerPortal.Domain.Entities;
using WrkPlan.CustomerPortal.Domain.Enums;

namespace WrkPlan.CustomerPortal.Infrastructure.Data;

public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var t1 = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var t2 = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var t3 = Guid.Parse("10000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<TenantRegistry>().HasData(
            new TenantRegistry { Id = t1, Name = "Acme Manufacturing", TenantKey = "acme", IsActive = true, ActivatedUtc = DateTime.UtcNow },
            new TenantRegistry { Id = t2, Name = "Northwind Finance", TenantKey = "northwind", IsActive = true, ActivatedUtc = DateTime.UtcNow },
            new TenantRegistry { Id = t3, Name = "BluePeak Services", TenantKey = "bluepeak", IsActive = true, ActivatedUtc = DateTime.UtcNow }
        );

        var c1 = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var c2 = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var c3 = Guid.Parse("20000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<CustomerProfile>().HasData(
            new CustomerProfile { Id = c1, TenantId = t1, CompanyName = "Acme Manufacturing", AddressLine1 = "100 Main", City = "Dallas", Country = "USA", IsPortalActive = true, Status = RecordStatus.Active },
            new CustomerProfile { Id = c2, TenantId = t2, CompanyName = "Northwind Finance", AddressLine1 = "22 Queen St", City = "Toronto", Country = "Canada", IsPortalActive = true, Status = RecordStatus.Active },
            new CustomerProfile { Id = c3, TenantId = t3, CompanyName = "BluePeak Services", AddressLine1 = "5 Harbor Rd", City = "London", Country = "UK", IsPortalActive = true, Status = RecordStatus.Active }
        );

        modelBuilder.Entity<Subscription>().HasData(
            new Subscription { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), CustomerProfileId = c1, PlanName = "Growth", Price = 1499, Cycle = SubscriptionCycle.Monthly, StartDateUtc = DateTime.UtcNow.AddMonths(-4), EndDateUtc = DateTime.UtcNow.AddMonths(8), RenewalDateUtc = DateTime.UtcNow.AddDays(5), AutoRenew = true },
            new Subscription { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), CustomerProfileId = c2, PlanName = "Enterprise", Price = 4999, Cycle = SubscriptionCycle.Quarterly, StartDateUtc = DateTime.UtcNow.AddMonths(-2), EndDateUtc = DateTime.UtcNow.AddMonths(10), RenewalDateUtc = DateTime.UtcNow.AddDays(27), AutoRenew = true },
            new Subscription { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), CustomerProfileId = c3, PlanName = "Core", Price = 999, Cycle = SubscriptionCycle.Annual, StartDateUtc = DateTime.UtcNow.AddMonths(-7), EndDateUtc = DateTime.UtcNow.AddMonths(5), RenewalDateUtc = DateTime.UtcNow.AddDays(2), AutoRenew = true }
        );

        modelBuilder.Entity<SubscriptionPlan>().HasData(
            new SubscriptionPlan
            {
                Id = Guid.Parse("31000000-0000-0000-0000-000000000001"),
                Name = "Core",
                Description = "Starter plan for lean teams",
                Cycle = SubscriptionCycle.Annual,
                Price = 999,
                IsActive = true,
                FeaturesJson = "[\"Portal dashboard\",\"Standard support\",\"Onboarding toolkit\"]"
            },
            new SubscriptionPlan
            {
                Id = Guid.Parse("31000000-0000-0000-0000-000000000002"),
                Name = "Growth",
                Description = "Growth-focused plan with advanced analytics",
                Cycle = SubscriptionCycle.Monthly,
                Price = 1499,
                IsActive = true,
                FeaturesJson = "[\"Everything in Core\",\"Advanced analytics\",\"Priority support\",\"API access\"]"
            },
            new SubscriptionPlan
            {
                Id = Guid.Parse("31000000-0000-0000-0000-000000000003"),
                Name = "Enterprise",
                Description = "Enterprise-grade controls and support",
                Cycle = SubscriptionCycle.Quarterly,
                Price = 4999,
                IsActive = true,
                FeaturesJson = "[\"Everything in Growth\",\"Dedicated success manager\",\"Custom SLAs\",\"White-glove onboarding\"]"
            }
        );

        modelBuilder.Entity<Invoice>().HasData(
            new Invoice { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), CustomerProfileId = c1, InvoiceNumber = "INV-ACME-1001", Total = 1499, DueUtc = DateTime.UtcNow.AddDays(10), Status = RecordStatus.Active },
            new Invoice { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), CustomerProfileId = c2, InvoiceNumber = "INV-NW-1001", Total = 4999, DueUtc = DateTime.UtcNow.AddDays(-3), Status = RecordStatus.Overdue },
            new Invoice { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), CustomerProfileId = c3, InvoiceNumber = "INV-BP-1001", Total = 999, DueUtc = DateTime.UtcNow.AddDays(14), Status = RecordStatus.Active }
        );

        modelBuilder.Entity<SupportTicket>().HasData(
            new SupportTicket { Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), CustomerProfileId = c1, Subject = "Need onboarding help", Priority = "High", Status = RecordStatus.Active },
            new SupportTicket { Id = Guid.Parse("50000000-0000-0000-0000-000000000002"), CustomerProfileId = c2, Subject = "Invoice discrepancy", Priority = "Normal", Status = RecordStatus.Active },
            new SupportTicket { Id = Guid.Parse("50000000-0000-0000-0000-000000000003"), CustomerProfileId = c3, Subject = "API token request", Priority = "Normal", Status = RecordStatus.Active }
        );

        modelBuilder.Entity<Announcement>().HasData(
            new Announcement { Id = Guid.Parse("60000000-0000-0000-0000-000000000001"), CustomerProfileId = c1, Title = "June Release", Message = "New billing analytics are live." },
            new Announcement { Id = Guid.Parse("60000000-0000-0000-0000-000000000002"), CustomerProfileId = c2, Title = "Renewal Notice", Message = "Your renewal window opens in 30 days." },
            new Announcement { Id = Guid.Parse("60000000-0000-0000-0000-000000000003"), CustomerProfileId = c3, Title = "Knowledge Base Update", Message = "New implementation checklist templates are available." }
        );
    }
}

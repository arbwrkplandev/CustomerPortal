using Microsoft.EntityFrameworkCore;
using WrkPlan.CustomerPortal.Domain.Entities;

namespace WrkPlan.CustomerPortal.Infrastructure.Data;

public class AdminDbContext(DbContextOptions<AdminDbContext> options) : DbContext(options)
{
    public DbSet<TenantRegistry> TenantRegistries => Set<TenantRegistry>();
    public DbSet<TenantConnectionMap> TenantConnectionMaps => Set<TenantConnectionMap>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<AppRole> Roles => Set<AppRole>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<NotificationSetting> NotificationSettings => Set<NotificationSetting>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<ServiceChangeHistory> ServiceChangeHistories => Set<ServiceChangeHistory>();
    public DbSet<TSIncreaseRequest> TSIncreaseRequests => Set<TSIncreaseRequest>();
    public DbSet<SalesQuote> SalesQuotes => Set<SalesQuote>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<Amendment> Amendments => Set<Amendment>();
    public DbSet<AgreementAcknowledgement> AgreementAcknowledgements => Set<AgreementAcknowledgement>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractDocument> ContractDocuments => Set<ContractDocument>();
    public DbSet<ContractAsset> ContractAssets => Set<ContractAsset>();
    public DbSet<ContractSignature> ContractSignatures => Set<ContractSignature>();
    public DbSet<ContractStatusHistory> ContractStatusHistories => Set<ContractStatusHistory>();
    public DbSet<BillingContact> BillingContacts => Set<BillingContact>();
    public DbSet<PaymentMethodMeta> PaymentMethodsMeta => Set<PaymentMethodMeta>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ManualPaymentEntry> ManualPaymentEntries => Set<ManualPaymentEntry>();
    public DbSet<PaymentModeHistory> PaymentModeHistory => Set<PaymentModeHistory>();
    public DbSet<PaymentEvent> PaymentEvents => Set<PaymentEvent>();
    public DbSet<RazorpaySetting> RazorpaySettings => Set<RazorpaySetting>();
    public DbSet<RazorpayWebhookEvent> RazorpayWebhookEvents => Set<RazorpayWebhookEvent>();
    public DbSet<OnboardingWorkflow> OnboardingWorkflows => Set<OnboardingWorkflow>();
    public DbSet<OnboardingStep> OnboardingSteps => Set<OnboardingStep>();
    public DbSet<ImplementationPlan> ImplementationPlans => Set<ImplementationPlan>();
    public DbSet<QuestionnaireResponse> QuestionnaireResponses => Set<QuestionnaireResponse>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();
    public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();
    public DbSet<TicketStatusHistory> TicketStatusHistories => Set<TicketStatusHistory>();
    public DbSet<Escalation> Escalations => Set<Escalation>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AnnouncementTenantTarget> AnnouncementTenantTargets => Set<AnnouncementTenantTarget>();
    public DbSet<ResourceFile> ResourceFiles => Set<ResourceFile>();
    public DbSet<Download> Downloads => Set<Download>();
    public DbSet<FeatureRequest> FeatureRequests => Set<FeatureRequest>();
    public DbSet<TrainingRequest> TrainingRequests => Set<TrainingRequest>();
    public DbSet<ArchiveRequest> ArchiveRequests => Set<ArchiveRequest>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<LoginAuditLog> LoginAuditLogs => Set<LoginAuditLog>();
    public DbSet<NotificationQueue> NotificationQueue => Set<NotificationQueue>();
    public DbSet<IntegrationEvent> IntegrationEvents => Set<IntegrationEvent>();
    public DbSet<CustomerThemePreference> CustomerThemePreferences => Set<CustomerThemePreference>();
    public DbSet<TbErpClientDetail> TB_ERP_CLIENT_DETAIL => Set<TbErpClientDetail>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<SubscriptionChangeLog> SubscriptionChangeLogs => Set<SubscriptionChangeLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TbErpClientDetail>().ToTable("TB_ERP_CLIENT_DETAIL", "dbo").HasKey(x => x.Id);

        modelBuilder.Entity<TenantRegistry>().HasIndex(x => x.TenantKey).IsUnique();
        modelBuilder.Entity<PaymentEvent>().HasIndex(x => x.IdempotencyKey).IsUnique();
        modelBuilder.Entity<Subscription>().Property(x => x.Price).HasPrecision(18, 2);
        modelBuilder.Entity<SalesQuote>().Property(x => x.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<Invoice>().Property(x => x.Total).HasPrecision(18, 2);
        modelBuilder.Entity<InvoiceItem>().Property(x => x.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>().Property(x => x.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<ManualPaymentEntry>().Property(x => x.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<Invoice>().Property(x => x.OutstandingAmount).HasPrecision(18, 2);
        modelBuilder.Entity<SubscriptionPlan>().Property(x => x.Price).HasPrecision(18, 2);
        modelBuilder.Entity<ContractSignature>().Property(x => x.PlacementXPercent).HasPrecision(5, 2);
        modelBuilder.Entity<ContractSignature>().Property(x => x.PlacementYPercent).HasPrecision(5, 2);

        modelBuilder.Entity<SupportTicket>().HasIndex(x => x.TicketNumber).IsUnique();
        modelBuilder.Entity<ContractDocument>().HasIndex(x => new { x.ContractId, x.IsLatest });
        modelBuilder.Entity<AnnouncementTenantTarget>().HasIndex(x => new { x.AnnouncementId, x.TenantId }).IsUnique();
        modelBuilder.Entity<RazorpayWebhookEvent>().HasIndex(x => x.EventId).IsUnique();
        modelBuilder.Entity<RazorpaySetting>().HasIndex(x => x.TenantId).IsUnique();
        modelBuilder.Entity<AuditEvent>().HasIndex(x => new { x.OccurredUtc, x.EventType });

        SeedData.Seed(modelBuilder);
    }
}

using WrkPlan.CustomerPortal.Domain.Common;
using WrkPlan.CustomerPortal.Domain.Enums;

namespace WrkPlan.CustomerPortal.Domain.Entities;

public class TenantRegistry : BaseEntity
{
    public string TenantKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? ActivatedUtc { get; set; }
}

public class TenantConnectionMap : BaseEntity
{
    public Guid TenantRegistryId { get; set; }
    public string EncryptedConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

public class AppUser : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsTwoFactorEnabled { get; set; }
}

public class AppRole : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

public class Permission : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CustomerProfile : BaseEntity
{
    public Guid TenantId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public RecordStatus Status { get; set; } = RecordStatus.Active;
    public bool IsPortalActive { get; set; }
    public bool IsPartnerPortalEnabled { get; set; }
}

public class Contact : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ContactType { get; set; } = string.Empty;
}

public class NotificationSetting : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public bool BillingAlerts { get; set; } = true;
    public bool RenewalAlerts { get; set; } = true;
    public bool ProductAnnouncements { get; set; } = true;
}

public class Subscription : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public SubscriptionCycle Cycle { get; set; } = SubscriptionCycle.Monthly;
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public DateTime RenewalDateUtc { get; set; }
    public bool AutoRenew { get; set; } = true;
}

public class Module : BaseEntity
{
    public Guid SubscriptionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LicenseCount { get; set; }
}

public class ServiceChangeHistory : BaseEntity
{
    public Guid SubscriptionId { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class TSIncreaseRequest : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public int RequestedAdditionalUsers { get; set; }
    public RecordStatus Status { get; set; } = RecordStatus.Draft;
}

public class SalesQuote : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public RecordStatus Status { get; set; } = RecordStatus.Draft;
}

public class SalesOrder : BaseEntity
{
    public Guid SalesQuoteId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime ExecutedUtc { get; set; }
}

public class Amendment : BaseEntity
{
    public Guid SalesOrderId { get; set; }
    public string AmendmentNumber { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public class AgreementAcknowledgement : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string AgreementUrl { get; set; } = string.Empty;
    public string AgreementVersion { get; set; } = string.Empty;
    public DateTime AcknowledgedUtc { get; set; }
}

public class Contract : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public DateTime EffectiveUtc { get; set; }
    public DateTime RenewalUtc { get; set; }
    public DateTime ExpiryUtc { get; set; }
    public string Title { get; set; } = "WrkPlan Service Agreement";
    public string Version { get; set; } = "v1";
    public string Status { get; set; } = "Draft";
    public string? LastActionBy { get; set; }
    public DateTime? SentUtc { get; set; }
    public DateTime? ViewedUtc { get; set; }
    public DateTime? CustomerSignedUtc { get; set; }
    public DateTime? CompletedUtc { get; set; }
}

public class ContractDocument : BaseEntity
{
    public Guid ContractId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string BlobPath { get; set; } = string.Empty;
    public string DocumentType { get; set; } = "Generated";
    public string VersionLabel { get; set; } = "v1";
    public bool IsLatest { get; set; } = true;
    public bool IsSigned { get; set; }
    public string? ContentType { get; set; }
    public long SizeBytes { get; set; }
}

public class ContractAsset : BaseEntity
{
    public Guid ContractId { get; set; }
    public string AssetType { get; set; } = "Logo";
    public string FileName { get; set; } = string.Empty;
    public string BlobPath { get; set; } = string.Empty;
    public string? ContentType { get; set; }
}

public class ContractSignature : BaseEntity
{
    public Guid ContractId { get; set; }
    public string SignerRole { get; set; } = "Customer";
    public string SignerEmail { get; set; } = string.Empty;
    public string SignatureBlobPath { get; set; } = string.Empty;
    public decimal PlacementXPercent { get; set; } = 70m;
    public decimal PlacementYPercent { get; set; } = 85m;
    public DateTime SignedUtc { get; set; } = DateTime.UtcNow;
}

public class ContractStatusHistory : BaseEntity
{
    public Guid ContractId { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ActorEmail { get; set; } = string.Empty;
    public string ActorRole { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? Notes { get; set; }
}

public class BillingContact : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class PaymentMethodMeta : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string StripePaymentMethodId { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public int ExpMonth { get; set; }
    public int ExpYear { get; set; }
    public bool IsDefault { get; set; }
}

public class Invoice : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime DueUtc { get; set; }
    public RecordStatus Status { get; set; } = RecordStatus.Draft;
    public decimal OutstandingAmount { get; set; }
    public string PaymentStatus { get; set; } = "Unpaid";
    public DateTime? LastPaymentUtc { get; set; }
    public string? LastPaymentMode { get; set; }
}

public class InvoiceItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class Payment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidUtc { get; set; }
    public string ProviderRef { get; set; } = string.Empty;
    public string Mode { get; set; } = "online";
    public string Source { get; set; } = "Online";
    public string Provider { get; set; } = "Razorpay";
    public string Status { get; set; } = "Success";
    public string? Notes { get; set; }
}

public class ManualPaymentEntry : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerProfileId { get; set; }
    public Guid? InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidUtc { get; set; }
    public string Mode { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Status { get; set; } = "Recorded";
}

public class PaymentModeHistory : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerProfileId { get; set; }
    public Guid? InvoiceId { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid? ManualPaymentEntryId { get; set; }
    public string Mode { get; set; } = string.Empty;
    public string Source { get; set; } = "Online";
    public string ReferenceId { get; set; } = string.Empty;
    public DateTime OccurredUtc { get; set; } = DateTime.UtcNow;
}

public class PaymentEvent : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    public string RawPayload { get; set; } = string.Empty;
}

public class RazorpaySetting : BaseEntity
{
    public Guid TenantId { get; set; } = Guid.Empty;
    public string KeyIdEncrypted { get; set; } = string.Empty;
    public string KeySecretEncrypted { get; set; } = string.Empty;
    public string WebhookSecretEncrypted { get; set; } = string.Empty;
    public bool IsTestMode { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public DateTime? LastValidatedUtc { get; set; }
}

public class RazorpayWebhookEvent : BaseEntity
{
    public string EventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public bool IsProcessed { get; set; }
    public string? ProcessingError { get; set; }
    public DateTime? ProcessedUtc { get; set; }
}

public class OnboardingWorkflow : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string Name { get; set; } = "Default";
    public RecordStatus Status { get; set; } = RecordStatus.Draft;
}

public class OnboardingStep : BaseEntity
{
    public Guid OnboardingWorkflowId { get; set; }
    public OnboardingStepType StepType { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = true;
    public int SortOrder { get; set; }
    public bool IsCompleted { get; set; }
    public Guid? DependsOnStepId { get; set; }
}

public class ImplementationPlan : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string JsonPlan { get; set; } = string.Empty;
}

public class QuestionnaireResponse : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string QuestionKey { get; set; } = string.Empty;
    public string ResponseValue { get; set; } = string.Empty;
}

public class SupportTicket : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
    public RecordStatus Status { get; set; } = RecordStatus.Active;
    public string WorkflowStatus { get; set; } = "Open";
    public DateTime? ResolvedUtc { get; set; }
    public int? ResolvedInDays { get; set; }
    public string? ResolutionMessage { get; set; }
}

public class TicketComment : BaseEntity
{
    public Guid SupportTicketId { get; set; }
    public string Body { get; set; } = string.Empty;
    public string AuthorRole { get; set; } = "Customer";
    public string AuthorEmail { get; set; } = string.Empty;
}

public class TicketMessage : BaseEntity
{
    public Guid SupportTicketId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string AuthorRole { get; set; } = "Customer";
    public string AuthorEmail { get; set; } = string.Empty;
    public bool IsResolution { get; set; }
}

public class TicketStatusHistory : BaseEntity
{
    public Guid SupportTicketId { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public string ChangedByRole { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class Escalation : BaseEntity
{
    public Guid SupportTicketId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class Announcement : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public Guid? TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;
    public bool IsDeleted { get; set; }
    public bool IsGlobal { get; set; }
    public DateTime? PublishedUtc { get; set; }
}

public class AnnouncementTenantTarget : BaseEntity
{
    public Guid AnnouncementId { get; set; }
    public Guid TenantId { get; set; }
}

public class ResourceFile : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class Download : BaseEntity
{
    public Guid ResourceFileId { get; set; }
    public Guid UserId { get; set; }
}

public class FeatureRequest : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string Summary { get; set; } = string.Empty;
}

public class TrainingRequest : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public DateTime RequestedUtc { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class ArchiveRequest : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ActivityLog : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
}

public class AuditEvent : BaseEntity
{
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string DetailsJson { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public DateTime OccurredUtc { get; set; } = DateTime.UtcNow;
}

public class LoginAuditLog : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class NotificationQueue : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime ScheduledUtc { get; set; }
}

public class IntegrationEvent : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}

public class CustomerThemePreference : BaseEntity
{
    public Guid CustomerProfileId { get; set; }
    public bool IsDarkMode { get; set; }
    public int BackgroundR { get; set; } = 18;
    public int BackgroundG { get; set; } = 25;
    public int BackgroundB { get; set; } = 38;
}

public class TbErpClientDetail
{
    public int Id { get; set; }
    public string? ClientName { get; set; }
    public string? ClientCode { get; set; }
}

// ── Subscription plan catalog ───────────────────────────────────────────────
public class SubscriptionPlan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SubscriptionCycle Cycle { get; set; } = SubscriptionCycle.Monthly;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    /// <summary>Newline-separated feature list</summary>
    public string FeaturesJson { get; set; } = "[]";
}

// ── Subscription change audit trail ────────────────────────────────────────
public class SubscriptionChangeLog : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerProfileId { get; set; }
    public string OldPlanName { get; set; } = string.Empty;
    public string NewPlanName { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

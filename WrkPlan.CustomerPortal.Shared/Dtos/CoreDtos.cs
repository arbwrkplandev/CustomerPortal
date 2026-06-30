namespace WrkPlan.CustomerPortal.Shared.Dtos;

public record ApiErrorDto(string Code, string Message, string? CorrelationId = null);

public record ApiResponseDto<T>(bool Success, T? Data, ApiErrorDto? Error = null);

public record AuthRequestDto(string Email, string Password, string TenantKey);
public record AuthResponseDto(string AccessToken, string RefreshToken, string Role, Guid UserId, Guid TenantId);

public record TenantProvisionRequestDto(string CompanyName, string TenantKey, string AdminEmail, string PlanName, decimal Price, string Cycle);
public record TenantProvisionResponseDto(Guid TenantId, string DatabaseName, string InitialAdminEmail, string InitialPassword);

public record ThemePreferenceDto(bool IsDarkMode, int BackgroundR, int BackgroundG, int BackgroundB);

public record DashboardCardDto(string Title, string Value, string Status, string? Subtext = null);
public record DashboardResponseDto(List<DashboardCardDto> Cards);

public record SalesQuoteDto(Guid Id, string QuoteNumber, decimal Amount, string Status);
public record AnnouncementDto(
    Guid Id,
    string Title,
    string Message,
    string? Topic = null,
    bool IsPublished = true,
    DateTime? PublishedUtc = null,
    bool IsGlobal = false,
    List<Guid>? TenantIds = null);
public record SupportTicketDto(
    Guid Id,
    string Subject,
    string Priority,
    string Status,
    string? TicketNumber = null,
    DateTime? CreatedUtc = null,
    DateTime? ResolvedUtc = null,
    int? ResolvedInDays = null,
    string? ResolutionMessage = null,
    string? TenantName = null);
public record CreateSupportTicketDto(string Subject, string Priority, string Description);
public record InvoiceSummaryDto(string InvoiceNumber, decimal Total, string Status, DateTime DueUtc);

public record WebhookEventDto(string IdempotencyKey, string EventType, string Payload);

public record OnboardingStepDto(Guid Id, string Title, bool IsRequired, bool IsCompleted, Guid? DependsOnStepId);
public record OnboardingWorkflowDto(Guid WorkflowId, string Name, string Status, List<OnboardingStepDto> Steps);

public record PagedRequestDto(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    string? SortBy = null,
    string? SortOrder = null,
    bool Desc = false)
{
    public int SafePageNumber => PageNumber < 1 ? 1 : PageNumber;
    public int SafePageSize => PageSize < 1 ? 10 : PageSize > 200 ? 200 : PageSize;
    public bool IsDesc => !string.IsNullOrWhiteSpace(SortOrder)
        ? string.Equals(SortOrder, "desc", StringComparison.OrdinalIgnoreCase)
        : Desc;
}

public record PagedResultDto<T>(int PageNumber, int PageSize, int TotalCount, int TotalPages, IReadOnlyCollection<T> Items);

public record ErpClientDto(int Id, string? ClientName, string? ClientCode);

public record TenantSummaryDto(Guid Id, string Name, string TenantKey, bool IsActive, DateTime? ActivatedUtc, string? CompanyName, string? Plan, decimal? SubscriptionPrice);

public record CreateTenantAccountRequestDto(
    string TenantName,
    string TenantKey,
    string CompanyName,
    string AdminEmail,
    string AdminPassword,
    bool ActivateNow = true);

public record QuestionnaireTemplateItemDto(Guid Id, string QuestionText, bool IsRequired, int SortOrder);
public record UpsertQuestionnaireTemplateDto(string QuestionText, bool IsRequired, int SortOrder);
public record QuestionnaireAnswerDto(string QuestionKey, string ResponseValue);
public record SubmitQuestionnaireResponseDto(List<QuestionnaireAnswerDto> Answers);

// ── Contracts ───────────────────────────────────────────────────────────────
public record ContractSummaryDto(
    Guid Id,
    string ContractNumber,
    string Title,
    string Version,
    string Status,
    string? ESignStatus,
    string? ESignFieldsJson,
    DateTime EffectiveUtc,
    DateTime ExpiryUtc,
    DateTime? SentUtc,
    DateTime? ViewedUtc,
    DateTime? CustomerSignedUtc,
    DateTime? CompletedUtc,
    string? TenantName = null);

public record ContractStatusHistoryDto(
    Guid Id,
    string FromStatus,
    string ToStatus,
    string Action,
    string ActorEmail,
    string ActorRole,
    DateTime CreatedUtc,
    string? Notes,
    string? IpAddress);

public record ContractVersionDto(
    Guid Id,
    string FileName,
    string VersionLabel,
    string DocumentType,
    bool IsSigned,
    bool IsLatest,
    DateTime CreatedUtc,
    long SizeBytes);

public record ContractDetailDto(
    Guid Id,
    Guid CustomerProfileId,
    string ContractNumber,
    string Title,
    string HeaderText,
    string BodyJson,
    string ESignStatus,
    string? SignedPdfPath,
    List<ESignFieldDto> ESignFields,
    List<ESignEntryDto> ESignEntries,
    string Version,
    string Status,
    DateTime EffectiveUtc,
    DateTime ExpiryUtc,
    DateTime? SentUtc,
    DateTime? ViewedUtc,
    DateTime? CustomerSignedUtc,
    DateTime? CompletedUtc,
    string? LastActionBy,
    List<ContractVersionDto> Versions,
    List<ContractStatusHistoryDto> History);

public record CreateGeneratedContractDto(
    Guid CustomerProfileId,
    string ContractNumber,
    string Title,
    string HeaderText,
    string BodyJson,
    DateTime EffectiveUtc,
    DateTime ExpiryUtc,
    string Version,
    string Status);

public record ContractCustomerProfileOptionDto(
    Guid CustomerProfileId,
    Guid TenantId,
    string TenantName,
    string? CompanyName,
    string? ContactEmail);

public record UploadManualContractDto(
    Guid CustomerProfileId,
    string ContractNumber,
    DateTime EffectiveUtc,
    DateTime ExpiryUtc,
    string Version,
    string FileName,
    string ContentType,
    string FileBase64);

public record UploadContractAssetDto(
    Guid ContractId,
    string AssetType,
    string FileName,
    string ContentType,
    string FileBase64);

public record UploadSignedContractDto(
    Guid ContractId,
    string FileName,
    string ContentType,
    string FileBase64,
    string? Notes = null);

public record CustomerApplySignatureDto(
    Guid ContractId,
    string SignatureFileName,
    string SignatureContentType,
    string SignatureBase64,
    decimal PlacementXPercent,
    decimal PlacementYPercent,
    bool Confirm = false);

public record ContractStatusTransitionDto(Guid ContractId, string ToStatus, string? Notes = null);

public record ESignFieldDto(string Id, string Label, string Type, bool Required, int Order);
public record AddESignFieldsDto(Guid ContractId, List<ESignFieldDto> Fields);
public record SendContractDto(Guid ContractId);
public record SubmitESignEntryDto(Guid ContractId, string FieldId, string FieldLabel, string ValueDataUrl, string SignedByName);
public record ESignEntryDto(string FieldId, string FieldLabel, string ValueDataUrl, string SignedByName, DateTime SignedAtUtc);
public record ESignSubmitResultDto(bool IsComplete, string? SignedPdfPath);

public record ContractSignFieldDto(
    string FieldId,
    string Label,
    string FieldType,
    int PageNumber,
    float X,
    float Y,
    float Width,
    float Height,
    bool IsRequired);

public record ContractWithSignFieldsDto(
    Guid ContractId,
    string ContractNumber,
    List<ContractSignFieldDto> SignatureFields,
    string Status,
    DateTime? SentToCustomerUtc);

public record SignatureSessionDto(
    Guid SessionId,
    string SessionToken,
    Guid ContractId,
    string ContractNumber,
    List<ContractSignFieldDto> Fields,
    string CompanyName);

public record SubmitFieldSignatureDto(
    string SessionToken,
    string FieldId,
    string SignatureDataUrl,
    string SignedByName);

public record SigningStatusDto(
    Guid SessionId,
    int TotalRequired,
    int TotalSigned,
    bool IsComplete,
    string? SignedPdfPath);

public record PendingContractSessionDto(
    Guid SessionId,
    string SessionToken,
    Guid ContractId,
    string ContractNumber,
    DateTime ExpiresUtc,
    string Status);

// ── Support admin workflow ─────────────────────────────────────────────────
public record SupportTicketMessageDto(
    Guid Id,
    string Message,
    string AuthorRole,
    string AuthorEmail,
    DateTime CreatedUtc,
    bool IsResolution);

public record SupportTicketStatusHistoryDto(
    Guid Id,
    string FromStatus,
    string ToStatus,
    string ChangedBy,
    string ChangedByRole,
    DateTime CreatedUtc,
    string? Notes);

public record SupportTicketDetailDto(
    Guid Id,
    string TicketNumber,
    string Subject,
    string Priority,
    string Status,
    string? TenantName,
    DateTime CreatedUtc,
    DateTime? ResolvedUtc,
    int? ResolvedInDays,
    string? ResolutionMessage,
    List<SupportTicketMessageDto> Messages,
    List<SupportTicketStatusHistoryDto> StatusHistory);

public record AdminSupportTicketFilterDto(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    string? Status = null,
    string? Priority = null,
    Guid? TenantId = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null);

public record SupportTicketCountersDto(int Open, int InProgress, int Resolved, int Overdue);

public record AdminSupportTicketListDto(
    PagedResultDto<SupportTicketDto> Tickets,
    SupportTicketCountersDto Counters);

public record AdminReplyTicketDto(Guid TicketId, string Message);
public record ResolveTicketDto(Guid TicketId, string ResolutionMessage);
public record ChangeTicketStatusDto(Guid TicketId, string Status, string? Notes = null);

// ── Admin announcements ────────────────────────────────────────────────────
public record UpsertAnnouncementDto(
    Guid? Id,
    string Title,
    string Message,
    string Topic,
    bool IsGlobal,
    bool IsPublished,
    List<Guid>? TenantIds);

public record AnnouncementFilterDto(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    string? Topic = null,
    bool? IsPublished = null);

// ── Billing monitoring ─────────────────────────────────────────────────────
public record BillingMonitorItemDto(
    Guid TenantId,
    Guid CustomerProfileId,
    string TenantName,
    string CompanyName,
    string CurrentPlan,
    string BillingCycle,
    decimal DueAmount,
    decimal Outstanding,
    DateTime NextRenewalDate,
    string PaymentStatus,
    DateTime? LastPaymentDate,
    string? LastPaymentMode);

public record BillingMonitoringResultDto(
    PagedResultDto<BillingMonitorItemDto> Items,
    int OverdueCount,
    int UpcomingRenewalCount);

public record BillingMonitoringFilterDto(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    string? PaymentStatus = null,
    DateTime? RenewalFromUtc = null,
    DateTime? RenewalToUtc = null,
    string? SortBy = null,
    string? SortOrder = null);

public record InvoicePaymentHistoryDto(
    Guid PaymentId,
    decimal Amount,
    DateTime PaidUtc,
    string Mode,
    string Source,
    string ReferenceId,
    string? Notes);

public record ManualPaymentCreateDto(
    Guid CustomerProfileId,
    Guid? InvoiceId,
    decimal Amount,
    DateTime PaidUtc,
    string Mode,
    string ReferenceId,
    string? Notes);

public record PaymentModeHistoryDto(
    string Mode,
    string Source,
    string ReferenceId,
    DateTime OccurredUtc);

// ── Customer payment flow ──────────────────────────────────────────────────
public record PaymentPreviewRequestDto(Guid CustomerProfileId, string Cycle);

public record PaymentPreviewDto(
    Guid CustomerProfileId,
    string Cycle,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    decimal PlanAmount,
    decimal Taxes,
    decimal Fees,
    decimal TotalAmount,
    Guid InvoiceId,
    string InvoiceNumber,
    string Currency);

public record RazorpayOrderCreateRequestDto(Guid CustomerProfileId, Guid InvoiceId, string Cycle);

public record RazorpayOrderCreateResponseDto(
    string KeyId,
    string OrderId,
    decimal Amount,
    string Currency,
    string Description,
    string CustomerName,
    string CustomerEmail,
    Guid InvoiceId,
    string InvoiceNumber);

public record RazorpayVerifyRequestDto(
    string RazorpayOrderId,
    string RazorpayPaymentId,
    string RazorpaySignature,
    Guid InvoiceId,
    Guid CustomerProfileId,
    string Cycle,
    string Mode = "Online");

public record PaymentProcessResultDto(
    bool Success,
    string Status,
    string Message,
    Guid InvoiceId,
    string InvoiceNumber,
    DateTime? PaidUtc);

// ── Razorpay admin settings ────────────────────────────────────────────────
public record RazorpaySettingsUpsertDto(
    string KeyId,
    string KeySecret,
    string WebhookSecret,
    bool IsTestMode,
    bool ValidateConnection = false);

public record RazorpaySettingsViewDto(
    string KeyIdMasked,
    bool IsTestMode,
    bool IsActive,
    DateTime? LastValidatedUtc,
    bool HasSecretConfigured,
    bool HasWebhookSecretConfigured);

// ── Audit / reports ────────────────────────────────────────────────────────
public record AuditReportFilterDto(
    int PageNumber = 1,
    int PageSize = 25,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    Guid? TenantId = null,
    string? EventType = null,
    string? User = null,
    string? SortBy = null,
    string? SortOrder = null);

public record AuditReportItemDto(
    DateTime OccurredUtc,
    string EventType,
    string Action,
    string EntityType,
    string EntityId,
    string? TenantName,
    string User,
    string UserRole,
    string Details);

// ── Invoice detail ──────────────────────────────────────────────────────────
public record InvoiceLineDto(string Description, decimal Amount);
public record InvoiceDetailDto(
    Guid Id, string InvoiceNumber, string PlanName, string BillingCycle,
    decimal BaseAmount, decimal Taxes, decimal Discounts, decimal Total,
    string Status, DateTime DueUtc, DateTime? PaidUtc, string? ProviderRef,
    List<InvoiceLineDto> Lines);

// ── Subscription plan catalog ───────────────────────────────────────────────
public record SubscriptionPlanDto(
    Guid Id, string Name, string Description, string Cycle,
    decimal Price, bool IsActive, List<string> Features);

public record CreateSubscriptionPlanDto(
    string Name, string Description, string Cycle,
    decimal Price, List<string> Features);

public record UpdateSubscriptionPlanDto(
    string Name, string Description, string Cycle,
    decimal Price, bool IsActive, List<string> Features);

public record AssignSubscriptionPlanDto(Guid TenantId, Guid PlanId, DateTime EffectiveDate, string? Notes);

// ── Customer current subscription ──────────────────────────────────────────
public record CustomerSubscriptionDto(
    Guid SubscriptionId, Guid CustomerProfileId, string PlanName, string PlanDescription,
    string BillingCycle, decimal Price, DateTime StartDate,
    DateTime EndDate, DateTime RenewalDate, bool AutoRenew,
    string Status, List<string> Features);

// ── Subscription change log ─────────────────────────────────────────────────
public record SubscriptionChangeLogDto(
    Guid Id, string TenantName, string OldPlan, string NewPlan,
    string ChangedBy, DateTime ChangedUtc, string? Notes);

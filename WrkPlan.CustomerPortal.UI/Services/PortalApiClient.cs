using System.Net.Http.Headers;
using System.Net.Http.Json;
using WrkPlan.CustomerPortal.Shared.Dtos;

namespace WrkPlan.CustomerPortal.UI.Services;

public class PortalApiClient(IHttpClientFactory httpClientFactory, SessionService session)
{
    public async Task<ApiResponseDto<AuthResponseDto>?> LoginAsync(AuthRequestDto request, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("WrkPlanApi");
        var response = await client.PostAsJsonAsync("api/Auth/login", request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(ct);
            return new ApiResponseDto<AuthResponseDto>(false, null, new ApiErrorDto("login_failed", errorText));
        }

        return await response.Content.ReadFromJsonAsync<ApiResponseDto<AuthResponseDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<DashboardResponseDto>?> GetHubDashboardAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<DashboardResponseDto>>("api/CustomerHub/dashboard", ct);
    }

    public async Task<ApiResponseDto<DashboardResponseDto>?> GetPortalDashboardAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<DashboardResponseDto>>("api/Dashboard/portal", ct);
    }

    public async Task<ApiResponseDto<List<AnnouncementDto>>?> GetAnnouncementsAsync(CancellationToken ct = default)
    {
        var paged = await GetAnnouncementsPagedAsync(1, 10, "createdUtc", "desc", null, ct);
        return paged is null
            ? null
            : new ApiResponseDto<List<AnnouncementDto>>(paged.Success, paged.Data?.Items.ToList(), paged.Error);
    }

    public async Task<ApiResponseDto<List<SupportTicketDto>>?> GetSupportTicketsAsync(CancellationToken ct = default)
    {
        var paged = await GetSupportTicketsPagedAsync(1, 10, "createdUtc", "desc", null, ct);
        return paged is null
            ? null
            : new ApiResponseDto<List<SupportTicketDto>>(paged.Success, paged.Data?.Items.ToList(), paged.Error);
    }

    public async Task<ApiResponseDto<SupportTicketDto>?> CreateSupportTicketAsync(CreateSupportTicketDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/SupportTickets", request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(ct);
            return new ApiResponseDto<SupportTicketDto>(false, null, new ApiErrorDto("ticket_create_failed", errorText));
        }

        return await response.Content.ReadFromJsonAsync<ApiResponseDto<SupportTicketDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<List<InvoiceSummaryDto>>?> GetInvoicesAsync(CancellationToken ct = default)
    {
        var paged = await GetInvoicesPagedAsync(1, 10, "createdUtc", "desc", null, ct);
        return paged is null
            ? null
            : new ApiResponseDto<List<InvoiceSummaryDto>>(paged.Success, paged.Data?.Items.ToList(), paged.Error);
    }

    public async Task<ApiResponseDto<PagedResultDto<AnnouncementDto>>?> GetAnnouncementsPagedAsync(
        int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? sortOrder = null, string? search = null,
        CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var qs = $"api/Announcements?pageNumber={pageNumber}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(sortBy)) qs += $"&sortBy={Uri.EscapeDataString(sortBy)}";
        if (!string.IsNullOrWhiteSpace(sortOrder)) qs += $"&sortOrder={Uri.EscapeDataString(sortOrder)}";
        if (!string.IsNullOrWhiteSpace(search)) qs += $"&search={Uri.EscapeDataString(search)}";
        return await client.GetFromJsonAsync<ApiResponseDto<PagedResultDto<AnnouncementDto>>>(qs, ct);
    }

    public async Task<ApiResponseDto<PagedResultDto<SupportTicketDto>>?> GetSupportTicketsPagedAsync(
        int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? sortOrder = null, string? search = null,
        CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var qs = $"api/SupportTickets?pageNumber={pageNumber}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(sortBy)) qs += $"&sortBy={Uri.EscapeDataString(sortBy)}";
        if (!string.IsNullOrWhiteSpace(sortOrder)) qs += $"&sortOrder={Uri.EscapeDataString(sortOrder)}";
        if (!string.IsNullOrWhiteSpace(search)) qs += $"&search={Uri.EscapeDataString(search)}";
        return await client.GetFromJsonAsync<ApiResponseDto<PagedResultDto<SupportTicketDto>>>(qs, ct);
    }

    public async Task<ApiResponseDto<PagedResultDto<InvoiceSummaryDto>>?> GetInvoicesPagedAsync(
        int pageNumber = 1, int pageSize = 10, string? sortBy = null, string? sortOrder = null, string? search = null,
        CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var qs = $"api/Invoices?pageNumber={pageNumber}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(sortBy)) qs += $"&sortBy={Uri.EscapeDataString(sortBy)}";
        if (!string.IsNullOrWhiteSpace(sortOrder)) qs += $"&sortOrder={Uri.EscapeDataString(sortOrder)}";
        if (!string.IsNullOrWhiteSpace(search)) qs += $"&search={Uri.EscapeDataString(search)}";
        return await client.GetFromJsonAsync<ApiResponseDto<PagedResultDto<InvoiceSummaryDto>>>(qs, ct);
    }

    public async Task<ApiResponseDto<ThemePreferenceDto>?> GetThemeAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<ThemePreferenceDto>>("api/Theme", ct);
    }

    public async Task<ApiResponseDto<ThemePreferenceDto>?> SaveThemeAsync(ThemePreferenceDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PutAsJsonAsync("api/Theme", request, ct);
        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponseDto<ThemePreferenceDto>(false, null, new ApiErrorDto("theme_update_failed", await response.Content.ReadAsStringAsync(ct)));
        }

        return await response.Content.ReadFromJsonAsync<ApiResponseDto<ThemePreferenceDto>>(cancellationToken: ct);
    }

    public async Task ResetThemeAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        await client.DeleteAsync("api/Theme/reset", ct);
    }

    public async Task<ApiResponseDto<List<string>>?> GetQuestionnaireTemplateAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<List<string>>>("api/Questionnaire/template", ct);
    }

    public async Task<ApiResponseDto<List<QuestionnaireTemplateItemDto>>?> GetQuestionnaireTemplateItemsAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<List<QuestionnaireTemplateItemDto>>>("api/Questionnaire/template-items", ct);
    }

    public async Task<ApiResponseDto<QuestionnaireTemplateItemDto>?> CreateQuestionnaireTemplateItemAsync(
        UpsertQuestionnaireTemplateDto request,
        CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Questionnaire/template-items", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<QuestionnaireTemplateItemDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> DeleteQuestionnaireTemplateItemAsync(Guid id, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.DeleteAsync($"api/Questionnaire/template-items/{id}", ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> SaveQuestionnaireResponsesAsync(
        SubmitQuestionnaireResponseDto request,
        CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Questionnaire/responses", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<decimal>?> GetOnboardingProgressAsync(List<OnboardingStepDto> steps, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Onboarding/progress", steps, ct);
        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponseDto<decimal>(false, 0m, new ApiErrorDto("onboarding_progress_failed", await response.Content.ReadAsStringAsync(ct)));
        }

        return await response.Content.ReadFromJsonAsync<ApiResponseDto<decimal>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<List<TenantSummaryDto>>?> GetTenantsAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<List<TenantSummaryDto>>>("api/AdminTenants", ct);
    }

    public async Task<ApiResponseDto<string>?> ToggleTenantAsync(Guid id, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PatchAsync($"api/AdminTenants/{id}/toggle", null, ct);
        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponseDto<string>(false, null, new ApiErrorDto("toggle_failed", await response.Content.ReadAsStringAsync(ct)));
        }

        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<TenantSummaryDto>?> CreateTenantAsync(CreateTenantAccountRequestDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/AdminTenants", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<TenantSummaryDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<PagedResultDto<ErpClientDto>>?> GetErpClientsAsync(
        int page = 1, int pageSize = 20, string? search = null, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var qs = $"api/AdminClients/erp-clients?PageNumber={page}&PageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
            qs += $"&Search={Uri.EscapeDataString(search)}";
        return await client.GetFromJsonAsync<ApiResponseDto<PagedResultDto<ErpClientDto>>>(qs, ct);
    }

    // ── Invoice endpoints ───────────────────────────────────────────────────
    public async Task<ApiResponseDto<InvoiceDetailDto>?> GetInvoiceDetailAsync(
        string invoiceNumber, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<InvoiceDetailDto>>(
            $"api/Invoices/{Uri.EscapeDataString(invoiceNumber)}", ct);
    }

    public async Task<(byte[] Bytes, string ContentType, string FileName)?> DownloadInvoicePdfAsync(
        string invoiceNumber, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Invoices/{Uri.EscapeDataString(invoiceNumber)}/pdf");
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/pdf";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName
            ?? $"{invoiceNumber}.pdf";
        fileName = fileName.Trim('"');
        return (bytes, contentType, fileName);
    }

    // ── Admin subscription plan endpoints ──────────────────────────────────
    public async Task<ApiResponseDto<List<SubscriptionPlanDto>>?> GetSubscriptionPlansAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<List<SubscriptionPlanDto>>>("api/AdminSubscriptionPlans", ct);
    }

    public async Task<ApiResponseDto<SubscriptionPlanDto>?> CreateSubscriptionPlanAsync(
        CreateSubscriptionPlanDto dto, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/AdminSubscriptionPlans", dto, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<SubscriptionPlanDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> UpdateSubscriptionPlanAsync(
        Guid id, UpdateSubscriptionPlanDto dto, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PutAsJsonAsync($"api/AdminSubscriptionPlans/{id}", dto, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> ToggleSubscriptionPlanAsync(Guid id, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PatchAsync($"api/AdminSubscriptionPlans/{id}/toggle", null, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> AssignSubscriptionPlanAsync(
        AssignSubscriptionPlanDto dto, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/AdminSubscriptionPlans/assign", dto, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<List<SubscriptionChangeLogDto>>?> GetSubscriptionChangeLogAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<List<SubscriptionChangeLogDto>>>("api/AdminSubscriptionPlans/change-log", ct);
    }

    // ── Customer subscription ───────────────────────────────────────────────
    public async Task<ApiResponseDto<CustomerSubscriptionDto>?> GetMySubscriptionAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<CustomerSubscriptionDto>>("api/CustomerSubscription", ct);
    }

    // -- Contracts -----------------------------------------------------------
    public async Task<ApiResponseDto<List<ContractSummaryDto>>?> GetContractsAsync(Guid? tenantId = null, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var endpoint = tenantId.HasValue
            ? $"api/Contracts?tenantId={tenantId.Value}"
            : "api/Contracts";
        return await client.GetFromJsonAsync<ApiResponseDto<List<ContractSummaryDto>>>(endpoint, ct);
    }

    public async Task<ApiResponseDto<List<ContractCustomerProfileOptionDto>>?> GetContractCustomerProfilesAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<List<ContractCustomerProfileOptionDto>>>("api/Contracts/profiles", ct);
    }

    public async Task<ApiResponseDto<ContractDetailDto>?> GetContractDetailAsync(Guid contractId, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<ContractDetailDto>>($"api/Contracts/{contractId}", ct);
    }

    public async Task<ApiResponseDto<ContractSummaryDto>?> CreateGeneratedContractAsync(CreateGeneratedContractDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Contracts/generated", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<ContractSummaryDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<ContractSummaryDto>?> UploadManualContractAsync(UploadManualContractDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Contracts/manual", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<ContractSummaryDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> UploadContractAssetAsync(UploadContractAssetDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Contracts/asset", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> TransitionContractStatusAsync(ContractStatusTransitionDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Contracts/status", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> UploadCustomerSignedManualContractAsync(UploadSignedContractDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Contracts/customer/manual-signed", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> ApplyCustomerDigitalSignatureAsync(CustomerApplySignatureDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Contracts/customer/apply-signature", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<(byte[] Bytes, string ContentType, string FileName)?> DownloadContractDocumentAsync(Guid documentId, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Contracts/documents/{documentId}/download");
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName
            ?? $"contract-{documentId}.bin";
        fileName = fileName.Trim('"');
        contentType = NormalizeBinaryContentType(contentType, fileName, bytes);
        return (bytes, contentType, fileName);
    }

    public async Task<ApiResponseDto<string>?> AddESignFieldsAsync(AddESignFieldsDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Contracts/esign-fields", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> SendContractAsync(SendContractDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Contracts/send", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<ESignSubmitResultDto>?> SubmitESignEntryAsync(SubmitESignEntryDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Contracts/esign-submit", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<ESignSubmitResultDto>>(cancellationToken: ct);
    }

    public async Task<(byte[] Bytes, string ContentType, string FileName)?> DownloadSignedPdfAsync(Guid contractId, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Contracts/signed-pdf/{contractId}");
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/pdf";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName
            ?? $"signed-contract-{contractId}.pdf";
        fileName = fileName.Trim('"');
        contentType = NormalizeBinaryContentType(contentType, fileName, bytes);
        return (bytes, contentType, fileName);
    }

    private static string NormalizeBinaryContentType(string contentType, string fileName, byte[] bytes)
    {
        if (string.Equals(contentType, "application/octet-stream", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(contentType))
        {
            if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                || (bytes.Length >= 4 && bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46))
            {
                return "application/pdf";
            }
        }

        return contentType;
    }

    // -- Support -------------------------------------------------------------
    public async Task<ApiResponseDto<SupportTicketDetailDto>?> GetSupportTicketDetailAsync(Guid ticketId, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<SupportTicketDetailDto>>($"api/SupportTickets/{ticketId}", ct);
    }

    public async Task<ApiResponseDto<AdminSupportTicketListDto>?> GetAdminSupportTicketsAsync(AdminSupportTicketFilterDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/SupportTickets/admin/list", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<AdminSupportTicketListDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> AdminReplySupportTicketAsync(AdminReplyTicketDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/SupportTickets/admin/reply", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> ChangeSupportTicketStatusAsync(ChangeTicketStatusDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/SupportTickets/admin/status", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> ResolveSupportTicketAsync(ResolveTicketDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/SupportTickets/admin/resolve", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    // -- Admin announcements -------------------------------------------------
    public async Task<ApiResponseDto<PagedResultDto<AnnouncementDto>>?> GetAdminAnnouncementsAsync(AnnouncementFilterDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Announcements/admin/list", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<AnnouncementDto>>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<AnnouncementDto>?> UpsertAnnouncementAsync(UpsertAnnouncementDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Announcements/admin/upsert", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<AnnouncementDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> PublishAnnouncementAsync(Guid announcementId, bool publish, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsync($"api/Announcements/admin/{announcementId}/publish?publish={publish}", null, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> DeleteAnnouncementAsync(Guid announcementId, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.DeleteAsync($"api/Announcements/admin/{announcementId}", ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    // -- Billing monitoring --------------------------------------------------
    public async Task<ApiResponseDto<BillingMonitoringResultDto>?> GetBillingMonitoringAsync(BillingMonitoringFilterDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Billing/monitoring", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<BillingMonitoringResultDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<string>?> RecordManualPaymentAsync(ManualPaymentCreateDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Billing/manual-payment", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<string>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<List<InvoicePaymentHistoryDto>>?> GetCustomerPaymentHistoryAsync(Guid customerProfileId, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<List<InvoicePaymentHistoryDto>>>($"api/Billing/{customerProfileId}/payments", ct);
    }

    public async Task<ApiResponseDto<List<PaymentModeHistoryDto>>?> GetCustomerPaymentModesAsync(Guid customerProfileId, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<List<PaymentModeHistoryDto>>>($"api/Billing/{customerProfileId}/payment-modes", ct);
    }

    // -- Audit reports -------------------------------------------------------
    public async Task<ApiResponseDto<PagedResultDto<AuditReportItemDto>>?> GetAuditReportAsync(AuditReportFilterDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Reports/audit", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<AuditReportItemDto>>>(cancellationToken: ct);
    }

    public async Task<(byte[] Bytes, string ContentType, string FileName)?> ExportAuditCsvAsync(AuditReportFilterDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/Reports/audit-export", request, ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "text/csv";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName
            ?? $"audit-report-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        fileName = fileName.Trim('"');
        return (bytes, contentType, fileName);
    }

    // -- Razorpay settings and payment checkout -----------------------------
    public async Task<ApiResponseDto<RazorpaySettingsViewDto>?> GetRazorpaySettingsAsync(CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        return await client.GetFromJsonAsync<ApiResponseDto<RazorpaySettingsViewDto>>("api/RazorpayAdmin/settings", ct);
    }

    public async Task<ApiResponseDto<RazorpaySettingsViewDto>?> SaveRazorpaySettingsAsync(RazorpaySettingsUpsertDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/RazorpayAdmin/settings", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<RazorpaySettingsViewDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<PaymentPreviewDto>?> GetPaymentPreviewAsync(PaymentPreviewRequestDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/RazorpayCheckout/preview", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<PaymentPreviewDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<RazorpayOrderCreateResponseDto>?> CreateRazorpayOrderAsync(RazorpayOrderCreateRequestDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/RazorpayCheckout/create-order", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<RazorpayOrderCreateResponseDto>>(cancellationToken: ct);
    }

    public async Task<ApiResponseDto<PaymentProcessResultDto>?> VerifyRazorpayPaymentAsync(RazorpayVerifyRequestDto request, CancellationToken ct = default)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.PostAsJsonAsync("api/RazorpayCheckout/verify", request, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponseDto<PaymentProcessResultDto>>(cancellationToken: ct);
    }

    private HttpClient CreateAuthenticatedClient()
    {
        if (!session.IsAuthenticated || session.Auth is null)
        {
            throw new InvalidOperationException("User is not authenticated.");
        }

        var client = httpClientFactory.CreateClient("WrkPlanApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.Auth.AccessToken);
        if (!string.IsNullOrWhiteSpace(session.TenantKey))
        {
            client.DefaultRequestHeaders.Remove("X-Tenant-Key");
            client.DefaultRequestHeaders.Add("X-Tenant-Key", session.TenantKey);
        }

        return client;
    }
}

# WrkPlan Multi-Tenant Platform (.NET 8)

Production-grade clean architecture solution with two modules:

- Customer Portal (customer-facing Blazor Web App)
- Customer Hub (internal WrkPlan team dashboard/back-office)

## 1) Completed Feature Summary

### Customer Hub (Internal)
- Tenant provisioning workflow endpoint with:
	- tenant registry creation
	- tenant DB mapping persistence
	- baseline customer profile + admin user generation
- Customer lifecycle base endpoints (admin clients, dashboard, reports, audit)
- Existing admin DB table inspection endpoint for `dbo.TB_ERP_CLIENT_DETAIL`
- Sales/billing/contracts/onboarding/support/integrations API surfaces scaffolded with DTOs and Swagger metadata
- Stripe webhook processing with signature verification + idempotency handling
- First payment activation flow toggles tenant and portal active state
- Background reminder worker foundation (renewals/payment method checks)

### Customer Portal (External)
- Modern Blazor dashboard shell with:
	- animated cards
	- responsive layouts
	- status semantics (`ok`, `warning`, `overdue`)
- Login path choice page:
	- Customer Portal Login
	- Customer Hub Login
	- Partner Portal stub architecture
- Onboarding hub with step gating/dependency logic UI
- Theme settings page:
	- light/dark mode switch
	- RGB background preference input with validation and live preview
	- reset to default
	- persisted to local storage (and API contract in place)
- Plan recommendation helper component

## 2) Final Project Structure

- `WrkPlan.CustomerPortal.UI` (Blazor Web App)
- `WrkPlan.CustomerPortal.API` (ASP.NET Core Web API)
- `WrkPlan.CustomerPortal.Application` (use-cases/services/interfaces)
- `WrkPlan.CustomerPortal.Domain` (entities/enums)
- `WrkPlan.CustomerPortal.Infrastructure` (EF Core, multi-tenancy, Stripe, seeds)
- `WrkPlan.CustomerPortal.Shared` (DTOs/contracts)
- `WrkPlan.CustomerPortal.Tests` (critical unit tests)

## 3) DB Migrations / Table Summary

EF Core migrations generated under:
- `WrkPlan.CustomerPortal.Infrastructure/Data/Migrations`

Created migration set:
- `InitialAdminSchema`
- `DecimalPrecisionFix`

Admin model includes (as requested):
- Tenant / security: `TenantRegistry`, `TenantConnectionMap`, `Users`, `Roles`, `UserRoles`, `Permissions`
- Customer/account: `CustomerProfiles`, `Contacts`, `NotificationSettings`, `CustomerThemePreferences`
- Subscription/services: `Subscriptions`, `Modules`, `ServiceChangeHistory`, `TSIncreaseRequests`
- Sales/contracts: `SalesQuotes`, `SalesOrders`, `Amendments`, `AgreementAcknowledgements`, `Contracts`, `ContractDocuments`
- Billing/payments: `BillingContacts`, `PaymentMethodsMeta`, `Invoices`, `InvoiceItems`, `Payments`, `PaymentEvents`
- Onboarding: `OnboardingWorkflows`, `OnboardingSteps`, `ImplementationPlans`, `QuestionnaireResponses`
- Support/content: `SupportTickets`, `TicketComments`, `Escalations`, `Announcements`, `ResourceFiles`, `Downloads`
- Requests/ops/audit: `FeatureRequests`, `TrainingRequests`, `ArchiveRequests`, `ActivityLogs`, `LoginAuditLogs`, `NotificationQueue`, `IntegrationEvents`
- Existing external/admin table mapping: `dbo.TB_ERP_CLIENT_DETAIL`

Seed data includes:
- 3 customers
- subscriptions
- invoices (including overdue)
- support tickets
- announcements

### 3.1) New Table Dictionary (Added in This Solution)

All EF entities below are persisted in the admin database unless explicitly noted.

| Table | Purpose | Key Columns |
|---|---|---|
| `TenantRegistries` | Master tenant registry and activation state | `Id`, `TenantKey` (unique), `Name`, `IsActive`, `ActivatedUtc` |
| `TenantConnectionMaps` | Tenant-to-database mapping | `Id`, `TenantRegistryId`, `DatabaseName`, `EncryptedConnectionString` |
| `Users` | Internal/customer user accounts | `Id`, `TenantId`, `Email`, `PasswordHash`, `DisplayName`, `IsTwoFactorEnabled` |
| `Roles` | Role catalog | `Id`, `Name` |
| `UserRoles` | User-role assignments | `Id`, `UserId`, `RoleId` |
| `Permissions` | Permission key catalog | `Id`, `Key`, `Description` |
| `CustomerProfiles` | Customer organization profile | `Id`, `TenantId`, `CompanyName`, `City`, `Country`, `Status`, `IsPortalActive`, `IsPartnerPortalEnabled` |
| `Contacts` | Customer contacts | `Id`, `CustomerProfileId`, `Name`, `Email`, `ContactType` |
| `NotificationSettings` | Customer notification preferences | `Id`, `CustomerProfileId`, `BillingAlerts`, `RenewalAlerts`, `ProductAnnouncements` |
| `Subscriptions` | Plan and billing-cycle subscription record | `Id`, `CustomerProfileId`, `PlanName`, `Price`, `Cycle`, `StartDateUtc`, `EndDateUtc`, `RenewalDateUtc`, `AutoRenew` |
| `Modules` | Subscribed product modules and license allocation | `Id`, `SubscriptionId`, `Name`, `LicenseCount` |
| `ServiceChangeHistories` | Subscription/service audit events | `Id`, `SubscriptionId`, `ChangeType`, `Notes` |
| `TSIncreaseRequests` | Additional user/license requests | `Id`, `CustomerProfileId`, `RequestedAdditionalUsers`, `Status` |
| `SalesQuotes` | Sales quotation records | `Id`, `CustomerProfileId`, `QuoteNumber`, `Amount`, `Status` |
| `SalesOrders` | Executed sales order from quote | `Id`, `SalesQuoteId`, `OrderNumber`, `ExecutedUtc` |
| `Amendments` | Post-order contract/order amendments | `Id`, `SalesOrderId`, `AmendmentNumber`, `Summary` |
| `AgreementAcknowledgements` | Agreement acceptance tracking | `Id`, `CustomerProfileId`, `AgreementUrl`, `AgreementVersion`, `AcknowledgedUtc` |
| `Contracts` | Contract lifecycle records | `Id`, `CustomerProfileId`, `ContractNumber`, `EffectiveUtc`, `RenewalUtc` |
| `ContractDocuments` | Contract document metadata | `Id`, `ContractId`, `FileName`, `BlobPath` |
| `BillingContacts` | Billing-specific customer contacts | `Id`, `CustomerProfileId`, `Name`, `Email` |
| `PaymentMethodsMeta` | Stored payment method metadata | `Id`, `CustomerProfileId`, `StripePaymentMethodId`, `Last4`, `ExpMonth`, `ExpYear`, `IsDefault` |
| `Invoices` | Billing invoices | `Id`, `CustomerProfileId`, `InvoiceNumber`, `Total`, `DueUtc`, `Status` |
| `InvoiceItems` | Invoice line items | `Id`, `InvoiceId`, `Description`, `Amount` |
| `Payments` | Payment transactions against invoices | `Id`, `InvoiceId`, `Amount`, `PaidUtc`, `ProviderRef` |
| `PaymentEvents` | Payment/webhook event log and idempotency control | `Id`, `CustomerProfileId`, `Provider`, `EventType`, `IdempotencyKey` (unique), `RawPayload` |
| `OnboardingWorkflows` | Onboarding workflow instance per customer | `Id`, `CustomerProfileId`, `Name`, `Status` |
| `OnboardingSteps` | Onboarding step definitions/state | `Id`, `OnboardingWorkflowId`, `StepType`, `Title`, `IsRequired`, `SortOrder`, `IsCompleted`, `DependsOnStepId` |
| `ImplementationPlans` | Implementation plan payload storage | `Id`, `CustomerProfileId`, `JsonPlan` |
| `QuestionnaireResponses` | Questionnaire answers per customer | `Id`, `CustomerProfileId`, `QuestionKey`, `ResponseValue` |
| `SupportTickets` | Customer support cases | `Id`, `CustomerProfileId`, `Subject`, `Priority`, `Status` |
| `TicketComments` | Ticket conversation/comments | `Id`, `SupportTicketId`, `Body` |
| `Escalations` | Ticket escalation metadata | `Id`, `SupportTicketId`, `Reason` |
| `Announcements` | Customer announcements/news | `Id`, `CustomerProfileId`, `Title`, `Message` |
| `ResourceFiles` | Knowledge/resource links/files | `Id`, `CustomerProfileId`, `Name`, `Url` |
| `Downloads` | Resource download events | `Id`, `ResourceFileId`, `UserId` |
| `FeatureRequests` | Product feature requests | `Id`, `CustomerProfileId`, `Summary` |
| `TrainingRequests` | Training request queue | `Id`, `CustomerProfileId`, `RequestedUtc`, `Notes` |
| `ArchiveRequests` | Archive/decommission requests | `Id`, `CustomerProfileId`, `Reason` |
| `ActivityLogs` | General activity audit log | `Id`, `TenantId`, `UserId`, `Action` |
| `LoginAuditLogs` | Login attempts and result audit | `Id`, `TenantId`, `UserId`, `IpAddress`, `Success` |
| `NotificationQueue` | Outbound scheduled notifications | `Id`, `TenantId`, `Channel`, `Payload`, `ScheduledUtc` |
| `IntegrationEvents` | External integration event journal | `Id`, `TenantId`, `Provider`, `EventName`, `Payload` |
| `CustomerThemePreferences` | Tenant/customer UI theme profile | `Id`, `CustomerProfileId`, `IsDarkMode`, `BackgroundR`, `BackgroundG`, `BackgroundB` |
| `dbo.TB_ERP_CLIENT_DETAIL` | Existing external/admin ERP client table mapped for inspection | `Id`, `ClientName`, `ClientCode` |

### 3.2) Cross-Cutting Schema Rules

- Most entity tables inherit `BaseEntity` and include: `Id`, `CreatedUtc`, `UpdatedUtc`.
- Decimal precision is configured as `decimal(18,2)` for money-like columns: `Subscriptions.Price`, `SalesQuotes.Amount`, `Invoices.Total`, `InvoiceItems.Amount`, `Payments.Amount`.
- Unique constraints:
	- `TenantRegistries.TenantKey`
	- `PaymentEvents.IdempotencyKey`

## 4) API Endpoint Map

Implemented controller set in API project:

- `AuthController`
- `TenantProvisioningController`
- `CustomerHubController`
- `AdminClientsController`
- `SalesQuotesController`
- `ContractsController`
- `SubscriptionsController`
- `BillingController`
- `PaymentsController` (Stripe)
- `InvoicesController`
- `DashboardController`
- `SupportTicketsController`
- `AnnouncementsController`
- `KnowledgeBaseController`
- `OnboardingController`
- `QuestionnaireController`
- `FilesController`
- `CustomerSuccessController`
- `ReportsController`
- `AuditController`
- `IntegrationsController`
- `ThemeController`

Important routes:
- `POST /api/Auth/login`
- `POST /api/TenantProvisioning`
- `GET /api/CustomerHub/dashboard`
- `GET /api/AdminClients/erp-client-detail`
- `POST /api/Payments/checkout`
- `POST /api/Payments/stripe/webhook`
- `POST /api/Files/transform`
- `GET /api/Theme`, `PUT /api/Theme`, `DELETE /api/Theme/reset`

Swagger is enabled in development.

## 5) Stripe Setup / Config

Set values in API config or secret store:

- `Stripe:PublishableKey`
- `Stripe:SecretKey`
- `Stripe:WebhookSecret`

Webhook endpoint:
- `POST /api/Payments/stripe/webhook`

Implemented payment flow:
- checkout session creation (one-time/subscription)
- signature verification
- idempotent event handling (`PaymentEvents` unique `IdempotencyKey`)
- tenant activation trigger on `checkout.session.completed`

## 6) Demo Users / Roles / Credentials

Seed/customer samples:
- `acme`
- `northwind`
- `bluepeak`

Provisioning-generated initial customer admin password:
- `Welcome@123`

JWT role usage:
- internal policies expect `WrkPlanAdmin`
- customer usage expects tenant claim (`tenant_id`)

## 7) Assumptions / Deferred Enhancements

Implemented now:
- end-to-end architecture skeleton
- tenant middleware and strict API tenant context foundation
- key workflows and test coverage for high-risk logic

Deferred for next increments:
- full granular RBAC permission matrix with policy handlers
- full production auth stack (refresh token store, lockout, MFA provider wiring)
- full quote execution/e-sign document state machine
- complete UI coverage for every module page and CRUD forms
- production-grade provider connectors (Microsoft Graph email/calendar, DocuSign/PandaDoc/AdobeSign concrete adapters)
- robust tenant database physical provisioning automation scripts per environment
- advanced charting and cross-tenant analytics widgets

## 8) Run Locally End-to-End

1. Build solution

```powershell
dotnet build WrkPlan.CustomerPortal.sln
```

2. Apply migrations (admin DB)

```powershell
dotnet ef database update --project WrkPlan.CustomerPortal.Infrastructure/WrkPlan.CustomerPortal.Infrastructure.csproj --startup-project WrkPlan.CustomerPortal.API/WrkPlan.CustomerPortal.API.csproj --context WrkPlan.CustomerPortal.Infrastructure.Data.AdminDbContext
```

3. Run API

```powershell
dotnet run --project WrkPlan.CustomerPortal.API/WrkPlan.CustomerPortal.API.csproj
```

4. Run UI

```powershell
dotnet run --project WrkPlan.CustomerPortal.UI/WrkPlan.CustomerPortal.UI.csproj
```

5. Open:
- API Swagger: `https://localhost:<api-port>/swagger`
- UI: `https://localhost:<ui-port>/`

## Multi-Tenant Resolution Strategy

Current strategy is hybrid claim/header resolution:
- primary: `tenant_id` claim from JWT
- fallback: `X-Tenant-Key` request header

This is implemented in `TenantResolverMiddleware` and can be extended to subdomain-based resolution.

## Theme Tokens / Branding Notes

The UI uses reusable design tokens and CSS variables in layout styles:
- primary/secondary/accent
- background/surface
- text primary/secondary
- semantic colors (success/warning/error)

Tenant RGB preference is injected as CSS custom properties and applied safely in gradients.
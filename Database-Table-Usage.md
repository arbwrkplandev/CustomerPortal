# WrkPlan Customer Portal Database/Table Usage

## Database Name
- Primary database: `nextgen_admin`
- Source: API connection string default in `WrkPlan.CustomerPortal.API/Program.cs`

## Table Inventory and Usage

| Table / DbSet | Purpose | Used In Project |
|---|---|---|
| TenantRegistries (`TenantRegistry`) | Master tenant registry (name, key, active status) | Auth, Admin tenant list/create/toggle, provisioning service |
| TenantConnectionMaps (`TenantConnectionMap`) | Stores tenant DB mapping | Tenant provisioning service |
| Users (`AppUser`) | Tenant user credentials | Auth login password validation, tenant creation/provisioning |
| Roles (`AppRole`) | Role master | Schema only (not actively queried in current controllers) |
| UserRoles (`UserRole`) | User-role mapping | Schema only |
| Permissions (`Permission`) | Permission catalog | Schema only |
| CustomerProfiles (`CustomerProfile`) | Customer company profile per tenant | Dashboards, invoices, tickets, subscriptions, onboarding mapping |
| Contacts (`Contact`) | Contact records | Schema only |
| NotificationSettings (`NotificationSetting`) | Alert preference toggles | Schema only |
| Subscriptions (`Subscription`) | Plan and billing cycle data | Hub KPIs, invoice detail fallback, subscription APIs, plan assignment |
| Modules (`Module`) | Subscription module licensing | Schema only |
| ServiceChangeHistories (`ServiceChangeHistory`) | Subscription/service history | Schema only |
| TSIncreaseRequests (`TSIncreaseRequest`) | Team-size increase requests | Schema only |
| SalesQuotes (`SalesQuote`) | Quote records | Placeholder endpoint |
| SalesOrders (`SalesOrder`) | Sales order records | Schema only |
| Amendments (`Amendment`) | Commercial amendments | Schema only |
| AgreementAcknowledgements (`AgreementAcknowledgement`) | Agreement acceptance history | Schema only |
| Contracts (`Contract`) | Contract headers | Placeholder endpoint |
| ContractDocuments (`ContractDocument`) | Contract files | Schema only |
| BillingContacts (`BillingContact`) | Billing contacts | Schema only |
| PaymentMethodsMeta (`PaymentMethodMeta`) | Payment method metadata | Reminder service scans this table |
| Invoices (`Invoice`) | Customer invoices | Invoice list/detail/pdf, dashboard widgets, seed/demo data |
| InvoiceItems (`InvoiceItem`) | Invoice line items | Invoice detail API |
| Payments (`Payment`) | Payment transactions | Invoice detail API |
| PaymentEvents (`PaymentEvent`) | Stripe webhook idempotency/event log | Stripe webhook handling |
| OnboardingWorkflows (`OnboardingWorkflow`) | Workflow header | Schema only |
| OnboardingSteps (`OnboardingStep`) | Workflow step rows | Progress logic uses DTO payload (table currently not queried) |
| ImplementationPlans (`ImplementationPlan`) | JSON implementation plans | Schema only |
| QuestionnaireResponses (`QuestionnaireResponse`) | Questionnaire responses and template storage | Customer onboarding save; admin template CRUD |
| SupportTickets (`SupportTicket`) | Customer support tickets | Ticket APIs, dashboard cards, seed/demo data |
| TicketComments (`TicketComment`) | Ticket comment thread | Ticket create API |
| Escalations (`Escalation`) | Escalation records | Schema only |
| Announcements (`Announcement`) | Tenant announcements | Announcement APIs, dashboard cards, seed/demo data |
| ResourceFiles (`ResourceFile`) | Knowledge/resource files | Placeholder endpoint scope |
| Downloads (`Download`) | File download logs | Schema only |
| FeatureRequests (`FeatureRequest`) | Feature request records | Schema only |
| TrainingRequests (`TrainingRequest`) | Training session requests | Schema only |
| ArchiveRequests (`ArchiveRequest`) | Archive requests | Schema only |
| ActivityLogs (`ActivityLog`) | Operational activity logs | Schema only |
| LoginAuditLogs (`LoginAuditLog`) | Login audit records | Audit placeholder endpoint scope |
| NotificationQueue (`NotificationQueue`) | Outbound notification queue | Schema only |
| IntegrationEvents (`IntegrationEvent`) | Integration event payloads | Integration placeholder scope |
| CustomerThemePreferences (`CustomerThemePreference`) | Per-customer theme settings | Theme APIs/services |
| TB_ERP_CLIENT_DETAIL (`TbErpClientDetail`) | External ERP client registry | Admin ERP listing endpoint |
| SubscriptionPlans (`SubscriptionPlan`) | Plan catalog | Admin plan CRUD, assignment, customer subscription view |
| SubscriptionChangeLogs (`SubscriptionChangeLog`) | Plan assignment audit trail | Admin change log endpoint |

## Key Files Where Table Access Occurs
- `WrkPlan.CustomerPortal.API/Controllers/ApiControllers.cs`
- `WrkPlan.CustomerPortal.Infrastructure/Services/CoreInfraServices.cs`
- `WrkPlan.CustomerPortal.Infrastructure/Data/SeedData.cs`
- `WrkPlan.CustomerPortal.Infrastructure/Data/AdminDbContext.cs`

## Notes
- This project uses EF Core with `AdminDbContext` as the central schema map.
- Several tables are currently defined for full platform scope but not yet actively queried by live UI/API workflows.
- `TB_ERP_CLIENT_DETAIL` query includes a fallback to fully qualified `[nextgen_admin].[dbo].[TB_ERP_CLIENT_DETAIL]` to handle server-default DB mismatches.

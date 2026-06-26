using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WrkPlan.CustomerPortal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdminBillingRazorpayAuditFlows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mode",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastPaymentMode",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPaymentUtc",
                table: "Invoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OutstandingAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Announcements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGlobal",
                table: "Announcements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Announcements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedUtc",
                table: "Announcements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Announcements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AnnouncementTenantTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnnouncementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementTenantTargets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OccurredUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManualPaymentEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualPaymentEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentModeHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ManualPaymentEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Mode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentModeHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RazorpaySettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyIdEncrypted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeySecretEncrypted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebhookSecretEncrypted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTestMode = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastValidatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorpaySettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RazorpayWebhookEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessingError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorpayWebhookEvents", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "IsDeleted", "IsGlobal", "IsPublished", "PublishedUtc", "TenantId", "Topic" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9377), false, false, true, null, null, "" });

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "IsDeleted", "IsGlobal", "IsPublished", "PublishedUtc", "TenantId", "Topic" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9385), false, false, true, null, null, "" });

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "IsDeleted", "IsGlobal", "IsPublished", "PublishedUtc", "TenantId", "Topic" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9389), false, false, true, null, null, "" });

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9078));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9084));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9099));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "DueUtc", "LastPaymentMode", "LastPaymentUtc", "OutstandingAmount", "PaymentStatus" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9278), new DateTime(2026, 7, 6, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9282), null, null, 0m, "Unpaid" });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc", "LastPaymentMode", "LastPaymentUtc", "OutstandingAmount", "PaymentStatus" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9291), new DateTime(2026, 6, 23, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9294), null, null, 0m, "Unpaid" });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc", "LastPaymentMode", "LastPaymentUtc", "OutstandingAmount", "PaymentStatus" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9296), new DateTime(2026, 7, 10, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9298), null, null, 0m, "Unpaid" });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9224));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9230));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9234));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9151), new DateTime(2027, 2, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9163), new DateTime(2026, 7, 1, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9165), new DateTime(2026, 2, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9156) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9171), new DateTime(2027, 4, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9175), new DateTime(2026, 7, 23, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9175), new DateTime(2026, 4, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9174) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9178), new DateTime(2026, 11, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9181), new DateTime(2026, 6, 28, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9182), new DateTime(2025, 11, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9180) });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9332));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9337));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9341));

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(8680), new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(8676) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(8687), new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(8685) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(8690), new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(8689) });

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementTenantTargets_AnnouncementId_TenantId",
                table: "AnnouncementTenantTargets",
                columns: new[] { "AnnouncementId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_OccurredUtc_EventType",
                table: "AuditEvents",
                columns: new[] { "OccurredUtc", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_RazorpaySettings_TenantId",
                table: "RazorpaySettings",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RazorpayWebhookEvents_EventId",
                table: "RazorpayWebhookEvents",
                column: "EventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnouncementTenantTargets");

            migrationBuilder.DropTable(
                name: "AuditEvents");

            migrationBuilder.DropTable(
                name: "ManualPaymentEntries");

            migrationBuilder.DropTable(
                name: "PaymentModeHistory");

            migrationBuilder.DropTable(
                name: "RazorpaySettings");

            migrationBuilder.DropTable(
                name: "RazorpayWebhookEvents");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "LastPaymentMode",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "LastPaymentUtc",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "OutstandingAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "IsGlobal",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "PublishedUtc",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "Announcements");

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8331));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8338));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8341));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7885));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7891));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7896));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8223), new DateTime(2026, 7, 6, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8227) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8230), new DateTime(2026, 6, 23, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8232) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8239), new DateTime(2026, 7, 10, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8240) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8164));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8170));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8175));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7993), new DateTime(2027, 2, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8006), new DateTime(2026, 7, 1, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8007), new DateTime(2026, 2, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7998) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8098), new DateTime(2027, 4, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8103), new DateTime(2026, 7, 23, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8104), new DateTime(2026, 4, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8102) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8107), new DateTime(2026, 11, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8110), new DateTime(2026, 6, 28, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8111), new DateTime(2025, 11, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8110) });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8282));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8287));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8291));

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7549), new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7544) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7557), new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7555) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7560), new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(7559) });
        }
    }
}

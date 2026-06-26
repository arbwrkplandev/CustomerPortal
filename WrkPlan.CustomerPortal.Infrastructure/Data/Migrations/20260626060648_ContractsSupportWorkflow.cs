using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WrkPlan.CustomerPortal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContractsSupportWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorEmail",
                table: "TicketComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorRole",
                table: "TicketComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResolutionMessage",
                table: "SupportTickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResolvedInDays",
                table: "SupportTickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedUtc",
                table: "SupportTickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketNumber",
                table: "SupportTickets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkflowStatus",
                table: "SupportTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedUtc",
                table: "Contracts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerSignedUtc",
                table: "Contracts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryUtc",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastActionBy",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentUtc",
                table: "Contracts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ViewedUtc",
                table: "Contracts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ContractDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                table: "ContractDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsLatest",
                table: "ContractDocuments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSigned",
                table: "ContractDocuments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "SizeBytes",
                table: "ContractDocuments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "VersionLabel",
                table: "ContractDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ContractAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlobPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractSignatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignerRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureBlobPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlacementXPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    PlacementYPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SignedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractSignatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractStatusHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupportTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsResolution = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupportTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedByRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatusHistories", x => x.Id);
                });

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
                columns: new[] { "CreatedUtc", "ResolutionMessage", "ResolvedInDays", "ResolvedUtc", "TicketNumber", "WorkflowStatus" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8282), null, null, null, "TKT-20260626-0001", "Open" });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "ResolutionMessage", "ResolvedInDays", "ResolvedUtc", "TicketNumber", "WorkflowStatus" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8287), null, null, null, "TKT-20260626-0002", "Open" });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "ResolutionMessage", "ResolvedInDays", "ResolvedUtc", "TicketNumber", "WorkflowStatus" },
                values: new object[] { new DateTime(2026, 6, 26, 6, 6, 46, 211, DateTimeKind.Utc).AddTicks(8291), null, null, null, "TKT-20260626-0003", "Open" });

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

            migrationBuilder.Sql(@"
;WITH seq AS (
    SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedUtc, Id) AS rn
    FROM SupportTickets
)
UPDATE st
SET TicketNumber = CONCAT('TKT-', FORMAT(GETUTCDATE(),'yyyyMMdd'), '-', RIGHT('0000' + CAST(seq.rn AS varchar(10)), 4))
FROM SupportTickets st
INNER JOIN seq ON seq.Id = st.Id;");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_TicketNumber",
                table: "SupportTickets",
                column: "TicketNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractDocuments_ContractId_IsLatest",
                table: "ContractDocuments",
                columns: new[] { "ContractId", "IsLatest" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractAssets");

            migrationBuilder.DropTable(
                name: "ContractSignatures");

            migrationBuilder.DropTable(
                name: "ContractStatusHistories");

            migrationBuilder.DropTable(
                name: "TicketMessages");

            migrationBuilder.DropTable(
                name: "TicketStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_TicketNumber",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_ContractDocuments_ContractId_IsLatest",
                table: "ContractDocuments");

            migrationBuilder.DropColumn(
                name: "AuthorEmail",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "AuthorRole",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "ResolutionMessage",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "ResolvedInDays",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "ResolvedUtc",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "TicketNumber",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "WorkflowStatus",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "CompletedUtc",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "CustomerSignedUtc",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ExpiryUtc",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "LastActionBy",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SentUtc",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ViewedUtc",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ContractDocuments");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "ContractDocuments");

            migrationBuilder.DropColumn(
                name: "IsLatest",
                table: "ContractDocuments");

            migrationBuilder.DropColumn(
                name: "IsSigned",
                table: "ContractDocuments");

            migrationBuilder.DropColumn(
                name: "SizeBytes",
                table: "ContractDocuments");

            migrationBuilder.DropColumn(
                name: "VersionLabel",
                table: "ContractDocuments");

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3640));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3644));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3646));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3440));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3445));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3447));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3571), new DateTime(2026, 7, 5, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3577) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3579), new DateTime(2026, 6, 22, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3581) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3585), new DateTime(2026, 7, 9, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3587) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3540));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3545));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3548));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3485), new DateTime(2027, 2, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3499), new DateTime(2026, 6, 30, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3500), new DateTime(2026, 2, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3492) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3506), new DateTime(2027, 4, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3509), new DateTime(2026, 7, 22, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3510), new DateTime(2026, 4, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3508) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3511), new DateTime(2026, 11, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3514), new DateTime(2026, 6, 27, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3514), new DateTime(2025, 11, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3513) });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3610));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3614));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3616));

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3203), new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3200) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3207), new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3207) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3210), new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3209) });
        }
    }
}

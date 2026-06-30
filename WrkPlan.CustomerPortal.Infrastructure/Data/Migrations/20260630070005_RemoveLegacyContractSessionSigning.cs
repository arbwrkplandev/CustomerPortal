using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WrkPlan.CustomerPortal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLegacyContractSessionSigning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractFieldSignatures");

            migrationBuilder.DropTable(
                name: "ContractSignatureSessions");

            migrationBuilder.DropColumn(
                name: "ContractStatus",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SignatureFields",
                table: "Contracts");

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2363));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2365));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2028));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2046));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2048));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2287), new DateTime(2026, 7, 10, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2290) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2292), new DateTime(2026, 6, 27, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2295) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2296), new DateTime(2026, 7, 14, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2298) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2171));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2176));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2178));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2102), new DateTime(2027, 2, 28, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2121), new DateTime(2026, 7, 5, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2122), new DateTime(2026, 2, 28, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2110) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2127), new DateTime(2027, 4, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2131), new DateTime(2026, 7, 27, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2131), new DateTime(2026, 4, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2130) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2137), new DateTime(2026, 11, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2140), new DateTime(2026, 7, 2, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2140), new DateTime(2025, 11, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2139) });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2324));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2333));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(2335));

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(1810), new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(1806) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(1815), new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(1814) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(1820), new DateTime(2026, 6, 30, 7, 0, 4, 404, DateTimeKind.Utc).AddTicks(1819) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractStatus",
                table: "Contracts",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SignatureFields",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ContractSignatureSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiresUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    SessionToken = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SignedPdfBlobPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractSignatureSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractSignatureSessions_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractFieldSignatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractSignatureSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FieldId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SignatureDataUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedByName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractFieldSignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractFieldSignatures_ContractSignatureSessions_ContractSignatureSessionId",
                        column: x => x.ContractSignatureSessionId,
                        principalTable: "ContractSignatureSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9121));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9125));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9133));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8905));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8923));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8925));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9057), new DateTime(2026, 7, 10, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9060) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9062), new DateTime(2026, 6, 27, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9063) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9065), new DateTime(2026, 7, 14, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9066) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9018));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9023));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9026));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8961), new DateTime(2027, 2, 28, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8973), new DateTime(2026, 7, 5, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8974), new DateTime(2026, 2, 28, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8965) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8980), new DateTime(2027, 4, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8982), new DateTime(2026, 7, 27, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8983), new DateTime(2026, 4, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8982) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8985), new DateTime(2026, 11, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8987), new DateTime(2026, 7, 2, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8988), new DateTime(2025, 11, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8986) });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9090));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9094));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(9097));

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8649), new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8646) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8654), new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8653) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8657), new DateTime(2026, 6, 30, 6, 50, 38, 987, DateTimeKind.Utc).AddTicks(8656) });

            migrationBuilder.CreateIndex(
                name: "IX_ContractFieldSignatures_ContractSignatureSessionId_FieldId",
                table: "ContractFieldSignatures",
                columns: new[] { "ContractSignatureSessionId", "FieldId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractSignatureSessions_ContractId_CustomerProfileId_IsCompleted",
                table: "ContractSignatureSessions",
                columns: new[] { "ContractId", "CustomerProfileId", "IsCompleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractSignatureSessions_SessionToken",
                table: "ContractSignatureSessions",
                column: "SessionToken",
                unique: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WrkPlan.CustomerPortal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContractESignSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractStatus",
                table: "Contracts",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SentToCustomerUtc",
                table: "Contracts",
                type: "datetime2",
                nullable: true);

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
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionToken = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpiresUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SignedPdfBlobPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    FieldId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SignatureDataUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedByName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4445));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4449));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4451));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4112));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4117));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4128));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4263), new DateTime(2026, 7, 10, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4267) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4272), new DateTime(2026, 6, 27, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4274) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4275), new DateTime(2026, 7, 14, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4277) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4224));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4228));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4231));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4160), new DateTime(2027, 2, 28, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4173), new DateTime(2026, 7, 5, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4175), new DateTime(2026, 2, 28, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4167) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4181), new DateTime(2027, 4, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4187), new DateTime(2026, 7, 27, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4187), new DateTime(2026, 4, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4186) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4189), new DateTime(2026, 11, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4191), new DateTime(2026, 7, 2, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4191), new DateTime(2025, 11, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4190) });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4410));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4415));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(4418));

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(3917), new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(3914) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(3922), new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(3921) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(3924), new DateTime(2026, 6, 30, 5, 48, 43, 726, DateTimeKind.Utc).AddTicks(3923) });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractFieldSignatures");

            migrationBuilder.DropTable(
                name: "ContractSignatureSessions");

            migrationBuilder.DropColumn(
                name: "ContractStatus",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SentToCustomerUtc",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SignatureFields",
                table: "Contracts");

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9377));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9385));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9389));

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
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9278), new DateTime(2026, 7, 6, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9282) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9291), new DateTime(2026, 6, 23, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9294) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 26, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9296), new DateTime(2026, 7, 10, 7, 11, 40, 511, DateTimeKind.Utc).AddTicks(9298) });

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
        }
    }
}

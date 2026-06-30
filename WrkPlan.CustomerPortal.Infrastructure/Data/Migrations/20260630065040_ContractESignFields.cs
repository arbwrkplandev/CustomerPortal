using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WrkPlan.CustomerPortal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContractESignFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ESignFieldsJson",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ESignStatus",
                table: "Contracts",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SignedPdfPath",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContractESignEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FieldLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueDataUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedByName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractESignEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractESignEntries_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
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
                name: "IX_ContractESignEntries_ContractId_FieldId",
                table: "ContractESignEntries",
                columns: new[] { "ContractId", "FieldId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractESignEntries");

            migrationBuilder.DropColumn(
                name: "ESignFieldsJson",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ESignStatus",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SignedPdfPath",
                table: "Contracts");

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
        }
    }
}

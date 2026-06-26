using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WrkPlan.CustomerPortal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionPlanManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriptionChangeLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldPlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewPlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cycle = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FeaturesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 35, 55, 668, DateTimeKind.Utc).AddTicks(222));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 35, 55, 668, DateTimeKind.Utc).AddTicks(226));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 35, 55, 668, DateTimeKind.Utc).AddTicks(230));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8173));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8181));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8199));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8342), new DateTime(2026, 7, 5, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8351) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8354), new DateTime(2026, 6, 22, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8356) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8358), new DateTime(2026, 7, 9, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8359) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8256), new DateTime(2027, 2, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8274), new DateTime(2026, 6, 30, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8275), new DateTime(2026, 2, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8264) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8284), new DateTime(2027, 4, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8288), new DateTime(2026, 7, 22, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8289), new DateTime(2026, 4, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8287) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8291), new DateTime(2026, 11, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8294), new DateTime(2026, 6, 27, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8294), new DateTime(2025, 11, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(8293) });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 35, 55, 668, DateTimeKind.Utc).AddTicks(126));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 35, 55, 668, DateTimeKind.Utc).AddTicks(147));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 10, 35, 55, 668, DateTimeKind.Utc).AddTicks(152));

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(7700), new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(7694) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(7707), new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(7706) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(7710), new DateTime(2026, 6, 25, 10, 35, 55, 667, DateTimeKind.Utc).AddTicks(7709) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionChangeLogs");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8156));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8159));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8161));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(7985));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(7991));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8004));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8082), new DateTime(2026, 7, 5, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8084) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8087), new DateTime(2026, 6, 22, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8089), new DateTime(2026, 7, 9, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8091) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8028), new DateTime(2027, 2, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8040), new DateTime(2026, 6, 30, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8041), new DateTime(2026, 2, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8034) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8047), new DateTime(2027, 4, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8049), new DateTime(2026, 7, 22, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8050), new DateTime(2026, 4, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8048) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8051), new DateTime(2026, 11, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8053), new DateTime(2026, 6, 27, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8054), new DateTime(2025, 11, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8053) });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8121));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8127));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(8130));

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(7818), new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(7814) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(7827), new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(7826) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(7829), new DateTime(2026, 6, 25, 7, 32, 53, 11, DateTimeKind.Utc).AddTicks(7828) });
        }
    }
}

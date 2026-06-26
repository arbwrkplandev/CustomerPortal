using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WrkPlan.CustomerPortal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedSubscriptionPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "Id", "CreatedUtc", "Cycle", "Description", "FeaturesJson", "IsActive", "Name", "Price", "UpdatedUtc" },
                values: new object[,]
                {
                    { new Guid("31000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3540), 3, "Starter plan for lean teams", "[\"Portal dashboard\",\"Standard support\",\"Onboarding toolkit\"]", true, "Core", 999m, null },
                    { new Guid("31000000-0000-0000-0000-000000000002"), new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3545), 1, "Growth-focused plan with advanced analytics", "[\"Everything in Core\",\"Advanced analytics\",\"Priority support\",\"API access\"]", true, "Growth", 1499m, null },
                    { new Guid("31000000-0000-0000-0000-000000000003"), new DateTime(2026, 6, 25, 10, 36, 39, 607, DateTimeKind.Utc).AddTicks(3548), 2, "Enterprise-grade controls and support", "[\"Everything in Growth\",\"Dedicated success manager\",\"Custom SLAs\",\"White-glove onboarding\"]", true, "Enterprise", 4999m, null }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000003"));

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
    }
}

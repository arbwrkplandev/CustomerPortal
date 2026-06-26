using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WrkPlan.CustomerPortal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DecimalPrecisionFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1897));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1900));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1906));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1670));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1675));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1677));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1766), new DateTime(2026, 7, 5, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1774) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1776), new DateTime(2026, 6, 22, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1778) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1780), new DateTime(2026, 7, 9, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1781) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1703), new DateTime(2027, 2, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1714), new DateTime(2026, 6, 30, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1716), new DateTime(2026, 2, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1708) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1723), new DateTime(2027, 4, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1725), new DateTime(2026, 7, 22, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1726), new DateTime(2026, 4, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1725) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1728), new DateTime(2026, 11, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1730), new DateTime(2026, 6, 27, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1731), new DateTime(2025, 11, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1730) });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1862));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1866));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1868));

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1415), new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1412) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1432), new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1431) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1434), new DateTime(2026, 6, 25, 7, 32, 28, 389, DateTimeKind.Utc).AddTicks(1433) });
        }
    }
}

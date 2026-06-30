using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WrkPlan.CustomerPortal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContractESignValueBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ValueBytes",
                table: "ContractESignEntries",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7671));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7675));

            migrationBuilder.UpdateData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7677));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7384));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7389));

            migrationBuilder.UpdateData(
                table: "CustomerProfiles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7391));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7602), new DateTime(2026, 7, 10, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7605) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7608), new DateTime(2026, 6, 27, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7609) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "DueUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7611), new DateTime(2026, 7, 14, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7612) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7562));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7568));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7571));

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7424), new DateTime(2027, 2, 28, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7437), new DateTime(2026, 7, 5, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7438), new DateTime(2026, 2, 28, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7430) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7452), new DateTime(2027, 4, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7455), new DateTime(2026, 7, 27, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7456), new DateTime(2026, 4, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7454) });

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedUtc", "EndDateUtc", "RenewalDateUtc", "StartDateUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7529), new DateTime(2026, 11, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7532), new DateTime(2026, 7, 2, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7533), new DateTime(2025, 11, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7531) });

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7639));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7644));

            migrationBuilder.UpdateData(
                table: "SupportTickets",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "CreatedUtc",
                value: new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7646));

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7244), new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7241) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7250), new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7249) });

            migrationBuilder.UpdateData(
                table: "TenantRegistries",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                columns: new[] { "ActivatedUtc", "CreatedUtc" },
                values: new object[] { new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7252), new DateTime(2026, 6, 30, 7, 22, 22, 126, DateTimeKind.Utc).AddTicks(7252) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueBytes",
                table: "ContractESignEntries");

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
    }
}

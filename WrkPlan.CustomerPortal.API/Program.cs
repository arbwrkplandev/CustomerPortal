using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using WrkPlan.CustomerPortal.API.Middleware;
using WrkPlan.CustomerPortal.API.Services;
using WrkPlan.CustomerPortal.Application.Interfaces;
using WrkPlan.CustomerPortal.Application.Services;
using WrkPlan.CustomerPortal.Infrastructure.Data;
using WrkPlan.CustomerPortal.Infrastructure.MultiTenancy;
using WrkPlan.CustomerPortal.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console());

builder.Services.AddControllers();
builder.Services.AddDataProtection();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WrkPlan Customer Platform API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("AdminDb")
    ?? "Server=.;Database=nextgen_admin;Trusted_Connection=True;TrustServerCertificate=True";

builder.Services.AddDbContext<AdminDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<TenantDbContext>(opt => opt.UseSqlServer(connectionString));

builder.Services.AddSingleton<TenantContext>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
builder.Services.AddScoped<IThemePreferenceService, ThemePreferenceService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IPaymentService, StripePaymentService>();
builder.Services.AddScoped<IOnboardingEngineService, OnboardingEngineService>();
builder.Services.AddScoped<ICsvTransformService, CsvTransformService>();
builder.Services.AddScoped<IRenewalService, RenewalService>();
builder.Services.AddScoped<IQuoteTransitionService, QuoteTransitionService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "ChangeThisInProduction_AtLeast32Characters!"))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TenantScoped", policy => policy.RequireClaim("tenant_id"));
    options.AddPolicy("WrkPlanAdmin", policy => policy.RequireRole("WrkPlanAdmin"));
});

builder.Services.AddHostedService<ReminderBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
        db.Database.Migrate();
        logger.LogInformation("Admin database is ready.");
        await SeedDemoDataAsync(db);
        logger.LogInformation("Demo data verified.");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Admin database migration failed. API will continue running, but DB-backed endpoints may fail until SQL Server is reachable.");
    }
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<TenantResolverMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "WrkPlan.CustomerPortal.API" }));

app.Run();

static async Task SeedDemoDataAsync(AdminDbContext db)
{
    var acmeProfileId = Guid.Parse("20000000-0000-0000-0000-000000000001");

    // Extra invoices for Acme
    var extraInvoiceIds = new[]
    {
        Guid.Parse("40000000-0000-0000-0001-000000000001"),
        Guid.Parse("40000000-0000-0000-0001-000000000002"),
        Guid.Parse("40000000-0000-0000-0001-000000000003"),
        Guid.Parse("40000000-0000-0000-0001-000000000004"),
    };

    var invoiceSeeds = new[]
    {
        (extraInvoiceIds[0], "INV-ACME-1002", 2499m, DateTime.UtcNow.AddDays(15), WrkPlan.CustomerPortal.Domain.Enums.RecordStatus.Active),
        (extraInvoiceIds[1], "INV-ACME-1003", 749m,  DateTime.UtcNow.AddDays(-7), WrkPlan.CustomerPortal.Domain.Enums.RecordStatus.Overdue),
        (extraInvoiceIds[2], "INV-ACME-1004", 1499m, DateTime.UtcNow.AddDays(30), WrkPlan.CustomerPortal.Domain.Enums.RecordStatus.Active),
        (extraInvoiceIds[3], "INV-ACME-1005", 499m,  DateTime.UtcNow.AddDays(3),  WrkPlan.CustomerPortal.Domain.Enums.RecordStatus.Active),
    };

    foreach (var (id, num, total, due, status) in invoiceSeeds)
    {
        if (!await db.Invoices.AnyAsync(x => x.Id == id))
        {
            db.Invoices.Add(new WrkPlan.CustomerPortal.Domain.Entities.Invoice
            {
                Id = id, CustomerProfileId = acmeProfileId,
                InvoiceNumber = num, Total = total, DueUtc = due, Status = status,
                CreatedUtc = DateTime.UtcNow
            });
        }
    }

    // Extra support tickets for Acme
    var ticketSeeds = new[]
    {
        (Guid.Parse("50000000-0000-0000-0001-000000000001"), "Bulk data import failing on step 3", "High"),
        (Guid.Parse("50000000-0000-0000-0001-000000000002"), "SSO configuration walkthrough needed", "Normal"),
        (Guid.Parse("50000000-0000-0000-0001-000000000003"), "Custom report template request", "Low"),
        (Guid.Parse("50000000-0000-0000-0001-000000000004"), "Mobile app access for field team", "Normal"),
    };

    foreach (var (id, subject, priority) in ticketSeeds)
    {
        if (!await db.SupportTickets.AnyAsync(x => x.Id == id))
        {
            db.SupportTickets.Add(new WrkPlan.CustomerPortal.Domain.Entities.SupportTicket
            {
                Id = id, CustomerProfileId = acmeProfileId,
                Subject = subject, Priority = priority,
                Status = WrkPlan.CustomerPortal.Domain.Enums.RecordStatus.Active,
                CreatedUtc = DateTime.UtcNow
            });
        }
    }

    // Extra announcements for Acme
    var announcementSeeds = new[]
    {
        (Guid.Parse("60000000-0000-0000-0001-000000000001"), "Q3 Feature Roadmap", "Bulk scheduling, AI summarisation, and advanced reporting are shipping in Q3."),
        (Guid.Parse("60000000-0000-0000-0001-000000000002"), "Scheduled Maintenance", "Planned downtime on Sunday 2am–4am UTC. No action required."),
        (Guid.Parse("60000000-0000-0000-0001-000000000003"), "New Onboarding Templates", "Five new implementation playbook templates are now available in your hub."),
    };

    foreach (var (id, title, message) in announcementSeeds)
    {
        if (!await db.Announcements.AnyAsync(x => x.Id == id))
        {
            db.Announcements.Add(new WrkPlan.CustomerPortal.Domain.Entities.Announcement
            {
                Id = id, CustomerProfileId = acmeProfileId,
                Title = title, Message = message,
                CreatedUtc = DateTime.UtcNow
            });
        }
    }

    await db.SaveChangesAsync();
}


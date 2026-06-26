using Microsoft.EntityFrameworkCore;
using WrkPlan.CustomerPortal.Domain.Entities;

namespace WrkPlan.CustomerPortal.Infrastructure.Data;

public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<QuestionnaireResponse> QuestionnaireResponses => Set<QuestionnaireResponse>();
}

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Association> Associations => Set<Association>();
    public DbSet<Cagnotte> Cagnottes => Set<Cagnotte>();
    public DbSet<Participation> Participations => Set<Participation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Appliquer toutes les configurations
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
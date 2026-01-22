using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.Nom)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Prenom)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Role)
            .HasConversion<int>()
            .IsRequired();

        // Ignorer la propriété calculée
        builder.Ignore(u => u.NomComplet);

        // Index
        builder.HasIndex(u => u.Email);
        builder.HasIndex(u => u.AssociationId);
    }
}

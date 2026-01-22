using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Infrastructure.Data.Configurations;

public class CagnotteConfiguration : IEntityTypeConfiguration<Cagnotte>
{
    public void Configure(EntityTypeBuilder<Cagnotte> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nom)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.ObjectifFinancier)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);

        builder.Property(c => c.Statut)
            .HasConversion<int>()
            .IsRequired();

        // Index
        builder.HasIndex(c => c.GestionnaireId);
        builder.HasIndex(c => c.Statut);
        builder.HasIndex(c => c.DateCreation);

        // Relations
        builder.HasOne(c => c.Gestionnaire)
            .WithMany(u => u.CagnottesGerees)
            .HasForeignKey(c => c.GestionnaireId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Participations)
            .WithOne(p => p.Cagnotte)
            .HasForeignKey(p => p.CagnotteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignorer les propriétés calculées
        builder.Ignore(c => c.MontantCollecte);
        builder.Ignore(c => c.PourcentageAtteint);
        builder.Ignore(c => c.ObjectifAtteint);
        builder.Ignore(c => c.NombreParticipants);
    }
}

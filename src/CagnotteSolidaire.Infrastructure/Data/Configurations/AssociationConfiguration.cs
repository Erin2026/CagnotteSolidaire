using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Infrastructure.Data.Configurations;

public class AssociationConfiguration : IEntityTypeConfiguration<Association>
{
    public void Configure(EntityTypeBuilder<Association> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nom)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.SIREN)
            .IsRequired()
            .HasMaxLength(9);

        builder.Property(a => a.RNA)
            .HasMaxLength(10);

        builder.Property(a => a.Departement)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(a => a.Adresse)
            .HasMaxLength(500);

        builder.Property(a => a.CodePostal)
            .HasMaxLength(5);

        builder.Property(a => a.Ville)
            .HasMaxLength(200);

        // Index pour recherche rapide
        builder.HasIndex(a => a.SIREN).IsUnique();
        builder.HasIndex(a => a.RNA);

        // Relations
        builder.HasMany(a => a.Gestionnaires)
            .WithOne(u => u.Association)
            .HasForeignKey(u => u.AssociationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Pas de relation directe avec Cagnottes ici (passe par ApplicationUser)
    }
}


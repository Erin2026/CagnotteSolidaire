using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Infrastructure.Data.Configurations;

public class ParticipationConfiguration : IEntityTypeConfiguration<Participation>
{
    public void Configure(EntityTypeBuilder<Participation> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Montant)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Commentaire)
            .HasMaxLength(500);

        // Index
        builder.HasIndex(p => p.CagnotteId);
        builder.HasIndex(p => p.ParticipantId);
        builder.HasIndex(p => new { p.CagnotteId, p.ParticipantId });

        // Relations
        builder.HasOne(p => p.Cagnotte)
            .WithMany(c => c.Participations)
            .HasForeignKey(p => p.CagnotteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Participant)
            .WithMany(u => u.Participations)
            .HasForeignKey(p => p.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using System;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorSozluk.Infrastructure.Persistence.EntryConfiguration.Entry;

public class EntryFavoriteEntityConfiguration : BaseEntityConfiguration<EntryFavorite>
{
    public override void Configure(EntityTypeBuilder<EntryFavorite> builder)
    {
        base.Configure(builder);

        builder.ToTable("entryfavorite", BlazorSozlukContext.DEFAUlt_SCHEMA);

        builder.HasOne(i => i.Entry)
               .WithMany(i => i.EntryFavorites)
               .HasForeignKey(i => i.EntryId);

        builder.HasOne(i => i.CreatedUser)
               .WithMany(i => i.EntryFavorites)
               .HasForeignKey(i => i.CreatedById);
    }

}


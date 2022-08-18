using System;
using BlazorSozluk.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorSozluk.Infrastructure.Persistence.EntryConfiguration.Entry;

public class EntryEntityConfiguration : BaseEntityConfiguration<Api.Domain.Models.Entry>
{

    public override void Configure(EntityTypeBuilder<Api.Domain.Models.Entry> builder)
    {
        base.Configure(builder);

        builder.ToTable("entry", BlazorSozlukContext.DEFAUlt_SCHEMA);

        builder.HasOne(i => i.CreatedBy)
               .WithMany(i => i.Entries)
               .HasForeignKey(i => i.CreatedById);
    }

}


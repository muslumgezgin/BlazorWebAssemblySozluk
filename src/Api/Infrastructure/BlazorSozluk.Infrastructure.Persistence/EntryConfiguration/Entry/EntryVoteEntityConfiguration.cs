using System;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorSozluk.Infrastructure.Persistence.EntryConfiguration.Entry;

public class EntryVoteEntityConfiguration : BaseEntityConfiguration<EntryVote>
{
    public override void Configure(EntityTypeBuilder<EntryVote> builder)
    {
        base.Configure(builder);

        builder.ToTable("entryvote", BlazorSozlukContext.DEFAUlt_SCHEMA);

        builder.HasOne(i => i.Entry)
               .WithMany(i => i.EntryVotes)
               .HasForeignKey(i => i.EntryId);
    }

}


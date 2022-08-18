using System;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorSozluk.Infrastructure.Persistence.EntryConfiguration.EntryComment
{
    public class EntryCommentVoteEntityConfiguration : BaseEntityConfiguration<EntryCommentVote>
    {
        public override void Configure(EntityTypeBuilder<EntryCommentVote> builder)
        {
            base.Configure(builder);

            builder.ToTable("entrycommentvote", BlazorSozlukContext.DEFAUlt_SCHEMA);

            builder.HasOne(i => i.EntryComment)
                   .WithMany(i => i.EntryCommentVotes)
                   .HasForeignKey(i => i.EntryCommentId);

        }
    }
}


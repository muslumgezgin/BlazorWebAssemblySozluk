﻿using System;
using System.Reflection;
using BlazorSozluk.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorSozluk.Infrastructure.Persistence.Context
{
    public class BlazorSozlukContext : DbContext
    {
        public const string DEFAUlt_SCHEMA = "dbo";

        public BlazorSozlukContext( DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Entry> Entries { get; set; }

        public DbSet<EntryVote> EntryVotes { get; set; }

        public DbSet<EntryFavorite> EntryFavorites { get; set; }

        public DbSet<EntryComment> EntryComments { get; set; }

        public DbSet<EntryCommentVote> EntryCommentVotes { get; set; }

        public DbSet<EntryCommentFavorite> EntryCommentFavorites { get; set; }

        public DbSet<EmailConfirmation> EmailConfirmations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override int SaveChanges()
        {
            OnBeforeSave();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSave();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSave()
        {
            var addedEntites = ChangeTracker.Entries()
                                            .Where(i => i.State == EntityState.Added)
                                            .Select(i => (BaseEntity)i.Entity);

            PreparedAddedEntities(addedEntites);

        }

        private void PreparedAddedEntities(IEnumerable<BaseEntity> entities)
        {
            foreach (var entity in entities)
            {
                if(entity.CreateDate == DateTime.MinValue)
                    entity.CreateDate = DateTime.Now;
            }
        }

    }
}


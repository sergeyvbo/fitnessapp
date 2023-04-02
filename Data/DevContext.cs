using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Fitnessapp.Models.dev;

namespace Fitnessapp.Data
{
    public partial class devContext : DbContext
    {
        public devContext()
        {
        }

        public devContext(DbContextOptions<devContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Fitnessapp.Models.dev.Workout>()
              .HasOne(i => i.Exercise)
              .WithMany(i => i.Workouts)
              .HasForeignKey(i => i.exercise_id)
              .HasPrincipalKey(i => i.id);

            builder.Entity<Fitnessapp.Models.dev.Exercise>()
              .HasOne(i => i.Day)
              .WithMany(i => i.Exercises)
              .HasForeignKey(i => i.day_id)
              .HasPrincipalKey(i => i.id);

            builder.Entity<Fitnessapp.Models.dev.Workout>()
              .Property(p => p.date)
              .HasDefaultValueSql(@"CURRENT_DATE");
            this.OnModelBuilding(builder);
        }

        public DbSet<Fitnessapp.Models.dev.Workout> Workouts { get; set; }

        public DbSet<Fitnessapp.Models.dev.Exercise> Exercises { get; set; }

        public DbSet<Fitnessapp.Models.dev.Day> Days { get; set; }
    }
}
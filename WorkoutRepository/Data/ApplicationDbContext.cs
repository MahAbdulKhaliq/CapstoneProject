using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutRepository.Models;

namespace WorkoutRepository.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WorkoutRepository.Models.Exercise> Exercise { get; set; }
        public DbSet<WorkoutRepository.Models.PrimaryEquipment> PrimaryEquipment { get; set; }
        public DbSet<WorkoutRepository.Models.MuscleGroup> MuscleGroup { get; set; }

        public DbSet<WorkoutRepository.Models.FavouriteExercise> FavouriteExercise { get; set; }
    }
}

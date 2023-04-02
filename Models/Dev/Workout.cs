using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnessapp.Models.dev
{
    [Table("workouts", Schema = "public")]
    public partial class Workout
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int exercise_id { get; set; }

        public decimal? weight1 { get; set; }

        public int? reps1 { get; set; }

        public decimal? weight2 { get; set; }

        public int? reps2 { get; set; }

        public decimal? weight3 { get; set; }

        public int? reps3 { get; set; }

        public DateTime date { get; set; }

        public Exercise Exercise { get; set; }

    }
}
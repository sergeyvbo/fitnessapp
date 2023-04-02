using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnessapp.Models.dev
{
    [Table("exercises", Schema = "public")]
    public partial class Exercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }

        [Required]
        public int day_id { get; set; }

        public ICollection<Workout> Workouts { get; set; }

        public Day Day { get; set; }

    }
}
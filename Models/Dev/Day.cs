using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnessapp.Models.dev
{
    [Table("days", Schema = "public")]
    public partial class Day
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public string name { get; set; }

        public ICollection<Exercise> Exercises { get; set; }

    }
}
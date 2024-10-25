using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TennisCoach.Models
{
    public class CreateSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Foreign key for Coach
        [Required]
        public int CoachId { get; set; }

        [ForeignKey("CoachId")]
        public virtual Coaches Coach { get; set; }

        public string Description { get; set; }

        [ForeignKey("Member")]
        public int? MemberId { get; set; } 

        public virtual Members Member { get; set; }

    }

}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TennisCoach.Models
{
    public class Schedules
    { 
        [Key]
    
        public int ScheduleId { get; set; }
        public string Location { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int CoachId { get; set; }
        public Coaches Coach { get; set; }
        public int? MemberId { get; set; }
        public virtual ICollection<Enrollments> Enrollments { get; set; }
        public Members Member { get; internal set; }
    }

}

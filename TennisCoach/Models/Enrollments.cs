using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TennisCoach.Models
{
    public class Enrollments
    {
        [Key]

        public int EnrollmentId { get; set; }
        public int ScheduleId { get; set; }
        public int MemberId { get; set; }

        public virtual Schedules Schedule { get; set; }
        public virtual Members Member { get; set; }

    }

}






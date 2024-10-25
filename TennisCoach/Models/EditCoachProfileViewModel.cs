namespace TennisCoach.Models
{
    public class EditCoachProfileViewModel
    {
        
            public int CoachId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Biography { get; set; }
            public IFormFile PhotoFile { get; set; }
        

    }

}

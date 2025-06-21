
namespace api.Dtos.Preference
{
    public class UserPreferencesDto
    {
        public int user_id { get; set; }
        public int? user_preference_id { get; set; }
        public int? restriction_id { get; set; }
        public string restriction { get; set; }
        public int? goal_id { get; set; }
        public string goal { get; set; }   
    }
}
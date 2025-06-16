
namespace api.Models
{
    public class UserPreference
    {
        public int user_id { get; set; }
        public int? user_preference_id { get; set; }
        public int? restriction_id { get; set; }
        public string restriction { get; set; }
        public int? goal_id { get; set; }
        public string goal { get; set; }   
    }
}

namespace api.Dtos.Preference
{
    public class UserDietaryRestrictionDto
    {
        public int id { get; set; }
        public int user_preference_id { get; set; }
        public PreferenceDto preference { get; set; }
        public int restriction_id { get; set; } 
        public DietaryRestrictionDto dietary_Restriction { get; set; }  
    }
}
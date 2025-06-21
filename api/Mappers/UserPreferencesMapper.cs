using api.Dtos.Preference;
using api.Models;

namespace api.Mappers
{
    public static class UserPreferencesMapper
    {
        public static UserPreferencesDto ToDto(int userId, UserPreference userPreferences)
        {
             return new UserPreferencesDto
             {
                 user_preference_id = userPreferences.user_preference_id,
                 user_id = userId,
            };
        }
    }
}

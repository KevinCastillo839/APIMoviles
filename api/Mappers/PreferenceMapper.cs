using api.Dtos.Preference;
using api.Models;

namespace api.Mappers
{
    public static class PreferenceMapper
    {
        //Map from Preference to PreferenceDto
        public static PreferenceDto ToDto(this Preference preference)
        {
            return new PreferenceDto
            {
                id = preference.id,
                user_id = preference.user_id,
                created_at = preference.created_at,
                updated_at = preference.updated_at
            };
        }

        //Map from CreatePreferenceRequestDto to Preference
        public static Preference ToPreferenceFromCreateDto(this CreatePreferenceRequestDto createPreferenceRequest)
        {
            return new Preference
            {
                user_id = createPreferenceRequest.user_id,
                created_at = createPreferenceRequest.created_at,
                updated_at = createPreferenceRequest.updated_at
            };
        }
  
        // Map from UpdatePreferenceRequestDto to Preference
        public static Preference ToPreferenceFromUpdateDto(this UpdatePreferenceRequestDto updatePreferenceRequest, Preference existingPreference)
        {
            existingPreference.user_id = updatePreferenceRequest.user_id;
            existingPreference.updated_at = updatePreferenceRequest.updated_at ?? DateTime.UtcNow;

            return existingPreference;
        }
    }
}

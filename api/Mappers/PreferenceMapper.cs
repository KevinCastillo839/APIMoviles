using api.Dtos.Preference;
using api.Models;

namespace api.Mappers
{
    public static class PreferenceMapper
    {
        // Mapear de Preference a PreferenceDto
        public static PreferenceDto ToDto(this Preference preference)
        {
            return new PreferenceDto
            {
                id = preference.id,
                user_id = preference.user_id,
                is_vegetarian = preference.is_vegetarian,
                is_gluten_free = preference.is_gluten_free,
                is_vegan = preference.is_vegan,
                dietary_goals = preference.dietary_goals,
                created_at = preference.created_at,
                updated_at = preference.updated_at
            };
        }

        // Mapear de CreatePreferenceRequestDto a Preference
        public static Preference ToPreferenceFromCreateDto(this CreatePreferenceRequestDto createPreferenceRequest)
        {
            return new Preference
            {
                user_id = createPreferenceRequest.user_id,
                is_vegetarian = createPreferenceRequest.is_vegetarian,
                is_gluten_free = createPreferenceRequest.is_gluten_free,
                is_vegan = createPreferenceRequest.is_vegan,
                dietary_goals = createPreferenceRequest.dietary_goals,
                created_at = createPreferenceRequest.created_at,
                updated_at = createPreferenceRequest.updated_at
            };
        }

        // Mapear de UpdatePreferenceRequestDto a Preference
        public static Preference ToPreferenceFromUpdateDto(this UpdatePreferenceRequestDto updatePreferenceRequest, Preference existingPreference)
        {
            existingPreference.user_id = updatePreferenceRequest.user_id;
            existingPreference.is_vegetarian = updatePreferenceRequest.is_vegetarian;
            existingPreference.is_gluten_free = updatePreferenceRequest.is_gluten_free;
            existingPreference.is_vegan = updatePreferenceRequest.is_vegan;
            existingPreference.dietary_goals = updatePreferenceRequest.dietary_goals;
            existingPreference.updated_at = updatePreferenceRequest.updated_at ?? DateTime.UtcNow;

            return existingPreference;
        }
    }
}

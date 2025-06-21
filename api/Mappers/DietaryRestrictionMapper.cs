using api.Dtos.Preference;
using api.Models;

namespace api.Mappers
{
    public static class DietaryRestrictionMapper
    {
        public static DietaryRestrictionDto ToDto(this Dietary_Restriction dietary_Restriction)
        {
            return new DietaryRestrictionDto
            {
                id = dietary_Restriction.id,
                name = dietary_Restriction.name,
                created_at = dietary_Restriction.created_at
            };
        }

    }
}

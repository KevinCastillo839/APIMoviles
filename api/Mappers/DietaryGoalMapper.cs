using api.Dtos.Preference;
using api.Models;

namespace api.Mappers
{
    public static class DietaryGoalMapper
    {
        public static DietaryGoalDto ToDto(this Dietary_Goal dietary_Goal)
        {
            return new DietaryGoalDto
            {
                id = dietary_Goal.id,
                goal = dietary_Goal.goal,
                created_at = dietary_Goal.created_at
            };
        }

    }
}

using api.Dtos.Preference;
using api.Models;

namespace api.Mappers
{
  public static class UserDietaryRestrictionMapper
  {
    public static UserDietaryRestrictionDto ToDto(this User_Dietary_Restriction restrictionItem)
    {
      return new UserDietaryRestrictionDto
      {
        id = restrictionItem.id,
        user_preference_id=restrictionItem.user_preference_id,
        restriction_id=restrictionItem.restriction_id

      };
    }
    public static List<User_Dietary_Restriction> ToAllergyFromCreateDto(this CreateUserDietaryRestrictionRequestDto createUserRequest)
    {
        return createUserRequest.restriction_ids.Select(restrictionId => new User_Dietary_Restriction
        {
            user_preference_id = createUserRequest.user_preference_id,
            restriction_id = restrictionId
        }).ToList();
    }
  }
}
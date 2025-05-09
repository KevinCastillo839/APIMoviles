using api.Dtos.Allergy;
using api.Models;

namespace api.Mappers
{
  public static class UserAllergyMapper
  {
    public static UserAllergyDto ToDto(this User_Allergy allergyItem)
    {
      return new UserAllergyDto
      {
        id = allergyItem.id,
        user_id=allergyItem.user_id,
        allergie_id=allergyItem.allergy_id

      };
    }
    public static List<User_Allergy> ToAllergyFromCreateDto(this CreateUserAllergyRequestDto createUserRequest)
    {
        return createUserRequest.allergy_ids.Select(allergyId => new User_Allergy
        {
            user_id = createUserRequest.user_id,
            allergy_id = allergyId,
            created_at = createUserRequest.created_at,
            updated_at = createUserRequest.updated_at
        }).ToList();
    }
  }
}
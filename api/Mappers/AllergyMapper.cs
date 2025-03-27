using api.Dtos.Allergy;
using api.Models;

namespace api.Mappers
{
  public static class AllergyMapper
  {
    public static AllergyDto ToDto(this Allergy allergyItem)
    {
      return new AllergyDto
      {
        id = allergyItem.id,
        name = allergyItem.name,
        description = allergyItem.description,
        created_at = allergyItem.created_at,
        updated_at = allergyItem.updated_at

      };
    }
     public static Allergy ToAllergyFromCreateDto(this CreateAllergyRequestDto createUserRequest)
    {
      return new Allergy
      {
        name = createUserRequest.name,
        description = createUserRequest.description,
        created_at = createUserRequest.created_at,
        updated_at = createUserRequest.updated_at

      };
    }
  }
}
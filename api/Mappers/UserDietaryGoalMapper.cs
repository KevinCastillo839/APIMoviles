using api.Dtos.Preference;
using api.Models;

namespace api.Mappers
{
  public static class UserDietaryGoalMapper
  {
    public static UserDietaryGoalDto ToDto(this User_Dietary_Goal dietaryGoalItem)
    {
      return new UserDietaryGoalDto
      {
        id = dietaryGoalItem.id,
        user_preference_id=dietaryGoalItem.user_preference_id,
        goal_id=dietaryGoalItem.goal_id

      };
    }
    public static User_Dietary_Goal ToAllergyFromCreateDto(this CreateUserDietaryGoalRequestDto createUserRequest)
    {
        return new User_Dietary_Goal
        {
            user_preference_id = createUserRequest.user_preference_id,
            goal_id = createUserRequest.goal_id
        };
    }
  }
}
namespace api.Dtos.Recipe
{
  public class RecipeDto
  {
        public int id { get; set; }
        public string name { get; set; }
        public string instructions { get; set; }
        public string category { get; set; }
        public int preparation_time { get; set; }
        public string image_url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
  }
}
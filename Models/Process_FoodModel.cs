namespace FoodPenguinAPI.Models
{
    public class Process_FoodModel
    {
        public int food_id { get; set; }
        public int? order_id { get; set; }
        public int? price { get; set; }
        public string? remark { get; set; }
        public int? finish { get; set; }
    }
}

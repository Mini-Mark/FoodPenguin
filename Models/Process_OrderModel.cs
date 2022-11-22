namespace FoodPenguinAPI.Models
{
    public class Process_OrderModel
    {
        public int? recvp_id { get; set; }
        public int? order_id { get; set; }
        public int? status { get; set; }
        public string? pay_img { get; set; }
        public Boolean? finish { get; set; }
    }
}

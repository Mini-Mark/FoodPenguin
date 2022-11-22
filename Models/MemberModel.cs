namespace FoodPenguinAPI.Models
{
    public class MemberModel
    {
        public int order_id { get; set; }
        public int user_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string nickname { get; set; }
        public string remark { get; set; }
        public int status { get; set; }
    }
}

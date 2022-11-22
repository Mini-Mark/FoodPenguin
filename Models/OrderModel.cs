namespace FoodPenguinAPI.Models
{
    public class OrderModel
    {
        public int order_id { get; set; }
        public int user_id { get; set; }
        public int loc_id { get; set; }
        public int market_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string nickname { get; set; }
        public string location_name { get; set; }
        public string market_name { get; set; }
        public string remark { get; set; }
        public string time { get; set; }
        public int status { get; set; }
        public int? owner_recvp_id { get; set; }
        public int? owner_recvp_user_id { get; set; }
        public string? owner_recvp_user_name { get; set; }
        public List<FoodModel> food { get; set; }

        public class OrderInsert
        {
            public int user_id { get; set; }
            public int loc_id { get; set; }
            public int market_id { get; set; }
            public List<string> food { get; set; }
            public string remark { get; set; }
        }
    }
}

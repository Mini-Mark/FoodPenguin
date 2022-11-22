namespace FoodPenguinAPI.Models
{
    public class ListModel
    {

        public class All
        {
            public int id { get; set; }
            public int user_id { get; set; }
            public int loc_id { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string nickname { get; set; }
            public string location_name { get; set; }
            public string remark { get; set; }
            public string special { get; set; }
            public string time { get; set; }
            public string type { get; set; }
        }

        public class Order
        {
            public int order_id { get; set; }
            public int user_id { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string nickname { get; set; }
            public int loc_id { get; set; }
            public string location_name { get; set; }
            public int market_id { get; set; }
            public string market_name { get; set; }
            public string remark { get; set; }
            public string time { get; set; }
            public string type { get; set; }
        }

        public class Receive
        {
            public int recvp_id { get; set; }
            public int user_id { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string nickname { get; set; }
            public int loc_id { get; set; }
            public string location_name { get; set; }
            public string desc { get; set; }
            public int max { get; set; }
            public string time { get; set; }
            public string type { get; set; }
        }
    }
}

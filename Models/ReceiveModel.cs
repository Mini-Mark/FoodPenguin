namespace FoodPenguinAPI.Models
{
    public class ReceiveModel
    {
        public int recvp_id { get; set; }
        public int loc_id { get; set; }
        public int user_id { get; set; }
        public string location_name { get; set; }
        public string desc { get; set; }
        public int max { get; set; }
        public string time { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string nickname { get; set; }
        public Boolean allowJoin { get; set; }
        public List<MarketModel> market { get; set; }
        public List<MemberModel> member { get; set; }

        public class ReceiveInsert
        {
            public int user_id { get; set; }
            public int loc_id { get; set; }
            public List<int> market { get; set; }
            public string desc { get; set; }
            public int max { get; set; }
        }
    }
}

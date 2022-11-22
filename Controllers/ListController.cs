using FoodPenguinAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.X509;
using System.Collections.Generic;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace FoodPenguinAPI.Controllers
{
    public class ListController : Controller
    {
        Database db = new Database();

        [HttpGet]
        public JsonResult getList(string? keyword, string sort)
        {
            string sql = "";
            if (sort == "all")
            {


                sql = "SELECT order_post.order_id as 'id',order_post.user_id,user.firstname,user.lastname,user.nickname,order_post.loc_id," +
                    "location.name as 'location_name',remark, market.name as 'special',time,'order' as 'type' FROM `order_post` " +
                    "INNER JOIN `user` ON `user`.`user_id` = order_post.user_id " +
                    "INNER JOIN location ON location.loc_id = order_post.loc_id " +
                    "INNER JOIN market ON market.market_id = order_post.market_id " +
                    "INNER JOIN process_order ON process_order.order_id = order_post.order_id ";

                sql += $"WHERE process_order.finish != 1 AND process_order.status = 1 AND process_order.recvp_id IS NULL ";

                Console.WriteLine(sql);

                if (keyword != null)
                {
                    sql += $"AND (location.name LIKE '%{keyword}%' OR market.name LIKE '%{keyword}%') ";
                }

                sql += "UNION SELECT receive_post.recvp_id, receive_post.user_id,user.firstname,user.lastname,user.nickname,receive_post.loc_id,location.name,`desc`,`max`,time,'receive' " +
                    "FROM `receive_post` INNER JOIN `user` ON `user`.`user_id` = receive_post.user_id " +
                    "INNER JOIN location ON location.loc_id = receive_post.loc_id";

                sql += $" WHERE receive_post.allowJoin = 1";

                if (keyword != null)
                {
                    sql += $" AND location.name LIKE '%{keyword}%'";
                }

            }
            else if (sort == "receive")
            {


                sql = "SELECT receive_post.recvp_id, receive_post.user_id,user.firstname,user.lastname,user.nickname,receive_post.loc_id," +
                    "location.name as 'location_name',`desc`,`max`,time,'receive' as 'type' FROM `receive_post` " +
                    "INNER JOIN `user` ON `user`.`user_id` = receive_post.user_id " +
                    "INNER JOIN location ON location.loc_id = receive_post.loc_id";

                sql += $" WHERE receive_post.allowJoin = 1";

                if (keyword != null)
                {
                    sql += $" AND location.name LIKE '%{keyword}%'";
                }

            }
            else if (sort == "order")
            {


                sql = "SELECT order_post.order_id as 'order_id',order_post.user_id,user.firstname,user.lastname,user.nickname,order_post.loc_id," +
                    "location.name as 'location_name',order_post.market_id,market.name as 'market_name',remark,time,'order' as 'type' FROM `order_post` " +
                    "INNER JOIN `user` ON order_post.user_id = `user`.`user_id` " +
                    "INNER JOIN location ON location.loc_id = order_post.loc_id " +
                    "INNER JOIN market ON market.market_id = order_post.market_id " +
                    "INNER JOIN process_order ON process_order.order_id = order_post.order_id";

                sql += $" WHERE process_order.finish != 1 AND process_order.status = 1 AND process_order.recvp_id IS NULL";

                if (keyword != null)
                {
                    sql += $" AND (market.name LIKE '%{keyword}%' OR location.name LIKE '%{keyword}%')";
                }

            }
            else
            {
                return new JsonResult(NotFound());
            }

            sql += " ORDER BY time DESC";

            var dataset = db.query_select(sql);
            //JObject manageJson = new JObject();
            //manageJson.Add("data", JArray.Parse(dataset.data));

            if (sort == "all")
            {
                List<ListModel.All> model = new List<ListModel.All>();
                model = JArray.Parse(dataset.data).ToObject<List<ListModel.All>>();
                return new JsonResult(Ok(model));
            }
            else if (sort == "receive")
            {
                List<ListModel.Receive> model = new List<ListModel.Receive>();
                model = JArray.Parse(dataset.data).ToObject<List<ListModel.Receive>>();
                return new JsonResult(Ok(model));
            }
            else if (sort == "order")
            {
                List<ListModel.Order> model = new List<ListModel.Order>();
                model = JArray.Parse(dataset.data).ToObject<List<ListModel.Order>>();
                return new JsonResult(Ok(model));
            }
            return new JsonResult(NotFound());

        }

        [HttpGet]
        public JsonResult getListProgressFromUser(int user_id)
        {

            JObject manageJson = new JObject();

            Boolean isReceive = true;

            string sql = $"SELECT receive_post.recvp_id, receive_post.user_id,user.firstname," +
                $"user.lastname,user.nickname,receive_post.loc_id,location.name as 'location_name'," +
                $"`desc`,`max`,time,'receive' as 'type' FROM `receive_post` " +
                $"INNER JOIN process_order ON receive_post.recvp_id = process_order.recvp_id " +
                $"INNER JOIN `user` ON `user`.`user_id` = receive_post.user_id " +
                $"INNER JOIN location ON location.loc_id = receive_post.loc_id " +
                $"WHERE process_order.finish = 0 AND receive_post.user_id = {user_id} " +
                $"LIMIT 1";

            var dataset = db.query_select(sql, singleValue: true);

            if (dataset.row == 0)
            {
                sql = $"SELECT receive_post.recvp_id, receive_post.user_id,user.firstname," +
                $"user.lastname,user.nickname,receive_post.loc_id,location.name as 'location_name'," +
                $"`desc`,`max`,time,'receive' as 'type' FROM `receive_post` " +
                $"INNER JOIN `user` ON `user`.`user_id` = receive_post.user_id " +
                $"INNER JOIN location ON location.loc_id = receive_post.loc_id " +
                $"WHERE receive_post.user_id = {user_id} AND receive_post.allowJoin = 1 LIMIT 1";

                dataset = db.query_select(sql, singleValue: true);
            }

            if (dataset.row == 0)
            {
                isReceive = false;
                sql = $"SELECT order_post.order_id as 'order_id',order_post.user_id,user.firstname,user.lastname,user.nickname," +
                    $"order_post.loc_id,location.name as 'location_name',order_post.market_id,market.name as 'market_name',remark,time,'order' as 'type' " +
                    $"FROM order_post INNER JOIN process_order " +
                    $"ON order_post.order_id = process_order.order_id " +
                    "INNER JOIN `user` ON order_post.user_id = `user`.`user_id` " +
                    "INNER JOIN location ON location.loc_id = order_post.loc_id " +
                    "INNER JOIN market ON market.market_id = order_post.market_id " +
                    $"WHERE process_order.finish = 0 AND order_post.user_id = {user_id}";

                dataset = db.query_select(sql);

                if (dataset.row == 0)
                {
                    return new JsonResult(NotFound());
                }
            }

            if (isReceive)
            {
                ListModel.Receive model = new ListModel.Receive();
                model = JObject.Parse(dataset.data).ToObject<ListModel.Receive>();
                return new JsonResult(Ok(model));
            }
            else
            {

                List<ListModel.Order> model = new List<ListModel.Order>();
                model = JArray.Parse(dataset.data).ToObject<List<ListModel.Order>>();
                return new JsonResult(Ok(model));
            }

        }
    }
}

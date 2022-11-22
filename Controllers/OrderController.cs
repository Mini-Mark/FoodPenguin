using FoodPenguinAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System.Data;

namespace FoodPenguinAPI.Controllers
{
    public class OrderController : Controller
    {
        Database db = new Database();

        [HttpGet]
        public JsonResult getOrderDetailByID(int order_id)
        {

            string sql_check_data = $"SELECT * FROM `process_order` WHERE order_id = {order_id} AND recvp_id IS NOT NULL";
            var dataset_check_data = db.query_select(sql_check_data);

            string sql = "";


            if (dataset_check_data.row == 0)
            {
                sql = $"SELECT order_post.order_id,order_post.user_id,order_post.loc_id,order_post.market_id,user.firstname," +
                                $"user.lastname,user.nickname,location.name as location_name,market.name as market_name,order_post.remark," +
                                $"order_post.time,process_order.status FROM `order_post` " +
                                $"INNER JOIN user ON user.user_id = order_post.user_id " +
                                $"INNER JOIN process_order ON process_order.order_id = order_post.order_id " +
                                $"INNER JOIN market ON market.market_id = order_post.market_id " +
                                $"INNER JOIN location ON order_post.loc_id = location.loc_id WHERE order_post.order_id = {order_id}";
            }
            else if (dataset_check_data.row == 1)
            {
                sql = $"SELECT order_post.order_id, order_post.user_id, order_post.loc_id, order_post.market_id, UserA.firstname, " +
                $"UserA.lastname, UserA.nickname, location.name AS location_name, market.name AS market_name, " +
                $"order_post.remark, order_post.time,process_order.status, process_order.recvp_id AS 'owner_recvp_id', UserB.user_id AS 'owner_recvp_user_id', " +
                $"CONCAT( UserB.firstname, ' ', UserB.lastname, ' (', UserB.nickname, ')' ) AS 'owner_recvp_user_name' FROM `order_post` " +
                $"INNER JOIN `user` AS UserA ON UserA.user_id = order_post.user_id " +
                $"INNER JOIN market ON market.market_id = order_post.market_id " +
                $"INNER JOIN location ON order_post.loc_id = location.loc_id " +
                $"INNER JOIN process_order ON process_order.order_id = order_post.order_id " +
                $"INNER JOIN receive_post ON process_order.recvp_id = receive_post.recvp_id " +
                $"INNER JOIN `user` AS UserB ON UserB.user_id = receive_post.user_id " +
                $"WHERE order_post.order_id = {order_id}";
            }
            else
            {
                return new JsonResult(NotFound());
            }

            var dataset = db.query_select(sql, singleValue: true);

            if (dataset.row != 0)
            {

                string sql_market = $"SELECT food_id,name,amount FROM `food` WHERE order_id = {order_id};";
                var dataset_market = db.query_select(sql_market);

                JObject manageJson = JObject.Parse(dataset.data);
                manageJson.Add("food", JArray.Parse(dataset_market.data));

                //To Model
                OrderModel model = manageJson.ToObject<OrderModel>();

                return new JsonResult(Ok(model));
            }
            else
            {
                return new JsonResult(NotFound());
            }
        }

        [HttpPost]
        public JsonResult insertOrder([FromBody] OrderModel.OrderInsert model)
        {
            string sql = $"INSERT INTO `order_post` (`user_id`, `remark`, `market_id`, `loc_id`) VALUES ('{model.user_id}', '{model.remark}', '{model.market_id}', '{model.loc_id}');";
            var dataset = db.query_insert(sql);

            if (dataset.status)
            {
                string sql_set = "INSERT INTO `food` (`name`, `amount`, `order_id`) VALUES";

                foreach (var item in model.food)
                {
                    string[] food_detail = item.Split(",");

                    string food_name = food_detail[0];
                    string food_amount = food_detail[1];

                    sql_set += $"('{food_name}', '{food_amount}', '{dataset.data}'),";

                }
                sql_set = sql_set.Remove(sql_set.Length - 1);
                sql_set += ";";


                var dataset_market = db.query_insert(sql_set);



                //Insert Process Order
                string sql_insert_process_order = $"INSERT INTO `process_order` (`recvp_id`, `order_id`, `status`, `pay_img`, `finish`) VALUES (NULL, '{dataset.data}', '1', NULL, '0');";
                var dataset_insert_process_order = db.query_insert(sql_insert_process_order);

                if (dataset_insert_process_order.status)
                {
                    string sql_find_food = $"SELECT * FROM `food` WHERE order_id = {dataset.data}";
                    var dataset_find_food = db.query_select(sql_find_food);

                    string sql_insert_process_food = " INSERT INTO `process_food` (`food_id`, `order_id`) VALUES";
                    foreach (JObject item in JArray.Parse(dataset_find_food.data))
                    {
                        string food_id = item.GetValue("food_id").ToString();
                        sql_insert_process_food += $"('{food_id}', '{dataset.data}'),";
                    }
                    sql_insert_process_food = sql_insert_process_food.Remove(sql_insert_process_food.Length - 1);
                    sql_insert_process_food += ";";

                    var dataset_insert_process_food = db.query_insert(sql_insert_process_food);
                    if (dataset_insert_process_food.status)
                    {

                        string sql_find_allowJoin = $"SELECT * FROM `receive_post` INNER JOIN receive_loc " +
                            $"ON receive_loc.recvp_id = receive_post.recvp_id " +
                            $"WHERE `receive_post`.allowJoin = 1 AND `receive_post`.loc_id = {model.loc_id}" +
                            $" AND receive_loc.market_id = {model.market_id} LIMIT 1";

                        var dataset_find_allowJoin = db.query_select(sql_find_allowJoin, singleValue: true);
                        Console.WriteLine(sql_find_allowJoin);
                        Console.WriteLine(dataset_find_allowJoin.data);

                        if (dataset_find_allowJoin.row == 1)
                        {
                            JObject recvp_joined = JObject.Parse(dataset_find_allowJoin.data);
                            int recvp_max = recvp_joined.GetValue("max").ToObject<int>();
                            int recvp_id = recvp_joined.GetValue("recvp_id").ToObject<int>();


                            string sql_update_process = $"UPDATE `process_order` SET `recvp_id`= {recvp_id} WHERE order_id = {dataset.data}";
                            var dataset_update_process = db.query_update(sql_update_process);
                            if (dataset_update_process.status == false)
                            {
                                return new JsonResult(NotFound());
                            }
                            else
                            {
                                string sql_count_min = $"SELECT * FROM `process_order` WHERE recvp_id = {recvp_id}";
                                var dataset_count_min = db.query_select(sql_count_min);

                                if (dataset_count_min.row == recvp_max)
                                {
                                    string sql_update_allow_join = $"UPDATE `receive_post` SET `allowJoin` = '0' WHERE `receive_post`.`recvp_id` = {recvp_id};";
                                    var dataset_update_allow_join = db.query_update(sql_update_allow_join);
                                    if (dataset_update_allow_join.status == false)
                                    {
                                        return new JsonResult(NotFound());
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        return new JsonResult(NotFound());
                    }
                }
                else
                {
                    return new JsonResult(NotFound());
                }

                if (dataset_market.status)
                {
                    return new JsonResult(Ok(dataset.data));
                }
                else
                {
                    return new JsonResult(NotFound());
                }
            }
            else
            {
                return new JsonResult(NotFound());
            }
        }

        [HttpDelete]
        public JsonResult cancelOrder(int order_id)
        {
            string sql_check_recvp_joined = $"SELECT `recvp_id` FROM `process_order` WHERE `order_id` = {order_id};";
            var dataset_check_recvp_joined = db.query_select(sql_check_recvp_joined,singleValue: true);

            string check_recvp = JObject.Parse(dataset_check_recvp_joined.data).GetValue("recvp_id").ToString();

            if (check_recvp != "")
            {
                string sql_update_recvp = $"UPDATE `receive_post` SET `allowJoin` = '1' WHERE `receive_post`.`recvp_id` = {check_recvp};";
                var dataset_update_recvp = db.query_update(sql_update_recvp);
            }

            string sql = $"UPDATE `process_order` SET `status` = '0', `finish` = '1',`recvp_id` = NULL WHERE `process_order`.`order_id` = {order_id};";
            var dataset_process_order = db.query_update(sql);

            sql = $"UPDATE `process_food` SET `finish` = '1' WHERE `process_food`.`order_id` = {order_id};";
            var dataset_process_food = db.query_update(sql);

            if (dataset_process_order.status && dataset_process_food.status)
            {
                return new JsonResult(Ok());
                //return Content(Helper.sendSuccess(dataset_process_order.data), "application/json");
            }
            else
            {
                return new JsonResult(NotFound());
                //return Content(Helper.sendError(dataset_process_order.data), "application/json");
            }
        }
    }
}

using FoodPenguinAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities.Collections;
using System.Collections.Generic;
using System.Data;

namespace FoodPenguinAPI.Controllers
{
    public class ReceiveController : Controller
    {

        Database db = new Database();

        [HttpGet]
        public JsonResult getReceiveDetailByID(int recvp_id)
        {
            string sql = $"SELECT `recvp_id`,`receive_post`.`loc_id`,`location`.`name` as location_name,`desc`,`max`,`time`,`receive_post`.`user_id`,`user`.`firstname`,`user`.`lastname`,`user`.`nickname`,`receive_post`.`allowJoin`" +
                $" FROM `receive_post` INNER JOIN `user` ON receive_post.user_id = `user`.`user_id` " +
                $"INNER JOIN `location` ON `receive_post`.`loc_id` = `location`.`loc_id` WHERE `recvp_id` = {recvp_id};";

            var dataset = db.query_select(sql, singleValue: true);

            if (dataset.row != 0)
            {

                string sql_market = $"SELECT `receive_loc`.`market_id`,`market`.name FROM `receive_loc` " +
                    $"INNER JOIN `market` ON market.market_id = receive_loc.market_id WHERE recvp_id = {recvp_id};";
                var dataset_market = db.query_select(sql_market);

                JObject manageJson = JObject.Parse(dataset.data);
                manageJson.Add("market", JArray.Parse(dataset_market.data));


                string sql_member = $"SELECT process_order.order_id,order_post.user_id,firstname,lastname,nickname,remark,`status` " +
                    $"FROM `process_order` INNER JOIN order_post ON process_order.order_id = order_post.order_id " +
                    $"INNER JOIN `user` ON `user`.`user_id` = order_post.user_id " +
                    $"WHERE process_order.recvp_id = {recvp_id}";

                var dataset_member = db.query_select(sql_member);

                manageJson.Add("member", JArray.Parse(dataset_member.data));

                //To Model
                ReceiveModel model = manageJson.ToObject<ReceiveModel>();

                return new JsonResult(Ok(model));
            }
            else
            {
                return new JsonResult(NotFound());
            }
        }

        [HttpPost]
        public JsonResult insertReceive([FromBody] ReceiveModel.ReceiveInsert model)
        {
            string sql = $"INSERT INTO `receive_post` (`loc_id`, `user_id`, `desc`, `max`) VALUES('{model.loc_id}', '{model.user_id}', '{model.desc}', '{model.max}')";
            var dataset = db.query_insert(sql);

            if (dataset.status)
            {
                string sql_set = "INSERT INTO `receive_loc` (`market_id`, `recvp_id`) VALUES";
                foreach (var item in model.market)
                {
                    sql_set += $"('{item}', '{dataset.data}'),";
                }
                sql_set = sql_set.Remove(sql_set.Length - 1);
                sql_set += ";";


                var dataset_market = db.query_insert(sql_set);
                if (dataset_market.status)
                {

                    string sql_process_order_select = $"SELECT * FROM `process_order` INNER JOIN order_post " +
                        $"ON order_post.order_id = process_order.order_id " +
                        $"WHERE `status` = 1 AND process_order.recvp_id IS NULL " +
                        $"AND order_post.loc_id = {model.loc_id} AND (";

                    foreach (var item in model.market)
                    {
                        sql_process_order_select += $"order_post.market_id = {item} OR ";
                    }
                    sql_process_order_select = sql_process_order_select.Remove(sql_process_order_select.Length - 3);
                    sql_process_order_select += $") LIMIT {model.max};";

                    var dataset_prcs_select = db.query_select(sql_process_order_select);

                    Console.WriteLine(sql_process_order_select);
                    Console.WriteLine(dataset_prcs_select.data);
                    Console.WriteLine(dataset_prcs_select.row);

                    if (dataset_prcs_select.row == model.max)
                    {
                        string sql_receive_update = $"UPDATE `receive_post` SET `allowJoin` = '0' WHERE `receive_post`.`recvp_id` = {dataset.data};";
                        var dataset_receive_update = db.query_update(sql_receive_update);
                        if (dataset_receive_update.status == false)
                        {
                            return new JsonResult(NotFound());
                        }
                    }

                    if (dataset_prcs_select.row > 0)
                    {

                        foreach (JObject item in JArray.Parse(dataset_prcs_select.data))
                        {
                            string sql_process_order_update = $"UPDATE `process_order` SET `recvp_id`= {dataset.data} " +
                                $"WHERE status = 1 AND process_order.order_id = {item.GetValue("order_id")} LIMIT {model.max}";
                            var dataset_process_order_update = db.query_update(sql_process_order_update);
                            if (dataset_process_order_update.status == false)
                            {
                                return new JsonResult(NotFound());
                            }
                        }  
                        
                    }
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
        public JsonResult cancelReceive(int recvp_id)
        {
            string sql = $"UPDATE `receive_post` SET `allowJoin` = '0' WHERE `receive_post`.`recvp_id` = {recvp_id};";
            var dataset_update_receive = db.query_update(sql);

            sql = $"SELECT * FROM `process_order` WHERE recvp_id = {recvp_id}";
            var dataset = db.query_select(sql);

            OrderController order_control = new OrderController();

            foreach (JObject item in JArray.Parse(dataset.data))
            {
                int order_id = item.GetValue("order_id").ToObject<int>();
                order_control.cancelOrder(order_id);
            }


            return new JsonResult(Ok());
        }
    }
}

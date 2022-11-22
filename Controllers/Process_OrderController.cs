using FoodPenguinAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;

namespace FoodPenguinAPI.Controllers
{
    public class Process_OrderController : Controller
    {
        Database db = new Database();

        [HttpGet]
        public JsonResult getProcessOrderDetailByOrderID(int order_id)
        {
            string sql = $"SELECT * FROM `process_order` WHERE order_id = {order_id}";

            var dataset = db.query_select(sql, singleValue: true);


            if (dataset.row == 0)
            {
                return new JsonResult(NotFound());
            }

            return new JsonResult(Ok(JObject.Parse(dataset.data).ToObject<Process_OrderModel>()));
        }

        [HttpPut]
        public JsonResult updateProcessOrderByOrderID([FromBody] Process_OrderModel model)
        {
            if (model.order_id != null)
            {
                List<string> process_data = new List<string>();

                if (model.recvp_id != null)
                {
                    if (model.recvp_id == 0)
                    {
                        process_data.Add($"`recvp_id`= NULL");
                    }
                    else
                    {
                        process_data.Add($"`recvp_id`='{model.recvp_id}'");
                    }
                }
                if (model.order_id != null) { process_data.Add($"`order_id`='{model.order_id}'"); }
                if (model.status != null) { process_data.Add($"`status`='{model.status}'"); }
                if (model.pay_img != null) { process_data.Add($"`pay_img`='{model.pay_img}'"); }
                if (model.finish != null) { process_data.Add($"`finish`='{model.finish}'"); }

                string listToSql = string.Join(",", process_data);

                string sql = $"UPDATE `process_order` SET {listToSql} WHERE `process_order`.`order_id` = {model.order_id}";
                var dataset = db.query_update(sql);

                if (dataset.status)
                {
                    return new JsonResult(Ok(model));
                }
                else
                {
                    return new JsonResult(NotFound());
                }
            }
            return new JsonResult(NotFound());
        }

        [HttpPut]
        public JsonResult updateProcessOrderByRecvpID([FromBody] Process_OrderModel model)
        {
            Console.WriteLine("asfasf");
            if (model.recvp_id != null)
            {
                List<string> process_data = new List<string>();

                if (model.recvp_id != null) { process_data.Add($"`recvp_id`='{model.recvp_id}'"); }
                if (model.order_id != null) { process_data.Add($"`order_id`='{model.order_id}'"); }
                if (model.status != null) {

                    Console.WriteLine(model.status);

                    if (model.status == 2)
                    {
                        string sql_edit_allowJoin = $"UPDATE `receive_post` SET `allowJoin` = '0' " +
                            $"WHERE `receive_post`.`recvp_id` = {model.recvp_id};";

                        var dataset_edit_allowJoin = db.query_update(sql_edit_allowJoin);

                        if (!dataset_edit_allowJoin.status)
                        {
                            return new JsonResult(NotFound());
                        }
                    }
                    
                    process_data.Add($"`status`='{model.status}'"); 
                
                }
                if (model.pay_img != null) { process_data.Add($"`pay_img`='{model.pay_img}'"); }
                if (model.finish != null) {

                    int finish_int = 0;

                    if(model.finish == true)
                    {
                        finish_int = 1;
                    }
                    else
                    {
                        finish_int = 0;
                    }

                    process_data.Add($"`finish`='{finish_int}'"); 
                }

                string listToSql = string.Join(",", process_data);

                string sql = $"UPDATE `process_order` SET {listToSql} WHERE `process_order`.`recvp_id` = {model.recvp_id}";
                var dataset = db.query_update(sql);

                if (dataset.status)
                {
                    return new JsonResult(Ok(model));
                }
                else
                {
                    return new JsonResult(NotFound());
                }
            }
            return new JsonResult(NotFound());
        }
    }
}

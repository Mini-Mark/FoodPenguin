using FoodPenguinAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;

namespace FoodPenguinAPI.Controllers
{
    public class Process_FoodController : Controller
    {
        Database db = new Database();

        [HttpGet]
        public JsonResult getProcessFoodDetailByOrderID(int order_id)
        {
            string sql = $"SELECT * FROM `process_food` WHERE order_id = {order_id}";

            var dataset = db.query_select(sql);

            if (dataset.row == 0)
            {
                return new JsonResult(NotFound());
            }

            return new JsonResult(Ok(JArray.Parse(dataset.data.Replace("\"finish\":true", "\"finish\":1").Replace("\"finish\":false", "\"finish\":0")).ToObject<List<Process_FoodModel>>()));
        }

        [HttpPut]
        public JsonResult updateProcessFood([FromBody] Process_FoodModel model)
        {
            if (model.food_id != null)
            {
                List<string> food_data = new List<string>();

                if (model.order_id != null) { food_data.Add($"`order_id`='{model.order_id}'"); }
                if (model.price != null) { food_data.Add($"`price`='{model.price}'"); }
                if (model.remark != null) { 
                    
                    if(model.remark == "")
                    {
                        food_data.Add($"`remark`= NULL");
                    }
                    else
                    {
                        food_data.Add($"`remark`='{model.remark}'");
                    }
                    
                
                }
                if (model.finish != null) { food_data.Add($"`finish`='{model.finish}'"); }

                string listToSql = string.Join(",", food_data);

                string sql = $"UPDATE `process_food` SET {listToSql} WHERE `process_food`.`food_id` = {model.food_id}";
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

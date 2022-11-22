using FoodPenguinAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json.Linq;
using System.Web;

namespace FoodPenguinAPI.Controllers
{
    public class UserController : Controller
    {
        Database db = new Database();

        [HttpGet]
        public JsonResult Login(string username, string password)
        {

            string sql = $"SELECT * FROM user WHERE username = '{username}' AND password = '{password}'";

            var dataset = db.query_select(sql, singleValue: true);

            if (dataset.row == 1)
            {
                //To Model
                UserModel model = new UserModel();

                model.user_id = JObject.Parse(dataset.data).GetValue("user_id").ToObject<int>();
                model.username = JObject.Parse(dataset.data).GetValue("username").ToString();
                model.password = JObject.Parse(dataset.data).GetValue("password").ToString();
                model.firstname = JObject.Parse(dataset.data).GetValue("firstname").ToString();
                model.lastname = JObject.Parse(dataset.data).GetValue("lastname").ToString();
                model.nickname = JObject.Parse(dataset.data).GetValue("nickname").ToString();
                model.pay_img = JObject.Parse(dataset.data).GetValue("pay_img").ToString();
                model.profile_img = JObject.Parse(dataset.data).GetValue("profile_img").ToString();

                return new JsonResult(Ok(model));
            }
            else
            {
                return new JsonResult(NotFound());
            }
        }

        [HttpGet]
        public JsonResult getUserData(int user_id, Boolean no_pass = true)
        {
            string sql = $"SELECT * FROM user WHERE user_id = '{user_id}'";

            var dataset = db.query_select(sql, singleValue: true);

            if (dataset.row == 1)
            {
                //To Model
                UserModel model = new UserModel();
                JObject datasetJson = JObject.Parse(dataset.data);

                model.user_id = datasetJson.GetValue("user_id").ToObject<int>();
                model.username = datasetJson.GetValue("username").ToString();
                model.firstname = datasetJson.GetValue("firstname").ToString();
                model.lastname = datasetJson.GetValue("lastname").ToString();
                model.nickname = datasetJson.GetValue("nickname").ToString();
                model.pay_img = datasetJson.GetValue("pay_img").ToString();
                model.profile_img = datasetJson.GetValue("profile_img").ToString();

                if (!no_pass)
                {
                    model.password = JObject.Parse(dataset.data).GetValue("password").ToString();
                }
                return new JsonResult(Ok(model));
            }
            else //Notfound User
            {
                //return Content(Helper.sendError(dataset.data), "application/json");
                return new JsonResult(NotFound());
            }
        }

        [HttpGet]
        public JsonResult getUserHistory(int user_id)
        {

            string sql = "SELECT order_id as 'id',order_post.user_id,user.firstname,user.lastname,user.nickname,order_post.loc_id," +
                    "location.name as 'location_name',remark, market.name as 'special',time,'order' as 'type' FROM `order_post` " +
                    "INNER JOIN `user` ON `user`.`user_id` = order_post.user_id " +
                    "INNER JOIN location ON location.loc_id = order_post.loc_id " +
                    $"INNER JOIN market ON market.market_id = order_post.market_id WHERE order_post.user_id = {user_id} " +
                    "UNION SELECT recvp_id, receive_post.user_id,user.firstname,user.lastname,user.nickname,receive_post.loc_id,location.name,`desc`,`max`,time,'receive' " +
                    "FROM `receive_post` INNER JOIN `user` ON `user`.`user_id` = receive_post.user_id " +
                    $"INNER JOIN location ON location.loc_id = receive_post.loc_id WHERE receive_post.user_id = {user_id} ORDER BY time DESC";

            var dataset = db.query_select(sql);
            //JObject manageJson = new JObject();
            //manageJson.Add("data", JArray.Parse(dataset.data));

            if (dataset.row != 0)
            {
                //To Model
                List<ListModel.All> model = new List<ListModel.All>();
                foreach (JObject item in JArray.Parse(dataset.data))
                {

                    ListModel.All subModel = new ListModel.All();
                    subModel.id = item.GetValue("id").ToObject<int>();
                    subModel.user_id = item.GetValue("user_id").ToObject<int>();
                    subModel.loc_id = item.GetValue("loc_id").ToObject<int>();
                    subModel.firstname = item.GetValue("firstname").ToString();
                    subModel.lastname = item.GetValue("lastname").ToString();
                    subModel.nickname = item.GetValue("nickname").ToString();
                    subModel.location_name = item.GetValue("location_name").ToString();
                    subModel.remark = item.GetValue("remark").ToString();
                    subModel.special = item.GetValue("special").ToString();
                    subModel.time = item.GetValue("time").ToString();
                    subModel.type = item.GetValue("type").ToString();

                    model.Add(subModel);
                }
                return new JsonResult(Ok(model));
            }
            else
            {
                return new JsonResult(NotFound());
            }
        }

        [HttpPut]
        public JsonResult updateUserData([FromBody] UserModel model)
        {
            if (model.user_id != null)
            {

                List<string> profile_data = new List<string>();

                if (model.username != null) { profile_data.Add($"`username`='{model.username}'"); }
                if (model.password != null) { profile_data.Add($"`password`='{model.password}'"); }
                if (model.firstname != null) { profile_data.Add($"`firstname`='{model.firstname}'"); }
                if (model.lastname != null) { profile_data.Add($"`lastname`='{model.lastname}'"); }
                if (model.nickname != null) { profile_data.Add($"`nickname`='{model.nickname}'"); }
                if (model.pay_img != null) { profile_data.Add($"`pay_img`='{model.pay_img}'"); }
                if (model.profile_img != null) { profile_data.Add($"`profile_img`='{model.profile_img}'"); }

                string listToSql = string.Join(",", profile_data);

                string sql = $"UPDATE user SET {listToSql} WHERE `user_id` = {model.user_id}";
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


            //string sql = $"SELECT * FROM user WHERE username = '{username}' AND password = '{password}'";

            //var dataset = db.query_select(sql);
        }
    }
}

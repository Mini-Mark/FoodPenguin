using FoodPenguinAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FoodPenguinAPI.Controllers
{
    public class LocationController : Controller
    {
        Database db = new Database();

        [HttpGet]
        public JsonResult getMarketFromLocID(int loc_id)
        {
            string sql = $"SELECT * FROM market WHERE loc_id = {loc_id}";

            var dataset = db.query_select(sql);

            if (dataset.row != 0)
            {
                //To Model

                Console.WriteLine(dataset.data);

                List<MarketModel> model = new List<MarketModel>();
                foreach (JObject item in JArray.Parse(dataset.data))
                {
                    MarketModel submodel = new MarketModel();
                    submodel.market_id = item.GetValue("market_id").ToObject<int>();
                    submodel.name = item.GetValue("name").ToString();

                    model.Add(submodel);
                }

                return new JsonResult(Ok(model));
            }
            else
            {
                return new JsonResult(NotFound());
            }
        }

        [HttpGet]
        public JsonResult getLocation()
        {
            string sql = $"SELECT * FROM location";

            var dataset = db.query_select(sql);

            //JObject manageJson = new JObject();
            //manageJson.Add("data", JArray.Parse(dataset.data));

            if (dataset.row != 0)
            {
                //To Model

                Console.WriteLine(dataset.data);

                List<LocationModel> model = new List<LocationModel>();
                foreach (JObject item in JArray.Parse(dataset.data))
                {
                    LocationModel submodel = new LocationModel();
                    submodel.loc_id = item.GetValue("loc_id").ToObject<int>();
                    submodel.name = item.GetValue("name").ToString();

                    model.Add(submodel);
                }

                return new JsonResult(Ok(model));
            }
            else
            {
                return new JsonResult(NotFound());
            }

        }
    }
}

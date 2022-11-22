using Newtonsoft.Json.Linq;
using System.Data;

namespace FoodPenguinAPI
{
    public class Helper
    {

        static public string sendSuccess(string json = null)
        {
            JObject manageJson;
            try
            {
                manageJson = JObject.Parse(json);
            }
            catch (Exception ex)
            {
                manageJson = new JObject();
            }
            manageJson.Add("API_Status", "Success");
            return manageJson.ToString();
        }
        static public string sendError(string json = null)
        {
            JObject manageJson;
            try
            {
                manageJson = JObject.Parse(json);
            }
            catch (Exception ex)
            {
                manageJson = new JObject();
            }
            manageJson.Add("API_Status", "Failed");
            return manageJson.ToString();
        }

    }
}

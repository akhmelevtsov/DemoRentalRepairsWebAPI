using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Infrastructure.Notifications.Models;
using Demo.RentalRepairs.Infrastructure.Repositories.EF.Entities;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.Infrastructure.Notifications
{
    public class  ModelSerializer
    {

        //----- Requests
        public NotificationQueueModel Deserialize(string r)
        {
            //var decoded = Base64Decode(r);
            var req = JsonConvert.DeserializeObject<NotificationQueueModel>(r, JsonSettings());

            return req;
        }



        internal string Serialize(NotificationQueueModel req)
        {
            var str=  JsonConvert.SerializeObject(req, JsonSettings());
            return Base64Encode(str);
        }

       

        private JsonSerializerSettings JsonSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                //MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            };
            return settings;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}

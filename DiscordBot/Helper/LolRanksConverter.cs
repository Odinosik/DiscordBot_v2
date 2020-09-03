using DiscordBot.Requests.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Helper
{
    public class LolRanksConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;
            switch (enumString)
            {
                case "IRON":
                    {
                        return eLolRanks.IRON;
                    }
                case "SILVER":
                    {
                        return eLolRanks.SILVER;
                    }
                case "GOLD":
                    {
                        return eLolRanks.GOLD;
                    }
                case "PLATINUM":
                    {
                        return eLolRanks.PLATINUM;
                    }
                default:
                    {
                        return eLolRanks.DIAMOND;
                    }
            }

        }
    }

    public class LolRomeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;
            switch (enumString)
            {
                case "I":
                    {
                        return eLolRome.I;
                    }
                case "II":
                    {
                        return eLolRome.II;
                    }
                case "III":
                    {
                        return eLolRome.III;
                    }
                case "IV":
                    {
                        return eLolRome.IV;
                    }
                default:
                    {
                        return eLolRome.IV;
                    }
            }

        }
    }

}

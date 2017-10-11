using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BattleShipGame
{
    [JsonConverter(typeof(DictionaryConverter))]
    public class TileDictionary : Dictionary<Coordinate, Tile>
    {
        //private readonly Dictionary<Coordinate, Tile> _dict;

        //public TileDictionary() { _dict = new Dictionary<Coordinate, Tile>(); }

        //public TileDictionary(IEnumerable<KeyValuePair<Coordinate, Tile>> items) { _dict = items.ToArray().ToDictionary(x=> x.Key, x=> x.Value); }

        //public static implicit operator TileDictionary(KeyValuePair<Coordinate, Tile>[] items) => new TileDictionary(items);
        //public static implicit operator TileDictionary(List<KeyValuePair<Coordinate, Tile>> items) => new TileDictionary(items);

        //public void Add(Coordinate key, Tile value)
        //{
        //    _dict.Add(key, value);
        //}

    }

    public class DictionaryConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => 
            serializer.Serialize(writer, value);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dict = new TileDictionary();
            var cr = new Coordinate();

            var started = true;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    default:
                        break;
                    case JsonToken.StartObject:
                        started = true;
                        break;
                    
                    case JsonToken.EndObject:
                        if (!started) return dict;
                        started = false;
                        break;
                    case JsonToken.PropertyName:
                        var readerValue = reader.Value.ToString().Split(',');
                        if (readerValue.Length == 2)
                            cr = new Coordinate(int.Parse(readerValue[0]), int.Parse(readerValue[1]));
                        break;
                    case JsonToken.Integer:
                            dict.Add(cr, new Tile { Status = (TileStatus) Enum.Parse(typeof (TileStatus), reader.Value.ToString()) });
                        break;
                }

               
            }

            return dict;
        }

        public override bool CanWrite => false;
        public override bool CanConvert(Type objectType) => typeof(TileDictionary).IsAssignableFrom(objectType);
    }
}

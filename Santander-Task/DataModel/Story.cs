using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Santander_Task.DataModel
{
    public class Story
    {
            public string By { get; set; }
            public string Descendants { get; set; }
            public int Id { get; set; }
            public int Score { get; set; }

            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime Time { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
    }
}

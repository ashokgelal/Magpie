using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MagpieUpdater.Models
{
    [DataContract]
    public class RemoteAppcast
    {
        [DataMember(Name = "channels", IsRequired = true)]
        public List<Channel> Channels { get; private set; }

        public Dictionary<string, object> RawDictionary { get; internal set; }

        public static RemoteAppcast MakeFromJson(string json)
        {
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var settings = new DataContractJsonSerializerSettings {UseSimpleDictionaryFormat = true};
                    var serializer = new DataContractJsonSerializer(typeof(RemoteAppcast), settings);
                    var appcast = (RemoteAppcast) serializer.ReadObject(ms);

                    ms.Seek(0, SeekOrigin.Begin);
                    serializer = new DataContractJsonSerializer(typeof(Dictionary<string, object>), settings);
                    var results = (Dictionary<string, object>) serializer.ReadObject(ms);
                    appcast.RawDictionary = results;
                    return appcast;
                }
            }
            catch (SerializationException)
            {
                return new RemoteAppcast()
                {
                    Channels = new List<Channel>(),
                    RawDictionary = new Dictionary<string, object>()
                };
            }
        }
    }
}
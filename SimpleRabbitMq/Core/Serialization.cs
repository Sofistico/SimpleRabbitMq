using System.Text;
using Newtonsoft.Json;

namespace SimpleRabbitMq.Core;

public class Serialization
{
    public static T DeserializeFromRabbitMQ<T>(byte[] byteArray)
    {
        if (byteArray.Length <= 0)
            new ArgumentException("The byteArray cannot be empty");

        string json = Encoding.UTF8.GetString(byteArray);

        return JsonConvert.DeserializeObject<T>(
            json
        );
    }

    public static byte[] SerializeToRabbitMQ(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        var s = JsonConvert.SerializeObject(obj);

        return Encoding.UTF8.GetBytes(s);
    }
}


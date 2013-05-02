using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VLT
{
    static class Serialization
    {
        // Save an object out to the disk
        public static void SerializeObject<T>(this T toSerialize, String filename)
        {
            FileStream fileStream = File.OpenWrite(filename);
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(fileStream, toSerialize);
        }

        /*public static T SerializeFromString<T>(string xml)
        {
            BinaryFormatter serializer = new BinaryFormatter(typeof(T));

            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }*/
    }
}

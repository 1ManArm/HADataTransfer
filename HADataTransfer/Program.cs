using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace HADataTransfer
{
    internal class Program
    {
        public class DataTransfer
        {
            public event Action<string> DataSent;
            public event Action<string> DataReceived;

            public void SendDataAsText(string data, string filePath)
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(data);
                }
                DataSent?.Invoke("Data sent as text");
            }

            public string ReceiveDataAsText(string filePath)
            {
                string data;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    data = reader.ReadToEnd();
                }
                DataReceived?.Invoke("Data received as text");
                return data;
            }

            public void SendDataAsJson<T>(T data, string filePath)
            {
                string jsonData = JsonSerializer.Serialize(data);
                File.WriteAllText(filePath, jsonData);
                DataSent?.Invoke("Data sent as JSON");
            }

            public T ReceiveDataAsJson<T>(string filePath)
            {
                string jsonData = File.ReadAllText(filePath);
                T data = JsonSerializer.Deserialize<T>(jsonData);
                DataReceived?.Invoke("Data received as JSON");
                return data;
            }

            public void SendDataAsXml<T>(T data, string filePath)
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(stream, data);
                }
                DataSent?.Invoke("Data sent as XML");
            }

            public T ReceiveDataAsXml<T>(string filePath)
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    T data = (T)serializer.Deserialize(stream);
                    DataReceived?.Invoke("Data received as XML");
                    return data;
                }
            }
        }
        static void Main(string[] args)
        {
            DataTransfer dataTransfer = new DataTransfer();

            dataTransfer.DataSent += message => Console.WriteLine("Sent: " + message);
            dataTransfer.DataReceived += message => Console.WriteLine("Received: " + message);

            dataTransfer.SendDataAsText("Hello, this is a text message", "textFile.txt");
            string textData = dataTransfer.ReceiveDataAsText("textFile.txt");
            Console.WriteLine("Received text data: " + textData);

            dataTransfer.SendDataAsJson(new { Name = "Alice", Age = 30 }, "data.json");
            var jsonData = dataTransfer.ReceiveDataAsJson<dynamic>("data.json");
            Console.WriteLine("Received JSON data: " + jsonData.Name + " - " + jsonData.Age);

            dataTransfer.SendDataAsXml(new { Name = "Bob", Age = 25 }, "data.xml");
            var xmlData = dataTransfer.ReceiveDataAsXml<dynamic>("data.xml");
            Console.WriteLine("Received XML data: " + xmlData.Name + " - " + xmlData.Age);
        }
    }
}
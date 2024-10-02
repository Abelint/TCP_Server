using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Web;
using ConsoleApp1.Model;
using System.Data.Common;
using ConsoleApp1.DB;

namespace ConsoleApp1
{
    internal class Program
    {
        static MyDBConnection db;
        static void Main()
        {
            string stroka = FromDBstr();
            Console.WriteLine(stroka);


            TcpListener server = null;
            //try
            //{
                // Задаем IP-адрес и порт сервера
            IPAddress ipAddress = IPAddress.Parse("192.168.0.153");
            int port = 80;

                // Создаем TcpListener для прослушивания входящих подключений
            server = new TcpListener(ipAddress, port);

                // Запускаем сервер
            server.Start();

            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            // таким образом можно сделать авторизацию с получением значения по id
            while (true)
            {
                TcpClient _client = server.AcceptTcpClient();
               
                NetworkStream streamNetwork = _client.GetStream();
                    
                byte[] data = new byte[1024];
                int _bytesRead = streamNetwork.Read(data, 0, data.Length);
                string request = Encoding.ASCII.GetString(data, 0, _bytesRead);
                try
                {

                
                        // Разбор запроса и получение параметров
                    var parameters = ParseRequestParameters(request);
               
                        // Получение значений параметров
                    string val1 = parameters[0]["id"];
                    string val2 = parameters[1]["name"];
                    string val3 = parameters[2]["status"];

                    DBO dbo = new DBO();
                    dbo.Id = Convert.ToInt32(val1);
                    dbo.Name = val2;
                    dbo.IsCompllete = Convert.ToBoolean(val3);
                    db.InsertUser(dbo);
                    // Ваш код для обработки значений параметров
                    string _response = $"val1 = {val1}, val2 = {val2}";
                    Console.WriteLine(_response);
                    stroka = FromDBstr();
                    Response(ref streamNetwork, stroka);
                }
                catch(Exception ex) {
                
                    Console.WriteLine(ex.Message);
                }
                _client.Close();
            }
                    //Console.ReadKey();
                    // Закрываем подключение
                   
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Ошибка: " + ex.Message);
            //}
            //finally
            //{
            //    // Останавливаем сервер
            //    //server.Stop();
            //}
        }
        //http://192.168.0.153:8080/?id=4&name=b&status=true
        static void Response(ref NetworkStream stream, string stroka)
        {

            // Создаем HTTP-ответ

            //string adder = "<html><body><h1>"+stroka+"</h1></body></html>";
            string adder =  stroka;
            int ltn=  adder.Length;
            string response = "HTTP/1.1 404 Not found\r\n" +
                              "Server: mywebserver/1.0.0(Win32)\r\n" +
                              "Content-Length: "+ltn.ToString()+"\r\n" +
                              "\r\n";
            response += adder;
            // Преобразуем строку в массив байтов
            byte[] data = Encoding.UTF8.GetBytes(response);

            // Отправляем данные клиенту
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Ответ отправлен.");
        }

        static List<Dictionary<string, string>> ParseRequestParameters(string request)
        {
            List<Dictionary<string, string>> parameters = new List<Dictionary<string, string>>();

            // Получаем часть запроса после символа "?"
            int queryIndex = request.IndexOf('?');
            if (queryIndex >= 0 && queryIndex < request.Length - 1)
            {
                string queryString = request.Substring(queryIndex + 1);
                queryString = queryString.Split("HTTP")[0].Trim();
                // Разделяем параметры
                var queryParts = queryString.Split('&');
                foreach (var part in queryParts)
                {
                    // Разделяем имя и значение параметра
                    var pair = part.Split('=');
                    if (pair.Length == 2)
                    {
                        string key = HttpUtility.UrlDecode(pair[0]);
                        string value = HttpUtility.UrlDecode(pair[1]);
                        Dictionary<string, string> _parameter = new Dictionary<string, string>();
                        // Добавляем параметр в словарь
                        _parameter[key] = value;
                        parameters.Add(_parameter);
                    }
                }
            }

            return parameters;
        }

        ///////////////////////////////////////////// работа с базой
        
        List<DBO> FromDB()
        {
            return db.GetItem();
        }
        static  string  FromDBstr()
        {
            db = new MyDBConnection();
           
            List<DBO> ds = db.GetItem();
            string str = "";
            foreach(DBO item in ds)
            {
                str+=+item.Id + " " + item.Name + " " +item.IsCompllete;
            }
            return str;
        }
    }
}

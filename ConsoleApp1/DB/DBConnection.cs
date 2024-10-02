using ConsoleApp1.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Diagnostics;

using Dapper;


namespace ConsoleApp1.DB
{
    internal class MyDBConnection : Controller
    {
        string strConnect = "Persist Security Info=False;Trusted_Connection=True;database=ForStudents;server=DESKTOP-FNPDG5H;Encrypt=True;TrustServerCertificate=true;";
       // string strConnect = "Persist Security Info=False;Trusted_Connection=True;database=ForStudents;server=DESKTOP-FNPDG5H;Encrypt=False;TrustServerCertificate=true;";
      // string strConnect = "Server=DESKTOP-FNPDG5H;Database=ForStudents;User Id=YourUserId;Password=YourPassword;Encrypt=True;TrustServerCertificate=true;";


        public IDbConnection Connection
        {
            get
            {
                
                return new SqlConnection(strConnect);
            }
        }

        //public IActionResult Index()
        //{
        //    var model = GetItem();

        //    return View();
        //}
        public List<DBO> GetItem()
        {
            
            using (IDbConnection db = Connection)
            {

                var result = db.Query<DBO>("SELECT * FROM Users").ToList();

                return result;
            }
        }
        public void Conn()
        {
            new SqlConnection(strConnect);
        }

        // Метод для вставки данных в таблицу 
        public void InsertUser(DBO dBO)
        {
            using (IDbConnection db = Connection)
            {
                // SQL-запрос для вставки данных
                var sqlQuery = "INSERT INTO Users (Name, isComplete) VALUES (@Name, @isComplete)";

                // Параметры для вставки
                var parameters = new
                {
                    name = dBO.Name,
                    isComplete = dBO.IsCompllete
                };

                // Выполнение запроса
                db.Execute(sqlQuery, parameters);
            }
        }
    }
}

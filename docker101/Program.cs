using System;
using StackExchange.Redis;
namespace docker101;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        while (true)
        {
            try
            {
                Thread.Sleep(2000);

#region Code from outside container 
                var con = ConnectionMultiplexer.Connect("localhost:6379");
                var db = con.GetDatabase();
                var value = db.StringGet("mykey1");
                Console.WriteLine($"redis mykey : {value}");
                con.Dispose();
#endregion

               #region Code from inside container 
                    var con = ConnectionMultiplexer.Connect("redisdb:6379");

                    var db = con.GetDatabase();
                    db.StringSet("mykey2", new RedisValue("sample value from c# code"));

                    Console.WriteLine("value set;");

                    var value = db.StringGet("mykey1");
                    var value2 = db.StringGet("mykey2");

                    Console.WriteLine($"redis mykey1 : {value}");
                    Console.WriteLine($"redis mykey2 : {value2}");
                    con.Dispose();
               #endregion
            }
            catch (Exception)
            {
                System.Console.WriteLine("exception");
            }
            finally
            {
                System.Console.WriteLine(DateTime.Now.ToString());
            }
        }



    }
}

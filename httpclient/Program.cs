// See https://aka.ms/new-console-template for more information
using httpclient;
using System.Net;
using System.Text;
using System.Xml;
using static System.Net.WebRequestMethods;

class Program
{
    static async Task Main(String[] args)
    {
        //var client = new Client();
        //// ////await client.TestClient2();
        //// ////Console.WriteLine("-------------------");
        //// ////await client.GetData();
        //// //await client.testFile();

        //await client.testSocketHttpHander();
        //
        //Hander hd  = new Hander();
        ////await hd.testHander2();  
        //httpListener lis = new httpListener();
        //await lis.testListener();

        //
        var server = new MyhtttpServer(new string[] { "http://localhost:8080/" });
        await server.Start();
        
    }
}

using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.IO;
using CIT_Assignment03;

Console.WriteLine("Hello, World!");

var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
server.Start();

Console.WriteLine("server is started.......");

//int i = 0;
while (true)
{
    Console.WriteLine("waiting for a connection....");
    var client = server.AcceptTcpClient();
    Console.WriteLine("client accepted");
    try
    {
        var stream = client.GetStream();

        var request = readRequest(stream);

        var response = new Response("", "");
        Console.WriteLine("THIS IS THE REQUEST METHOD:");
        Console.WriteLine(request.Method);

        Console.WriteLine("before checkRequest");
        checkRequest(request, response);
        Console.WriteLine("after checkRequest");

        sendRespond(stream, response);

        stream.Close();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }


    void sendRespond(NetworkStream stream, Response response)
    {
        var responseAsJson = JsonSerializer.Serialize<Response>(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        //  Console.WriteLine(responseAsJson);

        var responseBuffer = Encoding.UTF8.GetBytes(responseAsJson);

        //  Console.WriteLine(responseBuffer[4]);

        stream.Write(responseBuffer);
    }

    Request readRequest(NetworkStream stream)
    {
        var buffer = new byte[1024];
        var rcnt = stream.Read(buffer, 0, buffer.Length);
        var requestJson = Encoding.ASCII.GetString(buffer, 0, rcnt);
        Console.WriteLine(requestJson);
        var requestFromJson = JsonSerializer.Deserialize<Request>(requestJson);
        Console.WriteLine(requestFromJson);
        Console.WriteLine("readRequest done");
        return requestFromJson;
    }

    void checkRequest(Request request, Response response)
    {
        Console.WriteLine("first in checkRequest");

        // check if null and does missing xxxx:
        if (request.Method == null)
        {
            request.Method = "";
            response.addToStatus("missing method");
        }
        if (request.Path == null)
        {
            request.Path = "";
            response.addToStatus("missing path");
        }
        if (request.Date == null)
        {
            request.Date = DateTime.Now;
            response.addToStatus("missing date");
        }

        Console.WriteLine("THIS IS THE STATUS AFTER MISSING...");
        Console.WriteLine(response.Status);

        Console.WriteLine("after missing...");

        // string[] methods = { "create", "read", "update", "delete", "echo" };

        Console.WriteLine("starting illegal...");
        if (request.Method == "create")
        {
            response.addToStatus("illegal method");
        }

        Console.WriteLine("THIS IS STATUS AFTER ILLEGAL:");
        Console.WriteLine(response.Status);
    }
}
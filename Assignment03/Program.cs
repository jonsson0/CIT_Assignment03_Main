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

while (true)
{
    Console.WriteLine("waiting for a connection....");
    var client = server.AcceptTcpClient();
    Console.WriteLine("client accepted");
    try
    {
        var stream = client.GetStream();
        
        var request = readRequest(stream);

        Console.WriteLine("This is the request as json:");
        Console.WriteLine(JsonSerializer.Serialize<Request>(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

        var response = new Response(null, null);
        Console.WriteLine("THIS IS THE NEW RESPONSE:");
        Console.WriteLine(JsonSerializer.Serialize<Response>(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));


        if (request != null )
        {
           
            Console.WriteLine("THIS IS THE REQUEST METHOD:");
            Console.WriteLine(request.Method);

            Console.WriteLine("before checkRequest");
            checkRequest(request, response);
            Console.WriteLine("after checkRequest");

            sendRespond(stream, response);
        }

       
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
        var requestFromJson = new Request();
        requestFromJson = JsonSerializer.Deserialize<Request>(requestJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        Console.WriteLine(requestFromJson);
        Console.WriteLine("readRequest done");
        return requestFromJson;
    }

    void checkRequest(Request request, Response response)
    {
        Console.WriteLine("first in checkRequest");

        Console.WriteLine("THIS IS THE request:");
        Console.WriteLine(JsonSerializer.Serialize<Request>(request));

        // check if null and does missing xxxx:
        if (request.Method == null)
        {
            request.Method = "";
            response.addToStatus("missing method");
        }
        if (request.Path == null)
        {
            request.Path = "";
            response.addToStatus("missing resource");
        }
        if (request.Date == null)
        {
            request.Date = "";
            response.addToStatus("missing date");
        }
        if (request.Body == null && (request.Method.Equals("create") || request.Method.Equals("update") || request.Method.Equals("echo")))
        {
            request.Body = "";
            response.addToStatus("missing body");
        }
       
        Console.WriteLine("THIS IS THE RESPONSE:");
        Console.WriteLine(JsonSerializer.Serialize<Response>(response));

        Console.WriteLine("after missing...");

        Console.WriteLine("THIS IS THE request:");
        Console.WriteLine(JsonSerializer.Serialize<Request>(request));

        // string[] methods = { "create", "read", "update", "delete", "echo" };

        Console.WriteLine("starting illegal...");
        if (request.Method.Equals("create") || request.Method.Equals("read") || request.Method.Equals("update") || request.Method.Equals("delete") || request.Method.Equals("echo"))
        {
            Console.WriteLine("THIS IS THE REQUEST AS JSON");
            Console.WriteLine(JsonSerializer.Serialize<Request>(request));
           
        }
        else
        {
            response.addToStatus("illegal method");
        }

        if (request.Date.Length != UnixTimestamp().Length)
        {
            response.addToStatus("illegal date");
        }

        if (request.Path.Contains("api/categories") && !request.Body.StartsWith("{") && !request.Body.EndsWith("}"))
        {
            response.addToStatus("illegal body");

        }

        if (request.Date.Length != UnixTimestamp().Length)
        {
            response.addToStatus("illegal date");
        }
        Console.WriteLine("THIS IS THE RESPONSE AFTER ILLEGAL:");
        Console.WriteLine(JsonSerializer.Serialize<Response>(response));

        if (request.Method == "echo")
        {
            response.Body = request.Body;
        }

        // CHECK API:
        if (response.Status == null)
        {
            string[] apiElements = request.Path.Split('/');
            Console.WriteLine("0 spot:");
            Console.WriteLine(apiElements[0]);
            Console.WriteLine("1st spot:");
            Console.WriteLine(apiElements[1]);
            Console.WriteLine("2nd spot:");
            Console.WriteLine(apiElements[2]);
          Console.WriteLine("this is its length:");
          Console.WriteLine(apiElements.Length);

         
            
            if (apiElements.Length < 3)
            {
                response.Status = "4 Bad Request";
            }

            Console.WriteLine("checking if " + apiElements[1] + " = api og " + apiElements[2] + " = categories");
            if (apiElements.Length == 3 && apiElements[1].Equals("api") && apiElements[2].Equals("categories"))
            {

            }
            else
            {
                response.Status = "4 Bad Request";
            }
        }

    }
}
static string UnixTimestamp()
{
    return DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
}

static TcpClient Connect()
{
    var client = new TcpClient();
    client.Connect(IPAddress.Loopback, 5000);
    return client;
}
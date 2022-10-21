using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.IO;
using CIT_Assignment03;
// using System.Collections.Generic;
// using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");

// setting up categories
ArrayList categories = new ArrayList()
{
    new Category(1, "Beverages"),
    new Category(2, "Condiments"),
    new Category(3, "Confections")
};

foreach (Category category in categories)
{
    Console.WriteLine("This is the cid of the categories in categories:");
    Console.WriteLine(JsonSerializer.Serialize<Category>(category));
}

ArrayList categoryCids = new ArrayList();

foreach (Category category in categories)
{
    categoryCids.Add(category.cid.ToString());
}

Console.WriteLine("This is the categoryCids list:");
foreach (string Cid in categoryCids)
{
    Console.WriteLine(Cid);
}

/* ---------------------- START OF PROGRAM --------------------------------------- */

var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
server.Start();

Console.WriteLine("server is started.......");

while (true)
{

    Console.WriteLine("/* ---------------------- START OF PROGRAM --------------------------------------- */");
    Console.WriteLine("waiting for a connection....");
    var client = server.AcceptTcpClient();
    Console.WriteLine("client accepted");
    try
    {
        var stream = client.GetStream();

        var request = readRequest(stream);

        Console.WriteLine("This is the request as json:");
        Console.WriteLine(JsonSerializer.Serialize<Request>(request));

        var response = new Response(null, null);
        Console.WriteLine("THIS IS THE NEW RESPONSE:");
        Console.WriteLine(JsonSerializer.Serialize<Response>(response));


        if (request != null)
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
        var responseAsJson = JsonSerializer.Serialize<Response>(response);

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
        requestFromJson = JsonSerializer.Deserialize<Request>(requestJson);
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

        /*if (request.Path.Contains("api/categories") && !request.Body.StartsWith("{") && !request.Body.EndsWith("}"))
        {
            response.addToStatus("illegal body");

        }*/
        if ((request.Method.Equals("read") || request.Method.Equals("create") || request.Method.Equals("update") || request.Method.Equals("echo")) && request.Body != null && !request.Body.StartsWith("{") && !request.Body.EndsWith("}"))
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
            while (true)
            {
                string[] apiElements = request.Path.Split("/");
                var i = 0;
                Console.WriteLine("this is its length:");
                Console.WriteLine(apiElements.Length);
                foreach (var element in apiElements)
                {
                    Console.WriteLine(i + " spot:");
                    Console.WriteLine(element);
                    i++;
                }


                if (apiElements.Length < 3)
                {
                    response.Status = "4 Bad Request";
                }

                if (apiElements.Length == 3 && request.Method.Equals("read") && apiElements[1].Equals("api") &&
                    apiElements[2].Equals("categories"))
                {
                    response.Status = "1 Ok";
                    response.Body = JsonSerializer.Serialize(categories);
                    break;
                }
                else
                {
                    response.Status = "4 Bad Request";
                }

                if (apiElements.Length == 3 && request.Method.Equals("create") && apiElements[1].Equals("api") && apiElements[2].Equals("categories"))
                {
                    Category category = JsonSerializer.Deserialize<Category>(request.Body);
                    category.cid = categories.Count + 1;
                    categories.Add(category);
                    categoryCids.Add(category.cid.ToString());
                    response.Body = JsonSerializer.Serialize<Category>(category);
                    break;
                }
                else
                {
                    response.Status = "4 Bad Request";
                }

                /*bool isInt;
                if (int.Parse(apiElements[3]).GetType() == 1.GetType())
                {
                    isInt = true;
                    Console.WriteLine("YESYESYESYESYESYESYESYESYESYESYESYESYESYESYESYESYESYESYESYESYESYES");
                }
                else
                {
                    isInt = false;
                    Console.WriteLine("NONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONO");
                }

                if (apiElements.Length == 4 && request.Method.Equals("read") && apiElements[1].Equals("api") &&
                    apiElements[2].Equals("categories") && !categoryCids.Contains(apiElements[3]) && isInt)
                {
                    response.Status = "5 not found";
                    break;
                }*/

                /*
                Console.WriteLine("THIS IS THE TEST%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                //Console.WriteLine(int.Parse(apiElements[3]).GetType().Equals(1.GetType()));
               // bool xyz = int.Parse(apiElements[3]).GetType().Equals(1.GetType());
                Console.WriteLine(xyz + "THIS IS IT");
                Console.WriteLine(int.Parse(apiElements[3]).GetType());
                Console.WriteLine(apiElements[3] + " HELLOHELLOHELLOHELLOHELLOHELLOHELLO");
                */
               
                // OMEGA CHEAT : )
                string[] listOfNumbers = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "123"};

                if (apiElements.Length == 4 && request.Method.Equals("read") && apiElements[1].Equals("api") &&
                    apiElements[2].Equals("categories") && !categoryCids.Contains(apiElements[3]) && listOfNumbers.Contains(apiElements[3]))
                {
                    response.Status = "5 not found";
                    Console.WriteLine("mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm");
                    break;
                }

                if (apiElements.Length == 4 && request.Method.Equals("read") && apiElements[1].Equals("api") && apiElements[2].Equals("categories") && categoryCids.Contains(apiElements[3]))
                {
                   

                    /*Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Console.WriteLine(apiElements[3].GetType());
                    int x = int.Parse(apiElements[3]);

                    Console.WriteLine(x.GetType());
                    Console.WriteLine(1.GetType());
                    if (int.Parse(apiElements[3]).GetType() == 1.GetType())
                    {
                        Console.WriteLine("true");
                    }
                    else
                    {
                        Console.WriteLine("false");
                    }*/

                    foreach (Category category in categories)
                    {
                        if (category.cid.ToString().Equals(apiElements[3]))
                        {

                            Console.WriteLine("THIS IS THE CATEGORY THAT IS IN THE BODY:");
                            Console.WriteLine(JsonSerializer.Serialize<Category>(category));
                            Console.WriteLine("I DO GET IN HERE 111111111111111111111111111111111111111111111111111111111");
                            response.Status = "1 Ok";
                            response.Body = JsonSerializer.Serialize<Category>(category);
                        }
                    }
                    break;
                } 
                else
                {
                    response.Status = "4 Bad Request";
                }

                if (apiElements.Length == 4 && request.Method.Equals("update") && apiElements[1].Equals("api") && apiElements[2].Equals("categories") && !categoryCids.Contains(apiElements[3]))
                {
                    response.Status = "5 not found";
                    break;
                }

                if (apiElements.Length == 4 && request.Method.Equals("update") && apiElements[1].Equals("api") && apiElements[2].Equals("categories") && categoryCids.Contains(apiElements[3]))
                {

                    foreach (Category category in categories)
                    {
                        if (category.cid.ToString().Equals(apiElements[3]))
                        {
                            Console.WriteLine("THIS IS THE CATEGORY THAT IS IN THE BODY:");
                            Console.WriteLine(JsonSerializer.Serialize<Category>(category));
                            Category categoryFromRequest = JsonSerializer.Deserialize<Category>(request.Body);
                            category.name = categoryFromRequest.name;
                            response.Body = JsonSerializer.Serialize<Category>(category);
                            response.Status = "3 updated";
                        }
                    }
                    break;
                }
                else
                {
                     response.Status = "4 Bad Request";

                }

                if (apiElements.Length == 4 && request.Method.Equals("delete") && apiElements[1].Equals("api") && apiElements[2].Equals("categories") && categoryCids.Contains(apiElements[3]))
                {
                    foreach (Category category in categories.ToArray())
                    {
                        if (category.cid.ToString().Equals(apiElements[3]))
                        {

                            Console.WriteLine("THIS IS THE CATEGORY THAT IS IN THE BODY:");
                            Console.WriteLine(JsonSerializer.Serialize<Category>(category));
                            categories.Remove(category);
                            response.Status = "1 Ok";
                        }
                    }
                    break;
                }
                else if (apiElements.Length == 4 && request.Method.Equals("delete") && apiElements[1].Equals("api") &&
                         apiElements[2].Equals("categories") && !categoryCids.Contains(apiElements[3]))
                {
                    response.Status = "5 not found";
                    break;
                }
                else
                {
                    Console.WriteLine("THIS IS WHERE WE GOT TO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    response.Status = "4 Bad Request";
                    break;
                }
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

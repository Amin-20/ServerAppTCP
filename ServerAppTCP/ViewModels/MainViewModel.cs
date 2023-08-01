using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerAppTCP.ViewModels
{
    public class MainViewModel: BaseViewModel
    {
        static TcpListener listener = null;
        static BinaryReader br = null;
        static BinaryWriter bw = null;
        static List<TcpClient> Clients { get; set; }
        static void Main(string[] args)
        {
            var ip = IPAddress.Parse("10.1.18.1");
            var port = 27001;

            var ep = new IPEndPoint(ip, port);
            listener = new TcpListener(ep);
            listener.Start(10);
            Clients = new List<TcpClient>();

            Console.WriteLine($"Listening on {listener.LocalEndpoint}");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Clients.Add(client);
                Console.WriteLine($"{client.Client.RemoteEndPoint} connected...");

                Task.Run(() =>
                {
                    ////FOR ONLY ONE CLIENT
                    //var reader = Task.Run(() =>
                    //{
                    //    var stream = client.GetStream();
                    //    br = new BinaryReader(stream);
                    //    while (true)
                    //    {
                    //        var msg = br.ReadString();
                    //        Console.WriteLine($"CLIENT : {client.Client.RemoteEndPoint} : {msg}");
                    //    }
                    //});

                    //var writer = Task.Run(() =>
                    //{
                    //    var stream = client.GetStream();
                    //    bw = new BinaryWriter(stream);
                    //    while (true)
                    //    {
                    //        var msg = Console.ReadLine();
                    //        bw.Write(msg);
                    //    }
                    //});




                    var reader = Task.Run(() =>
                    {
                        foreach (var item in Clients)
                        {
                            Task.Run(() =>
                            {
                                while (true)
                                {
                                    var stream = item.GetStream();
                                    br = new BinaryReader(stream);
                                    try
                                    {
                                        var msg = br.ReadString();
                                        Console.WriteLine($"CLIENT : {item.Client.RemoteEndPoint} : {msg}");

                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine($"{item.Client.RemoteEndPoint} removed");
                                        Clients.Remove(item);
                                    }
                                }
                            }).Wait(50);
                        }
                    });


                    //OKAY
                    var writer = Task.Run(() =>
                    {
                        while (true)
                        {

                            var msg = Console.ReadLine();
                            foreach (var item in Clients)
                            {
                                var stream = item.GetStream();
                                bw = new BinaryWriter(stream);
                                bw.Write(msg);
                            }
                        }
                    });


                });
            }
        }


    }
}

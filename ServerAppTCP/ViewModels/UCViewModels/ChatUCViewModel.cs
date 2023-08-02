using ServerAppTCP.Commands;
using ServerAppTCP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerAppTCP.ViewModels.UCViewModels
{
    public class ChatUCViewModel : BaseViewModel
    {
        public RelayCommand SendCommand { get; set; }

        private string ipAdress;

        public string IpAdress
        {
            get { return ipAdress; }
            set { ipAdress = value; OnPropertyChanged(); }
        }

        private int port;
        public int Port
        {
            get { return port; }
            set { port = value; OnPropertyChanged(); }
        }


        private string messageText;

        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; OnPropertyChanged(); }
        }
        static TcpListener listener = null;
        static BinaryWriter bw = null;
        static List<TcpClient> Clients { get; set; }

        public ChatUCViewModel()
        {
            SendCommand = new RelayCommand((obj) =>
            {
                var ip = IPAddress.Parse(IpAdress);
                var port = Port;

                var ep = new IPEndPoint(ip, port);
                listener = new TcpListener(ep);
                listener.Start(5);
                Clients = new List<TcpClient>();

                Task.Run(() =>
                {
                    while (true)
                    {
                        var client = listener.AcceptTcpClient();
                        Clients.Add(client);

                        Task.Run(() =>
                        { 
                            var writer = Task.Run(() =>
                            {
                                while (true)
                                {
                                    foreach (var item in Clients)
                                    {
                                        var stream = item.GetStream();
                                        bw = new BinaryWriter(stream);
                                        bw.Write(MessageText);
                                    }
                                }
                            });
                        });
                    }
                });
            });
        }

    }
}

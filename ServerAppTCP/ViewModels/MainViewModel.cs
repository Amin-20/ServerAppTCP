using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerAppTCP.Commands;
using ServerAppTCP.Views.UserControls;
using System.Windows;
using ServerAppTCP.Models;
using System.Collections.ObjectModel;
using ServerAppTCP.ViewModels.UCViewModels;

namespace ServerAppTCP.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand OpenServerCommand { get; set; }
        public RelayCommand SelectedUserCommand { get; set; }

        static TcpListener listener = null;
        static BinaryReader br = null;
        static BinaryWriter bw = null;
        private string serverStatus;

        public string ServerStatus
        {
            get { return serverStatus; }
            set { serverStatus = value; OnPropertyChanged(); }
        }

        private ObservableCollection<User> users;

        public ObservableCollection<User> Users
        {
            get { return users; }
            set { users = value; OnPropertyChanged(); }
        }
        
        private User selectedUser;

        public User SelectedUser
        {
            get { return selectedUser; }
            set { selectedUser = value;}
        }


        static List<TcpClient> Clients { get; set; }

        public MainViewModel()
        {
            Users = new ObservableCollection<User>();

            SelectedUserCommand = new RelayCommand((obj) =>
            {
                var chatUC = new ChatUC();
                var chatUCvm = new ChatUCViewModel();
                chatUCvm.IpAdress = selectedUser.IpAdress;
                chatUCvm.Port=selectedUser.Port;
                chatUC.DataContext = chatUCvm;
                App.myWrapPanel.Children.Add(chatUC);
            });

            OpenServerCommand = new RelayCommand((obj) =>
            {
                var ip = IPAddress.Parse("10.1.18.2");
                var port = 27001;

                var ep = new IPEndPoint(ip, port);
                listener = new TcpListener(ep);
                listener.Start(10);
                ServerStatus = "Server Up . . .";
                Clients = new List<TcpClient>();

                Task.Run(() =>
                {
                    while (true)
                    {
                        var client = listener.AcceptTcpClient();
                        Clients.Add(client);

                        Task.Run(() =>
                        {
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
                                                var user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(msg);
                                                App.Current.Dispatcher.Invoke((System.Action)delegate
                                                {
                                                    Users.Add(user);
                                                });
                                            }
                                            catch (Exception)
                                            {

                                                Clients.Remove(item);
                                            }
                                        }
                                    }).Wait(50);
                                }
                            });

                            //var writer = Task.Run(() =>
                            //{
                            //    while (true)
                            //    {
                            //        var msg = Console.ReadLine();
                            //        foreach (var item in Clients)
                            //        {
                            //            var stream = item.GetStream();
                            //            bw = new BinaryWriter(stream);
                            //            bw.Write(msg);
                            //        }
                            //    }
                            //});
                        });
                    }
                });
            });
        }
    }
}

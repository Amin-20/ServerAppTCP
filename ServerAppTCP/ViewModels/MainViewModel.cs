using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerAppTCP.Commands;
using System.Windows;
using ServerAppTCP.Models;
using System.Collections.ObjectModel;

namespace ServerAppTCP.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand OpenServerCommand { get; set; }
        public RelayCommand SelectedUserCommand { get; set; }
        public RelayCommand SendCommand { get; set; }


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
            set { selectedUser = value; }
        }

        private string messageText;

        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; OnPropertyChanged(); }
        }


        private bool isTextB = false;

        public bool IsTextB
        {
            get { return isTextB; }
            set { isTextB = value; OnPropertyChanged(); }
        }


        private bool isSendButton = false;

        public bool IsSendButton
        {
            get { return isSendButton; }
            set { isSendButton = value; OnPropertyChanged(); }
        }

        static List<TcpClient> Clients { get; set; }

        public MainViewModel()
        {
            var ip = IPAddress.Parse("10.1.18.2");
            var port = 27001;

            var ep = new IPEndPoint(ip, port);
            listener = new TcpListener(ep);
            Clients = new List<TcpClient>();

            Users = new ObservableCollection<User>();

            SelectedUserCommand = new RelayCommand((obj) =>
            {
                IsSendButton = true;
                IsTextB = true;
            });

            OpenServerCommand = new RelayCommand((obj) =>
            {
                listener.Start(10);
                ServerStatus = "Server Up . . .";
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
                        });
                    }
                });
            });

            SendCommand = new RelayCommand((obj) =>
            {
                //var user = new User();
                //foreach (var item in Users)
                //{
                //    if (item.Name == selectedUser.Name)
                //    {
                //        user = item;
                //    }
                //}

                //listener = new TcpListener(user.LocalAdress);
                //Clients = new List<TcpClient>();
                //listener.Start(10);

                Task.Run(() =>
                {
                    var writer = Task.Run(() =>
                    {
                        foreach (var item in Clients)
                        {
                            var stream = item.GetStream();
                            bw = new BinaryWriter(stream);
                            bw.Write(MessageText);
                            MessageText = String.Empty;
                        }
                    });
                });


            });


        }
    }
}

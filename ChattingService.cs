using ChattingInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ChattingServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode =
    InstanceContextMode.Single)]

    public class ChattingService : IChattingService
    {
        //This holds the users data such as username and stores it into this dictionary
        //A reference so other commands can refer to this for stored data.
        public ConcurrentDictionary<string, ConnectedClient> _connectedClients =
        new ConcurrentDictionary<string,ConnectedClient>();


        public int Login(string userName)
        {
            //is anyone else logged in with my name?
            foreach (var client in _connectedClients)
            {
                if (client.Key.ToLower() == userName.ToLower())
                {
                    //if yes
                    return 1;
                }

            }

            //This gets the callback
            var establishedUserConnection = OperationContext.Current.GetCallbackChannel<IClient>();

            ConnectedClient newclient = new ConnectedClient();
            newclient.connection = establishedUserConnection;
            newclient.UserName = userName;


            _connectedClients.TryAdd(userName, newclient);

            updateHelper(0, userName);


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Client login: {0} at {1}", newclient.UserName, System.DateTime.Now);
            Console.ResetColor();

            return 0;
        }


        //logout section...
        public void Logout()
        {
            ConnectedClient client = GetMyClient();
            if (client != null)
            {
                ConnectedClient removedClient;
                _connectedClients.TryRemove(client.UserName, out removedClient);

                updateHelper(1, removedClient.UserName);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Client logoff: {0} at {1}", removedClient.UserName, System.DateTime.Now);
                Console.ResetColor();
            }
        }

        //matching connection via the concurrent dictionary
        public ConnectedClient GetMyClient()
        {
            var establishedUserConnection = OperationContext.Current.GetCallbackChannel<IClient>();
            foreach (var client in _connectedClients)
            {
                if (client.Value.connection == establishedUserConnection)
                {
                    return client.Value;
                }
            }
            return null;

        }



        //new command...
        public void SendMessageToAll(string message, string userName)
        {
            foreach (var client in _connectedClients)
            {
                if (client.Key.ToLower() != userName.ToLower())
                {
                    client.Value.connection.GetMessage(message, userName);
                }

            }

        }
        private void updateHelper(int value, string UserName)
        {

            foreach (var client in _connectedClients)
            {
                if(client.Value.UserName.ToLower() != UserName.ToLower())
                {
                client.Value.connection.GetUpdate(value, UserName);
                }
              
            }

        }

        public List<string> GetCurrentUsers()
        {
            List<string> listOfUsers = new List<string>();
            foreach(var client in _connectedClients)
            {
                listOfUsers.Add(client.Value.UserName);

            }
            return listOfUsers; 
        }
    }
}   
            




    
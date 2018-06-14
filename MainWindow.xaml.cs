using ChattingInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChattingClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// This is the main class...
    public partial class MainWindow : Window
    {

        public static IChattingService Server;
        //Duplex channel allows more than one user to connect at one time
        private static DuplexChannelFactory<IChattingService> _channelFactory;
        public MainWindow()
        {
            InitializeComponent();
            _channelFactory = new DuplexChannelFactory<IChattingService>(new ClientCallback(),
            "ChattingServicesEndPoint");
            Server = _channelFactory.CreateChannel();

            //I have slpit the classes into sections so it is easier to read and understand.

        }
        public void TakeMessage(string message, string userName)


        {
            //Show username + message in the text box.
            //End user will also see message and username in this order.
            TextDisplayTextBox.Text += userName + ": " + message + "\n";
            TextDisplayTextBox.ScrollToEnd();
        }

        //New command...
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            //if message in message text box has no characters - dont allow send.
            if(MessageTextBox.Text.Length == 0)
            {
                return;
            }

            //This line of command tells the system to show username and message sent.
            Server.SendMessageToAll(MessageTextBox.Text, UserNameTextBox.Text);
            TakeMessage(MessageTextBox.Text, "You");
            MessageTextBox.Text = "";
        }


        //New command...
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //username must be entered in order for action LOGIN to be successful
            int returnValue = Server.Login(UserNameTextBox.Text);
            if (returnValue == 1)
            {
                //if I am logged in, do not let someone else login again...
                MessageBox.Show("You are already logged in. Log out first and try again");
            }

            //Otherwise connect the user successfully...
            else if (returnValue == 0)
            {
                //once username is entered, do not allow another client to login with same credentials.
                UserNameTextBox.IsEnabled = false;
                //disable login button.
                LoginButton.IsEnabled = false;
                //Once logged in, message view box will be set as non edit mode ***
                TextDisplayTextBox.IsEnabled = false;

                //load our users...
                LoadUserList(Server.GetCurrentUsers());
                
            }

        }

        //This line of code makes the text box public so that the messages can appear
        //The line of code below fixed the issue
        //This is now classed as  FIX.
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
     
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Server.Logout();
        }

        public void AddUserToList(string userName)
        {
            if (UsersListBox.Items.Contains(userName))
            {
                return;
            }

            UsersListBox.Items.Add(userName);
        }
        public void RemoveUserFromList(string userName)
        {
            if (UsersListBox.Items.Contains(userName))
            {
                UsersListBox.Items.Remove(userName);
            }
        }

        private void LoadUserList(List<string> users)
        {
            foreach(var user in users)
            {
                AddUserToList(user);   
            }
        }
    }
}
    

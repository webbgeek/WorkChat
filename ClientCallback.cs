using ChattingInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChattingClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ClientCallback : IClient
{

        public void GetMessage(string message, string userName)
        {
            //casting the main window so that we can add the takemessage function.
            //Main window holds the reference to the window that we need to link it to.
            //This is a basic form of casting.
            ((MainWindow)Application.Current.MainWindow).TakeMessage(message, userName);
        }

        public void GetUpdate(int value, string UserName)
        {
           switch (value)
            {
                case 0:
                    {
                        ((MainWindow)Application.Current.MainWindow).AddUserToList(UserName);
                        break;
                    }
                case 1:
                    {
                        ((MainWindow)Application.Current.MainWindow).RemoveUserFromList(UserName);
                        break;
                    }
            }
        }
    }
        }

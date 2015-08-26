using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleEvent
{
    class Program
    {
        static EventGenerator eventSender;
        static EventGenerator eventReveiver;
        static void Main(string[] args)
        {
            EventGenerator eventGenerator = new EventGenerator();
            eventSender = eventGenerator;
            eventSender.Completed += SendCompleted;

            eventReveiver = eventGenerator;
            eventReveiver.Completed += ReceiveCompleted;

            StringEventArgs stringEventArg = new StringEventArgs("john");

            eventReveiver.TriggerEvent(stringEventArg);

            eventSender.TriggerEvent(stringEventArg);
            Console.ReadLine();
        }

        static void ReceiveCompleted(object sender,StringEventArgs m)
        {
            Console.WriteLine(m.EventParameter + " Receive Completed");
        }

        static void SendCompleted(object sender, StringEventArgs m)
        {
            Console.WriteLine(m.EventParameter + " Send Completed");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleEvent
{
    public class EventGenerator
    {
        public event EventHandler<StringEventArgs> Completed;

        public void TriggerEvent(StringEventArgs m)
        {
            if (Completed != null)
            {
                Completed(this,m);
            }
        }
    }

    public class StringEventArgs:EventArgs
    {
        public StringEventArgs(string m):base()
        {
            eventArgs = m;
        }

        private string eventArgs;
        public string EventParameter
        {
            get {
                return eventArgs;
            }
        }
    }
}

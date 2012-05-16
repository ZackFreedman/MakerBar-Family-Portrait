using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Komodex.NETMF.MicroTweet;

namespace MakerBarFamilyPicture
{ 
    public class Program
    {
        static bool timeUpdated = false;
        static TwitterClient twitterClient = null;

        public static void Main()
        {
            SevenSegmentDisplaysMultiplexedWithADecadeCounter display = new SevenSegmentDisplaysMultiplexedWithADecadeCounter();
            
            display.SetUpPorts();
            display.Begin();

            //Thread clockThread = new Thread(UpdateTime);
            Debug.Print("Updating date/time...");
            //clockThread.Start();

            /*
            int countdown = 1000;

            while (clockThread.IsAlive)
            {
                display.SetDisplay(0, 88, false);
                display.Display();
                Thread.Sleep(200);
                display.SetDisplay(0, 00, true);
                display.Display();
                Thread.Sleep(200);
                if (countdown-- < 0)
                    clockThread.Abort();
            }
             */

            while (!timeUpdated)
            {
                UpdateTime();
            }

            if (timeUpdated)
            {
                Debug.Print("Setting up Twitter");

                twitterClient = new TwitterClient(
                    "Placeholder", // Consumer Key
                    "Placeholder", // Consumer Secret. Shhh! 
                    "Placeholder", // Access Token
                    "Placeholder"); // Access Token Secret. Shhh!
                twitterClient.DebugMessage += new DebugMessageEventHandler(tweetBot_DebugMessage);
            }

            Debug.Print("Current date/time: " + DateTime.Now.ToString());

            display.SetDisplay(0, 00, true);
            display.SetDisplay(1, 11, true);
            display.SetDisplay(2, 22, true);
            display.SetDisplay(3, 33, true);

            Thread.Sleep(1000);

            int a, b, c, d;
            twitterClient.GetTotals(out a, out b, out c, out d);

            while (true)
            {
                display.Display();
            }
        }

        static void UpdateTime()
        {
            timeUpdated = false;
            timeUpdated = NTP.UpdateTimeFromNTPServer("time-a.nist.gov");
        }

        static void tweetBot_DebugMessage(object sender, DebugMessageEventArgs e)
        {
            Debug.Print(e.Message);
        }
    }
}

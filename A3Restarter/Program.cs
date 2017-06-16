using System;
using System.Diagnostics;
using System.Threading;

namespace A3Restarter
{
    class Program
    {
        private static int secondsToRestart = 10860;    //3 hours and 1 minute to allow for server start time
        private static int timeLeftToRestart = 0;
        private static int displayInterval = 60000; //Update every 60 seconds

        private static string modsParameter = "";
        private static string servermodsParameter = "";

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Missing -mod and -servermod parameters.");
                Console.ReadLine();
                return;
            }

            modsParameter = args[0];
            servermodsParameter = args[1];

            timeLeftToRestart = secondsToRestart * 1000;

            LaunchServer();

            // Create a Timer object that knows to call our TimerCallback

            Timer t = new Timer(TimerCallback, null, 0, displayInterval);
            // Wait for the user to hit <Enter>
            Console.ReadLine();
        }

        private static void TimerCallback(Object o)
        {
            if (timeLeftToRestart <= 0)
            {
                Console.WriteLine("Prepare to die Server!");

                //kill process and relaunch it
                foreach (var process in Process.GetProcessesByName("arma3server"))
                {
                    process.Kill();
                }

                timeLeftToRestart = secondsToRestart * 1000;

                LaunchServer();

            } else
            {
                TimeSpan t = TimeSpan.FromMilliseconds(timeLeftToRestart);

                string timeLeft = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                t.Hours,
                                t.Minutes,
                                t.Seconds);

                Console.WriteLine("Time till restart: " + timeLeft);

                timeLeftToRestart = timeLeftToRestart - displayInterval;
            }
            
            // Force a garbage collection to occur for this demo.
            GC.Collect();
        }

        private static void LaunchServer()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = @"C:\Games\Arma\Server\arma3server.exe";
            //-mod=@Exile;@Bornholm;@mas;@NATO_Rus_Vehicle;@CUP_Units;@CUP_Vehicles;@CUP_Weapons;@CBA_A3;
            startInfo.Arguments = servermodsParameter + @" " + modsParameter + @" -config=C:\Games\Arma\Server\@ExileServer\config.cfg -port=2302 -profiles=SC -cfg=C:\Games\Arma\Server\@ExileServer\basic.cfg -name=SC -autoinit";
            process.StartInfo = startInfo;
            process.Start();

            Console.WriteLine("Launching server...");
        }
    }
}

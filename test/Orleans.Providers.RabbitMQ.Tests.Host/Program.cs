using System;
using System.Threading.Tasks;

namespace Orleans.Providers.RabbitMQ.Tests.Host
{
    class Program
    {
        public static int Main(string[] args)
        {
            TestSilo.StartSilo().Wait();
            WaitForKey();
            return TestSilo.StopSilo().Result;
        }

        private static void WaitForKey()
        {
            bool exit = false;

            Task.Factory.StartNew(() =>
            {
                Console.ReadKey();
                exit = true;
            });

            while (!exit)
            {
                Task.Delay(50).Wait();
            }
        }
    }
}
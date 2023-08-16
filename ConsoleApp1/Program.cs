using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        int a = 0;
        UdpCommunicator communicator = new UdpCommunicator(8000); // nasłuchuje na porcie 8000

        // Rozpocznij asynchroniczne nasłuchiwanie w tle
        communicator.TaskReceive();
        // ... jakikolwiek inny kod ...
        //communicator.StopListening();

        Console.WriteLine("Nasłuchiwanie w tle rozpoczęte...");

        while (true)
        {
            // Nadawanie wiadomości co 10 sekund
            communicator.Send("Hello!", "127.0.0.1", 8000);
            Console.WriteLine("Wysłano wiadomość.");
            Thread.Sleep(10000); // Czekaj 10 sekund
            a++;
            if(a == 2)
            {
                communicator.StopListening();
            }

        }
    }
}

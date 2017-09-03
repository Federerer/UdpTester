using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace UdpTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            var program = Task.Run(async () =>
            {
                var discovery = new TPUdpClient("192.168.1.200", 50000);
                discovery.Connect();

                var data = new byte[1400];
                ushort id = 0;

                var sw = new Stopwatch();

                Console.CursorVisible = false;

                while (true)
                {
                    try
                    {
                        var idBytes = BitConverter.GetBytes(id);
                        Array.Copy(idBytes, data, idBytes.Length);

                        sw.Restart();
                        var response = await discovery.SendRequest(data);
                        sw.Stop();

                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine($"Time - {sw.ElapsedMilliseconds.ToString().PadLeft(3)} ms, {response}");

                        ++id;
                        //await Task.Delay(250);
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

            }, tokenSource.Token);

            Console.ReadLine();
            tokenSource.Cancel();

            program.Wait();
            
        }
    }
}

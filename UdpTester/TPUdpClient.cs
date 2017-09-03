using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace UdpTester
{
    public class TPUdpClient
    {
        private UdpClient _client;
        private IPEndPoint _remoteEndPoint;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        private Request _pendingRequest;

        public TPUdpClient(string address, ushort port)
        {
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
            _client = new UdpClient(port);
        }

        public void Connect()
        {
            _client.Connect(_remoteEndPoint);

            var reciverTask = Task.Run(async () =>
            {
                while (true)
                {
                    var result = await _client.ReceiveAsync();
                    _pendingRequest.Response = Response.Deserialize(result.Buffer);

                    if (_pendingRequest.Response.Id == _pendingRequest.Id)
                    {
                        _semaphore.Release();
                    }
                }
            });

        }

        public async Task<Response> SendRequest(byte[] request, TimeSpan? timeout = null)
        {

            int milis = (int?)timeout?.TotalMilliseconds ?? 1000;

            _pendingRequest.Id = BitConverter.ToUInt16(request, 0);

            await _client.SendAsync(request, request.Length);

            if (_semaphore.Wait(milis))
            {
                return _pendingRequest.Response;
            }
            else
            {
                throw new TimeoutException("Request timeout");
            }

        }

        struct Request
        {
            public Response Response { get; set; }
            public ushort Id { get; set; }
        }
    }

}

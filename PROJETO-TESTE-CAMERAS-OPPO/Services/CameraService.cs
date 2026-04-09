using PROJETO_TESTE_CAMERAS_OPPO.Variaveis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PROJETO_TESTE_CAMERAS_OPPO.Services
{
    public class CameraService
    {
        public TcpListener _listener;
        public CancellationTokenSource _cts;
        public event Action OnError;

        public async Task IniciarTcpServer(int dataPort)
        {
            _listener = new TcpListener(System.Net.IPAddress.Any, dataPort);
            _listener.Start();
            _cts = new CancellationTokenSource();
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        var client = await _listener.AcceptTcpClientAsync();
                        HandleClientAsync(client);
                    }                    
                }
                catch (ObjectDisposedException)
                {
                    // O listener foi parado, sair do loop
                    break;
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                        var vetor = receivedData.Split(',');

                       
                        foreach (var item in vetor)
                        {
                            var valor = item.Trim();
                            if (valor == "") continue;

                            if (valor.Contains("ERROR"))
                                OnError?.Invoke();
                            else
                                VarGlobal.LeiturasTCP.Add(valor);
                        }                       

                        return;
                    }
                }
            }
        }
    }
}

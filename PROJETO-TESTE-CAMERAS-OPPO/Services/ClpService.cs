using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyModbus;

namespace PROJETO_TESTE_CAMERAS_OPPO.Services
{
    public class ClpService
    {
        public ModbusClient _CLP;
        private string _ipAddress;
        private int _port;

        public bool IsConnected => _CLP != null && _CLP.Connected;

        public void Conectar(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;

            _CLP = new ModbusClient(_ipAddress, _port);
            _CLP.UnitIdentifier = 1;
            _CLP.Connect();
        }

        public bool TentarReconectar()
        {
            try
            {
                Desconectar();
                Conectar(_ipAddress, _port);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int[] LerRegistradores(int enderecoInicial, int quantidade)
        {
            if (!IsConnected) return null;

            try
            {
                return _CLP.ReadHoldingRegisters(enderecoInicial, quantidade);
            }
            catch
            {
                return null;
            }
        }

        public void EscreverRegistro(int endereco, int valor)
        {
            if (IsConnected)
            {
                _CLP.WriteSingleRegister(endereco, valor);
            }
        }

        public void Desconectar()
        {
            if (IsConnected)
            {
                _CLP.Disconnect();
            }
        }

    }
}

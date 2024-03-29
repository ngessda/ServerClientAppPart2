﻿using System.Net.Sockets;
using System.Text;

namespace NetCommunication
{
    public class NetIO
    {
        private Socket _socket;
        private IParser _parser;
        private object _lock = new object();
        private bool _stopped = false;
        private bool Stopped
        {
            get
            {
                lock (_lock)
                {
                    return _stopped;
                }
            }
            set
            {
                lock (_lock)
                {
                    _stopped = value;
                }
            }
        }

        public void Stop()
        {
            Stopped = true;
            _socket.Close();
        }

        public void Communicate()
        {
            while (!Stopped)
            {
                try
                {
                    string data = Receive();
                    _parser.Parse(data);
                }
                catch
                {
                    Stopped = true;
                }
            }
        }

        public NetIO(Socket socket, IParser parser)
        {
            _socket = socket;
            _parser = parser;
        }

        public void Send(string data)
        {
            if (data.Trim() == string.Empty)
            {
                return;
            }

            try
            {
                byte[] replyBuffer = Encoding.UTF8.GetBytes(data);
                _socket.Send(replyBuffer);
            }
            catch(Exception ex)
            {
                Console.WriteLine("[Error]: не удалось отправить сообщение");
            }
        }

        private string Receive()
        {
            var buffer = new byte[1024];
            var count = _socket.Receive(buffer);
            if (count == 0)
            {
                return string.Empty;
            }
            var result = Encoding.UTF8.GetString(buffer, 0, count);
            return result;
        }
    }
}

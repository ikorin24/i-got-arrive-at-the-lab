using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IGotArrivedAtTheLab
{
    public delegate void MessageRecevedEventHandler(string message);

    public class Comm : IDisposable
    {
        private string _portname;
        private byte[] _buf = new byte[256];
        private StringBuilder _receveBuf = new StringBuilder();
        private SerialPort _serialPort;
        Task _task;
        CancellationTokenSource _tokenSource;
        public event MessageRecevedEventHandler MessageReceved;

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();
            var canceled = new Func<bool>(() => _tokenSource?.Token.IsCancellationRequested ?? true);
            _task = Task.Factory.StartNew(() => 
            {
                while(true) {
                    StartPrivate(canceled);
                }
            }, _tokenSource.Token);
        }

        public void Dispose()
        {
            _tokenSource?.Cancel();
            _tokenSource = null;
            try {
                _task.Wait();
            }
            catch(Exception) {
                Debug.WriteLine("Task Canceled");
                // nop
            }
        }

        public void SendMessage(string msg)
        {
            _serialPort?.Write($"{Config.Instance.MsgHeader}{msg}");
            Thread.Sleep(100);
        }

        private void StartPrivate(Func<bool> canceled)
        {
            _portname = GetPortName(canceled);
            _serialPort = new SerialPort();
            try {
                _serialPort.BaudRate = Config.Instance.Baudrate;
                _serialPort.PortName = _portname;
                _serialPort.WriteTimeout = 1000;
                _serialPort.ReadTimeout = 1000;
                _serialPort.Open();
                _serialPort.Write($"{Config.Instance.MsgHeader}{Config.Instance.ConnectedMsg}");
                Polling(_serialPort, canceled);
            }
            finally {
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        private void Polling(SerialPort serialPort, Func<bool> canceled)
        {
            while(true) {
                if(canceled()) {
                    throw new TaskCanceledException();
                }
                try {
                    if(serialPort.IsOpen == false) { return; }
                    var readlen = serialPort.Read(_buf, 0, _buf.Length);
                    if(readlen > 0) {
                        var receved = Encoding.ASCII.GetString(_buf.Take(readlen).ToArray());
                        _receveBuf.Append(receved);
                        var str = _receveBuf.ToString();
                        if(str == Config.Instance.ArrivingMsg || str == Config.Instance.TestArrivingMsg || str == Config.Instance.Ping) {
                            MessageReceved?.Invoke(str);
                            _receveBuf.Clear();
                        }
                    }
                }
                catch(Exception) {
                }
                Thread.Sleep(20);
            }
        }

        private string GetPortName(Func<bool> canceled)
        {
            string connectedPortname = null;
            while(true) {
                if(canceled()) { throw new TaskCanceledException(); }
                var tasks = SerialPort.GetPortNames().Select(portname =>
                {
                    return Task.Factory.StartNew(() => 
                    {
                        using(var serialPort = new SerialPort()) {
                            serialPort.BaudRate = Config.Instance.Baudrate;
                            serialPort.PortName = portname;
                            serialPort.ReadTimeout = 2000;
                            serialPort.WriteTimeout = 2000;
                            serialPort.Open();
                            serialPort.Write($"{Config.Instance.MsgHeader}{Config.Instance.Ping}");
                            Thread.Sleep(100);
                            try {
                                var readlen = serialPort.Read(_buf, 0, _buf.Length);
                                if(readlen > 0) {
                                    var receved = Encoding.ASCII.GetString(_buf.Take(readlen).ToArray());
                                    if(receved == $"{Config.Instance.MsgHeader}{Config.Instance.Ping}") {
                                        connectedPortname = portname;
                                    }
                                }
                            }
                            catch(Exception) {
                            }
                        }
                    });
                }).ToArray();
                try {
                    Task.WaitAll(tasks);
                }
                catch(Exception) {
                }
                if(connectedPortname != null) { return connectedPortname; }
                Thread.Sleep(20);
            }
        }
    }
}

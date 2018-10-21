using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QApp.Utility {
    /// <summary>
    /// The goal of this client is to create persistent TCP connections
    /// with no worries over the connection locking up or not getting disposed
    /// of.
    /// </summary>
    public class BetterTcpClient : IDisposable {

        private object lockEverything = new object();
        private string host;
        private int port;
        private TimeSpan lockedUpTimeout;
        private bool disposed;

        public readonly TcpClient InternalClient;
        private NetworkStream stream;

        public BetterTcpClient(string host, int port, TimeSpan throwTimeoutExceptionAfter) {
            lock (lockEverything) {
                this.host = host;
                this.port = port;
                this.lockedUpTimeout = throwTimeoutExceptionAfter;
                InternalClient = new TcpClient();
                InternalClient.ReceiveTimeout = 100;
                InternalClient.SendTimeout = (int)throwTimeoutExceptionAfter.TotalMilliseconds;
            }
        }

        public async Task ConnectAsync(CancellationToken? cToken = null) {
            TimeSpan timeout;
            lock (lockEverything) {
                if (this.disposed) {
                    throw new ObjectDisposedException("Client disposed.");
                }
                if (this.stream != null) {
                    throw new InvalidOperationException("Client has not been connected, or was disposed.");
                }
                timeout = this.lockedUpTimeout;
            }
            await InternalClient.ConnectAsync(host, port).WaitAsync(timeout, cToken);
            lock (lockEverything) {
                this.stream = InternalClient.GetStream();
            }
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken? cToken = null) {
            NetworkStream s;
            TimeSpan timeout;
            lock (lockEverything) {
                if (disposed) {
                    throw new ObjectDisposedException("Client disposed.");
                }
                s = this.stream;
                timeout = this.lockedUpTimeout;
            }
            if (s == null) {
                throw new InvalidOperationException("Client has not been connected.");
            }
            if (!s.DataAvailable) {
                return 0;
            }
            try {
                return await s.ReadAsync(buffer, offset, count).WaitAsync(timeout, cToken);
            }
            catch (Exception) when (disposeFilter()) {
                return -1; //This never runs;
            }
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken? cToken = null) {
            NetworkStream s;
            TimeSpan timeout;
            lock (lockEverything) {
                if (disposed) {
                    throw new ObjectDisposedException("Client disposed.");
                }
                s = this.stream;
                timeout = this.lockedUpTimeout;
            }
            if (s == null) {
                throw new InvalidOperationException("Client has not been connected.");
            }
            try {
                await s.WriteAsync(buffer, offset, count).WaitAsync(timeout, cToken);
            }
            catch (Exception) when (disposeFilter()) {
                //This never runs;
            }
        }

        private bool disposeFilter() {
            this.Dispose();
            return false;
        }

        public void Dispose() {
            lock (lockEverything) {
                if (disposed) {
                    return;
                }
                disposed = true;
                try {
                    if (this.stream != null) {
                        this.stream.Dispose();
                    }
                }
                catch { }
                try {
                    this.InternalClient.Dispose();
                }
                catch { }
            }
        }
    }
}

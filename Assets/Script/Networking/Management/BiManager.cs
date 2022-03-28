using System;
using System.IO;
using System.Text;
using Network;
using UnityEngine;

namespace Script.Networking.Management
{
    public class BiManager
    {
        protected readonly NetworkConfiguration _networkConfiguration;
        protected readonly Action _onOtherSideConnect;
        private ConnectionState _connectionState;

        protected Connection Other;

        protected BiManager(NetworkConfiguration networkConfiguration, Action onOtherSideConnect)
        {
            _networkConfiguration = networkConfiguration;
            _onOtherSideConnect = onOtherSideConnect;
        }

        public ConnectionState ConnectionState
        {
            get => _connectionState;
            protected set
            {
                Debug.Log($"Connection state changed to: {value}");
                _connectionState = value;
            }
        }

        protected Stream GetDebugLogStream()
        {
            return new DebugLogStream();
        }

        private class DebugLogStream : Stream
        {
            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => 0;

            public override long Position
            {
                get => 0;
                set => throw new InvalidOperationException();
            }

            public override void Flush()
            {
                //empty
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                // empty
                return 0;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                //empty
                return 0;
            }

            public override void SetLength(long value)
            {
                //empty
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                Debug.Log(Encoding.UTF8.GetString(buffer));
            }
        }
    }
}
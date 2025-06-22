using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GIKCore.Net
{
    public class ICommonSocket : IAbstractSocket
    {
        private Socket mSocket;

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent _connectDone = new ManualResetEvent(false);
        private ManualResetEvent _sendDone = new ManualResetEvent(false);

        private SocketAsyncEventArgs _socketEventArg;
        private SocketAsyncEventArgs _socketEventArgRead;
        private SocketAsyncEventArgs _socketEventArgSend;

        public ICommonSocket(ISocket handler)
        {
            socketHandler = handler;
        }

        public override void Close()
        {
            if (mSocket != null)
            {
                if (_socketEventArg != null)
                    _socketEventArg.Dispose();

                if (_socketEventArgRead != null)
                    _socketEventArgRead.Dispose();

                if (_socketEventArgSend != null)
                    _socketEventArgSend.Dispose();
                mSocket.Close();
            }

            mSocket = null;
            _byteLeftPrev = null;

            if (socketHandler != null)
                socketHandler.isConnected = false;
        }

        public override void Connect(string host, int port)
        {
            try
            {
                IPAddress ipAddress;
                if (Uri.CheckHostName(host) == UriHostNameType.IPv4)
                {
                    ipAddress = IPAddress.Parse(host);
                }
                else
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(host);
                    ipAddress = hostEntry.AddressList[0];
                }

                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                _connectDone.Reset();

                // Create a stream-based, TCP socket using the InterNetwork Address Family.
                mSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Create a SocketAsyncEventArgs object to be used in the connection request
                _socketEventArg = new SocketAsyncEventArgs();
                _socketEventArg.RemoteEndPoint = endPoint;

                _socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
                {
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Connect:
                            {
                                if (e.SocketError == SocketError.Success)
                                {
                                // Create SocketAsyncEventArgs context object
                                _socketEventArgSend = new SocketAsyncEventArgs();

                                // Set properties on context object
                                _socketEventArgSend.RemoteEndPoint = mSocket.RemoteEndPoint;
                                    _socketEventArgSend.UserToken = null;

                                // Inline event handler for the Completed event.
                                // Note: This event handler was implemented inline in order 
                                // to make this method self-contained.
                                _socketEventArgSend.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
                                    {
                                    // Unblock the UI thread
                                    _sendDone.Set();
                                    });

                                    _socketEventArgRead = new SocketAsyncEventArgs();
                                    _socketEventArgRead.SetBuffer(new Byte[ISocket.MAX_BUFFER_SIZE], 0, ISocket.MAX_BUFFER_SIZE);
                                    _socketEventArgRead.Completed += new EventHandler<SocketAsyncEventArgs>(SocketDataReceivedCallback);
                                    mSocket.ReceiveAsync(_socketEventArgRead);

                                    if (ISocket.allowLog) Debug.Log("------- Socket connect success");
                                    socketHandler.isConnected = true;
                                    socketHandler.ResetCount();
                                    socketHandler.SocketConnectSuccess();
                                }
                                else
                                {
                                    Debug.Log("------- Reconnect but failed!");
                                    socketHandler.isConnected = false;
                                    socketHandler.SocketError();
                                }

                                _connectDone.Set();

                                break;
                            }
                        case SocketAsyncOperation.Receive:
                            break;
                        case SocketAsyncOperation.Send:
                            break;
                        default:
                            break;
                    }
                });

                mSocket.ConnectAsync(_socketEventArg);
                _connectDone.WaitOne(ISocket.TIMEOUT_MILLISECONDS);
            }
            catch (Exception e)
            {
                Debug.Log("------- ERROR SOCKET ::: " + e.Message);
                Close();
                socketHandler.SocketError();
            }
        }

        public override void Send(byte[] byteData)
        {
            // Create SocketAsyncEventArgs context object

            // Add the data to be sent into the buffer
            byte[] payload = byteData;

            _socketEventArgSend.SetBuffer(payload, 0, payload.Length);

            // Sets the state of the event to nonsignaled, causing threads to block
            _sendDone.Reset();

            // Make an asynchronous Send request over the socket
            mSocket.SendAsync(_socketEventArgSend);

            // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
            // If no response comes back within this time then proceed
            _sendDone.WaitOne(ISocket.TIMEOUT_MILLISECONDS);
        }

        protected override void ReceiveData(byte[] buffer, int offset, int dummy, int totalTransfered)
        {
            int countRead = 0, currentRead = 0;
            while (countRead < totalTransfered && socketHandler.isConnected)
            {
                if ((totalTransfered - countRead) < ISocket.REQUIRE_BYTE_READ_MIN)
                { //so byte con lai ko du 10 cho header de xu ly tiep
                    _byteLeftPrev = new byte[totalTransfered - countRead];
                    Buffer.BlockCopy(buffer, offset, _byteLeftPrev, 0, totalTransfered - countRead);
                    return;
                }

                currentRead = ProcessReceiveData(buffer, countRead, 0, totalTransfered);

                if (currentRead < 0)//co loi hoac noi' package
                {
                    return;
                }
                else
                {
                    countRead += currentRead;
                }

            }
        }

        protected override int ProcessReceiveData(byte[] buffer, int offset, int dummy, int totalTransfered)
        {
            try
            {
                int countRead = 0;

                if (buffer.Length < ISocket.REQUIRE_BYTE_READ_MIN)
                {
                    Debug.Log("------- Not enough header byte ::: " + buffer.Length);
                    _byteLeftPrev = buffer;
                    return -1;
                }

                //MAGIC
                byte[] bMagic = new byte[2];
                Buffer.BlockCopy(buffer, offset, bMagic, 0, 2);
                offset += 2;

                string strMagic = Encoding.UTF8.GetString(bMagic, 0, 2);

                byte[] bService = new byte[4];
                Buffer.BlockCopy(buffer, offset, bService, 0, 4);
                offset += 4;

                byte[] bLength = new byte[4];
                Buffer.BlockCopy(buffer, offset, bLength, 0, 4);
                offset += 4;

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bService);
                    Array.Reverse(bLength);
                }

                int id = BitConverter.ToInt32(bService, 0);
                int length = BitConverter.ToInt32(bLength, 0);

                if (ISocket.allowLog)
                {
                    Debug.Log("------- RECEIVED PACKAGE ::: " + id);
                    //LogWriterHandle.WriteLog("------- RECEIVED PACKAGE ::: " + id);
                }

                if (!ISocket.MAGICS.Contains(strMagic))
                {
                    Debug.Log("------- Magic fail ::: " + strMagic);
                    return -1;
                }

                if (length > 1000000)
                {
                    Debug.Log("------- Length too long ::: " + length);
                    return -1;
                }

                //check enough
                int numByteLeft = totalTransfered - offset;
                if (numByteLeft < length)
                {
                    Debug.Log("------- Not receive enough cardData ::: service: " + id + " ::: numByteLeft: " + numByteLeft + " ::: length: " + length);

                    //ghep vao dau cua packet sau
                    _byteLeftPrev = new byte[numByteLeft + ISocket.REQUIRE_BYTE_READ_MIN];
                    Buffer.BlockCopy(buffer, offset - ISocket.REQUIRE_BYTE_READ_MIN, _byteLeftPrev, 0, numByteLeft + ISocket.REQUIRE_BYTE_READ_MIN);

                    return -1;
                }

                byte[] data = new byte[length];
                Buffer.BlockCopy(buffer, offset, data, 0, length);
                countRead += ISocket.REQUIRE_BYTE_READ_MIN + length;

                socketHandler.ReceivePackage(id, data);

                return countRead;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                socketHandler.ReceivePackageFail();
                return -1;
            }
        }

        private void SocketDataReceivedCallback(object obj, SocketAsyncEventArgs async)
        {
            try
            {
                if (async.SocketError == SocketError.Success)
                {
                    if (async.BytesTransferred > 0)
                    {
                        if (_byteLeftPrev == null || _byteLeftPrev.Length == 0)
                        {
                            ReceiveData(async.Buffer, async.Offset, 0, async.BytesTransferred);
                        }
                        else
                        {
                            int totalByte = async.BytesTransferred + _byteLeftPrev.Length;
                            byte[] tmp = new byte[totalByte];
                            Buffer.BlockCopy(_byteLeftPrev, 0, tmp, 0, _byteLeftPrev.Length);
                            Buffer.BlockCopy(async.Buffer, async.Offset, tmp, _byteLeftPrev.Length, async.BytesTransferred);
                            _byteLeftPrev = null;
                            ReceiveData(tmp, 0, 0, totalByte);
                        }
                    }
                    else
                    {
                        if (socketHandler.isConnected)
                        {
                            Debug.Log("------- byte transferred less than 0 ::: " + async.BytesTransferred);
                            Close();
                            socketHandler.SocketDisconectRead();
                        }
                    }

                    mSocket.ReceiveAsync(_socketEventArgRead);
                }
                else
                {
                    Debug.Log("-------ERROR SOCKET ::: " + async.SocketError);
                    Close();
                    socketHandler.SocketError();
                }
            }
            catch (Exception e)
            {
                Debug.Log("----- ERROR SOCKET ::: " + e.Message);
                Close();
                socketHandler.SocketError();
            }
        }
    }
}

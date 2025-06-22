namespace GIKCore.Net
{
    public abstract class IAbstractSocket
    {
        public abstract void Connect(string host, int port);
        public abstract void Close();
        public abstract void Send(byte[] byteData);

        protected abstract void ReceiveData(byte[] buffer, int offset, int dummy, int totalTransfered);
        protected abstract int ProcessReceiveData(byte[] buffer, int offset, int dummy, int totalTransfered);

        protected ISocket socketHandler;
        protected byte[] _byteLeftPrev;
    }
}

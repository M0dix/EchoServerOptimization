namespace EchoServerOptimization
{

    
    public class BufferPool
    {
        private readonly Stack<byte[]> _pool = new Stack<byte[]>();
        private readonly int _bufferSize;

        public BufferPool(int bufferSize, int initialCapacity)
        {
            _bufferSize = bufferSize;
            for(int i = 0; i < initialCapacity; i++)
            {
                _pool.Push(new byte[_bufferSize]);
            }
        }

        public byte[] Get()
        {
            lock(_pool)
            {
                return _pool.Count > 0 ? _pool.Pop() : new byte[_bufferSize];
            }
        }
    
        public void Return(byte[] buffer) 
        { 
            lock( _pool)
            {
                _pool.Push(buffer);
            }
        }
    }
}

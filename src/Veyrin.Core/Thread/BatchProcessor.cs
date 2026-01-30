using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Thread
{
    public class BatchProcessor<T>
    {
        private readonly List<T> _buffer = new();
        private readonly int _batchSize;
        private readonly Func<List<T>, Task> _processor;
        private readonly AsyncLock _lock = new();

        public BatchProcessor(int batchSize, Func<List<T>, Task> processor)
        {
            _batchSize = batchSize;
            _processor = processor;
        }

        public async Task AddAsync(T item)
        {
            List<T>? toProcess = null;
            using (await _lock.LockAsync())
            {
                _buffer.Add(item);
                if (_buffer.Count >= _batchSize)
                {
                    toProcess = new List<T>(_buffer);
                    _buffer.Clear();
                }
            }

            if (toProcess != null) await _processor(toProcess);
        }
    }
}

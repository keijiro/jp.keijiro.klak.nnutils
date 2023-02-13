using UnityEngine;

// GPU to CPU readback helpers

namespace Klak.NNUtils {

public class BufferReader<T> where T : struct
{
    public BufferReader(GraphicsBuffer source, int length)
      => (_source, _cache) = (source, new T[length]);

    public System.ReadOnlySpan<T> Cached => Read();

    public System.ReadOnlySpan<T> Read()
    {
        if (_isCached) return _cache;
        _source.GetData(_cache, 0, 0, _cache.Length);
        _isCached = true;
        return _cache;
    }

    public void InvalidateCache() => _isCached = false;

    GraphicsBuffer _source;
    T[] _cache;
    bool _isCached;
}

public class CountedBufferReader<T> where T : struct
{
    public CountedBufferReader(GraphicsBuffer array, GraphicsBuffer count, int max)
      => (_source, _cache) = ((array, count), (new T[max], new int[1]));

    public System.ReadOnlySpan<T> Cached => Read();

    public System.ReadOnlySpan<T> Read()
    {
        if (!_isCached)
        {
            _source.count.GetData(_cache.count, 0, 0, 1);
            _source.array.GetData(_cache.array, 0, 0, _cache.count[0]);
            _isCached = true;
        }
        return new System.ReadOnlySpan<T>(_cache.array, 0, _cache.count[0]);
    }

    public void InvalidateCache() => _isCached = false;

    (GraphicsBuffer array, GraphicsBuffer count) _source;
    (T[] array, int[] count) _cache;
    bool _isCached;

}

} // namespace Klak.NNUtils

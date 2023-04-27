using Unity.Barracuda;
using UnityEngine;

namespace Klak.NNUtils {

public static class RTUtil
{
    public static RenderTexture NewArgbUav(int w, int h)
    {
        var rt = new RenderTexture(w, h, 0, RenderTextureFormat.Default,
                                            RenderTextureReadWrite.Linear);
        rt.enableRandomWrite = true;
        rt.Create();
        return rt;
    }

    public static RenderTexture NewSingleChannelUAV(int w, int h)
    {
        var rt = new RenderTexture(w, h, 0, RenderTextureFormat.R8,
                                            RenderTextureReadWrite.Linear);
        rt.enableRandomWrite = true;
        rt.Create();
        return rt;
    }

    public static void Destroy(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying)
            Object.Destroy(o);
        else
            Object.DestroyImmediate(o);
    }
}

public static class BufferUtil
{
    public unsafe static GraphicsBuffer NewStructured<T>(int length) where T : unmanaged
      => new GraphicsBuffer(GraphicsBuffer.Target.Structured, length, sizeof(T));

    public unsafe static GraphicsBuffer NewAppend<T>(int length) where T : unmanaged
      => new GraphicsBuffer(GraphicsBuffer.Target.Append, length, sizeof(T));

    public unsafe static GraphicsBuffer NewRaw(int length)
      => new GraphicsBuffer(GraphicsBuffer.Target.Raw, length, sizeof(uint));

    public unsafe static GraphicsBuffer NewDrawArgs()
      => new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 4, sizeof(uint));

    public static (Tensor, ComputeTensorData) NewTensor(TensorShape shape, string name)
    {
#if BARRACUDA_4_0_0_OR_LATER
        var data = new ComputeTensorData(shape, name, false);
        var tensor = TensorFloat.Zeros(shape);
        tensor.AttachToDevice(data);
#else
        var data = new ComputeTensorData(shape, name, ComputeInfo.ChannelsOrder.NHWC, false);
        var tensor = new Tensor(shape, data);
#endif
        return (tensor, data);
    }
}

} // namespace Klak.NNUtils

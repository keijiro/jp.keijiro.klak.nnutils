using Unity.Sentis;
using UnityEngine;

// Extension methods

namespace Klak.NNUtils.Extensions {

public static class ComputeShaderExtensions
{
    public static void DispatchThreads
      (this ComputeShader compute, int kernel, int x, int y, int z)
    {
        uint xc, yc, zc;
        compute.GetKernelThreadGroupSizes(kernel, out xc, out yc, out zc);
        x = (x + (int)xc - 1) / (int)xc;
        y = (y + (int)yc - 1) / (int)yc;
        z = (z + (int)zc - 1) / (int)zc;
        compute.Dispatch(kernel, x, y, z);
    }
}

public static class TensorShapeExtensions
{
    public static int GetWidth(in this TensorShape shape)
      => shape[2];

    public static int GetHeight(in this TensorShape shape)
      => shape[1];
}

public static class ModelInputExtensions
{
    public static TensorShape GetTensorShape(in this Model.Input input)
      => input.shape.ToTensorShape();
}

public static class IWorkerExtensions
{
    public static ComputeBuffer PeekOutputBuffer(this Worker worker)
      => ((ComputeTensorData)worker.PeekOutput().dataOnBackend).buffer;

    public static ComputeBuffer PeekOutputBuffer(this Worker worker, string tensorName)
      => ((ComputeTensorData)worker.PeekOutput(tensorName).dataOnBackend).buffer;
}

} // namespace Klak.NNUtils.Extensions

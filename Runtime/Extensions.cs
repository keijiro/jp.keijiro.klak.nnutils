using Unity.Barracuda;
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
#if BARRACUDA_4_0_0_OR_LATER
      => shape[1];
#else
      => shape.width;
#endif
}

public static class ModelInputExtensions
{
    public static TensorShape GetTensorShape(in this Model.Input input)
#if BARRACUDA_4_0_0_OR_LATER
      => input.shape.ToTensorShape();
#else
      => new TensorShape(input.shape);
#endif
}

public static class IWorkerExtensions
{
    public static ComputeBuffer PeekOutputBuffer(this IWorker worker)
      => ((ComputeTensorData)worker.PeekOutput().tensorOnDevice).buffer;

    public static ComputeBuffer PeekOutputBuffer(this IWorker worker, string tensorName)
      => ((ComputeTensorData)worker.PeekOutput(tensorName).tensorOnDevice).buffer;
}

} // namespace Klak.NNUtils.Extensions

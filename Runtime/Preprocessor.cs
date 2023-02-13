using Unity.Barracuda;
using UnityEngine;
using Klak.NNUtils.Extensions;

// Common image preprocessor for NN models

namespace Klak.NNUtils {

public class ImagePreprocess : System.IDisposable
{
    int _width, _height;
    bool _nchw;
    Tensor _tensor;
    ComputeTensorData _tensorData;

    public Vector4 ColorCoeffs { get; set; } = new Vector4(-1, -1, -1, 2);

    public Tensor Tensor => _tensor;

    public ImagePreprocess(int width, int height, bool nchwFix = false)
    {
        _width = width;
        _height = height;
#if BARRACUDA_4_0_0_OR_LATER
        _nchw = nchwFix;
#endif
        var shape = _nchw ? new TensorShape(1, 3, _height, _width) :
                            new TensorShape(1, _height, _width, 3);
        (_tensor, _tensorData) = BufferUtil.NewTensor(shape, "preprocess");
    }

    public void Dispose()
    {
        _tensor?.Dispose();
        _tensor = null;
        _tensorData = null;
    }

    public void Dispatch(Texture source, ComputeShader compute, int pass = 0)
    {
        compute.SetTexture(pass, "Input", source);
        compute.SetBuffer(pass, "Output", _tensorData.buffer);
        compute.SetInts("InputSize", _width, _height);
        compute.SetVector("ColorCoeffs", ColorCoeffs);
        compute.SetBool("InputIsLinear", QualitySettings.activeColorSpace == ColorSpace.Linear);
        compute.SetBool("OutputIsNCHW", _nchw);
        compute.DispatchThreads(pass, _width, _height, 1);
    }
}

} // namespace Klak.NNUtils

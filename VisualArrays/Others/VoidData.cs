using System.ComponentModel;
using System.Runtime.InteropServices;

namespace VisualArrays.Others;

/// <summary>
/// 
/// </summary>
[Serializable]
[TypeConverter(typeof(VoidDataConverter))]
[ComVisible(true)]
public struct VoidData
{
    private void ResetVoidData()
    {
        va_x = 0;
        va_y = 0;
        va_color = Color.Red;
    }
    private bool ShouldSerializeVoidData()
    {
        return (va_x != 0 || va_y != 0 || va_color != Color.Red);
    }

    private int va_x;
    /// <summary>
    /// 
    /// </summary>
    [DefaultValue(typeof(int), "0"), Description("Position X")]
    public int X
    {
        get => va_x;
        set => va_x = value;
    }
    private int va_y;
    /// <summary>
    /// 
    /// </summary>
    [DefaultValue(typeof(int), "0"), Description("Position Y")]
    public int Y
    {
        get => va_y;
        set => va_y = value;
    }
    private Color va_color;
    /// <summary>
    /// 
    /// </summary>
    [DefaultValue(typeof(Color), "Red"), Description("Couleur de la sélection")]
    public Color Color
    {
        get => va_color;
        set => va_color = value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pX"></param>
    /// <param name="pY"></param>
    /// <param name="pColor"></param>
    public VoidData(int pX, int pY, Color pColor)
    {
        va_x = pX;
        va_y = pY;
        va_color = pColor;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("{0},{1},{2}", va_x, va_y, va_color.ToKnownColor());
    }
}
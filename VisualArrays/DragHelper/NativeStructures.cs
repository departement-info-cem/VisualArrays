using System.Runtime.InteropServices;

namespace VisualArrays.DragHelper;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Win32Point
{
    /// <summary>
    /// 
    /// </summary>
    public int x;
    /// <summary>
    /// 
    /// </summary>
    public int y;
}
/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Win32Size
{
    /// <summary>
    /// 
    /// </summary>
    public int cx;
    /// <summary>
    /// 
    /// </summary>
    public int cy;
}
/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ShDragImage
{
    /// <summary>
    /// 
    /// </summary>
    public Win32Size sizeDragImage;
    /// <summary>
    /// 
    /// </summary>
    public Win32Point ptOffset;
    /// <summary>
    /// 
    /// </summary>
    public IntPtr hbmpDragImage;
    /// <summary>
    /// 
    /// </summary>
    public int crColorKey;
}
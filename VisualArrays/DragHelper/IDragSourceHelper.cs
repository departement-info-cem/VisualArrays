using System.Runtime.InteropServices;
using IDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace VisualArrays.DragHelper;

/// <summary>
/// 
/// </summary>
[ComVisible(true)]
[ComImport]
[Guid("DE5BF786-477A-11D2-839D-00C04FD918D0")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IDragSourceHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dragImage"></param>
    /// <param name="dataObject"></param>
    void InitializeFromBitmap(
        [In, MarshalAs(UnmanagedType.Struct)] ref ShDragImage dragImage,
        [In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="pt"></param>
    /// <param name="dataObject"></param>
    void InitializeFromWindow(
        [In] IntPtr hwnd,
        [In] ref Win32Point pt,
        [In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject);
}
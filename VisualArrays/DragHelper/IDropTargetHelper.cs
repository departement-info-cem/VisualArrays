using System.Runtime.InteropServices;
using IDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace VisualArrays.DragHelper;

/// <summary>
/// 
/// </summary>
[ComVisible(true)]
[ComImport]
[Guid("4657278B-411B-11D2-839A-00C04FD918D0")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IDropTargetHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hwndTarget"></param>
    /// <param name="dataObject"></param>
    /// <param name="pt"></param>
    /// <param name="effect"></param>
    void DragEnter(
        [In] IntPtr hwndTarget,
        [In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject,
        [In] ref Win32Point pt,
        [In] int effect);
    /// <summary>
    /// 
    /// </summary>
    void DragLeave();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pt"></param>
    /// <param name="effect"></param>
    void DragOver(
        [In] ref Win32Point pt,
        [In] int effect);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataObject"></param>
    /// <param name="pt"></param>
    /// <param name="effect"></param>
    void Drop(
        [In, MarshalAs(UnmanagedType.Interface)] IDataObject dataObject,
        [In] ref Win32Point pt,
        [In] int effect);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="show"></param>
    void Show(
        [In] bool show);
}
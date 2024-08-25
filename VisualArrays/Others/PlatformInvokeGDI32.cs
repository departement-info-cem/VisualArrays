using System.Runtime.InteropServices;

namespace VisualArrays.Others;

/// <summary>
/// This class shall keep the GDI32 APIs used in our program.
/// </summary>
internal class PlatformInvokeGDI32
{

    #region Class Variables
    /// <summary>
    /// 
    /// </summary>
    public const int SRCCOPY = 13369376;
    #endregion

    #region Class Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hDc"></param>
    /// <returns></returns>
    [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
    public static extern IntPtr DeleteDC(IntPtr hDc);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hDc"></param>
    /// <returns></returns>
    [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    public static extern IntPtr DeleteObject(IntPtr hDc);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hdcDest"></param>
    /// <param name="xDest"></param>
    /// <param name="yDest"></param>
    /// <param name="wDest"></param>
    /// <param name="hDest"></param>
    /// <param name="hdcSource"></param>
    /// <param name="xSrc"></param>
    /// <param name="ySrc"></param>
    /// <param name="RasterOp"></param>
    /// <returns></returns>
    [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
    public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
        wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hdc"></param>
    /// <param name="nWidth"></param>
    /// <param name="nHeight"></param>
    /// <returns></returns>
    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hdc"></param>
    /// <returns></returns>
    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hdc"></param>
    /// <param name="bmp"></param>
    /// <returns></returns>
    [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
    #endregion

}

/// <summary>
/// This class shall keep the User32 APIs used in our program.
/// </summary>
internal class PlatformInvokeUSER32
{
    #region Class Variables
    /// <summary>
    /// 
    /// </summary>
    public const int SM_CXSCREEN = 0;
    /// <summary>
    /// 
    /// </summary>
    public const int SM_CYSCREEN = 1;
    #endregion

    #region Class Functions
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
    public static extern IntPtr GetDesktopWindow();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ptr"></param>
    /// <returns></returns>
    [DllImport("user32.dll", EntryPoint = "GetDC")]
    public static extern IntPtr GetDC(IntPtr ptr);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="abc"></param>
    /// <returns></returns>
    [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
    public static extern int GetSystemMetrics(int abc);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ptr"></param>
    /// <returns></returns>
    [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
    public static extern IntPtr GetWindowDC(Int32 ptr);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="hDc"></param>
    /// <returns></returns>
    [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
    public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

    #endregion
}

/// <summary>
/// This class shall keep all the functionality for capturing the desktop.
/// </summary>
internal class CaptureScreen
{

    #region Public Class Functions
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Bitmap GetDesktopImage()
    {

        //Variable to keep the handle of the btimap.
        IntPtr va_HBitmap = IntPtr.Zero;

        //Variable to keep the refrence to the desktop bitmap.
        Bitmap bmp = null;

        //In size variable we shall keep the size of the screen.
        SIZE size;

        //Here we get the handle to the desktop device context.
        IntPtr hDC = PlatformInvokeUSER32.GetDC(PlatformInvokeUSER32.GetDesktopWindow());

        //Here we make a compatible device context in memory for screen device context.
        IntPtr hMemDC = PlatformInvokeGDI32.CreateCompatibleDC(hDC);

        //We pass SM_CXSCREEN constant to GetSystemMetrics to get the X coordinates of screen.
        size.cx = PlatformInvokeUSER32.GetSystemMetrics(PlatformInvokeUSER32.SM_CXSCREEN);

        //We pass SM_CYSCREEN constant to GetSystemMetrics to get the Y coordinates of screen.
        size.cy = PlatformInvokeUSER32.GetSystemMetrics(PlatformInvokeUSER32.SM_CYSCREEN);

        //We create a compatible bitmap of screen size and using screen device context.
        va_HBitmap = PlatformInvokeGDI32.CreateCompatibleBitmap(hDC, size.cx, size.cy);

        //As va_HBitmap is IntPtr we can not check it against null. For this purspose IntPtr.Zero is used.
        if (va_HBitmap != IntPtr.Zero)
        {
            //Here we select the compatible bitmap in memeory device context and keeps the refrence to Old bitmap.
            IntPtr hOld = PlatformInvokeGDI32.SelectObject(hMemDC, va_HBitmap);

            //We copy the Bitmap to the memory device context.
            PlatformInvokeGDI32.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, 0, 0, PlatformInvokeGDI32.SRCCOPY);

            //We select the old bitmap back to the memory device context.
            PlatformInvokeGDI32.SelectObject(hMemDC, hOld);

            //We delete the memory device context.
            PlatformInvokeGDI32.DeleteDC(hMemDC);

            //We release the screen device context.
            PlatformInvokeUSER32.ReleaseDC(PlatformInvokeUSER32.GetDesktopWindow(), hDC);

            //Image is created by Image bitmap handle and assigned to Bitmap variable.
            bmp = Image.FromHbitmap(va_HBitmap);

            //Delete the compatible bitmap object.
            PlatformInvokeGDI32.DeleteObject(va_HBitmap);

            return bmp;
        }
        //If va_HBitmap is null retunrn null.
        return null;
    }
    #endregion
}

//This structure shall be used to keep the size of the screen.
/// <summary>
/// 
/// </summary>
internal struct SIZE
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
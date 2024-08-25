using System.Drawing.Drawing2D;
using VisualArrays.Others;

namespace VisualArrays.CellVisualElement;

/// <summary>
/// Représente un forme apparaissant sur une cellule d'une grille.
/// </summary>
public class ShapeElement : CellVisualElement
{
    private static readonly Color[] va_tabCouleurs = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Yellow, Color.Magenta, Color.Cyan, Color.DarkViolet, Color.Tomato };
    private static int va_indexCouleur = 0;

    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit le radius utilisé lorsque la forme est RoundRect.
    /// </summary>
    public int Radius { get; set; }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la couleur de la forme.
    /// </summary>
    public Color Color { get; set; }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la forme de l'élément.
    /// </summary>
    public enuShape Shape { get; set; }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la taille du crayon qui dessine la forme.
    /// </summary>
    public int PenWidth { get; set; }
    //-------------------------------------------------------------------------------------
    #region Constructeur
    /// <summary>
    /// Initialise un élément visuel d'une certaine forme.
    /// </summary>
    /// <param name="pShape">Une forme.</param>
    public ShapeElement(enuShape pShape)
    {
        Shape = pShape;
        PenWidth = 1;
        Radius = 10;
        Color = va_tabCouleurs[va_indexCouleur++ % va_tabCouleurs.Length];
    }
    /// <summary>
    /// Initialise un élément visuel d'une certaine forme.
    /// </summary>
    /// <param name="pShape">Une forme.</param>
    /// <param name="pPenWidth">Taille du crayon qui dessine la forme.</param>
    public ShapeElement(enuShape pShape, int pPenWidth)
    {
        Shape = pShape;
        PenWidth = pPenWidth;
        Radius = 10;
        Color = va_tabCouleurs[va_indexCouleur++ % va_tabCouleurs.Length];
    }
    /// <summary>
    /// Initialise un élément visuel d'une certaine forme.
    /// </summary>
    /// <param name="pShape">Une forme.</param>
    /// <param name="pPenWidth">Taille du crayon qui dessine la forme.</param>
    /// <param name="pRadius">Radius utilisé lorsque la forme est RoundRect.</param>
    public ShapeElement(enuShape pShape, int pPenWidth,int pRadius)
    {
        Shape = pShape;
        PenWidth = pPenWidth;
        Radius = pRadius;
        Color = va_tabCouleurs[va_indexCouleur++ % va_tabCouleurs.Length];
    }
    /// <summary>
    /// Initialise un élément visuel d'une certaine forme.
    /// </summary>
    /// <param name="pShape">Une forme.</param>
    /// <param name="pAlignment">Un alignement.</param>
    public ShapeElement(enuShape pShape, ContentAlignment pAlignment)
    {
        Shape = pShape;
        va_alignment = pAlignment;
        PenWidth = 1;
        Radius = 10;
        Color = va_tabCouleurs[va_indexCouleur++ % va_tabCouleurs.Length];
    }
    /// <summary>
    /// Initialise un élément visuel d'une certaine forme et couleur.
    /// </summary>
    /// <param name="pShape">Une forme.</param>
    /// <param name="pPenWidth">Taille du crayon qui dessine la forme.</param>
    /// <param name="pColor">Couleur du crayon qui dessine la forme.</param>
    /// <param name="pRadius">Radius utilisé lorsque la forme est RoundRect.</param>
    public ShapeElement(enuShape pShape, int pPenWidth, Color pColor, int pRadius)
    {
        Shape = pShape;
        PenWidth = pPenWidth;
        va_alignment = ContentAlignment.MiddleCenter;
        Radius = pRadius;
        Color = pColor;
    }
    /// <summary>
    /// Initialise un élément visuel d'une certaine forme et couleur.
    /// </summary>
    /// <param name="pShape">Une forme.</param>
    /// <param name="pPenWidth">Taille du crayon qui dessine la forme.</param>
    /// <param name="pColor">Couleur du crayon qui dessine la forme.</param>
    /// <param name="pZoom">Facteur d'aggrandissement de la forme en poucentage.</param>
    /// <param name="pAlignment">Un alignement.</param>
    public ShapeElement(enuShape pShape, int pPenWidth, Color pColor, int pZoom, ContentAlignment pAlignment)
    {
        Shape = pShape;
        PenWidth = pPenWidth;
        Color = pColor;
        va_zoom = pZoom;
        va_alignment = pAlignment;
        Radius = 10;
    }
    #endregion

    private static void DrawRoundRect(Graphics pGraphics, Rectangle r, Color color, int radius, int width)
    {
        int dia = 2 * radius;

        // set to pixel mode
        GraphicsUnit oldPageUnit = pGraphics.PageUnit;
        //int oldPageUnit = pGraphics.SetSetPageUnit(UnitPixel);

        // define the pen
        Pen pen = new(color, 1);
        pen.Alignment = PenAlignment.Center;

        // get the corner path
        GraphicsPath path = new();

        // get path
        GetRoundRectPath(path, r, dia);

        // draw the round rect
        pGraphics.DrawPath(pen, path);

        // if width > 1
        for (int i = 1; i < width; i++)
        {
            // left stroke
            r.Inflate(-1, 0);
            // get the path
            GetRoundRectPath(path, r, dia);

            // draw the round rect
            pGraphics.DrawPath(pen, path);

            // up stroke
            r.Inflate(0, -1);

            // get the path
            GetRoundRectPath(path, r, dia);

            // draw the round rect
            pGraphics.DrawPath(pen, path);
        }

        // restore page unit
        pGraphics.PageUnit = oldPageUnit;
    }

    internal static void GetRoundRectPath(GraphicsPath pPath, Rectangle r, int dia)
    {
        // diameter can't exceed width or height
        if (dia > r.Width) dia = r.Width;
        if (dia > r.Height) dia = r.Height;

        // define a corner 
        Rectangle Corner = r with { Width = dia, Height = dia };

        // begin path
        pPath.Reset();

        // top left
        pPath.AddArc(Corner, 180, 90);

        // tweak needed for radius of 10 (dia of 20)
        if (dia == 20)
        {
            Corner.Width += 1;
            Corner.Height += 1;
            r.Width -= 1; r.Height -= 1;
        }

        // top right
        Corner.X += (r.Width - dia - 1);
        pPath.AddArc(Corner, 270, 90);

        // bottom right
        Corner.Y += (r.Height - dia - 1);
        pPath.AddArc(Corner, 0, 90);

        // bottom left
        Corner.X -= (r.Width - dia - 1);
        pPath.AddArc(Corner, 90, 90);

        // end path
        pPath.CloseFigure();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pShape"></param>
    /// <param name="pGraphics"></param>
    /// <param name="pBounds"></param>
    /// <param name="pPen"></param>
    /// <param name="pRadius"></param>
    public static void DrawShape(enuShape pShape, Graphics pGraphics, Rectangle pBounds, Pen pPen, int pRadius)
    {
        Point[] pts; int centreX, centreY, unQuart;
        //pPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
        pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        switch (pShape)
        {
            case enuShape.RoundRect:
                DrawRoundRect(pGraphics, pBounds, pPen.Color, pRadius, (int)pPen.Width);
                break;
            case enuShape.Rectangle:
                pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                pGraphics.DrawRectangle(pPen, pBounds);
                break;
            case enuShape.Ellipse:
                pGraphics.DrawEllipse(pPen, pBounds);
                break;
            case enuShape.TriangleIsoUp:
                pts = new Point[3];
                pts[0] = new Point(pBounds.Left, pBounds.Bottom);
                pts[1] = new Point(pBounds.Left + pBounds.Width / 2, pBounds.Top);
                pts[2] = new Point(pBounds.Right, pBounds.Bottom);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.TriangleIsoDown:
                pts = new Point[3];
                pts[0] = new Point(pBounds.Left, pBounds.Top);
                pts[1] = new Point(pBounds.Right, pBounds.Top);
                pts[2] = new Point(pBounds.Left + pBounds.Width / 2, pBounds.Bottom);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.TriangleIsoLeft:
                pts = new Point[3];
                pts[0] = new Point(pBounds.Right, pBounds.Top);
                pts[1] = new Point(pBounds.Right, pBounds.Bottom);
                pts[2] = new Point(pBounds.Left, pBounds.Top + pBounds.Height / 2);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.TriangleIsoRight:
                pts = new Point[3];
                pts[0] = new Point(pBounds.Left, pBounds.Top);
                pts[1] = new Point(pBounds.Left, pBounds.Bottom);
                pts[2] = new Point(pBounds.Right, pBounds.Top + pBounds.Height / 2);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.Losange:
                pts = new Point[4];
                centreX = pBounds.Left + pBounds.Width / 2;
                centreY = pBounds.Top + pBounds.Height / 2;
                pts[0] = new Point(pBounds.Left, centreY);
                pts[1] = new Point(centreX, pBounds.Top);
                pts[2] = new Point(pBounds.Right, centreY);
                pts[3] = new Point(centreX, pBounds.Bottom);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.TriangleRectBL:
                pts = new Point[3];
                pts[0] = new Point(pBounds.Left, pBounds.Top);
                pts[1] = new Point(pBounds.Left, pBounds.Bottom);
                pts[2] = new Point(pBounds.Right, pBounds.Bottom);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.TriangleRectBR:
                pts = new Point[3];
                pts[0] = new Point(pBounds.Left, pBounds.Bottom);
                pts[1] = new Point(pBounds.Right, pBounds.Bottom);
                pts[2] = new Point(pBounds.Right, pBounds.Top);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.TriangleRectTL:
                pts = new Point[3];
                pts[0] = new Point(pBounds.Left, pBounds.Top);
                pts[1] = new Point(pBounds.Left, pBounds.Bottom);
                pts[2] = new Point(pBounds.Right, pBounds.Top);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.TriangleRectTR:
                pts = new Point[3];
                pts[0] = new Point(pBounds.Left, pBounds.Top);
                pts[1] = new Point(pBounds.Right, pBounds.Top);
                pts[2] = new Point(pBounds.Right, pBounds.Bottom);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.HexagonH:
                pts = new Point[6];
                centreY = pBounds.Top + pBounds.Height / 2;
                unQuart = pBounds.Width / 4;
                pts[0] = new Point(pBounds.Left, centreY);
                pts[1] = new Point(pBounds.Left + unQuart, pBounds.Top);
                pts[2] = new Point(pBounds.Right - unQuart, pBounds.Top);
                pts[3] = new Point(pBounds.Right, centreY);
                pts[4] = new Point(pBounds.Right - unQuart, pBounds.Bottom);
                pts[5] = new Point(pBounds.Left + unQuart, pBounds.Bottom);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.HexagonV:
                pts = new Point[6];
                centreX = pBounds.Left + pBounds.Width / 2;
                unQuart = pBounds.Height / 4;
                pts[0] = new Point(pBounds.Left, pBounds.Top + unQuart);
                pts[1] = new Point(centreX, pBounds.Top);
                pts[2] = new Point(pBounds.Right, pBounds.Top + unQuart);
                pts[3] = new Point(pBounds.Right, pBounds.Bottom - unQuart);
                pts[4] = new Point(centreX, pBounds.Bottom);
                pts[5] = new Point(pBounds.Left, pBounds.Bottom - unQuart);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            case enuShape.Parallelogram:
                pts = new Point[4];
                unQuart = pBounds.Width / 4;
                pts[0] = new Point(pBounds.Left, pBounds.Bottom);
                pts[1] = new Point(pBounds.Left + unQuart, pBounds.Top);
                pts[2] = new Point(pBounds.Right, pBounds.Top);
                pts[3] = new Point(pBounds.Right - unQuart, pBounds.Bottom);
                pGraphics.DrawPolygon(pPen, pts);
                break;
            default:
                break;
        }
        pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
    }
    /// <summary>
    /// Dessine l'élément visuel selon la forme courante.
    /// </summary>
    /// <param name="pGraphics"></param>
    /// <param name="pBounds"></param>
    public override void Draw(Graphics pGraphics, Rectangle pBounds)
    {
        int largeurZoom = pBounds.Width * va_zoom / 100;
        int hauteurZoom = pBounds.Height * va_zoom / 100;
        Rectangle contour = BoundsFromAlignment(pBounds, new Size(largeurZoom, hauteurZoom), va_alignment);
        int posX = contour.Left + va_margin.Left - va_margin.Right;
        int posY = contour.Top + va_margin.Top - va_margin.Bottom;
        Rectangle contourSelonPenWidth = new(posX + (PenWidth >> 1), posY + (PenWidth >> 1), contour.Width - PenWidth, contour.Height - PenWidth);
        DrawShape(Shape, pGraphics, contourSelonPenWidth, new Pen(Color, PenWidth),Radius);
    }
}
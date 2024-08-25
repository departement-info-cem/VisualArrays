namespace VisualArrays.CellVisualElement;

/// <summary>
/// Représente un forme apparaissant sur une cellule d'une grille.
/// </summary>
public class BorderElement : CellVisualElement
{
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la couleur de la forme.
    /// </summary>
    public Color Color { get; set; }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la taille des bordures.
    /// </summary>
    public Padding Border { get; set; }

    //-------------------------------------------------------------------------------------
    #region Constructeur
    /// <summary>
    /// Initialise un élément visuel de type bordure.
    /// </summary>
    public BorderElement()
    {
        Border = new Padding(1);
        Color = Color.Red;
    }
    /// <summary>
    /// Initialise un élément visuel de type bordure.
    /// </summary>
    /// <param name="pBorder">Taille des bordures</param>
    /// <param name="pColor">Couleur des bordures</param>
    public BorderElement(Padding pBorder, Color pColor)
    {
        Border = pBorder;
        Color = pColor;
    }
    #endregion

    /// <summary>
    /// Dessine l'élément visuel
    /// </summary>
    /// <param name="pGraphics"></param>
    /// <param name="pBounds">Coutour utilisé pour la bordure</param>
    /// <param name="pBorder">Taille de chacune des bordures</param>
    /// <param name="pColor">Couleur de toutes les bordures</param>
    public static void DrawBorder(Graphics pGraphics, Rectangle pBounds, Padding pBorder, Color pColor)
    {
        //Point[] pts; int centreX, centreY, unQuart;
        //pPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
        //pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

        if (pBorder.Left > 0)
        {
            Pen leftPen = new Pen(pColor, pBorder.Left);
            int posX = pBounds.Left + (pBorder.Left >> 1);
            pGraphics.DrawLine(leftPen, posX, pBounds.Top, posX, pBorder.Left == 1 ? pBounds.Bottom - 1 : pBounds.Bottom);
        }

        if (pBorder.Top > 0)
        {
            Pen topPen = new Pen(pColor, pBorder.Top);
            int posY = pBounds.Top + (pBorder.Top >> 1);
            pGraphics.DrawLine(topPen, pBounds.Left, posY, pBorder.Top == 1 ? pBounds.Right - 1 : pBounds.Right, posY);
        }

        if (pBorder.Right > 0)
        {
            Pen rightPen = new Pen(pColor, pBorder.Right);
            int posX = pBounds.Right - pBorder.Right + (pBorder.Right >> 1);
            pGraphics.DrawLine(rightPen, posX, pBounds.Top, posX, pBorder.Right == 1 ? pBounds.Bottom - 1 : pBounds.Bottom);
        }

        if (pBorder.Bottom > 0)
        {
            Pen bottomPen = new Pen(pColor, pBorder.Bottom);
            int posY = pBounds.Bottom - pBorder.Bottom + (pBorder.Bottom >> 1);
            pGraphics.DrawLine(bottomPen, pBounds.Left, posY, pBorder.Bottom == 1 ? pBounds.Right - 1 : pBounds.Right, posY);
        }
    }
    /// <summary>
    /// Dessine l'élément visuel avec les bordures courantes.
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
        Rectangle contourSelonPenWidth = new Rectangle(posX,posY, contour.Width, contour.Height);
        DrawBorder(pGraphics, contourSelonPenWidth, Border,Color);
    }
}
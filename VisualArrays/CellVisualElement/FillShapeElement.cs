using System.Drawing.Drawing2D;
using VisualArrays.Others;

namespace VisualArrays.CellVisualElement
{
    /// <summary>
    /// Représente un forme apparaissant sur une cellule d'une grille.
    /// </summary>
    public class FillShapeElement : CellVisualElement
    {
        private static readonly Color[] va_tabCouleurs = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Yellow, Color.Magenta, Color.Cyan, Color.DarkViolet, Color.Tomato };
        private static int va_indexCouleur = 0;
        //-------------------------------------------------------------------------------------
        private Color va_color;
        /// <summary>
        /// Obtient ou définit la couleur de la forme
        /// </summary>
        public Color Color
        {
            get { return va_color; }
            set
            {
                va_color = value;
                //va_owner.RedessinerCelluleEtSprites(Index);
            }
        }
        //-------------------------------------------------------------------------------------
        private enuShape va_shape;
        /// <summary>
        /// Obtient ou définit la forme de l'élément
        /// </summary>
        public enuShape Shape
        {
            get { return va_shape; }
            set
            {
                va_shape = value;
                //va_owner.RedessinerCelluleEtSprites(Index);
            }
        }
        //-------------------------------------------------------------------------------------
        /// <summary>
        /// Obtient ou définit radius utilisé lorsque la forme est RoundRect.
        /// </summary>
        protected int va_radius;
        /// <summary>
        /// Obtient ou définit radius utilisé lorsque la forme est RoundRect.
        /// </summary>
        public int Radius
        {
            get { return va_radius; }
            set { va_radius = value; }
        }

        //-------------------------------------------------------------------------------------
        #region Constructeur
        /// <summary>
        /// Initialise un élément visuel d'une certaine forme.
        /// </summary>
        /// <param name="pShape">Forme de l'élément</param>
        public FillShapeElement(enuShape pShape)
        {
            va_color = va_tabCouleurs[va_indexCouleur++ % va_tabCouleurs.Length];
            va_shape = pShape;
            va_radius = 10;
        }
        /// <summary>
        /// Initialise un élément visuel d'une certaine forme.
        /// </summary>
        /// <param name="pShape">Forme de l'élément</param>
        /// <param name="pColor">Couleur de la forme</param>
        public FillShapeElement(enuShape pShape, Color pColor)
        {
            va_shape = pShape;
            va_color = pColor;
            va_radius = 10;
            va_alignment = ContentAlignment.MiddleCenter;
        }
        /// <summary>
        /// Initialise un élément visuel d'une certaine forme.
        /// </summary>
        /// <param name="pShape">Forme de l'élément</param>
        /// <param name="pColor">Couleur de la forme</param>
        /// <param name="pRadius">Radius utilisé lorsque la forme est RoundRect</param>
        public FillShapeElement(enuShape pShape, Color pColor, int pRadius)
        {
            va_shape = pShape;
            va_color = pColor;
            va_radius = pRadius;
            va_alignment = ContentAlignment.MiddleCenter;
        }
        ///// <summary>
        ///// Initialise un élément visuel d'une certaine forme et couleur
        ///// </summary>
        ///// <param name="pShape">Forme de l'élément</param>
        ///// <param name="pColor">Couleur de la forme</param>
        ///// <param name="pZoom">Facteur d'aggrandissement de la forme</param>
        //public FillShapeElement(enuShape pShape,Color pColor,int pZoom)
        //{
        //    va_shape = pShape;
        //    va_color = pColor;
        //    va_zoom = pZoom;
        //    va_radius = 10;
        //}
        /// <summary>
        /// Initialise un élément visuel d'une certaine forme et couleur
        /// </summary>
        /// <param name="pShape">Forme de l'élément</param>
        /// <param name="pColor">Couleur de la forme</param>
        /// <param name="pZoom">Facteur d'aggrandissement de la forme</param>
        /// <param name="pAlignment">Alignement de la forme</param>
        public FillShapeElement(enuShape pShape, Color pColor, int pZoom,ContentAlignment pAlignment)
        {
            va_shape = pShape;
            va_color = pColor;
            va_zoom = pZoom;
            va_alignment = pAlignment;
            va_radius = 10;
        }
        #endregion


        private static void FillRoundRect(Graphics pGraphics, Brush pBrush, Rectangle r, Color border, int radius)
        {
            int dia = 2 * radius;

            // set to pixel mode
            GraphicsUnit oldPageUnit = pGraphics.PageUnit;
            pGraphics.PageUnit = GraphicsUnit.Pixel;

            // define the pen
            Pen pen = new Pen(border, 1);
            pen.Alignment = PenAlignment.Center;

            // get the corner path
            GraphicsPath path = new GraphicsPath();

            // get path
            ShapeElement.GetRoundRectPath(path, r, dia);

            // fill
            pGraphics.FillPath(pBrush, path);

            // draw the border last so it will be on top
            pGraphics.DrawPath(pen, path);

            // restore page unit
            pGraphics.PageUnit = oldPageUnit;
        }
        /// <summary>
        /// </summary>
        /// <param name="pShape"></param>
        /// <param name="pGraphics"></param>
        /// <param name="pBounds"></param>
        /// <param name="pColor"></param>
        /// <param name="pAlpha"></param>
        /// <param name="pRadius"></param>
        public static void DrawFillShape(enuShape pShape, Graphics pGraphics, Rectangle pBounds, Color pColor,int pAlpha, int pRadius)
        {
            Point[] pts; int centreX, centreY, unQuart;
            pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Brush brush = new SolidBrush(Color.FromArgb(pAlpha, pColor));
            switch (pShape)
            {
                case enuShape.RoundRect:
                    FillRoundRect(pGraphics, brush, pBounds, Color.FromArgb(pAlpha, pColor), pRadius);
                    break;
                case enuShape.Rectangle:
                    pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    pGraphics.FillRectangle(brush, pBounds);
                    break;
                case enuShape.Ellipse:
                    pGraphics.FillEllipse(brush, pBounds);
                    break;
                case enuShape.TriangleIsoUp:
                    pts = new Point[3];
                    pts[0] = new Point(pBounds.Left, pBounds.Bottom);
                    pts[1] = new Point(pBounds.Left + pBounds.Width / 2, pBounds.Top);
                    pts[2] = new Point(pBounds.Right, pBounds.Bottom);
                    pGraphics.FillPolygon(brush, pts);
                    break;
                case enuShape.TriangleIsoDown:
                    pts = new Point[3];
                    pts[0] = new Point(pBounds.Left, pBounds.Top);
                    pts[1] = new Point(pBounds.Right, pBounds.Top);
                    pts[2] = new Point(pBounds.Left + pBounds.Width / 2, pBounds.Bottom);
                    pGraphics.FillPolygon(brush, pts);
                    break;
                case enuShape.TriangleIsoLeft:
                    pts = new Point[3];
                    pts[0] = new Point(pBounds.Right, pBounds.Top);
                    pts[1] = new Point(pBounds.Right, pBounds.Bottom);
                    pts[2] = new Point(pBounds.Left, pBounds.Top + pBounds.Height / 2);
                    pGraphics.FillPolygon(brush, pts);
                    break;
                case enuShape.TriangleIsoRight:
                    pts = new Point[3];
                    pts[0] = new Point(pBounds.Left, pBounds.Top);
                    pts[1] = new Point(pBounds.Left, pBounds.Bottom);
                    pts[2] = new Point(pBounds.Right, pBounds.Top + pBounds.Height / 2);
                    pGraphics.FillPolygon(brush, pts);
                    break;
                case enuShape.Losange:
                    pts = new Point[4];
                    centreX = pBounds.Left + pBounds.Width / 2;
                    centreY = pBounds.Top + pBounds.Height / 2;
                    pts[0] = new Point(pBounds.Left, centreY);
                    pts[1] = new Point(centreX, pBounds.Top);
                    pts[2] = new Point(pBounds.Right, centreY);
                    pts[3] = new Point(centreX, pBounds.Bottom);
                    pGraphics.FillPolygon(brush, pts);
                    break;
                case enuShape.TriangleRectBL:
                    pts = new Point[3];
                    pts[0] = new Point(pBounds.Left, pBounds.Top);
                    pts[1] = new Point(pBounds.Left, pBounds.Bottom);
                    pts[2] = new Point(pBounds.Right, pBounds.Bottom);
                    pGraphics.FillPolygon(brush, pts);
                    break;
                case enuShape.TriangleRectBR:
                    pts = new Point[3];
                    pts[0] = new Point(pBounds.Left, pBounds.Bottom);
                    pts[1] = new Point(pBounds.Right, pBounds.Bottom);
                    pts[2] = new Point(pBounds.Right, pBounds.Top);
                    pGraphics.FillPolygon(brush, pts);
                    break;
                case enuShape.TriangleRectTL:
                    pts = new Point[3];
                    pts[0] = new Point(pBounds.Left, pBounds.Top);
                    pts[1] = new Point(pBounds.Left, pBounds.Bottom);
                    pts[2] = new Point(pBounds.Right, pBounds.Top);
                    pGraphics.FillPolygon(brush, pts);
                    break;
                case enuShape.TriangleRectTR:
                    pts = new Point[3];
                    pts[0] = new Point(pBounds.Left, pBounds.Top);
                    pts[1] = new Point(pBounds.Right, pBounds.Top);
                    pts[2] = new Point(pBounds.Right, pBounds.Bottom);
                    pGraphics.FillPolygon(brush, pts);
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
                    pGraphics.FillPolygon(brush, pts);
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
                    pGraphics.FillPolygon(brush, pts);
                    break;
                case enuShape.Parallelogram:
                    pts = new Point[4];
                    unQuart = pBounds.Width / 4;
                    pts[0] = new Point(pBounds.Left, pBounds.Bottom);
                    pts[1] = new Point(pBounds.Left + unQuart, pBounds.Top);
                    pts[2] = new Point(pBounds.Right, pBounds.Top);
                    pts[3] = new Point(pBounds.Right - unQuart, pBounds.Bottom);
                    pGraphics.FillPolygon(brush, pts);
                    break;
                default:
                    break;
            }
            brush.Dispose();
            pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        }
        /// <summary>
        /// Dessine une forme pleine
        /// </summary>
        /// <param name="pGraphics"></param>
        /// <param name="pBounds"></param>
        public override void Draw(Graphics pGraphics, Rectangle pBounds)
        {
            int largeurZoom = pBounds.Width * va_zoom / 100;
            int hauteurZoom = pBounds.Height * va_zoom / 100;
            Rectangle contour = BoundsFromAlignment(pBounds, new Size(largeurZoom, hauteurZoom),va_alignment);
            int posX = contour.Left + va_margin.Left - va_margin.Right;
            int posY = contour.Top + va_margin.Top - va_margin.Bottom;
            Rectangle contourWithMargin = new Rectangle(posX, posY, contour.Width, contour.Height);
            DrawFillShape(va_shape, pGraphics, contourWithMargin, va_color,255,va_radius);
        }
    }
}

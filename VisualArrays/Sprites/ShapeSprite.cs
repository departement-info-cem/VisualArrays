using System.ComponentModel;
using VisualArrays.CellVisualElement;
using VisualArrays.Others;

namespace VisualArrays.Sprites
{
    /// <summary>
    /// 
    /// </summary>
    public class ShapeSprite : Sprite
    {
        private Color va_color;
        /// <summary>
        /// Obtient ou définit la couleur de la forme
        /// </summary>
        [DefaultValue(typeof(Color), "Red"), Description("Couleur de fond du 'Sprite'")]
        [Localizable(true), Category("Layout")]
        public Color Color
        {
            get => va_color;
            set
            {
                va_color = value;
                va_pen = new Pen(va_color, va_penWidth);
                m_owner?.UpdateSprites(m_bounds);
            }
        }
        //-------------------------------------------------------------------------------------
        private enuShape va_shape;
        /// <summary>
        /// Obtient ou définit la forme de l'élément
        /// </summary>
        [DefaultValue(enuShape.Ellipse), Description("Forme utilisée pour dessiner le 'Sprite'")]
        [Localizable(true), Category("Layout")]
        public enuShape Shape
        {
            get => va_shape;
            set
            {
                va_shape = value;
                m_owner?.UpdateSprites(m_bounds);
            }
        }

        //-------------------------------------------------------------------------------------
        private int va_penWidth;
        private Pen va_pen;
        /// <summary>
        /// Obtient ou définit la taille du crayon qui dessine la forme.
        /// </summary>
        [DefaultValue(typeof(int), "3"), Description("Taille du crayon utilisé pour dessiner la forme du 'Sprite'")]
        public int PenWidth
        {
            get => va_penWidth;
            set
            {
                va_penWidth = value;
                va_pen = new Pen(va_color, va_penWidth);
                m_owner?.UpdateSprites(m_bounds);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ShapeSprite()
        {
            va_shape = enuShape.Ellipse;
            va_color = Color.Red;
            va_penWidth = 3;
            va_pen = new Pen(va_color, va_penWidth);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pGraphics"></param>
        public override void Draw(Graphics pGraphics)
        {
            if (m_visible)
            {
                base.Draw(pGraphics);

                Rectangle contour = m_bounds;
                //contour.X += 1;
                //contour.Width -= 2;
                //contour.Y += 1;
                //contour.Height -= 2;
                int largeurZoom = contour.Width; // *va_zoom / 100;
                int hauteurZoom = contour.Height; // *va_zoom / 100;
                contour = CellVisualElement.CellVisualElement.BoundsFromAlignment(contour, new Size(largeurZoom, hauteurZoom), m_alignment);
                Rectangle contourSelonPenWidth = new(contour.Left + (va_penWidth >> 1), contour.Top + (va_penWidth >> 1), contour.Width - PenWidth, contour.Height - PenWidth);
                ShapeElement.DrawShape(va_shape, pGraphics, contourSelonPenWidth, va_pen,10);
            }
        }
        //===========================================================================
        /// <summary>
        /// Dessine le Sprite à la coordonnée 0,0 dans le graphics
        /// </summary>
        /// <param name="pGraphics">Destination du dessin</param>
        public override void DrawAtOrigin(Graphics pGraphics)
        {
            Rectangle contour = new(0, 0, m_bounds.Width, m_bounds.Height);
            Rectangle contourSelonPenWidth = new(contour.Left + (va_penWidth >> 1), contour.Top + (va_penWidth >> 1), contour.Width - PenWidth, contour.Height - PenWidth);
            ShapeElement.DrawShape(va_shape, pGraphics, contourSelonPenWidth, va_pen, 10);
        }
    }
}


using System.ComponentModel;
using VisualArrays.CellVisualElement;
using VisualArrays.Others;

namespace VisualArrays.Sprites
{
    /// <summary>
    /// Représente un 'Sprite' utilisant une forme pleine
    /// </summary>
    public class FillShapeSprite:Sprite
    {
        //-------------------------------------------------------------------------------------
        private Color m_color;
        /// <summary>
        /// Obtient ou définit la couleur de la forme
        /// </summary>
        [DefaultValue(typeof(Color), "Blue"), Description("Couleur de fond du 'Sprite'")]
        [Localizable(true), Category("Layout")]
        public Color Color
        {
            get => m_color;
            set
            {
                m_color = value;
                if (m_owner == null) return;
                m_owner.UpdateSprites(m_bounds);
            }
        }
        //-------------------------------------------------------------------------------------
        private int m_opacity = 192;
        /// <summary>
        /// Obtient et définit le niveau d'opacité du Sprite entre 0 et 255.
        /// </summary>
        [DefaultValue(192), Description("Niveau d'opacity du Sprite")]
        public int Opacity
        {
            get => m_opacity;
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException(
                        "Opacity",
                        value,
                        "doit être >= 0 et <= à 255");
                }
                if (value != m_opacity)
                {
                    m_opacity = value;
                    if (m_owner == null) return;
                    m_owner.UpdateSprites(m_bounds);
                }
            }
        }
        //-------------------------------------------------------------------------------------
        private enuShape m_shape;
        /// <summary>
        /// Obtient ou définit la forme de l'élément
        /// </summary>
        [DefaultValue(enuShape.Ellipse), Description("Forme utilisée pour dessiner le 'Sprite'")]
        [Localizable(true), Category("Layout")]
        public enuShape Shape
        {
            get => m_shape;
            set
            {
                m_shape = value;
                if (m_owner == null) return;
                m_owner.UpdateSprites(m_bounds);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public FillShapeSprite()
        {
            m_shape = enuShape.Ellipse;
            m_color = Color.Blue;
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
                contour.X += 1;
                contour.Width -= 2;
                contour.Y += 1;
                contour.Height -= 2;
                int largeurZoom = contour.Width; // *va_zoom / 100;
                int hauteurZoom = contour.Height; // *va_zoom / 100;
                contour = CellVisualElement.CellVisualElement.BoundsFromAlignment(contour, new Size(largeurZoom, hauteurZoom), m_alignment);
                //va_bounds = contour;
                FillShapeElement.DrawFillShape(m_shape, pGraphics, contour, m_color,m_opacity,10);
            }
        }
        //===========================================================================
        /// <summary>
        /// Dessine le Sprite à la coordonnée 0,0 dans le graphics
        /// </summary>
        /// <param name="pGraphics">Destination du dessin</param>
        public override void DrawAtOrigin(Graphics pGraphics)
        {
            Rectangle contour = new Rectangle(0, 0, m_bounds.Width, m_bounds.Height);
            FillShapeElement.DrawFillShape(m_shape, pGraphics, contour, m_color, m_opacity, 10);
        }
    }
}

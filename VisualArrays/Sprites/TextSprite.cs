using System.ComponentModel;
using VisualArrays.CellVisualElement;
using VisualArrays.Others;

namespace VisualArrays.Sprites
{
    /// <summary>
    /// Un 'Sprite' utilisant une chaîne pour s'afficher
    /// </summary>
    public class TextSprite : Sprite
    {
        #region Text, TextColor, Font
        //-------------------------------------------------------------------------------------
        private const string m_defaultText = "Text";
        private string m_text = m_defaultText;
        /// <summary>
        /// Obtient ou définit le texte du 'Sprite'.
        /// </summary>
        [DefaultValue(typeof(string), m_defaultText), Description("Texte du 'Sprite'")]
        [Localizable(true)]
        public string Text
        {
            get => m_text;
            set
            {
                m_text = value;
                if (m_owner == null) return;
                RecalcBoundsAndRedraw();
            }
        }
        //-------------------------------------------------------------------------------------
        private Color m_textColor = Color.Red;
        /// <summary>
        /// Obtient ou définit la couleur du texte du 'Sprite'
        /// </summary>
        [DefaultValue(typeof(Color), "Red"), Description("Couleur du texte du 'Sprite'")]
        [Localizable(true)]
        public Color TextColor
        {
            get => m_textColor;
            set
            {
                m_textColor = value;
                m_owner?.UpdateSprites(m_bounds);
            }
        }
        //-------------------------------------------------------------------------------------
        private static readonly Font m_defaultFont = new("Arial", 18);
        private Font m_font = m_defaultFont;
        /// <summary>
        /// Obtient ou définit la police utilisée par l'élément. 
        /// </summary>
        [Description("Police utilisée pour afficher le texte du Sprite")]
        public Font Font
        {
            get => m_font;
            set
            {
                m_font = value;
                if (m_owner == null) return;
                RecalcBoundsAndRedraw();
            }
        }
        private void ResetFont()
        {
            m_font = m_defaultFont;
        }
        private bool ShouldSerializeFont()
        {
            return !m_font.Equals(m_defaultFont);
        }
        #endregion

        #region BkgStyle, BkgShape, ShapeColor

        private Color m_shapeColor = Color.Blue;
        /// <summary>
        /// Obtient ou définit la couleur de la forme
        /// </summary>
        [DefaultValue(typeof(Color), "Blue"), Description("Couleur de fond du 'Sprite'")]
        public Color ShapeColor
        {
            get => m_shapeColor;
            set
            {
                m_shapeColor = value;
                m_owner?.UpdateSprites(m_bounds);
            }
        }
        //===========================================================================================
        /// <summary>
        /// Obtient et définit le style de fond du 'Sprite'.
        /// </summary>
        protected internal enuBkgStyle m_backgroundStyle = enuBkgStyle.FillShape;
        /// <summary>
        /// Obtient et définit le style de fond du 'Sprite'.
        /// </summary>
        [DefaultValue(enuBkgStyle.FillShape), Description("Style de fond du 'Sprite'")]
        public enuBkgStyle BackgroundStyle
        {
            get => m_backgroundStyle;
            set
            {
                m_backgroundStyle = value;
                m_owner?.UpdateSprites(m_bounds);
            }
        }
        //-------------------------------------------------------------------------------------
        private enuShape m_backgroundShape = enuShape.Ellipse;
        /// <summary>
        /// Obtient ou définit la forme du fond du 'Sprite'
        /// </summary>
        [DefaultValue(enuShape.Ellipse), Description("Forme utilisée pour dessiner le fond du 'Sprite'")]
        public enuShape BackgroundShape
        {
            get => m_backgroundShape;
            set
            {
                m_backgroundShape = value;
                m_owner?.UpdateSprites(m_bounds);
            }
        }
        //===========================================================================================
        private int m_radius = 10;
        /// <summary>
        /// Obtient et définit le radius utilisé lorsque enuShape est RoundRect.
        /// </summary>
        [DefaultValue(10), Description("Radius utilisé lorsque BkgShape est RoundRect")]
        public int Radius
        {
            get => m_radius;
            set
            {
                if (value != m_radius)
                {
                    m_radius = value;
                    if (m_owner == null) return;
                    m_owner.UpdateSprites(m_bounds);
                }
            }
        }
        //===========================================================================================
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

        #endregion

        #region Margin

        internal static readonly Padding m_defaultMargin = new(6);
        //===========================================================================================
        private Padding m_margin = m_defaultMargin;
        /// <summary>
        /// Obtient et définit l'espacement externe entre le texte et le fond.
        /// </summary>
        [Browsable(true),
         NotifyParentProperty(true),
         EditorBrowsable(EditorBrowsableState.Always),
         Description("Espacement externe entre le texte et le fond"), Category("Layout")]
        public Padding Margin
        {
            get => m_margin;
            set
            {
                if (value != m_margin)
                {
                    m_margin = value;
                    if (m_owner == null) return;
                    RecalcBoundsAndRedraw();
                }
            }
        }
        private void ResetMargin()
        {
            Margin = m_defaultMargin;
        }
        private bool ShouldSerializeMargin()
        {
            return m_margin != m_defaultMargin;
        }

        #endregion

        #region Constructeur
        /// <summary>
        /// Initialise un 'TextSprite'
        /// </summary>
        public TextSprite()
        {
        }
        #endregion

        #region Draw, DrawAtOrigin
        /// <summary>
        /// Dessine le fond ainsi que le texte du 'Sprite'
        /// </summary>
        /// <param name="pGraphics"></param>
        public override void Draw(Graphics pGraphics)
        {
            if (m_visible)
            {
                base.Draw(pGraphics);
                int posX = m_bounds.Left + m_margin.Left;
                int posY = m_bounds.Top + m_margin.Top;
                switch (m_backgroundStyle)
                {
                    case enuBkgStyle.None:
                        break;
                    case enuBkgStyle.Border:
                        break;
                    case enuBkgStyle.FillShape:
                        FillShapeElement.DrawFillShape(m_backgroundShape, pGraphics, m_bounds, m_shapeColor, m_opacity, m_radius);
                        break;
                    case enuBkgStyle.Shape:
                        ShapeElement.DrawShape(m_backgroundShape, pGraphics, m_bounds, new Pen(m_shapeColor), m_radius);
                        break;
                    case enuBkgStyle.Image:
                        //pGraphics.DrawImage
                        break;
                }
                pGraphics.DrawString(m_text, m_font, new SolidBrush(Color.FromArgb(m_opacity, m_textColor)), posX, posY);
            }
        }
        //===========================================================================
        /// <summary>
        /// Dessine le Sprite à la coordonnée 0,0 dans le graphics
        /// </summary>
        /// <param name="pGraphics">Destination du dessin</param>
        public override void DrawAtOrigin(Graphics pGraphics)
        {
            SizeF pt = pGraphics.MeasureString(m_text, m_font);
            Rectangle contour = m_bounds with { X = 0, Y = 0 };
            FillShapeElement.DrawFillShape(m_backgroundShape, pGraphics, contour, m_shapeColor, m_opacity, m_radius);
            int posX = (m_bounds.Width - (int)pt.Width) >> 1;
            int posY = (m_bounds.Height - (int)pt.Height) >> 1;
            pGraphics.DrawString(m_text, m_font, new SolidBrush(Color.FromArgb(m_opacity, m_textColor)), posX, posY);
        }
        #endregion

        /// <summary>
        /// Calcul le rectangle contour du Sprite en fonction de son alignement et de son DisplayIndex
        /// </summary>
        protected override void RecalcBounds()
        {
            Graphics graphique = m_owner.CreateGraphics();
            SizeF pt = graphique.MeasureString(m_text, m_font);

            if (m_displayIndex == -1)
            {
                if (AlignOnGrid)
                {
                    Size taille = new((int)pt.Width + m_margin.Size.Width, (int)pt.Height + m_margin.Size.Height);
                    m_bounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(m_owner.GridBounds, taille, m_alignment);
                }
                else
                {
                    m_bounds = new Rectangle(m_location, new Size((int)pt.Width, (int)pt.Height));
                    m_bounds.Width += m_margin.Size.Width;
                    m_bounds.Height += m_margin.Size.Height;
                    m_bounds.X -= m_margin.Left;
                    m_bounds.Y -= m_margin.Top;
                }
            }
            else
            {
                Size taille = new((int)pt.Width + m_margin.Size.Width, (int)pt.Height + m_margin.Size.Height);
                Rectangle cellBounds = m_owner.GetCellBounds(m_displayIndex);
                m_bounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(cellBounds, new Size(taille.Width, taille.Height), m_alignment);
            }
            m_bounds.Offset(m_offsetX, m_offsetY);
        }
    }
}


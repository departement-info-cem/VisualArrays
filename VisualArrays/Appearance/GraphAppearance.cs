using System.ComponentModel;
using VisualArrays.Others;

namespace VisualArrays.Appearance
{
    #region GraphAppearance

    /// <summary>
    /// Fournit les informations concernant l'apparence en mode graphique
    /// </summary>
    [TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence en mode graphique")]
    public class GraphAppearance
    {
        private Control m_owner;
        /// <summary>
        /// Initialise un GraphAppearance object
        /// </summary>
        /// <param name="pOwner">VisualArray propriétaire du CellsAppearance</param>
        public GraphAppearance(Control pOwner)
        {
            m_owner = pOwner;
        }
        //============================================================================================
        private Color m_barValueColor = Color.Silver;
        /// <summary>
        /// Obtient ou définit la couleur utilisée pour la valeur de la barre du graphique
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(typeof(Color), "Silver"),
        Description("Couleur de la valeur de la barre du graphique")]
        public Color BarValueColor
        {
            get => m_barValueColor;
            set
            {
                if (value != m_barValueColor)
                {
                    m_barValueColor = value;
                    m_owner.Refresh();
                }
            }
        }
        //============================================================================================
        private static readonly Font m_defaultFont = new Font("Arial", 10);
        private Font m_barValueFont = m_defaultFont;
        /// <summary>
        /// Obtient ou définit la police utilisée pour la valeur de la barre du graphique
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        Description("Police utilisée pour la valeur de la barre du graphique")]
        public Font BarValueFont
        {
            get => m_barValueFont;
            set
            {
                if (value != m_barValueFont)
                {
                    m_barValueFont = value;
                    m_owner.Refresh();
                }
            }
        }
        private void ResetBarValueFont()
        {
            BarValueFont = m_defaultFont;
        }
        private bool ShouldSerializeBarValueFont()
        {
            return !m_barValueFont.Equals(m_defaultFont);
        }
        //============================================================================================
        private enuGraphValueStyle m_barValueStyle = enuGraphValueStyle.None;
        /// <summary>
        /// Obtient ou définit le style de la valeur affichée avec la barre.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(enuGraphValueStyle.None),
        Description("Style de la valeur de la barre du graphique")]
        public enuGraphValueStyle BarValueStyle
        {
            get => m_barValueStyle;
            set
            {
                if (m_barValueStyle != value)
                {
                    m_barValueStyle = value;
                    m_owner.Refresh();
                }
            }
        }
        //============================================================================================
        private Padding m_barMargin;
        /// <summary>
        /// Obtient ou définit la marge autour de la barre du graphique.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        Description("Marge autour de la barre du graphique")]
        public Padding BarMargin
        {
            get => m_barMargin;
            set
            {
                if (m_barMargin != value)
                {
                    m_barMargin = value;
                    m_owner.Refresh();
                }
            }
        }
        private void ResetBarMargin()
        {
            m_barMargin = new Padding(0, 0, 0, 0);
            m_owner.Refresh();
        }
        private bool ShouldSerializeBarMargin()
        {
            return m_barMargin != new Padding(0, 0, 0, 0);
        }
        //============================================================================================
        private enuGraphBarStyle m_barStyle = enuGraphBarStyle.FillShape;
        /// <summary>
        /// Obtient et définit le style de la barre du graphique.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(enuGraphBarStyle.FillShape),
        Description("Style de la barre du graphique")]
        public enuGraphBarStyle BarStyle
        {
            get => m_barStyle;
            set
            {
                if (value != m_barStyle)
                {
                    m_barStyle = value;
                    m_owner.Refresh();
                }
            }
        }
        //============================================================================================
        private Color m_barColor = Color.Lime;
        /// <summary>
        /// Obtient et définit la couleur de la barre du graphique
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(typeof(Color), "Lime"),
        Description("Couleur de la barre du graphique")]
        public Color BarColor
        {
            get => m_barColor;
            set
            {
                if (value != m_barColor)
                {
                    m_barColor = value;
                    m_owner.Refresh();
                }
            }
        }
        //============================================================================================
        private Image m_barImage = null;
        /// <summary>
        /// Obtient ou définit l'image utilisée pour la barre du graphique.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(null),
        Description("Image utilisée pour la barre du graphique")]
        public Image BarImage
        {
            get => m_barImage;
            set
            {
                if (value != m_barImage)
                {
                    m_barImage = value;
                    m_owner.Refresh();
                }
            }
        }
        //============================================================================================
        private enuBarAlign m_barBarAlign = enuBarAlign.Normal;
        /// <summary>
        /// Obtient ou définit l'alignement de la barre.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(enuBarAlign.Normal),
        Description("Alignement de la barre du graphique")]
        public enuBarAlign BarAlign
        {
            get => m_barBarAlign;
            set
            {
                if (m_barBarAlign != value)
                {
                    m_barBarAlign = value;
                    m_owner.Refresh();
                }
            }
        }
    }

    #endregion
}

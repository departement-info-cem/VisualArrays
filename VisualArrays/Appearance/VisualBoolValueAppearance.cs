using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using VisualArrays.VisualCells;

namespace VisualArrays
{
    #region VisualBoolValueAppearance

    /// <summary>
    /// Fournit les informations concernant l'apparence.
    /// </summary>
    [TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence du contrôle pour cette valeur")]
    public class VisualBoolValueAppearance:IBackgroundAppearance
    {
        /// <summary>
        /// 
        /// </summary>
        protected VisualValue<bool> m_owner;
        /// <summary>
        /// Initialise un VisualBoolValueAppearance object
        /// </summary>
        /// <param name="pOwner">VisualArray propriétaire du CellsAppearance</param>
        public VisualBoolValueAppearance(VisualValue<bool> pOwner)
        {
            m_owner = pOwner;
        }

        //============================================================================================
        private enuShape m_shape = enuShape.Ellipse;
        /// <summary>
        /// Obtient et définit la forme utilisée pour dessiner le fond.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(enuShape.Ellipse),
        Description("Forme utilisée pour dessiner le fond")] 
        public enuShape Shape
        {
            get { return m_shape; }
            set
            {
                if (value != m_shape)
                {
                    m_shape = value;
                    if (m_style == enuBkgStyle.FillShape || m_style == enuBkgStyle.Shape)
                        m_owner.UpdateVisualElement();
                }

            }
        }
        //============================================================================================
        /// <summary>
        /// Obtient ou définit l'image de fond.
        /// </summary>
        private Image m_image = null;
        /// <summary>
        /// Obtient ou définit l'image de fond.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(null),
        Description("Image utilisée pour dessiner le fond")]
        public Image Image
        {
            get { return m_image; }
            set
            {
                if (value != m_image)
                {
                    m_image = value;
                    if (m_style == enuBkgStyle.Image)
                        m_owner.UpdateVisualElement();
                }
            }
        }
        //===========================================================================================
        private int m_penWidth = 2;
        /// <summary>
        /// Obtient et définit la taille du crayon pour dessiner une forme contour.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(2),
        Description("Taille du crayon pour dessiner une forme contour dans le fond")]
        public int PenWidth
        {
            get { return m_penWidth; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(
                        "PenWidth",
                        value,
                        "doit être >= 1");
                }
                if (m_penWidth != value)
                {
                    m_penWidth = value;
                    if (m_style == enuBkgStyle.Shape)
                        m_owner.UpdateVisualElement();
                }
            }
        }

        //===========================================================================================
        private enuBkgStyle m_style = enuBkgStyle.FillShape;
        /// <summary>
        /// Obtient et définit le style de fond.
        /// </summary>
        [Browsable(true),
         NotifyParentProperty(true),
         EditorBrowsable(EditorBrowsableState.Always),
         DefaultValue(enuBkgStyle.FillShape),
         Description("Style de fond")]
        public enuBkgStyle Style
        {
            get { return m_style; }
            set
            {
                if (m_style != value)
                {
                    m_style = value;
                    m_owner.UpdateVisualElement();
                }
            }
        }
        //===========================================================================================
        private Padding m_border = new Padding(1, 1, 1, 1);
        /// <summary>
        /// Obtient et définit la taille des bordures.
        /// </summary>
        [Browsable(true),
         NotifyParentProperty(true),
         EditorBrowsable(EditorBrowsableState.Always),
         Description("Taille de chacune des bordures")]
        public Padding Border
        {
            get { return m_border; }
            set
            {
                if (value != m_border)
                {
                    m_border = value;
                    if (m_style == enuBkgStyle.Border)
                        m_owner.UpdateVisualElement();
                }
            }
        }
        private void ResetBorder()
        {
            m_border = new Padding(1, 1, 1, 1);
            if (m_style == enuBkgStyle.Border)
                m_owner.UpdateVisualElement();
        }
        private bool ShouldSerializeBorder()
        {
            return m_border != new Padding(1, 1, 1, 1);
        }
        //============================================================================================
        private Color m_backgroundColor = Color.DarkGreen;
        /// <summary>
        /// Obtient et définit la couleur de fond.
        /// </summary>
        [Browsable(true),
         NotifyParentProperty(true),
         EditorBrowsable(EditorBrowsableState.Always),
         DefaultValue(typeof(Color), "DarkGreen"),
         Description("Couleur de fond")]
        public Color BackgroundColor
        {
            get { return m_backgroundColor; }
            set
            {
                if (value != m_backgroundColor)
                {
                    m_backgroundColor = value;
                    if (m_style == enuBkgStyle.Border || m_style == enuBkgStyle.FillShape || m_style == enuBkgStyle.Shape)
                        m_owner.UpdateVisualElement();
                }
            }
        }

        //===========================================================================================
        private int m_radius = 10;
        /// <summary>
        /// Obtient et définit le radius utilisé lorsque enuShape est RoundRect.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(10),
        Description("Radius utilisé lorsque enuShape est RoundRect")]
        public int Radius
        {
            get
            {
                return m_radius;
            }
            set
            {
                if (value != m_radius)
                {
                    m_radius = value;
                    if ((m_style == enuBkgStyle.FillShape || m_style == enuBkgStyle.Shape) && m_shape == enuShape.RoundRect)
                        m_owner.UpdateVisualElement();
                }
            }
        }
    }
    #endregion
}

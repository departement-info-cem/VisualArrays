using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;

namespace VisualArrays
{
    #region SpecialValueAppearance

    /// <summary>
    /// Fournit les informations concernant l'apparence de la valeur spéciale de la grille
    /// </summary>
    [TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence de la valeur spéciale de la grille")]
    public class SpecialValueAppearance<Type>:IBackgroundAppearance
    {
        /// <summary>
        /// VisualArray qui possède cette instance
        /// </summary>
        protected VisualValueArray<Type> m_owner;
        /// <summary>
        /// Initialise un SpecialValueAppearance object
        /// </summary>
        /// <param name="pOwner">VisualArray propriétaire</param>
        public SpecialValueAppearance(VisualValueArray<Type> pOwner)
        {
            m_owner = pOwner;
        }

        //============================================================================================
        private Color m_textColor = Color.Red;
        /// <summary>
        /// Obtient et définit la couleur du texte de la valeur spéciale.
        /// </summary>
        [Browsable(true),
         NotifyParentProperty(true),
         EditorBrowsable(EditorBrowsableState.Always),
         DefaultValue(typeof(Color), "Red"),
         Description("Couleur du texte de la valeur spéciale")]
        public Color TextColor
        {
            get { return m_textColor; }
            set
            {
                if (m_textColor != value)
                {
                    m_textColor = value;
                    m_owner.Refresh();
                }
            }
        }

        //============================================================================================
        private Font m_font = new Font("Arial", 10);
        /// <summary>
        /// Obtient ou définit la police utilisée pour afficher la valeur spéciale.
        /// </summary>
        [Browsable(true),
       NotifyParentProperty(true),
       EditorBrowsable(EditorBrowsableState.Always),
       Description("Police utilisée pour afficher la valeur spéciale")]
        public Font Font
        {
            get { return m_font; }
            set
            {
                if (m_font != value)
                {
                    m_font = value;
                    m_owner.Refresh();
                }
            }
        }
        private void ResetFont()
        {
            m_font = new Font("Arial", 10);
        }
        private bool ShouldSerializeFont()
        {
            return !m_font.Equals(new Font("Arial", 10));
        }


        //===========================================================================================
        private bool m_showValue = false;
        /// <summary>
        /// Indique si la valeur spéciale doit être affiché
        /// </summary>
        [Browsable(true),
         NotifyParentProperty(true),
         EditorBrowsable(EditorBrowsableState.Always),
         DefaultValue(false),
        Description("Indique si la valeur spéciale doit être affiché")]
        public bool ShowValue
        {
            get { return m_showValue; }
            set
            {
                if (value != m_showValue)
                {
                    m_showValue = value;
                    m_owner.Refresh();
                }
            }
        }

        //===========================================================================================
        private bool m_enabled = true;
        /// <summary>
        /// Indique si la cellule doit se comporter comme une cellule active lorsqu'elle contient la SpecialValue
        /// </summary>
        [Browsable(true),
         NotifyParentProperty(true),
         EditorBrowsable(EditorBrowsableState.Always),
         DefaultValue(true),
        Description("Indique si la cellule doit se comporter comme une cellule active lorsqu'elle contient la SpecialValue")]
        public bool Enabled
        {
            get { return m_enabled; }
            set
            {
                if (value != m_enabled)
                {
                    m_enabled = value;
                    m_owner.Refresh();
                }
            }
        }

        //===========================================================================================
        private Padding m_border = new Padding(1, 1, 1, 1);
        /// <summary>
        /// Obtient et définit la taille des bordures pour la valeur spéciale
        /// </summary>
        [Browsable(true),
         NotifyParentProperty(true),
         EditorBrowsable(EditorBrowsableState.Always),
         Description("Taille de chacune des bordures autour des cellules contenant la valeur spéciale")]
        public Padding Border
        {
            get { return m_border; }
            set
            {
                if (value != m_border)
                {
                    m_border = value;
                    if (m_style == enuBkgStyle.Border)
                    {
                        m_owner.UpdateSpecialValueVisualElement(m_style);
                        m_owner.Refresh();
                    }
                }
            }
        }
        private void ResetBorder()
        {
            m_border = new Padding(1, 1, 1, 1);
        }
        private bool ShouldSerializeBorder()
        {
            return m_border != new Padding(1, 1, 1, 1);
        }
        //============================================================================================
        private Color m_backgroundColor = Color.LightGray;
        /// <summary>
        /// Obtient et définit la couleur de la forme ou de la bordure sous la valeur spéciale.
        /// </summary>
        [Browsable(true),
       NotifyParentProperty(true),
       EditorBrowsable(EditorBrowsableState.Always),
       DefaultValue(typeof(Color), "LightGray"),
       Description("Couleur de la forme ou de la bordure sous la valeur spéciale")]
        public virtual Color BackgroundColor
        {
            get { return m_backgroundColor; }
            set
            {
                if (value != m_backgroundColor)
                {
                    m_backgroundColor = value;
                    if (m_style == enuBkgStyle.FillShape || m_style == enuBkgStyle.Shape || m_style == enuBkgStyle.Border)
                    {
                        m_owner.UpdateSpecialValueVisualElement(m_style);
                        m_owner.Refresh();
                    }
                }
            }
        }
        //============================================================================================
        private Image m_image = null;
        /// <summary>
        /// Obtient ou définit l'image affichée derrière la valeur spéciale
        /// </summary>
        [Browsable(true),
      NotifyParentProperty(true),
      EditorBrowsable(EditorBrowsableState.Always),
      DefaultValue(null),
      Description("Image affichée derrière la valeur spéciale")]
        public Image Image
        {
            get { return m_image; }
            set
            {
                if (value != m_image)
                {
                    m_image = value;
                    if (m_owner.InDesignMode && value != null && m_style != enuBkgStyle.Image)
                        Style = enuBkgStyle.Image;
                    else if (m_style == enuBkgStyle.Image)
                    {
                        m_owner.UpdateSpecialValueVisualElement(m_style);
                        m_owner.Refresh();
                    }
                }
            }
        }
        //===========================================================================================
        private enuShape m_shape = enuShape.Rectangle;
        /// <summary>
        /// Obtient ou définit la forme dessinée derrière la valeur spéciale
        /// </summary>
        [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuShape.Rectangle),
     Description("Forme dessinée derrière la valeur spéciale")]
        public enuShape Shape
        {
            get { return m_shape; }
            set
            {
                if (value != m_shape)
                {
                    m_shape = value;
                    if (m_style == enuBkgStyle.FillShape || m_style == enuBkgStyle.Shape)
                    {
                        m_owner.UpdateSpecialValueVisualElement(m_style);
                        m_owner.Refresh();
                    }
                }
            }
        }
        //===========================================================================================
        private int m_penWidth = 2;
        /// <summary>
        /// Obtient ou définit la taille du crayon utilisé pour dessiner la forme derrière la valeur spéciale
        /// </summary>
        [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(2),
     Description("Taille du crayon utilisé  pour dessiner la forme derrière la valeur spéciale")]
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
                if (value != m_penWidth)
                {
                    m_penWidth = value;
                    if (m_style == enuBkgStyle.Shape)
                    {
                        m_owner.UpdateSpecialValueVisualElement(m_style);
                        m_owner.Refresh();
                    }
                }
            }
        }
        //===========================================================================================
        private enuBkgStyle m_style = enuBkgStyle.None;
        /// <summary>
        /// Obtient et définit le style de fond des cellules contenant void.
        /// </summary>
        [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuBkgStyle.None),
     Description("Style du fond des cellules contenant la valeur spéciale")]
        public enuBkgStyle Style
        {
            get { return m_style; }
            set
            {
                if (value != m_style)
                {
                    m_style = value;
                    m_owner.UpdateSpecialValueVisualElement(m_style);
                    m_owner.Refresh();
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
                    {
                        m_owner.UpdateSpecialValueVisualElement(m_style);
                        m_owner.Refresh();
                    }
                }
            }
        }
    }
    #endregion
}

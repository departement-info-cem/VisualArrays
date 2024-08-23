using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;

namespace VisualArrays
{
    #region StrikeAppearance

    /// <summary>
    /// Fournit les informations concernant l'apparence du Strike
    /// </summary>
    [TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence du Strike")]
    public class StrikeAppearance
    {
        /// <summary>
        /// VisualArray qui possède cette instance
        /// </summary>
        protected BaseGrid m_owner;
        /// <summary>
        /// Initialise un StrikeAppearance object
        /// </summary>
        /// <param name="pOwner">VisualArray propriétaire</param>
        public StrikeAppearance(BaseGrid pOwner)
        {
            m_owner = pOwner;
        }

        //===========================================================================================
        private enuStrikeStyle m_style = enuStrikeStyle.None;
        /// <summary>
        /// Obtient et définit le style du trait du Strike
        /// </summary>
        [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuStrikeStyle.None),
     Description("Style du trait du Strike")]
        public enuStrikeStyle Style
        {
            get { return m_style; }
            set
            {
                if (value != m_style)
                {
                    m_style = value;
                    m_owner.Refresh();
                }
            }
        }
        //============================================================================================
        private Color m_color = Color.Red;
        /// <summary>
        /// Obtient et définit la couleur du trait du Strike.
        /// </summary>
        [Browsable(true),
       NotifyParentProperty(true),
       EditorBrowsable(EditorBrowsableState.Always),
       DefaultValue(typeof(Color), "Red"),
       Description("Couleur du trait du Strike")]
        public virtual Color Color
        {
            get { return m_color; }
            set
            {
                if (value != m_color)
                {
                    m_color = value;
                    if (m_style == enuStrikeStyle.Diagonal || m_style == enuStrikeStyle.Cross)
                        m_owner.Refresh();
                }
            }
        }
        //===========================================================================================
        private int m_penWidth = 3;
        /// <summary>
        /// Obtient ou définit la taille du crayon utilisé pour dessiner le trait du Strike
        /// </summary>
        [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(3),
     Description("Taille du crayon utilisé pour dessiner le trait du Strike")]
        public int PenWidth
        {
            get { return m_penWidth; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(
                        "StrikePenWidth",
                        value,
                        "doit être >= 1");
                }
                if (value != m_penWidth)
                {
                    m_penWidth = value;
                    if (m_style != enuStrikeStyle.None)
                        m_owner.Refresh();
                }
            }
        }
        //===========================================================================================
        private int m_margin = 1;
        /// <summary>
        /// Obtient ou définit la marge appliquée au trait
        /// </summary>
        [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(1),
     Description("Marge appliquée au trait")]
        public int Margin
        {
            get { return m_margin; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "Margin",
                        value,
                        "doit être >= 0");
                }
                if (value != m_margin)
                {
                    m_margin = value;
                    if (m_style != enuStrikeStyle.None)
                        m_owner.Refresh();
                }
            }
        }
        //===========================================================================================
        private Image m_image = null;
        /// <summary>
        /// Obtient et définit l'image du trait lorsque le Style du Strike est Image
        /// </summary>
        [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(null),
     Description("Image du trait lorsque le Style du Strike est Image")]
        public Image Image
        {
            get { return m_image; }
            set
            {
                if (value != m_image)
                {
                    m_image = value;
                    m_owner.Refresh();
                }
            }
        }
        //============================================================================================
        /// <summary>
        /// Obtient et définit l'alignement de l'image du Strike.
        /// </summary>
        private ContentAlignment m_align = ContentAlignment.MiddleCenter;
        /// <summary>
        /// Obtient et définit l'alignement de l'image du Strike.
        /// </summary>
        [DefaultValue(ContentAlignment.MiddleCenter), Browsable(true), Description("Alignement de l'image du Strike")]
        public ContentAlignment Align
        {
            get { return m_align; }
            set
            {
                if (value != m_align)
                {
                    m_align = value;
                    m_owner.Refresh();
                }
            }
        }
    }
    #endregion
}

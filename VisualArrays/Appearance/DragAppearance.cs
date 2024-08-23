using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;

namespace VisualArrays
{
    #region DragAppearance

    /// <summary>
    /// Fournit les informations concernant l'apparence de la sélection
    /// </summary>
    [TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence de la destination lors d'une opération glisser/déposer")]
    public class DragAppearance
    {
        internal static readonly Padding m_defaultDragPadding = new Padding(1, 1, 1, 1);
        //===========================================================================================
        private Padding m_padding = m_defaultDragPadding;
        /// <summary>
        /// Obtient et définit l'espacement entre la cellule et la sélection.
        /// </summary>
        [Browsable(true),
         NotifyParentProperty(true),
         EditorBrowsable(EditorBrowsableState.Always),
         Description("Espacement entre le contour de la cellule et la sélection")]
        public Padding Padding
        {
            get { return m_padding; }
            set
            {
                if (value != m_padding)
                {
                    m_padding = value;
                }
            }
        }
        private void ResetPadding()
        {
            m_padding = m_defaultDragPadding;
        }
        private bool ShouldSerializePadding()
        {
            return m_padding != m_defaultDragPadding;
        }


        //============================================================================================
        private Image image = null;
        /// <summary>
        /// Obtient ou définit l'image de fond des cellules.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(null),
        Description("Image utilisée pour représenter la destination de l'opération glisser/déposer")] 
        public Image Image
        {
            get { return image; }
            set {
                if (value != image)
                image = value; 
            }
        }
        //============================================================================================
        private int alpha = 128;
        /// <summary>
        /// Obtient ou définit la transparence appliquée sur la destination.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(128),
        Description("Transparence appliquée à la destination (0 à 255)")]
        public int Alpha
        {
            get { return alpha; }
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException(
                        "Alpha",
                        value,
                        "doit être >= 0 et <= 255");
                }
                if (value != alpha)
                    alpha = value;
            }
        }
        //===========================================================================================
        private enuDragStyle style = enuDragStyle.Shape;
        /// <summary>
        /// Obtient ou définit le style de fond de la sélection.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(enuDragStyle.Shape),
        Description("Style utilisé pour représenter la destination")]
        public enuDragStyle Style
        {
            get { return style; }
            set { 
                if (value != style)
                style = value; 
            }
        }
        //============================================================================================
        private enuShape shape = enuShape.Rectangle;
        /// <summary>
        /// Obtient ou définit la forme utilisée pour représenter la sélection.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(enuShape.Rectangle),
        Description("Forme utilisée pour représenter la destination")]  
        public enuShape Shape
        {
            get { return shape; }
            set {
                if (value != shape)
                shape = value; 
            }
        }
        //============================================================================================
        private int penWidth = 3;
        /// <summary>
        /// Obtient ou définit l'épaisseur du crayon utilisé pour dessiner la forme.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(3),
        Description("Taille du crayon utilisée pour dessiner la forme")]  
        public int PenWidth
        {
            get { return penWidth; }
            set
            {
                if (value < 1) 
                {
                    throw new ArgumentOutOfRangeException(
                        "Size",
                        value,
                        "doit être >= 1");                }
                if (value != penWidth)
                    penWidth = value;
            }
        }
        //============================================================================================
        private Color color = Color.Green;
        /// <summary>
        /// Obtient ou définit la couleur de la forme utilisée pour représenter la destination.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(typeof(Color), "Green"),
        Description("Couleur de la destination")]        
        public Color Color
        {
            get { return color; }
            set {
                if (value != color) 
                color = value; 
            }
        }

        //============================================================================================
        private bool showSource = true;
        /// <summary>
        /// Obtient ou définit si on doit mettre en évidence la cellule source de l'opération glisser/déposer.
        /// </summary>
        [Browsable(true),
        NotifyParentProperty(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(true),
        Description("Indique si on doit mettre en évidence la cellule source de l'opération glisser/déposer")]
        public bool ShowSource
        {
            get { return showSource; }
            set
            {
                if (value != showSource)
                    showSource = value;
            }
        }
        //===========================================================================================
        private int radius = 10;
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
                return radius;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(
                        "Radius",
                        value,
                        "doit être >= 1");
                }
                if (value != radius)
                    radius = value;
            }
        }
    }

    #endregion
}

using System.ComponentModel;
using VisualArrays.Others;

namespace VisualArrays.Appearance;

#region SelectionAppearance

/// <summary>
/// Fournit les informations concernant l'apparence de la sélection
/// </summary>
[TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence de la sélection")]
public class SelectionAppearance
{
    internal static readonly Padding m_defaultSelectionPadding = new(1, 1, 1, 1);
    //===========================================================================================
    private Padding m_padding = m_defaultSelectionPadding;
    /// <summary>
    /// Obtient et définit l'espacement entre la cellule et la sélection.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     Description("Espacement entre le contour de la cellule et la sélection")]
    public Padding Padding
    {
        get => m_padding;
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
        m_padding = m_defaultSelectionPadding;
    }
    private bool ShouldSerializePadding()
    {
        return m_padding != m_defaultSelectionPadding;
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
     Description("Image utilisée pour la sélection")] 
    public Image Image
    {
        get => image;
        set {
            if (value != image)
            {
                image = value;
            }
        }
    }
    //===========================================================================================
    private enuSelectionStyle style = enuSelectionStyle.Shape;
    /// <summary>
    /// Obtient ou définit le style de fond de la sélection.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuSelectionStyle.Shape),
     Description("Style utilisé pour la sélection")]  
    public enuSelectionStyle Style
    {
        get => style;
        set { 
            if (value != style)
            {
                style = value;
            }
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
     Description("Forme utilisée pour la sélection")]  
    public enuShape Shape
    {
        get => shape;
        set {
            if (value != shape)
            {
                shape = value;
            }
        }
    }
    //============================================================================================
    private int penWidth = 3;
    /// <summary>
    /// Obtient ou définit l'épaisseur du crayon utilisé pour dessiner la forme de la sélection.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(3),
     Description("Taille du crayon utilisée pour dessiner la sélection")]  
    public int PenWidth
    {
        get => penWidth;
        set
        {
            if (value < 1) 
            {
                throw new ArgumentOutOfRangeException(
                    "Size",
                    value,
                    "doit être >= 1");                }
            if (value != penWidth)
            {
                penWidth = value;
            }
        }
    }
    //============================================================================================
    private Color color = Color.Blue;
    /// <summary>
    /// Obtient ou définit la couleur de la forme de la sélection.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(typeof(Color), "Blue"),
     Description("Couleur de la sélection")]        
    public Color Color
    {
        get => color;
        set {
            if (value != color)
            {
                color = value;
            }
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
        get => radius;
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
            {
                radius = value;
            }
        }
    }
}

#endregion
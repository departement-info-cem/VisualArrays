using System.ComponentModel;
using VisualArrays.Others;

namespace VisualArrays.Appearance;

#region VBACellAppearance

/// <summary>
/// Fournit les informations concernant l'apparence des cellules dont l'état Enabled est true
/// </summary>
[TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence des cellules dont l'état Enabled est true")]
public class VBACellAppearance:IBackgroundAppearance
{
    /// <summary>
    /// Initialise un BackgroundAppearance object
    /// </summary>
    protected BaseGrid m_owner;
    /// <summary>
    /// Initialise un VBACellAppearance object
    /// </summary>
    /// <param name="pOwner">VisualArray propriétaire du CellsAppearance</param>
    public VBACellAppearance(BaseGrid pOwner)
    {
        m_owner = pOwner;
    }

    //============================================================================================
    private enuShape m_shape = enuShape.Ellipse;
    /// <summary>
    /// Obtient et définit la forme utilisée pour dessiner le fond de toutes les cellules.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuShape.Ellipse),
     Description("Forme utilisée pour dessiner le fond des cellules")] 
    public enuShape Shape
    {
        get => m_shape;
        set
        {
            if (value != m_shape)
            {
                m_shape = value;
                if (m_style == enuBkgStyle.FillShape || m_style == enuBkgStyle.Shape)
                    m_owner.UpdateCellsBkgVisualElement();
            }

        }
    }
    //============================================================================================
    /// <summary>
    /// Obtient ou définit l'image de fond des cellules.
    /// </summary>
    private Image m_image = null;
    /// <summary>
    /// Obtient ou définit l'image de fond des cellules.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(null),
     Description("Image utilisée pour le fond des cellules")]
    public Image Image
    {
        get => m_image;
        set
        {
            if (value != m_image)
            {
                m_image = value;
                if (m_style == enuBkgStyle.Image)
                    m_owner.UpdateCellsBkgVisualElement();
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
     Description("Taille du crayon pour dessiner une forme contour dans le fond des cellules")]
    public int PenWidth
    {
        get => m_penWidth;
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
                    m_owner.UpdateCellsBkgVisualElement();
            }
        }
    }

    //===========================================================================================
    private enuBkgStyle m_style = enuBkgStyle.FillShape;
    /// <summary>
    /// Obtient et définit le style de fond des cellules.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuBkgStyle.FillShape),
     Description("Style de fond des cellules")]
    public enuBkgStyle Style
    {
        get => m_style;
        set
        {
            if (m_style != value)
            {
                m_style = value;
                m_owner.UpdateCellsBkgVisualElement();
            }
        }
    }

    //===========================================================================================
    private Padding m_border = new(1, 1, 1, 1);
    /// <summary>
    /// Obtient et définit la taille des bordures autour des cellules.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     Description("Taille de chacune des bordures autour des cellules")]
    public Padding Border
    {
        get => m_border;
        set
        {
            if (value != m_border)
            {
                m_border = value;
                if (m_style == enuBkgStyle.Border)
                    m_owner.UpdateCellsBkgVisualElement();
            }
        }
    }
    private void ResetBorder()
    {
        m_border = new Padding(1, 1, 1, 1);
        if (m_style == enuBkgStyle.Border)
            m_owner.UpdateCellsBkgVisualElement();
    }
    private bool ShouldSerializeBorder()
    {
        return m_border != new Padding(1, 1, 1, 1);
    }
    //============================================================================================
    private Color m_backgroundColor = Color.DarkGreen;
    /// <summary>
    /// Obtient et définit la couleur de fond des cellules.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(typeof(Color), "DarkGreen"),
     Description("Couleur de fond des cellules")]
    public Color BackgroundColor
    {
        get => m_backgroundColor;
        set
        {
            if (value != m_backgroundColor)
            {
                m_backgroundColor = value;
                if (m_style == enuBkgStyle.Border || m_style == enuBkgStyle.FillShape || m_style == enuBkgStyle.Shape)
                    m_owner.UpdateCellsBkgVisualElement();
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
        get => m_radius;
        set
        {
            if (value != m_radius)
            {
                m_radius = value;
                if ((m_style == enuBkgStyle.FillShape || m_style == enuBkgStyle.Shape) && m_shape == enuShape.RoundRect)
                    m_owner.UpdateCellsBkgVisualElement();
            }
        }
    }
}
#endregion
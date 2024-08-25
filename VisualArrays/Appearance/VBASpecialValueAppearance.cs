using System.ComponentModel;
using VisualArrays.Others;
using VisualArrays.VisualArrays;

namespace VisualArrays.Appearance;

#region VBASpecialValueAppearance

/// <summary>
/// Fournit les informations concernant l'apparence de la valeur spéciale de la grille
/// </summary>
[TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence de la valeur spéciale de la grille")]
public class VBASpecialValueAppearance<Type>:IBackgroundAppearance
{
    /// <summary>
    /// VisualArray qui possède cette instance
    /// </summary>
    protected VisualValueArray<Type> m_owner;
    /// <summary>
    /// Initialise un SpecialValueAppearance object
    /// </summary>
    /// <param name="pOwner">VisualArray propriétaire</param>
    public VBASpecialValueAppearance(VisualValueArray<Type> pOwner)
    {
        m_owner = pOwner;
    }

    //===========================================================================================
    private Padding m_border = new(1, 1, 1, 1);
    /// <summary>
    /// Obtient et définit la taille des bordures pour la valeur spéciale
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     Description("Taille de chacune des bordures autour des cellules contenant la valeur spéciale")]
    public Padding Border
    {
        get => m_border;
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
    private Color m_backgroundColor = Color.Lime;
    /// <summary>
    /// Obtient et définit la couleur de la forme ou de la bordure sous la valeur spéciale.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(typeof(Color), "Lime"),
     Description("Couleur de la forme ou de la bordure sous la valeur spéciale")]
    public virtual Color BackgroundColor
    {
        get => m_backgroundColor;
        set
        {
            if (value != m_backgroundColor)
            {
                m_backgroundColor = value;
                if (m_style is enuBkgStyle.FillShape or enuBkgStyle.Shape or enuBkgStyle.Border)
                {
                    m_owner.UpdateSpecialValueVisualElement(m_style);
                    m_owner.Refresh();
                }
            }
        }
    }

    //===========================================================================================
    private bool m_enabled = true;
    /// <summary>
    /// Indique si la cellule doit se comporter comme une cellule inactive lorsqu'elle contient la SpecialValue
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(true),
     Description("Indique si la cellule doit se comporter comme une cellule inactive lorsqu'elle contient la SpecialValue")]
    public bool Enabled
    {
        get => m_enabled;
        set
        {
            if (value != m_enabled)
            {
                m_enabled = value;
                m_owner.Refresh();
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
        get => m_image;
        set
        {
            if (value != m_image)
            {
                m_image = value;
                if (m_style == enuBkgStyle.Image)
                {
                    m_owner.UpdateSpecialValueVisualElement(m_style);
                    m_owner.Refresh();
                }
            }
        }
    }
    //===========================================================================================
    private enuShape m_shape = enuShape.Ellipse;
    /// <summary>
    /// Obtient ou définit la forme dessinée derrière la valeur spéciale
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuShape.Ellipse),
     Description("Forme dessinée derrière la valeur spéciale")]
    public enuShape Shape
    {
        get => m_shape;
        set
        {
            if (value != m_shape)
            {
                m_shape = value;
                if (m_style is enuBkgStyle.FillShape or enuBkgStyle.Shape)
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
    private enuBkgStyle m_style = enuBkgStyle.FillShape;
    /// <summary>
    /// Obtient et définit le style de fond des cellules contenant void.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuBkgStyle.FillShape),
     Description("Style du fond des cellules contenant la valeur spéciale")]
    public enuBkgStyle Style
    {
        get => m_style;
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
        get => m_radius;
        set
        {
            if (value != m_radius)
            {
                m_radius = value;
                if (m_style is enuBkgStyle.FillShape or enuBkgStyle.Shape && m_shape == enuShape.RoundRect)
                    m_owner.UpdateCellsBkgVisualElement();
            }
        }
    }
}
#endregion
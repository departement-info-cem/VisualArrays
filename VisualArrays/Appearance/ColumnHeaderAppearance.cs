using System.ComponentModel;
using VisualArrays.Others;

namespace VisualArrays.Appearance;

#region ColumnHeaderAppearance

/// <summary>
/// Fournit les informations concernant l'apparence de l'en-tête des colonnes
/// </summary>
[TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence de l'en-tête des colonnes")]
public class ColumnHeaderAppearance
{
    private BaseGrid m_owner;
    /// <summary>
    /// Initialise un ColumnHeaderAppearance object
    /// </summary>
    /// <param name="pOwner">VisualArray propriétaire du CellsAppearance</param>
    public ColumnHeaderAppearance(BaseGrid pOwner)
    {
        m_owner = pOwner;
    }
    //============================================================================================
    private static readonly Font m_defaultFont = new("Arial", 10);
    private Font m_font = m_defaultFont;
    /// <summary>
    /// Obtient ou définit la police pour l'en-tête des colonnes.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     Description("Police du texte de l'en-tête des colonnes")]
    public Font Font
    {
        get => m_font;
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
        Font = m_defaultFont;
    }
    private bool ShouldSerializeFont()
    {
        return !m_font.Equals(m_defaultFont);
    }
    //============================================================================================
    private Color m_foreColor = Color.White;
    /// <summary>
    /// Obtient ou définit la couleur du texte pour l'en-tête des colonnes.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(typeof(Color), "White"),
     Description("Couleur du texte pour l'en-tête des colonnes")]
    public Color ForeColor
    {
        get => m_foreColor;
        set
        {
            if (value != m_foreColor)
            {
                m_foreColor = value;
                m_owner.Refresh();
            }
        }
    }
    //============================================================================================
    private Color m_backgroundColor = Color.DarkGreen;
    /// <summary>
    /// Obtient ou définit la couleur de fond des cellules d'en-tête de colonnes.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(typeof(Color), "DarkGreen"),
     Description("Couleur de fond des cellules d'en-tête de colonnes")]
    public Color BackgroundColor
    {
        get => m_backgroundColor;
        set
        {
            if (value != m_backgroundColor)
            {
                m_backgroundColor = value;
                m_owner.Refresh();
            }
        }
    }

    //============================================================================================
    private enuHeaderBkgStyle m_style = enuHeaderBkgStyle.Fill;
    /// <summary>
    /// Obtient ou définit le style de fond des cellules d'en-tête de colonnes.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuHeaderBkgStyle.Fill),
     Description("Style de fond des cellules d'en-tête de colonnes")]
    public enuHeaderBkgStyle Style
    {
        get => m_style;
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
    private int m_height = 20;
    /// <summary>
    /// Obtient ou définit la hauteur de l'en-tête des colonnes.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(20),
     Description("Hauteur de l'en-tête des colonnes")]
    public int Height
    {
        get => m_height;
        set
        {
            if (value != m_height)
            {
                m_height = value;
                if (m_visible)
                {
                    m_owner.va_enteteColHaut = m_height;
                    m_owner.ReCalculerTaille();
                    m_owner.Refresh();
                }
            }
        }
    }

    //============================================================================================
    private enuDataStyle m_valueStyle = enuDataStyle.Index;
    /// <summary>
    /// Obtient ou définit le style du contenu de l'en-tête des colonnes.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuDataStyle.Index),
     Description("Style des valeurs de l'en-tête des colonnes")]
    public enuDataStyle ValueStyle
    {
        get => m_valueStyle;
        set
        {
            if (value != m_valueStyle)
            {
                m_valueStyle = value;
                m_owner.Refresh();
            }
        }
    }

    //===========================================================================================
    private bool m_visible = false;
    /// <summary>
    /// Indique si l'en-tête est visible ou non
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(false),
     Description("Indique si l'en-tête est visible ou non")]
    public bool Visible
    {
        get => m_visible;
        set
        {
            if (value != m_visible)
            {
                m_visible = value;
                if (m_visible)
                    m_owner.va_enteteColHaut = m_height;
                else
                    m_owner.va_enteteColHaut = 0;
                m_owner.ReCalculerTaille();
                m_owner.Refresh();
            }
        }
    }
}

#endregion
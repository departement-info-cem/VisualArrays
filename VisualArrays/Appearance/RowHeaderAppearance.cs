using System.ComponentModel;
using VisualArrays.Others;

namespace VisualArrays.Appearance;

#region RowHeaderAppearance

/// <summary>
/// Fournit les informations concernant l'apparence de l'en-tête des rangées
/// </summary>
[TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence de l'en-tête des rangées")]
public class RowHeaderAppearance
{
    private BaseGrid m_owner;
    /// <summary>
    /// Initialise un RowHeaderAppearance object
    /// </summary>
    /// <param name="pOwner">VisualArray propriétaire du CellsAppearance</param>
    public RowHeaderAppearance(BaseGrid pOwner)
    {
        m_owner = pOwner;
    }
    //============================================================================================
    private Font m_font = new("Arial", 10);
    /// <summary>
    /// Obtient ou définit la police pour l'en-tête des rangées.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     Description("Police du texte de l'en-tête des rangées")]
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
        m_font = new Font("Arial", 10);
    }
    private bool ShouldSerializeFont()
    {
        return !m_font.Equals(new Font("Arial", 10));
    }
    //============================================================================================
    private Color m_foreColor = Color.White;
    /// <summary>
    /// Obtient ou définit la couleur du texte pour l'en-tête des rangées.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(typeof(Color), "Blue"),
     Description("Couleur du texte pour l'en-tête des rangées")]
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
    /// Obtient ou définit la couleur de fond des cellules d'en-tête de rangées.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(typeof(Color), "DarkGreen"),
     Description("Couleur de fond des cellules d'en-tête de rangées")]
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
    /// Obtient ou définit le style de fond des cellules d'en-tête de rangées.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuHeaderBkgStyle.Fill),
     Description("Style de fond des cellules d'en-tête de rangées")]
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
    private int m_width = 20;
    /// <summary>
    /// Obtient ou définit la largeur de l'en-tête des rangées.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(20),
     Description("Largeur de l'en-tête des rangées")]
    public int Width
    {
        get => m_width;
        set
        {
            if (value != Width)
            {
                m_width = value;
                if (m_visible)
                {
                    m_owner.va_enteteLgnLarg = m_width;
                    m_owner.ReCalculerTaille();
                    m_owner.Refresh();
                }
            }
        }
    }

    //============================================================================================
    private enuDataStyle m_valueStyle = enuDataStyle.Index;
    /// <summary>
    /// Obtient ou définit le style du contenu de l'en-tête des rangées.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuDataStyle.Index),
     Description("Style des valeurs de l'en-tête des rangées")]
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
                    m_owner.va_enteteLgnLarg = m_width;
                else
                    m_owner.va_enteteLgnLarg = 0;
                m_owner.ReCalculerTaille();
                m_owner.Refresh();
            }
        }
    }
}

#endregion
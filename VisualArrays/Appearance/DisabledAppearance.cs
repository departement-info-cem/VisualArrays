using System.ComponentModel;
using VisualArrays.Others;

namespace VisualArrays.Appearance;

#region DisabledAppearance

/// <summary>
/// Fournit les informations concernant l'apparence d'une cellule dont l'état Enabled est false
/// </summary>
[TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est false")]
public class DisabledAppearance : IBackgroundAppearance
{
    /// <summary>
    /// VisualArray qui possède cette instance
    /// </summary>
    protected BaseGrid m_owner;
    /// <summary>
    /// Initialise un DisabledAppearance object
    /// </summary>
    /// <param name="pOwner">VisualArray propriétaire</param>
    public DisabledAppearance(BaseGrid pOwner)
    {
        m_strikeAppearance = new global::VisualArrays.Appearance.StrikeAppearance(pOwner);
        m_owner = pOwner;
    }

    //===========================================================================================
    private Padding m_border = new Padding(1, 1, 1, 1);
    /// <summary>
    /// Obtient et définit la taille des bordures autour d'une cellule Enabled false
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     Description("Taille de chacune des bordures autour d'une cellule Enabled false")]
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
                    m_owner.UpdateDisableVisualElement(m_style);
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
    private Color m_backgroundColor = Color.DarkGray;
    /// <summary>
    /// Obtient et définit la couleur de la forme ou de la bordure d'une cellule Enabled false.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(typeof(Color), "DarkGray"),
     Description("Couleur de la forme ou de la bordure d'une cellule Enabled false")]
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
                    m_owner.UpdateDisableVisualElement(m_style);
                    m_owner.Refresh();
                }
            }
        }
    }
    //============================================================================================
    private Image m_image = null;
    /// <summary>
    /// Obtient ou définit l'image affichée derrière une cellule Enabled false
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(null),
     Description("Image affichée derrière une cellule Enabled false")]
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
                    m_owner.UpdateDisableVisualElement(m_style);
                    m_owner.Refresh();
                }
            }
        }
    }
    //===========================================================================================
    private enuShape m_shape = enuShape.Rectangle;
    /// <summary>
    /// Obtient ou définit la forme dessinée derrière une cellule Enabled false
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuShape.Rectangle),
     Description("Forme dessinée derrière une cellule Enabled false")]
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
                    m_owner.UpdateDisableVisualElement(m_style);
                    m_owner.Refresh();
                }
            }
        }
    }
    //===========================================================================================
    private int m_penWidth = 2;
    /// <summary>
    /// Obtient ou définit la taille du crayon utilisé pour dessiner la forme derrière une cellule Enabled false
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(2),
     Description("Taille du crayon utilisé  pour dessiner la forme derrière une cellule Enabled false")]
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
                    m_owner.UpdateDisableVisualElement(m_style);
                    m_owner.Refresh();
                }
            }
        }
    }
    //===========================================================================================
    private enuBkgStyle m_style = enuBkgStyle.None;
    /// <summary>
    /// Obtient et définit le style de fond pour une cellule Enabled false
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(enuBkgStyle.None),
     Description("Style du fond pour une cellule Enabled false")]
    public enuBkgStyle Style
    {
        get { return m_style; }
        set
        {
            if (value != m_style)
            {
                m_style = value;
                m_owner.UpdateDisableVisualElement(m_style);
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
                    m_owner.UpdateCellsBkgVisualElement();
            }
        }
    }
    //===========================================================================================
    private StrikeAppearance m_strikeAppearance;
    /// <summary>
    /// Obtient et définit différents spects de l'apparence du Strike.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
     Description("Détermine différents aspects de l'apparence du Strike")]
    public StrikeAppearance StrikeAppearance
    {
        get { return m_strikeAppearance; }
        set
        {
            if (value != m_strikeAppearance)
            {
                m_strikeAppearance = value;
                m_owner.Refresh();
            }
        }
    }
}
#endregion
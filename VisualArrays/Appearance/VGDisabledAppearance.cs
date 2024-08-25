using System.ComponentModel;
using VisualArrays.Others;
using VisualArrays.VisualArrays;

namespace VisualArrays.Appearance;

#region VGDisabledAppearance : DisabledAppearance

/// <summary>
/// Fournit les informations concernant l'apparence d'une cellule dont l'état Enabled est false
/// </summary>
[TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est false")]
public class VGDisabledAppearance<Type> : DisabledAppearance
{
    /// <summary>
    /// Initialise un SpecialValueAppearance object
    /// </summary>
    /// <param name="pOwner">VisualArray propriétaire</param>
    public VGDisabledAppearance(VisualValueArray<Type> pOwner)
        : base(pOwner)
    {
    }

    //============================================================================================
    /// <summary>
    /// Obtient ou définit l'ImageList utilisée pour dessiner la cellule en mode View ImageList
    /// </summary>
    private ImageList m_imageList = null;
    /// <summary>
    /// Obtient ou définit l'ImageList utilisée pour dessiner la cellule en mode View ImageList
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(null),
     Description("ImageList utilisée pour dessiner la cellule en mode View ImageList")]
    public ImageList ImageList
    {
        get => m_imageList;
        set
        {
            if (value != m_imageList)
            {
                m_imageList = value;
                if (m_owner.InDesignMode && value != null && m_owner is VisualIntArray)
                {
                    VisualIntArray grille = (VisualIntArray)m_owner;
                    grille.View = enuIntView.ImageList;
                }
                else
                    m_owner.Refresh();
            }
        }
    }
    //============================================================================================
    private Color m_textColor = Color.Black;
    /// <summary>
    /// Obtient et définit la couleur de la valeur d'une cellule Enabled false.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(typeof(Color), "Black"),
     Description("Couleur du texte de la valeur d'une cellule Enabled false")]
    public Color TextColor
    {
        get => m_textColor;
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
    private Font m_font = new("Arial", 10);
    /// <summary>
    /// Obtient ou définit la police utilisée pour afficher la valeur d'une cellule Enabled false.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     Description("Police utilisée pour afficher la valeur d'une cellule Enabled false")]
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

    //===========================================================================================
    private bool m_showValue = true;
    /// <summary>
    /// Indique si la valeur doit être affichée lorsque la cellule est Enabled false
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(true),
     Description("Indique si la valeur doit être affichée lorsque la cellule est Enabled false")]
    public bool ShowValue
    {
        get => m_showValue;
        set
        {
            if (value != m_showValue)
            {
                m_showValue = value;
                m_owner.Refresh();
            }
        }
    }

    //============================================================================================
    private float m_imageBrightness = 0.2f;
    /// <summary>
    /// Obtient et définit une valeur entre 0 et 1 controlant la brillance
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(0.2f),
     Description("Une valeur entre 0 et 1 controlant la brillance")]
    public float ImageBrightness
    {
        get => m_imageBrightness;
        set
        {
            switch (value)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(
                        "ImageBrightness",
                        value,
                        "doit être >= 0");
                case > 1:
                    throw new ArgumentOutOfRangeException(
                        "ImageBrightness",
                        value,
                        "doit être <= 1");
            }

            if (m_imageBrightness != value)
            {
                m_imageBrightness = value;
                m_owner.Refresh();
            }
        }
    }
}
#endregion
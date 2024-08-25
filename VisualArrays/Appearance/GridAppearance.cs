using System.ComponentModel;

namespace VisualArrays.Appearance;

#region GridAppearance

/// <summary>
/// Fournit les informations concernant l'apparence du quadrillage de la grille
/// </summary>
[TypeConverter(typeof(AppearanceConverter)), Description("Détermine différents aspects de l'apparence du quadrillage")]
public class GridAppearance
{
    private BaseGrid m_owner;
    /// <summary>
    /// Initialise un GridAppearance object
    /// </summary>
    /// <param name="pOwner">VisualArray propriétaire du GridAppearance</param>
    public GridAppearance(BaseGrid pOwner)
    {
        m_owner = pOwner;
    }
    //============================================================================================
    private Color m_color = Color.Gray;
    /// <summary>
    /// Obtient ou définit la couleur du quadrillage de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(typeof(Color), "Gray"),
     Description("Couleur du quadrillage de la grille")]
    public Color Color
    {
        get { return m_color; }
        set
        {
            if (value != m_color)
            {
                m_color = value;
                m_owner.Refresh();
            }
        }
    }
    //============================================================================================
    private int m_lineSize = 1;
    /// <summary>
    /// Obtient ou définit l'épaisseur des lignes du quadrillage de la grille.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     DefaultValue(1),
     Description("Épaisseur des lignes du quadrillage de la grille")]
    public int LineSize
    {
        get { return m_lineSize; }
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "LineSize",
                    value,
                    "doit être >= 0");
            }

            if (value != m_lineSize)
            {
                m_lineSize = value;

                m_owner.ReCalculerTaille();
                m_owner.Refresh();
            }
        }
    }
}

#endregion
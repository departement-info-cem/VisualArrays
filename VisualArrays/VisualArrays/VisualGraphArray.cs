using System.ComponentModel;
using VisualArrays.Appearance;

namespace VisualArrays.VisualArrays;

/// <summary>
/// Cette classe représente les propriétés nécessaire pour afficher des graphiques
/// </summary>
public abstract partial class VisualGraphArray<Type> : VisualValueArray<Type>
{
    #region Code pour la gestion d'un click dans une cellule en mode graphique

    //============================================================================================
    /// <summary>
    /// Indique si le contrôle permet la saisie d'une valeur directement en cliquant dans la cellule
    /// </summary>
    protected bool va_allowGraphClick = true;
    /// <summary>
    /// Indique si le contrôle permet la saisie d'une valeur directement en cliquant dans la cellule
    /// </summary>
    [DefaultValue(true), Category("VisualArrays"), Browsable(true), Description("Indique si le contrôle permet d'utiliser l'opération glisser.")]
    public bool AllowGraphClick
    {
        get { return va_allowGraphClick; }
        set { va_allowGraphClick = value; }
    }
    //============================================================================================
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pIndex"></param>
    /// <param name="pPixelOffset"></param>
    internal abstract void SetValue(int pIndex, int pPixelOffset);
    #endregion

    #region GraphAppearance

    //============================================================================================
    private GraphAppearance m_graphAppearance = null;
    /// <summary>
    /// Détermine différents aspects de l'apparence en mode graphique
    /// </summary>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("CellAppearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public GraphAppearance GraphAppearance
    {
        get { return m_graphAppearance; }
        set
        {
            if (value != m_graphAppearance)
            {
                m_graphAppearance = value;
                Refresh();
            }
        }
    }
    #endregion

    #region Constructeur
    //============================================================================================
    /// <summary>
    /// Initialise les champs concernant la partie graphique.
    /// </summary>
    public VisualGraphArray()
    {
        m_graphAppearance = new GraphAppearance(this);
        InitializeComponent();
    }

    #endregion

}
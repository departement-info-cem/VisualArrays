using VisualArrays.VisualCells;

namespace VisualArrays.VisualContainer;

/// <summary>
/// Représente un conteneur de VisualChar
/// </summary>
public partial class VisualCharContainer : BaseVisualContainer<char,VisualChar>
{
    #region Constructeur
    //========================================================================================================================
    /// <summary>
    /// Initialise un VisualCharContainer
    /// </summary>
    public VisualCharContainer()
    {
        InitializeComponent();
    }
    #endregion
}
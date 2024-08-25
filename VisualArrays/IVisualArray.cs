using VisualArrays.Others;

namespace VisualArrays;

/// <summary>
/// Définit des fonctionnalités communes pour une grille de cellules
/// </summary>
internal interface IVisualArray<TEventArgs> where TEventArgs:CellMouseEventArgs1D
{
    #region ÉVÉNEMENTS avec CellMouseEventArgs
    /// <summary>
    /// Se produit lors d'un MouseDown dans une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    event EventHandler<TEventArgs> CellMouseDown;

    /// <summary>
    /// Se produit lors d'un MouseDown dans une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    event EventHandler<TEventArgs> CellMouseUp;

    /// <summary>
    /// Se produit lors d'un MouseClick dans une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    event EventHandler<TEventArgs> CellMouseClick;

    /// <summary>
    /// Se produit lors d'un MouseDoubleClick dans une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    event EventHandler<TEventArgs> CellMouseDoubleClick;

    /// <summary>
    /// Se produit lorsque le pointeur de la souris entre dans une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    event EventHandler<CellEventArgs> CellMouseEnter;

    /// <summary>
    /// Se produit lorsque le pointeur de la souris quitte une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    event EventHandler<CellEventArgs> CellMouseLeave;

    /// <summary>
    /// Se produit lorsque le pointeur de la souris se déplace sur une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    event EventHandler<TEventArgs> CellMouseMove;

    #endregion

    #region Propriétés
    /// <summary>
    /// Fournit le nombre de cellules dans la grille
    /// </summary>
    int Length { get; }
    #endregion

    #region Méthodes

    #endregion
}
namespace VisualArrays.CellVisualElement;

/// <summary>
/// Représente un élément visuel apparaissant sur une cellule d'une grille.
/// </summary>
public abstract class CellVisualElement
{
    /// <summary>
    /// Prochain élément visuel à dessiner sur la cellule
    /// </summary>
    internal CellVisualElement NextVisualElement { get; set; }

    /// <summary>
    /// Initialise un élément visuel.
    /// </summary>
    protected CellVisualElement()
    {
        NextVisualElement = null;
        va_alignment = ContentAlignment.MiddleCenter;
        va_zoom = 100;
        Margin = new Padding();
    }

    #region Propriétés

    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit le facteur d'agrandissement de la représentation de l'élément visuel.
    /// </summary>
    protected int va_zoom;
    /// <summary>
    /// Obtient ou définit le facteur d'agrandissement de la représentation de l'élément visuel.
    /// </summary>
    public int Zoom
    {
        get => va_zoom;
        set
        {
            if (value > 100)
            {
                value = 100;
            }

            if (value < 5)
            {
                value = 5;
            }

            va_zoom = value;
        }
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la marge autour de l'élément visuel.
    /// </summary>
    protected Padding va_margin;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la marge autour de l'élément visuel.
    /// </summary>
    public Padding Margin
    {
        get => va_margin;
        set => va_margin = value;
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit l'alignement de l'élément sur une cellule.
    /// </summary>
    protected ContentAlignment va_alignment;
    /// <summary>
    /// Obtient ou définit l'alignement de l'élément sur une cellule.
    /// </summary>
    public ContentAlignment Alignment
    {
        get => va_alignment;
        set => va_alignment = value;
    }
    #endregion

    #region Alignement
    /// <summary>
    /// Fournit un rectangle permettant d'aligner un élément sur une cellule.
    /// </summary>
    /// <param name="pCellBounds">Taille de la cellule sur lequel le Sprite apparaît.</param>
    /// <param name="pSize">Taille de l'élément à aligner.</param>
    /// <param name="pAlignment">Alignement.</param>
    internal static Rectangle BoundsFromAlignment(Rectangle pCellBounds, Size pSize, ContentAlignment pAlignment)
    {
        int posX = 0, posY = 0;
        switch (pAlignment)
        {
            case ContentAlignment.BottomCenter:
                posY = pCellBounds.Y + pCellBounds.Height - pSize.Height;
                posX = pCellBounds.X + (pCellBounds.Width - pSize.Width) / 2;
                break;
            case ContentAlignment.BottomLeft:
                posY = pCellBounds.Y + pCellBounds.Height - pSize.Height;
                posX = pCellBounds.X;
                break;
            case ContentAlignment.BottomRight:
                posY = pCellBounds.Y + pCellBounds.Height - pSize.Height;
                posX = pCellBounds.X + pCellBounds.Width - pSize.Width;
                break;
            case ContentAlignment.MiddleCenter:
                posY = pCellBounds.Y + (pCellBounds.Height - pSize.Height) / 2;
                posX = pCellBounds.X + (pCellBounds.Width - pSize.Width) / 2;
                break;
            case ContentAlignment.MiddleLeft:
                posY = pCellBounds.Y + (pCellBounds.Height - pSize.Height) / 2;
                posX = pCellBounds.X;
                break;
            case ContentAlignment.MiddleRight:
                posY = pCellBounds.Y + (pCellBounds.Height - pSize.Height) / 2;
                posX = pCellBounds.X + pCellBounds.Width - pSize.Width;
                break;
            case ContentAlignment.TopCenter:
                posY = pCellBounds.Y;
                posX = pCellBounds.X + (pCellBounds.Width - pSize.Width) / 2;
                break;
            case ContentAlignment.TopLeft:
                posY = pCellBounds.Y;
                posX = pCellBounds.X;
                break;
            case ContentAlignment.TopRight:
                posY = pCellBounds.Y;
                posX = pCellBounds.X + pCellBounds.Width - pSize.Width;
                break;
            default:
                break;
        }
        return new Rectangle(posX, posY, pSize.Width, pSize.Height);
    }
    #endregion

    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Dessine le Sprite dans le graphique.
    /// </summary>
    /// <param name="pGraphics">Graphique dans lequel le Sprite doit se dessiner.</param>
    /// <param name="pBounds">Zone dans lequel le Sprite doit être dessiné</param>
    public abstract void Draw(Graphics pGraphics, Rectangle pBounds);

}
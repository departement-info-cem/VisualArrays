using VisualArrays.VisualArrays;

namespace VisualArrays.CellVisualElement;

/// <summary>
/// Représente un élément visuel affichant une image.
/// </summary>
public class ImageElement:CellVisualElement
{

    #region Propriétés
    //-------------------------------------------------------------------------------------
    private Image va_image;
    /// <summary>
    /// Obtient ou définit l'image associée avec l'ImageElement.
    /// </summary>
    public Image Image
    {
        get => va_image;
        set => va_image = value;
        //va_owner.RedessinerCelluleEtSprites(Index);
    }
    #endregion

    #region Constructeur
    /// <summary>
    /// Initialise un élément visuel affichant une image.
    /// </summary>
    /// <param name="pImage">Image à utiliser.</param>
    public ImageElement(Image pImage)
    {
        va_image = pImage;
    }
    /// <summary>
    /// Initialise un élément visuel affichant une image.
    /// </summary>
    /// <param name="pImage">Image à utiliser.</param>
    /// <param name="pZoom">Facteur d'agrandissement de l'image.</param>
    public ImageElement(Image pImage,int pZoom)
    {
        va_image = pImage;
        va_zoom = pZoom;
    }
    /// <summary>
    /// Initialise un élément visuel affichant une image.
    /// </summary>
    /// <param name="pImage">Image à utiliser.</param>
    /// <param name="pZoom">Facteur d'agrandissement de l'image.</param>
    /// <param name="pAlignment">Alignement de l'élément visuel.</param>
    public ImageElement(Image pImage, int pZoom, ContentAlignment pAlignment)
    {
        va_image = pImage;
        va_zoom = pZoom;
        va_alignment = pAlignment;
    }
    /// <summary>
    /// Initialise un élément visuel affichant une image.
    /// </summary>
    /// <param name="pImageFileName">`Nom du fichier contenant l'image.</param>
    /// <param name="pZoom">Facteur d'aggrandissement de l'image.</param>
    /// <param name="pAlignment">Alignement de l'élément visuel.</param>
    public ImageElement(string pImageFileName, int pZoom, ContentAlignment pAlignment)
    {
        va_image = null;
        va_zoom = pZoom;
        va_alignment = pAlignment;
        if (File.Exists(pImageFileName))
        {
            try
            {
                va_image = Image.FromFile(pImageFileName);
            }
            catch
            {
                throw new VisualArrayException("Impossible de charger l'image : " + pImageFileName);
            }
        }
        else
            throw new VisualArrayException("Impossible de charger l'image : " + pImageFileName);

    }
    #endregion
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Dessine l'élément visuel avec l'image courante.
    /// </summary>
    /// <param name="pGraphics">Objet graphique où dessiner.</param>
    /// <param name="pBounds"></param>
    public override void Draw(Graphics pGraphics, Rectangle pBounds)
    {
        if (va_image != null)
        {
            //int largeurZoom = pBounds.Width * va_zoom / 100;
            //int hauteurZoom = pBounds.Height * va_zoom / 100;
            //Rectangle contour = BoundsFromAlignment(pBounds, new Size(largeurZoom, hauteurZoom), va_alignment);
            Rectangle contour = BoundsFromAlignment(pBounds, va_image.Size, va_alignment);
            int posX = contour.Left + va_margin.Left - va_margin.Right;
            int posY = contour.Top + va_margin.Top - va_margin.Bottom;
            Rectangle contourWithMargin = contour with { X = posX, Y = posY };
            pGraphics.DrawImage(va_image, contourWithMargin);
        }
    }
}
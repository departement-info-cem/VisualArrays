namespace VisualArrays.Sprites
{
    /// <summary>
    /// Représente un 'Sprite' utilisant une image pour se dessiner
    /// </summary>
    public class ImageSprite:Sprite
    {
        /// <summary>
        /// 
        /// </summary>
        public ImageSprite()
        {
        }

        #region NEW STUFF (Utiliser une ImageList)
        //============================================================================================
        private readonly ImageList m_imageList = null;

        //============================================================================================
        private const int IMAGE_INDEX = 0;

        #endregion

        //============================================================================================
        /// <summary>
        /// Obtient et définit l'image du Sprite.
        /// </summary>
        private readonly Image m_image = null;

        /// <summary>
        /// Dessine un 'Sprite' utilisant une image pour se représenter
        /// </summary>
        /// <param name="pGraphics"></param>
        public override void Draw(Graphics pGraphics)
        {
            if (!m_visible) return;

            base.Draw(pGraphics);

            if (m_image != null)
            {
                pGraphics.DrawImage(m_image, m_bounds);
            }
            else if (m_imageList != null && IMAGE_INDEX >= 0 && IMAGE_INDEX < m_imageList.Images.Count)
            {
                Image objImage = m_imageList.Images[IMAGE_INDEX];
                pGraphics.DrawImage(objImage, m_bounds);
            }
        }
        //===========================================================================
        /// <summary>
        /// Dessine le Sprite à la coordonnée 0,0 dans le graphics
        /// </summary>
        /// <param name="pGraphics">Destination du dessin</param>
        public override void DrawAtOrigin(Graphics pGraphics)
        {
            Rectangle contour = m_bounds with { X = 0, Y = 0 };
            if (m_image != null)
            {
                pGraphics.DrawImage(m_image, contour);
            }
            else if (m_imageList != null && IMAGE_INDEX >= 0 && IMAGE_INDEX < m_imageList.Images.Count)
            {
                Image objImage = m_imageList.Images[IMAGE_INDEX];
                pGraphics.DrawImage(objImage, contour);
            }
        }
    }
}

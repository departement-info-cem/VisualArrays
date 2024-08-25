using System.ComponentModel;

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
        private ImageList m_imageList = null;
        /// <summary>
        /// Obtient et définit l'ImageList utilisé pour dessiner le Sprite
        /// </summary>
        [Description("ImageList utilisé pour dessiner le Sprite")]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Localizable(true), Category("Layout")]
        public ImageList ImageList
        {
            get { return m_imageList; }
            set
            {
                m_imageList = value;
                if (m_owner == null) return;
                m_owner.UpdateSprites(m_bounds);
            }
        }

        //============================================================================================
        private int m_imageIndex  = 0;
        /// <summary>
        /// Obtient et définit l'index de l'image utilisée pour dessiner le Sprite
        /// </summary>
        [Description("Index de l'image utilisé pour dessiner le Sprite")]
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Localizable(true), Category("Layout")]
        public int ImageIndex
        {
            get { return m_imageIndex; }
            set
            {
                m_imageIndex = value;
                if (m_owner == null) return;
                m_owner.UpdateSprites(m_bounds);
            }
        }
        #endregion

        //============================================================================================
        /// <summary>
        /// Obtient et définit l'image du Sprite.
        /// </summary>
        protected Image m_image = null;
        /// <summary>
        /// Obtient et définit l'image du Sprite.
        /// </summary>
        [DefaultValue(null), Description("Image utilisée pour représenter le Sprite")]
        [Localizable(true), Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image Image
        {
            get { return m_image; }
            set
            {
                m_image = value;
                if (m_image != null)
                    Size = new System.Drawing.Size(m_image.Width, m_image.Height);
                if (m_owner != null)
                    m_owner.UpdateSprites(m_bounds);
            }
        }
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
                //int largeurZoom = m_image.Size.Width; // *va_zoom / 100;
                //int hauteurZoom = m_image.Size.Height; // *va_zoom / 100;
                //Rectangle contour = VisualElement.BoundsFromAlignment(va_bounds, new Size(largeurZoom, hauteurZoom), va_alignment);
                //va_bounds = contour;
                pGraphics.DrawImage(m_image, m_bounds);

//pGraphics.DrawImage(m_image, 10, 10, m_image.Width, m_image.Height);
//m_image.RotateFlip(RotateFlipType.RotateNoneFlipX);
//pGraphics.DrawImage(m_image, 160, 10, m_image.Width, m_image.Height);

//                Point[] destinationPoints = {
//new Point(200, 20),   // destination for upper-left point of  
//                      // original 
//new Point(110, 100),  // destination for upper-right point of  
//                      // original 
//new Point(250, 30)};  // destination for lower-left point of  
//                // original
            }
            else if (m_imageList != null && m_imageIndex >= 0 && m_imageIndex < m_imageList.Images.Count)
            {
                Image objImage = m_imageList.Images[m_imageIndex];
                //int largeurZoom = objImage.Size.Width; // *va_zoom / 100;
                //int hauteurZoom = objImage.Size.Height; //*va_zoom / 100;
                //Rectangle contour = VisualElement.BoundsFromAlignment(va_bounds, new Size(largeurZoom, hauteurZoom), va_alignment);
                //va_bounds = contour;
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
            Rectangle contour = new Rectangle(0, 0, m_bounds.Width, m_bounds.Height);
            if (m_image != null)
            {
                pGraphics.DrawImage(m_image, contour);
            }
            else if (m_imageList != null && m_imageIndex >= 0 && m_imageIndex < m_imageList.Images.Count)
            {
                Image objImage = m_imageList.Images[m_imageIndex];
                pGraphics.DrawImage(objImage, contour);
            }
        }
    }
}

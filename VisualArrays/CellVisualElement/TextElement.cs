using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace VisualArrays
{
    /// <summary>
    /// 
    /// </summary>
    public class TextElement:CellVisualElement
    {
        #region Propriétés

        //-------------------------------------------------------------------------------------
        private string va_text;
        /// <summary>
        /// Obtient ou définit le texte d'un élément.
        /// </summary>
        public string Text
        {
            get { return va_text; }
            set { va_text = value; }
        }
        //-------------------------------------------------------------------------------------
        private Color va_color;
        /// <summary>
        /// Obtient ou définit la couleur du texte à afficher.
        /// </summary>
        public Color Color
        {
            get { return va_color; }
            set
            {
                va_color = value;
            }
        }
        //-------------------------------------------------------------------------------------
        private Font va_font;
        /// <summary>
        /// Obtient ou définit la police utilisée par l'élément. 
        /// </summary>
        public Font Font
        {
            get { return va_font; }
            set
            {
                va_font = value;
            }
        }
        #endregion

        #region Constructeurs
        //-------------------------------------------------------------------------------------
        /// <summary>
        /// Initialise un élément affichant du texte.
        /// </summary>
        /// <param name="pFont">Une police.</param>
        /// <param name="pText">Le texte.</param>
        public TextElement(Font pFont,string pText)
        {
            va_font = pFont;
            va_text = pText;
            va_color = Color.Red;
        }
        //-------------------------------------------------------------------------------------
        /// <summary>
        /// Initialise un élément affichant du texte.
        /// </summary>
        /// <param name="pFont">Police utilsée pour dessiner le texte</param>
        /// <param name="pText">Texte à afficher</param>
        /// <param name="pColor">Couleur du texte</param>
        public TextElement(Font pFont, string pText, Color pColor)
        {
            va_font = pFont;
            va_color = pColor;
            va_text = pText;
        }
        //-------------------------------------------------------------------------------------
        /// <summary>
        /// Initialise un élément affichant du texte.
        /// </summary>
        /// <param name="pFont">Une police.</param>
        /// <param name="pText">Le texte.</param>
        /// <param name="pColor">Une couleur.</param>
        /// <param name="pAlignment">Un alignement.</param>
        /// <param name="pMargin">Marge autour du texte</param>
        public TextElement(Font pFont, string pText, Color pColor, ContentAlignment pAlignment, Padding pMargin)
        {
            va_font = pFont;
            va_color = pColor;
            va_text = pText;
            va_alignment = pAlignment;
            va_margin = pMargin;
        }
        #endregion
        ///-------------------------------------------------------------------------------------
        /// <summary>
        /// Dessine le texte de l'élément.
        /// </summary>
        /// <param name="pGraphics">Graphique dans lequel le texte est dessiné.</param>
        /// <param name="pBounds">Rectangle dans lequel le texte doit être dessiné</param>
        public override void Draw(Graphics pGraphics, Rectangle pBounds)
        {
            SizeF pt = pGraphics.MeasureString(va_text, va_font);
            //Rectangle newBounds = new Rectangle(
            //int largeurZoom = pBounds.Width * va_zoom / 100 - 1;
            //int hauteurZoom = pBounds.Height * va_zoom / 100 - 1;

            //int posX = pBounds.X + va_margin.Left;
            //int posY = pBounds.Y + va_margin.Top;

            //Rectangle newBounds = new Rectangle(posX, posY, largeurZoom, hauteurZoom);
            Rectangle contour = BoundsFromAlignment(pBounds, new Size((int)pt.Width, (int)pt.Height), va_alignment);
            int posX = contour.Left + va_margin.Left - va_margin.Right;
            int posY = contour.Top + va_margin.Top - va_margin.Bottom;
            pGraphics.DrawString(va_text, va_font, new SolidBrush(va_color), posX, posY);
        }
        //-------------------------------------------------------------------------------------
        /// <summary>
        /// Fournit le texte de l'élément
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return va_text;
        }
    }
}

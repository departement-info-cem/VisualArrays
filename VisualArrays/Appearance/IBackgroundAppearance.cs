using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;

namespace VisualArrays
{
    #region IBackgroundAppearance
    /// <summary>
    /// Définit les éléments nécessaires pour contrôler l'apparence du fond des cellules
    /// </summary>
    public interface IBackgroundAppearance
    {
        /// <summary>
        /// Obtient et définit la radius utilisé lorsque enuShape est RoundRect.
        /// </summary>
        int Radius { get; set; }
        /// <summary>
        /// Obtient et définit la forme utilisée pour dessiner le fond de toutes les cellules.
        /// </summary>
        enuShape Shape { get; set; }
        /// <summary>
        /// Obtient ou définit l'image de fond des cellules.
        /// </summary>
        Image Image { get; set; }
        /// <summary>
        /// Obtient et définit la taille du crayon pour dessiner une forme contour.
        /// </summary>
        int PenWidth { get; set; }
        /// <summary>
        /// Obtient et définit le style de fond des cellules.
        /// </summary>
        enuBkgStyle Style { get; set; }
        /// <summary>
        /// Obtient et définit la taille des bordures autour des cellules.
        /// </summary>
        Padding Border { get; set; }
        /// <summary>
        /// Obtient et définit la couleur de fond des cellules.
        /// </summary>
        Color BackgroundColor { get; set; }
    }
    #endregion
}

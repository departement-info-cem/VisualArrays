using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace VisualArrays
{
    #region Interface ICellDraw
    /// <summary>
    ///     Prend en charge le dessin du contenu d'une cellule.
    /// </summary>
    public interface ICellDraw
    {
        //============================================================================================
        /// <summary>
        /// Dessine le contenu d'une cellule.
        /// </summary>
        /// <param name="pGraphics">objet graphique où dessiner</param>
        /// <param name="pCellContentBounds">Contour de la zone de contenu de la cellule</param>
        /// <param name="pCellBounds">Contour complet de la cellule</param>
        /// <param name="pEnabled">État de la cellule à dessiner</param>
        void DrawCellContent(Graphics pGraphics, Rectangle pCellContentBounds, Rectangle pCellBounds, bool pEnabled);
        //============================================================================================
        /// <summary>
        /// Dessine le contenu d'une cellule pour une opération glisser/déposer
        /// </summary>
        /// <param name="pGraphics">objet graphique où dessiner</param>
        /// <param name="pCellContentBounds">Contour de la zone de contenu de la cellule</param>
        /// <param name="pCellBounds">Contour complet de la cellule</param>
        /// <param name="pEnabled">État de la cellule à dessiner</param>
        void DrawCellDragContent(Graphics pGraphics, Rectangle pCellContentBounds, Rectangle pCellBounds, bool pEnabled);
    }
    #endregion

    #region Cell
    /// <summary>
    /// Représente une cellule d'une VisualArrays
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Obtient ou définit un VisualElement sur la cellule en dessous de la valeur
        /// </summary>
        public CellVisualElement LayerUnder = null;
        /// <summary>
        /// Obtient ou définit le premier élément visuel sur la cellule au dessus de la valeur
        /// </summary>
        public CellVisualElement LayerOver = null;
        /// <summary>
        /// Obtient ou définit le VisualElement utilisé pour dessiner le fond de la cellule
        /// </summary>
        public CellVisualElement Background = null;
        /// <summary>
        /// Obtient ou définit si la valeur de la cellule peut être modifiée ou non 
        /// </summary>
        public bool ReadOnly = false;
        /// <summary>
        /// Obtient ou définit si la cellule est sélectionée ou non 
        /// </summary>
        public bool Selected = false;
        /// <summary>
        /// Obtient ou définit si la cellule est active ou non 
        /// </summary>
        public bool Enabled = true;
        /// <summary>
        /// Obtient ou définit si la cellule est visible ou non
        /// </summary>
        public bool Visible = true;
        /// <summary>
        /// Obtient ou définit des données utilisateur pour le contenu et l'affichage d'une cellule
        /// </summary>
        public ICellDraw UserContent = null;
        /// <summary>
        /// Obtient ou définit des données utilisateur supplémentaire à associer avec la cellule
        /// </summary>
        public Object UserData = null;
        /// <summary>
        /// Remet la cellule dans son état initial
        /// </summary>
        internal void Reset()
        {
            LayerUnder = null;
            LayerOver = null;
            Background = null;
            ReadOnly = false;
            Selected = false;
            Enabled = true;
            Visible = true;
            UserContent = null;
            UserData = null;
        }
    }
    #endregion
  
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualArrays
{
    /// <summary>
    /// 
    /// </summary>
    internal enum enuTypeElement { Cell, Sprite };
    /// <summary>
    /// Représente les informations impliquées dans une opération glisser/déposer
    /// </summary>
    internal class DragAndDropInfos
    {
        /// <summary>
        /// Type d'élément impliqué dans l'opération glisser/déposer
        /// </summary>
        public enuTypeElement TypeElement;
        /// <summary>
        /// Nom de la grille source de l'opération glisser/déposer
        /// </summary>
        public string SourceGridName;
        /// <summary>
        /// Index source de l'opération glisser/déposer
        /// </summary>
        public int SourceIndex;
        /// <summary>
        /// Rangée source de l'opération glisser/déposer
        /// </summary>
        public int SourceRow;
        /// <summary>
        /// Colonne source de l'opération glisser/déposer
        /// </summary>
        public int SourceColumn;
        /// <summary>
        /// Sprite source de l'opération glisser/déposer, null si c'est une cellule
        /// </summary>
        public Sprite DragSprite;
        /// <summary>
        /// Initialise un DragAndDropInfos
        /// </summary>
        /// <param name="pTypeElement"></param>
        /// <param name="pSourceGridName"></param>
        /// <param name="pSourceIndex"></param>
        /// <param name="pSourceRow"></param>
        /// <param name="pSourceColumn"></param>
        /// <param name="pDragSprite"></param>
        public DragAndDropInfos(enuTypeElement pTypeElement, string pSourceGridName, int pSourceIndex, int pSourceRow, int pSourceColumn, Sprite pDragSprite)
        {
            TypeElement = pTypeElement;
            SourceGridName = pSourceGridName;
            SourceIndex = pSourceIndex;
            SourceRow = pSourceRow;
            SourceColumn = pSourceColumn;
            DragSprite = pDragSprite;
        }
    }
}

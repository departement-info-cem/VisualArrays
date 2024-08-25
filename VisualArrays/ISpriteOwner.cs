namespace VisualArrays
{
    /// <summary>
    /// 10/02/2012
    /// PROBLÈMES : Impossible d'utiliser simplement les Sprites actuels avec une grille VisualContainer. 
    /// 
    ///     - La propriété DisplayAddress n'est pas compatible
    ///     - Le code pour Sprite sur plusieurs cellules n'est pas compatible
    ///     - Le SegmentSprite n'est pas compatible
    /// 
    /// Définit les fonctionnalités que doit posséder une grille offrant des Sprites 
    /// </summary>
    public interface ISpriteOwner
    {
        /// <summary>
        /// Obtient le contour d'une cellule.
        /// </summary>
        /// <param name="pIndex">Index de la cellule</param>
        /// <returns>Contour d'une cellule</returns>
        Rectangle GetCellBounds(int pIndex);
        /// <summary>
        /// Force l'affichage de tous les Sprites touchés par le rectangle
        /// </summary>
        /// <param name="pBounds">Rectangle de la zone à mettre à jour</param>
        void UpdateSprites(Rectangle pBounds);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace VisualArrays
{
    #region enuBarAlign
    /// <summary>
    /// Alignement d'une barre dans un graphique
    /// </summary>
    public enum enuBarAlign
    {
        /// <summary>
        /// Selon l'orientation de gauche à droite et de bas en haut.
        /// </summary>
        Normal,
        /// <summary>
        /// Selon l'orientation de droite à gauche et de haut en bas.
        /// </summary>
        Reverse,
        /// <summary>
        /// Au milieu de la zone
        /// </summary>
        Middle
    }
    #endregion

    #region enuDirection
    /// <summary>
    /// Quatre directions dans un déplacement en 2 dimensions.
    /// </summary>
    public enum enuDirection
    {
        /// <summary>
        /// Vers la gauche.
        /// </summary>
        Left,
        /// <summary>
        /// Vers le haut.
        /// </summary>
        Top,
        /// <summary>
        /// Vers la droite.
        /// </summary>
        Right,
        /// <summary>
        /// Vers le bas.
        /// </summary>
        Bottom
    }
    #endregion

    #region enuCharView
    /// <summary>
    /// Les styles de visualisation pour la VisualCharArray.
    /// </summary>
    public enum enuCharView
    {
        /// <summary>
        /// Style caractère.
        /// </summary>
        Char,
        /// <summary>
        /// Style code.
        /// </summary>
        Code
    }
    #endregion

    #region enuStringView
    /// <summary>
    /// Les styles de visualisation pour la VisualStringArray.
    /// </summary>
    public enum enuStringView
    {
        /// <summary>
        /// Style texte.
        /// </summary>
        Text,
        /// <summary>
        /// Style image.
        /// </summary>
        ImageFile
    }
    #endregion

    #region enuIntView
    /// <summary>
    /// Les styles de visualisation pour la VisualIntArray.
    /// </summary>
    public enum enuIntView
    {
        /// <summary>
        /// Style nombre.
        /// </summary>
        Number,
        /// <summary>
        /// Style nombre / maximum
        /// </summary>
        NumberFraction,
        /// <summary>
        /// Style graphique.
        /// </summary>
        Graph,
        /// <summary>
        /// Style digit.
        /// </summary>
        Digit,
        /// <summary>
        /// Style image.
        /// </summary>
        ImageList,
        /// <summary>
        /// Style image graphique + nombre.
        /// </summary>
        GraphNumber,
        /// <summary>
        /// Style image graphique + nombre / maximum.
        /// </summary>
        GraphNumberFraction
    }
    #endregion

    #region enuDecimalView
    /// <summary>
    /// Les styles de visualisation pour la VisualDecimalArray.
    /// </summary>
    public enum enuDecimalView
    {
        /// <summary>
        /// Style nombre.
        /// </summary>
        Number,
        /// <summary>
        /// Style graphique.
        /// </summary>
        Graph,
        /// <summary>
        /// Style graphique + nombre.
        /// </summary>
        GraphNumber
    }
    #endregion

    #region enuGraphBarStyle
    /// <summary>
    /// Les styles de barres
    /// </summary>
    public enum enuGraphBarStyle
    {
        /// <summary>
        /// Utilise une image pour dessiner les barres
        /// </summary>
        Image,
        /// <summary>
        /// Dessiner un rectagle plein
        /// </summary>
        FillShape
    }
    #endregion

    #region enuGraphValueStyle
    /// <summary>
    /// Les styles de visualisation pour la valeur du graphique.
    /// </summary>
    public enum enuGraphValueStyle
    {
        /// <summary>
        /// Aucune valeur.
        /// </summary>
        None,
        /// <summary>
        /// La valeur est affichée normalement.
        /// </summary>
        Normal,
        /// <summary>
        /// La valeur est affichée avec un symbole %.
        /// </summary>
        Pourcent
    }
    #endregion

    #region enuBkgStyle
    //============================================================================================
    /// <summary>
    /// Les styles de fond d'une cellule.
    /// </summary>
    public enum enuBkgStyle
    {
        /// <summary>
        /// Pas de fond, la cellule est transparente.
        /// </summary>
        None,
        /// <summary>
        /// Pas de fond, seulement une bordure.
        /// </summary>
        Border,
        /// <summary>
        /// Le fond est dessiné par une forme pleine.
        /// </summary>
        FillShape,
        /// <summary>
        /// Le fond est dessiné par une forme contour.
        /// </summary>
        Shape,
        /// <summary>
        /// Le fond est dessiné par une image.
        /// </summary>
        Image,
    }
    #endregion

    #region enuStrikeStyle
    //============================================================================================
    /// <summary>
    /// Les styles de trait d'une cellule Enabled false.
    /// </summary>
    public enum enuStrikeStyle
    {
        /// <summary>
        /// Pas de trait.
        /// </summary>
        None,
        /// <summary>
        /// Trace une diagonale.
        /// </summary>
        Diagonal,
        /// <summary>
        /// Trace une croix.
        /// </summary>
        Cross,
        /// <summary>
        /// Affiche la StrikeImage.
        /// </summary>
        Image
    }
    #endregion

    #region enuShape
    //============================================================================================
    /// <summary>
    /// Les formes à superposer sur une cellule.
    /// </summary>
    public enum enuShape
    {
        /// <summary>
        /// Un rectangle aux coins arrondis
        /// </summary>
        RoundRect,
        /// <summary>
        /// Un rectangle.
        /// </summary>
        Rectangle,
        /// <summary>
        /// Un ellipse.
        /// </summary>
        Ellipse,
        /// <summary>
        /// Un triangle isocèle vers le haut.
        /// </summary>
        TriangleIsoUp,
        /// <summary>
        /// Un triangle isocèle vers le bas.
        /// </summary>
        TriangleIsoDown,
        /// <summary>
        /// Un triangle isocèle vers la droite.
        /// </summary>
        TriangleIsoRight,
        /// <summary>
        /// Un triangle isocèle vers la gauche.
        /// </summary>
        TriangleIsoLeft,
        /// <summary>
        /// Un triangle rectangle vers le bas gauche.
        /// </summary>
        TriangleRectBL,
        /// <summary>
        /// Un triangle rectangle vers le bas droit.
        /// </summary>
        TriangleRectBR,
        /// <summary>
        /// Un triangle rectangle vers le haut gauche.
        /// </summary>
        TriangleRectTL,
        /// <summary>
        /// Un triangle rectangle vers le haut droit.
        /// </summary>
        TriangleRectTR,
        /// <summary>
        /// Un losange.
        /// </summary>
        Losange,
        /// <summary>
        /// Un hexagone horizontal.
        /// </summary>
        HexagonH,
        /// <summary>
        /// Un hexagone vertical.
        /// </summary>
        HexagonV,
        /// <summary>
        /// Un parallélogramme.
        /// </summary>
        Parallelogram
    }
    #endregion

    #region enuSelectionStyle
    //============================================================================================
    /// <summary>
    /// Les styles disponibles pour la sélection.
    /// </summary>
    public enum enuSelectionStyle
    {
        /// <summary>
        /// La sélection utilise une forme.
        /// </summary>
        Shape,
        /// <summary>
        /// Le sélection utilise une image.
        /// </summary>
        Image
    }
    #endregion

    #region enuDragStyle
    //============================================================================================
    /// <summary>
    /// Les styles disponibles pour l'opération glisser/déposer.
    /// </summary>
    public enum enuDragStyle
    {
        /// <summary>
        /// Le destination utilise une forme pleine.
        /// </summary>
        FillShape,
        /// <summary>
        /// La destination utilise une forme.
        /// </summary>
        Shape,
        /// <summary>
        /// La destination utilise une image.
        /// </summary>
        Image
    }
    #endregion

    #region enuHeaderBkgStyle
    //============================================================================================
    /// <summary>
    /// Les styles de fond d'un en-tête.
    /// </summary>
    public enum enuHeaderBkgStyle
    {
        /// <summary>
        /// Pas de fond de cellule, la cellule est transparente.
        /// </summary>
        None,
        /// <summary>
        /// Le fond de la cellule est plein.
        /// </summary>
        Fill,
    }
    #endregion

    #region enuResizeMode
    //============================================================================================
    /// <summary>
    /// Les modes de redimentionnement de la grille.
    /// </summary>
    public enum enuResizeMode
    {
        /// <summary>
        /// Horizontalement et verticalement.
        /// </summary>
        Normal,
        /// <summary>
        /// Par ligne et par colonne.
        /// </summary>
        RowColumn,
        /// <summary>
        /// Par ligne verticalement.
        /// </summary>
        Row,
        /// <summary>
        /// Par colonne horizontalement.
        /// </summary>
        Column
    }
    #endregion

    #region enuAddressMode
    //============================================================================================
    /// <summary>
    /// Les modes d'interprétation de la valeur d'un index.
    /// </summary>
    public enum enuAddressMode
    {
        /// <summary>
        /// 
        /// </summary>
        Normal,
        /// <summary>
        /// 
        /// </summary>
        Column,
        /// <summary>
        /// 
        /// </summary>
        ColumnReverse,
        /// <summary>
        /// 
        /// </summary>
        Reverse,
        /// <summary>
        /// 
        /// </summary>
        ReverseRow,
        /// <summary>
        /// 
        /// </summary>
        ReverseColumn,
        /// <summary>
        /// 
        /// </summary>
        StairsTopLeft,
        /// <summary>
        /// 
        /// </summary>
        StairsTopRight,
        /// <summary>
        /// 
        /// </summary>
        StairsBottomLeft,
        /// <summary>
        /// 
        /// </summary>
        StairsBottomRight
    }
    #endregion

    #region enuAddressView
    //============================================================================================
    /// <summary>
    /// Les modes de visualisation des adresses en mode conception.
    /// </summary>
    public enum enuAddressView
    {
        /// <summary>
        /// Aucun affichage.
        /// </summary>
        None,
        /// <summary>
        /// Affichage sous forme d'index.
        /// </summary>
        Mode1D,
        /// <summary>
        /// Afficahge sous forme de coordonnées.
        /// </summary>
        Mode2D
    }
    #endregion

    #region enuDataStyle
    //============================================================================================
    /// <summary>
    /// Les styles d'affichage des en-têtes.
    /// </summary>
    public enum enuDataStyle
    {
        /// <summary>
        /// Avec un index selon la base 0.
        /// </summary>
        Index,
        /// <summary>
        /// Avec un index selon la base 1.
        /// </summary>
        IndexBase1,
        ///// <summary>
        ///// Avec des lettres.
        ///// </summary>
        //Letter,
        ///// <summary>
        ///// Avec des nombres par puissance de 2.
        ///// </summary>
        //Binary,
        /// <summary>
        /// Avec un texte personalisé.
        /// </summary>
        User
    }
    #endregion

    #region enuValueFormat
    //============================================================================================
    /// <summary>
    /// Les différents formats d'affichage d'une valeur décimale.
    /// </summary>
    public enum enuValueFormat
    {
        /// <summary>
        /// Affichage standard.
        /// </summary>
        Standard,
        /// <summary>
        /// Affichage monétaire avec $
        /// </summary>
        Currency,
        /// <summary>
        /// Affichage en pourcentage avec %
        /// </summary>
        Pourcent
    }
    #endregion
}

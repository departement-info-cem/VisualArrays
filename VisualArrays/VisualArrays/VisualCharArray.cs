using System.ComponentModel;
using VisualArrays.Others;

namespace VisualArrays.VisualArrays
{
    /// <summary>
    /// Représente une grille dont les cellules sont caractères.  
    /// </summary>
    [ToolboxBitmap(typeof(VisualCharArray), "Resources.tbxVisualCharArray")]
    public partial class VisualCharArray : VisualValueArray<char>
    {
        #region Champs et Propriétés

        //============================================================================================
        //private char m_defaultValue = 'A';
        /// <summary>
        /// Obtient ou définit la valeur par défaut de toutes les cellules.
        /// </summary>
        [DefaultValue('A'), Category("VisualArrays"), Browsable(true), Description("Valeur par défaut de toutes les cellules")]
        public char DefaultValue
        {
            get => m_defaultValue;
            set
            {
                if (value != m_defaultValue)
                {
                    m_defaultValue = value;
                    ResetAllValuesToDefault();
                    this.Refresh();
                }
            }
        }

        //============================================================================================
        //private char m_specialValue = '?';
        /// <summary>
        /// Obtient ou définit la valeur spéciale à afficher différement des autres valeurs.
        /// </summary>
        [DefaultValue('?'), Category("VisualArrays"), Browsable(true), Description("Valeur spéciale à afficher différemment des autres valeurs, voir SpecialValueAppearance")]
        public char SpecialValue
        {
            get => m_specialValue;
            set
            {
                if (value != m_specialValue)
                {
                    m_specialValue = value;
                    this.Refresh();
                }
            }
        }
        //============================================================================================
        private enuCharView va_view;
        /// <summary>
        /// Obtient et définit le style de visualisation pour les valeurs de la grille
        /// </summary>
        [DefaultValue(enuCharView.Char), Category("CellAppearance"), Browsable(true), Description("Obtient et définit le style de visualisation pour les valeurs de la grille.")]
        public enuCharView View
        {
            get => va_view;
            set
            {
                va_view = value;
                Refresh();
            }
        }
        //============================================================================================
        private char va_imlBaseChar = 'A';
        /// <summary>
        /// Obtient ou définit le caractère de base pour l'indéxation des images en mode View ImageList
        /// </summary>
        [DefaultValue('A'), Category("VisualArrays"), Browsable(true), Description("Caractère de base pour l'indéxation des images en mode View ImageList")]
        public char ImlBaseChar
        {
            get => va_imlBaseChar;
            set
            {
                va_imlBaseChar = value;
                this.Refresh();
            }
        }
        //============================================================================================
        private bool va_autoNextIndex = false;
        /// <summary>
        /// Obtient ou définit si la saisie d'un caractère nous augmente la SelectedIndex
        /// </summary>
        [DefaultValue(false), Category("VisualArrays"), Browsable(true), Description("Indique si la saisie d'un caractère nous augmente la SelectedIndex.")]
        public bool AutoNextIndex
        {
            get => va_autoNextIndex;
            set
            {
                va_autoNextIndex = value;
                this.Refresh();
            }
        }
        #endregion

        #region Constructeur
        /// <summary>
        /// Instancie une grille visuelle de caractères.
        /// </summary>
        public VisualCharArray()
        {
            m_defaultValue = 'A';
            m_specialValue = '?';
            ResetAllValuesToDefault();
            InitializeComponent();
        }
        #endregion

        #region OnMouseWheel, OnKeyPress
        //============================================================================================
        /// <summary>
        /// Se produit lorsque le MouseWheel change de valeur
        /// </summary>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (SelectedIndex != -1 && !ReadOnly && va_selectionMode == SelectionMode.One && !va_tabCells[SelectedAddress.Row, SelectedAddress.Column].ReadOnly)
            {
                char valeur = this[SelectedIndex];
                if (e.Delta > 0)
                    valeur++;
                else if (e.Delta < 0)
                    valeur--;
                if (valeur < 0) valeur = (char)0;
                if (valeur > 255) valeur = (char)255;
                this[SelectedIndex] = valeur;
            }
        }
        //============================================================================================
        /// <summary>
        /// Accepte tous les caractères saisies au clavier
        /// </summary>
        /// <param name="e">infos sur la touche pressée</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (SelectedIndex != -1 && !va_readOnly && va_selectionMode != SelectionMode.None && !va_tabCells[SelectedAddress.Row, SelectedAddress.Column].ReadOnly)
            {
                if (e.KeyChar == (char)Keys.Enter) return;
                char oldValeur = this[SelectedIndex];
                va_currentKeyTime = DateTime.Now;
                char valeur = e.KeyChar;
                if (valeur != oldValeur)
                    this[SelectedIndex] = valeur;
                va_currentKeyTime = new DateTime(0);

                // on va essayer d'aller au prochain index disponible
                if (va_autoNextIndex && SelectedIndex != -1 && SelectedIndex < Length - 1)
                {
                    int currentIndex = SelectedIndex + 1;
                    while (currentIndex != SelectedIndex)
                    {
                        if (GetCellEnabled(currentIndex) && GetCellVisibility(currentIndex) && this[currentIndex] != SpecialValue)
                        {
                            SelectedIndex = currentIndex;
                            break;
                        }
                        currentIndex++;
                        if (currentIndex >= Length)
                            currentIndex = 0;
                    }
                }
            }
            base.OnKeyPress(e);
        }
        #endregion

        #region DrawCellContent, DrawCellDragContent
        //--------------------------------------------------------------------------------------
        /// <summary>
        /// Dessiner le contenu d'une cellule dans une zone de la taille de la cellule, 
        /// servant à l'opération glisser/déposer.
        /// </summary>
        /// <param name="pGraphics"></param>
        /// <param name="pContentBounds">Contour du contenu de la cellule</param>
        /// <param name="pRow">Rangée</param>
        /// <param name="pColumn">Colonne</param>
        protected override void DrawCellDragContent(Graphics pGraphics, Rectangle pContentBounds, int pRow, int pColumn)
        {
            Cell cell = va_tabCells[pRow, pColumn];
            pContentBounds = new Rectangle(0, 0, pContentBounds.Width, pContentBounds.Height);

            char valeurAAfficher = va_tabValues[pRow, pColumn];
            // Dessiner le fond de la cellule si nécessaire
            if (valeurAAfficher == m_specialValue)
            {
                va_specialValueVisualElement?.Draw(pGraphics, pContentBounds);
            }
            else
            {
                cell.Background?.Draw(pGraphics, pContentBounds);
            }

            switch (va_view)
            {
                case enuCharView.Char:
                    pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    string laChaine = valeurAAfficher.ToString();
                    if (valeurAAfficher != m_specialValue)
                        DrawText(pGraphics, pContentBounds, laChaine, EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                    else if (SpecialValueAppearance.ShowValue) // c'est la valeur spéciale
                        DrawText(pGraphics, pContentBounds, laChaine, SpecialValueAppearance.TextColor, SpecialValueAppearance.Font, m_cellContentAlign);
                    break;
                //case enuCharView.ImageList:
                //    // Ne pas dessiner l'image si la valeur à afficher est la valeur spéciale
                //    if (valeurAAfficher == m_specialValue) break;

                //    ImageList imageList = va_enabledAppearance.ImageList;

                //    if (imageList != null)
                //    {
                //        if (valeurAAfficher >= 0 && valeurAAfficher < imageList.Images.Count)
                //        {
                //            Image objImage = imageList.Images[valeurAAfficher];
                //            Rectangle imageBounds = CellVisualElement.BoundsFromAlignment(pContentBounds, objImage.Size, m_cellContentAlign);
                //            if (cell.Enabled)
                //                pGraphics.DrawImage(objImage, new Point(imageBounds.Left, imageBounds.Top));
                //            else
                //                VisualArraysTools.DrawDisabledImage(pGraphics, imageBounds, objImage, DisabledAppearance.ImageBrightness);
                //        }
                //        else
                //        {
                //            pGraphics.FillRectangle(new SolidBrush(BackColor), pContentBounds);
                //            //pGraphics.SetClip(cellContentBounds); // Pour ne pas dessiner l'extérieur des cellules
                //            //DrawText(pGraphics, pRow, pColumn, "?", EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                //        }
                //    }
                //    //else // il n'y pas d'imageList d'associé avec la grille
                //      //  DrawText(pGraphics, pRow, pColumn, "?", EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                //    break;
                default:
                    break;
            }
        }
        //============================================================================================
        /// <summary>
        /// Dessine la cellule à l'adresse pRow et pColumn.
        /// </summary>
        /// <param name="pGraphics">Objet graphique où dessiner.</param>
        /// <param name="pRow">Rangée.</param>
        /// <param name="pColumn">Colonne.</param>
        protected override void DrawCellContent(Graphics pGraphics, int pRow, int pColumn)
        {
            Rectangle cellContentBounds = GetCellContentBounds(pRow, pColumn);
            Rectangle cellBounds = GetCellBounds(pRow, pColumn);
            Cell cell = va_tabCells[pRow, pColumn];

            // Étape 1 : On commence par dessiner le fond de la grille
            if (BackgroundImage != null)
                pGraphics.DrawImage(BackgroundImage, cellBounds, cellBounds, GraphicsUnit.Pixel);
            else
                pGraphics.FillRectangle(new SolidBrush(BackColor), cellBounds);

            // Étape 2 : Si la cellule n'est pas visible, alors on quitte (sans même afficher son adresse)
            if (!cell.Visible) return;


            // Étape 3 : Si l'utilisateur désire dessiner lui même le contenu de la cellule
            if (cell.UserContent != null)
            {
                pGraphics.SetClip(cellBounds); // Pour ne pas dessiner l'extérieur des cellules
                cell.UserContent.DrawCellContent(pGraphics, cellContentBounds, cellBounds, cell.Enabled);
            }
            else
            {   // Étape 3B : Selon l'état de la cellule Enabled == true
                pGraphics.SetClip(cellContentBounds); // Pour ne pas dessiner l'extérieur des cellules

                char valeurAAfficher = va_tabValues[pRow, pColumn];

                cell.LayerUnder?.Draw(pGraphics, cellContentBounds);

                if (cell.Enabled) //  la cellule est active
                {
                    if (valeurAAfficher == m_specialValue) // la valeur est spéciale
                    {
                        va_specialValueVisualElement?.Draw(pGraphics, cellContentBounds);
                    }
                    else // la valeur est normale
                    {
                        cell.Background?.Draw(pGraphics, cellContentBounds);
                    }
                }
                else // la cellule est inactive
                {
                    va_disabledVisualElement?.Draw(pGraphics, cellContentBounds);
                }

                #region Code pour dessiner la valeur de la cellule
                string laChaine = null;
                Rectangle displayRectangle = GetCellContentBounds(pRow, pColumn, EnabledAppearance.Padding);
                if (!DesignMode || va_addressView == enuAddressView.None)
                    switch (va_view)
                    {
                        case enuCharView.Char:
                            laChaine = va_tabValues[pRow, pColumn].ToString();

                            if (cell.Enabled) // la cellule est active
                            {
                                if (valeurAAfficher == m_specialValue) // c'est la valeur spéciale
                                {
                                    if (SpecialValueAppearance.ShowValue)
                                        VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, SpecialValueAppearance.TextColor, SpecialValueAppearance.Font, m_cellContentAlign);
                                }
                                else // la valeur est normale
                                    VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                            }
                            else // la cellule est inactive
                            {
                                if (va_disabledAppearance.ShowValue)
                                    VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, DisabledAppearance.TextColor, DisabledAppearance.Font, m_cellContentAlign);
                            }
                            break;
                        case enuCharView.Code:
                            laChaine = ((int)va_tabValues[pRow, pColumn]).ToString();

                            if (cell.Enabled) // la cellule est active
                            {
                                if (valeurAAfficher == m_specialValue) // c'est la valeur spéciale
                                {
                                    if (SpecialValueAppearance.ShowValue)
                                        VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, SpecialValueAppearance.TextColor, SpecialValueAppearance.Font, m_cellContentAlign);
                                }
                                else // la valeur est normale
                                {
                                    VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                                }
                            }
                            else // la cellule est inactive
                            {
                                if (va_disabledAppearance.ShowValue)
                                    VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, DisabledAppearance.TextColor, DisabledAppearance.Font, m_cellContentAlign);
                            }
                            break;
                        //case enuCharView.ImageList:
                        //    // Ne pas dessiner l'image si la valeur à afficher est la valeur spéciale
                        //    if (valeurAAfficher == m_specialValue) break;

                        //    ImageList imageList;
                        //    if (cell.Enabled)
                        //        imageList = va_enabledAppearance.ImageList;
                        //    else
                        //    { // on va utiliser le même ImageList que pour les cellules actives
                        //        imageList = va_disabledAppearance.ImageList;
                        //        if (imageList == null)
                        //            imageList = va_enabledAppearance.ImageList;
                        //    }

                        //    if (imageList != null)
                        //    {
                        //        valeurAAfficher = (char)(valeurAAfficher - va_imlBaseChar); // pour décaler les index selon le caractère de base
                        //        if (valeurAAfficher >= 0 && valeurAAfficher < imageList.Images.Count)
                        //        {
                        //            Image objImage = imageList.Images[valeurAAfficher];
                        //            Rectangle imageBounds = CellVisualElement.BoundsFromAlignment(cellContentBounds, objImage.Size, m_cellContentAlign);
                        //            if (cell.Enabled)
                        //                pGraphics.DrawImage(objImage, new Point(imageBounds.Left, imageBounds.Top));
                        //            else
                        //                VisualArraysTools.DrawDisabledImage(pGraphics, imageBounds, objImage, DisabledAppearance.ImageBrightness);
                        //        }
                        //        else
                        //        {
                        //            pGraphics.FillRectangle(new SolidBrush(BackColor), cellContentBounds);
                        //            //pGraphics.SetClip(cellContentBounds); // Pour ne pas dessiner l'extérieur des cellules
                        //            VisualArraysTools.DrawText(pGraphics, displayRectangle, "?", EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                        //        }
                        //    }
                        //    else // il n'y pas d'imageList d'associé avec la grille
                        //        VisualArraysTools.DrawText(pGraphics, displayRectangle, "?", EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                        //    break;
                    }
                #endregion

            }
            // Étape 4 : On va dessiner les couches supplémentaires soit les VisualElement ajoutés
            CellVisualElement.CellVisualElement layerVE = cell.LayerOver;
            while (layerVE != null)
            {
                layerVE.Draw(pGraphics, cellContentBounds);
                layerVE = layerVE.NextVisualElement;
            }

            pGraphics.ResetClip();

            // Étape 5 : Si la cellule est inactive et qu'une raillure doit être dessinée
            if (!cell.Enabled && va_disabledAppearance.StrikeAppearance.Style != enuStrikeStyle.None)
                DrawStrike(pGraphics, cellBounds, va_disabledAppearance);

            // Étape 6 : Si la cellule est sélectionnée, alors on doit dessiner la sélection
            if (cell.Selected)
                DrawSelection(pGraphics, pRow, pColumn);

            // Étape 7 : Si nous sommes en mode désign alors on doit dessiner l'adresse de la cellule
            if (DesignMode)
                DrawAddress(pGraphics, pRow, pColumn);
        }
        #endregion

        #region RandomValue
        /// <summary>
        /// Génère un caractère aléatoire.
        /// </summary>
        /// <param name="pMin">Borne inférieure inclue dans l'intervalle</param>
        /// <param name="pMax">Borne supérieure inclue dans l'intervalle</param>
        /// <returns>Un caractère aléatoire entre pMin et pMax</returns>
        public char RandomValue(char pMin, char pMax)
        {
            return VisualArraysTools.RandomChar(pMin, pMax);
        }
        #endregion
    }
}

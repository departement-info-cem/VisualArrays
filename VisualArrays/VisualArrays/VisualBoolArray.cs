using System.ComponentModel;
using VisualArrays.Appearance;
using VisualArrays.Others;

namespace VisualArrays.VisualArrays
{
    /// <summary>
    /// Représente une grille dont les cellules sont true ou false.  
    /// </summary>
    [ToolboxBitmap(typeof(VisualBoolArray), "Resources.tbxVisualBoolArray")]
    public partial class VisualBoolArray : VisualValueArray<bool>
    {
        #region Champs et Propriétés
        //============================================================================================
        //private bool m_defaultValue = false;
        /// <summary>
        /// Obtient ou définit la valeur par défaut de toutes les cellules.
        /// </summary>
        [DefaultValue(false), Category("VisualArrays"), Browsable(true), Description("Valeur par défaut de toutes les cellules")]
        public bool DefaultValue
        {
            get { return m_defaultValue; }
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
        //private bool m_specialValue = true;
        /// <summary>
        /// Obtient ou définit la valeur spéciale à afficher différement des autres valeurs.
        /// </summary>
        [DefaultValue(true), Category("VisualArrays"), Browsable(true), Description("Valeur spéciale à afficher différemment des autres valeurs, voir SpecialValueAppearance")]
        public bool SpecialValue
        {
            get { return m_specialValue; }
            set
            {
                if (value != m_specialValue)
                {
                    m_specialValue = value;
                    this.Refresh();
                }
            }
        }
        #endregion

        #region Constructeur
        /// <summary>
        /// Instancie une grille lumineuse; les cellules sont booléennes, soit allumées ou éteintes.
        /// </summary>
        public VisualBoolArray()
        {
            m_defaultValue = false;
            m_specialValue = true;
            va_enabledAppearance = new VBACellAppearance(this);
            UpdateCellsBkgVisualElement();
            va_disabledAppearance = new DisabledAppearance(this); // PAS NÉCESSAIRE ????
            UpdateDisableVisualElement(va_disabledAppearance.Style); // PAS NÉCESSAIRE ????
            va_specialValueAppearance = new VBASpecialValueAppearance<bool>(this);
            UpdateSpecialValueVisualElement(va_specialValueAppearance.Style);
            InitializeComponent();
        }
        #endregion

        #region DrawCellDragContent et DrawCellContent
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

            bool valeurAAfficher = va_tabValues[pRow, pColumn];

            if (valeurAAfficher == m_specialValue)
            {
                if (va_specialValueVisualElement != null)
                    va_specialValueVisualElement.Draw(pGraphics, pContentBounds);
            }
            else
                if (cell.Background != null) cell.Background.Draw(pGraphics, pContentBounds);
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

                bool valeurAAfficher = va_tabValues[pRow, pColumn];

                if (cell.LayerUnder != null)
                    cell.LayerUnder.Draw(pGraphics, cellContentBounds);

                // 2013-02-28 -----------------------------------------------------------------------------------
                if (cell.Enabled) // la cellule est active
                {
                    if (valeurAAfficher == m_specialValue) // la valeur est spéciale
                    {
                        if (va_specialValueVisualElement != null) // autrement pas de fond pour la specialValue
                            va_specialValueVisualElement.Draw(pGraphics, cellContentBounds);
                    }
                    else // la valeur est normale
                    {
                        if (cell.Background != null)
                            cell.Background.Draw(pGraphics, cellContentBounds);
                    }
                }
                else // la cellule est inactive
                {
                    if (va_disabledVisualElement != null)
                        va_disabledVisualElement.Draw(pGraphics, cellContentBounds);
                }
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

        #region Méthodes override

        //============================================================================================
        /// <summary>
        /// Se produit lorsque le MouseWheel change de valeur
        /// </summary>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (SelectedIndex != -1 && !ReadOnly && va_selectionMode == SelectionMode.One && !va_tabCells[SelectedAddress.Row,SelectedAddress.Column].ReadOnly)
                this[SelectedIndex] = !this[SelectedIndex];
        }
        //============================================================================================
        /// <summary>
        /// Accepte tous les caractères saisies au clavier
        /// </summary>
        /// <param name="e">infos sur la touche pressée</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            //if (SelectedIndex != -1 && !va_readOnly && va_selectionMode != SelectionMode.None && !va_tabCells[SelectedAddress.Row,SelectedAddress.Column].ReadOnly)
            //    if (e.KeyChar == (char)Keys.Enter)
            //        this[SelectedIndex] = !this[SelectedIndex];
            base.OnKeyPress(e);
        }
        ///// <summary>
        ///// 
        ///// </summary>
        //public void SpeedTest()
        //{
        //    for (int ligne = 0; ligne < va_tabCellules.GetLength(0); ligne++)
        //        for (int colonne = 0; colonne < va_tabCellules.GetLength(1); colonne++)
        //            va_tabCellules[ligne, colonne] = true;
        //    Refresh();
        //}
        #endregion

        #region RandomValue
        /// ----------------------------------------------------------------------------------
        /// <summary>
        /// Génère aléatoirement la valeur true ou false.
        /// </summary>
        /// <returns>True ou false.</returns>
        public bool RandomValue()
        {
            return VisualArraysTools.RandomBool();
        }
        #endregion

        #region SpecialValueAppearance
        /// <summary>
        /// Détermine différents aspects de l'apparence de la valeur spéciale
        /// </summary>
        protected new VBASpecialValueAppearance<bool> va_specialValueAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence de la valeur spéciale
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("VisualArrays")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new VBASpecialValueAppearance<bool> SpecialValueAppearance
        {
            get { return va_specialValueAppearance; }
            set
            {
                if (value != va_specialValueAppearance)
                {
                    va_specialValueAppearance = value;
                    UpdateSpecialValueVisualElement(va_specialValueAppearance.Style);
                    this.Refresh();
                }
            }
        }
        #endregion

        #region EnabledAppearance
        /// <summary>
        /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est true
        /// </summary>
        protected new VBACellAppearance va_enabledAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est true
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("CellAppearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new VBACellAppearance EnabledAppearance
        {
            get { return va_enabledAppearance; }
            set
            {
                va_enabledAppearance = value;
                UpdateCellsBkgVisualElement();
            }
        }
        //============================================================================================
        internal override void UpdateCellsBkgVisualElement()
        {
            UpdateCellsBkgVisualElement(va_enabledAppearance);
        }
        //============================================================================================
        internal override void UpdateSpecialValueVisualElement(enuBkgStyle pStyle)
        {
            va_specialValueVisualElement = UpdateBackgroundVisualElement(va_specialValueAppearance);
            //switch (pStyle)
            //{
            //    case enuBkgStyle.None:
            //        va_specialValueVisualElement = null;
            //        break;
            //    case enuBkgStyle.Border:
            //        va_specialValueVisualElement = new BorderElement(va_specialValueAppearance.Border, va_specialValueAppearance.BackgroundColor);
            //        break;
            //    case enuBkgStyle.FillShape:
            //        va_specialValueVisualElement = new FillShapeElement(va_specialValueAppearance.Shape, va_specialValueAppearance.BackgroundColor);
            //        break;
            //    case enuBkgStyle.Shape:
            //        va_specialValueVisualElement = new ShapeElement(va_specialValueAppearance.Shape, va_specialValueAppearance.PenWidth, va_specialValueAppearance.BackgroundColor);
            //        break;
            //    case enuBkgStyle.Image:
            //        va_specialValueVisualElement = new ImageElement(va_specialValueAppearance.Image);
            //        break;
            //}
        }
        #endregion

        #region DisabledAppearance
        /// <summary>
        /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est false
        /// </summary>
        protected new DisabledAppearance va_disabledAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est false
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("VisualArrays")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new DisabledAppearance DisabledAppearance
        {
            get { return va_disabledAppearance; }
            set
            {
                va_disabledAppearance = value;
                UpdateDisableVisualElement(value.Style);
                this.Refresh();
            }
        }

        //============================================================================================
        internal override void UpdateDisableVisualElement(enuBkgStyle pStyle)
        {
            va_disabledVisualElement = UpdateBackgroundVisualElement(va_disabledAppearance);
            //switch (pStyle)
            //{
            //    case enuBkgStyle.None:
            //        va_disabledVisualElement = null;
            //        break;
            //    case enuBkgStyle.Border:
            //        va_disabledVisualElement = new BorderElement(va_disabledAppearance.Border, va_disabledAppearance.BackgroundColor);
            //        break;
            //    case enuBkgStyle.FillShape:
            //        va_disabledVisualElement = new FillShapeElement(va_disabledAppearance.Shape, va_disabledAppearance.BackgroundColor);
            //        break;
            //    case enuBkgStyle.Shape:
            //        va_disabledVisualElement = new ShapeElement(va_disabledAppearance.Shape, va_disabledAppearance.PenWidth, va_disabledAppearance.BackgroundColor);
            //        break;
            //    case enuBkgStyle.Image:
            //        va_disabledVisualElement = new ImageElement(va_disabledAppearance.Image);
            //        break;
            //}
        }
        #endregion
    }
}

using System.ComponentModel;
using VisualArrays.Others;

namespace VisualArrays.VisualArrays
{
    /// <summary>
    /// Représente une grille permettant l'affichage de nombres  
    /// </summary>
    [ToolboxBitmap(typeof(VisualDecimalArray), "Resources.tbxVisualDecimalArray")]
    public partial class VisualDecimalArray : VisualGraphArray<decimal>
    {
        #region CHAMPS ET PROPRIÉTÉS
        //============================================================================================
        //private decimal m_defaultValue = 0;
        /// <summary>
        /// Obtient ou définit la valeur par défaut de toutes les cellules.
        /// </summary>
        [DefaultValue(typeof(decimal), "0"), Category("VisualArrays"), Browsable(true), Description("Valeur par défaut de toutes les cellules")]
        public decimal DefaultValue
        {
            get => m_defaultValue;
            set
            {
                if (value < va_minimum || value > va_maximum)
                    throw new ArgumentOutOfRangeException("DefaultValue", value, "'DefaultValue' doit être compris entre 'Minimum' et 'Maximum'");

                if (value != m_defaultValue)
                {
                    m_defaultValue = value;
                    ResetAllValuesToDefault();
                    this.Refresh();
                }
            }
        }

        //============================================================================================
        //private decimal m_specialValue = -1;
        /// <summary>
        /// Obtient ou définit la valeur spéciale à afficher différement des autres valeurs.
        /// </summary>
        [DefaultValue(typeof(decimal), "-1"), Category("VisualArrays"), Browsable(true), Description("Valeur spéciale à afficher différemment des autres valeurs, voir SpecialValueAppearance")]
        public decimal SpecialValue
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
        private enuDecimalView va_view = enuDecimalView.Number;
        /// <summary>
        /// Obtient et définit le style de visualisation pour les valeurs de la grille
        /// </summary>
        [DefaultValue(enuDecimalView.Number), Category("CellAppearance"), Browsable(true), Description("Obtient et définit le style de visualisation pour les valeurs de la grille.")]
        public enuDecimalView View
        {
            get => va_view;
            set
            {
                va_view = value;
                Refresh();
            }
        }
        //============================================================================================
        private decimal va_minimum = -1;
        /// <summary>
        /// Obtient et définit la valeur minimale pour toutes les cellules de la grille
        /// </summary>
        [DefaultValue(typeof(decimal), "-1"), Category("VisualArrays"), Browsable(true), Description("Obtient et définit la valeur minimale pour toutes les cellules de la grille.")]
        [RefreshProperties(RefreshProperties.All)]
        public decimal Minimum
        {
            get => va_minimum;
            set
            {
                va_minimum = value;
                if (va_minimum > va_maximum)
                    Maximum = value;
                //if (!DesignMode)
                Refresh();
            }
        }
        //============================================================================================
        private decimal va_maximum = 100;
        /// <summary>
        /// Obtient ou définit la valeur maximale pour toutes les cellules de la grille
        /// </summary>
        [DefaultValue(typeof(decimal), "100"), Category("VisualArrays"), Browsable(true), Description("Obtient et définit la valeur maximale pour toutes les cellules de la grille.")]
        [RefreshProperties(RefreshProperties.All)]
        public decimal Maximum
        {
            get => va_maximum;
            set
            {
                va_maximum = value;
                if (va_maximum < va_minimum)
                    Minimum = value;
                //if (!DesignMode)
                Refresh();
            }
        }
        //============================================================================================
        private enuValueFormat va_valueFormat = enuValueFormat.Standard;
        /// <summary>
        /// Obtient et définit le format d'affichage des valeurs dans la grille
        /// </summary>
        [DefaultValue(enuValueFormat.Standard), Category("VisualArrays"), Browsable(true), Description("Obtient et définit le format d'affichage des valeurs dans la grille.")]
        public enuValueFormat ValueFormat
        {
            get => va_valueFormat;
            set
            {
                va_valueFormat = value;
                Refresh();
            }
        }
        //============================================================================================
        private int va_decimalPlaces = 1;
        /// <summary>
        /// Obtient ou définit le nombre de décimales à afficher.
        /// </summary>
        [DefaultValue(1), Category("VisualArrays"), Browsable(true), Description("Indique le nombre de décimales à afficher.")]
        public int DecimalPlaces
        {
            get => va_decimalPlaces;
            set
            {
                va_decimalPlaces = value;
                this.Refresh();
            }
        }
        #endregion

        #region Constructeur
        //===============================================================================
        /// <summary>
        /// Initialise une nouvelle instance de la grille de digits avec les valeurs par défaut.
        /// </summary>
        public VisualDecimalArray()
        {
            m_defaultValue = 0;
            m_specialValue = -1;
            InitializeComponent();
        }
        #endregion

        #region OnMouseWheel, OnKeyPress, OnMouseEnter, OnMouseLeave
        //============================================================================================
        /// <summary>
        /// Se produit lorsque le MouseWheel change de valeur
        /// </summary>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (SelectedIndex != -1 && !ReadOnly && va_selectionMode == SelectionMode.One && !va_tabCells[SelectedAddress.Row, SelectedAddress.Column].ReadOnly)
            {
                decimal delta = (va_maximum - va_minimum) / 10;
                decimal minDelta = 1 / (decimal)Math.Pow(10, va_decimalPlaces);
                if (delta < minDelta)
                    delta = minDelta;
                decimal valeur = this[SelectedIndex];
                if (e.Delta > 0)
                    valeur += delta;
                else if (e.Delta < 0)
                    valeur -= delta;
                if (valeur < va_minimum) valeur = va_minimum;
                if (valeur > va_maximum) valeur = va_maximum;
                this[SelectedIndex] = valeur;
            }
        }
        //============================================================================================
        //private bool va_fraction = false;
        //private int va_digitFraction = 0;
        //private bool va_negatif = false;
        /// <summary>
        /// Accepte les touches 0 à 9 pour la saisie de nombres.
        /// </summary>
        /// <param name="e">infos sur la touche pressée</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (SelectedIndex != -1 && !va_readOnly && va_selectionMode != SelectionMode.None && !va_tabCells[SelectedAddress.Row, SelectedAddress.Column].ReadOnly)
            {
                decimal valeur;
                if (VisualArraysTools.ReadDecimal(e.KeyChar, this[SelectedIndex], va_maximum, va_decimalPlaces, out valeur))
                {
                    if (valeur < va_minimum) valeur = va_minimum;
                    if (valeur > va_maximum) valeur = va_maximum;
                    this[SelectedIndex] = valeur;
                }
            }
            base.OnKeyPress(e);
        }
        //============================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            //this.Cursor = Cursors.Hand;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            //this.Cursor = Cursors.Default;
        }
        #endregion

        #region DrawCellContent
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

                decimal valeurAAfficher = va_tabValues[pRow, pColumn];

                if (cell.LayerUnder != null)
                    cell.LayerUnder.Draw(pGraphics, cellContentBounds);

                // 2013-02-28 ------------------------------------------------------------------------------------------
                if (cell.Enabled) // la cellule est active.
                {
                    if (valeurAAfficher == m_specialValue) // la valeur est spéciale
                    {
                        if (va_specialValueVisualElement != null) // autrement pas de fond pour la specialValue
                            va_specialValueVisualElement.Draw(pGraphics, cellContentBounds);
                    }
                    else // une valeur normale
                    {
                        if (cell.Background != null)
                            cell.Background.Draw(pGraphics, cellContentBounds);
                    }
                }
                else // il s'agit d'une cellule inactive, quelle soit spéciale ou pas
                {
                    if (va_disabledVisualElement != null)
                        va_disabledVisualElement.Draw(pGraphics, cellContentBounds);
                }

                #region Code pour dessiner la valeur de la cellule

                string laChaine;
                if (va_valueFormat == enuValueFormat.Standard)
                    laChaine = valeurAAfficher.ToString("F" + va_decimalPlaces);
                else if (va_valueFormat == enuValueFormat.Currency)
                    laChaine = valeurAAfficher.ToString("C" + va_decimalPlaces);
                else
                    laChaine = (valeurAAfficher * 100).ToString("F" + va_decimalPlaces) + "%";

                Rectangle displayRectangle = GetCellContentBounds(pRow, pColumn, EnabledAppearance.Padding);
                if (!DesignMode || va_addressView == enuAddressView.None)
                    switch (va_view)
                    {
                        case enuDecimalView.Number:
                            if (cell.Enabled) // la cellule est active
                            {
                                if (valeurAAfficher == m_specialValue) // c'est la valeur spéciale
                                {
                                    if (SpecialValueAppearance.ShowValue)
                                        VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, SpecialValueAppearance.TextColor, SpecialValueAppearance.Font, m_cellContentAlign);
                                }
                                else // c'est une valeur normale
                                    VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                            }
                            else // une cellule inactive
                            {
                                if (va_disabledAppearance.ShowValue)
                                    VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, DisabledAppearance.TextColor, DisabledAppearance.Font, m_cellContentAlign);
                            }
                            break;
                        case enuDecimalView.Graph:
                        case enuDecimalView.GraphNumber:

                            VisualArraysTools.DrawBar(pGraphics, cellContentBounds, GraphAppearance, va_minimum, va_maximum, valeurAAfficher,va_decimalPlaces);
                            if (va_view == enuDecimalView.GraphNumber)
                            {
                                // Affichage de la valeur principale de la cellule : soit une valeur normale ou une valeur spéciale
                                if (cell.Enabled)
                                {
                                    if (valeurAAfficher != m_specialValue)
                                        VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                                    else if (SpecialValueAppearance.ShowValue) // c'est la valeur spéciale
                                        VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, SpecialValueAppearance.TextColor, SpecialValueAppearance.Font, m_cellContentAlign);
                                }
                                else if (va_disabledAppearance.ShowValue)
                                    VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, DisabledAppearance.TextColor, DisabledAppearance.Font, m_cellContentAlign);
                            }
                            break;
                        default:
                            break;
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

        #region Clear()
        // CLEAR ------------------------------------------------------------------------------
        /// <summary>
        /// Vide le contenu de la cellule dont l'index est fournit en paramètre.
        /// </summary>
        /// <param name="pIndex">index de la cellule à vider</param>
        /// ----------------------------------------------------------------------------------
        public override void Clear(int pIndex)
        {
            if (va_minimum <= m_defaultValue && va_maximum >= m_defaultValue)
                this[pIndex] = m_defaultValue;
        }
        // CLEAR ------------------------------------------------------------------------------
        /// <summary>
        /// Vide le contenu de la cellule dont la rangée et la colonne sont spécifiées.
        /// </summary>
        /// <param name="pRow">rangée de la cellule à vider</param>
        /// <param name="pColumn">colonne de la cellule à vider</param>
        /// ----------------------------------------------------------------------------------
        public override void Clear(int pRow, int pColumn)
        {
            if (va_minimum <= m_defaultValue && va_maximum >= m_defaultValue)
                this[pRow, pColumn] = m_defaultValue;
        }
        #endregion

        #region RandomValue
        /// <summary>
        /// Génère un nombre aléatoire décimal, choisi entre les valeurs des propriétés 'Minimum' et 'Maximum' 
        /// inclusivement.
        /// </summary>
        /// <returns>Un nombre aléatoire décimal entre 'Minimum' et 'Maximum' inclusivement</returns>
        public decimal RandomValue()
        {
            decimal facteur = (decimal)Math.Pow(10, va_decimalPlaces);
            int nombreAléatoire = va_objRandom.Next((int)(va_minimum * facteur), (int)(va_maximum * facteur + 1));
            return nombreAléatoire / facteur;
        }
        /// <summary>
        /// Génère un nombre décimal aléatoire.
        /// </summary>
        /// <param name="pMin">Borne inférieure inclue dans l'intervalle</param>
        /// <param name="pMax">Borne supérieure inclue dans l'intervalle</param>
        /// <returns>Un nombre aléatoire décimal entre pMin et pMax</returns>
        public decimal RandomValue(decimal pMin, decimal pMax)
        {
            decimal facteur = (decimal)Math.Pow(10, va_decimalPlaces);
            int nombreAléatoire = va_objRandom.Next((int)(pMin * facteur), (int)(pMax * facteur + 1));
            return nombreAléatoire / facteur;
        }
        #endregion

        #region Indexeurs
        /// <summary>
        /// Obtient ou définit le nombre à l'index spécifié en tenant compte du ModeAdressage.
        /// </summary>
        /// <param name="pIndex">index du digit</param>
        /// <returns>nombre à l'index spécifié</returns>
        public override decimal this[int pIndex]
        {
            get
            {
                Address adresse = IndexToAddress(pIndex);
                return va_tabValues[adresse.Row, adresse.Column];
            }
            set
            {
                BeforeValueChangedArgs<decimal> beforeValueChangedArgs = AcceptBeforeValueChanged(value);
                if (!beforeValueChangedArgs.AcceptValueChanged) return;
                value = beforeValueChangedArgs.NewValue;

                if (value < va_minimum || value > va_maximum)
                    throw new VisualArrayException("La valeur '" + value + "' n'est pas valide pour la cellule, elle doit être comprise entre 'Minimum' et 'Maximum'");

                Address adresse = IndexToAddress(pIndex);
                va_tabValues[adresse.Row, adresse.Column] = value;
                UpdateCellAndSprites(pIndex);
                //DessinerCellule(this.CreateGraphics(), adresse.Y, adresse.X);
                adresse = AddressFromAddressMode(adresse.Row, adresse.Column);
                SendValueChanged(pIndex, adresse.Row, adresse.Column,adresse);
            }
        }
        /// <summary>
        /// Obtient ou définit le nombre à la cellule dont la ligne et la colonne 
        /// sont spécifiées en tenant compte du mode d'adressage.
        /// </summary>
        /// <param name="pRow">rangée de la cellule à traiter</param>
        /// <param name="pColumn">colonne de la cellule à traiter</param>
        /// <returns>nombre à la coordonnée spécifié</returns>
        public override decimal this[int pRow, int pColumn]
        {
            get
            {
                Address adresse = AddressFromAddressMode(pRow, pColumn);
                return va_tabValues[adresse.Row, adresse.Column];
            }
            set
            {
                BeforeValueChangedArgs<decimal> beforeValueChangedArgs = AcceptBeforeValueChanged(value);
                if (!beforeValueChangedArgs.AcceptValueChanged) return;
                value = beforeValueChangedArgs.NewValue;

                if (value < va_minimum || value > va_maximum)
                    throw new VisualArrayException("La valeur '" + value + "' n'est pas valide pour la cellule, elle doit être comprise entre 'Minimum' et 'Maximum'");
                Address adresse = AddressFromAddressMode(pRow, pColumn);
                va_tabValues[adresse.Row, adresse.Column] = value;
                int index = IndexFromAddress(adresse.Row, adresse.Column);
                UpdateCellAndSprites(index);
                //DessinerCellule(this.CreateGraphics(), adresse.Y, adresse.X);
                SendValueChanged(index, pRow, pColumn, new Address(pRow, pColumn));
            }
        }
        #endregion

        internal override void SetValue(int pIndex, int pPixelOffset)
        {
            if (va_allowGraphClick)
                this[pIndex] = (va_maximum - va_minimum) / 2 + va_minimum;
        }
    }
}
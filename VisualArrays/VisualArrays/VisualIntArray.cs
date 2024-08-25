using System.ComponentModel;
using VisualArrays.Others;

namespace VisualArrays.VisualArrays
{
    /// <summary>
    /// Repr�sente une grille permettant l'affichage par segments de chiffres ou de symboles.  
    /// </summary>
    [ToolboxBitmap(typeof(VisualIntArray), "Resources.tbxVisualIntArray")]
    public partial class VisualIntArray : VisualGraphArray<int>
    {

        #region Champs et Propri�t�s
        //============================================================================================
        //private int m_defaultValue = 0;
        /// <summary>
        /// Obtient ou d�finit la valeur par d�faut de toutes les cellules.
        /// </summary>
        [DefaultValue(0), Category("VisualArrays"), Browsable(true), Description("Valeur par d�faut de toutes les cellules")]
        public int DefaultValue
        {
            get => m_defaultValue;
            set
            {
                if (value < va_minimum || value > va_maximum)
                    throw new ArgumentOutOfRangeException("DefaultValue", value, "'DefaultValue' doit �tre compris entre 'Minimum' et 'Maximum'");

                if (value != m_defaultValue)
                {
                    m_defaultValue = value;
                    ResetAllValuesToDefault();
                    this.Refresh();
                }
            }
        }

        //============================================================================================
        //private int m_specialValue = -1;
        /// <summary>
        /// Obtient ou d�finit la valeur sp�ciale � afficher diff�rement des autres valeurs.
        /// </summary>
        [DefaultValue(-1), Category("VisualArrays"), Browsable(true), Description("Valeur sp�ciale � afficher diff�remment des autres valeurs, voir SpecialValueAppearance")]
        public int SpecialValue
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
        private Color va_digitColor = Color.Red;
        /// <summary>
        /// Obtient et d�finit la couleur des segments allum�s.
        /// </summary>
        [DefaultValue(typeof(Color), "Red"), Category("VisualArrays"), Browsable(true), Description("Couleur des segments allum�s.")]
        public Color DigitColor
        {
            get => va_digitColor;
            set
            {
                va_digitColor = value;
                GridAppearance.Color = VisualArraysTools.CalculerCouleurEteinte(value);
            }
        }
        //============================================================================================
        private int va_minimum = -1; // autrement il n'est pas possible de mettre DefaultValue � -1
        /// <summary>
        /// Obtient et d�finit la valeur minimale pour toutes les cellules de la grille
        /// </summary>
        [DefaultValue(typeof(int), "-1"), Category("VisualArrays"), Browsable(true), Description("Obtient et d�finit la valeur minimale pour toutes les cellules de la grille.")]
        [RefreshProperties(RefreshProperties.All)]
        public int Minimum
        {
            get => va_minimum;
            set
            {
                if (value != va_minimum)
                {
                    va_minimum = value;

                    if (m_defaultValue < va_minimum)
                        DefaultValue = va_minimum;
                    if (va_minimum > va_maximum)
                        Maximum = value;
                    //if (!DesignMode)
                    Refresh();
                }
            }
        }
        //============================================================================================
        private int va_maximum = 100;
        /// <summary>
        /// Obtient ou d�finit la valeur maximale pour toutes les cellules de la grille
        /// </summary>
        [DefaultValue(typeof(int), "100"), Category("VisualArrays"), Browsable(true), Description("Obtient et d�finit la valeur maximale pour toutes les cellules de la grille.")]
        [RefreshProperties(RefreshProperties.All)]
        public int Maximum
        {
            get => va_maximum;
            set
            {
                if (value != va_maximum)
                {
                    va_maximum = value;
                    if (m_defaultValue > va_maximum)
                        DefaultValue = va_maximum;
                    if (va_maximum < va_minimum)
                        Minimum = value;
                    //if (!DesignMode)
                    Refresh();
                }
            }
        }
        //============================================================================================
        private enuIntView va_view = enuIntView.Number;
        /// <summary>
        /// Obtient et d�finit le style de visualisation pour les valeurs de la grille
        /// </summary>
        [DefaultValue(enuIntView.Number), Category("CellAppearance"), Browsable(true), Description("Obtient et d�finit le style de visualisation pour les valeurs de la grille.")]
        public enuIntView View
        {
            get => va_view;
            set
            {
                va_view = value;
                Refresh();
            }
        }
        #endregion

        #region Constructeur
        //===============================================================================
        /// <summary>
        /// Initialise une nouvelle instance de la grille avec les valeurs par d�faut.
        /// </summary>
        public VisualIntArray()
        {
            m_defaultValue = 0;
            m_specialValue = -1;
            InitializeComponent();
        }
        #endregion

        #region M�thodes
        // CLEAR ------------------------------------------------------------------------------
        /// <summary>
        /// Vide le contenu de la cellule dont l'index est fournit en param�tre.
        /// </summary>
        /// <param name="pIndex">index de la cellule � vider</param>
        /// ----------------------------------------------------------------------------------
        public override void Clear(int pIndex)
        {
            if (va_minimum <= m_defaultValue && va_maximum >= m_defaultValue)
                this[pIndex] = m_defaultValue;
        }
        // CLEAR ------------------------------------------------------------------------------
        /// <summary>
        /// Vide le contenu de la cellule dont la rang�e et la colonne sont sp�cifi�es.
        /// </summary>
        /// <param name="pRow">rang�e de la cellule � vider</param>
        /// <param name="pColumn">colonne de la cellule � vider</param>
        /// ----------------------------------------------------------------------------------
        public override void Clear(int pRow, int pColumn)
        {
            if (va_minimum <= m_defaultValue && va_maximum >= m_defaultValue)
                this[pRow, pColumn] = m_defaultValue;
        }
        //============================================================================================
        /// <summary>
        /// Se produit lorsque le MouseWheel change de valeur
        /// </summary>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (SelectedIndex != -1 && !ReadOnly && va_selectionMode == SelectionMode.One && !va_tabCells[SelectedAddress.Row,SelectedAddress.Column].ReadOnly)
            {
                int delta = (va_maximum - va_minimum) / 10;
                if (delta < 1) delta = 1;
                int valeur = this[SelectedIndex];
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
        /// <summary>
        /// Accepte les touches 0 � 9 pour la saisie de nombres.
        /// </summary>
        /// <param name="e">infos sur la touche press�e</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (SelectedIndex != -1 && !va_readOnly && va_selectionMode != SelectionMode.None && !va_tabCells[SelectedAddress.Row, SelectedAddress.Column].ReadOnly)
            {
                int valeur;
                if (VisualArraysTools.ReadInt(e.KeyChar, this[SelectedIndex], va_maximum, out valeur))
                {
                    if (valeur < va_minimum) valeur = va_minimum;
                    if (valeur > va_maximum) valeur = va_maximum;
                    this[SelectedIndex] = valeur;
                }
            }
            base.OnKeyPress(e);
        }
        #endregion

        #region DrawCellDragContent et DrawCellContent
        //--------------------------------------------------------------------------------------
        /// <summary>
        /// Dessiner le contenu d'une cellule dans une zone de la taille de la cellule, 
        /// servant � l'op�ration glisser/d�poser.
        /// </summary>
        /// <param name="pGraphics"></param>
        /// <param name="pContentBounds">Contour du contenu de la cellule</param>
        /// <param name="pRow">Rang�e</param>
        /// <param name="pColumn">Colonne</param>
        protected override void DrawCellDragContent(Graphics pGraphics, Rectangle pContentBounds, int pRow, int pColumn)
        {
            Cell cell = va_tabCells[pRow, pColumn];
            pContentBounds = new Rectangle(0, 0, pContentBounds.Width, pContentBounds.Height);

            int valeurAAfficher = va_tabValues[pRow, pColumn];
            // Dessiner le fond de la cellule si n�cessaire
            if (valeurAAfficher == m_specialValue)
            {
                va_specialValueVisualElement?.Draw(pGraphics, pContentBounds);
            }
            //else
            //{
            //    if (cell.Background != null) 
            //        cell.Background.Draw(pGraphics, pContentBounds);
            //}

            pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            string laChaine = valeurAAfficher.ToString(); 
            switch (va_view)
            {
                case enuIntView.Number:
                    if (valeurAAfficher != m_specialValue)
                        DrawText(pGraphics, pContentBounds, laChaine, EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                    else if (SpecialValueAppearance.ShowValue) // c'est la valeur sp�ciale
                        DrawText(pGraphics, pContentBounds, laChaine, SpecialValueAppearance.TextColor, SpecialValueAppearance.Font, m_cellContentAlign);
                    break;
                case enuIntView.Graph:
                case enuIntView.GraphNumber:
                    //Rectangle displayRectangle = GetCellContentBounds(pRow, pColumn, EnabledAppearance.Padding);
                    Rectangle displayRectangle = pContentBounds;
                    VisualArraysTools.DrawBar(pGraphics, pContentBounds, GraphAppearance, va_minimum, va_maximum, valeurAAfficher);
                    if (va_view == enuIntView.GraphNumber)
                    {
                        // Affichage de la valeur principale de la cellule : soit une valeur normale ou une valeur sp�ciale
                        if (cell.Enabled)
                        {
                            if (valeurAAfficher != m_specialValue)
                                VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                            else if (SpecialValueAppearance.ShowValue) // c'est la valeur sp�ciale
                                VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, SpecialValueAppearance.TextColor, SpecialValueAppearance.Font, m_cellContentAlign);
                        }
                        else if (va_disabledAppearance.ShowValue)
                            VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, DisabledAppearance.TextColor, DisabledAppearance.Font, m_cellContentAlign);
                    }
                    break;
                case enuIntView.Digit:
                    break;
                case enuIntView.ImageList:
                    // Ne pas dessiner l'image si la valeur � afficher est la valeur sp�ciale
                    if (valeurAAfficher == m_specialValue) break;

                    ImageList imageList = va_enabledAppearance.ImageList;

                    if (imageList != null)
                    {
                        if (valeurAAfficher >= 0 && valeurAAfficher < imageList.Images.Count)
                        {
                            Image objImage = imageList.Images[valeurAAfficher];
                            Rectangle imageBounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(pContentBounds, objImage.Size, m_cellContentAlign);
                            if (cell.Enabled)
                                pGraphics.DrawImage(objImage, new Point(imageBounds.Left, imageBounds.Top));
                            else
                                VisualArraysTools.DrawDisabledImage(pGraphics, imageBounds, objImage, DisabledAppearance.ImageBrightness);
                        }
                        else
                        {
                            pGraphics.FillRectangle(new SolidBrush(BackColor), pContentBounds);
                            //pGraphics.SetClip(cellContentBounds); // Pour ne pas dessiner l'ext�rieur des cellules
                            //DrawText(pGraphics, pRow, pColumn, "?", CellAppearance.TextColor, CellAppearance.Font, m_cellContentAlign);
                        }
                    }
                    //else // il n'y pas d'imageList d'associ� avec la grille
                    //  DrawText(pGraphics, pRow, pColumn, "?", CellAppearance.TextColor, CellAppearance.Font, m_cellContentAlign);
                    break;
                default:
                    break;
            }
        }
        //============================================================================================
        /// <summary>
        /// Dessine la cellule � l'adresse pRow et pColumn.
        /// </summary>
        /// <param name="pGraphics">Objet graphique o� dessiner.</param>
        /// <param name="pRow">Rang�e.</param>
        /// <param name="pColumn">Colonne.</param>
        protected override void DrawCellContent(Graphics pGraphics, int pRow, int pColumn)
        {
            Rectangle cellContentBounds = GetCellContentBounds(pRow, pColumn);
            Rectangle cellBounds = GetCellBounds(pRow, pColumn);
            Cell cell = va_tabCells[pRow, pColumn];

            // �tape 1 : On commence par dessiner le fond de la grille
            if (BackgroundImage != null)
                pGraphics.DrawImage(BackgroundImage, cellBounds, cellBounds, GraphicsUnit.Pixel);
            else
                pGraphics.FillRectangle(new SolidBrush(BackColor), cellBounds);

            // �tape 2 : Si la cellule n'est pas visible, alors on quitte (sans m�me afficher son adresse)
            if (!cell.Visible) return;


            // �tape 3 : Si l'utilisateur d�sire dessiner lui m�me le contenu de la cellule
            if (cell.UserContent != null)
            {
                pGraphics.SetClip(cellBounds); // Pour ne pas dessiner l'ext�rieur des cellules
                cell.UserContent.DrawCellContent(pGraphics, cellContentBounds, cellBounds, cell.Enabled);
            }
            else
            {   // �tape 3B : Selon l'�tat de la cellule Enabled == true
                pGraphics.SetClip(cellContentBounds); // Pour ne pas dessiner l'ext�rieur des cellules

                int valeurAAfficher = va_tabValues[pRow, pColumn];

                cell.LayerUnder?.Draw(pGraphics, cellContentBounds);

                // 2013-02-28 ------------------------------------------------------------------------------------------
                if (cell.Enabled) // la cellule est active.
                {
                    if (valeurAAfficher == m_specialValue) // la valeur est sp�ciale
                    {
                        va_specialValueVisualElement?.Draw(pGraphics, cellContentBounds);
                    }
                    else // une valeur normale
                    {
                        cell.Background?.Draw(pGraphics, cellContentBounds);
                    }
                }
                else // il s'agit d'une cellule inactive
                {
                    va_disabledVisualElement?.Draw(pGraphics, cellContentBounds);
                }

                //---------------------------------------------------------------------------------------------------
                #region Code pour dessiner la valeur de la cellule

                string laChaine = valeurAAfficher.ToString();

                Rectangle displayRectangle = GetCellContentBounds(pRow, pColumn, EnabledAppearance.Padding);
                if (!DesignMode || va_addressView == enuAddressView.None)
                    switch (va_view)
                    {
                        case enuIntView.Number:
                            if (cell.Enabled) // la cellule est active
                            {
                                if (valeurAAfficher == m_specialValue) // c'est la valeur sp�ciale
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
                        case enuIntView.Graph:
                        case enuIntView.GraphNumber:
                            VisualArraysTools.DrawBar(pGraphics, cellContentBounds, GraphAppearance, va_minimum, va_maximum, valeurAAfficher);
                            if (va_view == enuIntView.GraphNumber)
                            {
                                // Affichage de la valeur principale de la cellule : soit une valeur normale ou une valeur sp�ciale
                                if (cell.Enabled)
                                {
                                    if (valeurAAfficher != m_specialValue)
                                        VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                                    else if (SpecialValueAppearance.ShowValue) // c'est la valeur sp�ciale
                                        VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, SpecialValueAppearance.TextColor, SpecialValueAppearance.Font, m_cellContentAlign);
                                }
                                else if (va_disabledAppearance.ShowValue)
                                    VisualArraysTools.DrawText(pGraphics, displayRectangle, laChaine, DisabledAppearance.TextColor, DisabledAppearance.Font, m_cellContentAlign);
                            }
                            break;
                        case enuIntView.Digit:
                            //DessinerSegments(pGraphics, cellContentBounds, valeurAAfficher);
                            VisualArraysTools.DessinerSegments(pGraphics, cellContentBounds, valeurAAfficher, va_digitColor);
                            break;
                        case enuIntView.ImageList:
                            // Ne pas dessiner l'image si la valeur � afficher est la valeur sp�ciale
                            if (valeurAAfficher == m_specialValue) break;

                            ImageList imageList;
                            if (cell.Enabled)
                                imageList = va_enabledAppearance.ImageList;
                            else
                            { // on va utiliser le m�me ImageList que pour les cellules actives
                                imageList = va_disabledAppearance.ImageList;
                                if (imageList == null)
                                    imageList = va_enabledAppearance.ImageList;
                            }

                            if (imageList != null)
                            {
                                if (valeurAAfficher >= 0 && valeurAAfficher < imageList.Images.Count)
                                {
                                    Image objImage = imageList.Images[valeurAAfficher];
                                    Rectangle imageBounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(cellContentBounds, objImage.Size, m_cellContentAlign);
                                    if (cell.Enabled)
                                        pGraphics.DrawImage(objImage, new Point(imageBounds.Left, imageBounds.Top));
                                    else
                                        VisualArraysTools.DrawDisabledImage(pGraphics, imageBounds, objImage, DisabledAppearance.ImageBrightness);
                                }
                                else
                                {
                                    pGraphics.FillRectangle(new SolidBrush(BackColor), cellContentBounds);
                                    //pGraphics.SetClip(cellContentBounds); // Pour ne pas dessiner l'ext�rieur des cellules
                                    VisualArraysTools.DrawText(pGraphics, displayRectangle, "?", EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                                }
                            }
                            else // il n'y pas d'imageList d'associ� avec la grille
                                VisualArraysTools.DrawText(pGraphics, displayRectangle, "?", EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                            break;
                        default:
                            break;
                    }
                #endregion

            }
            // �tape 4 : On va dessiner les couches suppl�mentaires soit les VisualElement ajout�s
            CellVisualElement.CellVisualElement layerVE = cell.LayerOver;
            while (layerVE != null)
            {
                layerVE.Draw(pGraphics, cellContentBounds);
                layerVE = layerVE.NextVisualElement;
            }

            pGraphics.ResetClip();

            // �tape 5 : Si la cellule est inactive et qu'une raillure doit �tre dessin�e
            if (!cell.Enabled && va_disabledAppearance.StrikeAppearance.Style != enuStrikeStyle.None)
                DrawStrike(pGraphics, cellBounds,va_disabledAppearance);

            // �tape 6 : Si la cellule est s�lectionn�e, alors on doit dessiner la s�lection
            if (cell.Selected)
                DrawSelection(pGraphics, pRow, pColumn);

            // �tape 7 : Si nous sommes en mode d�sign alors on doit dessiner l'adresse de la cellule
            if (DesignMode)
                DrawAddress(pGraphics, pRow, pColumn);
        }
        #endregion
 
        #region RandomValue
        /// <summary>
        /// G�n�re un nombre al�atoire entier, choisi entre les valeurs des propri�t�s 'Minimum' et 'Maximum' 
        /// inclusivement.
        /// </summary>
        /// <returns>Un nombre al�atoire entier entre 'Minimum' et 'Maximum' inclusivement</returns>
        public int RandomValue()
        {
            return va_objRandom.Next(va_minimum, va_maximum + 1);
        }
        /// <summary>
        /// G�n�re un nombre entier al�atoire.
        /// </summary>
        /// <param name="pMin">Borne inf�rieure inclue dans l'intervalle</param>
        /// <param name="pMax">Borne sup�rieure inclue dans l'intervalle</param>
        /// <returns>Un nombre al�atoire entier entre pMin et pMax</returns>
        public int RandomValue(int pMin, int pMax)
        {
            return va_objRandom.Next(pMin, pMax+1);
        }
        #endregion

        #region Indexeur
        //===========================================================================================================
        /// <summary>
        /// Obtient ou d�finit le digit � l'index sp�cifi� en tenant compte du ModeAdressage.
        /// </summary>
        /// <param name="pIndex">index du digit</param>
        /// <returns></returns>
        public override int this[int pIndex]
        {
            get
            {
                Address adresse = IndexToAddress(pIndex);
                return va_tabValues[adresse.Row, adresse.Column];
            }
            set
            {
                BeforeValueChangedArgs<int> beforeValueChangedArgs = AcceptBeforeValueChanged(value);
                if (!beforeValueChangedArgs.AcceptValueChanged) return;
                value = beforeValueChangedArgs.NewValue;

                if (value < va_minimum || value > va_maximum)
                    throw new VisualArrayException("La valeur '" + value + "' n'est pas valide pour la cellule, elle doit �tre comprise entre 'Minimum' et 'Maximum'");

                Address adresse = IndexToAddress(pIndex);
                va_tabValues[adresse.Row, adresse.Column] = value;
                UpdateCellAndSprites(pIndex);
                //DessinerCellule(this.CreateGraphics(), adresse.Y, adresse.X);
                adresse = AddressFromAddressMode(adresse.Row, adresse.Column);
                SendValueChanged(pIndex, adresse.Row, adresse.Column,adresse);
            }
        }
        /// <summary>
        /// Obtient ou d�finit le nombre � la cellule dont la ligne et la colonne 
        /// sont sp�cifi�es en tenant compte du mode d'adressage.
        /// </summary>
        /// <param name="pRow">rang�e de la cellule � traiter</param>
        /// <param name="pColumn">colonne de la cellule � traiter</param>
        /// <returns>nombre � la coordonn�e sp�cifi�</returns>
        public override int this[int pRow, int pColumn]
        {
            get
            {
                Address adresse = AddressFromAddressMode(pRow, pColumn);
                return va_tabValues[adresse.Row, adresse.Column];
            }
            set
            {
                BeforeValueChangedArgs<int> beforeValueChangedArgs = AcceptBeforeValueChanged(value);
                if (!beforeValueChangedArgs.AcceptValueChanged) return;
                value = beforeValueChangedArgs.NewValue;

                if (value < va_minimum || value > va_maximum)
                    throw new VisualArrayException("La valeur '" + value + "' n'est pas valide pour la cellule, elle doit �tre comprise entre 'Minimum' et 'Maximum'");
                Address adresse = AddressFromAddressMode(pRow, pColumn);
                va_tabValues[adresse.Row, adresse.Column] = value;
                int index = IndexFromAddress(adresse.Row, adresse.Column);
                UpdateCellAndSprites(index);
                //DessinerCellule(this.CreateGraphics(), adresse.Y, adresse.X);
                SendValueChanged(index, pRow, pColumn, new Address(pRow,pColumn));
            }
        }
        ///// <summary>
        ///// Obtient ou d�finit la valeur pour la cellule � une adresse donn�e
        ///// </summary>
        ///// <param name="pAddress">Adressse de la cellule � manipuler</param>
        ///// <returns>valeur contenue dans la cellule</returns>
        //public int this[Address pAddress]
        //{
        //    get { return this[pAddress.Row, pAddress.Column]; }
        //    set { this[pAddress.Row, pAddress.Column] = value; }
        //}
        #endregion

        internal override void SetValue(int pIndex, int pPixelOffset)
        {
            //int plageDeValeurs = va_maximum - va_minimum;
            //Orientation orientation = Orientation.Horizontal;
            //if (plageDeValeurs > 0)
            //{
            //    // on va appliquer les marges sur la taille du graphique
            //    cellContentBounds.X += GraphAppearance.BarMargin.Left;
            //    cellContentBounds.Width -= GraphAppearance.BarMargin.Horizontal;
            //    cellContentBounds.Y += GraphAppearance.BarMargin.Top;
            //    cellContentBounds.Height -= GraphAppearance.BarMargin.Vertical;

            //    decimal valeurOrigine = pPixelOffset - va_minimum;
            //    // On va d�finir l'orientation des barres
            //    if (cellContentBounds.Width > cellContentBounds.Height)
            //    {
            //        orientation = Orientation.Horizontal;
            //        cellContentBounds.Width = (int)(cellContentBounds.Width * valeurOrigine / plageDeValeurs);
            //    }
            //    else
            //    {
            //        orientation = Orientation.Vertical;
            //        int hauteurBarre = (int)(cellContentBounds.Height * valeurOrigine / plageDeValeurs);
            //        cellContentBounds.Y = cellContentBounds.Y + cellContentBounds.Height - hauteurBarre;
            //    }
            if (va_allowGraphClick)
                this[pIndex] = (va_maximum - va_minimum) / 2 + va_minimum;
        }
    }
}


using System.ComponentModel;
using System.Windows.Forms.Design;
using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using DataObject = System.Windows.Forms.DataObject;
using System.Drawing.Imaging;
using VisualArrays.Appearance;
using VisualArrays.CellVisualElement;
using VisualArrays.Collections;
using VisualArrays.DragHelper;
using VisualArrays.Others;
using VisualArrays.Sprites;
using VisualArrays.VisualArrays;

namespace VisualArrays
{
    /// <summary>
    /// Représente une grille contenant des cellules sans contenu particulier.
    /// </summary>
    [ToolboxBitmap(typeof(BaseGrid),"Resources.tbxBaseVisualArray")]
    public partial class BaseGrid : Control, IVisualArray<CellMouseEventArgs>, ISpriteOwner
    {
        #region Tableaux des Cellules
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Tableau à 2 dimensions des cellules, chaque cellule contient 1 ou plusieurs VisualElement dans une liste chaînée
        /// </summary>
        protected Cell[,] va_tabCells = null;
        #endregion

        #region Swap
        /// <summary>
        /// Permute (échange) deux cellules.
        /// </summary>
        /// <param name="pIndex1">Index de la première cellule impliquée dans la permutation</param>
        /// <param name="pIndex2">Index de la deuxième cellule impliquée dans la permutation</param>
        public virtual void Swap(int pIndex1, int pIndex2)
        {
            Address adresse1 = IndexToAddress(pIndex1);
            Address adresse2 = IndexToAddress(pIndex2);
            if (pIndex1 != pIndex2)
            {
                Cell temp = va_tabCells[adresse1.Row, adresse1.Column];
                va_tabCells[adresse1.Row, adresse1.Column] = va_tabCells[adresse2.Row, adresse2.Column];
                va_tabCells[adresse2.Row, adresse2.Column] = temp;
                UpdateCellAndSprites(pIndex1);
                UpdateCellAndSprites(pIndex2);
            }
        }
        /// <summary>
        /// Indique si la cellule peut être sélectionnée
        /// </summary>
        /// <param name="pCell">Cellule à sélectionner</param>
        /// <param name="pRow">Rangée de la cellule</param>
        /// <param name="pColumn">Colonne de la cellule</param>
        /// <returns></returns>
        public virtual bool IsOkToSelect(Cell pCell,int pRow,int pColumn)
        {
            if (pCell == null) pCell = va_tabCells[pRow, pColumn];
            return pCell.Visible && pCell.Enabled;
        }
        #endregion

        #region CONSTANTES et STATIC

        /// <summary>
        /// Utilisé pour les méthodes des grilles produisants des nombres aléatoires.
        /// </summary>
        protected static readonly Random va_objRandom = new();
        /// <summary>
        /// Nombre de colonnes maximum dans la grille.
        /// </summary>
        protected const int NB_COLONNES_MAXIMUM = 128;

        /// <summary>
        /// Nombre de rangées maximum dans la grille.
        /// </summary>
        protected const int NB_RANGÉES_MAXIMUM = 128;

        /// <summary>
        /// Nombre de colonnes minimum dans la grille.
        /// </summary>
        protected const int NB_COLONNES_MINIMUM = 1;

        /// <summary>
        /// Nombre de rangées minimum dans la grille.
        /// </summary>
        protected const int NB_RANGÉES_MINIMUM = 1;
        /// <summary>
        /// Délai par défaut.
        /// </summary>
        protected const int DELAI_DEFAUT = 250;
        /// <summary>
        /// Délai maximum autorisé.
        /// </summary>
        protected const int DELAI_MAX = 5000;
        /// <summary>
        /// Délai minimum autorisé.
        /// </summary>
        protected const int DELAI_MIN = 0;
        /// <summary>
        /// Délai acceptable entre les touches.
        /// </summary>
        protected const int DELAI_INTER_TOUCHES = 40000000;
        #endregion

        #region Tableaux des entêtes personalisées
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Tableau à 1 dimension des en-têtes de colonnes.
        /// </summary>
        //private string[] va_columns = null;
        private HeaderArray va_columnHeaders;
        /// <summary>
        /// Tableau à 1 dimension des en-têtes de colonnes.
        /// </summary>
        [Browsable(false)]
        public HeaderArray ColumnHeaderArray => va_columnHeaders;

        //-----------------------------------------------------------------------------
        /// <summary>
        /// Tableau à 1 dimension des en-têtes des rangées.
        /// </summary>
        //protected string[] va_rows = null;
        private HeaderArray va_rowHeader;
        /// <summary>
        /// Tableau à 1 dimension des en-têtes des rangées.
        /// </summary>
        [Browsable(false)]
        public HeaderArray RowHeaderArray => va_rowHeader;

        //============================================================================================
        /// <summary>
        /// Obtient ou définit la hauteur réele de l'en-tête des colonnes.
        /// </summary>
        protected internal int va_enteteColHaut = 0;
        /// <summary>
        /// Obtient ou définit la largeur réele de l'en-tête des rangées.
        /// </summary>
        protected internal int va_enteteLgnLarg = 0;

        #endregion

        #region OffScreen Stuff
        //============================================================================================
        // 4 variables permettant de dessiner tous les éléments de la grille et de ses cellules
        // à l'extérieur de l'écran.
        private Bitmap va_gridOffScreenBitMap = null;
        private IntPtr va_gridHMemBmp;  // Pointeur vers la zone mémoire du bitmap
        private Graphics va_gridOffScreenGraphic = null;
        private IntPtr va_gridHMemdc;   // Pointeur vers la zone mémoire du dc
        //============================================================================================
        // 4 variables permettant de dessiner ce qui concerne les Sprites par dessus la grille
        // mais toujours à l'extérieur de l'écran.
        private Bitmap va_spriteOffScreenBitMap = null;
        private IntPtr va_spriteHMemBmp;  // Pointeur vers la zone mémoire du bitmap
        private Graphics va_spriteOffScreenGraphic = null;
        private IntPtr va_spriteHMemdc;   // Pointeur vers la zone mémoire du dc
        #endregion

        #region CHAMPS et PROPRIÉTÉS
        /// <summary>
        /// Fournit l'état du contrôle
        /// </summary>
        internal bool InDesignMode => DesignMode;

        //============================================================================================
        /// <summary>
        /// Obtient et définit la bordure autour de chaque cellule.
        /// </summary>
        protected int m_cellMargin = 2;
        /// <summary>
        /// Obtient et définit la bordure autour de chaque cellule.
        /// </summary>
        [DefaultValue(2), Browsable(true), Category("CellAppearance"), Description("Espacement autour de chaque cellule")]
        public int CellMargin
        {
            get => m_cellMargin;
            set
            {

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "Margin",
                        value,
                        "doit être >= 0");
                }

                if (m_cellMargin != value)
                {
                    m_cellMargin = value;
                    enuResizeMode backup = ResizeMode;
                    ResizeMode = enuResizeMode.Normal;
                    ReCalculerTaille();
                    ResizeMode = backup;
                    Refresh();
                }
            }
        }


        //============================================================================================
        /// <summary>
        /// Obtient et définit l'alignement du contenu principal pour toutes les cellules.
        /// </summary>
        protected ContentAlignment m_cellContentAlign = ContentAlignment.MiddleCenter;
        /// <summary>
        /// Obtient et définit l'alignement du contenu principal pour toutes les cellules.
        /// </summary>
        [DefaultValue(ContentAlignment.MiddleCenter),Browsable(true),Category("CellAppearance"), Description("Alignement du contenu principal pour toutes les cellules")]
        public ContentAlignment CellContentAlign
        {
            get => m_cellContentAlign;
            set
            {
                if (value != m_cellContentAlign)
                {
                    m_cellContentAlign = value;
                    UpdateCellsBkgVisualElement();
                }
            }
        }
        //============================================================================================
        private static readonly Size defaultCellSize = new(50, 50);
        private Size m_cellSize = defaultCellSize;
        /// <summary>
        /// Obtient et définit la largeur et la hauteur des cellules en pixels .
        /// </summary>
        [Browsable(true), Category("CellAppearance"), Description("Largeur et hauteur des cellules en pixels")]
        public Size CellSize
        {
            get => m_cellSize;
            set
            {
                if (value != m_cellSize)
                {
                    m_cellSize = value;
                    ReCalculerTaille();
                    Refresh();
                }
            }
        }
        private void ResetCellSize()
        {
            m_cellSize = defaultCellSize;
            ReCalculerTaille();
            Refresh();
        }
        private bool ShouldSerializeCellSize()
        {
            return m_cellSize != defaultCellSize;
        }


        //----------------------------------------------------------------------------------------
        /// <summary>
        /// Obtient et définit le délai d'attente en millisecondes.
        /// </summary>
        private int va_delay;
        //----------------------------------------------------------------------------------------
        /// <summary>
        /// Obtient et définit le délai d'attente en millisecondes.
        /// </summary>
        [DefaultValue(DELAI_DEFAUT), Browsable(true), Category("VisualArrays"), Description("Délai en millisecondes utilisé par la méthode Attendre.")]
        public int Delay
        {
            get => va_delay;
            set
            {
                if (va_delay == value) return;
                va_delay = value;
                if (va_delay < DELAI_MIN) va_delay = DELAI_MIN;
                else if (va_delay > DELAI_MAX)
                    va_delay = DELAI_MAX;
            }
        }
        //============================================================================================
        private bool va_allowDrag;
        /// <summary>
        /// Indique si le contrôle permet d'utiliser l'opération glisser.
        /// </summary>
        [DefaultValue(false), Category("VisualArrays"), Browsable(true), Description("Indique si le contrôle permet d'utiliser l'opération glisser.")]
        public bool AllowDrag
        {
            get => va_allowDrag;
            set => va_allowDrag = value;
        }
        //============================================================================================
        private bool va_allowSelfDrop;
        /// <summary>
        /// Indique si le contrôle permet d'utiliser l'opération glisser-déposer sur lui-même.
        /// </summary>
        [DefaultValue(false), Category("VisualArrays"), Browsable(true), Description("Indique si le contrôle permet d'utiliser l'opération glisser-déposer sur lui-même.")]
        public bool AllowSelfDrop
        {
            get => va_allowSelfDrop;
            set {
                if (value)
                    AllowDrop = true;
                va_allowSelfDrop = value; }
        }
        //============================================================================================
        private bool va_allowCellDrag;
        /// <summary>
        /// Indique si le contrôle permet d'utiliser l'opération glisser-déposer sur lui-même.
        /// </summary>
        [DefaultValue(true), Category("VisualArrays"), Browsable(true), Description("Indique si le contrôle permet d'utiliser l'opération glisser-déposer du contenu d'une cellule.")]
        public bool AllowCellDrag
        {
            get => va_allowCellDrag;
            set => va_allowCellDrag = value;
        }
        //============================================================================================
        private bool va_testMode = false;
        /// <summary>
        /// Indique si le contrôle est en mode test.
        /// </summary>
        [DefaultValue(false),Browsable(false)]
        public bool TestMode
        {
            get => va_testMode;
            set => va_testMode = value;
        }
        //============================================================================================
        /// <summary>
        /// Force la grille à se redessiner complètement.
        /// </summary>
        public override void Refresh()
        {
            //base.Refresh();
            //if (m_load)
            if (this.Parent != null)
                OnPaint(new PaintEventArgs(CreateGraphics(), ClientRectangle));
            //CreateGraphics().DrawImage(va_offScreenBitMap, ClientRectangle);
        }
        //============================================================================================
        /// <summary>
        /// Obtient ou définit si la touche enter provoque un déplacement automatique du SelectedIndex.
        /// </summary>
        private bool va_enterKeyNextIndex;
        /// <summary>
        /// Obtient ou définit si la touche enter provoque un déplacement automatique du SelectedIndex.
        /// </summary>
        [DefaultValue(false), Category("VisualArrays"), Browsable(true), Description("Indique si la touche Enter provoque un déplacement automatique du SelectedIndex.")]
        public bool EnterKeyNextIndex
        {
            get => va_enterKeyNextIndex;
            set => va_enterKeyNextIndex = value;
        }
        //============================================================================================
        /// <summary>
        /// Permet de déterminer le délai entre deux touches du clavier.
        /// </summary>
        protected DateTime va_currentKeyTime;

        //============================================================================================
        //private int va_baseAddress;
        ///// <summary>
        ///// Obtient et définit l'adresse de base des cellules.
        ///// </summary>
        //internal int BaseAddress
        //{
        //    get { return va_baseAddress; }
        //    set
        //    {
        //        va_baseAddress = value;
        //        this.Refresh();
        //    }
        //}
        //===========================================================================================
        private SpriteCollection va_sprites;
        /// <summary>
        /// Obtient la collection des objets Sprite qui s'affiche sur la grille.
        /// </summary>
        [Description("Représente la collection des Sprites appartenant à cette grille")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("VisualArrays")]
        [EditorAttribute(typeof(SpriteCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public SpriteCollection Sprites => va_sprites;

        //============================================================================================
        /// <summary>
        /// Obtient ou définit si les cellules de la grille sont en lecture seule.
        /// </summary>
        protected bool va_readOnly = false;
        /// <summary>
        /// Obtient ou définit si les cellules de la grille sont en lecture seule.
        /// </summary>
        [DefaultValue(false), Category("VisualArrays"), Browsable(true), Description("Indique si les cellules de la grille sont en lecture seule.")]
        public bool ReadOnly
        {
            get => va_readOnly;
            set => va_readOnly = value;
        }
        #endregion

        #region PERFORMANCE
        /// <summary>
        /// Indique que plusieurs modifications sont sur le point de se produire dans le contenu 
        /// de la grille.
        /// </summary>
        protected bool va_isUpdating;
        ///// <summary>
        ///// Indique si le contrôle est en cours de mise à jour.
        ///// </summary>
        //public bool IsUpdating
        //{
        //    get { return va_isUpdating; }
        //}
        /// <summary>
        /// Maintient les performances quand des cellules sont modifiées une par une en empêchant 
        /// le contrôle de dessiner tant que la méthode EndUpdate n'est pas appelée.
        /// </summary>
        public void BeginUpdate()
        {
            va_isUpdating = true;
        }
        /// <summary>
        /// Reprend la peinture de la grille après qu'elle a été interrompue par la méthode BeginUpdate.
        /// </summary>
        public virtual void EndUpdate()
        {
            va_isUpdating = false;
            Refresh();
        }
        #endregion

        #region SPRITES
        /// <summary>
        /// Indique si un Sprite sera en collision avec un autre Sprite s'il est déplacé à l'adresse pFuturAddress
        /// </summary>
        /// <param name="pSprite">Sprite succeptible d'être déplacé</param>
        /// <param name="pFuturAddress">Adresse ou l'on souhaite le déplacer</param>
        /// <returns></returns>
        public bool IsSpriteCollisionAt(Sprite pSprite, Address pFuturAddress)
        {
            //if (pFuturAddress.Column != 1 || pFuturAddress.Row != 1) return false;

            foreach (Sprite otherSprite in va_sprites)
            {
                if (otherSprite != pSprite && otherSprite.DisplayIndex != -1)
                {
                    foreach (Address adrOther in otherSprite.m_tabCells)
                    {
                        Address displayAddressOther = otherSprite.DisplayAddress;
                        displayAddressOther.Column += adrOther.Column;
                        displayAddressOther.Row += adrOther.Row;
                        foreach (Address adrSelf in pSprite.m_tabCells)
                        {
                            Address displayAddressSelf = pFuturAddress;
                            displayAddressSelf.Column += adrSelf.Column;
                            displayAddressSelf.Row += adrSelf.Row;
                            if (displayAddressOther == displayAddressSelf) 
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        //============================================================================================
        private Address[] SpriteAdressesFrom(Sprite pSprite)
        {
            throw new NotImplementedException();
        }
        //============================================================================================
        /// <summary>
        /// Indique si toutes les cellules du Sprite sont à l'intérieur de la grille si on le déplace a pFuturAddress
        /// </summary>
        /// <param name="pSprite">Sprite à vérifier</param>
        /// <param name="pFuturAddress">Adresse ou l'on souhaite déplacer le Sprite</param>
        /// <returns></returns>
        public bool IsSpriteInsideAt(Sprite pSprite,Address pFuturAddress)
        {
            if (pSprite.m_tabCells.Count == 0)
            {
                return pFuturAddress.Column >= 0 && pFuturAddress.Row >= 0 && pFuturAddress.Column < ColumnCount && pFuturAddress.Row < RowCount;
            }
            else
            {
                foreach (Address adr in pSprite.m_tabCells)
                {
                    if (pFuturAddress.Row + adr.Row < 0 || pFuturAddress.Row + adr.Row >= RowCount ||
                        pFuturAddress.Column + adr.Column < 0 || pFuturAddress.Column + adr.Column >= ColumnCount)
                        return false;
                }
            }
            return true;
        }
        #endregion

        #region SELECTION
        //============================================================================================
        /// <summary>
        /// Obtient ou définit le mode de sélection des cellules de la grille.
        /// </summary>
        protected SelectionMode va_selectionMode;
        /// <summary>
        /// Obtient ou définit le mode de sélection des cellules de la grille.
        /// </summary>
        [DefaultValue(SelectionMode.None), Category("VisualArrays"), Browsable(true), Description("Mode de sélection des cellules de la grille")]
        public SelectionMode SelectionMode
        {
            get => va_selectionMode;
            set {

                if (value == System.Windows.Forms.SelectionMode.MultiExtended)
                {
                    throw new ArgumentOutOfRangeException(
                        "SelectionMode",
                        value,
                        "doit être None, One ou MultiSimple");
                }
                if (value != va_selectionMode)
                {
                    va_selectionMode = value;
                    Refresh();
                }
            }
        }
        //============================================================================================
        /// <summary>
        /// Obtient ou définit l'adresse de la cellule sélectionnée en tenant compte du mode d'adressage.
        /// </summary>
        /// ------------------------------------------------------------------------------------------
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Address SelectedAddress
        {
            get => new(va_selectedIndex / va_columnCount, va_selectedIndex % va_columnCount);
            set
            {
                if (va_selectionMode == SelectionMode.None)
                    throw new VisualArrayException("Impossible de modifier la valeur de SelectedAddress lorsque SelectionMode est None.");

                if (value == Address.Empty) { SelectedIndex = -1; return; } // façon de désélectionner à l'aide de cette propriété

                if (value.Column < 0 || value.Column >= va_columnCount || value.Row < 0 || value.Row >= va_rowCount)
                    throw new VisualArrayException("La valeur : '" + value + "' n'est pas valide pour le SelectedAddress.");

                if (SelectedAddress == value) return; // pas de changement

                SelectedIndex = value.Row * va_columnCount + value.Column;
            }
        }
        //============================================================================================
        private int va_selectedIndex = -1;
        /// <summary>
        /// Obtient ou définit l'index de la cellule sélectionnée en tenant compte du mode d'adressage.
        /// </summary>
        /// ------------------------------------------------------------------------------------------
        [DefaultValue(-1), Browsable(false)]
        public int SelectedIndex
        {
            get { return va_selectedIndex; }
            set
            {
                if (va_selectedIndex == value) return; // pas de changement

                if (va_selectionMode == SelectionMode.None)
                    throw new VisualArrayException("Impossible de modifier la valeur de SelectedIndex lorsque SelectionMode est None.");
                if (value < -1 || value >= Length)
                    throw new VisualArrayException("La valeur : '" + value + "' n'est pas valide pour le SelectedIndex.");

                int oldSelectedIndex = va_selectedIndex;
                Address adresse;
                Cell cell;
                va_selectedIndex = value;

                #region SelectionMode.One

                if (va_selectionMode == SelectionMode.One) // SÉLECTION UNIQUE
                {

                    if (va_selectedIndex == -1) // sélectionner rien
                    {
                        adresse = AddressFromIndex(oldSelectedIndex);
                        va_tabCells[adresse.Row, adresse.Column].Selected = false;
                        UpdateCellAndSprites(oldSelectedIndex);
                        va_dragIndex = -1;
                        va_dragIndexSource = -1;
                    }
                    else if (oldSelectedIndex == -1) // il n'y avait aucune sélection
                    {
                        adresse = AddressFromIndex(va_selectedIndex);
                        cell = va_tabCells[adresse.Row, adresse.Column];
                        if (IsOkToSelect(cell,adresse.Row, adresse.Column))
                        {
                            cell.Selected = true;
                            UpdateCellAndSprites(va_selectedIndex);
                        }
                        else
                            throw new VisualArrayException("Imposible de sélectionner une cellule qui n'est pas visible et active");
                    }
                    else // on passe d'une sélection à une autre
                    {
                        adresse = AddressFromIndex(oldSelectedIndex);
                        va_tabCells[adresse.Row, adresse.Column].Selected = false;
                        UpdateCellAndSprites(oldSelectedIndex);
                        adresse = AddressFromIndex(va_selectedIndex);
                        cell = va_tabCells[adresse.Row, adresse.Column];
                        if (IsOkToSelect(cell, adresse.Row, adresse.Column))
                        {
                            va_tabCells[adresse.Row, adresse.Column].Selected = true;
                            UpdateCellAndSprites(va_selectedIndex);
                        }
                        else
                            throw new VisualArrayException("Imposible de sélectionner une cellule qui n'est pas visible et active");
                    }

                    SelectedIndexChanged?.Invoke(this, new EventArgs());
                    SelectionChanged?.Invoke(this, new EventArgs());
                }
                #endregion

                #region SelectionMode.MultiSimple
                else if (va_selectionMode == SelectionMode.MultiSimple) // SÉLECTION MULTIPLE SIMPLE
                {
                    if (va_selectedIndex == -1) // il faut désélectionner toutes les cellules
                    {
                        for (int row = 0; row < va_rowCount; row++)
                            for (int col = 0; col < va_columnCount; col++)
                            {
                                cell = va_tabCells[row, col];
                                if (cell.Selected)
                                {
                                    cell.Selected = false;
                                    UpdateCellAndSprites(row, col);
                                }
                            }

                        SelectedIndexChanged?.Invoke(this, new EventArgs());
                        SelectionChanged?.Invoke(this, new EventArgs());
                    }
                    else // il faut sélectionner la cellule
                    {
                        adresse = AddressFromIndex(va_selectedIndex);
                        cell = va_tabCells[adresse.Row, adresse.Column];
                        if (!IsOkToSelect(cell, adresse.Row, adresse.Column))
                            throw new VisualArrayException("Imposible de sélectionner une cellule qui n'est pas visible et active");

                        if (cell.Selected) return; // cette cellule est déjà sélectionnée, donc rien à faire.

                        cell.Selected = true; // on va sélectionner la cellule
                        UpdateCellAndSprites(va_selectedIndex);

                        if (va_selectedIndex > oldSelectedIndex && oldSelectedIndex != -1)
                            va_selectedIndex = oldSelectedIndex; // conserve l'ancien SelectedIndex
                        else
                        {
                            SelectedIndexChanged?.Invoke(this, new EventArgs());
                        }

                        SelectionChanged?.Invoke(this, new EventArgs());
                    }
                }
                #endregion

                va_currentKeyTime = new DateTime(0);
            }
        }
        //============================================================================================
        /// <summary>
        /// Obtient un tableau des adresses de toutes les cellules sélectionnées
        /// </summary>
        [Browsable(false)]
        public List<Address> SelectedAddresses
        {
            get
            {
                List<Address> liste = new List<Address>(Length);
                for (int row = 0; row < va_rowCount; row++)
                    for (int column = 0; column < va_columnCount; column++)
                        if (va_tabCells[row, column].Selected)
                            liste.Add(AddressFromAddressMode(row,column));
                return liste;
            }
        }
        //============================================================================================
        /// <summary>
        /// Obtient un tableau des indices de toutes les cellules sélectionnées
        /// </summary>
        [Browsable(false)]
        public List<int> SelectedIndices
        {
            get
            {
                List<int> liste = new List<int>(Length);
                for (int row = 0; row < va_rowCount; row++)
                    for (int column = 0; column < va_columnCount; column++)
                        if (va_tabCells[row, column].Selected)
                            liste.Add(IndexFromAddress(row,column));
                return liste;
            }
        }
        //============================================================================================
        /// <summary>
        /// Événement qui se produit lorsque la valeur de la propriété SelectedIndex est modifiée.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque la valeur de la propriété SelectedIndex est modifiée.")]
        public event EventHandler SelectedIndexChanged;

        //============================================================================================
        /// <summary>
        /// Événement qui se produit lorsque la sélection est modifiée.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque la sélection est modifiée.")]
        public event EventHandler SelectionChanged;

        #endregion

        #region ColumnHeaderAppearance

        private ColumnHeaderAppearance va_columnHeaderAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence de l'en-tête des colonnes
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("VisualArrays")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ColumnHeaderAppearance ColumnHeader
        {
            get => va_columnHeaderAppearance;
            set
            {
                if (va_columnHeaderAppearance != value)
                {
                    va_columnHeaderAppearance = value;
                    ReCalculerTaille();
                    Refresh();
                }
            }
        }

        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit la police pour l'en-tête des colonnes.
        ///// </summary>
        //protected internal Font va_columnHeaderFont;
        ///// <summary>
        ///// Obtient ou définit la police pour l'en-tête des colonnes.
        ///// </summary>
        //[Category("Column Header"), Browsable(true), Description("Police du texte pour l'en-tête des colonnes")]
        //public Font ColumnHeaderFont
        //{
        //    get { return va_columnHeaderFont; }
        //    set
        //    {
        //        va_columnHeaderFont = value;
        //        this.Refresh();
        //    }
        //}
        //private void ResetColumnHeaderFont()
        //{
        //    va_columnHeaderFont = new Font("Arial", 10);
        //}
        //private bool ShouldSerializeColumnHeaderFont()
        //{
        //    return !va_columnHeaderFont.Equals(new Font("Arial", 10));
        //}

        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit la couleur du texte pour les en-têtes de colonnes.
        ///// </summary>
        //protected internal Color va_columnHeaderTextColor;
        ///// <summary>
        ///// Obtient ou définit la couleur du texte pour les en-têtes de colonnes.
        ///// </summary>
        //[DefaultValue(typeof(Color), "White"), Category("Column Header"), Browsable(true), Description("Couleur du texte des en-têtes de colonnes.")]
        //public Color ColumnHeaderTextColor
        //{
        //    get { return va_columnHeaderTextColor; }
        //    set
        //    {
        //        va_columnHeaderTextColor = value;
        //        this.Refresh();
        //    }
        //}
        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit la couleur de fond des cellules d'en-tête de colonnes.
        ///// </summary>
        //protected internal Color va_columnHeaderBkgColor;
        ///// <summary>
        ///// Obtient ou définit la couleur de fond des cellules d'en-tête de colonnes.
        ///// </summary>
        //[DefaultValue(typeof(Color), "DarkGreen"), Category("Column Header"), Browsable(true), Description("Couleur de fond des cellules d'en-tête de colonnes.")]
        //public Color ColumnHeaderBkgColor
        //{
        //    get { return va_columnHeaderBkgColor; }
        //    set
        //    {
        //        va_columnHeaderBkgColor = value;
        //        this.Refresh();
        //    }
        //}
        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit le style de fond des cellules d'en-tête de colonnes.
        ///// </summary>
        //protected internal enuHeaderBkgStyle va_columnHeaderBkgStyle;
        ///// <summary>
        ///// Obtient ou définit le style de fond des cellules d'en-tête de colonnes.
        ///// </summary>
        //[DefaultValue(enuHeaderBkgStyle.Fill), Category("Column Header"), Browsable(true), Description("Style de fond des cellules d'en-tête de colonnes.")]
        //public enuHeaderBkgStyle ColumnHeaderBkgStyle
        //{
        //    get { return va_columnHeaderBkgStyle; }
        //    set
        //    {
        //        va_columnHeaderBkgStyle = value;
        //        this.Refresh();
        //    }
        //}
        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit la hauteur de l'en-tête des colonnes.
        ///// </summary>
        //protected internal int va_columnHeaderHeight;
        ///// <summary>
        ///// Obtient ou définit la hauteur de l'en-tête des colonnes.
        ///// </summary>
        //[DefaultValue(20), Category("Column Header"), Browsable(true), Description("Hauteur de l'en-tête des colonnes")]
        //public int ColumnHeaderHeight
        //{
        //    get { return va_columnHeaderHeight; }
        //    set
        //    {
        //        va_columnHeaderHeight = value;
        //        if (va_columnHeaderDataStyle != enuDataStyle.None)
        //            va_enteteColHaut = va_columnHeaderHeight;
        //        ReCalculerTaille();
        //        this.Refresh();
        //    }
        //}
        ////============================================================================================
        ///// <summary>
        ///// Obtient et définit le style du contenu d'en-tête pour les colonnes.
        ///// </summary>
        //protected internal enuDataStyle va_columnHeaderDataStyle;
        ///// <summary>
        ///// Obtient et définit le style du contenu d'en-tête pour les colonnes.
        ///// </summary>
        //[DefaultValue(enuDataStyle.None), Category("Column Header"), Browsable(true), Description("Style du contenu d'en-tête pour les colonnes")]
        //public enuDataStyle ColumnHeaderDataStyle
        //{
        //    get { return va_columnHeaderDataStyle; }
        //    set
        //    {
        //        if (value != va_columnHeaderDataStyle)
        //        {
        //            va_columnHeaderDataStyle = value;
        //            if (va_columnHeaderDataStyle == enuDataStyle.None)
        //                va_enteteColHaut = 0;
        //            else
        //                va_enteteColHaut = va_columnHeaderHeight;
        //            ReCalculerTaille();
        //            this.Refresh();
        //        }
        //    }
        //}
        #endregion

        #region RowHeaderAppearance

        private RowHeaderAppearance va_rowHeaderAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence de la sélection
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("VisualArrays")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RowHeaderAppearance RowHeader
        {
            get => va_rowHeaderAppearance;
            set
            {
                if (value != va_rowHeaderAppearance)
                {
                    va_rowHeaderAppearance = value;
                    ReCalculerTaille();
                    Refresh();
                }
            }
        }

        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit la police pour l'en-tête des rangées.
        ///// </summary>
        //protected internal Font va_rowHeaderFont;
        ///// <summary>
        ///// Obtient ou définit la police pour l'en-tête des rangées.
        ///// </summary>
        //[Category("Row Header"), Browsable(true), Description("Police du texte pour l'en-tête des rangées.")]
        //public Font RowHeaderFont
        //{
        //    get { return va_rowHeaderFont; }
        //    set
        //    {
        //        va_rowHeaderFont = value;
        //        this.Refresh();
        //    }
        //}
        //private void ResetRowHeaderFont()
        //{
        //    va_rowHeaderFont = new Font("Arial", 10);
        //}
        //private bool ShouldSerializeRowHeaderFont()
        //{
        //    return !va_rowHeaderFont.Equals(new Font("Arial", 10));
        //}
        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit la couleur du texte pour l'en-tête des rangées.
        ///// </summary>
        //protected internal Color va_rowHeaderTextColor;
        ///// <summary>
        ///// Obtient ou définit la couleur du texte pour l'en-tête des rangées.
        ///// </summary>
        //[DefaultValue(typeof(Color), "White"), Category("Row Header"), Browsable(true), Description("Couleur du texte pour l'en-tête des rangées.")]
        //public Color RowHeaderTextColor
        //{
        //    get { return va_rowHeaderTextColor; }
        //    set
        //    {
        //        va_rowHeaderTextColor = value;
        //        this.Refresh();
        //    }
        //}
        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit la couleur de fond des cellules d'en-tête de rangées.
        ///// </summary>
        //protected internal Color va_rowHeaderBkgColor;
        ///// <summary>
        ///// Obtient ou définit la couleur de fond des cellules d'en-tête de rangées.
        ///// </summary>
        //[DefaultValue(typeof(Color), "DarkGreen"), Category("Row Header"), Browsable(true), Description("Couleur de fond des cellules d'en-tête de rangées.")]
        //public Color RowHeaderBkgColor
        //{
        //    get { return va_rowHeaderBkgColor; }
        //    set
        //    {
        //        va_rowHeaderBkgColor = value;
        //        this.Refresh();
        //    }
        //}

        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit le style de fond des cellules d'en-tête de rangées.
        ///// </summary>
        //protected internal enuHeaderBkgStyle va_rowHeaderBkgStyle;
        ///// <summary>
        ///// Obtient ou définit le style de fond des cellules d'en-tête de rangées.
        ///// </summary>
        //[DefaultValue(enuHeaderBkgStyle.Fill), Category("Row Header"), Browsable(true), Description("Style de fond des cellules d'en-tête de rangées.")]
        //public enuHeaderBkgStyle RowHeaderBkgStyle
        //{
        //    get { return va_rowHeaderBkgStyle; }
        //    set
        //    {
        //        va_rowHeaderBkgStyle = value;
        //        this.Refresh();
        //    }
        //}
        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit la largeur de l'en-tête des rangées.
        ///// </summary>
        //protected internal int va_rowHeaderWidth;
        ///// <summary>
        ///// Obtient ou définit la largeur de l'en-tête des rangées.
        ///// </summary>
        //[DefaultValue(20), Category("Row Header"), Browsable(true), Description("Largeur de l'en-tête des rangées.")]
        //public int RowHeaderWidth
        //{
        //    get { return va_rowHeaderWidth; }
        //    set
        //    {
        //        va_rowHeaderWidth = value;
        //        if (va_rowHeaderDataStyle != enuDataStyle.None)
        //            va_enteteLgnLarg = va_rowHeaderWidth;
        //        ReCalculerTaille();
        //        this.Refresh();
        //    }
        //}

        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit le style du contenu de l'en-tête des rangées.
        ///// </summary>
        //protected internal enuDataStyle va_rowHeaderDataStyle;
        ///// <summary>
        ///// Obtient ou définit le style du contenu de l'en-tête des rangées.
        ///// </summary>
        //[DefaultValue(enuDataStyle.None), Category("Row Header"), Browsable(true), Description("Style du contenu de l'en-tête des rangées")]
        //public enuDataStyle RowHeaderDataStyle
        //{
        //    get { return va_rowHeaderDataStyle; }
        //    set
        //    {
        //        if (value != va_rowHeaderDataStyle)
        //        {
        //            va_rowHeaderDataStyle = value;
        //            if (va_rowHeaderDataStyle == enuDataStyle.None)
        //                va_enteteLgnLarg = 0;
        //            else
        //                va_enteteLgnLarg = va_rowHeaderWidth;
        //            ReCalculerTaille();
        //            this.Refresh();
        //        }
        //    }
        //}
        #endregion

        #region ADRESSAGE
        //============================================================================================
        /// <summary>
        /// Obtient ou définit le mode d'adressage de la grille.
        /// </summary>
        private enuAddressMode va_addressMode = enuAddressMode.Normal;
        /// <summary>
        /// Obtient ou définit le mode d'adressage de la grille.
        /// </summary>
        [DefaultValue(enuAddressMode.Normal), Category("VisualArrays"), Browsable(true), Description("Mode d'adressage des cellules")]
        public enuAddressMode AddressMode
        {
            get => va_addressMode;
            set
            {
                va_addressMode = value;
                RefreshSpritesBounds();
                this.Refresh();
            }
        }
        //============================================================================================
        private void RefreshSpritesBounds()
        {
            foreach (Sprite sprite in va_sprites)
                sprite.m_mustCalcBounds = true;
        }
        //============================================================================================
        /// <summary>
        /// Obtient ou définit le mode de visualisation des adresses en mode conception.
        /// </summary>
        protected enuAddressView va_addressView = enuAddressView.None;
        /// <summary>
        /// Obtient ou définit le mode de visualisation des adresses en mode conception.
        /// </summary>
        [DefaultValue(enuAddressView.None), Category("VisualArrays"), Browsable(true), Description("Détermine comment s'affichent les adresses en mode conception")]
        public enuAddressView AddressView
        {
            get => va_addressView;
            set
            {
                va_addressView = value;
                this.Refresh();
            }
        }
        #endregion

        #region EnabledAppearance et DisabledAppearance
        /// <summary>
        /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est true
        /// </summary>
        protected CellAppearance va_enabledAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est true
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("CellAppearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CellAppearance EnabledAppearance
        {
            get => va_enabledAppearance;
            set
            {
                va_enabledAppearance = value;
                UpdateCellsBkgVisualElement();
            }
        }

        /// <summary>
        /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est false
        /// </summary>
        protected DisabledAppearance va_disabledAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est false
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("CellAppearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DisabledAppearance DisabledAppearance
        {
            get => va_disabledAppearance;
            set
            {
                va_disabledAppearance = value;
                UpdateDisableVisualElement(value.Style);
                this.Refresh();
            }
        }
        #endregion

        #region SelectionAppearance

        private SelectionAppearance va_selectionAppearance = new();
        /// <summary>
        /// Détermine différents aspects de l'apparence de la sélection
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("CellAppearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SelectionAppearance SelectionAppearance
        {
            get => va_selectionAppearance;
            set
            {
                if (value != va_selectionAppearance)
                {
                    va_selectionAppearance = value;
                    UpdateSelectionVisualElement();
                }
            }
        }

        #endregion

        #region DragAppearance

        private DragAppearance va_dragAppearance = new();
        /// <summary>
        /// Détermine différents aspects de l'apparence de la destination dans une opération glisser/déposer
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("CellAppearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DragAppearance DragAppearance
        {
            get => va_dragAppearance;
            set
            {
                if (value != va_dragAppearance)
                {
                    va_dragAppearance = value;
                }
            }
        }

        #endregion

        #region Update VisualElement : Enabled, Disabled, Selection
        //----------------------------------------------------------------------------------------
        /// <summary>
        /// Élément visuel affiché dans le fond des cellules dont l'état Enabled est false.
        /// </summary>
        protected CellVisualElement.CellVisualElement va_disabledVisualElement;

        //----------------------------------------------------------------------------------------
        /// <summary>
        /// Élément visuel affiché lorsqu'une cellule est sélectionnée.
        /// </summary>
        protected CellVisualElement.CellVisualElement va_selectionVisualElement;

        //============================================================================================
        internal virtual void UpdateDisableVisualElement(enuBkgStyle pStyle)
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
        //==========================================================================================================================================
        internal CellVisualElement.CellVisualElement UpdateBackgroundVisualElement(IBackgroundAppearance pBackgroundAppearance)
        {
            switch (pBackgroundAppearance.Style)
            {
                case enuBkgStyle.None:
                    return null;
                case enuBkgStyle.Border:
                    return new BorderElement(pBackgroundAppearance.Border, pBackgroundAppearance.BackgroundColor);
                case enuBkgStyle.FillShape:
                    return new FillShapeElement(pBackgroundAppearance.Shape, pBackgroundAppearance.BackgroundColor,pBackgroundAppearance.Radius);
                case enuBkgStyle.Shape:
                    return new ShapeElement(pBackgroundAppearance.Shape, pBackgroundAppearance.PenWidth, pBackgroundAppearance.BackgroundColor,pBackgroundAppearance.Radius);
                case enuBkgStyle.Image:
                    return new ImageElement(pBackgroundAppearance.Image);
                default :
                    return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal virtual void UpdateCellsBkgVisualElement()
        {
            UpdateCellsBkgVisualElement(va_enabledAppearance);
        }
        //============================================================================================
        /// <summary>
        /// Change le fond de toutes les cellules d'une VisualBoolArray
        /// </summary>
        internal void UpdateCellsBkgVisualElement(IBackgroundAppearance pBackgroundAppearance)
        {
            if (pBackgroundAppearance == null) return;
            switch (pBackgroundAppearance.Style)
            {
                case enuBkgStyle.None:
                    for (int row = 0; row < va_tabCells.GetLength(0); row++)
                        for (int column = 0; column < va_tabCells.GetLength(1); column++)
                            va_tabCells[row, column].Background = null;
                    break;
                case enuBkgStyle.Border:
                    for (int row = 0; row < va_tabCells.GetLength(0); row++)
                        for (int column = 0; column < va_tabCells.GetLength(1); column++)
                            va_tabCells[row, column].Background = new BorderElement(pBackgroundAppearance.Border, pBackgroundAppearance.BackgroundColor);
                    break;
                case enuBkgStyle.FillShape:
                    for (int row = 0; row < va_tabCells.GetLength(0); row++)
                        for (int column = 0; column < va_tabCells.GetLength(1); column++)
                            va_tabCells[row, column].Background = new FillShapeElement(pBackgroundAppearance.Shape, pBackgroundAppearance.BackgroundColor,pBackgroundAppearance.Radius);
                    break;
                case enuBkgStyle.Shape:
                    for (int row = 0; row < va_tabCells.GetLength(0); row++)
                        for (int column = 0; column < va_tabCells.GetLength(1); column++)
                            va_tabCells[row, column].Background = new ShapeElement(pBackgroundAppearance.Shape, pBackgroundAppearance.PenWidth, pBackgroundAppearance.BackgroundColor, pBackgroundAppearance.Radius);
                    break;
                case enuBkgStyle.Image:
                    for (int row = 0; row < va_tabCells.GetLength(0); row++)
                        for (int column = 0; column < va_tabCells.GetLength(1); column++)
                            va_tabCells[row, column].Background = new ImageElement(pBackgroundAppearance.Image);
                    break;
                default:
                    break;
            }
            this.Refresh();
        }
        //============================================================================================
        private void UpdateSelectionVisualElement()
        {
            switch (va_selectionAppearance.Style)
            {
                case enuSelectionStyle.Shape:
                    va_selectionVisualElement = new ShapeElement(va_selectionAppearance.Shape, va_selectionAppearance.PenWidth, va_selectionAppearance.Color, va_selectionAppearance.Radius);
                    break;
                case enuSelectionStyle.Image:
                    va_selectionVisualElement = new ImageElement(va_selectionAppearance.Image);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region GridAppearance

        private GridAppearance va_gridAppearance;
        /// <summary>
        /// Détermine différents aspects de l'apparence de la sélection
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("VisualArrays")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridAppearance GridAppearance
        {
            get => va_gridAppearance;
            set
            {
                if (value != va_gridAppearance)
                {
                    va_gridAppearance = value;
                    ReCalculerTaille();
                    Refresh();
                }
            }
        }

        #endregion

        #region GRILLE
        ///// <summary>
        ///// Obtient ou définit une image à placer uniquement sous les cellules de la grille.
        ///// </summary>
        //protected internal Image va_gridBkgImage;
        ///// <summary>
        ///// Obtient ou définit une image à placer uniquement sous les cellules de la grille.
        ///// </summary>
        //[DefaultValue(null), Category("Grid"), Browsable(true), Description("Image à placer uniquement sous les cellules de la grille")]
        //public Image GridBkgImage
        //{
        //    get { return va_gridBkgImage; }
        //    set
        //    {
        //        va_gridBkgImage = value;
        //        this.Refresh();
        //    }
        //}
        //public override Image BackgroundImage
        //{
        //    get
        //    {
        //        return base.BackgroundImage;
        //    }
        //    set
        //    {
        //        base.BackgroundImage = value;
        //    }
        //}
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }
        //============================================================================================
        /// <summary>
        /// Obtient ou définit le mode de redimentionnement de la grille.
        /// </summary>
        private enuResizeMode va_resizeMode;
        /// <summary>
        /// Obtient ou définit le mode de redimentionnement de la grille.
        /// </summary>
        [DefaultValue(enuResizeMode.Normal), Category("VisualArrays"), Browsable(true), Description("Mode de redimentionnement de la grille")]
        public enuResizeMode ResizeMode
        {
            get => va_resizeMode;
            set => va_resizeMode = value;
        }
        //============================================================================================
        /// <summary>
        /// Obtient et définit le nombre de rangées dans la grille.
        /// </summary>
        private int va_rowCount = 3;
        /// <summary>
        /// Obtient et définit le nombre de rangées dans la grille.
        /// </summary>
        [DefaultValue(3), Category("VisualArrays"), Browsable(true), Description("Nombre de rangées de la grille")]
        public virtual int RowCount
        {
            get => va_rowCount;
            set
            {
                if (value < NB_RANGÉES_MINIMUM)
                    throw new VisualArrayException("Le nombre de rangées doit être supérieur ou égale à 1");
                else if (value > NB_RANGÉES_MAXIMUM)
                    throw new VisualArrayException("Le nombre de rangées ne doit pas dépasser " + NB_RANGÉES_MAXIMUM);

                if (va_rowCount == value) return;

                if (SelectedAddress.Row >= value) 
                    SelectedIndex = -1; // pour enlever la sélection si la grille est trop petite

                Cell[,] oldTab = va_tabCells;
                va_tabCells = new Cell[value, va_columnCount];
                int rowStop = value > va_rowCount ? va_rowCount : value;

                // on va conserver les cellules actuelles de la grille
                for (int column = 0; column < va_columnCount; column++)
                {
                    for (int row = 0; row < rowStop; row++)
                        va_tabCells[row, column] = oldTab[row, column];
                    for (int row = rowStop; row < value; row++)
                        va_tabCells[row, column] = new Cell();
                }

                va_rowCount = value;
                ResizeValueArray();
                ResizeRowHeader();
                ResetAllValuesToDefault();
                ReCalculerTaille();
                RelocateSprites(); 

                UpdateCellsBkgVisualElement();

                LengthChanged?.Invoke(this, EventArgs.Empty);
                this.Refresh();
            }
        }
        //============================================================================================
        /// <summary>
        /// Obtient ou définit le nombre de colonnes dans la grille.
        /// </summary>
        private int va_columnCount = 3;
        /// <summary>
        /// Obtient ou définit le nombre de colonnes dans la grille.
        /// </summary>
        [DefaultValue(3), Category("VisualArrays"), Browsable(true), Description("Nombre de colonnes de la grille")]
        public virtual int ColumnCount
        {
            get => va_columnCount;
            set
            {
                if (value < NB_COLONNES_MINIMUM)
                    throw new VisualArrayException("Le nombre de colonnes doit être supérieur ou égale à 1");
                else if (value > NB_COLONNES_MAXIMUM)
                    throw new VisualArrayException("Le nombre de colonnes ne doit pas dépasser " + NB_COLONNES_MAXIMUM);
                
                if (va_columnCount == value) return;

                if (SelectedAddress.Column >= value) 
                    SelectedIndex = -1; // pour enlever la sélection si la grille est trop petite

                Cell[,] oldTab = va_tabCells;
                va_tabCells = new Cell[va_rowCount, value];
                int columnStop = value > va_columnCount ? va_columnCount : value;

                // on va conserver les cellules actuelles de la grille
                for (int row = 0; row < va_rowCount; row++)
                {
                    for (int column = 0; column < columnStop; column++)
                        va_tabCells[row, column] = oldTab[row, column];
                    for (int column = columnStop; column < value; column++)
                        va_tabCells[row, column] = new Cell();
                }

                va_columnCount = value;

                ResizeValueArray();
                ResizeColumnHeader();
                ResetAllValuesToDefault();
                ReCalculerTaille();
                RelocateSprites(); 

                UpdateCellsBkgVisualElement();

                LengthChanged?.Invoke(this, EventArgs.Empty);
                this.Refresh();
            }
        }
        ///============================================================================================
        /// <summary>
        /// Replace les Sprites au besoin, si la position du Sprite est à l'extérieur des limites de la grille
        /// </summary>
        private void RelocateSprites()
        {
            foreach (Sprite sprite in Sprites)
            {
                int newRow = sprite.DisplayAddress.Row;
                int startRow = newRow;

                int newCol = sprite.DisplayAddress.Column;
                int startCol = newCol;

                if (newRow >= RowCount)
                    newRow = RowCount - 1;
                if (newCol >= ColumnCount)
                    newCol = ColumnCount - 1;
                if (newRow != startRow || newCol != startCol)
                    sprite.DisplayAddress = new Address(newRow, newCol);
            }
        }
        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit la couleur du quadrillage de la grille.
        ///// </summary>
        //protected internal Color va_gridColor;
        ///// <summary>
        ///// Obtient ou définit la couleur du quadrillage de la grille.
        ///// </summary>
        ///// -------------------------------------------------------------------------------------
        //[DefaultValue(typeof(Color), "Gray"), Category("Grid"), Browsable(true), Description("Couleur du quadrillage de la grille")]
        //public Color GridColor
        //{
        //    get { return va_gridColor; }
        //    set
        //    {
        //        va_gridColor = value;
        //        this.Refresh();
        //    }
        //}
        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit l'épaisseur du quadrillage de la grille.
        ///// </summary>
        //protected internal int va_gridWidth;
        ///// <summary>
        ///// Obtient ou définit l'épaisseur du quadrillage de la grille.
        ///// </summary>
        //[DefaultValue(1), Category("Grid"), Browsable(true), Description("Épaisseur du quadrillage de la grille")]
        //public int GridWidth
        //{
        //    get { return va_gridWidth; }
        //    set
        //    {
        //        if (value < 0)
        //            va_gridWidth = 0;
        //        else if (value > 50)
        //            va_gridWidth = 50;
        //        else
        //            va_gridWidth = value;

        //        ReCalculerTaille();
        //        this.Refresh();
        //    }
        //}
        ////============================================================================================
        ///// <summary>
        ///// Obtient ou définit la taille de l'espacement autour de la grille.
        ///// </summary>
        //protected internal int va_gridPadding;
        ///// <summary>
        ///// Obtient ou définit la taille de l'espacement autour de la grille.
        ///// </summary>
        //[DefaultValue(6), Category("Grid"), Browsable(true), Description("Espacement autour de la grille")]
        //public int GridPadding
        //{
        //    get { return va_gridPadding; }
        //    set
        //    {
        //        if (value < 0)
        //            va_gridPadding = 0;
        //        else if (value > Width / 3)
        //            va_gridPadding = Width / 3;
        //        else
        //            va_gridPadding = value;
        //        ReCalculerTaille();
        //        this.Refresh();
        //    }
        //}
        //============================================================================================
        /// <summary>
        /// Obtient le nombre de cellules dans la grille.
        /// </summary>
        [Browsable(false)]
        public int Length => va_columnCount * va_rowCount;

        #endregion

        #region CONSTRUCTEUR
        //============================================================================================
        /// <summary>
        /// Initialise une nouvelle instance de la grille avec les valeurs par défaut.
        /// </summary>
        public BaseGrid()
        {
            va_mouseCurrentIndex = -1;
            va_mouseCurrentColumn = -1;
            va_mouseCurrentRow = -1;
            va_lastMouseClickCell = -1;
            va_mouseDownCell = -1;
            va_mouseDownColumn = -1;
            va_mouseDownRow = -1;
             //--------------------------------------------------------------
            va_readOnly = false;
            va_isUpdating = false;
            va_delay = DELAI_DEFAUT;
            //--------------------------------------------------------------
            va_allowDrag = false;
            va_allowSelfDrop = false;
            va_allowCellDrag = true;
            //--------------------------------------------------------------
            va_sprites = new SpriteCollection(this);
            //va_baseAddress = 0;
            va_enterKeyNextIndex = false;
            va_currentKeyTime = new DateTime(0);
            //--------------------------------------------------------------
            va_rowHeaderAppearance = new RowHeaderAppearance(this);
            va_columnHeaderAppearance = new ColumnHeaderAppearance(this);
            va_enabledAppearance = new CellAppearance(this);
            va_gridAppearance = new GridAppearance(this);
            //--------------------------------------------------------------
            va_selectionAppearance = new SelectionAppearance();
            va_selectionVisualElement = new ShapeElement(va_selectionAppearance.Shape, va_selectionAppearance.PenWidth, va_selectionAppearance.Color, va_selectionAppearance.Radius);
            va_selectionMode = SelectionMode.None;
            va_dragAppearance = new DragAppearance();
            //--------------------------------------------------------------
            va_disabledAppearance = new DisabledAppearance(this);
            va_disabledVisualElement = new FillShapeElement(va_disabledAppearance.Shape, va_disabledAppearance.BackgroundColor, va_disabledAppearance.Radius);
            //--------------------------------------------------------------
            InitializeComponent();
            ReCalculerTaille();
            CreateCellArray(); //ResizeArrays(); 24/08/2011
            ResizeColumnHeader();
            ResizeRowHeader();
            UpdateCellsBkgVisualElement(); // 03-06-2011
        }
        #endregion

        #region Padding

        internal static readonly Padding m_defaultPadding = new(6);
        //===========================================================================================
        private Padding m_padding = m_defaultPadding;
        /// <summary>
        /// Obtient et définit l'espacement interne entre le contrôle et la zone contenant les cellules.
        /// </summary>
        [Browsable(true),
         NotifyParentProperty(true),
         EditorBrowsable(EditorBrowsableState.Always),
         Description("Espacement interne entre le contrôle et la zone contenant les cellules")]
        public new Padding Padding
        {
            get => m_padding;
            set
            {
                if (value != m_padding)
                {
                    m_padding = value;
                    enuResizeMode backup = ResizeMode;
                    ResizeMode = enuResizeMode.Normal;
                    ReCalculerTaille();
                    ResizeMode = backup;
                    Refresh();
                    base.OnPaddingChanged(EventArgs.Empty);
                }
            }
        }
        private void ResetPadding()
        {
            Padding = m_defaultPadding;
        }
        private bool ShouldSerializePadding()
        {
            return m_padding != m_defaultPadding;
        }
        #endregion

        #region MÉTHODES PUPLIQUES
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Génère une direction aléatoire.
        /// </summary>
        /// <returns>Une direction aléatoire</returns>
        public enuDirection RandomDirection()
        {
            return (enuDirection)va_objRandom.Next(0, 4);
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Génère une direction aléatoire différente de celle fournie en paramètre
        /// </summary>
        /// <param name="pCurrentDirection">Direction courante</param>
        /// <returns>Une direction aléatoire différente</returns>
        public enuDirection RandomDirection(enuDirection pCurrentDirection)
        {
            enuDirection newDirection = (enuDirection)va_objRandom.Next(0, 4);
            int deplacement = va_objRandom.Next(0, 3) + 1;
            if (newDirection == pCurrentDirection)
                newDirection += deplacement;
            if (newDirection > enuDirection.Bottom)
                newDirection = (enuDirection)((int)newDirection % 4);
            return newDirection;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Génère un nombre aléatoire.
        /// </summary>
        /// <returns>Un nombre aléatoire entre 0 et le nombre de cellules de la grille.</returns>
        public int RandomIndex()
        {
            return va_objRandom.Next(0, Length);
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Génère une adresse aléatoire.
        /// </summary>
        /// <returns>Une adresse aléatoire sous la forme rangée et colonne.</returns>
        public Address RandomAddress()
        {
            return new Address(va_objRandom.Next(0, RowCount), va_objRandom.Next(0, ColumnCount));
        }
        //------------------------------------------------------------------------------------------
        ///// <summary>
        ///// Génère un nombre entier aléatoire.
        ///// </summary>
        ///// <param name="pMin">Borne inférieure inclue dans l'intervalle</param>
        ///// <param name="pMax">Borne supérieure exlue de l'intervalle</param>
        ///// <returns>Un nombre aléatoire entre pMin et pMax - 1.</returns>
        //public int RandomInt(int pMin, int pMax)
        //{
        //    return va_objRandom.Next(pMin, pMax);
        //}
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Permet d'attendre un délai spécifié par la propriété Delay.
        /// </summary>
        public void Wait()
        {
            if (va_testMode || va_delay == 0) return;
            DateTime maintenantPlusDelai = DateTime.Now.AddMilliseconds(va_delay);
            while (DateTime.Now < maintenantPlusDelai) ;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Permet d'attendre un délai spécifié par le paramètre pDelai.
        /// </summary>
        /// <param name="pDelai">Le délai à attendre.</param>
        public void Wait(int pDelai)
        {
            if (va_testMode || pDelai <= 0) return;
            DateTime maintenantPlusDelai = DateTime.Now.AddMilliseconds(pDelai);
            while (DateTime.Now < maintenantPlusDelai) ;
        }

        //-----------------------------------------------------------------------------
        /// <summary>
        /// Ré-initialise chaque cellule à sa valeur par défaut.
        /// </summary>
        /// ---------------------------------------------------------------------------
        public void ClearValues()
        {
            ResetAllValuesToDefault();
            if (va_isUpdating) return;
            Refresh();
        }
        /// ---------------------------------------------------------------------------
        /// <summary>
        /// Ré-Initialise chaque cellule à son état initial par défaut.
        /// </summary>
        /// ---------------------------------------------------------------------------
        public void Clear()
        {
            // Cette section va remettre chaque cellule dans son état initial
            for (int row = 0; row < va_rowCount; row++)
                for (int column = 0; column < va_columnCount; column++)
                    va_tabCells[row, column].Reset();
            UpdateCellsBkgVisualElement();
            //==============================================================================
            // ResetAllValuesToDefault();
            //if (va_selectionMode != System.Windows.Forms.SelectionMode.None)
            //  SelectedIndex = -1;
            //==============================================================================
            if (va_selectionMode != System.Windows.Forms.SelectionMode.None)
                SelectedIndex = -1;
            ResetAllValuesToDefault();
            //==============================================================================
            if (va_isUpdating) return;
            Refresh();
        }
        #endregion

        #region MÉTHODES PROTECTED
        /// <summary>
        /// Dessiner l'en-tête d'une colonne.
        /// </summary>
        /// <param name="pColumn">Colonne à redessiner.</param>
        /// <param name="pText">Texte à redessiner.</param>
        internal void DrawColumnHeader(int pColumn, string pText)
        {
            DrawColumnsHeaders(this.CreateGraphics(), pColumn, pText, va_columnHeaderAppearance.ForeColor, va_columnHeaderAppearance.Font);
        }
        /// <summary>
        /// Dessiner l'en-tête d'une rangée.
        /// </summary>
        /// <param name="pRow">Rangée à redessiner.</param>
        /// <param name="pText">Texte à redessiner.</param>
        internal void DrawRowHeader(int pRow, string pText)
        {
            DrawRowsHeaders(this.CreateGraphics(), pRow, pText, va_rowHeaderAppearance.ForeColor, va_rowHeaderAppearance.Font);
        }        /// <summary>
        /// Dessine l'en-tête des colonnes.
        /// </summary>
        /// <param name="pGraphics">Objet graphique où dessiner</param>
        /// <param name="pColumn">Index de la colonne de la cellule.</param>
        /// <param name="pTexte">Texte à dessiner.</param>
        /// <param name="pCouleur">Couleur du texte.</param>
        /// <param name="pPolice">Police du texte.</param>
        protected void DrawColumnsHeaders(Graphics pGraphics, int pColumn, string pTexte, Color pCouleur, Font pPolice)
        {
            Brush objPinceau = new SolidBrush(pCouleur);
            //pGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            SizeF pt = pGraphics.MeasureString(pTexte, pPolice);
            int posY = Padding.Top + (va_enteteColHaut - (int)pt.Height) / 2;
            int posX = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize + m_cellMargin;

            Rectangle backRect = new(posX, Padding.Top, m_cellSize.Width, va_enteteColHaut - 2);
            switch (va_columnHeaderAppearance.Style)
            {
                case enuHeaderBkgStyle.None:
                    //si une image de fond existe, alors on doit la redessiner
                    if (BackgroundImage != null)
                        pGraphics.DrawImage(BackgroundImage, backRect, backRect, GraphicsUnit.Pixel);
                    else // sinon on doit redessiner avec la couleur de fond
                        pGraphics.FillRectangle(new SolidBrush(BackColor), backRect);
                    break;
                case enuHeaderBkgStyle.Fill:
                    pGraphics.FillRectangle(new SolidBrush(va_columnHeaderAppearance.BackgroundColor), backRect);
                    break;
                default:
                    break;
            }
            posX += (m_cellSize.Width - (int)pt.Width) / 2 + 1;
            pGraphics.DrawString(pTexte, pPolice, objPinceau, posX, posY);
        }
        /// <summary>
        /// Dessine l'en-tête des rangées.
        /// </summary>
        /// <param name="pGraphics">Objet graphique où dessiner.</param>
        /// <param name="pRow">Index de la rangée de la cellule.</param>
        /// <param name="pTexte">Texte à dessiner.</param>
        /// <param name="pCouleur">Couleur du texte.</param>
        /// <param name="pPolice">Police du texte.</param>
        protected void DrawRowsHeaders(Graphics pGraphics, int pRow, string pTexte, Color pCouleur, Font pPolice)
        {
            Brush objPinceau = new SolidBrush(pCouleur);

            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            SizeF pt = pGraphics.MeasureString(pTexte, pPolice);
            int posY = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize + m_cellMargin;
            int posX = Padding.Left + (va_enteteLgnLarg - (int)pt.Width) / 2;

            Rectangle backRect = new(Padding.Left, posY, va_enteteLgnLarg - 2, m_cellSize.Height);
            switch (va_rowHeaderAppearance.Style)
            {
                case enuHeaderBkgStyle.None:
                    //si une image de fond existe, alors on doit la redessiner
                    if (BackgroundImage != null)
                        pGraphics.DrawImage(BackgroundImage, backRect, backRect, GraphicsUnit.Pixel);
                    else // dans ce cas il faut redessiner avec la couleur de fond
                        pGraphics.FillRectangle(new SolidBrush(BackColor), backRect);
                    break;
                case enuHeaderBkgStyle.Fill:
                    pGraphics.FillRectangle(new SolidBrush(va_rowHeaderAppearance.BackgroundColor), backRect);
                    break;
                default:
                    break;
            }
            posY += (m_cellSize.Height - (int)pt.Height) / 2;
            pGraphics.DrawString(pTexte, pPolice, objPinceau, posX, posY);
        }
        /// <summary>
        /// Dessine le texte des en-têtes de rangées et de colonnes.
        /// </summary>
        /// <param name="pGraphics"></param>
        protected void DrawAllHeaders(Graphics pGraphics)
        {
            if (va_columnHeaderAppearance.Visible)
            {
                switch (va_columnHeaderAppearance.ValueStyle)
                {
                    case enuDataStyle.Index:
                        for (int colonne = 0; colonne < ColumnCount; colonne++)
                            DrawColumnsHeaders(pGraphics, colonne, (IndexFromAddress(0, colonne) % ColumnCount).ToString(), va_columnHeaderAppearance.ForeColor, va_columnHeaderAppearance.Font);
                        break;
                    case enuDataStyle.IndexBase1:
                        for (int colonne = 0; colonne < ColumnCount; colonne++)
                            DrawColumnsHeaders(pGraphics, colonne, (IndexFromAddress(0, colonne) % ColumnCount + 1).ToString(), va_columnHeaderAppearance.ForeColor, va_columnHeaderAppearance.Font);
                        break;
                    //case enuDataStyle.Letter:
                    //    for (int colonne = 0; colonne < ColumnCount; colonne++)
                    //        DrawColumnsHeaders(pGraphics, colonne, ((char)(IndexFromAddress(0, colonne) % ColumnCount + 'A')).ToString(), va_columnHeaderAppearance.ForeColor, va_columnHeaderAppearance.Font);
                    //    break;
                    //case enuDataStyle.Binary:
                    //    for (int colonne = 0; colonne < ColumnCount; colonne++)
                    //        DrawColumnsHeaders(pGraphics, colonne, (1 << IndexFromAddress(0, colonne) % ColumnCount).ToString(), va_columnHeaderAppearance.ForeColor, va_columnHeaderAppearance.Font);
                    //    break;
                    case enuDataStyle.User:
                        for (int colonne = 0; colonne < ColumnCount; colonne++)
                            if (va_columnHeaders[colonne] != null)
                                DrawColumnsHeaders(pGraphics, colonne, va_columnHeaders[colonne], va_columnHeaderAppearance.ForeColor, va_columnHeaderAppearance.Font);
                            else
                                DrawColumnsHeaders(pGraphics, colonne, String.Empty, va_columnHeaderAppearance.ForeColor, va_columnHeaderAppearance.Font);
                        break;
                    default:
                        break;
                }
            }
            if (va_rowHeaderAppearance.Visible)
            {
                switch (va_rowHeaderAppearance.ValueStyle)
                {
                    case enuDataStyle.Index:
                        for (int row = 0; row < RowCount; row++)
                            DrawRowsHeaders(pGraphics, row, (IndexFromAddress(row, 0) / ColumnCount).ToString(), va_rowHeaderAppearance.ForeColor, va_rowHeaderAppearance.Font);
                        break;
                    case enuDataStyle.IndexBase1:
                        for (int row = 0; row < RowCount; row++)
                            DrawRowsHeaders(pGraphics, row, (IndexFromAddress(row, 0) / ColumnCount + 1).ToString(), va_rowHeaderAppearance.ForeColor, va_rowHeaderAppearance.Font);
                        break;
                    //case enuDataStyle.Letter:
                    //    for (int row = 0; row < RowCount; row++)
                    //        DrawRowsHeaders(pGraphics, row, ((char)(IndexFromAddress(row, 0) / ColumnCount + 'A')).ToString(), va_rowHeaderAppearance.ForeColor, va_rowHeaderAppearance.Font);
                    //    break;
                    //case enuDataStyle.Binary:
                    //    for (int row = 0; row < RowCount; row++)
                    //        DrawRowsHeaders(pGraphics, row, (1 << IndexFromAddress(row, 0) / ColumnCount).ToString(), va_rowHeaderAppearance.ForeColor, va_rowHeaderAppearance.Font);
                    //    break;
                    case enuDataStyle.User:
                        for (int row = 0; row < RowCount; row++)
                            if (va_rowHeader[row] != null)
                                DrawRowsHeaders(pGraphics, row, va_rowHeader[row], va_rowHeaderAppearance.ForeColor, va_rowHeaderAppearance.Font);
                            else
                                DrawRowsHeaders(pGraphics, row, String.Empty, va_rowHeaderAppearance.ForeColor, va_rowHeaderAppearance.Font);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Dessine une chaîne dans un rectangle donné.
        /// </summary>
        /// <param name="pGraphics">Objet graphique où dessiner.</param>
        /// <param name="pContentBounds">Rectangle destinataire.</param>
        /// <param name="pTexte">Texte à dessiner.</param>
        /// <param name="pCouleur">Couleur du texte.</param>
        /// <param name="pPolice">Police du texte.</param>
        /// <param name="pAlignement">Alignement du texte.</param>
        protected void DrawText(Graphics pGraphics, Rectangle pContentBounds, string pTexte, Color pCouleur, Font pPolice, ContentAlignment pAlignement)
        {
            StringFormat format = new();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            pGraphics.DrawString(pTexte, pPolice,
                new SolidBrush(pCouleur), (RectangleF)pContentBounds, format);
        }
        ///// <summary>
        ///// Dessine une chaîne dans la cellule spécifiée.
        ///// </summary>
        ///// <param name="pGraphics">Objet graphique où dessiner.</param>
        ///// <param name="pRow">Index de la rangée de la cellule.</param>
        ///// <param name="pColumn">Index de la colonne de la cellule.</param>
        ///// <param name="pTexte">Texte à dessiner.</param>
        ///// <param name="pCouleur">Couleur du texte.</param>
        ///// <param name="pPolice">Police du texte.</param>
        ///// <param name="pAlignement">Alignement du texte.</param>
        //protected void DrawText(Graphics pGraphics, int pRow, int pColumn, string pTexte, Color pCouleur, Font pPolice, ContentAlignment pAlignement)
        //{
        //    Rectangle displayRectangle = GetCellContentBounds(pRow, pColumn);
        //    StringFormat format = new StringFormat();

        //    switch (pAlignement)
        //    {
        //        case ContentAlignment.BottomCenter:
        //            format.LineAlignment = StringAlignment.Far;
        //            format.Alignment = StringAlignment.Center;
        //            break;
        //        case ContentAlignment.BottomLeft:
        //            format.LineAlignment = StringAlignment.Far;
        //            format.Alignment = StringAlignment.Near;
        //            break;
        //        case ContentAlignment.BottomRight:
        //            format.LineAlignment = StringAlignment.Far;
        //            format.Alignment = StringAlignment.Far;
        //            break;
        //        case ContentAlignment.MiddleCenter:
        //            format.LineAlignment = StringAlignment.Center;
        //            format.Alignment = StringAlignment.Center;
        //            break;
        //        case ContentAlignment.MiddleLeft:
        //            format.LineAlignment = StringAlignment.Center;
        //            format.Alignment = StringAlignment.Near;
        //            break;
        //        case ContentAlignment.MiddleRight:
        //            format.LineAlignment = StringAlignment.Center;
        //            format.Alignment = StringAlignment.Far;
        //            break;
        //        case ContentAlignment.TopCenter:
        //            format.LineAlignment = StringAlignment.Near;
        //            format.Alignment = StringAlignment.Center;
        //            break;
        //        case ContentAlignment.TopLeft:
        //            format.LineAlignment = StringAlignment.Near;
        //            format.Alignment = StringAlignment.Near;
        //            break;
        //        case ContentAlignment.TopRight:
        //            format.LineAlignment = StringAlignment.Near;
        //            format.Alignment = StringAlignment.Far;
        //            break;
        //        default:
        //            break;
        //    }

        //    pGraphics.DrawString(pTexte, pPolice,
        //        new SolidBrush(pCouleur), (RectangleF)displayRectangle, format);
        //}
        //============================================================================================
        /// <summary>
        /// Dessine le quadrillage dans un objet graphique.
        /// </summary>
        /// <param name="pGraphics">Objet graphique où dessiner.</param>
        protected void DrawGrid(Graphics pGraphics)
        {
            int positionX, positionY, largeurTotale, hauteurTotale;

            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            Pen objCrayon = new(va_gridAppearance.Color, va_gridAppearance.LineSize);

            positionX = va_enteteLgnLarg + Padding.Left + (va_gridAppearance.LineSize >> 1);
            positionY = va_enteteColHaut + Padding.Top + (va_gridAppearance.LineSize >> 1);
            largeurTotale = va_columnCount * largeurCellule + va_enteteLgnLarg + Padding.Left + va_gridAppearance.LineSize;
            hauteurTotale = va_rowCount * hauteurCellule + va_enteteColHaut + Padding.Top + va_gridAppearance.LineSize;

            // correction pour un étrange bug lorsque la taille du crayon est exactement de 1
            if (va_gridAppearance.LineSize == 1)
            {
                largeurTotale--;
                hauteurTotale--;
            }

            if (va_gridAppearance.LineSize > 0)// Cette section dessine la grille seulement
            {
                //Permet de dessiner les lignes horizontales
                for (int i = 0; i <= va_rowCount; i++)
                {
                    pGraphics.DrawLine(objCrayon, positionX, positionY, largeurTotale, positionY);
                    positionY += hauteurCellule;
                }
                positionY = va_enteteColHaut + Padding.Top;
                // Permet de dessiner les lignes verticales
                for (int i = 0; i <= va_columnCount; i++)
                {
                    pGraphics.DrawLine(objCrayon, positionX, positionY, positionX, hauteurTotale);
                    positionX += largeurCellule;
                }
            }
        }
        //============================================================================================
        /// <summary>
        /// Recalcule la largeur et la hauteur de la grille.
        /// </summary>
        protected internal void ReCalculerTaille()
        {
            int width = va_enteteLgnLarg + (Padding.Left + Padding.Right) + ((va_gridAppearance.LineSize + m_cellSize.Width + (m_cellMargin << 1)) * va_columnCount) + va_gridAppearance.LineSize;
            int height = va_enteteColHaut + (Padding.Top + Padding.Bottom) + ((va_gridAppearance.LineSize + m_cellSize.Height + (m_cellMargin << 1)) * va_rowCount) + va_gridAppearance.LineSize;

            // Ligne supprimée le 2/11/2010
           // if (width == Width && height == Height && va_gridOffScreenBitMap != null) return; 

            //---------------------------------------------------------------------------------------------------------
            // On va libérer l'espace mémoire du offscreen bitmap avant de le recréer avec une nouvelle taille
            if (va_gridHMemdc.ToInt32() != 0)
            {
                // Clean up
                PlatformInvokeGDI32.DeleteObject(va_gridHMemBmp);
                PlatformInvokeGDI32.DeleteDC(va_gridHMemdc);
                va_gridOffScreenBitMap.Dispose();
            }
            // On va créer le offscreen bitmap avec la taille actuelle de la grille
            va_gridOffScreenBitMap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            // On va créer un objet Graphics pour dessiner
            Graphics clientDC = CreateGraphics();
            // On va obtenir le handle vers le DC de l'objet Graphics
            IntPtr hdc = clientDC.GetHdc();
            // On va obtenir un handle compatible pour GDI
            va_gridHMemdc = PlatformInvokeGDI32.CreateCompatibleDC(hdc);   
            // On va obtenir un handle sur le bitmap 
            va_gridHMemBmp = va_gridOffScreenBitMap.GetHbitmap();
            // On va associer le bitmap avec le DC
            PlatformInvokeGDI32.SelectObject(va_gridHMemdc, va_gridHMemBmp);
            // On va obtenir un objet Graphics sur lequel on dessine à partir du handle sur le DC
            va_gridOffScreenGraphic = Graphics.FromHdc(va_gridHMemdc);

            // On va libérer l'espace mémoire temporaire
            clientDC.ReleaseHdc(hdc);
            clientDC.Dispose();
            //---------------------------------------------------------------------------------------------------------

            // pas de zone de dessin si le nombre de sprites est 0
            if (va_sprites.Count > 0)
                CreateSpritesOffScreen(width, height);
            //---------------------------------------------------------------------------------------------------------
            Width = width;
            Height = height;
        }
        //============================================================================================
        /// <summary>
        /// Permet de créer une zone de dessin spécialement pour l'affichage des Sprites
        /// </summary>
        /// <param name="pWidth">Largeur du offscreen bitmap</param>
        /// <param name="pHeight">Hauteur du offscreen bitmap </param>
        private void CreateSpritesOffScreen(int pWidth, int pHeight)
        {
            if (va_spriteHMemdc.ToInt32() != 0)
            {
                // Clean up
                PlatformInvokeGDI32.DeleteObject(va_spriteHMemBmp);
                PlatformInvokeGDI32.DeleteDC(va_spriteHMemdc);
                va_spriteOffScreenBitMap.Dispose();
            }
            // On va créer le offscreen bitmap avec la taille actuelle de la grille
            va_spriteOffScreenBitMap = new Bitmap(pWidth, pHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            // On va créer un objet Graphics pour dessiner
            Graphics clientDC = CreateGraphics();
            // On va obtenir le handle vers le DC de l'objet Graphics
            IntPtr hdc = clientDC.GetHdc();
            // On va obtenir un handle compatible pour GDI
            va_spriteHMemdc = PlatformInvokeGDI32.CreateCompatibleDC(hdc);
            // On va obtenir un handle sur le bitmap 
            va_spriteHMemBmp = va_spriteOffScreenBitMap.GetHbitmap();
            // On va associer le bitmap avec le DC
            PlatformInvokeGDI32.SelectObject(va_spriteHMemdc, va_spriteHMemBmp);
            // On va obtenir un objet Graphics sur lequel on dessine à partir du handle sur le DC
            va_spriteOffScreenGraphic = Graphics.FromHdc(va_spriteHMemdc);

            // On va libérer l'espace mémoire temporaire
            clientDC.ReleaseHdc(hdc);
            clientDC.Dispose();
        }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Dessine la sélection sur la cellule selon SelectionAppearance.
        /// </summary>
        /// <param name="pGraphics">Objet graphique où dessiner.</param>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// ----------------------------------------------------------------------------------------------
        protected void DrawSelection(Graphics pGraphics, int pRow, int pColumn)
        {
            //pGraphics.SmoothingMode = SmoothingMode.HighQuality;

            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            Rectangle objRectangle = new();


            //va_selectionVisualElement.Draw(pGraphics, objRectangle);
            // Le code ci-dessous assure que le VisualElement ne sera pas affecté par les calculs de zoom
            if (va_selectionVisualElement is ShapeElement)
            {
                objRectangle.X = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize + (va_selectionAppearance.PenWidth >> 1) + va_selectionAppearance.Padding.Left;
                objRectangle.Y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize + (va_selectionAppearance.PenWidth >> 1) + va_selectionAppearance.Padding.Top;
                objRectangle.Width = m_cellSize.Width + (m_cellMargin << 1) - va_selectionAppearance.PenWidth - va_selectionAppearance.Padding.Left - va_selectionAppearance.Padding.Right;
                objRectangle.Height = m_cellSize.Height + (m_cellMargin << 1) - va_selectionAppearance.PenWidth - va_selectionAppearance.Padding.Top - va_selectionAppearance.Padding.Bottom;

                ShapeElement.DrawShape(va_selectionAppearance.Shape, pGraphics, objRectangle, new Pen(va_selectionAppearance.Color, va_selectionAppearance.PenWidth),va_selectionAppearance.Radius);
            }
            else
            {
                objRectangle.X = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize + va_selectionAppearance.Padding.Left;
                objRectangle.Y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize + va_selectionAppearance.Padding.Top;
                objRectangle.Width = m_cellSize.Width + (m_cellMargin << 1) - va_selectionAppearance.Padding.Left - va_selectionAppearance.Padding.Right;
                objRectangle.Height = m_cellSize.Height + (m_cellMargin << 1) - va_selectionAppearance.Padding.Top - va_selectionAppearance.Padding.Bottom;

                ImageElement element = (ImageElement)va_selectionVisualElement;
                pGraphics.DrawImage(element.Image, objRectangle);
            }
        }

        //-----------------------------------------------------------------------------
        /// <summary>
        /// Dessine la destination dans une opération glisser/déposer.
        /// </summary>
        /// <param name="pGraphics">Objet graphique où dessiner.</param>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// ----------------------------------------------------------------------------------------------
        protected void DrawDragDestination(Graphics pGraphics, int pRow, int pColumn)
        {
            //pGraphics.SmoothingMode = SmoothingMode.HighQuality;

            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            Rectangle objRectangle = new();


            //va_dragVisualElement.Draw(pGraphics, objRectangle);
            // Le code ci-dessous assure que le VisualElement ne sera pas affecté par les calculs de zoom
            switch (va_dragAppearance.Style)
            {
                case enuDragStyle.FillShape:
                    objRectangle.X = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize + va_dragAppearance.Padding.Left;
                    objRectangle.Y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize + va_dragAppearance.Padding.Top;
                    objRectangle.Width = m_cellSize.Width + (m_cellMargin << 1) - va_dragAppearance.Padding.Left - va_dragAppearance.Padding.Right;
                    objRectangle.Height = m_cellSize.Height + (m_cellMargin << 1) - va_dragAppearance.Padding.Top - va_dragAppearance.Padding.Bottom;

                    FillShapeElement.DrawFillShape(va_dragAppearance.Shape, pGraphics, objRectangle, va_dragAppearance.Color, va_dragAppearance.Alpha, va_dragAppearance.Radius);
                    break;
                case enuDragStyle.Shape:
                    objRectangle.X = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize + (va_dragAppearance.PenWidth >> 1) + va_dragAppearance.Padding.Left;
                    objRectangle.Y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize + (va_dragAppearance.PenWidth >> 1) + va_dragAppearance.Padding.Top;
                    objRectangle.Width = m_cellSize.Width + (m_cellMargin << 1) - va_dragAppearance.PenWidth - va_dragAppearance.Padding.Left - va_dragAppearance.Padding.Right;
                    objRectangle.Height = m_cellSize.Height + (m_cellMargin << 1) - va_dragAppearance.PenWidth - va_dragAppearance.Padding.Top - va_dragAppearance.Padding.Bottom;

                    ShapeElement.DrawShape(va_dragAppearance.Shape, pGraphics, objRectangle, new Pen(Color.FromArgb(va_dragAppearance.Alpha,va_dragAppearance.Color), va_dragAppearance.PenWidth), va_dragAppearance.Radius);
                    break;
                case enuDragStyle.Image:
                    if (va_dragAppearance.Image != null)
                    {
                        objRectangle.X = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize + va_dragAppearance.Padding.Left;
                        objRectangle.Y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize + va_dragAppearance.Padding.Top;
                        objRectangle.Width = m_cellSize.Width + (m_cellMargin << 1) - va_dragAppearance.Padding.Left - va_dragAppearance.Padding.Right;
                        objRectangle.Height = m_cellSize.Height + (m_cellMargin << 1) - va_dragAppearance.Padding.Top - va_dragAppearance.Padding.Bottom;

                        objRectangle = CellVisualElement.CellVisualElement.BoundsFromAlignment(objRectangle, va_dragAppearance.Image.Size, ContentAlignment.MiddleCenter);
                        pGraphics.DrawImage(va_dragAppearance.Image, objRectangle);
                    }
                    break;
                default:
                    break;
            }
        }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Dessine toutes les cellules de la grille.
        /// </summary>
        protected void DrawAllCells()
        {
            DrawAllCells(this.CreateGraphics());
        }
        //===============================================================================
        /// <summary>
        /// Transforme les coordonnées de la cellule (rangée, colonne) en pixels. 
        /// </summary>
        /// <param name="pRow">Rangée.</param>
        /// <param name="pColumn">Colonne.</param>
        /// <returns>Coordonnées en pixels.</returns>
        protected Point CelluleAPixels(int pRow, int pColumn)
        {
            Point ptPixels = new();

            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            ptPixels.X = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize + m_cellMargin;
            ptPixels.Y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize + m_cellMargin;
            return ptPixels;
        }
        /// -------------------------------------------------------------------------------------
        /// <summary>
        /// Fournit la rangée et la colonne en fonction des coordonnées en pixels.
        /// </summary>
        /// <param name="pX">Position sur l'axe des X en pixels.</param>
        /// <param name="pY">Position sur l'axe des Y en pixels.</param>
        /// <returns>Adresse de la cellule</returns>
        protected Address PixelsToAddress(int pX, int pY)
        {
            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            int startX = Padding.Left + va_enteteLgnLarg;
            int startY = Padding.Top + va_enteteColHaut;
            int x = pX - startX;
            int y = pY - startY;

            if (x < 0 || y < 0)
                return new Address(-1, -1);
            else
                return new Address(y / hauteurCellule, x / largeurCellule);
        }
        //===============================================================================
        /// <summary>
        /// Fournit une adresse sous la forme rangée et colonne à partir d'un index.
        /// </summary>
        /// <param name="pIndex">Index de la cellule.</param>
        /// <returns>Adresse sous la forme rangée et colonne.</returns>
        public Address AddressFromIndex(int pIndex)
        {
            return IndexToAddress(pIndex);
            //return new Address(coordonnee.Y, coordonnee.X);
        }
        //===============================================================================
        /// <summary>
        /// Convertit un index en une adresse, en tenant compte du mode d'adressage.
        /// Lève une exception si l'index est hors limite.
        /// </summary>
        /// <param name="pIndex">Index à convertir.</param>
        /// <returns>Adresse obtenue</returns>
        protected internal Address IndexToAddress(int pIndex)
        {
            //if (pIndex == -1) // enlevé pour éliminer le bug du Clear(-1);
            //    return Point.Empty;
            int column, row;
            switch (va_addressMode)
            {
                case enuAddressMode.Reverse:
                    pIndex = Length - pIndex - 1;
                    column = pIndex % ColumnCount;
                    row = pIndex / ColumnCount;
                    break;
                case enuAddressMode.Column:
                    column = pIndex / RowCount;
                    row = pIndex % RowCount;
                    break;
                case enuAddressMode.ColumnReverse:
                    pIndex = Length - pIndex - 1;
                    column = pIndex / RowCount;
                    row = pIndex % RowCount;
                    break;
                case enuAddressMode.ReverseRow:
                    column = pIndex % ColumnCount;
                    row = pIndex / ColumnCount;
                    row = RowCount - row - 1;
                    break;
                case enuAddressMode.ReverseColumn:
                    column = pIndex % ColumnCount;
                    column = ColumnCount - column - 1;
                    row = pIndex / ColumnCount;
                    break;
                case enuAddressMode.StairsTopLeft:
                    column = pIndex % ColumnCount;
                    row = pIndex / ColumnCount;
                    if (row % 2 == 1)
                        column = ColumnCount - column - 1;
                    //ligne = NbLignes - ligne - 1;
                    break;
                case enuAddressMode.StairsTopRight:
                    column = pIndex % ColumnCount;
                    row = pIndex / ColumnCount;
                    if (row % 2 == 0)
                        column = ColumnCount - column - 1;
                    //ligne = NbLignes - ligne - 1;
                    break;
                case enuAddressMode.StairsBottomLeft:
                    column = pIndex % ColumnCount;
                    row = pIndex / ColumnCount;
                    if (row % 2 == 1)
                        column = ColumnCount - column - 1;
                    row = RowCount - row - 1;
                    break;
                case enuAddressMode.StairsBottomRight:
                    column = pIndex % ColumnCount;
                    row = pIndex / ColumnCount;
                    if (row % 2 == 0)
                        column = ColumnCount - column - 1;
                    row = RowCount - row - 1;
                    break;
                default: // mode Normal
                    column = pIndex % ColumnCount;
                    row = pIndex / ColumnCount;
                    break;
            }
            if (column >= ColumnCount || column < 0 || row >= RowCount || row < 0)
                throw new VisualArrayException("Débordement de la grille : pIndex = " + pIndex + " , pRow = " + row + " et pColumn = " + column);
            return new Address(row,column);
        }
        //===========================================================================
        /// <summary>
        /// Obtient une adresse en tenant compte du mode d'adressage.
        /// </summary>
        /// <param name="pRow">Rangée.</param>
        /// <param name="pColumn">Colonne.</param>
        /// <returns>Adresse qui tient compte du mode d'adressage.</returns>
        protected internal Address AddressFromAddressMode(int pRow, int pColumn)
        {
            if (pColumn >= ColumnCount || pColumn < 0 || pRow >= RowCount || pRow < 0)
                throw new VisualArrayException("Débordement de la grille : pRow = " + pRow + " et pColumn = " + pColumn);

            int index = IndexFromAddress(pRow, pColumn);
            return new Address(index / ColumnCount, index % ColumnCount);
        }
        //===========================================================================
        /// <summary>
        /// Transforme les coordonnées en un index correspondant 
        /// aux cellules séquentiellement en tenant compte du ModeAdressage.
        /// </summary>
        /// <param name="pRow">Ligne de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>L'index d'une ligne/colonne.</returns>
        /// -------------------------------------------------------------------------
        public int IndexFromAddress(int pRow, int pColumn)
        {
            int index = 0;
            switch (va_addressMode)
            {
                case enuAddressMode.Reverse:
                    index = pRow * ColumnCount + pColumn;
                    index = Length - index - 1;
                    break;
                case enuAddressMode.Column:
                    index = pColumn * RowCount + pRow;
                    break;
                case enuAddressMode.ColumnReverse:
                    index = pColumn * RowCount + pRow;
                    index = Length - index - 1;
                    break;
                case enuAddressMode.ReverseRow:
                    pRow = RowCount - pRow - 1;
                    index = pRow * ColumnCount + pColumn;
                    break;
                case enuAddressMode.ReverseColumn:
                    pColumn = ColumnCount - pColumn - 1;
                    index = pRow * ColumnCount + pColumn;
                    break;
                case enuAddressMode.StairsTopLeft:
                    //pRow = NbLignes - pRow - 1;
                    if (pRow % 2 == 1)
                        pColumn = ColumnCount - pColumn - 1;
                    index = pRow * ColumnCount + pColumn;
                    break;
                case enuAddressMode.StairsTopRight:
                    //pRow = NbLignes - pRow - 1;
                    if (pRow % 2 == 0)
                        pColumn = ColumnCount - pColumn - 1;
                    index = pRow * ColumnCount + pColumn;
                    break;
                case enuAddressMode.StairsBottomLeft:
                    pRow = RowCount - pRow - 1;
                    if (pRow % 2 == 1)
                        pColumn = ColumnCount - pColumn - 1;
                    index = pRow * ColumnCount + pColumn;
                    break;
                case enuAddressMode.StairsBottomRight:
                    pRow = RowCount - pRow - 1;
                    if (pRow % 2 == 0)
                        pColumn = ColumnCount - pColumn - 1;
                    index = pRow * ColumnCount + pColumn;
                    break;
                default: // mode Normal
                    index = pRow * ColumnCount + pColumn;
                    break;
            }
            return index;
        }
        #endregion

        #region MÉTHODES PROTECTED ET VIRTUELLES
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Dessine toutes les cellules de la grille dans l'objet graphique.
        /// </summary>
        /// <param name="pGraphics">Objet graphique où dessiner.</param>
        protected void DrawAllCells(Graphics pGraphics)
        {
            for (int row = 0; row < va_rowCount; row++)
                for (int column = 0; column < va_columnCount; column++)
                    DrawCellContent(pGraphics, row, column);
        }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Dessine les adresses de chacune des cellules dans l'objet graphique.
        /// </summary>
        /// <param name="pGraphics">Objet graphique où dessiner.</param>
        protected void DrawAllAddress(Graphics pGraphics)
        {
            for (int row = 0; row < va_rowCount; row++)
                for (int column = 0; column < va_columnCount; column++)
                    DrawAddress(pGraphics, row, column);
        }
        //--------------------------------------------------------------------------------------
        /// <summary>
        /// Dessine dans un objet graphique, l'adresse de la cellule dont les coordonnées sont fournies.
        /// </summary>
        /// <param name="pGraphics">obj graphique où dessiner</param>
        /// <param name="pRow">Rangée</param>
        /// <param name="pColumn">Colonne</param>
        protected void DrawAddress(Graphics pGraphics, int pRow, int pColumn)
        {
            if (DesignMode && va_addressView != enuAddressView.None)
            {
                string indexEnChaine;
                if (va_addressView == enuAddressView.Mode1D)
                {
                    int index = IndexFromAddress(pRow, pColumn); // +va_baseAddress;
                    indexEnChaine = "[" + index + "]";
                }
                else
                {
                    Address resultat = AddressFromAddressMode(pRow, pColumn);
                    indexEnChaine = "[" + resultat.Row + "," + resultat.Column + "]";
                }
                Rectangle displayRectangle = GetCellContentBounds(pRow, pColumn);
                VisualArraysTools.DrawText(pGraphics, displayRectangle, indexEnChaine, ForeColor, Font, ContentAlignment.MiddleCenter);
            }
        }
        /// <summary>
        /// Dessine des raillures pour signifier une cellule utilisée ou inactive.
        /// </summary>
        /// <param name="pGraphics">Graphique utilisé</param>
        /// <param name="pBounds">Contour de la cellule</param>
        /// <param name="pDisabledAppearance">Caractéristiques d'une cellule inactive</param>/// 
        protected void DrawStrike(Graphics pGraphics, Rectangle pBounds, DisabledAppearance pDisabledAppearance)
        {
            pBounds.X += pDisabledAppearance.StrikeAppearance.Margin;
            pBounds.Y += pDisabledAppearance.StrikeAppearance.Margin;
            pBounds.Width -= pDisabledAppearance.StrikeAppearance.Margin << 1;
            pBounds.Height -= pDisabledAppearance.StrikeAppearance.Margin << 1;
            pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            switch (pDisabledAppearance.StrikeAppearance.Style)
            {
                case enuStrikeStyle.None:
                    break;
                case enuStrikeStyle.Diagonal:
                    Pen diagonalPen = new(pDisabledAppearance.StrikeAppearance.Color, pDisabledAppearance.StrikeAppearance.PenWidth);
                    pGraphics.DrawLine(diagonalPen, pBounds.Right, pBounds.Top, pBounds.Left, pBounds.Bottom);
                    break;
                case enuStrikeStyle.Cross:
                    Pen crossPen = new(pDisabledAppearance.StrikeAppearance.Color, pDisabledAppearance.StrikeAppearance.PenWidth);
                    pGraphics.DrawLine(crossPen, pBounds.Left, pBounds.Top, pBounds.Right, pBounds.Bottom);
                    pGraphics.DrawLine(crossPen, pBounds.Right, pBounds.Top, pBounds.Left, pBounds.Bottom);
                    break;
                case enuStrikeStyle.Image:
                    if (pDisabledAppearance.StrikeAppearance.Image != null)
                    {
                        Rectangle leRect = CellVisualElement.CellVisualElement.BoundsFromAlignment(pBounds, pDisabledAppearance.StrikeAppearance.Image.Size, pDisabledAppearance.StrikeAppearance.Align);
                        pGraphics.DrawImage(pDisabledAppearance.StrikeAppearance.Image, leRect);
                    }
                    break;
                default:
                    break;
            }
            pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        }
        //--------------------------------------------------------------------------------------
        /// <summary>
        /// Dessiner le contenu d'une cellule dans une zone de la taille de la cellule, 
        /// servant à l'opération glisser/déposer.
        /// </summary>
        /// <param name="pGraphics"></param>
        /// <param name="pContentBounds">Contour du contenu de la cellule</param>
        /// <param name="pRow">Rangée</param>
        /// <param name="pColumn">Colonne</param>
        protected virtual void DrawCellDragContent(Graphics pGraphics, Rectangle pContentBounds,int pRow, int pColumn)
        {
            Cell cell = va_tabCells[pRow, pColumn];
            Rectangle backRect = pContentBounds with { X = 0, Y = 0 };
            Rectangle cellBounds = GetCellBounds(pRow, pColumn);
            Rectangle borderRect = cellBounds with { X = 0, Y = 0 };
            cell.Background?.Draw(pGraphics, backRect);
            cell.UserContent?.DrawCellDragContent(pGraphics, backRect, borderRect, cell.Enabled);
        }
        //--------------------------------------------------------------------------------------
        /// <summary>
        /// Dessine dans un objet graphique, la cellule dont les coordonnées sont fournies.
        /// </summary>
        /// <param name="pGraphics">Objet graphique où dessiner.</param>
        /// <param name="pRow">Rangée.</param>
        /// <param name="pColumn">Colonne.</param>
        protected virtual void DrawCellContent(Graphics pGraphics, int pRow, int pColumn)
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

                if (cell.Enabled)
                {
                    cell.Background?.Draw(pGraphics, cellContentBounds);
                }
                else // dans ce cas la cellule n'est pas active Enabled == false
                {
                    va_disabledVisualElement?.Draw(pGraphics, cellContentBounds);
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
                DrawStrike(pGraphics, cellBounds,va_disabledAppearance);

            // Étape 6 : Si la cellule est sélectionnée, alors on doit dessiner la sélection
            if (cell.Selected)
                DrawSelection(pGraphics, pRow, pColumn);
            // Étape 7 : Si nous sommes en mode désign alors on doit dessiner l'adresse de la cellule
            if (DesignMode)
                DrawAddress(pGraphics, pRow, pColumn);
        }
        //protected virtual void DrawCellContent(Graphics pGraphics, int pRow, int pColumn)
        //{
        //    Rectangle cellContentBounds = GetCellContentBounds(pRow, pColumn);
        //    Rectangle cellBounds = GetCellBounds(pRow, pColumn);
        //    Cell cell = va_tabCells[pRow, pColumn];
        //    DrawCellBackground(pGraphics, cellContentBounds, cellBounds, cell);
        //    if (cell.Selected)
        //        DrawSelection(pGraphics, pRow, pColumn);
        //    //DessinerAdresse(pGraphics,pRow,pColumn);
        //}
        /// <summary>
        /// Dessine le fond d'une cellule.
        /// </summary>
        protected void DrawCellBackground(Graphics pGraphics, Rectangle pCellContentBounds, Rectangle pCellBounds, Cell pCell)
        {
            if (BackgroundImage != null)
                pGraphics.DrawImage(BackgroundImage, pCellBounds, pCellBounds, GraphicsUnit.Pixel);
            else
                pGraphics.FillRectangle(new SolidBrush(BackColor), pCellBounds);

            if (pCell.UserContent != null)
                pCell.UserContent.DrawCellContent(pGraphics, pCellContentBounds, pCellBounds,pCell.Enabled);
            else
            {
                pCell.Background?.Draw(pGraphics, pCellContentBounds);
                CellVisualElement.CellVisualElement layerVE = pCell.LayerOver;
                while (layerVE != null)
                {
                    layerVE.Draw(pGraphics, pCellContentBounds);
                    layerVE = layerVE.NextVisualElement;
                }
            }
        }
        //============================================================================================
        ///// <summary>
        ///// Dessine un Sprite sur la grille sur la cellule indiquée dans par le Sprite
        ///// </summary>
        ///// <param name="pobjSprite">Sprite à dessiner</param>
        //internal void DessinerUnSprite(Sprite pobjSprite)
        //{
        //    DessinerUnSprite(pobjSprite, this.CreateGraphics());
        //}
        //============================================================================================
        /// <summary>
        /// Fournit le rectangle englobant une cellule.
        /// </summary>
        /// <param name="pRow">Rangée</param>
        /// <param name="pColumn">Colonne</param>
        /// <returns>Rectangle calculé</returns>
        public Rectangle GetCellBounds(int pRow, int pColumn)
        {
            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            Rectangle contour = new();
            contour.X = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize;
            contour.Y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize;
            contour.Width = m_cellSize.Width + (m_cellMargin << 1);
            contour.Height = m_cellSize.Height + (m_cellMargin << 1);
            return contour;
        }
        /// <summary>
        /// Fournit le rectangle englobant toutes les cellules.
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        public Rectangle GridBounds
        {
            get
            {
                Rectangle contour = new();
                contour.X = va_enteteLgnLarg + Padding.Left;
                contour.Y = va_enteteColHaut + Padding.Top;
                contour.Width = Bounds.Width - Padding.Right - contour.X;
                contour.Height = Bounds.Height - Padding.Bottom - contour.Y;
                return contour;
            }
        }
        //============================================================================================
        /// <summary>
        /// Obtient le contour d'une cellule.
        /// </summary>
        /// <param name="pIndex">Index de la cellule</param>
        /// <returns>Contour d'une cellule</returns>
        public Rectangle GetCellBounds(int pIndex)
        {
            Address emplacement = IndexToAddress(pIndex);
            return GetCellBounds(emplacement.Row, emplacement.Column);
        }
        //============================================================================================
        /// <summary>
        /// Fournit le rectangle englobant la sélection.
        /// </summary>
        /// <param name="pRow">Rangée</param>
        /// <param name="pColumn">Colonne</param>
        /// <returns>Rectangle calculé.</returns>
        protected Rectangle GetCellSelectionBounds(int pRow, int pColumn)
        {
            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            Rectangle contour = new();
            contour.X = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize;
            contour.Y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize;
            contour.Width = m_cellSize.Width + (m_cellMargin << 1);
            contour.Height = m_cellSize.Height + (m_cellMargin << 1);
            return contour;
        }
        //============================================================================================
        /// <summary>
        /// Fournit le rectangle englobant le contenu d'une cellule en tenant compte de la marge.
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Rectangle calculé.</returns>
        public Rectangle GetCellContentBounds(int pRow, int pColumn)
        {
            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            Rectangle contour = new();
            contour.X = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize + m_cellMargin;
            contour.Y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize + m_cellMargin;
            contour.Width = m_cellSize.Width;
            contour.Height = m_cellSize.Height;
            return contour;
        }
        //============================================================================================
        /// <summary>
        /// Fournit le rectangle englobant le contenu d'une cellule en tenant compte de la marge et du padding
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pPadding">Espacement interne</param>
        /// <returns>Rectangle calculé.</returns>
        public Rectangle GetCellContentBounds(int pRow, int pColumn,Padding pPadding)
        {
            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            Rectangle contour = new();
            contour.X = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize + m_cellMargin + pPadding.Left;
            contour.Y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize + m_cellMargin + pPadding.Top;
            contour.Width = m_cellSize.Width - (pPadding.Left + pPadding.Right);
            contour.Height = m_cellSize.Height - (pPadding.Top + pPadding.Bottom);
            return contour;
        }
        //============================================================================================
        /// <summary>
        /// Fournit le point central d'une cellule.
        /// </summary>
        /// <param name="pRow">Ligne de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Point centrale de la cellule en pixels.</returns>
        public Point GetCellMiddle(int pRow, int pColumn)
        {
            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int x = va_enteteLgnLarg + Padding.Left + pColumn * largeurCellule + va_gridAppearance.LineSize + m_cellMargin;
            int y = va_enteteColHaut + Padding.Top + pRow * hauteurCellule + va_gridAppearance.LineSize + m_cellMargin;
            return new Point(x + m_cellSize.Width / 2,y + m_cellSize.Height / 2);
        }

        //============================================================================================
        ///// <summary>
        ///// Dessine un Sprite sur la grille sur la cellule indiquée dans par le Sprite
        ///// </summary>
        ///// <param name="pobjSprite">Sprite à dessiner</param>
        ///// <param name="pGraphics">objet graphique où dessiner</param>
        //protected internal void DessinerUnSprite(Sprite pobjSprite, Graphics pGraphics)
        //{
        //    pGraphics.SmoothingMode = SmoothingMode.HighQuality;
        //    Point emplacement = IndexACoordonnee(pobjSprite.Index);
        //    pobjSprite.Draw(pGraphics, GetCellContentBounds(emplacement.Y,emplacement.X));
        //}
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Redessine une cellule ainsi que le ou les Sprites qui s'y trouvent.
        /// </summary>
        /// <param name="pRow">Rangée de la cellule à mettre à jour</param>
        /// <param name="pColumn">Colonne de la cellule à mettre à jour</param>
        private void UpdateCellAndSprites(int pRow, int pColumn)
        {
            if (va_isUpdating) return;
            Rectangle cellBounds = GetCellBounds(pRow, pColumn);
            Cell cell = va_tabCells[pRow, pColumn];

            if (va_gridOffScreenGraphic != null && va_gridOffScreenBitMap != null)
            {
                DrawCellContent(va_gridOffScreenGraphic, pRow, pColumn);

                //DrawAddress(va_gridOffScreenGraphic, pRow, pColumn);

                //-------------------------------------------------------------------------------------------
                // Code pour afficher la cellule à l'écran
                // Si il n'y aucun sprites alors on peut dessiner directement dans l'écran
                if (va_sprites.Count == 0)
                {
                    Graphics objGraphique = CreateGraphics();
                    IntPtr hdc = objGraphique.GetHdc();
                    PlatformInvokeGDI32.BitBlt(hdc, cellBounds.X, cellBounds.Y, cellBounds.Width, cellBounds.Height, va_gridHMemdc, cellBounds.X, cellBounds.Y, 0x00CC0020);
                    objGraphique.ReleaseHdc(hdc);
                    objGraphique.Dispose();
                }
                else // dans ce cas il faut redessiner la cellule ainsi que les sprites qui la touche
                    UpdateSprites(cellBounds);
                //-------------------------------------------------------------------------------------------
            }
        }
        /// <summary>
        /// Redessine une cellule ainsi que le ou les Sprites qui s'y trouvent.
        /// </summary>
        /// <param name="pIndex">Index de la cellule à mettre à jour</param>
        protected void UpdateCellAndSprites(int pIndex)
        {
            if (va_isUpdating) return;
            Address emplacement = IndexToAddress(pIndex);
            Rectangle cellBounds = GetCellBounds(emplacement.Row, emplacement.Column);
            Cell cell = va_tabCells[emplacement.Row, emplacement.Column];

            if (va_gridOffScreenGraphic != null && va_gridOffScreenBitMap != null)
            {
                DrawCellContent(va_gridOffScreenGraphic, emplacement.Row, emplacement.Column);

                DrawAddress(va_gridOffScreenGraphic, emplacement.Row, emplacement.Column);

                //-------------------------------------------------------------------------------------------
                // Code pour afficher la cellule à l'écran
                // Si il n'y aucun sprites alors on peut dessiner directement dans l'écran
                if (va_sprites.Count == 0)
                {
                    Graphics objGraphique = CreateGraphics();
                    IntPtr hdc = objGraphique.GetHdc();
                    PlatformInvokeGDI32.BitBlt(hdc, cellBounds.X, cellBounds.Y, cellBounds.Width, cellBounds.Height, va_gridHMemdc, cellBounds.X, cellBounds.Y, 0x00CC0020);
                    objGraphique.ReleaseHdc(hdc);
                    objGraphique.Dispose();
                }
                else // dans ce cas il faut redessiner la cellule ainsi que les sprites qui la touche
                    UpdateSprites(cellBounds);
                //-------------------------------------------------------------------------------------------
            }
        }
        /// <summary>
        /// Redessine une cellule ainsi que le ou les Sprites qui s'y trouvent ainsi que le DragAppearance.
        /// </summary>
        /// <param name="pIndex">Index de la cellule à mettre à jour</param>
        protected void UpdateCellAndSpritesDuringDrag(int pIndex)
        {
            if (va_isUpdating) return;

            if (va_gridOffScreenGraphic != null && va_gridOffScreenBitMap != null)
            {
                Address emplacement = IndexToAddress(pIndex);
                Rectangle cellBounds = GetCellBounds(emplacement.Row, emplacement.Column);
                Cell cell = va_tabCells[emplacement.Row, emplacement.Column];

                DrawCellContent(va_gridOffScreenGraphic, emplacement.Row, emplacement.Column);

                //DrawAddress(va_gridOffScreenGraphic, emplacement.Row, emplacement.Column);
                DrawDragDestination(va_gridOffScreenGraphic, emplacement.Row, emplacement.Column);
                //-------------------------------------------------------------------------------------------
                // Code pour afficher la cellule à l'écran
                // Si il n'y aucun sprites alors on peut dessiner directement dans l'écran
                if (va_sprites.Count == 0)
                {
                    Graphics objGraphique = CreateGraphics();
                    IntPtr hdc = objGraphique.GetHdc();
                    PlatformInvokeGDI32.BitBlt(hdc, cellBounds.X, cellBounds.Y, cellBounds.Width, cellBounds.Height, va_gridHMemdc, cellBounds.X, cellBounds.Y, 0x00CC0020);
                    objGraphique.ReleaseHdc(hdc);
                    objGraphique.Dispose();
                }
                else // dans ce cas il faut redessiner la cellule ainsi que les sprites qui la touche
                    UpdateSprites(cellBounds);
                //-------------------------------------------------------------------------------------------
            }
        }
        /// <summary>
        /// Redessine une cellule ainsi que le ou les Sprites qui s'y trouvent ainsi que le DragAppearance.
        /// </summary>
        /// <param name="pSprite"></param>
        /// <param name="pRow"></param>
        /// <param name="pColumn"></param>
        /// <param name="pShowDrag"></param>
        protected void UpdateCellAndSpritesDuringDragPlus(Sprite pSprite, int pRow, int pColumn, bool pShowDrag)
        {
            if (va_isUpdating) return;

            if (va_gridOffScreenGraphic != null && va_gridOffScreenBitMap != null)
            {
                if (pSprite != null && pSprite.m_tabCells.Count > 0)
                {
                    foreach (Address adrCellSprite in pSprite.m_tabCells)
                    {
                        int row = pRow + adrCellSprite.Row;
                        int col = pColumn + adrCellSprite.Column;
                        if (row >= 0 && row < RowCount && col >= 0 && col < ColumnCount)
                        {
                            Rectangle cellBounds = GetCellBounds(row, col);
                            DrawCellContent(va_gridOffScreenGraphic, row, col);
                            if (pShowDrag) DrawDragDestination(va_gridOffScreenGraphic, row, col);
                            UpdateSprites(cellBounds);
                        }
                    }
                }
                else
                {
                    if (pRow >= 0 && pRow < RowCount && pColumn >= 0 && pColumn < ColumnCount)
                    {
                        Rectangle cellBounds = GetCellBounds(pRow, pColumn);
                        DrawCellContent(va_gridOffScreenGraphic, pRow, pColumn);
                        if (pShowDrag) DrawDragDestination(va_gridOffScreenGraphic, pRow, pColumn);
                        UpdateSprites(cellBounds);
                    }
                }
                    ////-------------------------------------------------------------------------------------------
                    //// Code pour afficher la cellule à l'écran
                    //// Si il n'y aucun sprites alors on peut dessiner directement dans l'écran
                    //if (va_sprites.Count == 0)
                    //{
                    //    Graphics objGraphique = CreateGraphics();
                    //    IntPtr hdc = objGraphique.GetHdc();
                    //    PlatformInvokeGDI32.BitBlt(hdc, cellBounds.X, cellBounds.Y, cellBounds.Width, cellBounds.Height, va_gridHMemdc, cellBounds.X, cellBounds.Y, 0x00CC0020);
                    //    objGraphique.ReleaseHdc(hdc);
                    //    objGraphique.Dispose();
                    //}
                    //else // dans ce cas il faut redessiner la cellule ainsi que les sprites qui la touche
                    //-------------------------------------------------------------------------------------------
            }
        }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Dessine à l'écran tous les Sprites qui touchent la zone fournie en paramètre
        /// </summary>
        /// <param name="pBounds">Zone à vérifier</param>
        public void UpdateSprites(Rectangle pBounds)
        {
            Sprite[] tabUpdateSprites = new Sprite[va_sprites.Count];
            int cptUpdateSprites = 0;
            foreach(Sprite sprite in va_sprites)
                if (Rectangle.Intersect(pBounds, sprite.Bounds) != Rectangle.Empty)
                { // On doit redessiner ce Sprite
                    tabUpdateSprites[cptUpdateSprites] = sprite;
                    cptUpdateSprites++;
                }

            Graphics objGraphique = CreateGraphics();
            IntPtr hdc = objGraphique.GetHdc();
            if (cptUpdateSprites > 0) // au moins 1 sprite touche la zone pBounds
            {
                if (va_spriteOffScreenBitMap == null) // il faut s'assurer que la zone offscreen existe
                    CreateSpritesOffScreen(Width, Height);

                PlatformInvokeGDI32.BitBlt(va_spriteHMemdc, pBounds.X, pBounds.Y, pBounds.Width, pBounds.Height, va_gridHMemdc, pBounds.X, pBounds.Y,0x00CC0020);

                for (int index = 0; index < cptUpdateSprites; index++)
                    tabUpdateSprites[index].Draw(va_spriteOffScreenGraphic);

                PlatformInvokeGDI32.BitBlt(hdc, pBounds.X, pBounds.Y, pBounds.Width, pBounds.Height, va_spriteHMemdc, pBounds.X, pBounds.Y, 0x00CC0020);
            }
            else // aucun sprite ne touche la zone pBounds alors on va dessiner la zone directement à l'écran
                PlatformInvokeGDI32.BitBlt(hdc, pBounds.X, pBounds.Y, pBounds.Width, pBounds.Height, va_gridHMemdc, pBounds.X, pBounds.Y, 0x00CC0020);
            objGraphique.ReleaseHdc(hdc);
            objGraphique.Dispose();
        }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Initialise toutes les valeurs de cellules à leurs valeurs par défaut.
        /// </summary>
        protected virtual void ResetAllValuesToDefault()
        {
        }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Modifie le nombres de cellules dans l'entête des colonnes
        /// </summary>
        protected void ResizeColumnHeader()
        {
            HeaderArray newColumnHeader = new(this, va_columnCount, true);
            if (va_columnHeaderAppearance.ValueStyle == enuDataStyle.User)
                Array.Copy(va_columnHeaders.va_elements, newColumnHeader.va_elements, Math.Min(va_columnHeaders.Length, newColumnHeader.Length));
            va_columnHeaders = newColumnHeader;
        }
        /// <summary>
        /// Modifie le nombres de cellules dans l'entête des rangées
        /// </summary>
        protected void ResizeRowHeader()
        {
            HeaderArray newRowHeader = new(this, va_rowCount, false);
            if (va_rowHeaderAppearance.ValueStyle == enuDataStyle.User)
                Array.Copy(va_rowHeader.va_elements, newRowHeader.va_elements, Math.Min(va_rowHeader.Length, newRowHeader.Length));
            va_rowHeader = newRowHeader;
        }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Modifie le nombre de cellules dans les tableaux à partir des propriétés va_rowCount et va_columnCount.
        /// </summary>
        protected virtual void ResizeValueArray()
        {
        }

        //-----------------------------------------------------------------------------
        /// <summary>
        /// va créer un tableau pour les cellules à partir des propriétés va_rowCount et va_columnCount.
        /// </summary>
        private void CreateCellArray()
        {
            va_tabCells = new Cell[va_rowCount, va_columnCount];
            for (int row = 0; row < va_rowCount; row++)
                for (int column = 0; column < va_columnCount; column++)
                    va_tabCells[row, column] = new Cell();
        }
        #endregion

        #region MÉTHODES : Clear
        // CLEAR ------------------------------------------------------------------------------
        /// <summary>
        /// Vide le contenu de la cellule dont l'index est fourni en paramètre.
        /// </summary>
        /// <param name="pIndex">Index de la cellule à vider.</param>
        /// ----------------------------------------------------------------------------------
        public virtual void Clear(int pIndex)
        {
        }
        // CLEAR ------------------------------------------------------------------------------
        /// <summary>
        /// Vide le contenu de la cellule à une position.
        /// </summary>
        /// <param name="pRow">Rangée de la cellule à vider.</param>
        /// <param name="pColumn">Colonne de la cellule à vider.</param>
        /// ----------------------------------------------------------------------------------
        public virtual void Clear(int pRow, int pColumn)
        {
        }
        #endregion

        #region ÉVÉNEMENTS
        /// <summary>
        /// Se produit lorsque le nombre de cellules de la grille change.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le nombre de cellules de la grille change.")]
        public event EventHandler LengthChanged;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Permet d'intercepter les événements reliés aux touches de déplacement du curseur
        /// afin de modifier les actions sur la grille.
        /// </summary>
        /// <param name="key">La touche courante.</param>
        /// <returns> Si la touche est acceptée ou non.</returns>
        /// -----------------------------------------------------------------------------
        protected override bool IsInputKey(Keys key)
        {
            if (!Enabled)
                return base.IsInputKey(key);

            if (va_selectionMode == SelectionMode.None || va_selectionMode == SelectionMode.MultiSimple)
            {
                switch (key)
                {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                        return true;
                    default:
                        return base.IsInputKey(key);
                }
            }
            Address adresse;
            if (va_selectedIndex == -1)
                adresse = new Address(0, 0);
            else
                adresse = IndexToAddress(va_selectedIndex);
            Address saveAddress = adresse;
            bool modifOk = false;
            int cptItération = 0;
            switch (key)
            {
                case Keys.Delete:
                    if (SelectedIndex != -1 && !va_readOnly)
                        Clear(SelectedIndex);
                    break;
                case Keys.Escape:
                    SelectedIndex = -1;
                    return true;
                case Keys.Enter:
                    va_currentKeyTime = new DateTime(0);
                    if (!va_enterKeyNextIndex)
                        return base.IsInputKey(key);
                    if (va_selectedIndex != -1 && va_selectedIndex < Length - 1)
                        IsInputKey(Keys.Right);
                    if (EnterKeyPress != null && va_selectedIndex >= 0)
                        EnterKeyPress(this, new EventArgs());
                    return true;
                case Keys.Up:
                    if (va_selectedIndex == -1)
                    {
                        adresse.Row++;
                        saveAddress = adresse;
                    }
                    while (adresse.Row > 0 && cptItération < Length)
                    {
                        cptItération++;
                        adresse.Row--;
                        if (IsOkToSelect(null,adresse.Row, adresse.Column))
                        {
                            modifOk = true;
                            break;
                        }
                    }
                    if (modifOk)
                        SelectedIndex = IndexFromAddress(adresse.Row, adresse.Column);
                    return true;
                case Keys.Down:
                    if (va_selectedIndex == -1)
                    {
                        adresse.Row--;
                        saveAddress = adresse;
                    }
                    while (adresse.Row < va_rowCount - 1 && cptItération < Length)
                    {
                        adresse.Row++;
                        cptItération++;
                        if (IsOkToSelect(null, adresse.Row, adresse.Column))
                        {
                            modifOk = true;
                            break;
                        }
                    }
                    if (modifOk)
                        SelectedIndex = IndexFromAddress(adresse.Row, adresse.Column);
                    return true;
                case Keys.Right:
                    if (va_selectedIndex == -1)
                    {
                        adresse.Column--;
                        saveAddress = adresse;
                    }
                    do
                    {
                        cptItération++;
                        adresse.Column++;
                        if (adresse.Column >= va_columnCount)
                        {
                            adresse.Column = 0;
                            adresse.Row++;
                            if (adresse.Row >= va_rowCount)
                                adresse.Row = 0;
                        }
                        if (IsOkToSelect(null, adresse.Row, adresse.Column))
                        {
                            modifOk = true;
                            break;
                        }
                    } while (adresse != saveAddress && cptItération < Length);
                    if (modifOk)
                        SelectedIndex = IndexFromAddress(adresse.Row, adresse.Column);
                    return true;
                case Keys.Left:
                    do
                    {
                        cptItération++;
                        adresse.Column--;
                        if (adresse.Column < 0)
                        {
                            adresse.Column = va_columnCount - 1;
                            adresse.Row--;
                            if (adresse.Row < 0)
                                adresse.Row = va_rowCount - 1;
                        }
                        if (IsOkToSelect(null, adresse.Row, adresse.Column))
                        {
                            modifOk = true;
                            break;
                        }
                    } while (adresse != saveAddress && cptItération < Length);
                    if (modifOk)
                        SelectedIndex = IndexFromAddress(adresse.Row, adresse.Column);
                    return true;
                case Keys.Home:
                    SelectedIndex = 0;
                    return true;
                case Keys.End:
                    SelectedIndex = Length - 1;
                    return true;
            }
            return base.IsInputKey(key);
        }
        //============================================================================================
        /// <summary>
        /// Se produit lorsque la touche Entrée est enfoncée.
        /// </summary>
        /// ------------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit la touche Entrée est enfoncée.")]
        public event EventHandler EnterKeyPress;

        //============================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //va_realGraphic = e.Graphics; // 27-04-2010
            if (va_gridOffScreenGraphic == null) ReCalculerTaille();

            va_gridOffScreenGraphic.Clear(BackColor);
            if (BackgroundImage != null)
            {
                Rectangle srcRect = new(new Point(0, 0), BackgroundImage.Size);
                va_gridOffScreenGraphic.DrawImage(BackgroundImage, srcRect, srcRect, GraphicsUnit.Pixel);
            }

            DrawAllHeaders(va_gridOffScreenGraphic);
            DrawGrid(va_gridOffScreenGraphic);
            DrawAllCells(va_gridOffScreenGraphic);
            //va_offScreenGraphic.SmoothingMode = SmoothingMode.HighQuality;
            if (va_selectionMode == SelectionMode.One && va_selectedIndex != -1) //DessinerSelection(va_offScreenGraphic);
            {
                Address adresse = IndexToAddress(va_selectedIndex);
                DrawSelection(va_gridOffScreenGraphic, adresse.Row, adresse.Column);
            }
            if (DesignMode && va_addressView != enuAddressView.None)
                DrawAllAddress(va_gridOffScreenGraphic);

          //----------------------------------------------------------------------------------------------------------------
            IntPtr hdc = e.Graphics.GetHdc();
            PlatformInvokeGDI32.BitBlt(hdc, ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height, va_gridHMemdc, ClientRectangle.X, ClientRectangle.Y, 0x00CC0020);
            e.Graphics.ReleaseHdc(hdc);
          //----------------------------------------------------------------------------------------------------------------
            foreach (Sprite objSprite in va_sprites)
                objSprite.Draw(e.Graphics);

            base.OnPaint(e); // 30-05-2011
        }
        //============================================================================================
        private void RecalculerSelonResizeMode()
        {
            int hauteurCellule, largeurCellule, nbRangées, nbColonnes;
            switch (va_resizeMode)
            {
                case enuResizeMode.Normal:
                    m_cellSize.Width = (Width - va_enteteLgnLarg - (Padding.Left + Padding.Right) - va_gridAppearance.LineSize) / va_columnCount - (m_cellMargin << 1) - va_gridAppearance.LineSize;
                    m_cellSize.Height = (Height - va_enteteColHaut - (Padding.Top + Padding.Bottom) - va_gridAppearance.LineSize) / va_rowCount - (m_cellMargin << 1) - va_gridAppearance.LineSize;
                    ReCalculerTaille();
                    break;
                case enuResizeMode.RowColumn:
                    largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
                    hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;
                    nbColonnes = (Width - va_enteteLgnLarg - (largeurCellule >> 1) - (Padding.Left + Padding.Right)) / largeurCellule + 1;
                    nbRangées = (Height - va_enteteColHaut - (hauteurCellule >> 1) - (Padding.Top + Padding.Bottom)) / hauteurCellule + 1;
                    ReCalculerTaille();
                    if ((va_columnCount != nbColonnes && nbColonnes > 0) || (va_rowCount != nbRangées && nbRangées > 0))
                    {
                        if (va_columnCount != nbColonnes && nbColonnes > 0)
                        {
                            va_columnCount = nbColonnes;
                            ResizeColumnHeader();
                        }
                        if (va_rowCount != nbRangées && nbRangées > 0)
                        {
                            va_rowCount = nbRangées;
                            ResizeRowHeader();
                        }
                        CreateCellArray();
                        ResizeValueArray();
                        ResetAllValuesToDefault();
                        UpdateCellsBkgVisualElement();
                        LengthChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case enuResizeMode.Row:
                    m_cellSize.Width = (Width - va_enteteLgnLarg - (Padding.Left + Padding.Right) - va_gridAppearance.LineSize) / va_columnCount - (m_cellMargin << 1) - va_gridAppearance.LineSize;
                    hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;
                    nbRangées = (Height - va_enteteColHaut - (hauteurCellule >> 1) - (Padding.Top + Padding.Bottom)) / hauteurCellule + 1;
                    ReCalculerTaille();
                    if (va_rowCount != nbRangées && nbRangées > 0)
                    {
                        va_rowCount = nbRangées;
                        ResizeRowHeader();
                        CreateCellArray();
                        ResizeValueArray();
                        ResetAllValuesToDefault();
                        UpdateCellsBkgVisualElement();
                        LengthChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case enuResizeMode.Column:
                    m_cellSize.Height = (Height - va_enteteColHaut - (Padding.Top + Padding.Bottom) - va_gridAppearance.LineSize) / va_rowCount - (m_cellMargin << 1) - va_gridAppearance.LineSize;
                    largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
                    nbColonnes = (Width - va_enteteLgnLarg - (largeurCellule >> 1) - (Padding.Left + Padding.Right)) / largeurCellule + 1;
                    ReCalculerTaille();
                    if (va_columnCount != nbColonnes && nbColonnes > 0)
                    {
                        va_columnCount = nbColonnes;
                        ResizeColumnHeader();
                        CreateCellArray();
                        ResizeValueArray();
                        ResetAllValuesToDefault();
                        UpdateCellsBkgVisualElement();
                        LengthChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            RecalculerSelonResizeMode();
            RefreshSpritesBounds();
        }
        /// <summary>
        /// Actuellement il n'y a rien de particulier à faire lorsque la grille n'est plus active
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            //base.OnEnabledChanged(e);
        }
        #endregion

        #region ÉVÉNEMENTS avec SpriteMouseEventArgs
        /// <summary>
        /// Se produit lors d'un MouseDown sur un Sprite visible.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lors d'un MouseDown sur un Sprite visible.")]
        public event EventHandler<SpriteMouseEventArgs> SpriteMouseDown;

        /// <summary>
        /// Se produit lors d'un MouseDown sur un Sprite visible.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lors d'un MouseUp sur un Sprite visible.")]
        public event EventHandler<SpriteMouseEventArgs> SpriteMouseUp;

        /// <summary>
        /// Se produit lors d'un MouseClick sur un Sprite visible.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lors d'un MouseClick sur un Sprite visible.")]
        public event EventHandler<SpriteMouseEventArgs> SpriteMouseClick;

        /// <summary>
        /// Se produit lors d'un MouseDoubleClick sur un Sprite visible.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lors d'un MouseDoubleClick sur un Sprite visible.")]
        public event EventHandler<SpriteMouseEventArgs> SpriteMouseDoubleClick;

        /// <summary>
        /// Se produit lorsque le pointeur de la souris entre sur un Sprite visible.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le pointeur de la souris entre sur un Sprite visible.")]
        public event EventHandler<SpriteEventArgs> SpriteMouseEnter;

        /// <summary>
        /// Se produit lorsque le pointeur de la souris quitte un Sprite visible.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le pointeur de la souris quitte un Sprite visible.")]
        public event EventHandler<SpriteEventArgs> SpriteMouseLeave;

        /// <summary>
        /// Se produit lorsque le pointeur de la souris se déplace sur un Sprite visible.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le pointeur de la souris se déplace sur un Sprite visible.")]
        public event EventHandler<SpriteMouseEventArgs> SpriteMouseMove;

        #endregion

        #region ÉVÉNEMENTS avec CellMouseEventArgs
        /// <summary>
        /// Se produit lors d'un MouseDown dans une cellule de la grille.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lors d'un MouseDown dans une cellule de la grille.")]
        public event EventHandler<CellMouseEventArgs> CellMouseDown;

        /// <summary>
        /// Se produit lors d'un MouseDown dans une cellule de la grille.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lors d'un MouseUp dans une cellule de la grille.")]
        public event EventHandler<CellMouseEventArgs> CellMouseUp;

        /// <summary>
        /// Se produit lors d'un MouseClick dans une cellule de la grille.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lors d'un MouseClick dans une cellule de la grille.")]
        public event EventHandler<CellMouseEventArgs> CellMouseClick;

        /// <summary>
        /// Se produit lors d'un MouseDoubleClick dans une cellule de la grille.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lors d'un MouseDoubleClick dans une cellule de la grille.")]
        public event EventHandler<CellMouseEventArgs> CellMouseDoubleClick;

        /// <summary>
        /// Se produit lorsque le pointeur de la souris entre dans une cellule de la grille.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le pointeur de la souris entre dans une cellule de la grille.")]
        public event EventHandler<CellEventArgs> CellMouseEnter;

        /// <summary>
        /// Se produit lorsque le pointeur de la souris quitte une cellule de la grille.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le pointeur de la souris quitte une cellule de la grille.")]
        public event EventHandler<CellEventArgs> CellMouseLeave;

        /// <summary>
        /// Se produit lorsque le pointeur de la souris se déplace sur une cellule de la grille.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le pointeur de la souris se déplace sur une cellule de la grille.")]
        public event EventHandler<CellMouseEventArgs> CellMouseMove;

        #endregion

        #region ÉVÉNEMENTS avec ColumnHeaderEventArgs et RowHeaderEventArgs

        /// <summary>
        /// Se produit lors d'un MouseDown sur l'en-tête des colonnes.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lors d'un MouseDown sur l'en-tête des colonnes.")]
        public event EventHandler<ColumnHeaderEventArgs> ColumnHeaderMouseDown;

        /// <summary>
        /// Se produit lors d'un MouseUp sur l'en-tête des colonnes.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lors d'un MouseUp sur l'en-tête des colonnes.")]
        public event EventHandler<ColumnHeaderEventArgs> ColumnHeaderMouseUp;

        /// <summary>
        /// Se produit lors d'un clic sur l'en-tête des colonnes.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lors d'un clic sur l'en-tête des colonnes.")]
        public event EventHandler<ColumnHeaderEventArgs> ColumnHeaderClick;

        /// <summary>
        /// Se produit lors d'un DoubleClic sur l'en-tête des colonnes.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lors d'un DoubleClick sur l'en-tête des colonnes.")]
        public event EventHandler<ColumnHeaderEventArgs> ColumnHeaderDoubleClick;

        /// <summary>
        /// Se produit lorsque le pointeur de la souris entre dans une cellule de l'en-tête des colonnes.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le pointeur de la souris entre dans une cellule de l'en-tête des colonnes.")]
        public event EventHandler<ColumnHeaderEventArgs> ColumnHeaderMouseEnter;

        /// <summary>
        /// Se produit lorsque le pointeur de la souris quitte une cellule de l'en-tête des colonnes.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le pointeur de la souris quitte une cellule de l'en-tête des colonnes.")]
        public event EventHandler<ColumnHeaderEventArgs> ColumnHeaderMouseLeave;


        /// <summary>
        /// Se produit lors d'un MouseDown sur l'en-tête des rangées.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lors d'un MouseDown sur l'en-tête des rangées.")]
        public event EventHandler<RowHeaderEventArgs> RowHeaderMouseDown;

        /// <summary>
        /// Se produit lors d'un MouseUp sur l'en-tête des rangées.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lors d'un MouseUp sur l'en-tête des rangées.")]
        public event EventHandler<RowHeaderEventArgs> RowHeaderMouseUp;

        /// <summary>
        /// Se produit lors d'un clic sur l'en-tête des rangées.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lors d'un clic sur l'en-tête des rangées.")]
        public event EventHandler<RowHeaderEventArgs> RowHeaderClick;

        /// <summary>
        /// Se produit lors d'un DoubleClic sur l'en-tête des rangées.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lors d'un DoubleClick sur l'en-tête des rangées.")]
        public event EventHandler<RowHeaderEventArgs> RowHeaderDoubleClick;

        /// <summary>
        /// Se produit lorsque le pointeur de la souris entre dans une de l'en-tête des colonnes.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le pointeur de la souris entre dans une cellule de l'en-tête des rangées.")]
        public event EventHandler<RowHeaderEventArgs> RowHeaderMouseEnter;

        /// <summary>
        /// Se produit lorsque le pointeur de la souris quitte une de l'en-tête des colonnes.
        /// </summary>
        /// -------------------------------------------------------------------------------------
        [Category("VisualArrays"), Description("Se produit lorsque le pointeur de la souris quitte une cellule de l'en-tête des rangées.")]
        public event EventHandler<RowHeaderEventArgs> RowHeaderMouseLeave;


        #endregion

        #region ÉVÉNEMENTS GÉNÉRÉS

        int va_mouseCurrentIndex;
        int va_mouseCurrentColumn;
        int va_mouseCurrentRow;
        Sprite va_mouseCurrentSprite = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            int saveMouseCurrentIndex = va_mouseCurrentIndex;
            int saveMouseCurrentColumn = va_mouseCurrentColumn;
            int saveMouseCurrentRow = va_mouseCurrentRow;
            Sprite saveMouseCurrentSprite = va_mouseCurrentSprite;

            va_mouseCurrentIndex = -1;
            va_mouseCurrentColumn = -1;
            va_mouseCurrentRow = -1;
            va_mouseCurrentSprite = null;

            if (CellMouseLeave != null && saveMouseCurrentIndex != -1)
                CellMouseLeave(this, new CellEventArgs(saveMouseCurrentIndex, saveMouseCurrentIndex / va_columnCount, saveMouseCurrentIndex % va_columnCount));

            if (ColumnHeaderMouseLeave != null && saveMouseCurrentColumn != -1)
                ColumnHeaderMouseLeave(this, new ColumnHeaderEventArgs(saveMouseCurrentColumn));

            if (RowHeaderMouseLeave != null && saveMouseCurrentRow != -1)
                RowHeaderMouseLeave(this, new RowHeaderEventArgs(saveMouseCurrentRow));

            if (SpriteMouseLeave != null && saveMouseCurrentSprite != null)
                SpriteMouseLeave(this, new SpriteEventArgs(saveMouseCurrentSprite));
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            int saveMouseCurrentIndex = va_mouseCurrentIndex;
            int saveMouseCurrentColumn = va_mouseCurrentColumn;
            int saveMouseCurrentRow = va_mouseCurrentRow;
            Sprite saveMouseCurrentSprite = va_mouseCurrentSprite;

            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            int x = e.X - Padding.Left - va_enteteLgnLarg;
            int y = e.Y - Padding.Top - va_enteteColHaut;

            int colonne = x / largeurCellule;
            int rangee = y / hauteurCellule;

            // Si on passe au dessus d'un Sprite visible
            Sprite objSprite = HitSprite(e.X, e.Y);
            if (objSprite != null)
            {
                //va_mouseCurrentIndex = -1; // 2012/09/18
                va_mouseCurrentColumn = -1;
                va_mouseCurrentRow = -1;

                if (ColumnHeaderMouseLeave != null && saveMouseCurrentColumn != -1)
                    ColumnHeaderMouseLeave(this, new ColumnHeaderEventArgs(saveMouseCurrentColumn));

                if (RowHeaderMouseLeave != null && saveMouseCurrentRow != -1)
                    RowHeaderMouseLeave(this, new RowHeaderEventArgs(saveMouseCurrentRow));

                //DEBUT 2012/09/18 

                //if (CellMouseLeave != null && saveMouseCurrentIndex != -1)
                  //  CellMouseLeave(this, new CellEventArgs(saveMouseCurrentIndex, saveMouseCurrentIndex / va_columnCount, saveMouseCurrentIndex % va_columnCount));
                int index = IndexFromAddress(rangee, colonne);

                if (index != va_mouseCurrentIndex) // on se déplace sur une nouvelle cellule
                {
                    if (CellMouseLeave != null && saveMouseCurrentIndex != -1)
                        CellMouseLeave(this, new CellEventArgs(saveMouseCurrentIndex, saveMouseCurrentIndex / va_columnCount, saveMouseCurrentIndex % va_columnCount));

                    va_mouseCurrentIndex = index;

                    CellMouseEnter?.Invoke(this, new CellEventArgs(index, rangee, colonne));
                }

                //FIN 2012/09/18 

                if (objSprite != va_mouseCurrentSprite) // on se déplace sur un nouveau Sprite
                {
                    if (SpriteMouseLeave != null && saveMouseCurrentSprite != null)
                        SpriteMouseLeave(this, new SpriteEventArgs(saveMouseCurrentSprite));

                    va_mouseCurrentSprite = objSprite;

                    SpriteMouseEnter?.Invoke(this, new SpriteEventArgs(objSprite));
                }

                SpriteMouseMove?.Invoke(this, new SpriteMouseEventArgs(e.Button, e.Clicks, x % largeurCellule, y % hauteurCellule, e.Delta, objSprite));
            }
            // Si on passe sur une cellule de l'en-tête des colonnes
            else if (y < 0 && e.Y >= Padding.Top && x >= 0 && colonne < ColumnCount)
            {
                va_mouseCurrentIndex = -1;
                va_mouseCurrentRow = -1;
                va_mouseCurrentSprite = null;

                if (SpriteMouseLeave != null && saveMouseCurrentSprite != null)
                    SpriteMouseLeave(this, new SpriteEventArgs(saveMouseCurrentSprite));

                // on doit vérfier si on quitte une autre zone
                if (RowHeaderMouseLeave != null && saveMouseCurrentRow != -1)
                    RowHeaderMouseLeave(this, new RowHeaderEventArgs(saveMouseCurrentRow));

                if (CellMouseLeave != null && saveMouseCurrentIndex != -1)
                    CellMouseLeave(this, new CellEventArgs(saveMouseCurrentIndex, saveMouseCurrentIndex / va_columnCount, saveMouseCurrentIndex % va_columnCount));

                if (colonne != va_mouseCurrentColumn) // nouvelle cellule en-tête de colonne
                {
                    va_mouseCurrentColumn = colonne;
                    if (ColumnHeaderMouseLeave != null && saveMouseCurrentColumn != -1)
                        ColumnHeaderMouseLeave(this, new ColumnHeaderEventArgs(saveMouseCurrentColumn));

                    ColumnHeaderMouseEnter?.Invoke(this, new ColumnHeaderEventArgs(colonne));
                }
            }
            // Si on passe sur une cellule de l'en-tête des rangées
            else if (x < 0 && e.X >= Padding.Left && y >= 0 && rangee < RowCount)
            {
                va_mouseCurrentIndex = -1;
                va_mouseCurrentColumn = -1;
                va_mouseCurrentSprite = null;

                if (SpriteMouseLeave != null && saveMouseCurrentSprite != null)
                    SpriteMouseLeave(this, new SpriteEventArgs(saveMouseCurrentSprite));

                if (ColumnHeaderMouseLeave != null && saveMouseCurrentColumn != -1)
                    ColumnHeaderMouseLeave(this, new ColumnHeaderEventArgs(saveMouseCurrentColumn));

                if (CellMouseLeave != null && saveMouseCurrentIndex != -1)
                    CellMouseLeave(this, new CellEventArgs(saveMouseCurrentIndex, saveMouseCurrentIndex / va_columnCount, saveMouseCurrentIndex % va_columnCount));


                if (rangee != va_mouseCurrentRow) // nouvelle cellule en-tête de rangée
                {
                    va_mouseCurrentRow = rangee;
                    if (RowHeaderMouseLeave != null && saveMouseCurrentRow != -1)
                        RowHeaderMouseLeave(this, new RowHeaderEventArgs(saveMouseCurrentRow));

                    RowHeaderMouseEnter?.Invoke(this, new RowHeaderEventArgs(rangee));
                }            
            }
            // Si on passe au dessus d'aucune cellule ou en-tête
            else if (x < 0 || colonne >= ColumnCount || y < 0 || rangee >= RowCount)
            {
                va_mouseCurrentIndex = -1;
                va_mouseCurrentColumn = -1;
                va_mouseCurrentRow = -1;
                va_mouseCurrentSprite = null;

                if (SpriteMouseLeave != null && saveMouseCurrentSprite != null)
                    SpriteMouseLeave(this, new SpriteEventArgs(saveMouseCurrentSprite));

                if (ColumnHeaderMouseLeave != null && saveMouseCurrentColumn != -1)
                    ColumnHeaderMouseLeave(this, new ColumnHeaderEventArgs(saveMouseCurrentColumn));

                if (RowHeaderMouseLeave != null && saveMouseCurrentRow != -1)
                    RowHeaderMouseLeave(this, new RowHeaderEventArgs(saveMouseCurrentRow));

                if (CellMouseLeave != null && saveMouseCurrentIndex != -1)
                    CellMouseLeave(this, new CellEventArgs(saveMouseCurrentIndex, saveMouseCurrentIndex / va_columnCount, saveMouseCurrentIndex % va_columnCount));
            }
            else // autrement on passe au dessus d'une cellule
            {
                va_mouseCurrentColumn = -1;
                va_mouseCurrentRow = -1;
                va_mouseCurrentSprite = null;

                if (SpriteMouseLeave != null && saveMouseCurrentSprite != null)
                    SpriteMouseLeave(this, new SpriteEventArgs(saveMouseCurrentSprite));

                if (ColumnHeaderMouseLeave != null && saveMouseCurrentColumn != -1)
                    ColumnHeaderMouseLeave(this, new ColumnHeaderEventArgs(saveMouseCurrentColumn));

                if (RowHeaderMouseLeave != null && saveMouseCurrentRow != -1)
                    RowHeaderMouseLeave(this, new RowHeaderEventArgs(saveMouseCurrentRow));

                int index = IndexFromAddress(rangee, colonne);

                if (index != va_mouseCurrentIndex) // on se déplace sur une nouvelle cellule
                {
                     if (CellMouseLeave != null && saveMouseCurrentIndex != -1)
                        CellMouseLeave(this, new CellEventArgs(saveMouseCurrentIndex, saveMouseCurrentIndex / va_columnCount, saveMouseCurrentIndex % va_columnCount));

                    va_mouseCurrentIndex = index;

                    CellMouseEnter?.Invoke(this, new CellEventArgs(index, rangee, colonne));
                }

                CellMouseMove?.Invoke(this, new CellMouseEventArgs(e.Button, e.Clicks, x % largeurCellule, y % hauteurCellule, e.Delta, index, index / va_columnCount, index % va_columnCount));

            }
            base.OnMouseMove(e); 
        }

        /// <summary>
        /// Fournit le Sprite en collision avec les coordonnées passées en paramètres
        /// </summary>
        /// <param name="pX">Coordonnée X à vérifier</param>
        /// <param name="pY">Coordonnée Y à vérifier</param>
        /// <returns></returns>
        private Sprite HitSprite(int pX, int pY)
        {
            Rectangle rectangle = new(pX, pY, 1, 1);
            for (int index = va_sprites.Count - 1; index >= 0; index--)
            {
                Sprite objSprite = va_sprites[index];
                if (objSprite.Hit(rectangle))
                    return objSprite;
            }
            return null;
        }

        int va_lastMouseClickCell;
        int va_lastMouseClickRow;
        int va_lastMouseClickColumn;
        Sprite va_lastMouseClickSprite = null;
        /// <summary>
        /// Se charge d'un double clic sur une cellule de la grille.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            int x = e.X - Padding.Left - va_enteteLgnLarg;
            int y = e.Y - Padding.Top - va_enteteColHaut;

            int colonne = x / largeurCellule;
            int rangee = y / hauteurCellule;

            if (colonne >= ColumnCount || rangee >= RowCount) return;

            Sprite objSprite = HitSprite(e.X, e.Y);
            if (objSprite != null && objSprite == va_lastMouseClickSprite)
            {
                va_lastMouseClickSprite = null;
                SpriteMouseDoubleClick?.Invoke(this, new SpriteMouseEventArgs(e.Button, e.Clicks, x % largeurCellule, y % hauteurCellule, e.Delta, objSprite));
            }
            else if (y < 0 && e.Y >= Padding.Top && x >= 0 && colonne < ColumnCount) // Si on clique sur une cellule de l'en-tête des rangées
            {
                if (colonne == va_lastMouseClickColumn)
                {
                    va_lastMouseClickColumn = -1;
                    ColumnHeaderDoubleClick?.Invoke(this, new ColumnHeaderEventArgs(colonne));
                }
            }
            // Si on clique sur une cellule de l'en-tête des rangées
            else if (x < 0 && e.X >= Padding.Left && y >= 0 && rangee < RowCount)
            {
                if (rangee == va_lastMouseClickRow)
                {
                    va_lastMouseClickRow = -1;
                    RowHeaderDoubleClick?.Invoke(this, new RowHeaderEventArgs(rangee));
                }
            }
            else
            {
                int index = IndexFromAddress(rangee, colonne);
                Cell cell = va_tabCells[rangee, colonne];
                if (index == va_lastMouseClickCell)
                {
                    va_lastMouseClickCell = -1;
                    if (CellMouseDoubleClick != null && IsOkToSelect(cell, rangee, colonne))
                        CellMouseDoubleClick(this, new CellMouseEventArgs(e.Button, e.Clicks, x % largeurCellule, y % hauteurCellule, e.Delta, index, index / va_columnCount, index % va_columnCount));
                }
            }
            base.OnMouseDoubleClick(e);
        }
        //================================================================================================================
        private void DoDragCell(MouseEventArgs e,int pIndex,int pRow,int pColumn)
        {
            //m_dragInfos = String.Format("{0},{1},{2},{3},{4}", "CELL", this.Name, pIndex, pRow, pColumn);
            m_dragInfos = new DragAndDropInfos(enuTypeElement.Cell, this.Name, pIndex, pIndex / va_columnCount, pIndex % va_columnCount, null);
            // Code pour tester le nouveau D&D
            Rectangle cellContentBounds = GetCellContentBounds(pRow, pColumn);
            Bitmap bmp = new(cellContentBounds.Width, cellContentBounds.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Magenta);
                DrawCellDragContent(g,cellContentBounds, pRow, pColumn);
            }

            DataObject data = new(new DragHelper.DataObject());

            ShDragImage shdi = new();
            Win32Size size;
            size.cx = bmp.Width;
            size.cy = bmp.Height;
            shdi.sizeDragImage = size;
            Point p = e.Location;
            Win32Point wpt;
            wpt.x = p.X - e.X + cellContentBounds.Width / 2;
            wpt.y = p.Y - e.Y + cellContentBounds.Height / 2;
            shdi.ptOffset = wpt;
            shdi.hbmpDragImage = bmp.GetHbitmap();
            shdi.crColorKey = Color.Magenta.ToArgb();

            IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();
            sourceHelper.InitializeFromBitmap(ref shdi, data);
            DoDragDrop(data, DragDropEffects.Move);
        }
        //================================================================================================================
        private void DoDragSprite(MouseEventArgs e, Sprite pSprite, int pIndex)
        {
            //m_dragInfos = String.Format("{0},{1},{2},{3},{4},{5}", "SPRITE", this.Name, pIndex, pIndex / va_columnCount, pIndex % va_columnCount, va_sprites.IndexOf(pSprite));
            m_dragInfos = new DragAndDropInfos(enuTypeElement.Sprite, this.Name, pIndex, pIndex / va_columnCount, pIndex % va_columnCount, pSprite);
            // Code pour tester le nouveau D&D
            Bitmap bmp = new(pSprite.Bounds.Width, pSprite.Bounds.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Magenta);
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //g.SmoothingMode = SmoothingMode.HighQuality;
                pSprite.DrawAtOrigin(g);
            }

            DataObject data = new(new DragHelper.DataObject());

            ShDragImage shdi = new();
            Win32Size size;
            size.cx = bmp.Width;
            size.cy = bmp.Height;
            shdi.sizeDragImage = size;
            Point p = e.Location;
            Win32Point wpt;
            wpt.x = p.X - e.X + pSprite.Bounds.Width / 2;
            wpt.y = p.Y - e.Y + pSprite.Bounds.Height / 2;
            shdi.ptOffset = wpt;
            shdi.hbmpDragImage = bmp.GetHbitmap();
            shdi.crColorKey = Color.Magenta.ToArgb();

            IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();
            sourceHelper.InitializeFromBitmap(ref shdi, data);
            DoDragDrop(data, DragDropEffects.Move);
        }
        //================================================================================================================
        private int va_mouseDownCell; // index de la cellule ou se produit le mouseDown
        private int va_mouseDownColumn; // index de la colonne ou se produit le mouseDown
        private int va_mouseDownRow; // index de la rangée ou se produit le mouseDown
        private Sprite va_mouseDownSprite = null; // référence sur le Sprite ayant reçu le mouseDown
        /// <summary>
        /// Se charge d'un MouseDown sur la grille, cela génère des événements CellMouse...
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //// Code pour tester le nouveau D&D
            //Bitmap bmp = new Bitmap(100, 100, PixelFormat.Format32bppArgb);
            //using (Graphics g = Graphics.FromImage(bmp))
            //{
            //    g.Clear(Color.Magenta);
            //    using (Pen pen = new Pen(Color.Blue, 4f))
            //    {
            //        g.DrawEllipse(pen, 20, 20, 60, 60);
            //        g.DrawEllipse(pen, 10, 10, 80, 80);
            //    }
            //}

            //DataObject data = new DataObject(new DragDropLib.DataObject());

            //ShDragImage shdi = new ShDragImage();
            //Win32Size size;
            //size.cx = bmp.Width;
            //size.cy = bmp.Height;
            //shdi.sizeDragImage = size;
            //Point p = e.Location;
            //Win32Point wpt;
            //wpt.x = p.X - e.X + 50;
            //wpt.y = p.Y - e.Y + 50;
            //shdi.ptOffset = wpt;
            //shdi.hbmpDragImage = bmp.GetHbitmap();
            //shdi.crColorKey = Color.Magenta.ToArgb();

            //IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();
            //sourceHelper.InitializeFromBitmap(ref shdi, data);
            //DoDragDrop(data, DragDropEffects.Copy);


            //return;

            // Début du code pour le Drag
            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            int x = e.X - Padding.Left - va_enteteLgnLarg;
            int y = e.Y - Padding.Top - va_enteteColHaut;

            int colonne = x / largeurCellule;
            int rangee = y / hauteurCellule;

            // le premier niveau de click est le Sprite
            Sprite objSprite = HitSprite(e.X, e.Y);
            if (objSprite != null && objSprite.AcceptClick)
            {
                    va_mouseDownSprite = objSprite;
                    SpriteMouseDown?.Invoke(this, new SpriteMouseEventArgs(e.Button, e.Clicks, x % largeurCellule, y % hauteurCellule, e.Delta,objSprite));

                    if (va_allowDrag && objSprite.AllowDrag)
                    {
                        SpriteDragEventArgs spriteDragInfos = new(this.Name,objSprite, false);
                        BeforeSpriteDrag?.Invoke(this, spriteDragInfos);

                        int index = objSprite.DisplayIndex;
                        if (!spriteDragInfos.Cancel)
                        {
                            va_dragIndexSource = index;
                            va_dragIndex = index;
                            if (va_selectionMode == SelectionMode.One)
                            {
                                int saveIndex = va_selectedIndex;
                                va_selectedIndex = index;
                                UpdateCellAndSprites(index);
                                va_selectedIndex = saveIndex;
                            }
                            //UpdateCellAndSpritesDuringDragPlus(objSprite, x % largeurCellule, y % hauteurCellule, true);
                            DoDragSprite(e, objSprite,index);
                            //DoDragDrop(new DataObject(texteComplet), DragDropEffects.All);
                        }
                    }
            }
            // Si on clique sur une cellule de l'en-tête des rangées
            else if (y < 0 && e.Y >= Padding.Top && x >= 0 && colonne < ColumnCount)
            {
                va_mouseDownColumn = colonne;
                ColumnHeaderMouseDown?.Invoke(this, new ColumnHeaderEventArgs(colonne));
            }
            // Si on clique sur une cellule de l'en-tête des rangées
            else if (x < 0 && e.X >= Padding.Left && y >= 0 && rangee < RowCount)
            {
                va_mouseDownRow = rangee;
                RowHeaderMouseDown?.Invoke(this, new RowHeaderEventArgs(rangee));
            }
            // Si on clique l'extérieur des cellules alors on quitte
            else if (x < 0 || y < 0 || colonne >= ColumnCount || rangee >= RowCount)
            {
                if (va_selectionMode != SelectionMode.None)
                    SelectedIndex = -1;
                base.OnMouseDown(e);
                return;
            }
            else // on clique sur une cellule de la grille
            {
                int index = IndexFromAddress(rangee, colonne);
 
                va_mouseDownCell = index; // nécessaire pour le CellMouseClick

                //-------------------------------------------------------------
                //if (this is GraphVisualArray)
                //{
                //    GraphVisualArray grille = (GraphVisualArray)this;

                //    Rectangle cellBounds = GetCellContentBounds(index / va_columnCount, index % va_columnCount);

                //    if (cellBounds.Width > cellBounds.Height)
                //    { // la barre est horizontale
                //        grille.SetValue(index, e.X - cellBounds.Left);
                //    }
                //    else
                //    { // la barre est verticale
                //        grille.SetValue(index, e.Y - cellBounds.Top);
                //    }
                //}
                //---------------------------------------------------------------

                Address adresse = AddressFromIndex(index);
                Cell cell = va_tabCells[adresse.Row, adresse.Column];
                if (IsOkToSelect(cell, adresse.Row, adresse.Column))
                {
                    CellMouseDown?.Invoke(this, new CellMouseEventArgs(e.Button, e.Clicks, x % largeurCellule, y % hauteurCellule, e.Delta, index, index / va_columnCount, index % va_columnCount));

                    //if (va_allowDrag) Cursor = Cursors.Hand;
                    if (va_selectionMode == SelectionMode.One)
                        SelectedIndex = index;
                    else if (va_selectionMode == SelectionMode.MultiSimple)
                    {
                        // 2 cas possibles, on ajoute à la sélection ou on supprime de la sélection

                        if (!cell.Selected) // Cas 1 : on va ajouter à la sélection
                            SelectedIndex = index;
                        else // Cas 2 : on supprime de la sélection
                        {
                            cell.Selected = false;
                            UpdateCellAndSprites(index);
                            if (index == va_selectedIndex) // 
                            { // il faut rechercher une autre cellule qui sera le SelectedIndex
                                SelectNextCell(); // aucune autre cellule à sélectionner

                                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                            }

                            SelectionChanged?.Invoke(this, EventArgs.Empty);
                        }
                    }

                    Focus();
                    va_currentKeyTime = new DateTime(0);

                    if (va_allowDrag && va_allowCellDrag)
                    {
                        CellDragEventArgs cellDragInfos = new(this.Name, index, new Address(index / va_columnCount, index % va_columnCount), false);
                        BeforeCellDrag?.Invoke(this, cellDragInfos);

                        if (!cellDragInfos.Cancel) // l'opération drag est accepté
                        {
                            va_dragIndexSource = index;
                            va_dragIndex = index;
                            if (va_selectionMode != SelectionMode.None)
                            {
                                int saveIndex = va_selectedIndex;
                                va_selectedIndex = index;
                                if (va_dragAppearance.ShowSource)
                                    UpdateCellAndSpritesDuringDrag(index);
                                va_selectedIndex = saveIndex;
                            }
                            else if (va_allowSelfDrop)
                            {
                                //bool saveState = cell.Selected;
                                va_dragIndex = index;
                                //cell.Selected = true;
                                if (va_dragAppearance.ShowSource)
                                    UpdateCellAndSpritesDuringDrag(index);
                                //cell.Selected = saveState;
                            }
                            //DoDragDrop(new DataObject(texteComplet), DragDropEffects.All);
                            DoDragCell(e, index, rangee,colonne);
                            UpdateCellAndSprites(index); // pour effacer la sélection si le DragDrop n'a pas lieu
                        }
                    }
                }
            }
            base.OnMouseDown(e);
        }
        /// <summary>
        /// Change le va_selectedIndex à la prochaine plus petite valeur pour une cellule sélectionnée
        /// </summary>
        /// <returns></returns>
        private void SelectNextCell()
        {
            for (int row = 0; row < va_rowCount; row++)
                for (int col = 0; col < va_columnCount; col++)
                {
                    Address adrSelonMode = AddressFromAddressMode(row, col);
                    if (va_tabCells[adrSelonMode.Row, adrSelonMode.Column].Selected)
                    {
                        va_selectedIndex = IndexFromAddress(adrSelonMode.Row, adrSelonMode.Column);
                        return;
                    }
                }
            va_selectedIndex = -1; // aucune autre cellule ne peut être sélectionnée
        }

        //protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        //{
        //        // Sets the custom cursor based upon the effect.
        //        e.UseDefaultCursors = false;
        //        if ((e.Effect & DragDropEffects.Move) == DragDropEffects.Move)
        //            Cursor.Current = Cursors.Hand;
        //    //base.OnGiveFeedback(gfbevent);
        //}
        /// <summary>
        /// Se charge d'un MouseUp sur la grille, cela génère des événements CellMouse...
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            int saveMouseDownCell = va_mouseDownCell;
            int saveMouseDownColumn = va_mouseDownColumn;
            int saveMouseDownRow = va_mouseDownRow;
            Sprite saveMouseDownSprite = va_mouseDownSprite;
            va_mouseDownCell = -1;
            va_mouseDownColumn = -1;
            va_mouseDownRow = -1;
            va_mouseDownSprite = null;

            int largeurCellule = m_cellSize.Width + (m_cellMargin << 1) + va_gridAppearance.LineSize;
            int hauteurCellule = m_cellSize.Height + (m_cellMargin << 1) + va_gridAppearance.LineSize;

            int x = e.X - Padding.Left - va_enteteLgnLarg;
            int y = e.Y - Padding.Top - va_enteteColHaut;

            int colonne = x / largeurCellule;
            int rangee = y / hauteurCellule;

            Sprite objSprite = HitSprite(e.X, e.Y);
            if (objSprite != null && objSprite.AcceptClick) // le premier niveau de click est le Sprite
            {
                SpriteMouseUp?.Invoke(this, new SpriteMouseEventArgs(e.Button, e.Clicks, x % largeurCellule, y % hauteurCellule, e.Delta, objSprite));

                if (saveMouseDownSprite == objSprite) // nous avons un SpriteMouseClick
                {
                    va_lastMouseClickSprite = objSprite;
                    SpriteMouseClick?.Invoke(this, new SpriteMouseEventArgs(e.Button, e.Clicks, x % largeurCellule, y % hauteurCellule, e.Delta, objSprite));
                }
            }
            else if (y < 0 && e.Y >= Padding.Top && x >= 0 && colonne < ColumnCount) // Si on relâche la souris sur l'en-tête des colonnes
            {
                ColumnHeaderMouseUp?.Invoke(this, new ColumnHeaderEventArgs(colonne));

                if (saveMouseDownColumn == colonne)
                {
                    va_lastMouseClickColumn = colonne;
                    ColumnHeaderClick?.Invoke(this, new ColumnHeaderEventArgs(colonne));
                }
            }
            // Si on relâche la souris sur l'en-tête des rangées
            else if (x < 0 && e.X >= Padding.Left && y >= 0 && rangee < RowCount)
            {
                RowHeaderMouseUp?.Invoke(this, new RowHeaderEventArgs(rangee));

                if (saveMouseDownRow == rangee)
                {
                    va_lastMouseClickRow = rangee;
                    RowHeaderClick?.Invoke(this, new RowHeaderEventArgs(rangee));
                }
            }
            // Si on relâche à l'extérieur des cellules alors on quitte
            else if (x < 0 || y < 0 || colonne >= ColumnCount || rangee >= RowCount)
            {
                base.OnMouseUp(e);
                return;
            }
            else // autrement on relâche sur une cellule de la grille
            {
                int index = IndexFromAddress(rangee, colonne);
                Cell cell = va_tabCells[rangee, colonne];
                if (CellMouseUp != null && IsOkToSelect(cell, rangee, colonne))
                    CellMouseUp(this, new CellMouseEventArgs(e.Button, e.Clicks, x % largeurCellule, y % hauteurCellule, e.Delta, index, index / va_columnCount, index % va_columnCount));

                if (saveMouseDownCell == index) // nous avons un CellMouseClick
                {
                    va_lastMouseClickCell = index;
                    if (CellMouseClick != null && IsOkToSelect(cell, rangee, colonne))
                        CellMouseClick(this, new CellMouseEventArgs(e.Button, e.Clicks, x % largeurCellule, y % hauteurCellule, e.Delta, index, index / va_columnCount, index % va_columnCount));
                }
            }
            base.OnMouseUp(e);
        } 
        #endregion

        #region DRAG & DROP
        /// -------------------------------------------------------------------------------------
        /// <summary>
        /// Se produit juste avant d'enclencher l'opération glisser-déposer d'un Sprite, permettant de la bloquer.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit juste avant d'enclencher l'opération glisser-déposer d'un Sprite, permettant de la bloquer.")]
        public event EventHandler<SpriteDragEventArgs> BeforeSpriteDrag;

        /// -------------------------------------------------------------------------------------
        /// <summary>
        /// Se produit juste avant d'enclencher l'opération glisser-déposer d'une cellule, permettant de la bloquer.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit juste avant d'enclencher l'opération glisser-déposer d'une cellule, permettant de la bloquer.")]
        public event EventHandler<CellDragEventArgs> BeforeCellDrag;

        /// -------------------------------------------------------------------------------------
        /// <summary>
        /// Se produit lorsqu'un Sprite est glisser-déposer sur la grille.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lorsqu'un Sprite est glisser-déposer sur la grille.")]
        public event EventHandler<SpriteDragDropEventArgs> SpriteDragDrop;

        /// -------------------------------------------------------------------------------------
        /// <summary>
        /// Se produit lorsqu'un Sprite est déposé à l'extérieur des cellules.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lorsqu'un Sprite est déposé à l'extérieur des cellules.")]
        public event EventHandler<SpriteOutsideDropEventArgs> SpriteOutsideDrop;
        /// -------------------------------------------------------------------------------------
        /// <summary>
        /// Se produit lorsqu'un Sprite survol une cellule dans une opération glisser-déposer.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lorsqu'un Sprite survol une cellule dans une opération glisser-déposer.")]
        public event EventHandler<SpriteDragOverEventArgs> SpriteDragOver;        
        /// -------------------------------------------------------------------------------------
        /// <summary>
        /// Se produit lorsqu'une cellule est glissée-déposée sur la grille.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lorsqu'une cellule est glissée-déposée sur la grille.")]
        public event EventHandler<CellDragDropEventArgs> CellDragDrop;

        /// -------------------------------------------------------------------------------------
        /// <summary>
        /// Se produit lorsqu'une cellule est glissée-déposée sur la grille.
        /// </summary>
        [Category("VisualArrays"), Description("Se produit lorsque la cellule destinataire change dans une opération glissée-déposée.")]
        public event EventHandler<CellDragOverEventArgs> CellDragOver;
        
        /// <summary>
        /// Se charge d'une opération "Déposer sur la grille".
        /// </summary>
        /// <param name="drgevent">Informations concernant l'opération</param>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (DesignMode || m_dragInfos == null)
            {
                drgevent.Effect = DragDropEffects.None;
                base.OnDragDrop(drgevent);
                return;
            }

            // DEBUT DRAGDROPLIB -----------------------------------------------------------------------------------------------
            drgevent.Effect = DragDropEffects.Move;
            Point p = Cursor.Position;
            Win32Point wp;
            wp.x = p.X;
            wp.y = p.Y;
            IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
            dropHelper.Drop((ComIDataObject)drgevent.Data, ref wp, (int)drgevent.Effect);
            // FIN DRAGDROPLIB -----------------------------------------------------------------------------------------------

            // Obtenir en coordonnées locales
            Point coordonnee = PointToClient(new Point(drgevent.X, drgevent.Y));
            if (m_dragInfos != null && m_dragInfos.TypeElement == enuTypeElement.Sprite && m_dragInfos.DragSprite.IsMultiCells)
                coordonnee = m_dragInfos.DragSprite.OriginCenter(coordonnee);
            Address adresse = PixelsToAddress(coordonnee.X, coordonnee.Y);

            int index = IndexFromAddress(adresse.Row, adresse.Column);
            bool dragOutside = adresse.Column < 0 || adresse.Column >= ColumnCount || adresse.Row < 0 || adresse.Row >= RowCount;

            if (m_dragInfos == null) return;

            if (dragOutside)
            {
                if (m_dragInfos.DragSprite != null && m_dragInfos.DragSprite.AllowOutsideDrop && m_dragInfos.SourceGridName == this.Name && va_allowSelfDrop)
                {
                    m_dragInfos.DragSprite.Visible = true;
                    SpriteOutsideDrop?.Invoke(this, new SpriteOutsideDropEventArgs(m_dragInfos.SourceGridName, m_dragInfos.DragSprite, m_dragInfos.SourceIndex, new Address(m_dragInfos.SourceRow, m_dragInfos.SourceColumn),coordonnee));

                }
                va_dragIndex = -1;
                va_dragIndexSource = -1;
                m_dragInfos = null; // POUR TERMINER OFFICIELLEMENT L'OPÉRATION GLISSER/DÉPOSER
                return;
            }

            Cell cell = va_tabCells[adresse.Row, adresse.Column];
            if (IsOkToSelect(cell, adresse.Row, adresse.Column)) // l'opération est acceptée seulement sur une cellule visible et active
            {
                //-----------------------------------------------------------------------------------------------
                if (m_dragInfos.SourceGridName != this.Name || (m_dragInfos.SourceGridName == this.Name && va_allowSelfDrop)) // L'opération est acceptée
                {
                    if (va_selectionMode != System.Windows.Forms.SelectionMode.None)
                        SelectedIndex = index;
                    else if (va_selectionMode == System.Windows.Forms.SelectionMode.None)
                    {
                        //UpdateCellAndSprites(index);
                        UpdateCellAndSpritesDuringDragPlus(m_dragInfos.DragSprite, adresse.Row, adresse.Column, false);

                        if (va_dragIndexSource != -1) UpdateCellAndSprites(va_dragIndexSource);
                    }
                    // on va s'assurer avant de notifier l'événement que l'index est différent si nous sommes sur la même grille
                    if ((index != m_dragInfos.SourceIndex && m_dragInfos.SourceGridName == this.Name) || m_dragInfos.SourceGridName != this.Name)
                    {
                        if (CellDragDrop != null && m_dragInfos.TypeElement == enuTypeElement.Cell)
                        {
                            CellDragDrop(this, new CellDragDropEventArgs(m_dragInfos.SourceGridName, m_dragInfos.SourceIndex, new Address(m_dragInfos.SourceRow, m_dragInfos.SourceColumn), index, new Address(index / va_columnCount, index % va_columnCount)));
                        }
                        else if (SpriteDragDrop != null && m_dragInfos.TypeElement == enuTypeElement.Sprite)
                        {
                            SpriteDragDrop(this, new SpriteDragDropEventArgs(m_dragInfos.SourceGridName, m_dragInfos.DragSprite, m_dragInfos.SourceIndex, new Address(m_dragInfos.SourceRow, m_dragInfos.SourceColumn), index, new Address(index / va_columnCount, index % va_columnCount), coordonnee));
                        }
                    }
                }
                va_dragIndex = -1;
                va_dragIndexSource = -1;
                if (m_dragInfos.DragSprite != null) m_dragInfos.DragSprite.Visible = true;
                m_dragInfos = null; // POUR TERMINER OFFICIELLEMENT L'OPÉRATION GLISSER/DÉPOSER
            }
        }

        private int va_dragIndexSource = -1;
        /// <summary>
        /// S'exécute lorsqu'une opération de glisser/déposer entre dans la grille
        /// </summary>
        /// <param name="drgevent">Informations concernant l'opération</param>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (DesignMode || m_dragInfos == null)
            {
                drgevent.Effect = DragDropEffects.None;
                base.OnDragEnter(drgevent);
                return;
            }

            if (m_dragInfos.DragSprite != null)
                m_dragInfos.DragSprite.Visible = false;

            // DEBUT DRAGDROPLIB -----------------------------------------------------------------------------------------------
            drgevent.Effect = DragDropEffects.Move;
            Point p = Cursor.Position;
            Win32Point wp;
            wp.x = p.X;
            wp.y = p.Y;
            IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
            dropHelper.DragEnter(IntPtr.Zero, (ComIDataObject)drgevent.Data, ref wp, (int)drgevent.Effect);
            // FIN DRAGDROPLIB -----------------------------------------------------------------------------------------------


            if (m_dragInfos != null) // MDD
            {
               // string texteComplet = m_dragInfosString; // (string)drgevent.Data.GetData(DataFormats.Text);
                //string[] elements = texteComplet.Split(',');
                //if (elements.Length < 5 || elements.Length > 6 || (elements[0] != "CELL" && elements[0] != "SPRITE"))
                   // drgevent.Effect = DragDropEffects.None;
                // TODO : Ce code provoque un OnDragAndDrop s'il est décommenté ! ce qui n'est pas souhaitable dans le jeu d'associations
                //else
                //    drgevent.Effect = DragDropEffects.Copy;
            }
            //else
            //{
            //    drgevent.Effect = DragDropEffects.None;
            //}

        }
        private int va_dragIndex = -1;
        /// <summary>
        /// S'exécute lorsque l'opération glisser/déposer quitte la grille
        /// </summary>
        /// <param name="e">Informations concernant l'opération</param>
        protected override void OnDragLeave(EventArgs e)
        {
            if (DesignMode || m_dragInfos == null)
            {
                base.OnDragLeave(e);
                return;
            }

            // DEBUT DRAGDROPLIB -----------------------------------------------------------------------------------------------
            IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
            dropHelper.DragLeave();
            // FIN DRAGDROPLIB -----------------------------------------------------------------------------------------------

            if (va_dragIndex != -1 && va_dragIndex != va_dragIndexSource)
                UpdateCellAndSprites(va_dragIndex);
            va_dragIndex = -1;
        }
        //========================================================================================================================
        private static DragAndDropInfos m_dragInfos = null;
        //========================================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (DesignMode || m_dragInfos == null)
            {
                drgevent.Effect = DragDropEffects.None;
                base.OnDragOver(drgevent);
                return;
            }
            // Obtenir en coordonnées locales
            Point coordonnee = PointToClient(new Point(drgevent.X, drgevent.Y));
            Address adresse;
            // On va vérifier si le Drag implique un Sprite multi cellules
            if (m_dragInfos != null && m_dragInfos.TypeElement == enuTypeElement.Sprite && m_dragInfos.DragSprite.IsMultiCells)
                coordonnee = m_dragInfos.DragSprite.OriginCenter(coordonnee);
            adresse = PixelsToAddress(coordonnee.X, coordonnee.Y);

            if (m_dragInfos == null) // MDD
            {
                drgevent.Effect = DragDropEffects.None;
                return;
            }

            if (adresse.Column < 0 || adresse.Column >= ColumnCount || adresse.Row < 0 || adresse.Row >= RowCount || (!va_allowSelfDrop && m_dragInfos.SourceGridName == this.Name))
            {
                if (va_dragIndex != -1)
                    UpdateCellAndSpritesDuringDragPlus(m_dragInfos.DragSprite, va_dragIndex / ColumnCount, va_dragIndex % ColumnCount, false);
                va_dragIndex = -1;
                drgevent.Effect = DragDropEffects.None;
            }
            else
            {
                drgevent.Effect = DragDropEffects.Move;
                int index = IndexFromAddress(adresse.Row, adresse.Column);
                Cell cell = va_tabCells[adresse.Row, adresse.Column];
                bool dragAccepted = true;
                // On drag un Sprite -------------------------------------------------------------------------------------
                if (m_dragInfos.TypeElement == enuTypeElement.Sprite && SpriteDragOver != null)
                {
                    SpriteDragOverEventArgs spriteDragOverEventArgs;
                    spriteDragOverEventArgs = new SpriteDragOverEventArgs(m_dragInfos.SourceGridName, m_dragInfos.DragSprite, m_dragInfos.SourceIndex, new Address(m_dragInfos.SourceRow, m_dragInfos.SourceColumn), index, new Address(index / va_columnCount, index % va_columnCount),coordonnee);
                    SpriteDragOver(this, spriteDragOverEventArgs);
                    dragAccepted = spriteDragOverEventArgs.Accepted;
                }
                // On drag une Cellule ------------------------------------------------------------------------------------
                else if (m_dragInfos.TypeElement == enuTypeElement.Cell && CellDragOver != null)
                {
                    CellDragOverEventArgs cellDragOverEventArgs;
                    cellDragOverEventArgs = new CellDragOverEventArgs(m_dragInfos.SourceGridName, m_dragInfos.SourceIndex, new Address(m_dragInfos.SourceRow, m_dragInfos.SourceColumn), index, new Address(index / va_columnCount, index % va_columnCount));
                    CellDragOver(this, cellDragOverEventArgs);
                    dragAccepted = cellDragOverEventArgs.Accepted;
                }
                // On va mettre en évidence la ou les cellules touchées par le Drag ---------------------------------------
                // On va vérifier la cellule peut recevoir le Drop
                if (IsOkToSelect(cell,adresse.Row, adresse.Column) && dragAccepted)
                {
                    if (va_dragIndex != index)
                    {
                        if (va_dragIndex != -1 && (va_dragIndex != va_dragIndexSource || !va_dragAppearance.ShowSource))
                        {
                            Address adr = IndexToAddress(va_dragIndex);
                            //UpdateCellAndSpritesDuringDragPlus(m_dragInfos.DragSprite, va_dragIndex / ColumnCount, va_dragIndex % ColumnCount, false);
                            UpdateCellAndSpritesDuringDragPlus(m_dragInfos.DragSprite, adr.Row, adr.Column, false);
                        }
                    }
                    va_dragIndex = index;
                    UpdateCellAndSpritesDuringDragPlus(m_dragInfos.DragSprite, adresse.Row, adresse.Column, true);
                }
                else
                {
                    if (va_dragIndex != -1)
                    {
                        Address adr = IndexToAddress(va_dragIndex);
                        UpdateCellAndSpritesDuringDragPlus(m_dragInfos.DragSprite, adr.Row, adr.Column, false);
                        //UpdateCellAndSpritesDuringDragPlus(m_dragInfos.DragSprite, va_dragIndex / ColumnCount, va_dragIndex % ColumnCount, false);
                    }

                    va_dragIndex = -1;
                    drgevent.Effect = DragDropEffects.Move;
                }

            }
            // DEBUT DRAGDROPLIB -----------------------------------------------------------------------------------------------
            drgevent.Effect = DragDropEffects.Move;
            Point p = Cursor.Position;
            Win32Point wp;
            wp.x = p.X;
            wp.y = p.Y;
            IDropTargetHelper dropHelper = (IDropTargetHelper)new DragDropHelper();
            dropHelper.DragOver(ref wp, (int)drgevent.Effect);
            // FIN DRAGDROPLIB -----------------------------------------------------------------------------------------------
        }
        #endregion

        #region MÉTHODES POUR MODIFIER LE BACKGROUND D'UNE CELLULE
        /// <summary>
        /// Assigne une nouvelle visibilité à une cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pVisible">true pour que la cellule soit visible, false autrement</param>
        public void SetCellVisibility(int pIndex, bool pVisible)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].Visible = pVisible;
            UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Assigne une nouvelle visibilité à une cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pVisible">true pour que la cellule soit visible, false autrement</param>
        public void SetCellVisibility(int pRow, int pColumn, bool pVisible)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            va_tabCells[adresse.Row, adresse.Column].Visible = pVisible;
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
        }

        /// <summary>
        /// Désactive la cellule sélectionnée, lève une exception si aucune cellule n'est sélectionnée
        /// </summary>
        public void DisableSelectedCell()
        {
            if (SelectedIndex == -1)
                throw new VisualArrayException("Impossible de désactiver une cellule lorsque SelectedIndex vaut -1");
            Address adresse = IndexToAddress(SelectedIndex);
            va_tabCells[adresse.Row, adresse.Column].Enabled = false;
            UpdateCellAndSprites(SelectedIndex);
            SelectedIndex = -1; // aucune sélection ne devrait exister
        }
        /// <summary>
        /// Rend inactive la cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        public void DisableCell(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].Enabled = false;
            UpdateCellAndSprites(pIndex);
            if (pIndex == SelectedIndex) SelectedIndex = -1; // désélectionner la cellule si elle est la cellule sélectionnée
        }
        /// <summary>
        /// Rend inactive la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        public void DisableCell(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            va_tabCells[adresse.Row, adresse.Column].Enabled = false;
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
            if (SelectedAddress.Row == pRow && SelectedAddress.Column == pColumn) SelectedIndex = -1; // désélectionner la cellule si elle est la cellule sélectionnée
        }
        /// <summary>
        /// Rend active la cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        public void EnableCell(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].Enabled = true;
            UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Rend active la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        public void EnableCell(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            va_tabCells[adresse.Row, adresse.Column].Enabled = true;
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
        }
        /// <summary>
        /// Assigne un nouvel état à la cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pEnabled">true pour que la cellule soit active, false autrement</param>
        public void SetCellEnabled(int pIndex, bool pEnabled)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].Enabled = pEnabled;
            UpdateCellAndSprites(pIndex);
            if (pIndex == SelectedIndex) SelectedIndex = -1; // désélectionner la cellule si elle est la cellule sélectionnée
        }
        /// <summary>
        /// Assigne un nouvel état à la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pEnabled">true pour que la cellule soit active, false autrement</param>
        public void SetCellEnabled(int pRow, int pColumn, bool pEnabled)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            va_tabCells[adresse.Row, adresse.Column].Enabled = pEnabled;
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
            if (SelectedAddress.Row == pRow && SelectedAddress.Column == pColumn) SelectedIndex = -1; // désélectionner la cellule si elle est la cellule sélectionnée
        }

        /// <summary>
        /// Assigne une nouvelle valeur à la propriété ReadOnly d'une cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pReadOnly">true pour que la cellule soit ReadOnly, false autrement</param>
        public void SetCellReadOnly(int pIndex, bool pReadOnly)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].ReadOnly = pReadOnly;
            UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Assigne un nouvel état à la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pReadOnly">true pour que la cellule soit ReadOnly, false autrement</param>
        public void SetCellReadOnly(int pRow, int pColumn, bool pReadOnly)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            va_tabCells[adresse.Row, adresse.Column].ReadOnly = pReadOnly;
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
        }

        /// <summary>
        /// Assigne une nouvelle valeur de zoom à l'élément
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pZoom">Zoom à assigner</param>
        public void SetCellBackgroundZoom(int pIndex, int pZoom)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            objVE.Zoom = pZoom;
            UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Assigne une nouvelle valeur de zoom à l'élément
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pZoom">Zoom à assigner</param>
        public void SetCellBackgroundZoom(int pRow, int pColumn, int pZoom)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            objVE.Zoom = pZoom;
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
        }

        /// <summary>
        /// Assigne (si possible) une nouvelle image de fond à l'élément
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pImage">Image à assigner</param>
        public void SetCellBackgroundImage(int pIndex, Image pImage)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ImageElement)
            {
                ((ImageElement)objVE).Image = pImage;
                UpdateCellAndSprites(pIndex);
            }
            else
                throw new VisualArrayException("Impossible de modifier l'image de l'élément avec son style actuel.");
        }
        /// <summary>
        /// Assigne (si possible) une nouvelle image de fond à l'élément
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pImage">Image à assigner</param>
        public void SetCellBackgroundImage(int pRow, int pColumn, Image pImage)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ImageElement)
            {
                ((ImageElement)objVE).Image = pImage;
                UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
            }
            else
                throw new VisualArrayException("Impossible de modifier l'image de l'élément avec son style actuel.");
        }

        /// <summary>
        /// Assigne (si possible) une nouvelle taille de crayon à l'élément
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pPenWidth">Taille du crayon utilisé pour tracer la forme</param>
        public void SetCellBackgroundPenWidth(int pIndex, int pPenWidth)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
            {
                ((ShapeElement)objVE).PenWidth = pPenWidth;
                UpdateCellAndSprites(pIndex);
            }
            else
                throw new VisualArrayException("Impossible de modifier la taille du crayon de l'élément avec son style actuel.");
        }
        /// <summary>
        /// Assigne (si possible) une nouvelle taille de crayon à l'élément
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pPenWidth">Taille du crayon utilisé pour tracer la forme</param>
        public void SetCellBackgroundPenWidth(int pRow, int pColumn, int pPenWidth)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
            {
                ((ShapeElement)objVE).PenWidth = pPenWidth;
                UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
            }
            else
                throw new VisualArrayException("Impossible de modifier la taille du crayon de l'élément avec son style actuel.");
        }
        /// <summary>
        /// Assigne (si possible) une nouvelle forme de fond à l'élément
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pShape">Forme à assigner</param>
        public void SetCellBackgroundShape(int pIndex, enuShape pShape)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
            {
                ((ShapeElement)objVE).Shape = pShape;
                UpdateCellAndSprites(pIndex);
            }
            else if (objVE is FillShapeElement)
            {
                ((FillShapeElement)objVE).Shape = pShape;
                UpdateCellAndSprites(pIndex);
            }
            else
                throw new VisualArrayException("Impossible de modifier la forme de l'élément avec son style actuel.");
        }
        /// <summary>
        /// Assigne (si possible) une nouvelle couleur de fond à l'élément
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pShape">Forme à assigner</param>
        public void SetCellBackgroundShape(int pRow, int pColumn, enuShape pShape)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
            {
                ((ShapeElement)objVE).Shape = pShape;
                UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
            }
            else if (objVE is FillShapeElement)
            {
                ((FillShapeElement)objVE).Shape = pShape;
                UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
            }
            else
                throw new VisualArrayException("Impossible de modifier la forme de l'élément avec son style actuel.");
        }
        //================
        /// <summary>
        /// Assigne une nouvelle configuration de bordure à la cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pBorder">Taille des bordures de la cellule</param>
        public void SetCellBackgroundBorder(int pIndex, Padding pBorder)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is BorderElement)
                ((BorderElement)objVE).Border = pBorder;
            else
                va_tabCells[adresse.Row, adresse.Column].Background = new BorderElement(pBorder, va_enabledAppearance.BackgroundColor);
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
        }
        /// <summary>
        /// Assigne une nouvelle configuration de bordures à la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pBorder">Taille des bordures de la cellule</param>
        public void SetCellBackgroundBorder(int pRow, int pColumn, Padding pBorder)
        {
            SetCellBackgroundBorder(pRow * va_columnCount + pColumn, pBorder);
        }
        /// <summary>
        /// Assigne (si possible) une nouvelle couleur de fond à l'élément
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pColor">Couleur à assigner</param>
        public void SetCellBackgroundColor(int pIndex, Color pColor)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
            {
                ((ShapeElement)objVE).Color = pColor;
                UpdateCellAndSprites(pIndex);
            }
            else if (objVE is FillShapeElement)
            {
                ((FillShapeElement)objVE).Color = pColor;
                UpdateCellAndSprites(pIndex);
            }
            else if (objVE is BorderElement)
            {
                ((BorderElement)objVE).Color = pColor;
                UpdateCellAndSprites(pIndex);
            }
            else
                throw new VisualArrayException("Impossible de modifier la couleur de l'élément avec son style actuel.");
        }
        /// <summary>
        /// Assigne (si possible) une nouvelle couleur de fond à l'élément
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pColor">Couleur à assigner</param>
        public void SetCellBackgroundColor(int pRow, int pColumn, Color pColor)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
            {
                ((ShapeElement)objVE).Color = pColor;
                UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
            }
            else if (objVE is FillShapeElement)
            {
                ((FillShapeElement)objVE).Color = pColor;
                UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
            }
            else if (objVE is BorderElement)
            {
                ((BorderElement)objVE).Color = pColor;
                int index = IndexFromAddress(adresse.Row, adresse.Column);
                UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
            }
            else
                throw new VisualArrayException("Impossible de modifier la couleur de l'élément avec son style actuel.");
        }

        /// <summary>
        /// Assigne un nouveau VisualElement pour le fond de la cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pVisualElement">VisualElement à assigner</param>
        public void SetCellBackgroundVisualElement(int pIndex, CellVisualElement.CellVisualElement pVisualElement)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].Background = pVisualElement;
            UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Assigne un nouveau VisualElement pour le fond de la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pVisualElement">VisualElement à assigner</param>
        public void SetCellBackgroundVisualElement(int pRow, int pColumn, CellVisualElement.CellVisualElement pVisualElement)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            va_tabCells[adresse.Row, adresse.Column].Background = pVisualElement;
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
        }



        /// <summary>
        /// Assigne un nouveau VisualElement pour le fond de la cellule sous la valeur
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pVisualElement">VisualElement à assigner</param>
        public void SetCellUnderValueVisualElement(int pIndex, CellVisualElement.CellVisualElement pVisualElement)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].LayerUnder = pVisualElement;
            UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Assigne un nouveau VisualElement pour le fond de la cellule sous la valeur
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pVisualElement">VisualElement à assigner</param>
        public void SetCellUnderValueVisualElement(int pRow, int pColumn, CellVisualElement.CellVisualElement pVisualElement)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            va_tabCells[adresse.Row, adresse.Column].LayerUnder = pVisualElement;
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
        }

        /// <summary>
        /// Supprime le 'VisualElement' sous la valeur pour cette cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        public void ClearCellUnderValueVisualElements(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].LayerUnder = null;
            UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Supprime le 'VisualElement' sous la valeur pour cette cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        public void ClearCellUnderValueVisualElements(int pRow, int pColumn)
        {
            ClearCellUnderValueVisualElements(pRow * va_columnCount + pColumn);
        }

        /// <summary>
        /// Assigne un objet de données à une cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pUserData">Objet à assigner</param>
        public void SetCellUserData(int pIndex, Object pUserData)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].UserData = pUserData;
            //UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Assigne un objet de données à une cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pUserData">Objet à assigner</param>
        public void SetCellUserData(int pRow, int pColumn, Object pUserData)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            va_tabCells[adresse.Row, adresse.Column].UserData = pUserData;
            //UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
        }
        /// <summary>
        /// Assigne un objet de données à une cellule se chargeant de dessiner son contenu
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pUserContent">Objet à assigner</param>
        public void SetCellUserContent(int pIndex, ICellDraw pUserContent)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].UserContent = pUserContent;
            UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Assigne un objet de données à une cellule se chargeant de dessiner son contenu
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pUserContent">Objet à assigner</param>
        public void SetCellUserContent(int pRow, int pColumn, ICellDraw pUserContent)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            va_tabCells[adresse.Row, adresse.Column].UserContent = pUserContent;
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
        }
        #endregion

        #region MÉTHODES POUR OBTENIR L'ÉTAT DU BACKGROUND D'UNE CELLULE
        /// <summary>
        /// Obtient (si possible) l'image de fond de la cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>Image de fond de la cellule</returns>
        public Image GetCellBackgroundImage(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ImageElement)
                return ((ImageElement)objVE).Image;
            else
                throw new VisualArrayException("Impossible d'obtenir l'image de l'élément avec la valeur actuelle du CellsBkgStyle.");
        }
        /// <summary>
        /// Obtient (si possible) l'image de fond de la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Image de fond de la cellule</returns>
        public Image GetCellBackgroundImage(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ImageElement)
                return ((ImageElement)objVE).Image;
            else
                throw new VisualArrayException("Impossible d'obtenir l'image de l'élément avec la valeur actuelle du CellsBkgStyle.");
        }

        /// <summary>
        /// Obtient (si possible) la taille du crayon de l'élément
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>Taille du crayon utilisée</returns>
        public int GetCellBackgroundPenWidth(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
                return ((ShapeElement)objVE).PenWidth;
            else
                throw new VisualArrayException("Impossible d'obtenir la taille du crayon de l'élément avec la valeur actuelle du CellsBkgStyle.");
        }
        /// <summary>
        /// Obtient (si possible) la taille du crayon de l'élément
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Taille du crayon utilisée</returns>
        public int GetCellBackgroundPenWidth(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
                return ((ShapeElement)objVE).PenWidth;
            else
                throw new VisualArrayException("Impossible d'obtenir la taille du crayon de l'élément avec la valeur actuelle du CellsBkgStyle.");
        }
        /// <summary>
        /// Obtient (si possible) les bordures de l'élément
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>Bordures utilisées pour dessiner la cellule</returns>
        public Padding GetCellBackgroundBorder(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is BorderElement)
                return ((BorderElement)objVE).Border;
            else
                throw new VisualArrayException("Impossible d'obtenir les bordures de l'élément avec la valeur actuelle du CellsBkgStyle.");
        }
        /// <summary>
        /// Obtient (si possible) les bordures de l'élément
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Bordures utilisées pour dessiner la cellule</returns>
        public Padding GetCellBackgroundBorder(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is BorderElement)
                return ((BorderElement)objVE).Border;
            else
                throw new VisualArrayException("Impossible d'obtenir les bordures de l'élément avec la valeur actuelle du CellsBkgStyle.");
        }
        /// <summary>
        /// Obtient (si possible) la forme de l'élément
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>Forme utilisée pour dessiner le fond de la cellule</returns>
        public enuShape GetCellBackgroundShape(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
                return ((ShapeElement)objVE).Shape;
            else if (objVE is FillShapeElement)
                return ((FillShapeElement)objVE).Shape;
            else
                throw new VisualArrayException("Impossible d'obtenir la forme de l'élément avec la valeur actuelle du CellsBkgStyle.");
        }
        /// <summary>
        /// Obtient (si possible) la forme de l'élément
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Forme utilisée pour dessiner le fond de la cellule</returns>
        public enuShape GetCellBackgroundShape(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
                return ((ShapeElement)objVE).Shape;
            else if (objVE is FillShapeElement)
                return ((FillShapeElement)objVE).Shape;
            else
                throw new VisualArrayException("Impossible d'obtenir la forme de l'élément avec la valeur actuelle du CellsBkgStyle.");
        }
        /// <summary>
        /// Obtient (si possible) la couleur de fond de l'élément
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>Couleur utilisée pour dessiner le fond de la cellule</returns>
        public Color GetCellBackgroundColor(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
                return ((ShapeElement)objVE).Color;
            else if (objVE is FillShapeElement)
                return ((FillShapeElement)objVE).Color;
            else
                throw new VisualArrayException("Impossible d'obtenir la couleur de l'élément avec la valeur actuelle du CellsBkgStyle.");
        }
        /// <summary>
        /// Obtient (si possible) la couleur de fond de l'élément
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Couleur utilisée pour dessiner le fond de la cellule</returns>
        public Color GetCellBackgroundColor(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].Background;
            if (objVE is ShapeElement)
                return ((ShapeElement)objVE).Color;
            else if (objVE is FillShapeElement)
                return ((FillShapeElement)objVE).Color;
            else
                throw new VisualArrayException("Impossible d'obtenir la couleur de l'élément avec la valeur actuelle du CellsBkgStyle.");
        }

        /// <summary>
        /// Obtient le VisualElement utilisé pour dessiner le fond de la cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>VisualElement utilisé pour dessiner le fond de la cellule</returns>
        public CellVisualElement.CellVisualElement GetCellBackgroundVisualElement(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            return va_tabCells[adresse.Row, adresse.Column].Background;
        }
        /// <summary>
        /// Obtient le VisualElement utilisé pour dessiner le fond de la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>VisualElement utilisé pour dessiner le fond de la cellule</returns>
        public CellVisualElement.CellVisualElement GetCellBackgroundVisualElement(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            return va_tabCells[adresse.Row, adresse.Column].Background;
        }

        /// <summary>
        /// Obtient le Zoom utilisé pour dessiner le fond de la cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>Zoom utilisé pour dessiner le fond de la cellule</returns>
        public int GetCellBackgroundZoom(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            return va_tabCells[adresse.Row, adresse.Column].Background.Zoom;
        }
        /// <summary>
        /// Obtient le Zoom utilisé pour dessiner le fond de la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Zoom utilisé pour dessiner le fond de la cellule</returns>
        public int GetCellBackgroundZoom(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            return va_tabCells[adresse.Row, adresse.Column].Background.Zoom;
        }
        /// <summary>
        /// Obtient l'objet de données utilisateur associé à une cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>Objet de données utilisateur associé à la cellule</returns>
        public Object GetCellUserData(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            return va_tabCells[adresse.Row, adresse.Column].UserData;
        }
        /// <summary>
        /// Obtient l'objet de données utilisateur associé à une cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Objet de données utilisateur associé à la cellule</returns>
        public Object GetCellUserData(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            return va_tabCells[adresse.Row, adresse.Column].UserData;
        }
        /// <summary>
        /// Obtient l'objet de données utilisateur associé à une cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>Objet de données utilisateur associé à la cellule</returns>
        public ICellDraw GetCellUserContent(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            return va_tabCells[adresse.Row, adresse.Column].UserContent;
        }
        /// <summary>
        /// Obtient l'objet de données utilisateur associé à une cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Objet de données utilisateur associé à la cellule</returns>
        public ICellDraw GetCellUserContent(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            return va_tabCells[adresse.Row, adresse.Column].UserContent;
        }

        /// <summary>
        /// Obtient l'état d'une cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>État de la cellule</returns>
        public bool GetCellEnabled(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            return va_tabCells[adresse.Row, adresse.Column].Enabled;
        }
        /// <summary>
        /// Obtient l'état d'une cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>État de la cellule</returns>
        public bool GetCellEnabled(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            return va_tabCells[adresse.Row, adresse.Column].Enabled;
        }

        /// <summary>
        /// Obtient l'état lecture seule d'une cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>État de la cellule</returns>
        public bool GetCellReadOnly(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            return va_tabCells[adresse.Row, adresse.Column].ReadOnly;
        }
        /// <summary>
        /// Obtient l'état lecture seule d'une cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>État de la cellule</returns>
        public bool GetCellReadOnly(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            return va_tabCells[adresse.Row, adresse.Column].ReadOnly;
        }

        /// <summary>
        /// Obtient l'état de visibilité d'une cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>État de visibilité de la cellule</returns>
        public bool GetCellVisibility(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            return va_tabCells[adresse.Row, adresse.Column].Visible;
        }
        /// <summary>
        /// Obtient l'état de visibilité d'une cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>État de visibilité de la cellule</returns>
        public bool GetCellVisibility(int pRow, int pColumn)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            return va_tabCells[adresse.Row, adresse.Column].Visible;
        }
        #endregion

        #region MÉTHODES CONCERNANT LES COUCHES DE FOND
        /// <summary>
        /// Supprime tous les 'VisualElement' associés avec la cellule (sauf le background)
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        public void ClearCellVisualElements(int pIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            va_tabCells[adresse.Row, adresse.Column].LayerOver = null;
            UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Supprime tous les 'VisualElement' associés avec la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        public void ClearCellVisualElements(int pRow, int pColumn)
        {
            ClearCellVisualElements(pRow * va_columnCount + pColumn);
        }
        /// <summary>
        /// Ajoute un nouveau VisualElement sur la cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pVisualElement">VisualElement à ajouter</param>
        public void AddCellVisualElement(int pIndex, CellVisualElement.CellVisualElement pVisualElement)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].LayerOver;

            if (objVE == null)
                va_tabCells[adresse.Row, adresse.Column].LayerOver = pVisualElement;
            else
            {
                while (objVE.NextVisualElement != null)
                    objVE = objVE.NextVisualElement;
                objVE.NextVisualElement = pVisualElement;
            }
            UpdateCellAndSprites(pIndex);
        }
        /// <summary>
        /// Ajoute un nouveau VisualElement sur la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pVisualElement">VisualElement à ajouter</param>
        public void AddCellVisualElement(int pRow, int pColumn, CellVisualElement.CellVisualElement pVisualElement)
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].LayerOver;

            if (objVE == null)
                va_tabCells[adresse.Row, adresse.Column].LayerOver = pVisualElement;
            else
            {
                while (objVE.NextVisualElement != null)
                    objVE = objVE.NextVisualElement;
                objVE.NextVisualElement = pVisualElement;
            }
            UpdateCellAndSprites(IndexFromAddress(adresse.Row, adresse.Column));
        }

        /// <summary>
        /// Fournit la liste de tous les 'VisualElement' attachés à cette cellule (sauf le background)
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <returns>Liste des éléments visuels attachés à cette cellule</returns>
        public List<CellVisualElement.CellVisualElement> GetCellVisualElements(int pIndex)
        {
            List<CellVisualElement.CellVisualElement> liste = new List<CellVisualElement.CellVisualElement>();
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].LayerOver;
            while (objVE != null)
            {
                liste.Add(objVE);
                objVE = objVE.NextVisualElement;
            }
            return liste;
        }
        /// <summary>
        /// Fournit la liste de tous les 'VisualElement' attachés à cette cellule (sauf le background)
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <returns>Liste des éléments visuels attachés à cette cellule</returns>
        public List<CellVisualElement.CellVisualElement> GetCellVisualElements(int pRow, int pColumn)
        {
            List<CellVisualElement.CellVisualElement> liste = new List<CellVisualElement.CellVisualElement>();
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].LayerOver;
            while (objVE != null)
            {
                liste.Add(objVE);
                objVE = objVE.NextVisualElement;
            }
            return liste;
        }
        /// <summary>
        /// Supprime le VisualElement à la couche indiquée dans le fond de la cellule
        /// </summary>
        /// <param name="pIndex">Index spécifié en tenant compte du mode d'adressage</param>
        /// <param name="pLayerIndex">Numéro de couche de l'élément à supprimer</param>
        public void RemoveCellVisualElement(int pIndex, int pLayerIndex)
        {
            Address adresse = IndexToAddress(pIndex);
            CellVisualElement.CellVisualElement objVE = va_tabCells[adresse.Row, adresse.Column].LayerOver;

            if (objVE == null || pLayerIndex < 0)
                throw new VisualArrayException("Impossible de supprimer le VisualElement à la couche indiquée, pLayerIndex = " + pLayerIndex);
            else // Il y a au moins un VisualElement sur la cellule
            {
                if (pLayerIndex == 0) // Il faut supprimer le premier élément
                {
                    va_tabCells[adresse.Row, adresse.Column].LayerOver = objVE.NextVisualElement;
                    UpdateCellAndSprites(pIndex);
                }
                else // ce n'est pas le premier à supprimer
                {
                    CellVisualElement.CellVisualElement objPrecedent = null;
                    while (objVE.NextVisualElement != null && pLayerIndex > 0)
                    {
                        objPrecedent = objVE;
                        objVE = objVE.NextVisualElement;
                        pLayerIndex--;
                    }
                    if (pLayerIndex == 0) // on peut le supprimer
                    {
                        objPrecedent.NextVisualElement = objVE.NextVisualElement;
                        UpdateCellAndSprites(pIndex);
                    }
                    else
                        throw new VisualArrayException("Impossible de supprimer le VisualElement à la couche indiquée, pLayerIndex = " + pLayerIndex);
                }
            }
        }
        /// <summary>
        /// Supprime le VisualElement à la couche indiquée dans le fond de la cellule
        /// </summary>
        /// <param name="pRow">Rangée de la cellule.</param>
        /// <param name="pColumn">Colonne de la cellule.</param>
        /// <param name="pLayerIndex">Numéro de couche de l'élément à supprimer</param>
        public void RemoveCellVisualElement(int pRow, int pColumn, int pLayerIndex)
        {
            RemoveCellVisualElement(pRow * va_columnCount + pColumn, pLayerIndex);
        }
        // Il faudra modifier la méthode Clear pour permettre d'effacer tous les éléments visuels d'une cellule.
        // Ajouter une méthode GetCellVisualElements qui retourne un tableau contenant tous les éléments visuels sur une cellule.
        // Ajouter une méthode permettant d'obtenir l'élément visuel sur une certaine couche par exemple : GetCellVisualElement(pIndex,pLayerIndex)
        // Ajouter une méthode pour supprimer un élément visuel sur une certaine couche : RemoveCellVisualElement(pIndex,pLayerIndex)
        #endregion

        #region Méthode pour animer le déplacement d'un Sprite
        //////============================================================================================================
        //// //<summary>
        //// //Permet de dessiner le contenu complet d'une cellule dans la zone offScreen incluant la sélection
        //// //</summary>
        //// //<param name="pAddress"></param>
        //////private void DrawCellOffScreen(Address pAddress)
        //////{
        //////    int index = pAddress.Row * va_columnCount + pAddress.Column;
        //////    DrawCellContent(va_gridOffScreenGraphic, pAddress.Row, pAddress.Column);

        //////    //if (va_selectedIndex == index)
        //////    //    DrawSelection(va_gridOffScreenGraphic, pAddress.Row, pAddress.Column);
        //////}
        ///============================================================================================================
        /// <summary>
        /// Permet d'animer le déplacement d'un Sprite de sa position actuelle vers la position pNewIndex
        /// </summary>
        /// <param name="pobjSprite">Sprite à déplacer</param>
        /// <param name="pNewIndex">Nouvelle position ou déplacement le Sprite</param>
        private void AnimateSprite(Sprite pobjSprite,int pNewIndex)
        {
            if (!va_testMode) // En mode test aucune animation se produit
            {
                int nbPas = pobjSprite.Duration * pobjSprite.FrameRate / 1000;
                int delai = pobjSprite.Duration / nbPas;

                int spriteIndex = pobjSprite.DisplayIndex;

                Address adresseDebut = AddressFromIndex(spriteIndex);
                Address adresseFin = AddressFromIndex(pNewIndex);
                Rectangle rectSpriteDebut = GetCellContentBounds(adresseDebut.Row, adresseDebut.Column);
                Rectangle rectSpriteFin = GetCellContentBounds(adresseFin.Row, adresseFin.Column);

                int distanceTotalX = rectSpriteFin.Left - rectSpriteDebut.Left;
                int distanceTotalY = rectSpriteFin.Top - rectSpriteDebut.Top;

                bool saveState = pobjSprite.m_visible;
                pobjSprite.m_visible = false;
                Rectangle oldPsritePos = Rectangle.Empty;

                for (int pas = 0; pas <= nbPas; pas++)
                {
                    Rectangle spriteAnimPos = rectSpriteDebut;
                    // calcul la position du Sprite en tenant compte du pas actuel
                    spriteAnimPos.Offset(distanceTotalX * pas / nbPas, distanceTotalY * pas / nbPas);

                    pobjSprite.MoveTo(new Point(spriteAnimPos.X, spriteAnimPos.Y));
                    //---------------------------------------------------------------------------------
                    DateTime maintenantPlusDelai = DateTime.Now.AddMilliseconds(delai);
                    while (DateTime.Now < maintenantPlusDelai) ;
                }
            }
        }
        #endregion

        #region Indexeur possible pour accéder directement aux cellules
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="pAddress"></param>
        ///// <returns></returns>
        //public Cell this[Address pAddress]
        //{
        //    get { return new Cell(); }
        //    set { }
        //}
        #endregion

        #region IsValidAddress
        /// <summary>
        /// Indique l'adresse pAddress est dans les limites de la grille
        /// </summary>
        /// <param name="pAddress">Adresse à vérifier</param>
        /// <returns>true si l'adresse est dans les limites de la grille</returns>
        public bool IsValidAddress(Address pAddress)
        {
            return pAddress.Column >= 0 && pAddress.Column < ColumnCount &&
                pAddress.Row >= 0 && pAddress.Row < RowCount;
        }
        #endregion
    }

    #region Classe interne GrilleDeBaseDesigner
    internal class GrilleDeBaseDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        public GrilleDeBaseDesigner()
            : base()
        {
        }

        public override SelectionRules SelectionRules
        {
            get
            {
                SelectionRules rules;
                rules = base.SelectionRules;
                BaseGrid objControle = (BaseGrid)Control;

                //if (objControle.ResizeMode == enuResizeMode. || objControle.ResizeMode == enuResizeMode.Colonne)
                //{
                //    rules = SelectionRules.Moveable | SelectionRules.Visible
                //        | SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
                //}
                //else if (objControle.ResizeMode == enuResizeMode.Vertical || objControle.ResizeMode == enuResizeMode.Ligne)
                //{
                //    rules = SelectionRules.Moveable | SelectionRules.Visible
                //        | SelectionRules.TopSizeable | SelectionRules.BottomSizeable;
                //}
                //else
                //{
                rules = SelectionRules.Moveable | SelectionRules.Visible | SelectionRules.AllSizeable;
                //}
                return rules;
            }
        }
    }
    #endregion
}

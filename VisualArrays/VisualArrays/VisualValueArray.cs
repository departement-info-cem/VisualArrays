using System.ComponentModel;
using System.Text;
using VisualArrays.Appearance;
using VisualArrays.CellVisualElement;
using VisualArrays.Others;

namespace VisualArrays.VisualArrays;

/// <summary>
/// Cette classe représente les propriétés nécessaire pour afficher des graphiques
/// </summary>
public abstract partial class VisualValueArray<Type> : BaseGrid
{
    #region Tableau de cellules
    //-----------------------------------------------------------------------------
    /// <summary>
    /// Tableau à 2 dimensions des cellules de la grille.
    /// </summary>
    protected Type[,] va_tabValues = null;
    #endregion

    #region DefaultValue et SpecialValue
    /// <summary>
    /// Valeur par défaut qui sera assignée à toutes les cellules
    /// </summary>
    protected Type m_defaultValue;
    /// <summary>
    /// Une valeur spéciale à afficher différemment des autres voir SpecialValueAppearance
    /// </summary>
    protected Type m_specialValue;

    #endregion

    #region SelectedValue
    //============================================================================================
    /// <summary>
    /// Obtient la valeur de la cellule sélectionnée
    /// </summary>
    [Browsable(false)]
    public Type SelectedValue
    {
        get
        {
            if (SelectedIndex == -1)
                throw new VisualArrayException("Impossible d'obtenir la valeur sélectionnée lorsque SelectedIndex vaut -1");
            return this[SelectedIndex];
        }

        //set
        //{
        //    if (SelectedIndex == -1)
        //        throw new VisualArrayException("Impossible de modifier la valeur sélectionnée lorsque SelectedIndex vaut -1");
        //    this[SelectedIndex] = value;
        //}
    }
    #endregion

    #region ContainsValue
    /// <summary>
    /// Détermine si pValue se retrouve dans l'une des cellules de la grille
    /// </summary>
    /// <param name="pValue"></param>
    /// <returns>Vrai si pValue se retrouve dans l'une des cellules de la grille</returns>
    public bool ContainsValue(Type pValue)
    {
        for (int ligne = 0; ligne < va_tabValues.GetLength(0); ligne++)
        for (int colonne = 0; colonne < va_tabValues.GetLength(1); colonne++)
            if (va_tabValues[ligne, colonne].Equals(pValue))
                return true;
        return false;
    }
    #endregion

    #region IsOkToSelect
    /// <summary>
    /// Indique si la cellule peut être sélectionnée
    /// </summary>
    /// <param name="pCell">Cellule à sélectionner</param>
    /// <param name="pRow">Rangée de la cellule</param>
    /// <param name="pColumn">Colonne de la cellule</param>
    /// <returns></returns>
    public override bool IsOkToSelect(Cell pCell, int pRow, int pColumn)
    {
        if (va_tabValues[pRow, pColumn] != null && va_tabValues[pRow, pColumn].Equals(m_specialValue) && !va_specialValueAppearance.Enabled)
            return false;
        return base.IsOkToSelect(pCell, pRow, pColumn);
    }
    #endregion

    #region ResizeValueArray,ResetAllValuesToDefault
    //============================================================================================
    /// <summary>
    /// Modifie le nombre de cellules dans le tableau à partir des propriétés RowCount et ColumnCount.
    /// </summary>
    protected override void ResizeValueArray()
    {
        va_tabValues = new Type[RowCount, ColumnCount];
    }
    //============================================================================================
    /// <summary>
    /// Initialise toutes les cellules au caractère 0
    /// </summary>
    protected override void ResetAllValuesToDefault()
    {
        for (int ligne = 0; ligne < va_tabValues.GetLength(0); ligne++)
        for (int colonne = 0; colonne < va_tabValues.GetLength(1); colonne++)
            va_tabValues[ligne, colonne] = m_defaultValue;
        SendValueChanged(-1,-1,-1,Address.Empty);
    }
    #endregion

    #region Swap et MixUp
    /// <summary>
    /// Permute (échange) deux cellules.
    /// </summary>
    /// <param name="pIndex1">Index de la première cellule impliquée dans la permutation</param>
    /// <param name="pIndex2">Index de la deuxième cellule impliquée dans la permutation</param>
    public override void Swap(int pIndex1, int pIndex2)
    {
        SelectedIndex = -1; // on va détruire toute sélection avant d'effectuer la permutation
        Address adresse1 = IndexToAddress(pIndex1);
        Address adresse2 = IndexToAddress(pIndex2);
        if (pIndex1 != pIndex2)
        {
            Cell temp = va_tabCells[adresse1.Row, adresse1.Column];
            va_tabCells[adresse1.Row, adresse1.Column] = va_tabCells[adresse2.Row, adresse2.Column];
            va_tabCells[adresse2.Row, adresse2.Column] = temp;
            SwapValues(adresse1, adresse2); //échange les valeurs des 2 cellules
            UpdateCellAndSprites(pIndex1);
            UpdateCellAndSprites(pIndex2);
        }
    }

    #region SwapValues
    /// <summary>
    /// Permute (échange) uniquement les valeurs de deux cellules.
    /// </summary>
    /// <param name="pAddress1">Adresse de la première cellule impliquée dans la permutation</param>
    /// <param name="pAddress2">Adresse de la deuxième cellule impliquée dans la permutation</param>
    protected void SwapValues(Address pAddress1, Address pAddress2)
    {
        Type temp = va_tabValues[pAddress1.Row, pAddress1.Column];
        va_tabValues[pAddress1.Row, pAddress1.Column] = va_tabValues[pAddress2.Row, pAddress2.Column];
        va_tabValues[pAddress2.Row, pAddress2.Column] = temp;
    }
    #endregion


    /// <summary>
    /// Mélange l'ensemble des cellules de la grille de façon aléatoire en effectuant des permutations
    /// Le nombre de permutations est Length * 2
    /// </summary>
    public void MixUp()
    {
        MixUp(Length * 2);
    }
    /// <summary>
    /// Mélange l'ensemble des cellules de la grille de façon aléatoire en effectuant des permutations
    /// </summary>
    /// <param name="pSwapCount">Nombre de permutations à effectuer pour mélanger les cellules de la grille.</param>
    public void MixUp(int pSwapCount)
    {
        if (Length < 2) return; //  Ne rien faire car seulement une seule cellule
        SelectedIndex = -1; // on va détruire toute sélection avant d'effectuer les permutations
        BeginUpdate();
        Address adresse1,adresse2;
        for (int permutation = 0; permutation < pSwapCount; permutation++)
        {
            do
            {
                adresse1 = RandomAddress();
                adresse2 = RandomAddress();
            } while (adresse1 == adresse2);

            Cell temp = va_tabCells[adresse1.Row, adresse1.Column];
            va_tabCells[adresse1.Row, adresse1.Column] = va_tabCells[adresse2.Row, adresse2.Column];
            va_tabCells[adresse2.Row, adresse2.Column] = temp;
            SwapValues(adresse1, adresse2); //échange les valeurs des 2 cellules
        }
        EndUpdate();
    }

    #endregion

    #region Événements

    ///// <summary>
    ///// Lance l'événement BeforeValueChanged et récolte un booléen permettant d'annuler la modification
    ///// </summary>
    ///// <returns></returns>
    ////protected bool AcceptBeforeValueChanged()
    ////{
    ////    if (BeforeValueChanged != null)
    ////    {
    ////        BeforeValueChangedArgs beforeValueChangedArgs = new BeforeValueChangedArgs(true);
    ////        BeforeValueChanged(this, beforeValueChangedArgs);
    ////        return beforeValueChangedArgs.AcceptValueChanged;
    ////    }
    ////    return true;
    ////}

    #endregion

    #region ValueChanged
    /// <summary>
    /// Reprend la peinture de la grille après qu'elle a été interrompue par la méthode BeginUpdate.
    /// </summary>
    public override void EndUpdate()
    {
        base.EndUpdate();
        SendValueChanged(-1,-1,-1,Address.Empty);
    }
    /// <summary>
    /// Déclenche l'événement ValueChanged.
    /// </summary>
    /// <param name="pIndex">Index de la cellule où se produit l'événement</param>
    /// <param name="pRow">Rangée de la cellule où se produit l'événement</param>
    /// <param name="pColumn">Colonne de la cellule où se produit l'événement</param>
    /// <param name="pAddress">Adresse de la cellule où se produit l'événement</param>
    protected void SendValueChanged(int pIndex,int pRow,int pColumn,Address pAddress)
    {
        if (ValueChanged != null && !va_isUpdating)
            ValueChanged(this, new ValueChangedEventArgs(pIndex,pRow,pColumn,pAddress));
    }
    /// <summary>
    /// Se produit lorsque la valeur d'une ou plusieurs cellules changent.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    [Category("VisualArrays"), Description("Se produit lorsque la valeur d'une cellule change")]
    public event EventHandler<ValueChangedEventArgs> ValueChanged;

    #endregion

    #region DisabledAppearance
    /// <summary>
    /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est false
    /// </summary>
    protected new VGDisabledAppearance<Type> va_disabledAppearance;
    /// <summary>
    /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est false
    /// </summary>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("CellAppearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public new VGDisabledAppearance<Type> DisabledAppearance
    {
        get => va_disabledAppearance;
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
        switch (pStyle)
        {
            case enuBkgStyle.None:
                va_disabledVisualElement = null;
                break;
            case enuBkgStyle.Border:
                va_disabledVisualElement = new BorderElement(va_disabledAppearance.Border, va_disabledAppearance.BackgroundColor);
                break;
            case enuBkgStyle.FillShape:
                va_disabledVisualElement = new FillShapeElement(va_disabledAppearance.Shape, va_disabledAppearance.BackgroundColor);
                break;
            case enuBkgStyle.Shape:
                va_disabledVisualElement = new ShapeElement(va_disabledAppearance.Shape, va_disabledAppearance.PenWidth, va_disabledAppearance.BackgroundColor,va_disabledAppearance.Radius);
                break;
            case enuBkgStyle.Image:
                va_disabledVisualElement = new ImageElement(va_disabledAppearance.Image);
                break;
        }
    }
    #endregion

    #region CellAppearance
    /// <summary>
    /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est true
    /// </summary>
    protected new VGCellAppearance<Type> va_enabledAppearance;
    /// <summary>
    /// Détermine différents aspects de l'apparence d'une cellule dont l'état Enabled est true
    /// </summary>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("CellAppearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public new VGCellAppearance<Type> EnabledAppearance
    {
        get => va_enabledAppearance;
        set
        {
            va_enabledAppearance = value;
            UpdateCellsBkgVisualElement();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal override void UpdateCellsBkgVisualElement()
    {
        UpdateCellsBkgVisualElement(va_enabledAppearance);
    }
    #endregion

    #region SpecialValueAppearance
    /// <summary>
    /// Détermine différents aspects de l'apparence de la valeur spéciale
    /// </summary>
    protected SpecialValueAppearance<Type> va_specialValueAppearance;
    /// <summary>
    /// Détermine différents aspects de l'apparence de la valeur spéciale
    /// </summary>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("CellAppearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public SpecialValueAppearance<Type> SpecialValueAppearance
    {
        get => va_specialValueAppearance;
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

    //============================================================================================
    internal virtual void UpdateSpecialValueVisualElement(enuBkgStyle pStyle)
    {
        switch (pStyle)
        {
            case enuBkgStyle.None:
                va_specialValueVisualElement = null;
                break;
            case enuBkgStyle.Border:
                va_specialValueVisualElement = new BorderElement(va_specialValueAppearance.Border, va_specialValueAppearance.BackgroundColor);
                break;
            case enuBkgStyle.FillShape:
                va_specialValueVisualElement = new FillShapeElement(va_specialValueAppearance.Shape, va_specialValueAppearance.BackgroundColor, va_specialValueAppearance.Radius);
                break;
            case enuBkgStyle.Shape:
                va_specialValueVisualElement = new ShapeElement(va_specialValueAppearance.Shape, va_specialValueAppearance.PenWidth, va_specialValueAppearance.BackgroundColor,va_specialValueAppearance.Radius);
                break;
            case enuBkgStyle.Image:
                va_specialValueVisualElement = new ImageElement(va_specialValueAppearance.Image);
                break;
        }
    }
    //----------------------------------------------------------------------------------------
    /// <summary>
    /// Élément visuel affiché dans le fond des cellules lorsqu'il s'agit de la valeur spéciale.
    /// </summary>
    protected internal CellVisualElement.CellVisualElement va_specialValueVisualElement;
    #endregion

    #region Redéfinir la propriété BackColor héritée de Control
    ///// <summary>
    ///// Obtient ou définit la couleur de fond de la surface complète de la grille
    ///// </summary>
    //[DefaultValue(typeof(Color), "Black")]
    //public override Color BackColor
    //{
    //    get
    //    {
    //        return base.BackColor;
    //    }
    //    set
    //    {
    //        base.BackColor = value;
    //    }
    //}
    #endregion

    #region Constructeur
    //============================================================================================
    /// <summary>
    /// Initialise les champs concernant la partie graphique.
    /// </summary>
    public VisualValueArray()
    {
        BackColor = Color.Black;

        va_enabledAppearance = new VGCellAppearance<Type>(this);
        UpdateCellsBkgVisualElement();
        //--------------------------------------------------------------
        va_disabledAppearance = new VGDisabledAppearance<Type>(this);
        va_disabledVisualElement = null;
        //--------------------------------------------------------------
        va_specialValueAppearance = new SpecialValueAppearance<Type>(this);
        va_specialValueVisualElement = null;

        //ColumnCount = 4;

        ResizeValueArray();

        InitializeComponent();
    }

    #endregion

    #region Événement BeforeValueChanged

    /// <summary>
    /// Se produit juste avant le changement de valeur d'une cellule.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    [Category("VisualArrays"), Description("Se produit juste avant le changement de valeur d'une cellule")]
    public event EventHandler<BeforeValueChangedArgs<Type>> BeforeValueChanged;

    /// <summary>
    /// Lance l'événement BeforeValueChanged et récolte un booléen permettant d'annuler la modification
    /// </summary>
    /// <returns></returns>
    protected BeforeValueChangedArgs<Type> AcceptBeforeValueChanged(Type pNewValue)
    {
        BeforeValueChangedArgs<Type> beforeValueChangedArgs = new BeforeValueChangedArgs<Type>(true, pNewValue);
        if (BeforeValueChanged != null)
        {
            BeforeValueChanged(this, beforeValueChangedArgs);
            return beforeValueChangedArgs;
        }
        return beforeValueChangedArgs;
    }

    #endregion

    #region Indexeurs

    /// <summary>
    /// Obtient ou définit la valeur à l'index spécifié en tenant compte du ModeAdressage.
    /// </summary>
    /// <param name="pIndex">index du digit</param>
    /// <returns>valeur à l'index</returns>
    public virtual Type this[int pIndex]
    {
        get
        {
            Address adresse = IndexToAddress(pIndex);
            return va_tabValues[adresse.Row, adresse.Column];
        }
        set
        {
            BeforeValueChangedArgs<Type> beforeValueChangedArgs = AcceptBeforeValueChanged(value);
            if (!beforeValueChangedArgs.AcceptValueChanged) return;
            value = beforeValueChangedArgs.NewValue;

            Address adresse = IndexToAddress(pIndex);
            if (!value.Equals(va_tabValues[adresse.Row, adresse.Column])) // 2014-11-10
            {
                va_tabValues[adresse.Row, adresse.Column] = value;
                UpdateCellAndSprites(pIndex);
                adresse = AddressFromAddressMode(adresse.Row, adresse.Column);
                SendValueChanged(pIndex, adresse.Row, adresse.Column, adresse);
            }
        }
    }
    /// <summary>
    /// Obtient ou définit la valeur d'une cellule en tenant compte du mode d'adressage.
    /// </summary>
    /// <param name="pRow">La rangée de la cellule. </param>
    /// <param name="pColumn">La colonne de la cellule.</param>
    /// <returns>Le valeur à la coordonnée spécifiée.</returns>
    public virtual Type this[int pRow, int pColumn]
    {
        get
        {
            Address adresse = AddressFromAddressMode(pRow, pColumn);
            return va_tabValues[adresse.Row, adresse.Column];
        }
        set
        {
            BeforeValueChangedArgs<Type> beforeValueChangedArgs = AcceptBeforeValueChanged(value);
            if (!beforeValueChangedArgs.AcceptValueChanged) return;
            value = beforeValueChangedArgs.NewValue;

            Address adresse = AddressFromAddressMode(pRow, pColumn);
            if (!value.Equals(va_tabValues[adresse.Row, adresse.Column])) // 2014-11-10
            {
                va_tabValues[adresse.Row, adresse.Column] = value;
                int index = IndexFromAddress(adresse.Row, adresse.Column);
                UpdateCellAndSprites(index);
                SendValueChanged(index, pRow, pColumn, new Address(pRow, pColumn));
            }
        }
    }
    /// <summary>
    /// Obtient ou définit la valeur pour la cellule à une adresse donnée
    /// </summary>
    /// <param name="pAddress">Adressse de la cellule à manipuler</param>
    /// <returns>valeur contenue dans la cellule</returns>
    public Type this[Address pAddress]
    {
        get => this[pAddress.Row, pAddress.Column];
        set => this[pAddress.Row, pAddress.Column] = value;
    }
    #endregion

    #region Itérateur
    //========================================================================================
    /// <summary>
    /// Permet de parcourir séquentiellement toutes les cellules de la grille.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Type> GetEnumerator()
    {
        for (int index = 0; index < Length; index++)
            yield return this[index];
    }
    #endregion

    #region ToArray et To2DArray
    //========================================================================================
    /// <summary>
    ///     Copie les valeurs du VisualBoolArray vers un nouveau tableau à une dimension.
    /// </summary>
    /// <returns>Tableau à une dimension contenant les copies des valeurs du VisualBoolArray</returns>
    public Type[] ToArray()
    {
        Type[] tabElements = new Type[Length];
        for (int index = 0; index < Length; index++)
            tabElements[index] = this[index];
        return tabElements;
    }
    //========================================================================================
    /// <summary>
    ///     Copie les valeurs du VisualBoolArray vers un nouveau tableau à 2 dimensions.
    /// </summary>
    /// <returns>Tableau à 2 dimensions contenant les copies des valeurs du VisualBoolArray</returns>
    public Type[,] To2DArray()
    {
        Type[,] tabElements = va_tabValues;
        Array.Copy(va_tabValues, tabElements, va_tabValues.Length);
        return tabElements;
    }
    //========================================================================================
    /// <summary>
    ///     Trie en ordre croissant les valeurs des cellules de la grille.
    /// </summary>
    public void Sort()
    {
        BeginUpdate();
        Type[] tabElements = ToArray();
        Array.Sort(tabElements);
        for (int index = 0; index < Length; index++)
            this[index] = tabElements[index];
        EndUpdate();
    }
    #endregion

    #region ToString
    /// <summary>
    /// Fournit les valeurs de la grille sous la forme d'une chaîne avec les indices soit en 1D ou 2D
    /// selon la valeur de la propriété AddressView.
    /// </summary>
    /// <returns>Liste des valeurs de la grille</returns>
    public override string ToString()
    {
        StringBuilder objChaine = new();
        if (va_addressView == enuAddressView.Mode1D || va_addressView == enuAddressView.None)
        {
            for (int index = 0; index < Length; index++)
                objChaine.Append(" [" + index + "]=" + this[index] + " ,");
            objChaine.Remove(objChaine.Length - 1, 1);
        }
        else
        {
            for (int row = 0; row < RowCount; row++)
            for (int col = 0; col < ColumnCount; col++)
                objChaine.Append(" [" + row + "," + col + "]=" + this[row, col] + " ,");
            objChaine.Remove(objChaine.Length - 1, 1);
        }

        return objChaine.ToString();
    }
    #endregion

    #region Equals (NON ACTIF)
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="obj"></param>
    ///// <returns></returns>
    //public override bool Equals(object obj)
    //{
    //    //Check for null and compare run-time types.
    //    if (obj == null || GetType() != obj.GetType()) return false;
    //    VisualStringArray autreGrille = (VisualStringArray)obj;
    //    if (autreGrille.ColumnCount != ColumnCount || autreGrille.RowCount != RowCount) return false;
    //    for (int row = 0; row < RowCount; row++)
    //        for (int col = 0; col < ColumnCount; col++)
    //            if (this[row, col] != autreGrille[row, col]) return false;
    //    return base.Equals(obj);
    //}
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <returns></returns>
    //public override int GetHashCode()
    //{
    //    return base.GetHashCode();
    //}
    #endregion

    #region Clear()
    // CLEAR ------------------------------------------------------------------------------
    /// <summary>
    /// Vide le contenu de la cellule dont l'index est fourni en paramètre.
    /// </summary>
    /// <param name="pIndex">index de la cellule à vider</param>
    /// ----------------------------------------------------------------------------------
    public override void Clear(int pIndex)
    {
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
        this[pRow, pColumn] = m_defaultValue;
    }
    #endregion
}
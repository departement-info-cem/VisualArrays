using System.ComponentModel;
using VisualArrays.Others;
using VisualArrays.VisualCells;

namespace VisualArrays.VisualContainer;

/// <summary>
/// Représente un conteneur abstrait de VisualCell
/// </summary>
public abstract partial class BaseVisualContainer<BaseType,VisualType> : ContainerControl, IVisualArray<CellMouseEventArgs1D>
    where BaseType: struct 
    where VisualType : VisualValue<BaseType>
{

    #region Données membres
    /// <summary>
    /// Liste des VisualValue contenu dans le conteneur.
    /// </summary>
    private List<VisualValue<BaseType>> m_colVisualElements = new();
    #endregion

    //#region Sprites
    ////===========================================================================================
    //private SpriteCollection va_sprites = null;
    ///// <summary>
    ///// Obtient la collection des objets Sprite qui s'affiche sur la grille.
    ///// </summary>
    //[Description("Représente la collection des Sprites appartenant à cette grille")]
    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    //[Category("VisualArrays")]
    //[EditorAttribute(typeof(SpriteCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    //public SpriteCollection Sprites
    //{
    //    get { return va_sprites; }
    //}
    //#endregion

    #region Propriétés : ShowIndices, Length
    //========================================================================================
    private bool m_showIndices = true;
    /// <summary>
    /// Détermine si les VisualInt doivent afficher leur Index en mode conception
    /// </summary>
    [DefaultValue(true), Category("VisualArrays"), Description("Détermine si les VisualInt doivent afficher leur Index en mode conception")]
    public bool ShowIndices
    {
        get => m_showIndices;
        set
        {
            m_showIndices = value;
            RenumeroterLesIndex();
        }
    }
    //========================================================================================
    /// <summary>
    /// Nombre de VisualValue dans le conteneur.
    /// </summary>
    public int Length => m_colVisualElements.Count;

    #endregion

    #region Constructeur
    //========================================================================================
    /// <summary>
    /// Initialise un BaseVisualContainer
    /// </summary>
    protected BaseVisualContainer()
    {
        InitializeComponent();
    }
    #endregion

    #region ÉVÉNEMENTS avec CellMouseEventArgs
    /// <summary>
    /// Se produit lors d'un MouseDown dans une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    [Category("VisualArrays"), Description("Se produit lors d'un MouseDown dans une cellule de la grille.")]
    public event EventHandler<CellMouseEventArgs1D> CellMouseDown;

    /// <summary>
    /// Se produit lors d'un MouseDown dans une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    [Category("VisualArrays"), Description("Se produit lors d'un MouseUp dans une cellule de la grille.")]
    public event EventHandler<CellMouseEventArgs1D> CellMouseUp;

    /// <summary>
    /// Se produit lors d'un MouseClick dans une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    [Category("VisualArrays"), Description("Se produit lors d'un MouseClick dans une cellule de la grille.")]
    public event EventHandler<CellMouseEventArgs1D> CellMouseClick;

    /// <summary>
    /// Se produit lors d'un MouseDoubleClick dans une cellule de la grille.
    /// </summary>
    /// -------------------------------------------------------------------------------------
    [Category("VisualArrays"), Description("Se produit lors d'un MouseDoubleClick dans une cellule de la grille.")]
    public event EventHandler<CellMouseEventArgs1D> CellMouseDoubleClick;

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
    public event EventHandler<CellMouseEventArgs1D> CellMouseMove;

    #endregion

    #region Événements
    /// <summary>
    /// Se produit lorsque la valeur d'un contrôle contenu change.
    /// </summary>
    [Description("Se produit lorsque la valeur d'un contrôle contenu change.")]
    public event EventHandler<CellVCEventArgs> ValueChanged;

    #endregion

    #region OnControlAdded, OnControlRemoved
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnControlAdded(ControlEventArgs e)
    {
        if (e.Control is VisualValue<BaseType>)
        {
            VisualValue<BaseType> cell = (VisualValue<BaseType>)e.Control;
            cell.MouseClick += new MouseEventHandler(cell_MouseClick);
            cell.MouseDoubleClick += new MouseEventHandler(cell_MouseDoubleClick);
            cell.MouseDown += new MouseEventHandler(cell_MouseDown);
            cell.MouseUp += new MouseEventHandler(cell_MouseUp);
            cell.MouseLeave += new EventHandler(cell_MouseLeave);
            cell.MouseEnter += new EventHandler(cell_MouseEnter);
            cell.MouseHover += new EventHandler(cell_MouseHover);
            cell.MouseMove += new MouseEventHandler(cell_MouseMove);
            cell.TabIndexChanged += new EventHandler(cell_TabIndexChanged);
            cell.ValueChanged += new EventHandler(cell_ValueChanged);
            m_colVisualElements.Add(cell);
            RenumeroterLesIndex();
        }
    }
    //========================================================================================================================
    private void cell_TabIndexChanged(object sender, EventArgs e)
    {
        RenumeroterLesIndex();
    }
    //========================================================================================================================
    private void cell_ValueChanged(object sender, EventArgs e)
    {
        ValueChanged?.Invoke(this, new CellVCEventArgs(((VisualValue<BaseType>)sender).Index));
    }
    //========================================================================================================================
    /// <summary>
    /// Permet de trier les éléments et de fixer leur index séquentiellement à partir de 0.
    /// </summary>
    protected void RenumeroterLesIndex()
    {
        m_colVisualElements.Sort();
        for (int index = 0; index < m_colVisualElements.Count; index++)
        {
            VisualValue<BaseType> vi = m_colVisualElements[index];
            vi.ShowIndex = ShowIndices && DesignMode;
            vi.Index = index;
        }
    }
    //========================================================================================================================
    /// <summary>
    /// Se produit lorsqu'un contrôle est retiré du conteneur
    /// </summary>
    /// <param name="e"></param>
    protected override void OnControlRemoved(ControlEventArgs e)
    {
        if (e.Control is VisualValue<BaseType>)
        {
            VisualValue<BaseType> cell = (VisualValue<BaseType>)e.Control;
            cell.MouseClick -= cell_MouseClick;
            cell.MouseDoubleClick -= cell_MouseDoubleClick;
            cell.MouseDown -= cell_MouseDown;
            cell.MouseUp -= cell_MouseUp;
            cell.MouseLeave -= cell_MouseLeave;
            cell.MouseEnter -= cell_MouseEnter;
            cell.MouseHover -= cell_MouseHover;
            cell.MouseMove -= cell_MouseMove;
            cell.TabIndexChanged -= new EventHandler(cell_TabIndexChanged);
            cell.ValueChanged -= cell_ValueChanged;
            cell.ShowIndex = false;
            m_colVisualElements.Remove(cell);
            RenumeroterLesIndex();
        }
    }

    //========================================================================================================
    private void cell_MouseMove(object sender, MouseEventArgs e)
    {
        CellMouseMove?.Invoke(sender, new CellMouseEventArgs1D(e.Button, e.Clicks, e.X, e.Y, e.Delta, ((VisualValue<BaseType>)sender).Index));
    }
    //========================================================================================================
    private void cell_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        CellMouseDoubleClick?.Invoke(sender, new CellMouseEventArgs1D(e.Button, e.Clicks, e.X, e.Y, e.Delta, ((VisualValue<BaseType>)sender).Index));
    }
    //========================================================================================================
    private void cell_MouseClick(object sender, MouseEventArgs e)
    {
        CellMouseClick?.Invoke(sender, new CellMouseEventArgs1D(e.Button, e.Clicks, e.X, e.Y, e.Delta, ((VisualValue<BaseType>)sender).Index));
    }
    //========================================================================================================
    private void cell_MouseHover(object sender, EventArgs e)
    {
            
    }
    //========================================================================================================
    private void cell_MouseUp(object sender, MouseEventArgs e)
    {
        CellMouseUp?.Invoke(sender, new CellMouseEventArgs1D(e.Button, e.Clicks, e.X, e.Y, e.Delta, ((VisualValue<BaseType>)sender).Index));
    }
    //========================================================================================================
    private void cell_MouseLeave(object sender, EventArgs e)
    {
        CellMouseLeave?.Invoke(sender, new CellEventArgs(((VisualValue<BaseType>)sender).Index, -1, -1));
    }
    //========================================================================================================
    private void cell_MouseEnter(object sender, EventArgs e)
    {
        CellMouseEnter?.Invoke(sender, new CellEventArgs(((VisualValue<BaseType>)sender).Index, -1, -1));
    }
    //========================================================================================================
    private void cell_MouseDown(object sender, MouseEventArgs e)
    {
        CellMouseDown?.Invoke(sender, new CellMouseEventArgs1D(e.Button, e.Clicks, e.X, e.Y, e.Delta, ((VisualValue<BaseType>)sender).Index));
    }
    #endregion

    #region Itérateur
    //========================================================================================
    /// <summary>
    /// Permet de parcourir séquentiellement tous les VisualInt dans ce conteneur.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<BaseType> GetEnumerator()
    {
        foreach (VisualValue<BaseType> vi in m_colVisualElements)
            yield return vi.Value;
    }
    #endregion

    #region Indexeur
    //========================================================================================
    /// <summary>
    /// Accède aux VisualInt par un Index
    /// </summary>
    /// <param name="pIndex">Index du VisualInt dans le conteneur</param>
    /// <returns></returns>
    public BaseType this[int pIndex]
    {
        get
        {
            if (pIndex < 0 || pIndex >= m_colVisualElements.Count)
            {
                throw new IndexOutOfRangeException("Débordement de la grille : pIndex = " + pIndex + " , doit être compris entre 0 et " + (m_colVisualElements.Count - 1));
            }

            return m_colVisualElements[pIndex].Value;
        }
        set
        {
            if (pIndex < 0 || pIndex >= m_colVisualElements.Count)
            {
                throw new IndexOutOfRangeException("Débordement de la grille : pIndex = " + pIndex + " , doit être compris entre 0 et " + (m_colVisualElements.Count - 1));
            }

            m_colVisualElements[pIndex].Value = value;
        }
    }
    #endregion

    #region Méthodes redéfinies
    //========================================================================================================================
    /// <summary>
    /// Dessine le contrôle.
    /// </summary>
    /// <param name="pe"></param>
    protected override void OnPaint(PaintEventArgs pe)
    {
        Rectangle rect = new(0, 0, Width - 1, Height - 1);
        using (Pen cr = new(ForeColor))
            pe.Graphics.DrawRectangle(cr, rect);
    }
    #endregion
}
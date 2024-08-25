using System.ComponentModel;
using System.ComponentModel.Design;
using VisualArrays.Others;
using VisualArrays.VisualArrays;

namespace VisualArrays.Sprites;

/// <summary>
/// Représente un objet visuel qui se déplace sur une grille
/// </summary>
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[Designer(typeof(NamedComponentDesigner))]
public abstract class Sprite : Component
{
    private int m_cellWidth = 0;
    private int m_cellHeight = 0;
    private int m_rowCount = 0;
    private int m_colCount = 0;
    private int m_minRow = 0;
    private int m_minCol = 0;
    private int m_maxRow = 0;
    private int m_maxCol = 0;

    #region Détection de collision
    /// <summary>
    /// Détecte si un point est en collision avec le Sprite
    /// </summary>
    /// <param name="pRectangle">Rectangle à vérifier</param>
    /// <returns></returns>
    protected internal virtual bool Hit(Rectangle pRectangle)
    {
        bool inBounds = Visible && Rectangle.Intersect(m_bounds, pRectangle) != Rectangle.Empty;
        if (m_tabCells.Count == 0)
            return inBounds;

        if (!inBounds) return false;
        // On va déterminer la zone qu'occupe les points dans le Sprite
        int row = (pRectangle.Y - Bounds.Top) / m_cellHeight + m_minRow;
        int col = (pRectangle.X - Bounds.Left) / m_cellWidth + m_minCol;
        foreach (Address adr in m_tabCells)
        {
            if (adr.Row == row && adr.Column == col)
                return true;
        }
        return false;
    }
    #endregion

    #region Code pour un Sprite multi qui se déploie sur plusieurs cellules
    /// <summary>
    /// Liste des points qui composent le Sprite, si Count == 0 alors Sprite normal
    /// </summary>
    internal List<Address> m_tabCells = new();

    /// <summary>
    /// Ajoute une cellule au Sprite, nécessaire seulement si le Sprite est déployé sur plusieurs cellules.
    /// Permet de créer un Sprite avec une forme irrégulière, par exemple un coude.
    /// </summary>
    /// <param name="pRowOffset">Déplacement en rangée par rapport à son DisplayAddress</param>
    /// <param name="pColumnOffset">Déplacement en colonne par rapport à son DisplayAddress </param>
    public void AddCell(int pRowOffset, int pColumnOffset)
    {
        m_tabCells.Add(new Address(pRowOffset, pColumnOffset));

        // On va déterminer la zone qu'occupe les points dans le Sprite
        m_minRow = int.MaxValue;
        m_maxRow = int.MinValue;
        m_minCol = int.MaxValue;
        m_maxCol = int.MinValue;
        foreach (Address adr in m_tabCells)
        {
            if (adr.Row < m_minRow)
                m_minRow = adr.Row;
            if (adr.Column < m_minCol)
                m_minCol = adr.Column;
            if (adr.Row > m_maxRow)
                m_maxRow = adr.Row;
            if (adr.Column > m_maxCol)
                m_maxCol = adr.Column;
        }
        m_rowCount = m_maxRow - m_minRow + 1;
        m_colCount = m_maxCol - m_minCol + 1;
        m_cellHeight = Bounds.Height / m_rowCount;
        m_cellWidth = Bounds.Width / m_colCount;
    }
    /// <summary>
    /// Indique si le Sprite se déploie sur plusieurs cellules
    /// </summary>
    [Browsable(false)]
    public bool IsMultiCells => m_tabCells.Count > 1;

    /// <summary>
    /// Fournit la coordonnée du centre du point 0,0 de la zone du Sprite
    /// </summary>
    internal Point OriginCenter(Point pDragLocation)
    {
        int x = pDragLocation.X - (Bounds.Width / 2) + m_cellWidth / 2 + (0 - m_minCol) * m_cellWidth;
        int y = pDragLocation.Y - (Bounds.Height / 2) + m_cellHeight / 2 + (0 - m_minRow) * m_cellHeight;
        return new Point(x, y);
    }
    //============================================================================================
    /// <summary>
    /// Obtient un tableau des adresses de toutes les cellules qui composent le Sprite, lorsque que celles-ci
    /// se trouvent toutes sur la grille.
    /// </summary>
    [Browsable(false)]
    public List<Address> DisplayAddresses
    {
        get
        {
            List<Address> liste = new List<Address>();
            if (m_displayIndex == -1)
                liste.Add(new Address(-1, -1));
            else if (m_tabCells.Count == 0)
                liste.Add(DisplayAddress);
            else
                foreach (Address adr in m_tabCells)
                {
                    liste.Add(new Address(DisplayAddress.Row + adr.Row, DisplayAddress.Column + adr.Column));
                }
            return liste;
        }
    }
    //============================================================================================
    /// <summary>
    /// Obtient un tableau des indices de toutes les cellules qui composent le Sprite, lorsque celles-ci
    /// se trouvent toutes sur la grille.
    /// </summary>
    [Browsable(false)]
    public List<int> DisplayIndices
    {
        get
        {
            List<int> liste = new List<int>();
            if (m_displayIndex == -1)
                liste.Add(-1);
            else if (m_tabCells.Count == 0)
                liste.Add(DisplayIndex);
            else
                foreach (Address adr in m_tabCells)
                {
                    liste.Add(m_owner.IndexFromAddress(DisplayAddress.Row + adr.Row, DisplayAddress.Column + adr.Column));
                }
            return liste;
        }
    }
    #endregion

    #region Position en pixels du Sprite et alignement

    internal bool m_mustCalcBounds = true;
    /// <summary>
    /// Spécifie l'emplacement du Sprite dans la VisualArray en pixels
    /// </summary>
    protected Rectangle m_bounds = Rectangle.Empty;
    /// <summary>
    /// Spécifie l'emplacement et la taille du Sprite dans la VisualArray en pixels
    /// </summary>
    [Browsable(false)]
    public Rectangle Bounds => m_bounds;

    /// <summary>
    /// Spécifie le déplacement horizontale en pixels à appliquer sur l'emplacement du Sprite
    /// </summary>
    protected int m_offsetX = 0;
    /// <summary>
    /// Spécifie le déplacement horizontale en pixels à appliquer sur l'emplacement du Sprite
    /// </summary>
    [Localizable(true), Category("Layout")]
    [DefaultValue(typeof(int), "0"), Description("Déplacement horizontale en pixels à appliquer sur l'emplacement du Sprite")]
    public int OffsetX
    {
        get => m_offsetX;
        set
        {
            m_offsetX = value;
            if (m_owner == null) return;
            RecalcBoundsAndRedraw();
        }
    }
    /// <summary>
    /// Spécifie le déplacement verticale en pixels à appliquer sur l'emplacement du Sprite
    /// </summary>
    protected int m_offsetY = 0;
    /// <summary>
    /// Spécifie le déplacement verticale en pixels à appliquer sur l'emplacement du Sprite
    /// </summary>
    [Localizable(true), Category("Layout")]
    [NotifyParentProperty(true)]
    [DefaultValue(typeof(int), "0")]
    [Description("Déplacement verticale en pixels à appliquer sur l'emplacement du Sprite")]
    public int OffsetY
    {
        get => m_offsetY;
        set
        {
            m_offsetY = value;
            if (m_owner == null) return;
            RecalcBoundsAndRedraw();
        }
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit l'alignement de l'élément sur une cellule.
    /// </summary>
    protected ContentAlignment m_alignment = ContentAlignment.MiddleCenter;
    /// <summary>
    /// Obtient ou définit l'alignement de l'élément sur une cellule.
    /// </summary>
    [DefaultValue(ContentAlignment.MiddleCenter)]
    [Description("Alignement du 'Sprite' dans sa cellule")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Localizable(true), Category("Layout")]
    [NotifyParentProperty(true)]
    public ContentAlignment Alignment
    {
        get => m_alignment;
        set
        {
            m_alignment = value;
            if (m_owner == null) return;
            RecalcBoundsAndRedraw();
        }
    }

    #region AlignOnGrid
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si l'élément doit être aligné sur l'ensemble des cellules de la grille
    /// </summary>
    private bool m_alignOnGrid = false;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si l'élément doit être aligné sur l'ensemble des cellules de la grille
    /// </summary>
    [DefaultValue(false)]
    [Localizable(true), Category("Layout")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Description("Détermine si l'élément doit être aligné sur l'ensemble des cellules de la grille")]
    public bool AlignOnGrid
    {
        get => m_alignOnGrid;
        set
        {
            m_alignOnGrid = value;
            if (m_owner == null) return;
            RecalcBoundsAndRedraw();
        }
    }
    #endregion

    /// <summary>
    /// Calcul le rectangle contour du Sprite en fonction de son alignement et de son DisplayIndex
    /// </summary>
    protected virtual void RecalcBounds()
    {
        if (m_displayIndex == -1)
        {
            if (m_alignOnGrid)
                m_bounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(m_owner.GridBounds, Size, m_alignment);
            else
                m_bounds = new Rectangle(m_location, m_size);
        }
        else
        {
            Rectangle cellBounds = m_owner.GetCellBounds(m_displayIndex);
            m_bounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(cellBounds, m_size, m_alignment);
            m_bounds.Offset(m_offsetX, m_offsetY);
        }
    }
    /// <summary>
    /// Calcul le rectangle contour du Sprite en fonction de son alignement et de son DisplayIndex.
    /// Cette version redessine le Sprite à une nouvelle position en tenant compte de la distance de son déplacement.
    /// Pour un petit déplacement une seule zone est redessinée autrement 2 zones sont redessinées
    /// </summary>
    protected internal void RecalcBoundsAndRedraw()
    {
        Rectangle oldBounds = m_bounds;
        RecalcBounds(); 
        //if (m_displayIndex == -1)
        //{
        //    m_bounds = new Rectangle(m_location, m_size);
        //}
        //else
        //{
        //    Address emplacement = m_owner.IndexToAddress(m_displayIndex);
        //    Rectangle cellBounds = m_owner.GetCellBounds(emplacement.Row, emplacement.Column);
        //    m_bounds = CellVisualElement.BoundsFromAlignment(cellBounds, m_size, m_alignment);
        //    m_bounds.Offset(m_offsetX, m_offsetY);
        //}

        // dans ce cas les deux rectangles se touchent, il s'agit d'un déplacement inférieur à la taille du Sprite
        if (Rectangle.Intersect(m_bounds, oldBounds) != Rectangle.Empty)
        {
            Rectangle unionRect = Rectangle.Union(oldBounds, m_bounds);
            m_owner.UpdateSprites(unionRect);
        }
        else
        {
            m_owner.UpdateSprites(oldBounds);
            m_owner.UpdateSprites(m_bounds);
        }
    }
    #endregion

    #region Drag & Drop
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si le Sprite peut faire partie d'une opération glisser-déposer.
    /// </summary>
    internal protected bool m_allowDrag = false;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si le Sprite peut faire partie d'une opération glisser-déposer.
    /// </summary>
    [DefaultValue(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Description("Indique si le Sprite peut faire partie d'une opération glisser-déposer")]
    [Localizable(true), Category("Layout")]
    public bool AllowDrag
    {
        get => m_allowDrag;
        set
        {
            m_allowDrag = value;
            if (m_allowDrag)
                AcceptClick = true;
        }
    }
    #endregion

    #region Animation du Sprite
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si le déplacement du Sprite doit être animé de façon assynchrone.
    /// </summary>
    internal protected bool m_isMoving = false;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Indique si le Sprite se déplace actuellement
    /// </summary>
    [Browsable(false)]
    public bool IsMoving
    {
        get => m_isMoving;
        set => m_isMoving = value;
    }


    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si le déplacement du Sprite doit être animé de façon assynchrone.
    /// </summary>
    internal protected bool m_assync = false;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si le déplacement du Sprite doit être animé de façon assynchrone.
    /// </summary>
    [DefaultValue(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Description("Indique si le déplacement du Sprite doit être animé ou non")]
    [Localizable(true), Category("Animation")]
    public bool Assync
    {
        get => m_assync;
        set => m_assync = value;
    }

    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si le déplacement du Sprite doit être animé ou non.
    /// </summary>
    internal protected bool m_animated = false;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si le déplacement du Sprite doit être animé ou non.
    /// </summary>
    [DefaultValue(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Description("Indique si le déplacement du Sprite doit être animé ou non")]
    [Localizable(true), Category("Animation")]
    public bool Animated
    {
        get => m_animated;
        set => m_animated = value;
    }

    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si le Sprite doit suivre la grille lors d'un déplacement animé.
    /// </summary>
    internal protected bool m_followGrid = false;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si le Sprite doit suivre la grille lors d'un déplacement animé.
    /// </summary>
    [DefaultValue(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Description("Indique si le Sprite doit suivre la grille lors d'un déplacement animé")]
    [Localizable(true), Category("Animation")]
    public bool FollowGrid
    {
        get => m_followGrid;
        set => m_followGrid = value;
    }

    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la durée utilisée pour animer le déplacement du Sprite
    /// </summary>
    protected int m_duration = 500;
    /// <summary>
    /// Obtient ou définit la durée utilisée pour animer le déplacement du Sprite
    /// </summary>
    [DefaultValue(500)]
    [Description("Durée utilisée pour animer le déplacement du Sprite")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Localizable(true), Category("Animation")]
    public int Duration
    {
        get => m_duration;
        set
        {
            if (m_duration != value)
            {
                if (value < 100)
                {
                    throw new ArgumentOutOfRangeException(
                        "Duration",
                        value,
                        "doit être >= 100");
                }
                m_duration = value;
            }
        }
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la définition en image par secondes de l'animation du déplacement du Sprite
    /// </summary>
    protected int m_frameRate = 30;
    /// <summary>
    /// Obtient ou définit la définition en image par secondes de l'animation du déplacement du Sprite
    /// </summary>
    [DefaultValue(30)]
    [Description("Définition en image par secondes de l'animation du déplacement du Sprite")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Localizable(true), Category("Animation")]
    public int FrameRate
    {
        get => m_frameRate;
        set
        {
            if (m_frameRate != value)
            {
                if (value < 10)
                {
                    throw new ArgumentOutOfRangeException(
                        "FrameRate",
                        value,
                        "doit être >= 10");
                }
                m_frameRate = value;
            }
        }
    }
    /// <summary>
    /// Permet de déplacer graduellement le Sprite d'un DisplayIndex à un autre 
    /// </summary>
    /// <param name="pFuturIndex">Index futur à atteindre</param>
    private void Animate(int pFuturIndex)
    {
        if (m_followGrid && DisplayIndex != -1)
        {
            int startIndex = m_displayIndex;
            int lastIndex = pFuturIndex;
            if (pFuturIndex > startIndex)
                for (int index = startIndex; index <= lastIndex; index++)
                    AnimateToIndex(index);
            else
                for (int index = startIndex; index >= lastIndex; index--)
                    AnimateToIndex(index);
        }
        else
            AnimateToIndex(pFuturIndex);
    }
    /// <summary>
    /// Permet de déplacer graduellement le Sprite d'un DisplayIndex à un autre 
    /// </summary>
    /// <param name="pFuturIndex">Index futur à atteindre</param>
    private void AnimateToIndex(int pFuturIndex)
    {
        if (pFuturIndex == m_displayIndex) return;

        int nbPas = m_duration * m_frameRate / 1000;
        int delai = m_duration / nbPas;
        Rectangle startBounds = m_bounds;

        Address emplacement = m_owner.IndexToAddress(pFuturIndex);
        Rectangle cellBounds = m_owner.GetCellBounds(emplacement.Row, emplacement.Column);
        Rectangle futurBounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(cellBounds, m_size, m_alignment);
        futurBounds.Offset(m_offsetX, m_offsetY);

        int distanceTotalX = futurBounds.Left - startBounds.Left;
        int distanceTotalY = futurBounds.Top - startBounds.Top;

        for (int pas = 0; pas <= nbPas; pas++)
        {
            Rectangle spriteAnimPos = startBounds;
            // calcul la position du Sprite en tenant compte du pas actuel
            spriteAnimPos.Offset(distanceTotalX * pas / nbPas, distanceTotalY * pas / nbPas);
            MoveToThread(m_owner,this,new Point(spriteAnimPos.X, spriteAnimPos.Y));
            //---------------------------------------------------------------------------------
            DateTime maintenantPlusDelai = DateTime.Now.AddMilliseconds(delai);
            while (DateTime.Now < maintenantPlusDelai) ;
        }
        MoveToThread(m_owner,this,new Point(futurBounds.Left, futurBounds.Top));
        m_mustCalcBounds = false;
    }

    #endregion

    //----------------------------------------------------------------------------------------
    private delegate void DéplacerSprite(BaseGrid pGrille, Sprite pSprite,Point pPos);
    private void MoveToThread(BaseGrid pGrille, Sprite pSprite, Point pPos)
    {
        // Obtient une valeur indiquant si l'appelant doit appeler une méthode Invoke lors d'appels 
        // de méthode au contrôle parce que l'appelant se trouve sur un thread différent de celui 
        // sur lequel le contrôle a été créé. 
        if (pGrille.InvokeRequired)
        {
            DéplacerSprite unDel = MoveToThread;
            // Exécute un délégué sur le thread qui détient le handle de fenêtre sous-jacent du contrôle
            pGrille.Invoke(unDel, pGrille, pSprite, pPos);
        }
        else
            MoveTo(pPos);
    } 


    #region Propriétés : DisplayIndex et DisplayAddress
    private Thread m_thread = null;
    private int m_futurIndex = 0;
    private void DoAssyncAnim()
    {
        m_isMoving = true;
        Animate(m_futurIndex);
        m_displayIndex = m_futurIndex;
        // à ce stade l'animation est terminée
        m_isMoving = false;
    }
    //=================================================================================
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient sous forme d'index l'emplacement de l'item sur la grille propriétaire
    /// </summary>
    protected int m_displayIndex;
    /// <summary>
    /// Obtient sous forme d'index l'emplacement du 'Sprite' sur la grille propriétaire
    /// </summary>
    [DefaultValue(typeof(int), "0")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Localizable(true), Category("Layout")]
    public int DisplayIndex
    {
        get => m_displayIndex;
        set
        {
            // clause de garde
            if (m_owner == null) // on doit modifier la valeur même si la grille n'est pas encore associée
            {
                m_displayIndex = value;
                m_mustCalcBounds = true;
                return;
            }

            if (value == -1) // cas spécial ou on doit utiliser l'origine et non l'index
            {
                m_displayIndex = -1;
                RecalcBoundsAndRedraw();
                //MoveTo(m_location);
                return;
            }

            if (m_animated && m_visible && !m_owner.TestMode && value != m_displayIndex)
            {
                if (m_assync) // l'animation s'effectue de façon assynchrone avec un Thread
                {
                    if (m_thread is { IsAlive: true })
                    { // si l'animation est en cours ne rien faire

                    }
                    else // dans ce cas on va créer un nouveau thread puis le démarrer
                    {
                        m_futurIndex = value;
                        m_thread = new Thread(DoAssyncAnim);
                        m_thread.Start();
                    }
                }
                else
                {
                    Animate(value);
                    m_displayIndex = value;
                }
            }
            else // déplacement normal d'un Sprite sans animation (Fonctionnement : OK)
            {
                m_displayIndex = value;
                RecalcBoundsAndRedraw();
                //Rectangle oldBounds = m_bounds;

                //Address emplacement = m_owner.IndexToAddress(value);
                //Rectangle cellBounds = m_owner.GetCellContentBounds(emplacement.Row, emplacement.Column);
                //m_bounds = CellVisualElement.BoundsFromAlignment(cellBounds, m_size, m_alignment);
                //m_bounds.Offset(m_offsetX, m_offsetY);

                //m_mustCalcBounds = false;

                //m_displayIndex = value;
                //// dans ce cas les deux rectangles se touchent, il s'agit d'un déplacement inférieur à la taille du Sprite
                //if (Rectangle.Intersect(m_bounds, oldBounds) != Rectangle.Empty)
                //{
                //    Rectangle unionRect = Rectangle.Union(oldBounds, m_bounds);
                //    m_owner.UpdateSprites(unionRect);
                //}
                //else // dans ce cas il faut effacer le Sprite à son ancienne position pour le redessiner plus loin
                //{
                //    m_owner.UpdateSprites(oldBounds);
                //    m_owner.UpdateSprites(m_bounds);
                //}
            }
        }
    }

    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit l'adresse du Sprite sur la grille propriétaire.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Localizable(true), Category("Layout")]
    [Browsable(true)]
    [NotifyParentProperty(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("Adresse du Sprite sur la grille")]
    public Address DisplayAddress
    {
        get
        {
            if (m_displayIndex == -1)
                return new Address(-1, -1);

            if (m_owner != null)
            {
                return new Address(m_displayIndex / m_owner.ColumnCount, m_displayIndex % m_owner.ColumnCount);
            }

            return new Address(0, 0);
        }
        set
        {
            if (m_owner != null)
            {
                if (value.Row < 0 || value.Row >= m_owner.RowCount || value.Column < 0 || value.Column >= m_owner.ColumnCount)
                    throw new VisualArrayException("Cette adresse n'est pas valide : row = " + value.Row + " , column = " + value.Column);
                DisplayIndex = value.Row * m_owner.ColumnCount + value.Column;
            }
        }
    }
    internal static readonly Address defaultDisplayAddress = new(0, 0);
    private void ResetDisplayAddress()
    {
        DisplayAddress = defaultDisplayAddress;
    }
    private bool ShouldSerializeDisplayAddress()
    {
        return DisplayAddress != defaultDisplayAddress;
    }
    #endregion

    #region Déplacement d'un Sprite
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Déplace le Sprite à un emplacement spécifié en pixels, lorsque le Sprite est déplacé par
    /// cette méthode il devient automatiquement un Sprite libre dont le DisplayIndex devient NO_INDEX
    /// et son Address devient NO_ADDRESS.
    /// </summary>
    public void MoveTo(Point pLocation)
    {
        if (m_owner == null) return;
        //DisplayIndex = NO_INDEX; // il faut modifier DisplayIndex pour ne rien faire dans ce cas
        //Address = NO_ADDRESS; // il faut modifier Address pour ne rien faire dans ce cas
        // determiner la zone à mettre a jour

        Rectangle oldBounds = m_bounds;
        m_bounds.X = pLocation.X;
        m_bounds.Y = pLocation.Y;
        m_bounds.Width = m_size.Width;
        m_bounds.Height = m_size.Height;

        // dans ce cas les deux rectangles se touchent, il s'agit d'un déplacement inférieur à la taille du Sprite
        if (Rectangle.Intersect(m_bounds, oldBounds) != Rectangle.Empty)
        {
            Rectangle unionRect = Rectangle.Union(oldBounds, m_bounds);
            m_owner.UpdateSprites(unionRect);
            //va_owner.Invalidate(unionRect);
        }
        else
        {
            m_owner.UpdateSprites(oldBounds);
            m_owner.UpdateSprites(m_bounds);
        }
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Déplace le Sprite à la colonne suivante.
    /// </summary>
    public void MoveRight()
    {
        DisplayAddress = new Address(DisplayAddress.Row, DisplayAddress.Column + 1);
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Déplace le Sprite à la colonne précédente
    /// </summary>
    public void MoveLeft()
    {
        DisplayAddress = new Address(DisplayAddress.Row, DisplayAddress.Column - 1);
    }
    /// <summary>
    /// Déplace le Sprite à la rangée précédente
    /// </summary>
    public void MoveUp()
    {
        DisplayAddress = new Address(DisplayAddress.Row - 1, DisplayAddress.Column);
    }
    /// <summary>
    /// Déplace le Sprite à la rangée suivante
    /// </summary>
    public void MoveDown()
    {
        DisplayAddress = new Address(DisplayAddress.Row + 1, DisplayAddress.Column);
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Déplace le Sprite à un nouvel emplacement sur la grille.
    /// </summary>
    /// <param name="pRow">Rangée dans la grille.</param>
    /// <param name="pColumn">Colonne dans la grille.</param>
    public void MoveTo(int pRow, int pColumn)
    {
        DisplayIndex = pRow * m_owner.ColumnCount + pColumn;
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la direction courante du Sprite utilisée lors d'un déplacement d'un pas.
    /// </summary>
    internal protected enuDirection m_direction = enuDirection.Right;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit la direction courante du Sprite utilisée lors d'un déplacement d'un pas.
    /// </summary>
    [DefaultValue(enuDirection.Right)]
    [Localizable(true), Category("Layout")]
    [Description("Direction courante du Sprite utilisée lors d'un déplacement d'un pas")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public enuDirection Direction
    {
        get => m_direction;
        set => m_direction = value;
        //if (va_owner == null) return;
        //va_owner.UpdateSprites(va_bounds);
    }
    /// <summary>
    /// Fournit l'adresse future du Sprite si on le déplace d'un pas dans la direction courante avec MoveOneStep()
    /// </summary>
    [Browsable(false)]
    public Address NextStepAddress
    {
        get
        {
            switch (m_direction)
            {
                case enuDirection.Left:
                    return new Address(DisplayAddress.Row, DisplayAddress.Column - 1);
                case enuDirection.Top:
                    return new Address(DisplayAddress.Row - 1, DisplayAddress.Column);
                case enuDirection.Right:
                    return new Address(DisplayAddress.Row, DisplayAddress.Column + 1);
                case enuDirection.Bottom:
                    return new Address(DisplayAddress.Row + 1, DisplayAddress.Column);
                default:
                    return Address.Empty;
            }
        }
    }
    /// <summary>
    /// Déplace le Sprite d'un seul pas dans la direction courante.
    /// </summary>
    public void MoveOneStep()
    {
        switch (m_direction)
        {
            case enuDirection.Left:
                if (DisplayAddress.Column - 1 < 0)
                    throw new VisualArrayException("Débordement de la grille : DisplayAddress.Column = " + (DisplayAddress.Column - 1));

                MoveTo(DisplayAddress.Row, DisplayAddress.Column - 1);
                break;
            case enuDirection.Top:
                if (DisplayAddress.Row - 1 < 0)
                    throw new VisualArrayException("Débordement de la grille : DisplayAddress.Row = " + (DisplayAddress.Row - 1));

                MoveTo(DisplayAddress.Row - 1, DisplayAddress.Column);
                break;
            case enuDirection.Right:
                if (Owner != null && DisplayAddress.Column + 1 >= Owner.ColumnCount)
                    throw new VisualArrayException("Débordement de la grille : DisplayAddress.Column = " + (DisplayAddress.Column + 1));

                MoveTo(DisplayAddress.Row, DisplayAddress.Column + 1);
                break;
            case enuDirection.Bottom:
                if (Owner != null && DisplayAddress.Row + 1 >= Owner.RowCount)
                    throw new VisualArrayException("Débordement de la grille : DisplayAddress.Row = " + (DisplayAddress.Row + 1));

                MoveTo(DisplayAddress.Row + 1, DisplayAddress.Column);
                break;
        }
    }
    /// <summary>
    /// Change la direction du Sprite pour la direction suivante selon l'ordre suivant  : Left, Top, Right, Bottom
    /// </summary>
    public void ChangeDirection()
    {
        m_direction++;
        if (m_direction > enuDirection.Bottom)
            m_direction = enuDirection.Left;
    }
    /// <summary>
    /// Change la direction du Sprite pour une direction différente, séquentiellement ou aléatoirement.
    /// </summary>
    /// <param name="pRandomDirection">true, pour que la nouvelle direction soit choisie aléatoirement</param>
    public void ChangeDirection(bool pRandomDirection)
    {
        if (m_owner == null) return;
        m_direction = m_owner.RandomDirection(m_direction);
    }
    #endregion

    #region Propriétés : Owner, Size, Visible
    /// <summary>
    /// VisualArray propriétaire de ce Sprite.
    /// </summary>
    protected BaseGrid m_owner = null;
    /// <summary>
    /// VisualArray propriétaire de ce Sprite.
    /// </summary>
    [Browsable(false)]
    protected internal BaseGrid Owner
    {
        get => m_owner;
        set
        {
            m_owner = value;
            RecalcBounds();
        }
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si l'élément accepte les clicks.
    /// </summary>
    internal protected bool m_acceptClick = false;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si l'élément accepte les clicks.
    /// </summary>
    [DefaultValue(false)]
    [Localizable(true), Category("Layout")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Description("Détermine si l'élément accepte les clicks")]
    public bool AcceptClick
    {
        get => m_acceptClick;
        set => m_acceptClick = value;
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si l'élément accepte d'être déposé à l'extérieur d'une cellule.
    /// </summary>
    internal protected bool m_allowOutsideDrop = false;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si le Sprite accepte d'être déposé à l'extérieur d'une cellule.
    /// </summary>
    [DefaultValue(false)]
    [Localizable(true), Category("Layout")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Description("Détermine si le Sprite accepte d'être déposé à l'extérieur d'une cellule")]
    public bool AllowOutsideDrop
    {
        get => m_allowOutsideDrop;
        set => m_allowOutsideDrop = value;
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si l'élément est visible ou masqué.
    /// </summary>
    internal protected bool m_visible = true;
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Obtient ou définit si l'élément est visible ou masqué.
    /// </summary>
    [DefaultValue(true)]
    [Localizable(true), Category("Layout")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Description("Détermine l'élément est visible ou masqué")]
    public bool Visible
    {
        get => m_visible;
        set
        {
            m_visible = value;
            m_owner?.UpdateSprites(m_bounds);
        }
    }
    //============================================================================================
    internal static readonly Size defaultSpriteSize = new(25, 25);
    internal Size m_size = defaultSpriteSize;
    /// <summary>
    /// Obtient et définit la largeur et la hauteur du Sprite en pixels .
    /// </summary>
    [Localizable(true), Category("Layout")]
    [Browsable(true)]
    [NotifyParentProperty(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("Largeur et hauteur du Sprite en pixels")]
    public Size Size
    {
        get => m_size;
        set
        {
            if (value != m_size)
            {
                m_size = value;
                if (m_owner == null) return;
                RecalcBoundsAndRedraw();
            }
        }
    }
    private void ResetSize()
    {
        Size = defaultSpriteSize;
    }
    private bool ShouldSerializeSize()
    {
        return m_size != defaultSpriteSize;
    }

    //============================================================================================
    internal static readonly Point defaultLocation = new(0, 0);
    internal Point m_location = defaultLocation;
    /// <summary>
    /// Obtient et définit la position du Sprite en pixels lorsque son DisplayIndex est -1.
    /// </summary>
    [Localizable(true), Category("Layout")]
    [Browsable(true)]
    [NotifyParentProperty(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Description("Position du Sprite en pixels lorsque son DisplayIndex est -1")]
    public Point Location
    {
        get => m_location;
        set
        {
            if (value != m_location)
            {
                m_location = value;
                if (m_owner == null) return;
                RecalcBoundsAndRedraw();
            }
        }
    }
    private void ResetLocation()
    {
        Location = defaultLocation;
    }
    private bool ShouldSerializeLocation()
    {
        return m_location != defaultLocation;
    }

    #endregion

    #region Constructeur
    /// <summary>
    /// Initialise une instance de la classe Sprite
    /// </summary>
    public Sprite()
    {
    }
    #endregion

    #region Méthodes pour dessiner le Sprite

    /// <summary>
    /// Force le 'Sprite' à se redessiner et à redessiner tous les éléments à cet emplacement
    /// </summary>
    public void Refresh()
    {
        m_owner?.UpdateSprites(m_bounds);
    }
    /// <summary>
    /// Dessine ce 'Sprite'.
    /// </summary>
    /// <param name="pGraphics"></param>
    public virtual void Draw(Graphics pGraphics)
    {
        if (m_owner != null && m_mustCalcBounds)
        {
            //RecalcBoundsAndRedraw();
            RecalcBounds();
            m_mustCalcBounds = false;
        }
    }
    /// <summary>
    /// Dessine le Sprite à la coordonnée 0,0 dans le graphics
    /// </summary>
    /// <param name="pGraphics">Destination du dessin</param>
    public abstract void DrawAtOrigin(Graphics pGraphics);
    #endregion

    #region Méthodes : BringToFront et SendToBack
    //-------------------------------------------------------------------------
    /// <summary>
    /// Place le 'Sprite' à l'avant plan devant tous les autres Sprites
    /// </summary>
    public void BringToFront()
    {
        m_owner?.Sprites.BringToFront(this);
    }
    /// <summary>
    /// Place le 'Sprite' au dernier plan derrière tous les autres Sprites
    /// </summary>
    public void SendToBack()
    {
        m_owner?.Sprites.SendToBack(this);
    }
    #endregion

    #region IsValidMove
    /// <summary>
    /// Indique si le Sprite peut se rendre à pDestinationAddress. Typiquement on doit la redéfinir dans une sous-classe.
    /// Par défaut elle vérifie simplement si l'adresse destinataire est dans la grille.
    /// </summary>
    /// <param name="pDestination">Adresse ou l'on souhaite déplacer le Sprite</param>
    /// <returns>true si l'adresse est acceptée</returns>
    public virtual bool IsValidMove(Address pDestination)
    {
        return pDestination.Column >= 0 && pDestination.Column < m_owner.ColumnCount &&
               pDestination.Row >= 0 && pDestination.Row < m_owner.RowCount;
    }
    #endregion

    #region Propriété Name

    private string name;
    /// <summary>
    /// Obtient ou définit le nom du Sprite
    /// </summary>
    [Browsable(false)]
    public string Name
    {
        get => name;
        set => name = value;
    }



    ////-------------------------------------------------------------------------------------
    ///// <summary>
    ///// Obtient ou définit le facteur d'agrandissement de la représentation de l'élément visuel
    ///// </summary>
    //protected int va_zoom;
    ///// <summary>
    ///// Obtient ou définit le facteur d'agrandissement de la représentation de l'élément visuel
    ///// </summary>
    //[DefaultValue(typeof(int), "100")]
    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    //[Localizable(true), Category("Layout")]
    //public int Zoom
    //{
    //    get { return va_zoom; }
    //    set
    //    {
    //        if (value > 200) value = 200;
    //        if (value < 5) value = 5;
    //        va_zoom = value;
    //        if (va_owner == null) return;
    //        va_owner.UpdateSprites(va_bounds); ;
    //    }
    //}
    #endregion
}

#region NamedComponentDesigner nécessaire pour le fonctionnement de la propriété Name

internal class NamedComponentDesigner : ComponentDesigner
{

    private string Name
    {
        get => Component.Site.Name;
        set
        {
            // don't do anything here during loading, if a refactor changed it we don't want to do anything
            IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host is null or { Loading: false })
            {
                Component.Site.Name = value;
            }
        }
    }
    protected override void PreFilterProperties(System.Collections.IDictionary properties)
    {
        base.PreFilterProperties(properties);

        string[] shadowProps = new string[] {             
            "Name"
        };

        Attribute[] empty = new Attribute[0];
        PropertyDescriptor prop;
        for (int i = 0; i < shadowProps.Length; i++)
        {
            prop = (PropertyDescriptor)properties[shadowProps[i]];
            if (prop != null)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(NamedComponentDesigner), prop, empty);
            }
        }

    }

}
#endregion
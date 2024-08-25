using System.ComponentModel;
using System.Drawing.Drawing2D;
using VisualArrays.Others;
using VisualArrays.VisualArrays;

namespace VisualArrays.Sprites;

/// <summary>
/// Représente un 'Sprite' ayant la forme d'un segment partant d'une cellule vers une autre
/// </summary>
public class SegmentSprite : Sprite
{
    private Rectangle m_sourceBounds, m_destinationBounds;

    #region Propriétés : Color, Opacity
    //-------------------------------------------------------------------------------------
    private Color m_color = Color.Blue;
    /// <summary>
    /// Obtient ou définit la couleur de la forme
    /// </summary>
    [DefaultValue(typeof(Color), "Blue"), Description("Couleur de fond du 'Sprite'")]
    [Localizable(true), Category("Layout")]
    public Color Color
    {
        get => m_color;
        set
        {
            m_color = value;
            m_owner?.UpdateSprites(m_bounds);
        }
    }

    //-------------------------------------------------------------------------------------
    private int m_opacity = 255;
    /// <summary>
    /// Obtient et définit le niveau d'opacité du Sprite entre 0 et 255.
    /// </summary>
    [DefaultValue(255), Description("Niveau d'opacity du Sprite")]
    public int Opacity
    {
        get => m_opacity;
        set
        {
            if (value is < 0 or > 255)
            {
                throw new ArgumentOutOfRangeException(
                    "Opacity",
                    value,
                    "doit être >= 0 et <= à 255");
            }
            if (value != m_opacity)
            {
                m_opacity = value;
                if (m_owner == null) return;
                m_owner.UpdateSprites(m_bounds);
            }
        }
    }

    //-------------------------------------------------------------------------------------
    private int m_bulletSize = 6;
    /// <summary>
    /// Obtient et définit la taille des ronds au bout du segment.
    /// </summary>
    [DefaultValue(6), Description("Taille des ronds au bout du segment")]
    public int BulletSize
    {
        get => m_bulletSize;
        set
        {
            if (value is < 0 or > 255)
            {
                throw new ArgumentOutOfRangeException(
                    "BulletSize",
                    value,
                    "doit être >= 0 et <= à 255");
            }
            if (value != m_bulletSize)
            {
                m_bulletSize = value;
                if (m_owner == null) return;
                RecalcBoundsAndRedraw();
            }
        }
    }
    #endregion

    #region Propriétés : DestinationIndex, DestinationAddress
    private int m_destinationIndex = 0;
    /// <summary>
    /// Obtient ou définit l'index de la cellule où se termine le segment.
    /// </summary>
    [DefaultValue(0), Category("Layout")]
    public int DestinationIndex
    {
        get => m_destinationIndex;
        set
        {
            m_destinationIndex = value;
            if (m_owner != null)
                RecalcBoundsAndRedraw();
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
    public Address DestinationAddress
    {
        get
        {
            if (m_destinationIndex == -1)
                return new Address(-1, -1);

            if (m_owner != null)
            {
                return new Address(m_destinationIndex / m_owner.ColumnCount, m_destinationIndex % m_owner.ColumnCount);
            }

            return new Address(0, 0);
        }
        set
        {
            if (m_owner != null)
            {
                if (value.Row < 0 || value.Row >= m_owner.RowCount || value.Column < 0 || value.Column >= m_owner.ColumnCount)
                    throw new VisualArrayException("Cette adresse n'est pas valide : row = " + value.Row + " , column = " + value.Column);
                DestinationIndex = value.Row * m_owner.ColumnCount + value.Column;
            }
        }
    }
    #endregion

    #region Constructeur
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Initialise un SegmentSprite
    /// </summary>
    public SegmentSprite()
    {
    }
    #endregion

    #region Méthodes : RecalcBounds, Hit
    /// <summary>
    /// Calcul le rectangle contour du Sprite en fonction de son alignement.
    /// </summary>
    protected override void RecalcBounds()
    {
        //if (m_sourceAddress != Address.Empty && m_destinationAddress != Address.Empty)
        //{
        //    m_sourceBounds = m_owner.GetCellBounds(m_sourceAddress.Row, m_sourceAddress.Column);
        //    m_destinationBounds = m_owner.GetCellBounds(m_destinationAddress.Row, m_destinationAddress.Column);
        //}
        //else
        Size taille = new(m_bulletSize << 1, m_bulletSize << 1);
        if (m_displayIndex == -1)
        {
            if (AlignOnGrid)
            {
                m_bounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(m_owner.GridBounds, Size, m_alignment);
                m_sourceBounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(m_sourceBounds, taille, m_alignment);
                m_destinationBounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(m_destinationBounds, taille, m_alignment);
            }
            else
            {
                m_bounds = new Rectangle(m_location, m_size);
                m_sourceBounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(m_bounds, taille, ContentAlignment.MiddleLeft);
                m_destinationBounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(m_bounds, taille, ContentAlignment.MiddleRight);
            }
        }
        else
        {
            Address adrSource = m_owner.IndexToAddress(m_displayIndex);
            Address adrDestination = m_owner.IndexToAddress(m_destinationIndex);
            m_sourceBounds = m_owner.GetCellBounds(adrSource.Row, adrSource.Column);
            m_destinationBounds = m_owner.GetCellBounds(adrDestination.Row, adrDestination.Column);
            m_sourceBounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(m_sourceBounds, taille, m_alignment);
            m_destinationBounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(m_destinationBounds, taille, m_alignment);
        }

        m_bounds = Rectangle.Union(m_sourceBounds, m_destinationBounds);
        m_bounds.Width += 1;
        m_bounds.Height += 1;
        m_bounds.Offset(m_offsetX, m_offsetY);
    }

    //-------------------------------------------------------------------------------------
    /// <summary>
    /// Actuellement seulement les bouts sont détectés, pas le segment.
    /// </summary>
    /// <param name="pRectangle">Rectangle à vérifier</param>
    /// <returns></returns>
    protected internal override bool Hit(Rectangle pRectangle)
    {
        GraphicsPath path = new();
        path.AddEllipse(m_sourceBounds.Left, m_sourceBounds.Top, m_bulletSize << 1, m_bulletSize << 1);

        //Point[] tabPoint = { new Point(m_sourceBounds.Left - 6, m_sourceBounds.Top - 6), new Point(m_sourceBounds.Left + 6, m_sourceBounds.Top - 6) , 
        //                 new Point(m_destinationBounds.Left + 6, m_destinationBounds.Top + 6) , new Point(m_destinationBounds.Left - 6, m_destinationBounds.Top + 6),
        //                   new Point(m_sourceBounds.Left - 6, m_sourceBounds.Top - 6) };
        //path.AddPolygon(tabPoint);

        path.AddEllipse(m_destinationBounds.Left, m_destinationBounds.Top, m_bulletSize << 1, m_bulletSize << 1);
        Region objRegion = new(path);
        objRegion.Intersect(pRectangle);
        return !objRegion.IsEmpty(m_owner.CreateGraphics());
        //{
        //    // The point is in the region. Use an opaque brush.
        //    solidBrush.Color = Color.FromArgb(255, 255, 0, 0);
        //}

    }
    #endregion

    #region Méthodes : Draw, DrawAtOrigin
    /// <summary>
    /// Repositionne le Sprite
    /// </summary>
    /// <param name="pSourceIndex">Nouvelle valeur pour le DisplayIndex</param>
    /// <param name="pDestinationIndex">Nouvelle valeur pour le DestinationIndex</param>
    public void RelocateAt(int pSourceIndex, int pDestinationIndex)
    {
        m_destinationIndex = pDestinationIndex;
        DisplayIndex = pSourceIndex;
    }
    /// <summary>
    /// Repositionne le Sprite
    /// </summary>
    /// <param name="pSourceRow"></param>
    /// <param name="pSourceColumn"></param>
    /// <param name="pDestinationRow"></param>
    /// <param name="pDestinationColumn"></param>
    public void RelocateAt(int pSourceRow, int pSourceColumn, int pDestinationRow, int pDestinationColumn)
    {
        m_destinationIndex = pSourceRow * m_owner.ColumnCount + pSourceColumn;
        DisplayIndex = pDestinationRow * m_owner.ColumnCount + pDestinationColumn;
    }
    //-------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pGraphics"></param>
    public override void Draw(Graphics pGraphics)
    {
        if (m_visible)
        {
            base.Draw(pGraphics);

            Color couleurSegment = Color.FromArgb(m_opacity, m_color);
            Brush brush = new SolidBrush(couleurSegment);
            Rectangle contour = m_bounds;

            pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            pGraphics.DrawLine(new Pen(couleurSegment, m_bulletSize), m_sourceBounds.Left + m_bulletSize, m_sourceBounds.Top + m_bulletSize, m_destinationBounds.Left + m_bulletSize, m_destinationBounds.Top + m_bulletSize);
            pGraphics.FillEllipse(brush, m_sourceBounds.Left, m_sourceBounds.Top, m_bulletSize << 1, m_bulletSize << 1);
            pGraphics.FillEllipse(brush, m_destinationBounds.Left, m_destinationBounds.Top, m_bulletSize << 1, m_bulletSize << 1);

            pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        }
    }
    //===========================================================================
    /// <summary>
    /// Dessine le Sprite à la coordonnée 0,0 dans le graphics
    /// </summary>
    /// <param name="pGraphics">Destination du dessin</param>
    public override void DrawAtOrigin(Graphics pGraphics)
    {
        //Rectangle contour = new Rectangle(0, 0, m_bounds.Width, m_bounds.Height);

        Rectangle sourceBounds = m_sourceBounds;
        sourceBounds.Offset(-m_bounds.Left, -m_bounds.Top);
        Rectangle destinationBounds = m_destinationBounds;
        destinationBounds.Offset(-m_bounds.Left, -m_bounds.Top);

        pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

        Color couleurSegment = Color.FromArgb(m_opacity, m_color);
        Brush brush = new SolidBrush(couleurSegment);

        pGraphics.DrawLine(new Pen(couleurSegment, m_bulletSize), sourceBounds.Left + m_bulletSize, sourceBounds.Top + m_bulletSize, destinationBounds.Left + m_bulletSize, destinationBounds.Top + m_bulletSize);
        pGraphics.FillEllipse(brush, sourceBounds.Left, sourceBounds.Top, m_bulletSize << 1, m_bulletSize << 1);
        pGraphics.FillEllipse(brush, destinationBounds.Left, destinationBounds.Top, m_bulletSize << 1, m_bulletSize << 1);

        pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

        //throw new NotImplementedException();
    }
    #endregion
}
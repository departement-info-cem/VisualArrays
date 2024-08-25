using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VisualArrays.Appearance;
using VisualArrays.CellVisualElement;
using VisualArrays.Others;

namespace VisualArrays.VisualCells;

/// <summary>
/// Classe abstraite qui représente une cellule contenant une valeur. 
/// </summary>
[DefaultEvent("ValueChanged")]
public abstract partial class VisualValue<BaseType> : Control, IComparable<VisualValue<BaseType>>
{
    #region Événement
    /// <summary>
    /// Se produit lorsque la valeur du contrôle change
    /// </summary>
    public abstract event EventHandler ValueChanged;
    #endregion

    #region Constructeur
    //============================================================================================
    /// <summary>
    /// Initialise un contrôle VisualValue
    /// </summary>
    public VisualValue()
    {
        InitializeComponent();
    }
    #endregion

    #region Background VisualElement
    //============================================================================================
    /// <summary>
    /// VisualElement utilisé pour dessiner le fond de la cellule
    /// </summary>
    internal CellVisualElement.CellVisualElement m_backgroundVE = null;

    //============================================================================================
    /// <summary>
    /// Change le CellVisualElement pour le fond de la cellule
    /// </summary>
    internal CellVisualElement.CellVisualElement GetNewBkgVisualElement(IBackgroundAppearance pBackgroundAppearance)
    {
        if (pBackgroundAppearance == null) return null;
        switch (pBackgroundAppearance.Style)
        {
            case enuBkgStyle.None:
                return null;
            case enuBkgStyle.Border:
                return new BorderElement(pBackgroundAppearance.Border, pBackgroundAppearance.BackgroundColor);
            case enuBkgStyle.FillShape:
                return new FillShapeElement(pBackgroundAppearance.Shape, pBackgroundAppearance.BackgroundColor, pBackgroundAppearance.Radius);
            case enuBkgStyle.Shape:
                return new ShapeElement(pBackgroundAppearance.Shape, pBackgroundAppearance.PenWidth, pBackgroundAppearance.BackgroundColor, pBackgroundAppearance.Radius);
            case enuBkgStyle.Image:
                return new ImageElement(pBackgroundAppearance.Image);
            default:
                return null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal abstract void UpdateVisualElement();
    #endregion

    #region Propriétés Internal
    //============================================================================================
    private bool m_showIndex = false;
    /// <summary>
    /// Détermine si le contrôle doit afficher son Index plutôt que sa valeur.
    /// </summary>
    internal bool ShowIndex
    {
        get { return m_showIndex; }
        set
        {
            if (value == m_showIndex) return;
            m_showIndex = value;
            DrawContent(CreateGraphics());
        }
    }
    //============================================================================================
    internal int m_index = 0;
    /// <summary>
    /// Index du contrôle lorsque celui-ci se trouve dans un conteneur
    /// </summary>
    protected internal int Index
    {
        get { return m_index; }
        set
        {
            m_index = value;
            if (m_showIndex) // si nécessaire alors redessiner le contrôle
                DrawContent(CreateGraphics());
        }
    }
    #endregion

    #region ValueAlign
    //============================================================================================
    /// <summary>
    /// Obtient et définit l'alignement de la valeur dans la zone d'édition.
    /// </summary>
    protected ContentAlignment m_valueAlign = ContentAlignment.MiddleCenter;
    /// <summary>
    /// Obtient et définit l'alignement de la valeur dans la zone d'édition.
    /// </summary>
    [DefaultValue(ContentAlignment.MiddleCenter), Browsable(true), Description("Alignement de la valeur dans la zone d'édition")]
    public ContentAlignment ValueAlign
    {
        get { return m_valueAlign; }
        set
        {
            if (value != m_valueAlign)
            {
                m_valueAlign = value;
                DrawContent(null);
            }
        }
    }
    #endregion

    #region BorderColor, BorderSize, FocusColor, HideFocusRect
    //============================================================================================
    private static readonly Color m_defaultBorderColor = Color.Gray;
    //============================================================================================
    /// <summary>
    /// Obtient et définit la couleur de la bordure.
    /// </summary>
    private Color m_borderColor = Color.Gray;
    /// <summary>
    /// Obtient et définit la couleur de la bordure.
    /// </summary>
    [Browsable(true), Description("Couleur de la bordure")]
    public Color BorderColor
    {
        get { return m_borderColor; }
        set
        {
            if (value != m_borderColor)
            {
                m_borderColor = value;
                DrawContent(null);
            }
        }
    }
    private void ResetBorderColor()
    {
        BorderColor = m_defaultBorderColor;
    }
    private bool ShouldSerializeBorderColor()
    {
        return m_borderColor != m_defaultBorderColor;
    }
    //============================================================================================
    /// <summary>
    /// Obtient et définit la taille de la bordure.
    /// </summary>
    private int m_borderSize = 1;
    /// <summary>
    /// Obtient et définit la taille de la bordure.
    /// </summary>
    [DefaultValue(1), Browsable(true), Description("Taille de la bordure")]
    public int BorderSize
    {
        get { return m_borderSize; }
        set
        {
            if (value != m_borderSize)
            {
                m_borderSize = value;
                DrawContent(null);
            }
        }
    }
    //============================================================================================
    private bool m_hideFocusRect = false;
    /// <summary>
    /// Obtient et définit si on doit masquer le rectangle du focus.
    /// </summary>
    [DefaultValue(false), Browsable(true), Description("Indique si le rectangle du focus doit être masqué")]
    public bool HideFocusRect
    {
        get { return m_hideFocusRect; }
        set
        {
            if (value != m_hideFocusRect)
            {
                m_hideFocusRect = value;
                DrawContent(null);
            }
        }
    }
    //============================================================================================
    private static readonly Color m_defaultFocusColor = Color.Black;
    //============================================================================================
    /// <summary>
    /// Obtient et définit la couleur du focus.
    /// </summary>
    private Color m_focusColor = Color.Gray;
    /// <summary>
    /// Obtient et définit la couleur du focus.
    /// </summary>
    [Browsable(true), Description("Couleur du focus")]
    public Color FocusColor
    {
        get { return m_focusColor; }
        set
        {
            if (value != m_focusColor)
            {
                m_focusColor = value;
                DrawContent(null);
            }
        }
    }
    private void ResetFocusColor()
    {
        FocusColor = m_defaultFocusColor;
    }
    private bool ShouldSerializeFocusColor()
    {
        return m_focusColor != m_defaultFocusColor;
    }

    #endregion

    #region DrawContent
    /// <summary>
    /// Dessiner le contenu du contrôle
    /// </summary>
    /// <param name="pGraphics">Graphique destinataire</param>
    protected virtual void DrawContent(Graphics pGraphics)
    {
        // code pour dessiner la bordure
        if (m_borderSize > 0)
            using (Pen cr = new Pen(m_borderColor, m_borderSize))
            {
                pGraphics.DrawRectangle(cr, m_borderSize >> 1, m_borderSize >> 1, Width - m_borderSize, Height - m_borderSize);
            }

        if (!m_hideFocusRect)
        {
            if (ContainsFocus)
                // code pour dessiner le focus
                using (Pen cr = new Pen(m_focusColor))
                {
                    cr.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    pGraphics.DrawRectangle(cr, 1, 1, Width - 3, Height - 3);
                }
            else if (BorderSize < 2)
                using (Pen cr = new Pen(BackColor))
                {
                    pGraphics.DrawRectangle(cr, 1, 1, Width - 3, Height - 3);
                }
        }
    }
    #endregion

    #region ReadOnly
    //============================================================================================
    /// <summary>
    /// Obtient ou définit si le contrôle est en lecture seule.
    /// </summary>
    protected bool m_readOnly = false;
    /// <summary>
    /// Obtient ou définit si le contrôle est en lecture seule.
    /// </summary>
    [DefaultValue(false), Browsable(true), Description("Indique si le contrôle est en lecture seule.")]
    public bool ReadOnly
    {
        get { return m_readOnly; }
        set { m_readOnly = value; }
    }
    #endregion

    #region Enlever la propriété Text de la fenêtre des propriétés
    /// <summary>
    /// Texte associé au contrôle.
    /// </summary>
    [Browsable(false)]
    public override string Text
    {
        get
        {
            return base.Text;
        }
        set
        {
            base.Text = value;
        }
    }
    #endregion

    #region Padding

    internal static readonly Padding m_defaultPadding = new Padding(2);
    //===========================================================================================
    private Padding m_padding = m_defaultPadding;
    /// <summary>
    /// Obtient et définit l'espacement interne du contrôle.
    /// </summary>
    [Browsable(true),
     NotifyParentProperty(true),
     EditorBrowsable(EditorBrowsableState.Always),
     Description("Spécifie l'espacement interne du contrôle")]
    public new Padding Padding
    {
        get { return m_padding; }
        set
        {
            if (value != m_padding)
            {
                m_padding = value;
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

    #region Value
    //=========================================================================================================
    /// <summary>
    /// La valeur actuelle du contrôle VisualValue
    /// </summary>
    public abstract BaseType Value
    {
        get; 
        set;
    }
    #endregion

    #region Méthode CompareTo
    /// <summary>
    /// Utilise le TabIndex afin de déterminer la position du contrôle dans une liste ordonnée
    /// </summary>
    /// <param name="other">VisualValue avec lequel comparer</param>
    /// <returns>S'il est plus petit, plus grand ou égal</returns>
    public int CompareTo(VisualValue<BaseType> other)
    {
        return TabIndex.CompareTo(other.TabIndex);
    }
    #endregion

    #region OnGotFocus, OnLostFocus
    //=========================================================================================================
    /// <summary>
    /// Dessine un rectangle représentant le focus
    /// </summary>
    /// <param name="e"></param>
    protected override void OnGotFocus(EventArgs e)
    {
        using (Graphics gr = CreateGraphics())
            DrawContent(gr);
    }
    //=========================================================================================================
    /// <summary>
    /// Dessine le contrôle sans le focus
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLostFocus(EventArgs e)
    {
        using (Graphics gr = CreateGraphics())
            DrawContent(gr);
    }
    #endregion

    #region OnPaint
    //=========================================================================================================
    /// <summary>
    /// Dessine le contenu du contrôle
    /// </summary>
    /// <param name="pe"></param>
    protected override void OnPaint(PaintEventArgs pe)
    {
        DrawContent(pe.Graphics);
    }
    #endregion
}
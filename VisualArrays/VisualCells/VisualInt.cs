using System.ComponentModel;
using VisualArrays.Others;

// À AJOUTER :
//      - StrikeAppearance pour Enabled == false ???
//      - Événement ValueChanged...
namespace VisualArrays.VisualCells;

/// <summary>
/// Représente une cellule contenant un nombre entier. 
/// </summary>
public partial class VisualInt : VisualGraph<int>
{
    #region Événements
    /// <summary>
    /// Se produit lorsque la valeur du contrôle change
    /// </summary>
    [Description("Se produit lorsque la valeur du contrôle change")]
    public override event EventHandler ValueChanged;
    #endregion

    #region Constructeur
    //=========================================================================================================
    /// <summary>
    /// Initialise un contrôle VisualInt
    /// </summary>
    public VisualInt()
    {
        InitializeComponent();
    }
    #endregion

    #region Propriétés
    //============================================================================================
    private ImageList m_imageList = null;
    /// <summary>
    /// Obtient ou définit l'ImageList utilisée pour dessiner la valeur en mode View ImageList
    /// </summary>
    [Browsable(true), DefaultValue(null), Description("ImageList utilisée pour dessiner la valeur en mode View ImageList")]
    public ImageList ImageList
    {
        get => m_imageList;
        set
        {
            if (value != m_imageList)
            {
                m_imageList = value;
                Refresh();
            }
        }
    }
    //============================================================================================
    private enuIntView m_view = enuIntView.Number;
    /// <summary>
    /// Obtient et définit le style de visualisation la valeur du contrôle
    /// </summary>
    [DefaultValue(enuIntView.Number), Browsable(true), Description("Obtient et définit le style de visualisation pour la valeur du contrôle.")]
    public enuIntView View
    {
        get => m_view;
        set
        {
            if (m_view != value)
            {
                m_view = value;
                Refresh();
            }
        }
    }
    //============================================================================================
    private int m_minimum = -1;
    /// <summary>
    /// Obtient et définit la valeur minimale pour la propriété Value
    /// </summary>
    [DefaultValue(typeof(int), "-1"), Browsable(true), Description("Obtient et définit la valeur minimale pour la propriété Value")]
    [RefreshProperties(RefreshProperties.All)]
    public int Minimum
    {
        get => m_minimum;
        set
        {
            if (value != m_minimum)
            {
                m_minimum = value;

                if (m_value < m_minimum)
                    Value = m_minimum;
                if (m_minimum > m_maximum)
                    Maximum = value;
                //if (!DesignMode)
                Refresh();
            }
        }
    }
    //============================================================================================
    private int m_maximum = 100;
    /// <summary>
    /// Obtient ou définit la valeur maximale pour la propriété Value
    /// </summary>
    [DefaultValue(typeof(int), "100"), Browsable(true), Description("Obtient et définit la valeur maximale pour la propriété Value.")]
    [RefreshProperties(RefreshProperties.All)]
    public int Maximum
    {
        get => m_maximum;
        set
        {
            if (value != m_maximum)
            {
                m_maximum = value;
                if (m_value > m_maximum)
                    Value = m_maximum;
                if (m_maximum < m_minimum)
                    Minimum = value;
                //if (!DesignMode)
                Refresh();
            }
        }
    }
    //=========================================================================================================
    private int m_value = 0;
    /// <summary>
    /// La valeur actuelle du contrôle VisualInt
    /// </summary>
    [DefaultValue(0), Browsable(true), Description("La valeur actuelle du contrôle VisualInt")]
    public override int Value
    {
        get => m_value;
        set
        {
            if (value < m_minimum || value > m_maximum)
                throw new ArgumentOutOfRangeException("La valeur '" + value + "' n'est pas valide, elle doit être comprise entre 'Minimum' et 'Maximum'");

            m_value = value;
            Refresh();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    //=========================================================================================================
    private void SetValueWithoutValueChanged(int pValue)
    {
        if (pValue < m_minimum || pValue > m_maximum)
            throw new ArgumentOutOfRangeException("La valeur '" + pValue + "' n'est pas valide, elle doit être comprise entre 'Minimum' et 'Maximum'");

        m_value = pValue;
        Refresh();
    }
    #endregion

    #region Méthodes privées
    //=========================================================================================================
    /// <summary>
    /// Dessine le contenu du contrôle.
    /// </summary>
    /// <param name="pGraphics"></param>
    protected override void DrawContent(Graphics pGraphics)
    {
        if (pGraphics == null)
            pGraphics = CreateGraphics();

        Rectangle cellBounds = new(Padding.Left, Padding.Top, Width - (Padding.Left + Padding.Right), Height - (Padding.Top + Padding.Bottom));
        // Étape 1 : On commence par dessiner le fond de la cellule
        //if (BackgroundImage != null)
        //    pGraphics.DrawImage(BackgroundImage, cellBounds, cellBounds, GraphicsUnit.Pixel);
        //else
        //    pGraphics.FillRectangle(new SolidBrush(BackColor), cellBounds);

        //------------------------------------------------------------------------------------------------
        if (ShowIndex && DesignMode) // on va afficher l'index du contrôle dans son conteneur
        {
            VisualArraysTools.DrawText(pGraphics, cellBounds, "[" + Index + "]", ForeColor, Font, m_valueAlign);
        }
        else
        {// affichage normal de la valeur
            pGraphics.SetClip(cellBounds);
            switch (m_view)
            {
                case enuIntView.Number:
                    VisualArraysTools.DrawText(pGraphics, cellBounds, m_value.ToString(), ForeColor, Font, m_valueAlign);
                    break;
                case enuIntView.NumberFraction:
                    VisualArraysTools.DrawText(pGraphics, cellBounds, m_value + "/" + m_maximum, ForeColor, Font, m_valueAlign);
                    break;
                case enuIntView.Graph:
                    VisualArraysTools.DrawBar(pGraphics, cellBounds, GraphAppearance, m_minimum, m_maximum, m_value);
                    break;
                case enuIntView.Digit:
                    VisualArraysTools.DessinerSegments(pGraphics, cellBounds, m_value, ForeColor);
                    break;
                case enuIntView.ImageList:
                    if (m_imageList != null && m_value >= m_imageList.Images.Count)
                        VisualArraysTools.DrawText(pGraphics, cellBounds, m_value.ToString(), ForeColor, Font, m_valueAlign);
                    else
                        DrawValueFromImageList(pGraphics, cellBounds);
                    break;
                case enuIntView.GraphNumber:
                    VisualArraysTools.DrawBar(pGraphics, cellBounds, GraphAppearance, m_minimum, m_maximum, m_value);
                    VisualArraysTools.DrawText(pGraphics, cellBounds, m_value.ToString(), ForeColor, Font, m_valueAlign);
                    break;
                case enuIntView.GraphNumberFraction:
                    VisualArraysTools.DrawBar(pGraphics, cellBounds, GraphAppearance, m_minimum, m_maximum, m_value);
                    VisualArraysTools.DrawText(pGraphics, cellBounds, m_value + "/" + m_maximum, ForeColor, Font, m_valueAlign);
                    break;
                default:
                    break;
            }
            pGraphics.ResetClip();
        }
        base.DrawContent(pGraphics);
    }
    //=========================================================================================================
    private void DrawValueFromImageList(Graphics pGraphics, Rectangle pCellBounds)
    {
        if (m_imageList != null)
        {
            if (m_value >= 0 && m_value < m_imageList.Images.Count)
            {
                Image objImage = m_imageList.Images[m_value];
                Rectangle imageBounds = CellVisualElement.CellVisualElement.BoundsFromAlignment(pCellBounds, objImage.Size, m_valueAlign);
                if (Enabled)
                    pGraphics.DrawImage(objImage, new Point(imageBounds.Left, imageBounds.Top));
                else
                    VisualArraysTools.DrawDisabledImage(pGraphics, imageBounds, objImage, 0.2f);
            }
            else
            {
                pGraphics.FillRectangle(new SolidBrush(BackColor), pCellBounds);
                VisualArraysTools.DrawText(pGraphics, pCellBounds, "?", ForeColor, Font, m_valueAlign);
            }
        }
        else // il n'y pas d'imageList d'associé avec la grille
            VisualArraysTools.DrawText(pGraphics, pCellBounds, "?", ForeColor, Font, m_valueAlign);
    }
    #endregion

    #region Méthodes redéfinies
    //============================================================================================
    /// <summary>
    /// Se produit lorsque l'utilisateur change l'état du contrôle
    /// </summary>
    /// <param name="e"></param>
    protected override void OnEnabledChanged(EventArgs e)
    {
        Refresh();
    }
    //============================================================================================
    /// <summary>
    /// Se produit lorsque l'utilisateur click sur le contrôle
    /// </summary>
    /// <param name="e">MouseEventArgs</param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (ModifierKeys == Keys.Alt)
        {
            if (m_view < enuIntView.GraphNumber)
                View++;
            else
                View = enuIntView.Number;
        }
        else
        {
            if (ContainsFocus && (!ReadOnly))
            {
                switch (m_view)
                {
                    case enuIntView.Number:
                    case enuIntView.ImageList:
                        //if (e.Y < Height / 2)
                        //{
                        //    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        //        if (m_value < m_maximum) Value++;
                        //    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                        //        Value = m_maximum;
                        //}
                        //else
                        //{
                        //    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        //        if (m_value > m_minimum) Value--;
                        //    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                        //        Value = m_minimum;
                        //}
                        break;
                    case enuIntView.Graph:
                    case enuIntView.GraphNumber:
                        Rectangle cellBounds = new(Padding.Left, Padding.Top, Width - (Padding.Left + Padding.Right), Height - (Padding.Top + Padding.Bottom));
                        Value = (int)VisualArraysTools.ValueFromClick(e.Location, cellBounds, GraphAppearance, m_minimum, m_maximum);
                        break;
                    case enuIntView.Digit:
                        break;
                    default:
                        break;
                }
            }
            else
                Focus();
        }
    }
    //============================================================================================
    /// <summary>
    /// Se produit lorsque le MouseWheel change de valeur
    /// </summary>
    /// <param name="e">MouseEventArgs</param>
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (!ReadOnly)
        {
            int delta = (m_maximum - m_minimum) / 10;
            if (delta < 1) delta = 1;
            int valeur = m_value;
            if (e.Delta > 0)
                valeur += delta;
            else if (e.Delta < 0)
                valeur -= delta;
            if (valeur < m_minimum) valeur = m_minimum;
            if (valeur > m_maximum) valeur = m_maximum;
            Value = valeur;
        }
    }
    //============================================================================================
    /// <summary>
    /// Accepte les touches "flèches"
    /// </summary>
    /// <param name="e">infos sur la touche pressée</param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!m_readOnly)
        {
            if (e.KeyCode is Keys.Enter or Keys.Return)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    /// <summary>
    /// Gère la saisie d'un nombre entier
    /// </summary>
    /// <param name="e">KeyPressEventArgs</param>
    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        if (!m_readOnly)
        {
            int valeurObtenue;
            if (VisualArraysTools.ReadInt(e.KeyChar,m_value,m_maximum,out valeurObtenue))
            {
                if (valeurObtenue < m_minimum) valeurObtenue = m_minimum;
                if (valeurObtenue > m_maximum) valeurObtenue = m_maximum;
                if (WaitForEnter)
                    SetValueWithoutValueChanged(valeurObtenue);
                else
                    Value = valeurObtenue;
            }
        }
        base.OnKeyPress(e);
    }
    #endregion

    #region RandomizeValue
    /// <summary>
    /// Assigne une valeur aléatoire à la propriété Value dans l'intervalle : Minimum à Maximum
    /// </summary>
    public void RandomizeValue()
    {
        Value = VisualArraysTools.RandomInt(Minimum, Maximum + 1);
    }
    /// <summary>
    /// Assigne une valeur aléatoire à la propriété Value.
    /// </summary>
    /// <param name="pMinimum">borne inférieure de l'intervalle</param>
    /// <param name="pMaximum">borne supérieure de l'intervalle</param>
    public void RandomizeValue(int pMinimum, int pMaximum)
    {
        if (pMinimum < Minimum)
            throw new ArgumentOutOfRangeException("pMinimum ne peut être inférieure à la propriété Minimum");
        if (pMaximum > Maximum)
            throw new ArgumentOutOfRangeException("pMaximum ne peut être supérieure à la propriété Maximum");

        Value = VisualArraysTools.RandomInt(pMinimum, pMaximum + 1);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    internal override void UpdateVisualElement()
    {
        throw new NotImplementedException();
    }
}
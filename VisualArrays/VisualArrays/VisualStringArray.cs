using System.ComponentModel;
using VisualArrays.Others;

namespace VisualArrays.VisualArrays;

/// <summary>
/// Représente une grille dont les cellules sont chaînes de caractères.  
/// </summary>
[ToolboxBitmap(typeof(VisualStringArray), "Resources.tbxVisualStringArray")]
public partial class VisualStringArray : VisualValueArray<string>
{
    private Dictionary<string, Image> m_dictionnaire = new();

    #region Champs et Propriétés
    //============================================================================================
    //private string m_defaultValue = "text";
    /// <summary>
    /// Obtient ou définit la valeur par défaut de toutes les cellules.
    /// </summary>
    [DefaultValue("text"), Category("VisualArrays"), Browsable(true), Description("Valeur par défaut de toutes les cellules")]
    public string DefaultValue
    {
        get => m_defaultValue;
        set
        {
            if (value != m_defaultValue)
            {
                m_defaultValue = value;
                ResetAllValuesToDefault();
                Refresh();
            }
        }
    }

    //============================================================================================
    //private string m_specialValue = "";
    /// <summary>
    /// Obtient ou définit la valeur spéciale à afficher différement des autres valeurs.
    /// </summary>
    [DefaultValue(""), Category("VisualArrays"), Browsable(true), Description("Valeur spéciale à afficher différemment des autres valeurs, voir SpecialValueAppearance")]
    public string SpecialValue
    {
        get => m_specialValue;
        set
        {
            if (value != m_specialValue)
            {
                m_specialValue = value;
                Refresh();
            }
        }
    }
    //============================================================================================
    private int va_maxLength = 10;
    /// <summary>
    /// Obtient ou définit le nombre maximum de caractères que l'on peut saisir dans les cellules
    /// </summary>
    [DefaultValue(10), Category("VisualArrays"), Browsable(true), Description("Nombre maximum de caractères que l'on peut saisir dans une cellule.")]
    public int MaxLength
    {
        get => va_maxLength;
        set
        {
            va_maxLength = value;
            Refresh();
        }
    }
    //============================================================================================
    private enuStringView va_view;
    /// <summary>
    /// Obtient et définit le style de visualisation pour les valeurs de la grille
    /// </summary>
    [DefaultValue(enuStringView.Text), Category("CellAppearance"), Browsable(true), Description("Obtient et définit le style de visualisation pour les valeurs de la grille.")]
    public enuStringView View
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
    /// <summary>
    /// Instancie une grille visuelle de caractères.
    /// </summary>
    public VisualStringArray()
    {
        m_defaultValue = "text";
        m_specialValue = "";
        ResetAllValuesToDefault();
        InitializeComponent();
    }
    #endregion

    #region OnKeyPress
    //============================================================================================
    /// <summary>
    /// Accepte tous les caractères saisies au clavier jsuqu'à concurrence de MaxLength
    /// </summary>
    /// <param name="e">infos sur la touche pressée</param>
    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        if (SelectedIndex != -1 && !va_readOnly && va_selectionMode != SelectionMode.None && !va_tabCells[SelectedAddress.Row, SelectedAddress.Column].ReadOnly)
        {
            if (e.KeyChar == (char)Keys.Enter) return;
            string valeurCourante;
            string nouvelleValeur;
            if (DateTime.Now.Ticks - va_currentKeyTime.Ticks > DELAI_INTER_TOUCHES)
                valeurCourante = "";
            else
                valeurCourante = this[SelectedIndex];

            va_currentKeyTime = DateTime.Now;
            if (e.KeyChar == (char)Keys.Back)
            {
                if (valeurCourante.Length > 0)
                    nouvelleValeur = valeurCourante.Substring(0, valeurCourante.Length - 1);
                else
                    nouvelleValeur = "";
            }
            else
            {
                nouvelleValeur = valeurCourante + e.KeyChar;
            }
            if (nouvelleValeur.Length <= va_maxLength && View != enuStringView.ImageFile)
                this[SelectedIndex] = nouvelleValeur;
        }
        base.OnKeyPress(e);
    }
    #endregion

    #region DrawCellDragContent, DrawCellContent
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
        pContentBounds = pContentBounds with { X = 0, Y = 0 };

        string valeurAAfficher = va_tabValues[pRow, pColumn];
        // Dessiner le fond de la cellule si nécessaire
        if (valeurAAfficher == m_specialValue)
        {
            va_specialValueVisualElement?.Draw(pGraphics, pContentBounds);
        }
        else
        {
            cell.Background?.Draw(pGraphics, pContentBounds);
        }

        switch (va_view)
        {
            case enuStringView.Text:
                pGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                string laChaine = valeurAAfficher.ToString();
                if (valeurAAfficher != m_specialValue)
                    DrawText(pGraphics, pContentBounds, laChaine, EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                else if (SpecialValueAppearance.ShowValue) // c'est la valeur spéciale
                    DrawText(pGraphics, pContentBounds, laChaine, SpecialValueAppearance.TextColor, SpecialValueAppearance.Font, m_cellContentAlign);
                break;
            case enuStringView.ImageFile:
                // Ne pas afficher la valeur si c'est la valeur spéciale ou si la cellule est inactive et que la propriété Show est false
                if (valeurAAfficher == m_specialValue || (!cell.Enabled && !DisabledAppearance.ShowValue)) break;

                Image imageAAfficher;
                bool imageDejaChargée = m_dictionnaire.TryGetValue(valeurAAfficher, out imageAAfficher);

                if (imageDejaChargée)
                    pGraphics.DrawImage(imageAAfficher, pContentBounds);
                break;
            default:
                break;
        }
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

            string valeurAAfficher = va_tabValues[pRow, pColumn];

            cell.LayerUnder?.Draw(pGraphics, cellContentBounds);

            if (cell.Enabled) // la cellule est active
            {
                if (valeurAAfficher == m_specialValue) // la valeur est spéciale
                {
                    va_specialValueVisualElement?.Draw(pGraphics, cellContentBounds);
                }
                else // la valeur est normale
                {
                    cell.Background?.Draw(pGraphics, cellContentBounds);
                }
            }
            else // la cellule est inactive
            {
                va_disabledVisualElement?.Draw(pGraphics, cellContentBounds);
            }

            #region Code pour dessiner la valeur de la cellule
            Rectangle displayRectangle = GetCellContentBounds(pRow, pColumn, EnabledAppearance.Padding);
            if (!DesignMode || va_addressView == enuAddressView.None)
                switch (va_view)
                {
                    case enuStringView.Text:
                        if (cell.Enabled) // la cellule est active
                        {
                            if (valeurAAfficher == m_specialValue) // c'est la valeur spéciale
                            {
                                if (SpecialValueAppearance.ShowValue)
                                    VisualArraysTools.DrawText(pGraphics, displayRectangle, valeurAAfficher, SpecialValueAppearance.TextColor, SpecialValueAppearance.Font, m_cellContentAlign);
                            }
                            else // la valeur est normale
                            {
                                VisualArraysTools.DrawText(pGraphics, displayRectangle, valeurAAfficher, EnabledAppearance.TextColor, EnabledAppearance.Font, m_cellContentAlign);
                            }
                        }
                        else // la cellule est inactive
                        {
                            if (va_disabledAppearance.ShowValue)
                                VisualArraysTools.DrawText(pGraphics, displayRectangle, valeurAAfficher, DisabledAppearance.TextColor, DisabledAppearance.Font, m_cellContentAlign);
                        }
                        break;
                    case enuStringView.ImageFile:

                        // Ne pas afficher la valeur si c'est la valeur spéciale ou si la cellule est inactive et que la propriété Show est false
                        if (valeurAAfficher == m_specialValue || (!cell.Enabled && !DisabledAppearance.ShowValue)) break;

                        Image imageAAfficher;
                        bool imageDejaChargée = m_dictionnaire.TryGetValue(valeurAAfficher, out imageAAfficher);

                        if (imageDejaChargée)
                        {
                            if (cell.Enabled)
                                pGraphics.DrawImage(imageAAfficher, cellContentBounds);
                            else
                                VisualArraysTools.DrawDisabledImage(pGraphics, cellContentBounds, imageAAfficher, DisabledAppearance.ImageBrightness);
                        }
                        else
                        {
                            if (File.Exists(valeurAAfficher))
                            {
                                try
                                {
                                    imageAAfficher = Image.FromFile(valeurAAfficher);
                                    m_dictionnaire[valeurAAfficher] = imageAAfficher;
                                }
                                catch
                                {
                                    throw new VisualArrayException("Impossible de charger l'image : " + valeurAAfficher);
                                }
                            }
                            else
                                throw new VisualArrayException("Impossible de charger l'image : " + valeurAAfficher);

                            if (cell.Enabled)
                                pGraphics.DrawImage(imageAAfficher, cellContentBounds);
                            else
                                VisualArraysTools.DrawDisabledImage(pGraphics, cellContentBounds, imageAAfficher, DisabledAppearance.ImageBrightness);
                        }
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

}
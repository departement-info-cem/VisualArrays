using System.Drawing.Imaging;
using VisualArrays.Appearance;
using VisualArrays.Others;

namespace VisualArrays;

/// <summary>
/// Contient des méthodes utilitaires concernant les VisualArrays
/// </summary>
public static class VisualArraysTools
{
    #region TestMode et Wait

    /// <summary>
    /// Détermine si les VisualArrays sont actuellement exécutés dans un environnement de tests.
    /// </summary>
    private const bool TEST_MODE = false;

    //------------------------------------------------------------------------------------------
    /// <summary>
    /// Permet d'attendre un délai spécifié par le paramètre pDelai.
    /// </summary>
    /// <param name="pDelai">Le délai à attendre.</param>
    public static void Wait(int pDelai)
    {
        if (TEST_MODE || pDelai <= 0) return;
        DateTime maintenantPlusDelai = DateTime.Now.AddMilliseconds(pDelai);
        while (DateTime.Now < maintenantPlusDelai) ;
    }
    #endregion

    #region NbDigits, ReadInt
    //============================================================================================
    /// <summary>
    /// Fournit le nombre de caractères nécessaires pour réprésenter ce nombre
    /// </summary>
    /// <param name="pValeur"></param>
    /// <returns></returns>
    private static int NbDigits(int pValeur)
    {
        int cpt = 0;
        if (pValeur == 0) cpt = 1;
        else
            while (pValeur > 0)
            {
                cpt++;
                pValeur /= 10;
            }
        if (pValeur < 0)
            return cpt + 1;
        return cpt;
    }
    //================================================================================================================
    /// <summary>
    /// Permet de déterminer le délai entre deux touches du clavier.
    /// </summary>
    static DateTime m_currentKeyTime;
    static bool m_negatif = false;
    /// <summary>
    /// Délai acceptable entre les touches.
    /// </summary>
    const int DELAI_INTER_TOUCHES = 10000000;
    //================================================================================================================
    internal static bool ReadInt(char pChar, int pCurrentValue, int pMaxValue, out int pNewValue)
    {
        pNewValue = 0;
        if (pChar == (char)Keys.Enter) m_currentKeyTime = new DateTime(0);
        if (!((pChar >= '0' && pChar <= '9') || pChar == '-'))
            return false;

        int oldValue = pCurrentValue;
        pNewValue = Math.Abs(pCurrentValue);
        if (NbDigits(pNewValue) == NbDigits(pMaxValue))
            pNewValue = 0;
        if (pChar == '-')
        {
            m_negatif = true;
            m_currentKeyTime = DateTime.Now;
            pNewValue = 0;
        }
        if (DateTime.Now.Ticks - m_currentKeyTime.Ticks > DELAI_INTER_TOUCHES)
        {
            pNewValue = 0;
            m_negatif = false;
        }
        if (pChar >= '0' && pChar <= '9')
        {
            pNewValue = pNewValue * 10 + pChar - '0';
            pNewValue = m_negatif ? -pNewValue : pNewValue;
            m_currentKeyTime = DateTime.Now;
        }
        return oldValue != pNewValue;
    }

    //============================================================================================
    /// <summary>
    /// Fournit le nombre de caractères nécessaires pour réprésenter ce nombre
    /// </summary>
    /// <param name="pValeur"></param>
    /// <param name="pDecimalPlaces"></param>
    /// <returns></returns>
    internal static int NbDigits(decimal pValeur, int pDecimalPlaces)
    {
        long nombreEntier = (long)Math.Floor(pValeur);
        int cpt = 0;
        if (nombreEntier == 0) cpt = 1;
        else
            while (nombreEntier > 0)
            {
                cpt++;
                nombreEntier /= 10;
            }
        if (pDecimalPlaces == 0)
            return cpt;
        return cpt + pDecimalPlaces + 2;
    }
    //================================================================================================================
    private static bool m_fraction = false;
    private static int m_digitFraction = 0;
    //================================================================================================================
    internal static bool ReadDecimal(char pChar, decimal pCurrentValue, decimal pMaxValue, int pDecimalPlaces, out decimal pNewValue)
    {
        pNewValue = 0;
        if (pChar == (char)Keys.Enter) m_currentKeyTime = new DateTime(0);
        if (!(pChar == '.' || pChar == ',' || (pChar >= '0' && pChar <= '9') || pChar == '-'))
            return false;

        decimal oldValeur = pCurrentValue;
        pNewValue = Math.Abs(pCurrentValue);
        if (VisualArraysTools.NbDigits(pNewValue, pDecimalPlaces) == VisualArraysTools.NbDigits(pMaxValue, pDecimalPlaces))
            pNewValue = 0;
        if ((pChar == '.' || pChar == ',') && !m_fraction)
        {
            m_fraction = true;
            m_digitFraction = 0;
        }
        if (pChar == '-')
        {
            m_negatif = true;
            m_currentKeyTime = DateTime.Now;
            pNewValue = 0;
            m_fraction = false;
            m_digitFraction = 0;
        }
        if (DateTime.Now.Ticks - m_currentKeyTime.Ticks > DELAI_INTER_TOUCHES)
        {
            pNewValue = 0;
            m_fraction = false;
            m_negatif = false;
            m_digitFraction = 0;
        }
        if (pChar >= '0' && pChar <= '9')
        {
            if (m_fraction)
            {
                m_digitFraction++;
                if (m_digitFraction > pDecimalPlaces) return false;
                pNewValue = pNewValue * (decimal)Math.Pow(10, m_digitFraction);
                pNewValue += pChar - '0';
                pNewValue = pNewValue / (decimal)Math.Pow(10, m_digitFraction);
            }
            else
                pNewValue = pNewValue * 10 + pChar - '0';

            pNewValue = m_negatif ? -pNewValue : pNewValue;
            m_currentKeyTime = DateTime.Now;
        }
        return oldValeur != pNewValue;
    }

    #endregion

    #region DrawDisabledImage
    //================================================================================================================
    /// <summary>
    /// Dessine une image dont la luminosité est ajustable entre 0 (très pâle) et 1 (très claire)
    /// </summary>
    /// <param name="pGraphics"></param>
    /// <param name="pRect"></param>
    /// <param name="pImage"></param>
    /// <param name="pBrightness"></param>
    internal static void DrawDisabledImage(Graphics pGraphics, Rectangle pRect, Image pImage, float pBrightness)
    {
        Bitmap bitmap = new(pImage);

        // Create an array of matrix points
        float[][] ptsArray =
        {
            new float[] {1, 0, 0, 0, 0},
            new float[] {0, 1, 0, 0, 0},
            new float[] {0, 0, 1, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}};

        ptsArray[3][3] = pBrightness;
        // Create a color matrix
        ColorMatrix clrMatrix = new(ptsArray);
        //pGraphics.Clear(BackColor);

        // Create image attributes
        ImageAttributes imgAttributes = new();

        // Set color matrix
        imgAttributes.SetColorMatrix(clrMatrix,
            ColorMatrixFlag.Default,
            ColorAdjustType.Bitmap);

        // Draw image
        pGraphics.DrawImage(bitmap, pRect, 0, 0, bitmap.Width, bitmap.Height,
            GraphicsUnit.Pixel, imgAttributes);

        bitmap.Dispose();
    }

    #endregion

    #region CalculerCouleurEteinte,CalculerCouleurAllumée,DessinerSegments
    //============================================================================================
    /// <summary>
    /// Recalcule la couleur afin d'atténuer la couleur d'une cellule éteinte.
    /// </summary>
    /// <param name="pCouleur">Couleur à atténuer.</param>
    /// <returns></returns>
    internal static Color CalculerCouleurEteinte(Color pCouleur)
    {
        int couleur = pCouleur.ToArgb() & (Convert.ToInt32(Math.Pow(2, 24)) - 1);
        int b = (couleur & 255) / 3;
        int v = ((couleur / 256) & 255) / 3;
        int r = ((couleur / 65536) & 255) / 3;
        return Color.FromArgb(255, r, v, b);
    }
    //============================================================================================
    /// <summary>
    /// Recalcule la couleur afin d'éclaircir une couleur sombre.
    /// </summary>
    /// <param name="pCouleur">Couleur à atténuer.</param>
    /// <returns></returns>
    internal static Color CalculerCouleurAllumée(Color pCouleur)
    {
        int couleur = pCouleur.ToArgb() & (Convert.ToInt32(Math.Pow(2, 24)) - 1);
        int b = (couleur & 255) * 3 & 255;
        int v = ((couleur / 256) & 255) * 3 & 255;
        int r = ((couleur / 65536) & 255) * 3 & 255;
        return Color.FromArgb(255, r, v, b);
    }
    //================================================================================================================
    /// <summary>
    /// Dessiner le contenu d'une cellule en mode view Digit
    /// </summary>
    /// <param name="pGraphics">Graphique pour dessiner</param>
    /// <param name="pContentBounds">Contour de la cellule</param>
    /// <param name="pValue">Valeur à afficher</param>
    /// <param name="pColor">Couleur des segments</param>
    internal static void DessinerSegments(Graphics pGraphics, Rectangle pContentBounds, int pValue, Color pColor)
    {
        int cellsWidth = pContentBounds.Width, cellsHeight = pContentBounds.Height;
        int positionX = pContentBounds.X;
        int positionY = pContentBounds.Y;
        int hauteurMilieu = cellsHeight / 2;
        Pen objPenBrillant = new(pColor);
        Pen objPenSombre = new(CalculerCouleurEteinte(pColor));
        // Segment # 1 ================================================================
        if ((pValue & 1) > 0)
        {
            pGraphics.DrawLine(objPenBrillant, positionX, positionY + 2, positionX, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenBrillant, positionX + 1, positionY + 2, positionX + 1, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenBrillant, positionX + 2, positionY + 3, positionX + 2, positionY + hauteurMilieu - 2);
            pGraphics.DrawLine(objPenBrillant, positionX + 3, positionY + 4, positionX + 3, positionY + hauteurMilieu - 3);
            pGraphics.DrawLine(objPenBrillant, positionX + 4, positionY + 5, positionX + 4, positionY + hauteurMilieu - 4);
        }
        else
        {
            pGraphics.DrawLine(objPenSombre, positionX, positionY + 2, positionX, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenSombre, positionX + 1, positionY + 2, positionX + 1, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenSombre, positionX + 2, positionY + 3, positionX + 2, positionY + hauteurMilieu - 2);
            pGraphics.DrawLine(objPenSombre, positionX + 3, positionY + 4, positionX + 3, positionY + hauteurMilieu - 3);
            pGraphics.DrawLine(objPenSombre, positionX + 4, positionY + 5, positionX + 4, positionY + hauteurMilieu - 4);
        }
        // Segment # 2 ================================================================
        positionY = positionY + hauteurMilieu;
        if ((pValue & 2) > 0)
        {
            pGraphics.DrawLine(objPenBrillant, positionX, positionY + 2, positionX, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenBrillant, positionX + 1, positionY + 2, positionX + 1, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenBrillant, positionX + 2, positionY + 3, positionX + 2, positionY + hauteurMilieu - 2);
            pGraphics.DrawLine(objPenBrillant, positionX + 3, positionY + 4, positionX + 3, positionY + hauteurMilieu - 3);
            pGraphics.DrawLine(objPenBrillant, positionX + 4, positionY + 5, positionX + 4, positionY + hauteurMilieu - 4);
        }
        else
        {
            pGraphics.DrawLine(objPenSombre, positionX, positionY + 2, positionX, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenSombre, positionX + 1, positionY + 2, positionX + 1, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenSombre, positionX + 2, positionY + 3, positionX + 2, positionY + hauteurMilieu - 2);
            pGraphics.DrawLine(objPenSombre, positionX + 3, positionY + 4, positionX + 3, positionY + hauteurMilieu - 3);
            pGraphics.DrawLine(objPenSombre, positionX + 4, positionY + 5, positionX + 4, positionY + hauteurMilieu - 4);
        }
        //  Segment # 3 ================================================================
        positionY = positionY - hauteurMilieu;
        if ((pValue & 4) > 0)
        {
            pGraphics.DrawLine(objPenBrillant, positionX + 4, positionY, positionX + cellsWidth - 5, positionY);
            pGraphics.DrawLine(objPenBrillant, positionX + 4, positionY + 1, positionX + cellsWidth - 5, positionY + 1);
            pGraphics.DrawLine(objPenBrillant, positionX + 5, positionY + 2, positionX + cellsWidth - 6, positionY + 2);
            pGraphics.DrawLine(objPenBrillant, positionX + 6, positionY + 3, positionX + cellsWidth - 7, positionY + 3);
            pGraphics.DrawLine(objPenBrillant, positionX + 7, positionY + 4, positionX + cellsWidth - 8, positionY + 4);
        }
        else
        {
            pGraphics.DrawLine(objPenSombre, positionX + 4, positionY, positionX + cellsWidth - 5, positionY);
            pGraphics.DrawLine(objPenSombre, positionX + 4, positionY + 1, positionX + cellsWidth - 5, positionY + 1);
            pGraphics.DrawLine(objPenSombre, positionX + 5, positionY + 2, positionX + cellsWidth - 6, positionY + 2);
            pGraphics.DrawLine(objPenSombre, positionX + 6, positionY + 3, positionX + cellsWidth - 7, positionY + 3);
            pGraphics.DrawLine(objPenSombre, positionX + 7, positionY + 4, positionX + cellsWidth - 8, positionY + 4);
        }
        //  Segment # 4 ================================================================
        if ((pValue & 8) > 0)
        {
            pGraphics.DrawLine(objPenBrillant, positionX + 5, positionY + hauteurMilieu - 2, positionX + cellsWidth - 6, positionY + hauteurMilieu - 2);
            pGraphics.DrawLine(objPenBrillant, positionX + 4, positionY + hauteurMilieu - 1, positionX + cellsWidth - 5, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenBrillant, positionX + 4, positionY + hauteurMilieu, positionX + cellsWidth - 5, positionY + hauteurMilieu);
            pGraphics.DrawLine(objPenBrillant, positionX + 4, positionY + hauteurMilieu + 1, positionX + cellsWidth - 5, positionY + hauteurMilieu + 1);
            pGraphics.DrawLine(objPenBrillant, positionX + 5, positionY + hauteurMilieu + 2, positionX + cellsWidth - 6, positionY + hauteurMilieu + 2);
        }
        else
        {
            pGraphics.DrawLine(objPenSombre, positionX + 5, positionY + hauteurMilieu - 2, positionX + cellsWidth - 6, positionY + hauteurMilieu - 2);
            pGraphics.DrawLine(objPenSombre, positionX + 4, positionY + hauteurMilieu - 1, positionX + cellsWidth - 5, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenSombre, positionX + 4, positionY + hauteurMilieu, positionX + cellsWidth - 5, positionY + hauteurMilieu);
            pGraphics.DrawLine(objPenSombre, positionX + 4, positionY + hauteurMilieu + 1, positionX + cellsWidth - 5, positionY + hauteurMilieu + 1);
            pGraphics.DrawLine(objPenSombre, positionX + 5, positionY + hauteurMilieu + 2, positionX + cellsWidth - 6, positionY + hauteurMilieu + 2);
        }
        // Segment # 5 ================================================================
        if ((pValue & 16) > 0)
        {
            pGraphics.DrawLine(objPenBrillant, positionX + 4, positionY + cellsHeight, positionX + cellsWidth - 5, positionY + cellsHeight);
            pGraphics.DrawLine(objPenBrillant, positionX + 4, positionY + cellsHeight - 1, positionX + cellsWidth - 5, positionY + cellsHeight - 1);
            pGraphics.DrawLine(objPenBrillant, positionX + 5, positionY + cellsHeight - 2, positionX + cellsWidth - 6, positionY + cellsHeight - 2);
            pGraphics.DrawLine(objPenBrillant, positionX + 6, positionY + cellsHeight - 3, positionX + cellsWidth - 7, positionY + cellsHeight - 3);
            pGraphics.DrawLine(objPenBrillant, positionX + 7, positionY + cellsHeight - 4, positionX + cellsWidth - 8, positionY + cellsHeight - 4);
        }
        else
        {
            pGraphics.DrawLine(objPenSombre, positionX + 4, positionY + cellsHeight, positionX + cellsWidth - 5, positionY + cellsHeight);
            pGraphics.DrawLine(objPenSombre, positionX + 4, positionY + cellsHeight - 1, positionX + cellsWidth - 5, positionY + cellsHeight - 1);
            pGraphics.DrawLine(objPenSombre, positionX + 5, positionY + cellsHeight - 2, positionX + cellsWidth - 6, positionY + cellsHeight - 2);
            pGraphics.DrawLine(objPenSombre, positionX + 6, positionY + cellsHeight - 3, positionX + cellsWidth - 7, positionY + cellsHeight - 3);
            pGraphics.DrawLine(objPenSombre, positionX + 7, positionY + cellsHeight - 4, positionX + cellsWidth - 8, positionY + cellsHeight - 4);
        }
        // Segment # 6 ================================================================
        if ((pValue & 32) > 0)
        {
            pGraphics.DrawLine(objPenBrillant, positionX + cellsWidth - 1, positionY + 2, positionX + cellsWidth - 1, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenBrillant, positionX + cellsWidth - 2, positionY + 2, positionX + cellsWidth - 2, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenBrillant, positionX + cellsWidth - 3, positionY + 3, positionX + cellsWidth - 3, positionY + hauteurMilieu - 2);
            pGraphics.DrawLine(objPenBrillant, positionX + cellsWidth - 4, positionY + 4, positionX + cellsWidth - 4, positionY + hauteurMilieu - 3);
            pGraphics.DrawLine(objPenBrillant, positionX + cellsWidth - 5, positionY + 5, positionX + cellsWidth - 5, positionY + hauteurMilieu - 4);
        }
        else
        {
            pGraphics.DrawLine(objPenSombre, positionX + cellsWidth - 1, positionY + 2, positionX + cellsWidth - 1, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenSombre, positionX + cellsWidth - 2, positionY + 2, positionX + cellsWidth - 2, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenSombre, positionX + cellsWidth - 3, positionY + 3, positionX + cellsWidth - 3, positionY + hauteurMilieu - 2);
            pGraphics.DrawLine(objPenSombre, positionX + cellsWidth - 4, positionY + 4, positionX + cellsWidth - 4, positionY + hauteurMilieu - 3);
            pGraphics.DrawLine(objPenSombre, positionX + cellsWidth - 5, positionY + 5, positionX + cellsWidth - 5, positionY + hauteurMilieu - 4);
        }
        // Segment # 7 ================================================================
        positionY = positionY + hauteurMilieu;
        if ((pValue & 64) > 0)
        {
            pGraphics.DrawLine(objPenBrillant, positionX + cellsWidth - 1, positionY + 2, positionX + cellsWidth - 1, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenBrillant, positionX + cellsWidth - 2, positionY + 2, positionX + cellsWidth - 2, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenBrillant, positionX + cellsWidth - 3, positionY + 3, positionX + cellsWidth - 3, positionY + hauteurMilieu - 2);
            pGraphics.DrawLine(objPenBrillant, positionX + cellsWidth - 4, positionY + 4, positionX + cellsWidth - 4, positionY + hauteurMilieu - 3);
            pGraphics.DrawLine(objPenBrillant, positionX + cellsWidth - 5, positionY + 5, positionX + cellsWidth - 5, positionY + hauteurMilieu - 4);
        }
        else
        {
            pGraphics.DrawLine(objPenSombre, positionX + cellsWidth - 1, positionY + 2, positionX + cellsWidth - 1, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenSombre, positionX + cellsWidth - 2, positionY + 2, positionX + cellsWidth - 2, positionY + hauteurMilieu - 1);
            pGraphics.DrawLine(objPenSombre, positionX + cellsWidth - 3, positionY + 3, positionX + cellsWidth - 3, positionY + hauteurMilieu - 2);
            pGraphics.DrawLine(objPenSombre, positionX + cellsWidth - 4, positionY + 4, positionX + cellsWidth - 4, positionY + hauteurMilieu - 3);
            pGraphics.DrawLine(objPenSombre, positionX + cellsWidth - 5, positionY + 5, positionX + cellsWidth - 5, positionY + hauteurMilieu - 4);
        }
        // Segment # 8 ' Pour faire un point
        positionY = positionY - hauteurMilieu;
        positionX = positionX + cellsWidth;
        if ((pValue & 128) > 0)
        {
            objPenBrillant.Width = 6;
            pGraphics.DrawLine(objPenBrillant, positionX - 13, positionY + cellsHeight - 8, positionX - 7, positionY + cellsHeight - 8);
        }
        else
        {
            objPenSombre.Width = 6;
            pGraphics.DrawLine(objPenSombre, positionX - 13, positionY + cellsHeight - 8, positionX - 7, positionY + cellsHeight - 8);
        }
    }

    #endregion

    #region DrawText
    /// <summary>
    /// Dessine une chaîne dans un rectangle donné.
    /// </summary>
    /// <param name="pGraphics">Objet graphique où dessiner.</param>
    /// <param name="pContentBounds">Rectangle destinataire.</param>
    /// <param name="pTexte">Texte à dessiner.</param>
    /// <param name="pCouleur">Couleur du texte.</param>
    /// <param name="pPolice">Police du texte.</param>
    /// <param name="pAlignement">Alignement du texte.</param>
    internal static void DrawText(Graphics pGraphics, Rectangle pContentBounds, string pTexte, Color pCouleur, Font pPolice, ContentAlignment pAlignement)
    {
        StringFormat format = new();
        format.LineAlignment = StringAlignment.Center;
        format.Alignment = StringAlignment.Center;

        switch (pAlignement)
        {
            case ContentAlignment.BottomCenter:
                format.LineAlignment = StringAlignment.Far;
                format.Alignment = StringAlignment.Center;
                break;
            case ContentAlignment.BottomLeft:
                format.LineAlignment = StringAlignment.Far;
                format.Alignment = StringAlignment.Near;
                break;
            case ContentAlignment.BottomRight:
                format.LineAlignment = StringAlignment.Far;
                format.Alignment = StringAlignment.Far;
                break;
            case ContentAlignment.MiddleCenter:
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                break;
            case ContentAlignment.MiddleLeft:
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Near;
                break;
            case ContentAlignment.MiddleRight:
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Far;
                break;
            case ContentAlignment.TopCenter:
                format.LineAlignment = StringAlignment.Near;
                format.Alignment = StringAlignment.Center;
                break;
            case ContentAlignment.TopLeft:
                format.LineAlignment = StringAlignment.Near;
                format.Alignment = StringAlignment.Near;
                break;
            case ContentAlignment.TopRight:
                format.LineAlignment = StringAlignment.Near;
                format.Alignment = StringAlignment.Far;
                break;
            default:
                break;
        }

        pGraphics.DrawString(pTexte, pPolice,
            new SolidBrush(pCouleur), (RectangleF)pContentBounds, format);
    }
    #endregion

    #region DrawBar
    /// <summary>
    /// Dessine une barre pour un graphique
    /// </summary>
    /// <param name="pGraphics"></param>
    /// <param name="pBounds"></param>
    /// <param name="pGraphApp"></param>
    /// <param name="pMinimum"></param>
    /// <param name="pMaximum"></param>
    /// <param name="pValue"></param>
    /// <param name="pDecimalPlaces"></param>
    internal static void DrawBar(Graphics pGraphics, Rectangle pBounds, GraphAppearance pGraphApp, decimal pMinimum, decimal pMaximum, decimal pValue,int pDecimalPlaces)
    {
        decimal plageDeValeurs = pMaximum - pMinimum;
        Orientation orientation = Orientation.Horizontal;
        Rectangle barBounds = pBounds;
        if (plageDeValeurs > 0)
        {
            decimal valeurOrigine = pValue - pMinimum;

            #region Selon : BarMargin
            // on va appliquer les marges sur la taille du graphique
            barBounds.X += pGraphApp.BarMargin.Left;
            barBounds.Width -= pGraphApp.BarMargin.Horizontal;
            barBounds.Y += pGraphApp.BarMargin.Top;
            barBounds.Height -= pGraphApp.BarMargin.Vertical;
            #endregion

            #region Orientation selon la taille

            if (barBounds.Width > barBounds.Height)
                orientation = Orientation.Horizontal;
            else
                orientation = Orientation.Vertical;

            #endregion

            #region Selon : BarAlign
            switch (pGraphApp.BarAlign)
            {
                case enuBarAlign.Normal:
                    if (orientation == Orientation.Horizontal)
                        barBounds.Width = (int)(barBounds.Width * valeurOrigine / plageDeValeurs);
                    else
                    {
                        orientation = Orientation.Vertical;
                        int hauteurBarre = (int)(barBounds.Height * valeurOrigine / plageDeValeurs);
                        barBounds.Y = barBounds.Y + barBounds.Height - hauteurBarre;
                    }
                    break;
                case enuBarAlign.Reverse:
                    if (orientation == Orientation.Horizontal)
                    {
                        int largeur = (int)(barBounds.Width * valeurOrigine / plageDeValeurs);
                        barBounds.X += barBounds.Width - largeur;
                        barBounds.Width = largeur;
                    }
                    else
                        barBounds.Height = (int)(barBounds.Height * valeurOrigine / plageDeValeurs);
                    break;
                case enuBarAlign.Middle:
                    if (orientation == Orientation.Horizontal)
                    {
                        int largeur = (int)(barBounds.Width * valeurOrigine / plageDeValeurs);
                        barBounds.X += (barBounds.Width - largeur) >> 1;
                        barBounds.Width = largeur;
                    }
                    else
                    {
                        int hauteurBarre = (int)(barBounds.Height * valeurOrigine / plageDeValeurs);
                        barBounds.Y += (barBounds.Height - hauteurBarre) >> 1;
                        barBounds.Height = hauteurBarre;
                    }
                    break;
            }
            #endregion

            #region Selon : BarStyle
            switch (pGraphApp.BarStyle)
            {
                case enuGraphBarStyle.Image:
                    if (pGraphApp.BarImage != null)
                        pGraphics.DrawImage(pGraphApp.BarImage, barBounds);
                    else
                        pGraphics.FillRectangle(new SolidBrush(pGraphApp.BarColor), barBounds);
                    break;
                case enuGraphBarStyle.FillShape:
                    pGraphics.FillRectangle(new SolidBrush(pGraphApp.BarColor), barBounds);
                    break;
                default:
                    pGraphics.FillRectangle(new SolidBrush(pGraphApp.BarColor), barBounds);
                    break;
            }
            #endregion

            #region Affichage de la valeur de la barre
            string texte = pValue.ToString("F" + pDecimalPlaces);
            if (pGraphApp.BarValueStyle != enuGraphValueStyle.None) // affichage de la valeur de la barre du graphique
            {
                if (pGraphApp.BarValueStyle == enuGraphValueStyle.Pourcent)
                    texte += "%";

                SizeF taille = pGraphics.MeasureString(texte, pGraphApp.BarValueFont);

                PointF pt;
                if (orientation == Orientation.Horizontal)
                    pt = new PointF(barBounds.Right, barBounds.Top + (int)(((float)barBounds.Height - taille.Height) / 2));
                else // orientation Verticale
                    pt = new PointF(barBounds.Left + (int)(((float)barBounds.Width - taille.Width) / 2), barBounds.Top - taille.Height);

                pGraphics.DrawString(texte, pGraphApp.BarValueFont, new SolidBrush(pGraphApp.BarValueColor), pt);
            }
            #endregion
        }
    }
    //================================================================================================================
    internal static decimal ValueFromClick(Point pLocation, Rectangle pBounds, GraphAppearance pGraphApp, decimal pMinimum, decimal pMaximum)
    {
        decimal plageDeValeurs = pMaximum - pMinimum;
        Rectangle barBounds = pBounds;
        decimal clickValue = pMinimum;
        if (plageDeValeurs > 0)
        {
            // on va appliquer les marges sur la taille du graphique
            barBounds.X += pGraphApp.BarMargin.Left;
            barBounds.Width -= pGraphApp.BarMargin.Horizontal;
            barBounds.Y += pGraphApp.BarMargin.Top;
            barBounds.Height -= pGraphApp.BarMargin.Vertical;

            if (barBounds.Width > barBounds.Height)
            {
                clickValue = pMinimum + plageDeValeurs * (pLocation.X - barBounds.X) / barBounds.Width;
            }
            else
            {
                clickValue = pMaximum - plageDeValeurs * (pLocation.Y - barBounds.Y) / barBounds.Height;
            }

            if (clickValue < pMinimum)
                clickValue = pMinimum;
            if (clickValue > pMaximum)
                clickValue = pMaximum;
        }
        return clickValue;
    }
    //================================================================================================================
    /// <summary>
    /// Dessine une barre pour un graphique
    /// </summary>
    /// <param name="pGraphics"></param>
    /// <param name="pBounds"></param>
    /// <param name="pGraphApp"></param>
    /// <param name="pMinimum"></param>
    /// <param name="pMaximum"></param>
    /// <param name="pValue"></param>
    internal static void DrawBar(Graphics pGraphics, Rectangle pBounds, GraphAppearance pGraphApp, int pMinimum, int pMaximum, int pValue)
    {
        int plageDeValeurs = pMaximum - pMinimum;
        Orientation orientation = Orientation.Horizontal;
        Rectangle barBounds = pBounds;
        if (plageDeValeurs > 0)
        {
            decimal valeurOrigine = pValue - pMinimum;

            #region Selon : BarMargin
            barBounds.X += pGraphApp.BarMargin.Left;
            barBounds.Width -= pGraphApp.BarMargin.Horizontal;
            barBounds.Y += pGraphApp.BarMargin.Top;
            barBounds.Height -= pGraphApp.BarMargin.Vertical;
            #endregion

            #region Orientation selon la taille

            if (barBounds.Width > barBounds.Height)
                orientation = Orientation.Horizontal;
            else
                orientation = Orientation.Vertical;

            #endregion

            #region Selon : BarAlign
            switch (pGraphApp.BarAlign)
            {
                case enuBarAlign.Normal:
                    if (orientation == Orientation.Horizontal)
                        barBounds.Width = (int)(barBounds.Width * valeurOrigine / plageDeValeurs);
                    else
                    {
                        orientation = Orientation.Vertical;
                        int hauteurBarre = (int)(barBounds.Height * valeurOrigine / plageDeValeurs);
                        barBounds.Y = barBounds.Y + barBounds.Height - hauteurBarre;
                    }
                    break;
                case enuBarAlign.Reverse:
                    if (orientation == Orientation.Horizontal)
                    {
                        int largeur = (int)(barBounds.Width * valeurOrigine / plageDeValeurs);
                        barBounds.X += barBounds.Width - largeur;
                        barBounds.Width = largeur;
                    }
                    else
                        barBounds.Height = (int)(barBounds.Height * valeurOrigine / plageDeValeurs);
                    break;
                case enuBarAlign.Middle:
                    if (orientation == Orientation.Horizontal)
                    {
                        int largeur = (int)(barBounds.Width * valeurOrigine / plageDeValeurs);
                        barBounds.X += (barBounds.Width - largeur) >> 1;
                        barBounds.Width = largeur;
                    }
                    else
                    {
                        int hauteurBarre = (int)(barBounds.Height * valeurOrigine / plageDeValeurs);
                        barBounds.Y += (barBounds.Height - hauteurBarre) >> 1;
                        barBounds.Height = hauteurBarre;
                    }
                    break;
            }
            #endregion

            #region Selon : BarStyle
            switch (pGraphApp.BarStyle)
            {
                case enuGraphBarStyle.Image:
                    if (pGraphApp.BarImage != null)
                        pGraphics.DrawImage(pGraphApp.BarImage, barBounds);
                    else
                        pGraphics.FillRectangle(new SolidBrush(pGraphApp.BarColor), barBounds);
                    break;
                case enuGraphBarStyle.FillShape:
                    pGraphics.FillRectangle(new SolidBrush(pGraphApp.BarColor), barBounds);
                    break;
                default:
                    pGraphics.FillRectangle(new SolidBrush(pGraphApp.BarColor), barBounds);
                    break;
            }
            #endregion

            #region Affichage de la valeur de la barre
            string texte = pValue.ToString();
            if (pGraphApp.BarValueStyle != enuGraphValueStyle.None) // affichage de la valeur de la barre du graphique
            {
                if (pGraphApp.BarValueStyle == enuGraphValueStyle.Pourcent)
                    texte += "%";
                SizeF taille = pGraphics.MeasureString(texte, pGraphApp.BarValueFont);

                PointF pt;
                if (orientation == Orientation.Horizontal)
                    pt = new PointF(barBounds.Right, barBounds.Top + (int)(((float)barBounds.Height - taille.Height) / 2));
                else // orientation Verticale
                    pt = new PointF(barBounds.Left + (int)(((float)barBounds.Width - taille.Width) / 2), barBounds.Top - taille.Height);

                pGraphics.DrawString(texte, pGraphApp.BarValueFont, new SolidBrush(pGraphApp.BarValueColor), pt);
            }
            #endregion
        }
    }
    #endregion

    #region RandomInt, RandomChar, RandomBool
    /// <summary>
    /// Utilisé pour les méthodes des grilles produisants des nombres aléatoires.
    /// </summary>
    private static readonly Random va_objRandom = new((int)DateTime.Now.Ticks);
    //------------------------------------------------------------------------------------------
    /// <summary>
    /// Génère un nombre aléatoire décimale.
    /// </summary>
    /// <param name="pMin">Borne inférieure inclue dans l'intervalle</param>
    /// <param name="pMax">Borne supérieure exlue de l'intervalle</param>
    /// <returns>Un nombre decimal aléatoire entre pMin et pMax.</returns>
    public static decimal RandomDecimal(decimal pMin, decimal pMax)
    {
        double min = (double)pMin;
        double max = (double)pMax;
        double nombreAleatoire = (max - min) * va_objRandom.NextDouble() + min;
        return (decimal)nombreAleatoire;
    }
    //------------------------------------------------------------------------------------------
    /// <summary>
    /// Génère un nombre aléatoire entier.
    /// </summary>
    /// <param name="pMin">Borne inférieure inclue dans l'intervalle</param>
    /// <param name="pMax">Borne supérieure exlue de l'intervalle</param>
    /// <returns>Un nombre aléatoire entre pMin et pMax - 1.</returns>
    public static int RandomInt(int pMin, int pMax)
    {
        return va_objRandom.Next(pMin, pMax);
    }
    //------------------------------------------------------------------------------------------
    /// <summary>
    /// Génère un caractère aléatoire compris entre pMinChar et pMaxChar.
    /// </summary>
    /// <param name="pMin">Borne inférieure inclue dans l'intervalle</param>
    /// <param name="pMax">Borne supérieure inclue de l'intervalle</param>
    /// <returns>Un caractère aléatoire entre pMin et pMax inclusivement</returns>
    public static char RandomChar(char pMin, char pMax)
    {
        return (char)va_objRandom.Next(pMin, pMax + 1);
    }
    //------------------------------------------------------------------------------------------
    /// <summary>
    /// Génère une valeur booléenne aléatoire.
    /// </summary>
    /// <returns>Une valeur booléenne aléatoire</returns>
    public static bool RandomBool()
    {
        return va_objRandom.NextDouble() < 0.5;
    }
    #endregion
}
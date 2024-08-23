using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// À AJOUTER :
//      - StrikeAppearance pour Enabled == false ???
//      - Événement ValueChanged...
namespace VisualArrays.VisualCells
{
    /// <summary>
    /// Représente une cellule contenant un nombre entier. 
    /// </summary>
    public partial class VisualDecimal : VisualGraph<decimal>
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
        public VisualDecimal()
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
        [Category("VisualDecimal"), Browsable(true), DefaultValue(null), Description("ImageList utilisée pour dessiner la valeur en mode View ImageList")]
        public ImageList ImageList
        {
            get { return m_imageList; }
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
        private enuDecimalView m_view = enuDecimalView.Number;
        /// <summary>
        /// Obtient et définit le style de visualisation la valeur du contrôle
        /// </summary>
        [Category("VisualDecimal"), DefaultValue(enuDecimalView.Number), Browsable(true), Description("Obtient et définit le style de visualisation pour la valeur du contrôle.")]
        public enuDecimalView View
        {
            get { return m_view; }
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
        private decimal m_minimum = 0;
        /// <summary>
        /// Obtient et définit la valeur minimale pour la propriété Value
        /// </summary>
        [Category("VisualDecimal"), DefaultValue(typeof(decimal), "0"), Browsable(true), Description("Obtient et définit la valeur minimale pour la propriété Value")]
        [RefreshProperties(RefreshProperties.All)]
        public decimal Minimum
        {
            get
            {
                return m_minimum;
            }
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
        private decimal m_maximum = 100;
        /// <summary>
        /// Obtient ou définit la valeur maximale pour la propriété Value
        /// </summary>
        [Category("VisualDecimal"), DefaultValue(typeof(decimal), "100"), Browsable(true), Description("Obtient et définit la valeur maximale pour la propriété Value.")]
        [RefreshProperties(RefreshProperties.All)]
        public decimal Maximum
        {
            get
            {
                return m_maximum;
            }
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
        private decimal m_value = 0;
        /// <summary>
        /// La valeur actuelle du contrôle VisualDecimal
        /// </summary>
        [Category("VisualDecimal"), DefaultValue(typeof(decimal), "0"), Browsable(true), Description("La valeur actuelle du contrôle VisualInt")]
        public override decimal Value
        {
            get { return m_value; }
            set
            {
                if (value < m_minimum || value > m_maximum)
                    throw new ArgumentOutOfRangeException("La valeur '" + value + "' n'est pas valide, elle doit être comprise entre 'Minimum' et 'Maximum'");

                m_value = value;
                Refresh();
                //using (Graphics gr = CreateGraphics())
                //    DrawContent(gr);
                if (ValueChanged != null && !WaitForEnter)
                    ValueChanged(this, EventArgs.Empty);
            }
        }
        //=========================================================================================================
        private void SetValueWithoutValueChanged(decimal pValue)
        {
            if (pValue < m_minimum || pValue > m_maximum)
                throw new ArgumentOutOfRangeException("La valeur '" + pValue + "' n'est pas valide, elle doit être comprise entre 'Minimum' et 'Maximum'");

            m_value = pValue;
            Refresh();
        }

        //============================================================================================
        private int m_decimalPlaces = 1;
        /// <summary>
        /// Obtient ou définit le nombre de décimales à afficher.
        /// </summary>
        [Category("VisualDecimal"), DefaultValue(typeof(decimal), "1"), Browsable(true), Description("Indique le nombre de décimales à afficher.")]
        public int DecimalPlaces
        {
            get { return m_decimalPlaces; }
            set
            {
                m_decimalPlaces = value;
                this.Refresh();
            }
        }

        //============================================================================================
        private enuValueFormat m_valueFormat = enuValueFormat.Standard;
        /// <summary>
        /// Obtient et définit le format d'affichage des valeurs dans la grille
        /// </summary>
        [Category("VisualDecimal"), DefaultValue(enuValueFormat.Standard), Browsable(true), Description("Obtient et définit le format d'affichage des valeurs dans la grille.")]
        public enuValueFormat ValueFormat
        {
            get { return m_valueFormat; }
            set
            {
                m_valueFormat = value;
                Refresh();
            }
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

            Rectangle cellBounds = new Rectangle(Padding.Left, Padding.Top, Width - (Padding.Left + Padding.Right), Height - (Padding.Top + Padding.Bottom));
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
                string laChaine;
                if (m_valueFormat == enuValueFormat.Standard)
                    laChaine = m_value.ToString("F" + m_decimalPlaces);
                else if (m_valueFormat == enuValueFormat.Currency)
                    laChaine = m_value.ToString("C" + m_decimalPlaces);
                else
                    laChaine = (m_value * 100).ToString("F" + m_decimalPlaces) + " %";

                pGraphics.SetClip(cellBounds);
                switch (m_view)
                {
                    case enuDecimalView.Number:
                        VisualArraysTools.DrawText(pGraphics, cellBounds, laChaine, ForeColor, Font, m_valueAlign);
                        break;
                    case enuDecimalView.Graph:
                       
                        VisualArraysTools.DrawBar(pGraphics, cellBounds, GraphAppearance, m_minimum, m_maximum, m_value,m_decimalPlaces);
                        break;
                    case enuDecimalView.GraphNumber:
                        VisualArraysTools.DrawBar(pGraphics, cellBounds, GraphAppearance, m_minimum, m_maximum, m_value, m_decimalPlaces);
                        VisualArraysTools.DrawText(pGraphics, cellBounds, laChaine, ForeColor, Font, m_valueAlign);
                        break;
                    default:
                        break;
                }
                pGraphics.ResetClip();
            }
            base.DrawContent(pGraphics);
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
                if (m_view < enuDecimalView.GraphNumber)
                    View++;
                else
                    View = enuDecimalView.Number;
            }
            else
            {
                if (ContainsFocus && (!ReadOnly))
                {
                    switch (m_view)
                    {
                        case enuDecimalView.Number:
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
                        case enuDecimalView.Graph:
                        case enuDecimalView.GraphNumber:
                            Rectangle cellBounds = new Rectangle(Padding.Left, Padding.Top, Width - (Padding.Left + Padding.Right), Height - (Padding.Top + Padding.Bottom));
                            Value = VisualArraysTools.ValueFromClick(e.Location, cellBounds, GraphAppearance, m_minimum, m_maximum);
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
                decimal delta = (m_maximum - m_minimum) / 10;
                decimal minDelta = 1 / (decimal)Math.Pow(10, m_decimalPlaces);
                if (delta < minDelta) delta = minDelta;
                decimal valeur = m_value;
                if (e.Delta > 0)
                    valeur += delta;
                else if (e.Delta < 0)
                    valeur -= delta;
                if (valeur < m_minimum) valeur = m_minimum;
                if (valeur > m_maximum) valeur = m_maximum;
                Value = valeur;
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Permet de rendre actives les touches "flèches" du clavier.
        /// </summary>
        /// ----------------------------------------------------------------------------------------
        protected override bool IsInputKey(Keys key)
        {
            if (Keys.Up == key || Keys.Down == key || Keys.Right == key || Keys.Left == key)
                return true;
            else
                return base.IsInputKey(key);
        }
        /// <summary>
        /// Accepte les touches "flèches"
        /// </summary>
        /// <param name="e">infos sur la touche pressée</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!m_readOnly)
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                    if (ValueChanged != null)
                        ValueChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Gère la saisie d'un nombre decimal
        /// </summary>
        /// <param name="e">KeyPressEventArgs</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!m_readOnly)
            {
                decimal valeurObtenue;
                if (VisualArraysTools.ReadDecimal(e.KeyChar, m_value, m_maximum,m_decimalPlaces, out valeurObtenue))
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
            Value = VisualArraysTools.RandomDecimal(Minimum, Maximum);
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

            Value = VisualArraysTools.RandomDecimal(pMinimum, pMaximum);
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
}

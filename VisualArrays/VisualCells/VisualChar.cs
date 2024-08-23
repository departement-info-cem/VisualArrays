using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VisualArrays.VisualCells
{
    /// <summary>
    /// Représente une cellule booléenne. 
    /// </summary>
    public partial class VisualChar : VisualValue<char>
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
        /// Initialise un contrôle VisualChar
        /// </summary>
        public VisualChar()
        {
            InitializeComponent();
        }
        #endregion

        #region Propriétés
        //============================================================================================
        private bool m_caseSensitive = false;
        /// <summary>
        /// Obtient et définit les caractères saisies sont sensible à la case.
        /// </summary>
        [DefaultValue(false), Browsable(true), Description("Détermine si la saisir est sensible à la case")]
        public bool CaseSensitive
        {
            get { return m_caseSensitive; }
            set
            {
                if (value != m_caseSensitive)
                    m_caseSensitive = value;
            }
        }
        //============================================================================================
        private ImageList m_imageList = null;
        /// <summary>
        /// Obtient ou définit l'ImageList utilisée pour dessiner la valeur en mode View ImageList
        /// </summary>
        [Browsable(true), DefaultValue(null), Description("ImageList utilisée pour dessiner la valeur en mode View ImageList")]
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
        private enuCharView m_view = enuCharView.Char;
        /// <summary>
        /// Obtient et définit le style de visualisation la valeur du contrôle
        /// </summary>
        [DefaultValue(enuCharView.Char), Browsable(true), Description("Obtient et définit le style de visualisation pour la valeur du contrôle.")]
        public enuCharView View
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
        private char m_minimum = 'A';
        /// <summary>
        /// Obtient et définit la valeur minimale pour la propriété Value
        /// </summary>
        [DefaultValue(typeof(char), "A"), Browsable(true), Description("Obtient et définit la valeur minimale pour la propriété Value")]
        [RefreshProperties(RefreshProperties.All)]
        public char Minimum
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
        private char m_maximum = 'Z';
        /// <summary>
        /// Obtient ou définit la valeur maximale pour la propriété Value
        /// </summary>
        [DefaultValue(typeof(char), "Z"), Browsable(true), Description("Obtient et définit la valeur maximale pour la propriété Value.")]
        [RefreshProperties(RefreshProperties.All)]
        public char Maximum
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
        private char m_value = 'A';
        /// <summary>
        /// La valeur actuelle du contrôle VisualChar.
        /// </summary>
        [DefaultValue(typeof(char), "A"), Browsable(true), Description("La valeur actuelle du contrôle VisualChar")]
        public override char Value
        {
            get { return m_value; }
            set
            {
                if (value < m_minimum || value > m_maximum)
                    throw new ArgumentOutOfRangeException("La valeur '" + value + "' n'est pas valide, elle doit être comprise entre 'Minimum' et 'Maximum'");

                if (m_value != value)
                {
                    m_value = value;
                    Refresh();
                    //using (Graphics gr = CreateGraphics())
                    //    DrawContent(gr);
                    if (ValueChanged != null)
                        ValueChanged(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Méthodes privées
        //=========================================================================================================
        /// <summary>
        /// 
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
                pGraphics.SetClip(cellBounds);
                switch (m_view)
                {
                    case enuCharView.Char:
                        VisualArraysTools.DrawText(pGraphics, cellBounds, m_value.ToString(), ForeColor, Font, m_valueAlign);
                        break;
                    case enuCharView.Code:
                        VisualArraysTools.DrawText(pGraphics, cellBounds, ((int)m_value).ToString(), ForeColor, Font, m_valueAlign);
                        break;
                    default:
                        break;
                }
                pGraphics.ResetClip();
                //------------------------------------------------------------------------------------------------
            }
            base.DrawContent(pGraphics);
        }
        #endregion

        #region Méthodes redéfinies
        //============================================================================================
        /// <summary>
        /// Se produit lorsque le MouseWheel change de valeur
        /// </summary>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!ReadOnly)
            {
                int delta = 1;
                int valeur = m_value;
                if (e.Delta > 0)
                    valeur += delta;
                else if (e.Delta < 0)
                    valeur -= delta;
                if (valeur < m_minimum) valeur = m_minimum;
                if (valeur > m_maximum) valeur = m_maximum;
                Value = (char)valeur;
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
                char valeur = e.KeyChar;
                if (!m_caseSensitive) valeur = char.ToUpper(e.KeyChar);
                if (valeur < m_minimum) valeur = m_minimum;
                if (valeur > m_maximum) valeur = m_maximum;
                Value = valeur;
            }
            base.OnKeyPress(e);
        }

        //=========================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Alt)
            {
                if (m_view == enuCharView.Char)
                    View = enuCharView.Code;
                else
                    View = enuCharView.Char;
            }
            else
            {
                if (ContainsFocus && (!ReadOnly))
                {
                    if (e.Y < Height / 2)
                    {
                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                            if (m_value < m_maximum) Value++;
                        if (e.Button == System.Windows.Forms.MouseButtons.Right)
                            Value = m_maximum;
                    }
                    else
                    {
                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                            if (m_value > m_minimum) Value--;
                        if (e.Button == System.Windows.Forms.MouseButtons.Right)
                            Value = m_minimum;
                    }
                }
                else
                    Focus();
            }
        }

        #endregion

        #region RandomizeValue
        /// <summary>
        /// Assigne une valeur aléatoire à la propriété Value.
        /// </summary>
        /// <param name="pMinimum">borne inférieure de l'intervalle</param>
        /// <param name="pMaximum">borne supérieure de l'intervalle</param>
        public void RandomizeValue(char pMinimum, char pMaximum)
        {
            Value = VisualArraysTools.RandomChar(pMinimum, (char)(pMaximum + 1));
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

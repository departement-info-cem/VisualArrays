using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VisualArrays.VisualCells;

namespace VisualArrays
{
    /// <summary>
    /// Représente un conteneur de VisualChar
    /// </summary>
    public partial class VisualCharContainer : BaseVisualContainer<char,VisualChar>
    {
        #region Constructeur
        //========================================================================================================================
        /// <summary>
        /// Initialise un VisualCharContainer
        /// </summary>
        public VisualCharContainer()
        {
            InitializeComponent();
        }
        #endregion
    }
}

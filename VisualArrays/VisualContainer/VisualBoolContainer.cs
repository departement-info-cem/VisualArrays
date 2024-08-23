﻿using System;
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
    /// Représente un conteneur de VisualBool
    /// </summary>
    public partial class VisualBoolContainer : BaseVisualContainer<bool,VisualBool>
    {
        #region Constructeur
        //========================================================================================================================
        /// <summary>
        /// Initialise un VisualIntContainer
        /// </summary>
        public VisualBoolContainer()
        {
            InitializeComponent();
        }
        #endregion
    }
}

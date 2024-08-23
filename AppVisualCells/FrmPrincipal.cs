using VisualArrays;

namespace AppVisualCells;

//========================================================================================================================
    public partial class FrmPrincipal : Form
    {
         public FrmPrincipal()
        {
            InitializeComponent();
            visualStringArray1[0] = "ALLO";

            //for (int row = 0; row < viaTestThread.RowCount; row++)
            //    for (int col = 0; col < viaTestThread.ColumnCount; col++)
            //    {
            //        if (col != row)
            //        {
            //            viaTestThread.DisableCell(row, col);
            //            // ICI
            //            //viaSudoku[row, col] = 1;
            //        }
            //    }
        }

        //========================================================================================================================
        private void button1_Click(object sender, EventArgs e)
        {
            visualBool1.Value = !visualBool1.Value;
        }

        //========================================================================================================================
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            vsiNombre.Value = (int)numericUpDown1.Value;
        }

        //========================================================================================================================
        private void visualBool1_ValueChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = visualBool1.Value;
        }

        //========================================================================================================================
        private void btnVICTestClear_Click(object sender, EventArgs e)
        {
            //visualIntContainer1.Clear();
        }

        //========================================================================================================================
        private void btnVICTest1_Click(object sender, EventArgs e)
        {
            int total = 0;
            foreach (int valeur in visualIntContainer1)
            {
                total += valeur;
            }
            MessageBox.Show("La somme des valeurs est : " + total);
        }

        //========================================================================================================================
        private void btnVICTest2_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < visualIntContainer1.Length; index++)
                visualIntContainer1[index] += 3;
        }

        //========================================================================================================================
        private void btnVBCTestClear_Click(object sender, EventArgs e)
        {
            //visualBoolContainer1.Clear();
        }

        //========================================================================================================================
        private void btnVBCTest1_Click(object sender, EventArgs e)
        {
            int total = 0;
            foreach (bool valeur in visualBoolContainer1)
            {
                total += valeur ? 1 : 0;
            }
            MessageBox.Show("Le nombre de cellules allumées est de : " + total);
        }

        //========================================================================================================================
        private void btnVBCTest2_Click(object sender, EventArgs e)
        {
            //visualBoolContainer1.Clear();
            for (int index = 0; index < visualBoolContainer1.Length; index++)
            {
                VisualArraysTools.Wait(100);
                visualBoolContainer1[index] = index % 2 == 0;
            }
        }

        //========================================================================================================================
        private void visualIntContainer1_ValueChanged(object sender, CellVCEventArgs e)
        {
            lblVisualIntContainer.Text = "ValueChanged, Index = " + e.Index;
        }

        //========================================================================================================================
        private void visualBoolContainer1_ValueChanged(object sender, CellVCEventArgs e)
        {
            lblVisualBoolContainer.Text = "ValueChanged, Index = " + e.Index;
        }

        //========================================================================================================================
        private void vsiNombre_ValueChanged(object sender, EventArgs e)
        {
            vsiMiroir.Value = vsiNombre.Value;
        }

        //========================================================================================================================
        private void vsiMiroir_ValueChanged(object sender, EventArgs e)
        {
            for (int index = 0; index < visualIntContainer1.Length; index+=2)
                visualIntContainer1[index] = vsiMiroir.Value;
        }

        //========================================================================================================================
        private void visualBoolContainer1_CellMouseUp(object sender, CellMouseEventArgs1D e)
        {
            MessageBox.Show(e.Index.ToString());
        }

        //========================================================================================================================
        private void numPuissance_ValueChanged(object sender, EventArgs e)
        {
            for (int index = 0; index < (int)numPuissance.Value; index++)
                vbcMeter[index] = true;
            for (int index = (int)numPuissance.Value; index < vbcMeter.Length; index++)
                vbcMeter[index] = false;
        }
        //========================================================================================================================
        private void btnTestVCC_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < visualCharContainer1.Length; index++)
                visualCharContainer1[index] = (char)('A' + index);
        }

        private void btnVDC_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < visualDecimalContainer1.Length; index++)
                visualDecimalContainer1[index] = VisualArraysTools.RandomInt(0,100) * 0.5m;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            visualBool1.BorderSize = 0;
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            visualDecimal1.RandomizeValue();
        }

        private void visualIntArray2_CellMouseEnter(object sender, CellEventArgs e)
        {
            shapeSprite1.Visible = true;
            shapeSprite1.DisplayIndex = e.Index;
        }
        private void visualIntArray2_CellMouseLeave(object sender, CellEventArgs e)
        {
            shapeSprite1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            visualIntArray3.RowCount = visualIntArray3.RowCount - 1;
            visualIntArray3.ColumnCount = visualIntArray3.ColumnCount - 1;
        }

        private void visualIntArray3_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            textBox1.Text = e.Index + ":" + e.Row + ":" + e.Column + ":" + e.Address;
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            visualStringArray1[0, 0] = null;
        }

        private void btnTestAnimationSprites_Click(object sender, EventArgs e)
        {
            sprAnim1.Animated = true;
            sprAnim2.Animated = true;
            sprAnim3.Animated = true;
            sprAnim4.Animated = true;

            sprAnim1.DisplayAddress = new Address(4, 4);
            sprAnim2.DisplayAddress = new Address(0, 4);
            sprAnim3.DisplayAddress = new Address(4, 0);
            sprAnim4.DisplayAddress = new Address(0, 0);
        }

        private void viaTestThread_SpriteMouseDown(object sender, SpriteMouseEventArgs e)
        {
            Address da = e.Sprite.DisplayAddress;

            if (e.Sprite.IsMoving)
                e.Sprite.Visible = false;

            if (da.Column == 0)
                da.Column = viaTestThread.ColumnCount - 1;
            else
                da.Column = 0;

            if (da.Row == 0)
                da.Row = viaTestThread.RowCount -1;
            else
                da.Row = 0;

            e.Sprite.DisplayAddress = da;
        }

        private void viaTestThread_SpriteMouseDoubleClick(object sender, SpriteMouseEventArgs e)
        {
        }

        private void btnGo_Click(object sender, EventArgs e)
        {

            if (sprAnim1.DisplayAddress.Column == 0)
            {
                foreach (Sprite spr in viaTestThread.Sprites)
                {
                    Address pos = spr.DisplayAddress;
                    pos.Column = viaTestThread.ColumnCount - 1;
                    spr.DisplayAddress = pos;
                }
            }
            else 
            {
                foreach (Sprite spr in viaTestThread.Sprites)
                {
                    Address pos = spr.DisplayAddress;
                    pos.Column = 0;
                    spr.DisplayAddress = pos;
                }
            }
        }

        private void btnVisible_Click(object sender, EventArgs e)
        {
            foreach (Sprite spr in viaTestThread.Sprites)
            {
                spr.Visible = true;
            }
        }

    }
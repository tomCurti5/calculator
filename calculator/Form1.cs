using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace calculator
{
    public partial class frmMain : Form
    {
        public struct BtnStruct
        {
            public char Content;
            public bool IsBold;
            public BtnStruct(char c, bool b)
            {
                this.Content = c;
                this.IsBold = b;
            }
        }

        private BtnStruct[,] buttons =
        {
            { new BtnStruct('%',false),new BtnStruct('\u0152', false), new BtnStruct('C', false), new BtnStruct('\u232B', false) },
            { new BtnStruct('\u215F',false),new BtnStruct('\u00B2', false), new BtnStruct('\u221A', false), new BtnStruct('\u00F7', false) },
            { new BtnStruct('7',false),new BtnStruct('8', false), new BtnStruct('9', false), new BtnStruct('\u00D7', false) },
            { new BtnStruct('4',false),new BtnStruct('5', false), new BtnStruct('6', false), new BtnStruct('-', false) },
            { new BtnStruct('1',false),new BtnStruct('2', false), new BtnStruct('3', false), new BtnStruct('+', false) },
            { new BtnStruct('\u00B1',false),new BtnStruct('0', false), new BtnStruct(',', false), new BtnStruct('=', false) }
        };
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            MakeButtons(buttons.GetLength(0),buttons.GetLength(1));
        }

        private void MakeButtons(int rows, int cols)
        {
            int BtnWidth = 80;
            int BtnHeight = 60;

            int posX = 0;
            int posY = 116;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++) 
                {
                    Button myButton = new Button();
                    FontStyle fs = buttons[i, j].IsBold ? FontStyle.Bold : FontStyle.Regular;
                    myButton.BackColor = buttons[i, j].IsBold ? Color.White : Color.Transparent;
                    myButton.Font = new Font("Segoe UI", 16, fs);
                    myButton.Text = buttons[i, j].Content.ToString();                                                         
                    myButton.Width = BtnWidth;
                    myButton.Height = BtnHeight;
                    myButton.Top = posY;
                    myButton.Left = posX;
                    this.Controls.Add(myButton);
                    posX += myButton.Width;
                }
                posX = 0;
                posY += BtnHeight;
            }
        }
    }
}

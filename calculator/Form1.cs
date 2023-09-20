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
        public enum SymbolType
        {
            Number,
            Operator,
            DecimalPoint,
            PlusMinusSign,
            BackSpace,
            Undefined
        }
        public struct BtnStruct
        {
            public char Content;
            public bool IsBold;
            public SymbolType Type;
            public BtnStruct(char c, SymbolType t=SymbolType.Undefined, bool b=false)
            {
                this.Content = c;
                this.IsBold = b;
                this.Type = t;
            }
        }

        private BtnStruct[,] buttons =
        {
            { new BtnStruct('%'),new BtnStruct('\u0152'), new BtnStruct('C'), new BtnStruct('\u232B') },
            { new BtnStruct('\u215F'),new BtnStruct('\u00B2'), new BtnStruct('\u221A'), new BtnStruct('\u00F7') },
            { new BtnStruct('7',SymbolType.Number,true),new BtnStruct('8', SymbolType.Number,true), new BtnStruct('9', SymbolType.Number,true), new BtnStruct('\u00D7', SymbolType.Operator) },
            { new BtnStruct('4',SymbolType.Number,true),new BtnStruct('5', SymbolType.Number,true), new BtnStruct('6', SymbolType.Number,true), new BtnStruct('-', SymbolType.Operator) },
            { new BtnStruct('1',SymbolType.Number,true),new BtnStruct('2', SymbolType.Number,true), new BtnStruct('3', SymbolType.Number,true), new BtnStruct('+', SymbolType.Operator) },
            { new BtnStruct('\u00B1',SymbolType.PlusMinusSign),new BtnStruct('0', SymbolType.Number,true), new BtnStruct(',', SymbolType.DecimalPoint), new BtnStruct('=', SymbolType.Operator) }
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
                    myButton.Tag = buttons[i, j];
                    myButton.Click += Button_Click;
                    this.Controls.Add(myButton);
                    posX += myButton.Width;
                }
                posX = 0;
                posY += BtnHeight;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            BtnStruct clickedButtonStruct = (BtnStruct)clickedButton.Tag;
            switch (clickedButtonStruct.Type)
            {
                case SymbolType.Number:
                    if (lblResult.Text == "0")
                        lblResult.Text += clickedButton.Text;
                    break;
                case SymbolType.Operator:
                    break;
                case SymbolType.DecimalPoint:
                    if (lblResult.Text.IndexOf(",") == -1)
                        lblResult.Text += clickedButton.Text;
                    break;
                case SymbolType.PlusMinusSign:
                    if (lblResult.Text != "0")
                    {
                        if (lblResult.Text.IndexOf("-") == -1) 
                        {
                            lblResult.Text = "-" + lblResult.Text;
                        }
                        else
                        {
                            lblResult.Text = lblResult.Text.Substring(1);
                        }
                    }
                    break;

                case SymbolType.BackSpace:
                    lblResult.Text = lblResult.Text.Substring(0, lblResult.Text.Length - 1);
                    if (lblResult.Text.Length == 0 || lblResult.Text == "-0")
                    {
                        lblResult.Text = "0";
                    }
                    break;
                case SymbolType.Undefined:
                    break;
                default:
                    break;
            }
        }
    }
}

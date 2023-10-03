using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator_2023
{
    public partial class FormMain : Form
    {
        public enum SymbolType
        {
            Number,
            Operator,
            DecimalPoint,
            PlusMinusSign,
            BackSpace,
            ClearAll,
            ClearEntry,
            Undefined

        }
        public struct BtnStuct
        {
            public char Content;
            public bool IsBold;
            public SymbolType Type;
            public BtnStuct(char c, SymbolType t = SymbolType.Undefined, bool b = false)
            {
                this.Content = c;
                this.Type = t;
                this.IsBold = b;
            }
        }

        private BtnStuct[,] buttons =
        {
            {new BtnStuct('%'), new BtnStuct('\u0152',SymbolType.ClearEntry), new BtnStuct('C',SymbolType.ClearAll), new BtnStuct('\u232B',SymbolType.BackSpace)},
            { new BtnStuct('\u215F'), new BtnStuct('\u00B2'), new BtnStuct('\u221A'), new BtnStuct('\u00F7', SymbolType.Operator)},
            { new BtnStuct('7',SymbolType.Number,true), new BtnStuct('8',SymbolType.Number,true), new BtnStuct('9',SymbolType.Number,true), new BtnStuct('\u00D7',SymbolType.Operator)},
            { new BtnStuct('4',SymbolType.Number,true), new BtnStuct('5',SymbolType.Number,true), new BtnStuct('6',SymbolType.Number,true), new BtnStuct('-',SymbolType.Operator)},
            { new BtnStuct('1',SymbolType.Number,true), new BtnStuct('2',SymbolType.Number,true), new BtnStuct('3',SymbolType.Number,true), new BtnStuct('+',SymbolType.Operator)},
            { new BtnStuct('\u00B1', SymbolType.PlusMinusSign), new BtnStuct('0',SymbolType.Number,true), new BtnStuct(',',SymbolType.DecimalPoint), new BtnStuct('=',SymbolType.Operator)},

        };

        float lblResultBaseFontSize;
        const int lblResultWidthMargin = 24;
        const int lblResultMaxDigit = 20;

        char lastOperator = ' ';
        decimal operand1, operand2, result;
        BtnStuct LastButtonClicked;

        public FormMain()
        {
            InitializeComponent();
            lblResultBaseFontSize = lblResult.Font.Size;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Makebuttons(buttons.GetLength(0), buttons.GetLength(1));
        }

        private void Makebuttons(int rows, int cols)
        {
            int btnWidth = 80;
            int btnHeight = 60;
            int posX = 0;
            int posY = 116;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Button myButton = new Button();
                    FontStyle fs = buttons[i, j].IsBold ? FontStyle.Bold : FontStyle.Regular;
                    myButton.BackColor = buttons[i, j].IsBold ? Color.White : Color.Transparent;
                    myButton.Font = new Font("Segoe UI", 16);
                    myButton.Text = buttons[i, j].Content.ToString();
                    myButton.Width = btnWidth;
                    myButton.Height = btnHeight;
                    myButton.Top = posY;
                    myButton.Left = posX;
                    myButton.Click += Button_Click;
                    myButton.Tag = buttons[i, j];
                    this.Controls.Add(myButton);
                    posX += myButton.Width;
                }
                posX = 0;
                posY += btnHeight;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            BtnStuct clickedButtonStruct = (BtnStuct)clickedButton.Tag;

            switch (clickedButtonStruct.Type)
            {
                case SymbolType.Number:
                    if (lblResult.Text == "0" || LastButtonClicked.Type == SymbolType.Operator)
                    {
                        lblResult.Text = "";
                    }
                    lblResult.Text += clickedButton.Text;
                    break;
                case SymbolType.Operator:
                    ManageOperator(clickedButtonStruct);
                    break;
                case SymbolType.DecimalPoint:
                    if (lblResult.Text.IndexOf(",") == -1)
                    {
                        lblResult.Text += clickedButton.Text;
                    }
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
                    if(LastButtonClicked.Type != SymbolType.Operator)
                    {
                        lblResult.Text = lblResult.Text.Substring(0, lblResult.Text.Length - 1);
                        if (lblResult.Text.Length == 0 || lblResult.Text == "-0")
                        {
                            lblResult.Text = "0";
                        }
                    }
                    lblResult.Text = lblResult.Text.Substring(0, lblResult.Text.Length - 1);
                    if (lblResult.Text.Length == 0 || lblResult.Text == "-0")
                    {
                        lblResult.Text = "0";
                    }
                    break;
                case SymbolType.Undefined:

                    break;
                case SymbolType.ClearAll:
                    lblResult.Text = "0";
                    lastOperator = ' ';
                    break;
                default:
                    break;
            }
            if(clickedButtonStruct.Type != SymbolType.BackSpace)
                LastButtonClicked = clickedButtonStruct;
        }

        private void ManageOperator(BtnStuct clickedButtonSrtuct)
        {
            if (lastOperator == ' ')
            {
                operand1 = decimal.Parse(lblResult.Text);
                if (clickedButtonSrtuct.Content != '=') lastOperator = clickedButtonSrtuct.Content;
            }
            else
            {
                operand2 = decimal.Parse(lblResult.Text);
                switch (lastOperator)
                {
                    case '+':
                        result = operand1 + operand2;
                        break;
                    case '-':
                        result = operand1 - operand2;
                        break;
                    case '\u00D7':
                        result = operand1 * operand2;
                        break;
                    case '\u00F7':
                        if (operand2 != 0)
                        {
                            result = operand1 / operand2;
                        }
                        break;
                    default:
                        break;
                }
                operand1 = result;
                lastOperator = clickedButtonSrtuct.Content;
                lblResult.Text = result.ToString();

            }
        }

        private void lblResult_TextChanged(object sender, EventArgs e)
        {
            if (lblResult.Text == "-")
            {
                lblResult.Text = "0";
                return;
            }
            if (lblResult.Text.Length > 0)
            {
                decimal num = decimal.Parse(lblResult.Text);
                string stOut = "";
                NumberFormatInfo nfi = new CultureInfo("it-IT", false).NumberFormat;
                int decimalSeparatorPosition = lblResult.Text.IndexOf(",");
                nfi.NumberDecimalDigits = decimalSeparatorPosition == -1 ?
                    0 :
                    lblResult.Text.Length - decimalSeparatorPosition - 1;
                stOut = num.ToString("N", nfi);
                if (lblResult.Text.IndexOf(",") == lblResult.Text.Length - 1)
                {
                    stOut += ",";
                }
                lblResult.Text = stOut;

            }
            if (lblResult.Text.Length > lblResultMaxDigit)
            {
                lblResult.Text = lblResult.Text.Substring(0, lblResultMaxDigit);
            }

            int textWidth = TextRenderer.MeasureText(lblResult.Text, lblResult.Font).Width;
            float newSize = lblResult.Font.Size * (((float)lblResult.Size.Width - lblResultWidthMargin) / textWidth);
            if (newSize > lblResultBaseFontSize)
            {
                newSize = lblResultBaseFontSize;
            }
            lblResult.Font = new Font("Segoe UI", newSize, FontStyle.Bold);
        }
    }
}
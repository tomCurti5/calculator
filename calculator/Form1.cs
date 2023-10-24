using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator2023
{
    public partial class FormMain : Form
    {
        public enum SymbolType
        {
            Number,
            Operator,
            SpecialOperator,
            DecimalPoint,
            PlusMinusSign,
            Backspace,
            ClearAll,
            ClearEntry,
            Undefined
        }

        public struct BtnStruct
        {
            public char Content;
            public SymbolType Type;
            public bool IsBold;
            public BtnStruct(char c, SymbolType t = SymbolType.Undefined, bool b = false)
            {
                this.Content = c;
                this.Type = t;
                this.IsBold = b;
            }
        }

        private BtnStruct[,] buttons =
        {
            { new BtnStruct('%',SymbolType.SpecialOperator), new BtnStruct('\u0152', SymbolType.ClearEntry), new BtnStruct('C', SymbolType.ClearAll), new BtnStruct('\u232B', SymbolType.Backspace) },
            { new BtnStruct('\u215F',SymbolType.SpecialOperator), new BtnStruct('\u00B2',SymbolType.SpecialOperator), new BtnStruct('\u221A',SymbolType.SpecialOperator), new BtnStruct('\u00F7',SymbolType.Operator) },
            { new BtnStruct('7',SymbolType.Number,true), new BtnStruct('8',SymbolType.Number,true), new BtnStruct('9',SymbolType.Number,true), new BtnStruct('\u00D7',SymbolType.Operator) },
            { new BtnStruct('4',SymbolType.Number,true), new BtnStruct('5',SymbolType.Number,true), new BtnStruct('6',SymbolType.Number,true), new BtnStruct('-',SymbolType.Operator) },
            { new BtnStruct('1',SymbolType.Number,true), new BtnStruct('2',SymbolType.Number,true), new BtnStruct('3',SymbolType.Number,true), new BtnStruct('+',SymbolType.Operator) },
            { new BtnStruct('\u00B1',SymbolType.PlusMinusSign), new BtnStruct('0',SymbolType.Number,true), new BtnStruct(',',SymbolType.DecimalPoint), new BtnStruct('=',SymbolType.Operator) },
        };

        float lblResultBaseFontSize;
        const int lblResultWidthMargin = 24;
        decimal operand1, operand2, result;
        const int lblResultMaxDigit = 25;
        char lastOperator = ' ';
        BtnStruct lastButtonClicked;

        public FormMain()
        {
            InitializeComponent();
            lblResultBaseFontSize = lblResult.Font.Size;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            MakeButtons(buttons.GetLength(0), buttons.GetLength(1));
        }

        private void MakeButtons(int rows, int cols)
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
                    myButton.Font = new Font("Segoe UI", 16, fs);
                    myButton.BackColor = buttons[i, j].IsBold ? Color.White : Color.Transparent;
                    myButton.Text = buttons[i, j].Content.ToString();
                    myButton.Width = btnWidth;
                    myButton.Height = btnHeight;
                    myButton.Top = posY;
                    myButton.Left = posX;
                    myButton.Tag = buttons[i, j];
                    myButton.Click += Button_Click;
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
            BtnStruct clickedButtonStruct = (BtnStruct)clickedButton.Tag;

            switch (clickedButtonStruct.Type)
            {
                case SymbolType.Number:
                    if (lastButtonClicked.Content == '=')
                        ClearAll();
                    if (lblResult.Text == "0" || lastButtonClicked.Type == SymbolType.Operator)
                        lblResult.Text = "";
                    lblResult.Text += clickedButton.Text;
                    break;
                case SymbolType.SpecialOperator:
                    ManageSpecialOperator(clickedButtonStruct);
                    break;
                case SymbolType.Operator:
                    if (lastButtonClicked.Type == SymbolType.Operator && clickedButtonStruct.Content != '=')
                    {
                        lastOperator = clickedButtonStruct.Content;
                    }
                    else
                    {
                        ManageOperator(clickedButtonStruct);
                    }
                    break;
                case SymbolType.DecimalPoint:
                    if (lblResult.Text.IndexOf(",") == -1)
                        lblResult.Text += clickedButton.Text;
                    break;
                case SymbolType.PlusMinusSign:
                    if (lblResult.Text != "0")
                        if (lblResult.Text.IndexOf("-") == -1)
                            lblResult.Text = "-" + lblResult.Text;
                        else
                            lblResult.Text = lblResult.Text.Substring(1);
                    if (lastButtonClicked.Type == SymbolType.Operator)
                    {
                        operand1 = -operand1;
                    }
                    break;
                case SymbolType.Backspace:
                    if (lastButtonClicked.Type != SymbolType.Operator && lastButtonClicked.Type != SymbolType.SpecialOperator)
                    {
                        lblResult.Text = lblResult.Text.Substring(0, lblResult.Text.Length - 1);
                        if (lblResult.Text.Length == 0 || lblResult.Text == "-0")
                            lblResult.Text = "0";
                    }
                    break;
                case SymbolType.ClearAll:
                    ClearAll();
                    break;
                case SymbolType.ClearEntry:
                    if (lastButtonClicked.Content == '=')
                        ClearAll();
                    else
                        lblResult.Text = "0";
                    break;
                case SymbolType.Undefined:
                    break;
                default:
                    break;
            }
            if (clickedButtonStruct.Type != SymbolType.Backspace && clickedButtonStruct.Type != SymbolType.PlusMinusSign)
                lastButtonClicked = clickedButtonStruct;

            HandleHistory(clickedButtonStruct);
        }

        private void HandleHistory(BtnStruct clickedButtonStruct)
        {
            if (clickedButtonStruct.Type != SymbolType.PlusMinusSign && clickedButtonStruct.Content != '=' && clickedButtonStruct.Type != SymbolType.ClearEntry && clickedButtonStruct.Type != SymbolType.Backspace && clickedButtonStruct.Type != SymbolType.ClearAll)
                lblOperators.Text += clickedButtonStruct.Content + " ";
            else if (clickedButtonStruct.Type == SymbolType.ClearAll)
                lblOperators.Text = "";
            else if (clickedButtonStruct.Content == '=')
                lblOperators.Text += clickedButtonStruct.Content + " " + result;

        }


        private void ClearAll()
        {
            operand1 = 0;
            operand2 = 0;
            result = 0;
            lastOperator = ' ';
            lblResult.Text = "0";
        }

        private void ManageSpecialOperator(BtnStruct clickedButtonStruct)
        {
            operand2 = decimal.Parse(lblResult.Text);
            switch (clickedButtonStruct.Content)
            {
                case '%':
                    result = operand1 * operand2 / 100;
                    break;
                case '\u215F':
                    result = 1 / operand2;
                    break;
                case '\u00B2':
                    result = operand2 * operand2;
                    break;
                case '\u221A':
                    result = (decimal)Math.Sqrt((double)operand2);
                    break;
                default:
                    break;
            }
            lblResult.Text = result.ToString();
        }

        private void ManageOperator(BtnStruct clickedButtonStruct)
        {
            if (lastOperator == ' ')
            {
                operand1 = decimal.Parse(lblResult.Text);
                if (clickedButtonStruct.Content != '=')
                    lastOperator = clickedButtonStruct.Content;
            }
            else
            {
                if (lastButtonClicked.Content != '=')
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
                        if (operand2 != 0)
                        {
                            result = operand1 * operand2;
                        }
                        else
                        {
                            lblResult.Text = "Error: Division by zero";
                            return;
                        }
                        break;
                    case '\u00F7':
                        if (operand2 != 0)
                        {
                            result = operand1 / operand2;
                        }
                        else
                        {
                            lblResult.Text = "Error: Division by zero";
                            return;
                        }
                        break;
                    default:
                        break;
                }
                operand1 = result;
                if (clickedButtonStruct.Content != '=')
                {
                    lastOperator = clickedButtonStruct.Content;
                    if (lastButtonClicked.Content == '=')
                        operand2 = 0;
                }
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
                decimal num = decimal.Parse(lblResult.Text); string stOut = "";
                NumberFormatInfo nfi = new CultureInfo("it-IT", false).NumberFormat;
                int decimalSeparatorPosition = lblResult.Text.IndexOf(",");
                nfi.NumberDecimalDigits = decimalSeparatorPosition == -1 ?
                    0 :
                    lblResult.Text.Length - decimalSeparatorPosition - 1;
                stOut = num.ToString("N", nfi);
                if (lblResult.Text.IndexOf(",") == lblResult.Text.Length - 1)
                    stOut += ",";
                lblResult.Text = stOut;
            }
            if (lblResult.Text.Length > lblResultMaxDigit)
                lblResult.Text = lblResult.Text.Substring(0, lblResultMaxDigit);
            int textWidth = TextRenderer.MeasureText(lblResult.Text, lblResult.Font).Width;
            float newSize = lblResult.Font.Size * (((float)lblResult.Size.Width - lblResultWidthMargin) / textWidth);
            if (newSize > lblResultBaseFontSize)
                newSize = lblResultBaseFontSize;
            lblResult.Font = new Font("Segoe UI", newSize, FontStyle.Regular);
        }
    }
}
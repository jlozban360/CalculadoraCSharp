using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Drawing; // Asegúrate de tener esta importación para usar Color

namespace calculadorav2
{
    public partial class Calculadora : Form
    {
        //----------------------------------------------------------------------------------
        // Inicializar Calculadora y valores generales
        //----------------------------------------------------------------------------------

        private char[] symbolsPriorityArray = { '^', '√', '/', '*', '+', '-' };
        private bool errorShown = false;

        public Calculadora()
        {
            InitializeComponent();
            InitializeBlinkTimer();
        }

        private void Form1_Load(object sender, EventArgs e) {}

        //----------------------------------------------------------------------------------
        // Blinking func (parpadeo)
        //----------------------------------------------------------------------------------

        private System.Windows.Forms.Timer blinkTimer;
        private bool isBlinking = false;
        private bool timeToChange = false;

        private void InitializeBlinkTimer()
        {
            blinkTimer = new System.Windows.Forms.Timer();
            blinkTimer.Interval = 500;
            blinkTimer.Tick += BlinkTimer_Tick;
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            if (!timeToChange)
            {
                timeToChange = true;
                textBoxResultado.TextAlign = HorizontalAlignment.Center;
                textBoxResultado.ForeColor = Color.White;
                textBoxResultado.BackColor = Color.Blue;
            }
            else
            {
                timeToChange = false;

                Random rand = new Random();
                int randomValue = rand.Next(1, 3);

                switch (randomValue)
                {
                    case 1:
                        textBoxResultado.ForeColor = Color.Gold;
                        textBoxResultado.BackColor = Color.Green;
                        break; 
                    case 2:
                        textBoxResultado.ForeColor = Color.Black;
                        textBoxResultado.BackColor = Color.Yellow;
                        break;
                }
            }
        }

        private void StartBlinking()
        {
            if (!isBlinking)
            {
                BlinkTimer_Tick(0, null);
                blinkTimer.Start();
                isBlinking = true;
            }
        }

        private void StopBlinking()
        {
            if (isBlinking)
            {
                blinkTimer.Stop();
                textBoxResultado.ForeColor = Color.Black;
                textBoxResultado.BackColor = Color.LightGray;
                textBoxResultado.TextAlign = HorizontalAlignment.Left;
                isBlinking = false;
            }
        }

        //----------------------------------------------------------------------------------
        // Botones números
        //----------------------------------------------------------------------------------

        private void buttonNine_Click(object sender, EventArgs e) => handleNumberInput("9");
        private void buttonEight_Click(object sender, EventArgs e) => handleNumberInput("8");
        private void buttonSeven_Click(object sender, EventArgs e) => handleNumberInput("7");
        private void buttonSix_Click(object sender, EventArgs e) => handleNumberInput("6");
        private void buttonFive_Click(object sender, EventArgs e) => handleNumberInput("5");
        private void buttonFour_Click(object sender, EventArgs e) => handleNumberInput("4");
        private void buttonThree_Click(object sender, EventArgs e) => handleNumberInput("3");
        private void buttonTwo_Click(object sender, EventArgs e) => handleNumberInput("2");
        private void buttonOne_Click(object sender, EventArgs e) => handleNumberInput("1");
        private void buttonZero_Click(object sender, EventArgs e) => handleNumberInput("0");

        private void handleNumberInput(string input)
        {
            if (errorShown)
            {
                textBoxResultado.Clear();
                errorShown = false;
            }

            textBoxResultado.AppendText(input);
            StopBlinking();
        }

        //----------------------------------------------------------------------------------
        // Operaciones (Dividir, multiplicar, restar, sumar)
        //----------------------------------------------------------------------------------

        private void buttonDivide_Click(object sender, EventArgs e) => handleOperatorInput('/');
        private void buttonMultiply_Click(object sender, EventArgs e) => handleOperatorInput('*');
        private void buttonMinus_Click(object sender, EventArgs e) => handleOperatorInput('-');
        private void buttonPlus_Click(object sender, EventArgs e) => handleOperatorInput('+');

        private void handleOperatorInput(char operatorSymbol)
        {
            if (errorShown)
            {
                textBoxResultado.Clear();
                errorShown = false;
            }

            if (textBoxResultado.Text.Length > 0 || operatorSymbol == '-' || operatorSymbol == '√')
                replaceWith(operatorSymbol);

            StopBlinking();
        }

        //----------------------------------------------------------------------------------
        // Operaciones especiales (Elevar, raíz)
        //----------------------------------------------------------------------------------

        private void buttonRaiz_Click(object sender, EventArgs e) => handleOperatorInput('√');
        private void buttonElevar_Click(object sender, EventArgs e) => handleOperatorInput('^');

        //----------------------------------------------------------------------------------
        // Función C para borrar el último número
        //----------------------------------------------------------------------------------

        private void buttonC_Click(object sender, EventArgs e)
        {
            if (textBoxResultado.Text.Length > 0) deleteLastOperatorOrNumber();
            StopBlinking();
        }

        //----------------------------------------------------------------------------------
        // Función CE para reestablecer la calculadora
        //----------------------------------------------------------------------------------

        private void buttonCE_Click(object sender, EventArgs e)
        {
            textBoxResultado.Clear();
            StopBlinking();
        }

        //----------------------------------------------------------------------------------
        // Función deleteLast
        // que borra el último símbolo o el último número
        //----------------------------------------------------------------------------------

        private void buttonDelete_Click(object sender, EventArgs e) => deleteLast();

        //--------------------------------------------------------------------------------

        private void deleteLast()
        {
            if (textBoxResultado.Text.Length > 0)
            {
                string text = textBoxResultado.Text;
                textBoxResultado.Text = text.Remove(text.Length - 1, 1);
                StopBlinking();
            }
        }

        //----------------------------------------------------------------------------------
        // Función deleteLastOperatorOrNumber,
        // que borra el último símbolo o el último grupo de números
        //----------------------------------------------------------------------------------

        private void deleteLastOperatorOrNumber()
        {
            if (textBoxResultado.Text.Length > 0)
            {
                string text = textBoxResultado.Text;

                int lastIndex = text.Length - 1;

                if (char.IsDigit(text[lastIndex]))
                {
                    int start = lastIndex;

                    while (start >= 0 && char.IsDigit(text[start])) start--;
                    textBoxResultado.Text = text.Remove(start + 1);
                }
                else
                    textBoxResultado.Text = text.Remove(lastIndex, 1);

                StopBlinking();
            }
        }

        //----------------------------------------------------------------------------------
        // Función replaceWith, diseñada para comprobar si el último caracter es un símbolo,
        // sustituirlo para que no pueda haber dos signos seguidos y un arreglo para que no
        // borre el último número.
        //----------------------------------------------------------------------------------

        private void replaceWith(char character)
        {
            if (textBoxResultado.Text.Length == 0)
            {
                if (character == '√')
                    textBoxResultado.Text += character;

                return;
            }

            char[] charArray = textBoxResultado.Text.ToCharArray();
            bool containsSymbol = false;

            for (int i = 0; i < symbolsPriorityArray.Length; i++)
            {
                if (charArray[charArray.Length - 1] == symbolsPriorityArray[i])
                {
                    containsSymbol = true;
                    break;
                }
            }

            if (containsSymbol)
            {
                charArray[charArray.Length - 1] = character;
                textBoxResultado.Text = new string(charArray);
            }
            else
                textBoxResultado.Text += character;
        }

        //----------------------------------------------------------------------------------
        // v1. Función calculateThis, diseñada para splittear la cadena donde estén los símbolos
        // guardando cada operación en un array de operaciones distinto, volviendo a armar
        // las cadenas según la prioridad de las operaciones.
        //
        // v2. Optimización utilizando la librería Data Table -> Compute
        //----------------------------------------------------------------------------------

        private void buttonEquals_Click(object sender, EventArgs e)
        {
            calculateThis(textBoxResultado.Text);
        }

        //--------------------------------------------------------------------------------

        // v1 Utilizando conversores, listas, bucles y switches
        public void calculateThis(string calcStr)
        {
            try
            {
                calcStr = calcStr.Replace(" ", "");

                if (string.IsNullOrEmpty(calcStr) || !IsValidExpression(calcStr))
                {
                    throw new Exception("Expresión inválida");
                }

                List<double> numbers = new List<double>();
                List<char> operators = new List<char>();

                string tempNumber = "";

                foreach (char c in calcStr)
                {
                    if (char.IsDigit(c) || c == '.')
                    {
                        tempNumber += c;
                    }
                    else if (symbolsPriorityArray.Contains(c))
                    {
                        if (!string.IsNullOrEmpty(tempNumber))
                        {
                            numbers.Add(double.Parse(tempNumber));
                            tempNumber = "";
                        }
                        operators.Add(c);
                    }
                    else
                    {
                        throw new Exception("Carácter no permitido");
                    }
                }

                if (!string.IsNullOrEmpty(tempNumber))
                {
                    numbers.Add(double.Parse(tempNumber));
                }

                for (int i = 0; i < operators.Count; i++)
                {
                    if (operators[i] == '√')
                    {
                        double result = Math.Sqrt(numbers[i]);
                        numbers[i] = result;

                        operators.RemoveAt(i);
                        i--;
                    }
                    else if (operators[i] == '^')
                    {
                        double result = Math.Pow(numbers[i], numbers[i + 1]);
                        numbers[i] = result;

                        numbers.RemoveAt(i + 1);

                        operators.RemoveAt(i);
                        i--;
                    }
                }

                for (int i = 0; i < operators.Count; i++)
                {
                    if (operators[i] == '*' || operators[i] == '/')
                    {
                        double result = 0;

                        if (operators[i] == '*')
                        {
                            result = numbers[i] * numbers[i + 1];
                        }
                        else if (operators[i] == '/')
                        {
                            if (numbers[i + 1] == 0)
                            {
                                throw new DivideByZeroException("División por cero");
                            }
                            result = numbers[i] / numbers[i + 1];
                        }

                        numbers[i] = result;
                        numbers.RemoveAt(i + 1);
                        operators.RemoveAt(i);
                        i--;
                    }
                }

                while (operators.Count > 0)
                {
                    double result = 0;

                    if (operators[0] == '+')
                    {
                        result = numbers[0] + numbers[1];
                    }
                    else if (operators[0] == '-')
                    {
                        result = numbers[0] - numbers[1];
                    }

                    numbers[0] = result;
                    numbers.RemoveAt(1);
                    operators.RemoveAt(0);
                }

                textBoxResultado.Text = numbers[0].ToString();
                StartBlinking();
            }
            catch (DivideByZeroException)
            {
                errorShown = true;
                textBoxResultado.Text = "Error: División por cero";
            }
            catch (Exception ex)
            {
                errorShown = true;
                textBoxResultado.Text = "Error: " + ex.Message;
            }
        }

        //--------------------------------------------------------------------------------

        private bool IsValidExpression(string expression)
        {
            if (symbolsPriorityArray.Contains(expression[0]) && expression[0] != '-' && expression[0] != '√')
                return false;

            foreach (char c in expression)
            {
                if (!char.IsDigit(c) && !symbolsPriorityArray.Contains(c) && c != '.')
                    return false;
            }

            for (int i = 1; i < expression.Length; i++)
            {
                if (symbolsPriorityArray.Contains(expression[i]) && symbolsPriorityArray.Contains(expression[i - 1]) && expression[i - 1] != '-' && expression[i] != '√')
                    return false;
            }

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '√')
                {
                    if (i + 1 >= expression.Length || (!char.IsDigit(expression[i + 1]) && expression[i + 1] != '.'))
                        return false;
                }
            }

            return true;
        }
    }
}

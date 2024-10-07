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
        private char[] symbolsPriorityArray = { '^', '√', '/', '*', '+', '-' };
        private bool errorShown = false;
        private System.Windows.Forms.Timer blinkTimer; // Usando el espacio de nombres completo
        private bool isBlinking = false; // Estado del parpadeo

        public Calculadora()
        {
            InitializeComponent();
            InitializeBlinkTimer(); // Inicializar el Timer
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void InitializeBlinkTimer()
        {
            blinkTimer = new System.Windows.Forms.Timer();
            blinkTimer.Interval = 500; // Intervalo de parpadeo (en milisegundos)
            blinkTimer.Tick += BlinkTimer_Tick;
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            // Cambia el borde del TextBox entre rojo y transparente
            if (textBoxResultado.BorderStyle == BorderStyle.FixedSingle)
            {
                textBoxResultado.BorderStyle = BorderStyle.Fixed3D;
                textBoxResultado.BackColor = Color.Red;
            }
            else
            {
                textBoxResultado.BorderStyle = BorderStyle.FixedSingle;
                textBoxResultado.BackColor = Color.White;
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
            StopBlinking(); // Detener el parpadeo si se presiona un número
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

            // Permitir ingresar la raíz cuadrada incluso si no hay un número antes
            if (textBoxResultado.Text.Length > 0 || operatorSymbol == '-' || operatorSymbol == '√')
                replaceWith(operatorSymbol);

            StopBlinking(); // Detener el parpadeo si se presiona un operador
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
            StopBlinking(); // Detener el parpadeo
        }

        //----------------------------------------------------------------------------------
        // Función CE para reestablecer la calculadora
        //----------------------------------------------------------------------------------

        private void buttonCE_Click(object sender, EventArgs e)
        {
            textBoxResultado.Clear();
            StopBlinking(); // Detener el parpadeo
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
                StopBlinking(); // Detener el parpadeo
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
                {
                    textBoxResultado.Text = text.Remove(lastIndex, 1);
                }
                StopBlinking(); // Detener el parpadeo
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
                // Si no hay texto y se ingresa una raíz cuadrada, se permite
                if (character == '√')
                {
                    textBoxResultado.Text += character;
                }
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
            {
                textBoxResultado.Text += character;
            }
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
                // Elimina espacios
                calcStr = calcStr.Replace(" ", "");

                if (string.IsNullOrEmpty(calcStr) || !IsValidExpression(calcStr))
                {
                    throw new Exception("Expresión inválida");
                }

                List<double> numbers = new List<double>();
                List<char> operators = new List<char>();

                string tempNumber = "";

                // Divide la expresión en números y operadores
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

                // Primero manejamos las raíces y potencias
                for (int i = 0; i < operators.Count; i++)
                {
                    if (operators[i] == '√')
                    {
                        // Raíz cuadrada del número actual
                        double result = Math.Sqrt(numbers[i]);
                        numbers[i] = result;

                        // Eliminar el operador raíz
                        operators.RemoveAt(i);
                        i--;
                    }
                    else if (operators[i] == '^')
                    {
                        // Potencia de dos números consecutivos
                        double result = Math.Pow(numbers[i], numbers[i + 1]);
                        numbers[i] = result;

                        // Eliminar el número siguiente (ya utilizado)
                        numbers.RemoveAt(i + 1);

                        // Eliminar el operador potencia
                        operators.RemoveAt(i);
                        i--;
                    }
                }

                // Luego manejamos multiplicaciones y divisiones
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

                        // Eliminar el número siguiente
                        numbers.RemoveAt(i + 1);

                        // Eliminar el operador
                        operators.RemoveAt(i);
                        i--;
                    }
                }

                // Finalmente sumas y restas
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

                // Mostrar el resultado final
                textBoxResultado.Text = numbers[0].ToString();
                StartBlinking(); // Iniciar parpadeo al mostrar el resultado
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

        private void StartBlinking()
        {
            if (!isBlinking)
            {
                blinkTimer.Start();
                isBlinking = true;
            }
        }

        private void StopBlinking()
        {
            if (isBlinking)
            {
                blinkTimer.Stop();
                textBoxResultado.BorderStyle = BorderStyle.FixedSingle; // Restablecer estilo de borde
                textBoxResultado.BackColor = Color.White; // Restablecer color de fondo
                isBlinking = false;
            }
        }

        private bool IsValidExpression(string expression)
        {
            // Asegúrate de que la expresión no empiece con un operador que no sea negativo o raíz cuadrada
            if (symbolsPriorityArray.Contains(expression[0]) && expression[0] != '-' && expression[0] != '√')
                return false;

            // Verifica si hay caracteres inválidos
            foreach (char c in expression)
            {
                if (!char.IsDigit(c) && !symbolsPriorityArray.Contains(c) && c != '.')
                    return false;
            }

            // Verifica que no haya dos operadores seguidos, excepto el signo menos al inicio o después de un operador
            for (int i = 1; i < expression.Length; i++)
            {
                if (symbolsPriorityArray.Contains(expression[i]) && symbolsPriorityArray.Contains(expression[i - 1]) && expression[i - 1] != '-' && expression[i] != '√')
                {
                    return false;
                }
            }

            // Verifica que si hay una raíz, haya un número después
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '√')
                {
                    // Permitir √ al inicio o seguido de un número o decimal
                    if (i + 1 >= expression.Length || (!char.IsDigit(expression[i + 1]) && expression[i + 1] != '.'))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}

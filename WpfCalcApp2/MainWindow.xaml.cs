using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace WpfCalculator
{
    public partial class MainWindow : Window
    {
        private bool _showingSci = false;
        private bool _degMode = true;
        private bool _invMode = false;        // track INV toggle

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Num_Click(object s, RoutedEventArgs e)
        {
            var b = (Button)s;
            if (txtResult.Text == "0") txtResult.Text = b.Content.ToString();
            else txtResult.Text += b.Content;
        }

        private void Dot_Click(object s, RoutedEventArgs e)
        {
            if (!txtResult.Text.Contains(".")) txtResult.Text += ".";
        }

        private void Clear_Click(object s, RoutedEventArgs e)
        {
            txtResult.Text = "0";
            txtExpression.Text = "";
        }

        private void Percent_Click(object s, RoutedEventArgs e)
        {
            if (double.TryParse(txtResult.Text, out double v))
                txtResult.Text = (v / 100).ToString();
        }

        private void Op_Click(object s, RoutedEventArgs e)
        {
            var op = ((Button)s).Content.ToString();
            txtExpression.Text = txtResult.Text + " " + op;
            txtResult.Text = "0";
        }

        private void Equals_Click(object s, RoutedEventArgs e)
        {
            try
            {
                var exp = (txtExpression.Text + " " + txtResult.Text)
                          .Replace("×", "*")
                          .Replace("÷", "/")
                          .Replace("−", "-")
                          .Replace("^", " ^ ");

                if (exp.Contains("^"))
                {
                    var parts = exp.Split(new[] { '^' }, StringSplitOptions.RemoveEmptyEntries);
                    double a = Convert.ToDouble(parts[0]);
                    double b = Convert.ToDouble(parts[1]);
                    txtResult.Text = Math.Pow(a, b).ToString();
                }
                else
                {
                    var dt = new DataTable();
                    var val = dt.Compute(exp, "");
                    txtResult.Text = val.ToString();
                }
                txtExpression.Text = "";
            }
            catch
            {
                txtResult.Text = "Error";
            }
        }

        private void ToggleSci_Click(object s, RoutedEventArgs e)
        {
            _showingSci = !_showingSci;

            if (_showingSci)
            {
                BasicCol.Width = new GridLength(0);
                SciCol.Width = new GridLength(1, GridUnitType.Star);
                BasicPanel.Visibility = Visibility.Collapsed;
                SciPanel.Visibility = Visibility.Visible;
            }
            else
            {
                BasicCol.Width = new GridLength(1, GridUnitType.Star);
                SciCol.Width = new GridLength(0);
                BasicPanel.Visibility = Visibility.Visible;
                SciPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void DegRad_Click(object s, RoutedEventArgs e)
        {
            _degMode = !_degMode;
            btnDeg.Content = _degMode ? "DEG" : "RAD";
        }

        private void Inv_Click(object s, RoutedEventArgs e)
        {
            _invMode = !_invMode;
            // highlight button visually
            btnInv.Background = _invMode ? (System.Windows.Media.Brush)FindResource("BrandBlue") : System.Windows.Media.Brushes.White;
            btnInv.Foreground = _invMode ? System.Windows.Media.Brushes.White : (System.Windows.Media.Brush)FindResource("BrandBlue");
        }

        private void Constant_Click(object s, RoutedEventArgs e)
        {
            var c = ((Button)s).Content.ToString();
            txtResult.Text = (c == "π" ? Math.PI : Math.E).ToString();
        }

        private void Paren_Click(object s, RoutedEventArgs e)
        {
            txtResult.Text += ((Button)s).Content.ToString();
        }

        private void Func_Click(object s, RoutedEventArgs e)
        {
            var f = ((Button)s).Content.ToString();
            if (!double.TryParse(txtResult.Text, out double v))
            {
                txtResult.Text = "Error";
                return;
            }

            double res = v;
            // if INV mode, swap functions
            if (_invMode)
            {
                if (f == "sin") res = Math.Asin(_degMode ? v / 180 * Math.PI : v);
                else if (f == "cos") res = Math.Acos(_degMode ? v / 180 * Math.PI : v);
                else if (f == "tan") res = Math.Atan(_degMode ? v / 180 * Math.PI : v);
                else if (f == "ln") res = Math.Exp(v);
                else if (f == "log") res = Math.Pow(10, v);
                else if (f == "√") res = v * v;                         // inverse of sqrt is square
                else if (f == "!") res = FactorialInverse((int)v);
                else res = v;
            }
            else
            {
                if (f == "sin") res = Math.Sin(_degMode ? v * Math.PI / 180 : v);
                else if (f == "cos") res = Math.Cos(_degMode ? v * Math.PI / 180 : v);
                else if (f == "tan") res = Math.Tan(_degMode ? v * Math.PI / 180 : v);
                else if (f == "ln") res = Math.Log(v);
                else if (f == "log") res = Math.Log10(v);
                else if (f == "√") res = Math.Sqrt(v);
                else if (f == "!") res = Factorial(v < 0 ? -1 : (int)v);
            }

            txtResult.Text = res.ToString();
        }

        // inverse factorial is not exact; approximate with gamma function
        private double FactorialInverse(int n)
        {
            // use Gamma inverse is out of scope; placeholder
            return n; // no-op for now
        }

        private double Factorial(int n)
        {
            if (n < 0) return double.NaN;
            double r = 1;
            for (int i = 1; i <= n; i++) r *= i;
            return r;
        }
    }
}

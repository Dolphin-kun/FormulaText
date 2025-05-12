using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfMath.Parsers;
using WpfMath.Rendering;
using XamlMath;

namespace Formula.Tool
{
    public partial class LaTeXView : UserControl
    {
        public ObservableCollection<FormulaExample> Examples { get; } = [];

        public LaTeXView()
        {
            InitializeComponent();

            Examples.Add(new FormulaExample(@"x^n+y^n=z^n"));
            Examples.Add(new FormulaExample(@"\frac{1}{2}"));
            Examples.Add(new FormulaExample(@"\int_a^b f(x)dx"));
            Examples.Add(new FormulaExample(@"\sum_{i=1}^n i"));
            Examples.Add(new FormulaExample(@"\sqrt{x}"));
            Examples.Add(new FormulaExample(@"\left( \frac{a}{b} \right)"));
            Examples.Add(new FormulaExample(@"\alpha + \beta = \gamma"));
            Examples.Add(new FormulaExample(@"\lim_{x \to 0}"));
            

            this.DataContext = this;
            this.Loaded += TeXView_Loaded;
        }

        public class FormulaExample
        {
            public string Code { get; set; }
            public BitmapSource Rendered { get; }

            public FormulaExample(string code)
            {
                Code = code;
                Rendered = RenderFormula(code);
            }

            private static BitmapSource RenderFormula(string tex)
            {
                try
                {
                    var parser = WpfTeXFormulaParser.Instance;
                    var formula = parser.Parse(tex);
                    var environment = WpfTeXEnvironment.Create(TexStyle.Display, 30.0, "Arial");
                    var originalBitmap = formula.RenderToBitmap(environment, dpi: 300);
                    return originalBitmap;
                }
                catch
                {
                    return new RenderTargetBitmap(1, 1, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
                }
            }
        }

        private void TeXView_Loaded(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);

            if (parentWindow != null)
                parentWindow.Title = "TeX数式 記法リスト";
        }
    }
}

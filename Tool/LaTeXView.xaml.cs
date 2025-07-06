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

            Examples.Add(new FormulaExample("改行", @"a\\b"));

            Examples.Add(new FormulaExample("四則演算", @"a + b - c = d"));
            Examples.Add(new FormulaExample("四則演算", @"a \cdot b \times c \neq d"));
            Examples.Add(new FormulaExample("四則演算", @"\frac{a}{b} \div c = d"));

            Examples.Add(new FormulaExample("カッコ", @"\left( \frac{a}{b} \right)"));
            Examples.Add(new FormulaExample("カッコ", @"\left[ a + b \right]"));
            Examples.Add(new FormulaExample("カッコ", @"\left\{ a \times b \right\}"));

            Examples.Add(new FormulaExample("記号", @"\alpha + \beta = \gamma"));
            Examples.Add(new FormulaExample("記号", @"\int_a^b f(x)dx"));
            Examples.Add(new FormulaExample("記号", @"\sum_{i=1}^n i"));

            Examples.Add(new FormulaExample("サンプル", @"\int_0^{\infty}{x^{2n} e^{-a x^2} \, dx} = \frac{2n-1}{2a} \int_0^{\infty}{x^{2(n-1)} e^{-a x^2} \, dx} = \frac{(2n-1)!!}{2^{n+1}} \sqrt{\frac{\pi}{a^{2n+1}}}"));

            this.DataContext = this;
            this.Loaded += TeXView_Loaded;
        }

        public class FormulaExample
        {
            public string Category { get; set; }
            public string Code { get; set; }
            public BitmapSource Rendered { get; }

            public FormulaExample(string category, string code)
            {
                Category = category;
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
            {
                parentWindow.Title = "TeX数式 記法リスト";
                parentWindow.MinWidth = 700;
                parentWindow.MinHeight = 300;
            }
                
        }
    }
}

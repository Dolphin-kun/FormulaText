using YukkuriMovieMaker.Plugin;

namespace Formula.Tool
{
    class OpenToolWindow : IToolPlugin
    {
        public Type ViewModelType => typeof(OpenToolWindowView);

        public Type ViewType => typeof(LaTeXView);

        public string Name => "LaTeX数式";
    }
}

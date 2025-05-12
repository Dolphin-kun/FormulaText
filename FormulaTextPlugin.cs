using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace Formula
{
    public class SampleShapePlugin : IShapePlugin
    {
        public string Name => "LaTeX数式";

        public bool IsExoShapeSupported => false;

        public bool IsExoMaskSupported => false;

        public IShapeParameter CreateShapeParameter(SharedDataStore? sharedData)
        {
            return new FormulaTextParameter(sharedData);
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace Formula
{
    internal class FormulaTextParameter(SharedDataStore? sharedData) : ShapeParameterBase(sharedData)
    {

        [Display(GroupName = "", Name = "テキスト", Description = "書き方は「ツール」の「TeX数式 記法リスト」から")]
        [TextEditor(AcceptsReturn = true)]
        public string Text { get => text; set => Set(ref text, value); }
        string text = string.Empty;

        [Display(GroupName = "", Name = "サイズ", Description = "文字の大きさ")]
        [AnimationSlider("F1", "px", 1.0, 50.0)]
        public Animation Size { get; } = new Animation(34.0, 1, 1000.0);

        [Display(GroupName = "", Name = "文字色", Description = "文字の色")]
        [ColorPicker]
        public Color Color { get => color; set => Set(ref color, value); }
        Color color = Colors.White;

        public FormulaTextParameter() : this(null)
        {

        }

        public override IEnumerable<string> CreateMaskExoFilter(int keyFrameIndex, ExoOutputDescription desc, ShapeMaskExoOutputDescription shapeMaskDesc)
        {
            return [];
        }

        public override IEnumerable<string> CreateShapeItemExoFilter(int keyFrameIndex, ExoOutputDescription desc)
        {
            return [];
        }

        public override IShapeSource CreateShapeSource(IGraphicsDevicesAndContext devices)
        {
            return new FormulaTextSource(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Size];

        protected override void LoadSharedData(SharedDataStore store)
        {
            var sharedData = store.Load<SharedData>();
            if (sharedData is null)
                return;

            sharedData.CopyTo(this);
        }

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new SharedData(this));
        }

        class SharedData
        {
            public Animation Size { get; } = new Animation(100, 0, 1000);
            public SharedData(FormulaTextParameter param)
            {
                Size.CopyFrom(param.Size);
            }
            public void CopyTo(FormulaTextParameter param)
            {
                param.Size.CopyFrom(Size);
            }
        }
    }
}
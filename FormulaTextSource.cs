using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vortice;
using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.DXGI;
using Vortice.Mathematics;
using Vortice.WIC;
using WpfMath.Parsers;
using WpfMath.Rendering;
using XamlMath;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace Formula
{
    internal class FormulaTextSource : IShapeSource
    {
        readonly private IGraphicsDevicesAndContext devices;
        readonly private FormulaTextParameter formulaTextParameter;

        private double _size;
        private string _text;
        private System.Windows.Media.Color _color;

        ID2D1CommandList? commandList;

        public ID2D1Image Output => commandList ?? throw new Exception($"{nameof(commandList)}がnullです。事前にUpdateを呼び出す必要があります。");

        public FormulaTextSource(IGraphicsDevicesAndContext devices, FormulaTextParameter formulaTextParameter)
        {
            this.devices = devices;
            this.formulaTextParameter = formulaTextParameter;

            _text = string.Empty;
        }


        public void Update(TimelineItemSourceDescription timelineItemSourceDescription)
        {
            var fps = timelineItemSourceDescription.FPS;
            var frame = timelineItemSourceDescription.ItemPosition.Frame;
            var length = timelineItemSourceDescription.ItemDuration.Frame;

            var size = formulaTextParameter.Size.GetValue(frame, length, fps);
            var text = formulaTextParameter.Text;
            var color = formulaTextParameter.Color;

            //サイズが変わっていない場合は何もしない
            if (commandList != null && _size == size && _text != text && _color != color)
                return;

            var dc = devices.DeviceContext;

            commandList?.Dispose();//新規作成前に、前回のCommandListを必ず破棄する
            commandList = dc.CreateCommandList();

            dc.Target = commandList;
            dc.BeginDraw();
            dc.Clear(null);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var formulaBitmap = CreateFormulaBitmap(dc, text, size, color);

                if (formulaBitmap != null)
                {
                    var sourceRect = new RawRectF(
                        0, 0,
                        formulaBitmap.PixelSize.Width,
                        formulaBitmap.PixelSize.Height
                    );

                    var drawRect = new RawRectF(
                        -(float)formulaBitmap.Size.Width / 2,
                        -(float)formulaBitmap.Size.Height / 2,
                         (float)formulaBitmap.Size.Width / 2,
                         (float)formulaBitmap.Size.Height / 2
                    );
                    dc.DrawBitmap(formulaBitmap, drawRect, 1.0f, Vortice.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);
                    formulaBitmap.Dispose();
                }
            }


            dc.EndDraw();
            dc.Target = null;//Targetは必ずnullに戻す。
            commandList.Close();//CommandListはEndDraw()の後に必ずClose()を呼んで閉じる必要がある

            _size = size;
            _text = text;
            _color = color;
        }

        private static ID2D1Bitmap1 CreateFormulaBitmap(ID2D1DeviceContext dc, string tex, double targetSize, System.Windows.Media.Color color)
        {
            try
            {
                var parser = WpfTeXFormulaParser.Instance;
                var formula = parser.Parse(tex);

                var brush = new SolidColorBrush(color);
                var environment = WpfTeXEnvironment.Create(TexStyle.Display, 30.0, "Arial", brush);

                double baseDpi = 300;
                double dynamicDpi = baseDpi * (targetSize / 30.0);
                dynamicDpi = Math.Clamp(dynamicDpi, 1, int.MaxValue);
                var originalBitmap = formula.RenderToBitmap(environment, dpi: dynamicDpi);

                originalBitmap.Freeze();

                var stream = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(originalBitmap));
                encoder.Save(stream);
                stream.Position = 0;

                var imagingFactory = new IWICImagingFactory();
                var decoder = imagingFactory.CreateDecoderFromStream(stream, DecodeOptions.CacheOnLoad);
                var frame = decoder.GetFrame(0);

                var formatConverter = imagingFactory.CreateFormatConverter();
                formatConverter.Initialize(frame, Vortice.WIC.PixelFormat.Format32bppPBGRA);

                var bitmap = dc.CreateBitmapFromWicBitmap(formatConverter);

                stream.Dispose();
                formatConverter.Dispose();
                frame.Dispose();
                decoder.Dispose();

                return bitmap;
            }
            catch (Exception)
            {
                var bmpProps = new BitmapProperties1(
                    new Vortice.DCommon.PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)
                );

                return dc.CreateBitmap(
                    new SizeI(1, 1),
                    IntPtr.Zero,
                    0,
                    bmpProps
                );
            }
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    commandList?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
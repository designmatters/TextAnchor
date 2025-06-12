using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace TextAnchor.DemoApp;

public class AnnotationRenderer(TextDocument document) : IBackgroundRenderer
{
    private readonly TextSegmentCollection<TextSegment> _segments = new(document);
    public KnownLayer Layer => KnownLayer.Selection;

    public void AddHighlight(int offset, int length)
    {
        _segments.Add(new TextSegment { StartOffset = offset, Length = length });
    }

    public void Clear()
    {
        _segments.Clear();
    }

    public void Draw(TextView textView, DrawingContext context)
    {
        if (!textView.VisualLinesValid) return;
        textView.EnsureVisualLines();

        foreach (var segment in _segments)
        {
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
            {
                var padded = rect.Deflate(0.5);
                var borderPen = new Pen(Brushes.LightGray, 1);
                context.DrawRectangle(Brushes.LightGoldenrodYellow, borderPen, padded, 3, radiusY: 3);
            }
        }
    }
}
namespace TextAnchor;

public class Annotator
{
    private const int ContextLenght = 10;

    private int Hash { get; set; }
    public List<Annotation> Annotations { get; set; } = new();


    public void Add(string text, int start, int end)
    {
        if (start < 0 || end <= start || end > text.Length) return;

        Hash = text.GetHashCode();
        var caption = text.Substring(start, end - start);
        var contextBefore = text.Substring(Math.Max(0, start - ContextLenght), Math.Min(ContextLenght, start));
        var contextAfter = text.Substring(end, Math.Min(ContextLenght, text.Length - end));
        Annotations.Add(new Annotation(
            Start: start,
            End: end,
            Caption: caption,
            ContextBefore: contextBefore,
            ContextAfter: contextAfter
        ));
    }

    public void Edit(string newText)
    {
        var newHash = newText.GetHashCode();
        if (Hash == newHash) return;
        Hash = newHash;

        var lostAnnotations = new List<Annotation>();

        DoContextBasedEdit(newText, lostAnnotations);

        foreach (var r in lostAnnotations)
        {
            Annotations.Remove(r);
        }
    }

    private void DoContextBasedEdit(string newText, List<Annotation> annotationsToRemove)
    {
        foreach (var annotation in Annotations)
        {
            // The annotation is unedited and at the same location
            var captionMatch = newText.Substring(annotation.Start, annotation.End - annotation.Start);
            if (captionMatch == annotation.Caption)
                continue;

            // The annotation is unedited but at a different location - try to match it via it's context
            int surroundingLength = 3000000;
            int s = Math.Max(0, annotation.Start - surroundingLength);
            int l = Math.Min((annotation.End - annotation.Start) + 2 * surroundingLength, newText.Length - s);
            string surroundingText = newText.Substring(s, l);
            
            var newStart = 0;
            var newPos =
                surroundingText.IndexOf(annotation.ContextBefore + annotation.Caption + annotation.ContextAfter,
                    StringComparison.Ordinal);
            
            newStart = newPos + annotation.ContextBefore.Length;
            
            if (newPos == -1)
            {
                newPos = surroundingText.IndexOf(annotation.ContextBefore + annotation.Caption,
                    StringComparison.Ordinal);
                newStart = newPos + annotation.ContextBefore.Length;
            }

            if (newPos == -1)
            {
                newPos = surroundingText.IndexOf(annotation.Caption + annotation.ContextAfter,
                    StringComparison.Ordinal);
                newStart = newPos;
            }

            if (newPos != -1)
            {
                annotation.Start = newStart;
                annotation.End = annotation.Start + annotation.Caption.Length;
                var contextBefore =
                    newText.Substring(Math.Max(0, annotation.Start - 10), Math.Min(10, annotation.Start));
                var afterLength = Math.Min(10, newText.Length - annotation.End);
                var contextAfter = newText.Substring(annotation.End, afterLength);
                annotation.ContextBefore = contextBefore;
                annotation.ContextAfter = contextAfter;
            }
            else
            {
                //TODO: try to keep them via fuzzy matching.
                annotationsToRemove.Add(annotation);
            }
        }
    }
}
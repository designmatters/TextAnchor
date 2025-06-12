namespace TextAnchor;

public record Annotation(int Start, int End, string Caption, string ContextBefore, string ContextAfter)
{
    public int Start { get; set; } = Start;
    public int End { get; set; } = End;
    public string Caption { get; set; } = Caption;
    public string ContextBefore { get; set; } = ContextBefore;
    public string ContextAfter { get; set; } = ContextAfter;

    public override string ToString()
    {
        return $"[{Start}-{End}] \"{Caption}\"\nContext: \"{ContextBefore}⟨{Caption}⟩{ContextAfter}\"";
    }
}
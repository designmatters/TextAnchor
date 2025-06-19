using Xunit;

namespace TextAnchor.Tests.Unit;

public class Basics
{
    [Fact]
    public void Add_StoresAnnotation()
    {
        var annotator = new Annotator();
        annotator.Add("Hello world", 6, 11); //world

        var annotations = annotator.Annotations.ToList();

        Assert.Single(annotations);
        Assert.Equal(6, annotations[0].Start);
        Assert.Equal(11, annotations[0].End);
        Assert.Equal("world", annotations[0].Caption);
        Assert.Equal("Hello ", annotations[0].ContextBefore);
        Assert.Equal("", annotations[0].ContextAfter);
    }

    [Fact]
    public void Edit_ChangeBehindAnnotation_Unchanged()
    {
        var annotator = new Annotator();
        var original = "The quick brown fox";
        annotator.Add(original, 4, 9); // "quick"

        var updated = "The quick black fox";
        annotator.Edit(updated);

        var annotation = annotator.Annotations.First();

        Assert.Equal("quick", annotation.Caption);
        Assert.Equal(4, annotation.Start);
        Assert.Equal(9, annotation.End);
    }

    [Fact]
    public void Edit_ChangeBeforeAnnotation_Updated()
    {
        var annotator = new Annotator();
        annotator.Add("The quick brown fox", 4, 9); // quick

        annotator.Edit("The    quick brown fox");

        var annotation = annotator.Annotations.First();

        Assert.Equal("quick", annotation.Caption);
        Assert.Equal(7, annotation.Start);
        Assert.Equal(12, annotation.End);
    }

    [Fact]
    public void Edit_ChangesSimilarToCaption_Updated()
    {
        var annotator = new Annotator();
        annotator.Add("The quick quick quick brown fox", 4, 9); // quick

        annotator.Edit("The quick     quick quick brown fox");

        var annotation = annotator.Annotations.First();

        Assert.Equal("quick", annotation.Caption);
        Assert.Equal(4, annotation.Start);
        Assert.Equal(9, annotation.End);
    }

    [Fact]
    public void Edit_ChangesSimilarToContext_Updated()
    {
        var annotator = new Annotator();
        annotator.Add("The quick quick quick brown fox", 10, 15); // second quick

        annotator.Edit("The quick     quick quick brown fox");

        var annotation = annotator.Annotations.First();

        Assert.Equal("quick", annotation.Caption);
        Assert.Equal(14, annotation.Start);
        Assert.Equal(19, annotation.End);
    }

    [Fact]
    public void Edit_ChangesInsideAnnotation_Removed()
    {
        var annotator = new Annotator();
        annotator.Add("Hello world", 6, 11); // "world"

        annotator.Edit("Hello woXrld");

        var annotation = annotator.Annotations.FirstOrDefault();

        Assert.Null(annotation);
    }

    
    [Fact]
    public void Edit_RepetitiveCaptionAndContext_BUG()
    {
        var text = "this is it this is it this is it this is it";
        var annotator = new Annotator();

        annotator.Add(text, 11, 21); // 2nd "this is it

        annotator.Edit("this isxxit this is it this is it this is it");

        var annotation = annotator.Annotations.First();
        
        Assert.Equal("this is it", annotation.Caption);
        Assert.Equal(12, annotation.Start);
    }
}
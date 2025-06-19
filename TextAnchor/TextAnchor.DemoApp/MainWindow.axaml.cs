using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaEdit;

namespace TextAnchor.DemoApp;

public partial class MainWindow : Window
{
    private string TextInEditor { get; set; }
    private readonly Annotator? _annotator = new();
    private readonly AnnotationRenderer? _renderer;

    public MainWindow()
    {
        InitializeComponent();
        
        DataContext = this;

        _renderer = new AnnotationRenderer(MyEditor.Document);

        SetupExampleData();

        MyEditor.Text = TextInEditor;
        MyEditor.TextArea.TextView.BackgroundRenderers.Add(_renderer);
        MyEditor.TextArea.TextView.InvalidateVisual();
        UpdateRenderer();
    }

    private void SetupExampleData()
    {
         TextInEditor = """
                        Met 5.034 waren ze, de burgeronderzoekers die tussen 26 april en 4 mei een waterstaal namen uit een kanaal, rivier, gracht of beek in de buurt. Met die stalen werd de aanwezigheid van de E. colibacterie in onze waterlopen gemeten. Of er veel of weinig van die bacterie in het water zit, bepaalt in grote mate of een waterloop veilig genoeg is om in te zwemmen.

                        De stalen leverden 5.772 bruikbare metingen op, uitgevoerd in 1.893 verschillende waterlopen, in elke Vlaamse gemeente (behalve Herstappe en Mesen) en in elf van de negentien Brusselse gemeenten. In 300 waterlopen werd minstens vijf keer gemeten, op verschillende plaatsen.
                        """;

         _annotator.Add(TextInEditor, 4, 20);
         _annotator.Add(TextInEditor, 83, 118);
         _annotator.Add(TextInEditor, 167, 188);
         _annotator.Add(TextInEditor, 294, 334);
    }

    private bool _suspendRendering = false;

    private void UpdateRenderer()
    {
        if (_suspendRendering || _annotator == null || _renderer == null)
            return;

        _suspendRendering = true;
        _renderer.Clear();

        foreach (var annotation in _annotator.Annotations)
        {
            _renderer.AddHighlight(annotation.Start, annotation.End - annotation.Start);
        }

        _suspendRendering = false;
        MyAnnotationsList.ItemsSource = null;
        MyAnnotationsList.ItemsSource = _annotator.Annotations;
    }

    private void MyEditor_OnTextChanged(object? sender, EventArgs e)
    {
        if (sender is not TextEditor editor) return;

        _annotator.Edit(editor.Text);

        UpdateRenderer();
    }
}

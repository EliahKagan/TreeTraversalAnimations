<Query Kind="Statements">
  <NuGetReference>AutomaticGraphLayout.GraphViewerGDI</NuGetReference>
  <Namespace>Microsoft.Msagl.Drawing</Namespace>
  <Namespace>Microsoft.Msagl.GraphViewerGdi</Namespace>
  <Namespace>Size = System.Drawing.Size</Namespace>
  <Namespace>SizeF = System.Drawing.SizeF</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
</Query>

// TreeTraversalAnimations.linq - Simple binary-tree traversal visualizer.

#nullable enable

const bool repeatForever = true;

var root = N("A",
             N("B",
               L("D"), L("E")),
             N("C",
               N("F",
                 L("H"), L("I")),
                 L("G")));

var tree = new DrawingTree(root) {
    WidthScaleFactor = 0.8f,
    HeightScaleFactor = 1.2f,
};

var legend = new WebBrowser {
    Url = GetDocUrl("legend.html"),
    AutoSize = true,
};
legend.DocumentCompleted += delegate {
    var oldSize = legend.Document.Body.ScrollRectangle.Size;
    var newSize = new SizeF(width: oldSize.Width * 0.8f,
                            height: oldSize.Height * 1.1f);
    legend.Size = Size.Round(newSize);
};

var ui = new TableLayoutPanel {
    RowCount = 1,
    ColumnCount = 2,
    GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
    AutoSize = true,
    AutoSizeMode = AutoSizeMode.GrowAndShrink,
    AutoScroll = true,
};
ui.Controls.Add(legend);
ui.Controls.Add(tree.Viewer);
ui.Dump("Tree Traversal Animation");

do {
    await tree.PreorderLeftToRightRecursive(root);
    await tree.PreorderRightToLeftIterative(root);
    await tree.LevelOrderLeftToRight(root);
} while (repeatForever);

static Uri GetDocUrl(string filename)
    => new(Path.Combine(GetQueryDirectory(), filename));

static string GetQueryDirectory()
    => Path.GetDirectoryName(Util.CurrentQueryPath)
        ?? throw new NotSupportedException("Can't find query directory");

static TreeNode<T> N<T>(T key, TreeNode<T>? left, TreeNode<T>? right)
    => new TreeNode<T>(key, left, right);

static TreeNode<T> L<T>(T key) => N(key, null, null);

internal sealed record TreeNode<T>(T Key,
                                   TreeNode<T>? Left,
                                   TreeNode<T>? Right);

internal sealed class DrawingTree {
    internal DrawingTree(TreeNode<string> root)
    {
        Viewer = new() { Graph = _graph };
        Viewer.HandleCreated += viewer_HandleCreated;
        BuildTreeGraph(root);
    }

    internal float WidthScaleFactor
    {
        init => Viewer.Width = Convert.ToInt32(Viewer.Width * value);
    }

    internal float HeightScaleFactor
    {
        init => Viewer.Height = Convert.ToInt32(Viewer.Height * value);
    }

    internal GViewer Viewer { get; }

    internal async Task PreorderLeftToRightRecursive(TreeNode<string>? root)
    {
        if (root is null) return;

        await HighlightNodeAsync(root, Color.Orange);
        await PreorderLeftToRightRecursive(root.Left);
        await PreorderLeftToRightRecursive(root.Right);
    }

    internal async Task PreorderRightToLeftIterative(TreeNode<string>? root)
    {
        var stack = new Stack<TreeNode<string>?>();

        for (stack.Push(root); stack.Count != 0; ) {
            var node = stack.Pop();
            if (node is null) continue;

            await HighlightNodeAsync(node, Color.Red);
            stack.Push(node.Left);
            stack.Push(node.Right);
        }
    }

    internal async Task LevelOrderLeftToRight(TreeNode<string>? root)
    {
        var queue = new Queue<TreeNode<string>?>();

        for (queue.Enqueue(root); queue.Count != 0; ) {
            var node = queue.Dequeue();
            if (node is null) continue;

            await HighlightNodeAsync(node, Color.Blue);
            queue.Enqueue(node.Left);
            queue.Enqueue(node.Right);
        }
    }

    private sealed class Mutator : IDisposable {
        internal Mutator(GViewer viewer)
        {
            _viewer = viewer;
            _graph = _viewer.Graph;
            _viewer.Graph = null;
        }

        public void Dispose() => _viewer.Graph = _graph;

        private readonly GViewer _viewer;

        private readonly Graph _graph;
    }

    private static int ThreadId => Thread.CurrentThread.ManagedThreadId;

    private static void viewer_HandleCreated(object? sender, EventArgs e)
        => $"Graph viewer handle created on thread {ThreadId}.".Dump();

    private void BuildTreeGraph(TreeNode<string> root)
    {
        var queue = new Queue<(TreeNode<string>? parent,
                               TreeNode<string>? child)>();

        for (queue.Enqueue((null, root)); queue.Count != 0; ) {
            var (parent, child) = queue.Dequeue();
            if (child is null) continue;

            if (parent is not null) AddEdge(parent, child);

            queue.Enqueue((child, child.Right));
            queue.Enqueue((child, child.Left));
        }
    }

    private void AddEdge(TreeNode<string> parent, TreeNode<string> child)
    {
        $"Adding Edge {parent.Key} -> {child.Key}".Dump();

        using var mutator = new Mutator(Viewer);
        _graph.AddEdge(parent.Key, child.Key);
    }

    private async Task HighlightNodeAsync(TreeNode<string> node,
                                          Color flashColor)
    {
        var vertex = _graph.FindNode(node.Key);

        void SetColor(Color color)
        {
            using var mutator = new Mutator(Viewer);
            vertex.Attr.FillColor = color;
        }

        SetColor(flashColor);
        await Task.Delay(1000);
        SetColor(Color.White);

        if (_firstHighlight) {
            _firstHighlight = false;
            $"Highlighting on thread {ThreadId}.".Dump();
        }
    }

    private readonly Graph _graph = new();

    private bool _firstHighlight = true;
}

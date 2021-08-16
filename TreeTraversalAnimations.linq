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
//
// Copyright (C) 2020 Eliah Kagan <degeneracypressure@gmail.com>
//
// Permission to use, copy, modify, and/or distribute this software for any
// purpose with or without fee is hereby granted.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
// SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION
// OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
// CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

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
    await tree.InorderLeftToRightRecursive(root);
    await tree.PostorderLeftToRightRecursive(root);
    await tree.GeneralLeftToRightRecursive(root);

    await Pause();

    await tree.PreorderRightToLeftIterative(root);
    await tree.LevelOrderLeftToRight(root);

    await Pause();

    await tree.PreorderLeftToRightIterativeAlt(root);
    await tree.InorderLeftToRightIterativeAlt(root);
    await tree.PostorderLeftToRightIterativeAlt(root);
    await tree.GeneralLeftToRightIterativeAlt(root);

    await Pause();

    await tree.PreorderLeftToRightIterativeRec(root);
    await tree.InorderLeftToRightIterativeRec(root);
    await tree.PostorderLeftToRightIterativeRec(root);
    await tree.GeneralLeftToRightIterativeRec(root);

    await Pause();
} while (repeatForever);

static Uri GetDocUrl(string filename)
    => new(Path.Combine(GetQueryDirectory(), filename));

static string GetQueryDirectory()
    => Path.GetDirectoryName(Util.CurrentQueryPath)
        ?? throw new NotSupportedException("Can't find query directory");

static async Task Pause() => await Task.Delay(2000);

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

    internal async Task InorderLeftToRightRecursive(TreeNode<string>? root)
    {
        if (root is null) return;

        await InorderLeftToRightRecursive(root.Left);
        await HighlightNodeAsync(root, Color.Gold);
        await InorderLeftToRightRecursive(root.Right);
    }

    internal async Task PostorderLeftToRightRecursive(TreeNode<string>? root)
    {
        if (root is null) return;

        await PostorderLeftToRightRecursive(root.Left);
        await PostorderLeftToRightRecursive(root.Right);
        await HighlightNodeAsync(root, Color.YellowGreen);
    }

    internal async Task GeneralLeftToRightRecursive(TreeNode<string>? root)
    {
        if (root is null) return;

        await HighlightNodeAsync(root, Color.Orange);
        await GeneralLeftToRightRecursive(root.Left);
        await HighlightNodeAsync(root, Color.Gold);
        await GeneralLeftToRightRecursive(root.Right);
        await HighlightNodeAsync(root, Color.YellowGreen);
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

    internal async Task PreorderLeftToRightIterativeAlt(TreeNode<string>? root)
    {
        var stack = new Stack<TreeNode<string>>();

        while (stack.Count != 0 || root is not null) {
            // Go left as far as possible, performing the preorder action.
            for (; root is not null; root = root.Left) {
                await HighlightNodeAsync(root, Color.CornflowerBlue);
                stack.Push(root);
            }

            var cur = stack.Pop();

            // If there is a right subtree, visit that next.
            root = cur.Right;
        }
    }

    internal async Task InorderLeftToRightIterativeAlt(TreeNode<string>? root)
    {
        var stack = new Stack<TreeNode<string>>();

        while (stack.Count != 0 || root is not null) {
            // Go left as far as possible.
            for (; root is not null; root = root.Left) stack.Push(root);

            var cur = stack.Pop();

            // Do the inorder action.
            await HighlightNodeAsync(cur, Color.Chartreuse);

            // If there is a right subtree, visit that next.
            root = cur.Right;
        }
    }

    internal async Task
    PostorderLeftToRightIterativeAlt(TreeNode<string>? root)
    {
        var stack = new Stack<TreeNode<string>>();
        TreeNode<string>? post = null;

        while (stack.Count != 0 || root is not null) {
            // Go left as far as possible.
            for (; root is not null; root = root.Left) stack.Push(root);

            var cur = stack.Peek();

            if (cur.Right is not null && cur.Right != post) {
                // The right subtree is nonempty and unvisited. Go there next.
                root = cur.Right;
            } else {
                // The right subtree is empty or already explored.
                // Do the postorder action and retreat.
                post = cur;
                await HighlightNodeAsync(post, Color.DarkSalmon);
                stack.Pop();
            }
        }
    }

    internal async Task GeneralLeftToRightIterativeAlt(TreeNode<string>? root)
    {
        var stack = new Stack<TreeNode<string>>();
        TreeNode<string>? post = null;

        while (stack.Count != 0 || root is not null) {
            // Go left as far as possible, doing the preorder action.
            for (; root is not null; root = root.Left) {
                await HighlightNodeAsync(root, Color.CornflowerBlue);
                stack.Push(root);
            }

            var cur = stack.Peek();

            if (cur.Right is null || cur.Right != post) {
                // We've been left but not right. Do the inorder action.
                await HighlightNodeAsync(cur, Color.Chartreuse);
            }

            if (cur.Right is not null && cur.Right != post) {
                // The right subtree is nonempty and unvisited. Go there next.
                root = cur.Right;
            } else {
                // The right subtree is empty or already explored.
                // Do the postorder action and retreat.
                post = cur;
                await HighlightNodeAsync(post, Color.DarkSalmon);
                stack.Pop();
            }
        }
    }

    internal async Task PreorderLeftToRightIterativeRec(TreeNode<string>? root)
    {
        var stack = new Stack<Frame<string>>();

        for (stack.Push(new(root, State.GoLeft)); stack.Count != 0; ) {
            var frame = stack.Peek();

            switch (frame.State) {
            case State.GoLeft:
                if (frame.Node is null) {
                    stack.Pop();
                    continue;
                }

                await HighlightNodeAsync(frame.Node, Color.Cyan);
                frame.State = State.GoRight;
                stack.Push(new(frame.Node.Left, State.GoLeft));
                break;

            case State.GoRight:
                Debug.Assert(frame.Node is not null);

                frame.State = State.Retreat;
                stack.Push(new(frame.Node.Right, State.GoLeft));
                break;

            case State.Retreat:
                Debug.Assert(frame.Node is not null);

                stack.Pop();
                break;

            default:
                throw new NotSupportedException("Bug: invalid state");
            }
        }
    }

    internal async Task InorderLeftToRightIterativeRec(TreeNode<string>? root)
    {
        var stack = new Stack<Frame<string>>();

        for (stack.Push(new(root, State.GoLeft)); stack.Count != 0; ) {
            var frame = stack.Peek();

            switch (frame.State) {
            case State.GoLeft:
                if (frame.Node is null) {
                    stack.Pop();
                    continue;
                }

                frame.State = State.GoRight;
                stack.Push(new(frame.Node.Left, State.GoLeft));
                break;

            case State.GoRight:
                Debug.Assert(frame.Node is not null);

                await HighlightNodeAsync(frame.Node, Color.Yellow);
                frame.State = State.Retreat;
                stack.Push(new(frame.Node.Right, State.GoLeft));
                break;

            case State.Retreat:
                Debug.Assert(frame.Node is not null);

                stack.Pop();
                break;

            default:
                throw new NotSupportedException("Bug: invalid state");
            }
        }
    }

    internal async Task
    PostorderLeftToRightIterativeRec(TreeNode<string>? root)
    {
        var stack = new Stack<Frame<string>>();

        for (stack.Push(new(root, State.GoLeft)); stack.Count != 0; ) {
            var frame = stack.Peek();

            switch (frame.State) {
            case State.GoLeft:
                if (frame.Node is null) {
                    stack.Pop();
                    continue;
                }

                frame.State = State.GoRight;
                stack.Push(new(frame.Node.Left, State.GoLeft));
                break;

            case State.GoRight:
                Debug.Assert(frame.Node is not null);

                frame.State = State.Retreat;
                stack.Push(new(frame.Node.Right, State.GoLeft));
                break;

            case State.Retreat:
                Debug.Assert(frame.Node is not null);

                await HighlightNodeAsync(frame.Node, Color.Magenta);
                stack.Pop();
                break;

            default:
                throw new NotSupportedException("Bug: invalid state");
            }
        }
    }

    internal async Task GeneralLeftToRightIterativeRec(TreeNode<string>? root)
    {
        var stack = new Stack<Frame<string>>();

        for (stack.Push(new(root, State.GoLeft)); stack.Count != 0; ) {
            var frame = stack.Peek();

            switch (frame.State) {
            case State.GoLeft:
                if (frame.Node is null) {
                    stack.Pop();
                    continue;
                }

                await HighlightNodeAsync(frame.Node, Color.Cyan);
                frame.State = State.GoRight;
                stack.Push(new(frame.Node.Left, State.GoLeft));
                break;

            case State.GoRight:
                Debug.Assert(frame.Node is not null);

                await HighlightNodeAsync(frame.Node, Color.Yellow);
                frame.State = State.Retreat;
                stack.Push(new(frame.Node.Right, State.GoLeft));
                break;

            case State.Retreat:
                Debug.Assert(frame.Node is not null);

                await HighlightNodeAsync(frame.Node, Color.Magenta);
                stack.Pop();
                break;

            default:
                throw new NotSupportedException("Bug: invalid state");
            }
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
        await Task.Delay(700);
        SetColor(Color.White);

        if (_firstHighlight) {
            _firstHighlight = false;
            $"Highlighting on thread {ThreadId}.".Dump();
        }
    }

    private readonly Graph _graph = new();

    private bool _firstHighlight = true;
}

internal sealed class Frame<T> {
    internal Frame(TreeNode<T>? node, State state)
        => (Node, State) = (node, state);

    internal TreeNode<T>? Node { get; }

    internal State State { get; set; }
}

internal enum State {
    GoLeft,
    GoRight,
    Retreat,
}

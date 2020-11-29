<Query Kind="Statements">
  <NuGetReference>AutomaticGraphLayout.GraphViewerGDI</NuGetReference>
</Query>

using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

var root = N("A", N("B", L("D"), L("E")), N("C", L("F"), L("G")));
var tree = new DrawingTree(root);
tree.Show();

await tree.PreorderTraverseRecursive(root);
await tree.PreorderTraverseIterative(root);
await tree.LevelOrderTraverse(root);

static TreeNode<T> N<T>(T key, TreeNode<T>? left, TreeNode<T>? right)
    => new TreeNode<T>(key, left, right);

static TreeNode<T> L<T>(T key) => N(key, null, null);

internal sealed record TreeNode<T>(T Key,
                                   TreeNode<T>? Left,
                                   TreeNode<T>? Right);

internal sealed class DrawingTree {
    internal DrawingTree(TreeNode<string> root)
    {
        _viewer = new() { Graph = _graph };
        BuildTreeGraph(root);
    }

    internal void Show() => _viewer.Dump();

    internal async Task PreorderTraverseRecursive(TreeNode<string>? root)
    {
        if (root is null) return;

        await HighlightNodeAsync(root, Color.Magenta);
        await PreorderTraverseRecursive(root.Left);
        await PreorderTraverseRecursive(root.Right);
    }

    internal async Task PreorderTraverseIterative(TreeNode<string>? root)
    {
        var stack = new Stack<TreeNode<string>?>();
        
        for (stack.Push(root); stack.Count != 0; ) {
            var node = stack.Pop();
            if (node is null) continue;
            
            await HighlightNodeAsync(node, Color.Green);
            stack.Push(node.Right);
            stack.Push(node.Left);
        }
    }

    internal async Task LevelOrderTraverse(TreeNode<string>? root)
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

        using var mutator = new Mutator(_viewer);
        _graph.AddEdge(parent.Key, child.Key);
    }

    private async Task HighlightNodeAsync(TreeNode<string> node,
                                          Color flashColor)
    {
        var vertex = _graph.FindNode(node.Key);
        
        void SetColor(Color color)
        {
            using var mutator = new Mutator(_viewer);
            vertex.Attr.FillColor = color;
        }
        
        SetColor(flashColor);
        await Task.Delay(1000);
        SetColor(Color.White);
    }

    private readonly Graph _graph = new();
    
    private readonly GViewer _viewer;
}

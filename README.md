# TreeTraversalAnimations - Simple binary-tree traversal visualizer

TreeTraversalAnimations visualizes binary-tree traversal by several techniques.
Its purpose is to illustrate different techniques, how they affect the order in
which tree nodes are visited, and similarities and differences between their
implementations. The tree is drawn and animated using [Microsoft Automatic
Graph Layout](https://github.com/microsoft/automatic-graph-layout).

This program uses [asynchronous programming via `async` and
`await`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/)
to implement tree-traversal algorithms straightforwardly, even though they are
being suspended and resumed so that the UI thread remains responsive (see the
`await` expression in `DrawingTree.HighlightNodeAsync`).

**This program is related, conceptually, to
[Flood](https://github.com/EliahKagan/Flood).** This program is simpler and
much less visually interesting than Flood, but since it is simpler, it may be
easier to understand. I created this program originally as a way to introduce
people to the concepts of Flood, particularly the way it uses task-based
asynchronous programming with `async` and `await`, with single-threaded
concurrency, to keep algorithm implementations conceptually clear while
animating their changing state and sharing the UI thread with other operations.

TreeTraversalAnimations involves substantially different subject matter as
Flood, though (even though both are special cases of graph traversal). This
program demonstrates most of the major non-mutating techniques for traversing a
non-threaded binary tree without parent pointers: straightforward recursion;
two kinds of iterative techniques; and iterative implementations of recursive
techniques, using a state machine.

It does *not* cover mutating techniques such as [Morris
traversal](https://en.wikipedia.org/wiki/Tree_traversal#Morris_in-order_traversal_using_threading),
even though such approaches are sometimes useful and would be quite interesting
to visualize, since they modify the tree while it is being traversed. Perhaps a
later version could cover that as well.

## License

This program is licensed under [0BSD](https://spdx.org/licenses/0BSD.html). See
[`LICENSE`](LICENSE).

It retrieves the MSAGL components it needs from NuGet; MSAGL is licensed under
the [MIT
license](https://github.com/microsoft/automatic-graph-layout/blob/master/LICENSE).
No dependencies are distributed in this repository&mdash;all files here are
licensed under 0BSD.

For convenience, these are also included below. See [Notices](#Notices).

## How to Run

TreeTraversalAnimations only runs on Windows, because it uses
[LINQPad](https://www.linqpad.net/) and [Windows
Forms](https://en.wikipedia.org/wiki/Windows_Forms). (It also uses MSAGL in a
Windows-specific way, but MSAGL can be used on other systems.)

[Install LINQPad 6](https://www.linqpad.net/Download.aspx) if you don&rsquo;t
have it. You&rsquo;ll also need [.NET
5](https://dotnet.microsoft.com/download/dotnet/5.0) or [.NET
Core](https://dotnet.microsoft.com/download/dotnet/3.1), but LINQPad will
prompt to install one of those if you don&rsquo;t have them.

Then open `TreeTraversalAnimations.linq` in LINQPad 6. It may prompt you to ask
if it may retrieve a NuGet package for MSAGL, which TreeTraversalAnimations
requires.

Then run the query (<kbd>F5</kbd>). **I recommend arranging panels
vertically.** You can use <kbd>Ctrl</kbd>+<kbd>F8</kbd> to toggle that at any
time, including while TreeTraversalAnimations is running.

You can select the *simple mode* for fewer algorithms (and slightly slower
animation) or the *full mode* for more algorithms.

## Algorithms Implemented

In this section, references to methods in the code are to members of the
`DrawingTree` class, unless otherwise indicated. For example,
`PreorderLeftToRightRecursive` refers to
`DrawingTree.PreorderLeftToRightRecursive`.

### In simple mode

Simple mode showcases these algorithms:

- Recursive left-to-right preorder traversal (`PreorderLeftToRightRecursive`).
- Iterative right-to-left preorder traversal with a stack of nodes, in the
  usual way (`PreorderRightToLeftIterative`).
- Left-to-right level-order traversal with a queue of nodes, which is also
  iterative (`LevelOrderLeftToRight`).

Iterative preorder traversal is implemented right-to-left to avoid obscuring
its relationship to level-order traversal&mdash;that they can be, and in this
program are, written with the same code, except that iterative preorder
traversal uses a stack while level-order traversal uses a queue. However,
**iterative preorder traversal using this technique is also trivial to
implement left-to-right**: simply push child nodes to the stack in the opposite
order.

### In full mode

Full mode showcases substantially more algorithms. They fall into several groups.

#### Recursively implemented

Depth-first left-to-right traversals implemented by straightforward recursion:

- Recursive left-to-right preorder traversal (`PreorderLeftToRightRecursive`).
- Recursive left-to-right inorder traversal (`InorderLeftToRightRecursive`).
- Recursive left-to-right postorder traversal
  (`PostorderLeftToRightRecursive`).
- Recursive left-to-right general depth-first traversal, in which preorder,
  inorder, and postorder actions are all performed
  (`GeneralLeftToRightRecursive`).

#### Simple iterative preorder and level-order traversals

Simply implemented iterative traversals with a &ldquo;fringe&rdquo; data
structure into which children are placed, left child followed by right child:

- Iterative right-to-left preorder traversal with a stack of nodes, in the
  usual way (`PreorderRightToLeftIterative`).
- Left-to-right level-order traversal with a queue of nodes, which is also
  iterative (`LevelOrderLeftToRight`).

These are the second and third traversals showcased [in simple
mode](#in-simple-mode). See that section for more details.

#### Iterative pre-, in-, and postoder depth-first traversals by another technique

These are depth-first left-to-right iterative traversals implemented using a
different technique than the one most commonly used for iterative preorder
traversal. This technique is harder to understand but also somewhat more
general&mdash;it extends readily to iterative and postorder traversal. Note
that it is only for purposes of demonstration that these are left-to-right (any
technique can go in the other direction by visiting child nodes in the other
order).

- Iterative left-to-right preorder traversal with a stack of nodes, alternative
  technique (`PreorderLeftToRightIterativeAlt`).
- Iterative left-to-right inorder traversal with a stack of nodes
  (`InorderLeftToRightIterativeAlt`).
- Iterative left-to-right postorder traversal with a stack of nodes
  (`PostorderLeftToRightIterativeAlt`).
- Iterative left-to-right general depth-first traversal, in which preorder,
  inorder, and postorder actions are all performed
  (`GeneralLeftToRightIterativeAlt`).

Although I gave all those methods names ending in `Alt` to avoid obscuring
their close relationship to each other, it&rsquo;s really only the preorder
traversal that is an alternative to a more common implementation.

These iterative techniques more closely resemble how the recursive algorithm
works, though they are not the same as the recursive algorithm.

#### Iterative implementations of the recursive algorithms

These are depth-first iteratively implementations using state machines that
represent the process of making recursive calls. The stack is not of nodes, but
instead of *frames*, where each frame holds a reference to a node as well as a
datum indicating where to resume. Conceptually, the node field represents a
function parameter or local variable, while the state field represents an
instruction pointer.

- Iteratively implemented recursive left-to-right preorder traversal with a
  state machine (`PreorderLeftToRightIterativeRec`).
- Iteratively implemented recursive left-to-right inorder traversal with a
  state machine (`InorderLeftToRightIterativeRec`).
- Iteratively implemented recursive left-to-right postorder traversal with a
  state machine (`PostorderLeftToRightIterativeRec`).
- Iteratively implemented recursive left-to-right general depth-first traversal
  with a state machine, in which preorder, inorder, and postorder actions are
  all performed (`GeneralLeftToRightIterativeRec`).

These are actually the recursive *algorithms*, but with iterative
*implementations*. Notice that their implementations all resemble one another
very closely, in the same manner as the straightforward recursive
implementations all resemble one another (though these state-machine
implementations take considerably more code, since they are achieving recursion
without using the language feature for it).

This is not merely a curiosity&mdash;it has some useful applications, including
implementing recursive algorithms when the maximum safe stack depth might [be
exceeded](https://en.wikipedia.org/wiki/Stack_overflow) if actual recursion
were used. That can happen with binary trees, albeit not if they are
[self-balancing](https://en.wikipedia.org/wiki/Self-balancing_binary_search_tree).

This is related to the concept of [continuation-passing
style](https://en.wikipedia.org/wiki/Continuation-passing_style), though that
is far more general. [Roslyn](https://en.wikipedia.org/wiki/Roslyn_(compiler)),
the C# compiler, uses state-machine based implementations for iterators
(implementing `IEnumerable`/`IEnumerable<T>`) and asynchronous methods
(implementing `Task`/`Task<T>` and other such types); it converts code from
direct style into continuation-passing style to do this.

This program uses asynchronous programming to seamlessly interleave traversals
with UI operations. So it turns out that the simple recursive implementations
work somewhat like these state machines, in the
[CIL](https://en.wikipedia.org/wiki/Common_Intermediate_Language) code
generated by Roslyn.

## Notices

As [mentioned above](#license), this program [**is licensed**](LICENSE) under
[0BSD](https://spdx.org/licenses/0BSD.html):

> Copyright (C) 2020, 2021 Eliah Kagan <degeneracypressure@gmail.com>
>
> Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted.
>
> THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM
LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
PERFORMANCE OF THIS SOFTWARE.

This program uses the [Microsoft Automatic Graph Layout
(MSAGL)](https://github.com/microsoft/automatic-graph-layout) library, which
[**is
licensed**](https://github.com/microsoft/automatic-graph-layout/blob/master/LICENSE)
under the [MIT license](https://spdx.org/licenses/MIT.html):

> Microsoft Automatic Graph Layout,MSAGL 
>
> Copyright (c) Microsoft Corporation
>
> All rights reserved. 
>
> MIT License 
>
> Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
""Software""), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:
>
> The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.
>
> THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

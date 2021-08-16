<Query Kind="Statements">
  <Namespace>LC = LINQPad.Controls</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// launcher.linq - Launcher implementation for TreeTraversalAnimations.
// You should run TreeTraversalAnimations.linq instead of this file.
//
// Copyright (C) 2021 Eliah Kagan <degeneracypressure@gmail.com>
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

Util.CreateSynchronizationContext();

static Task<bool> RunLauncherAsync()
{
    var tcs = new TaskCompletionSource<bool>();

    var simpleMode = new LC.RadioButton(groupName: "mode-option",
                                        text: "Simple (fewer algorithms)",
                                        isChecked: true);

    var fullMode = new LC.RadioButton(groupName: "mode-option",
                                      text: "Full (more algorithms)",
                                      isChecked: false);

    var launchedLabel = new LC.Label {
        Text = "(Launched. You can stop and re-run the LINQPad query to"
                + " re-enable the launcher.)",
        Visible = false,
    };

    var launch = new LC.Button("Launch!", sender => {
        sender.Enabled = false;
        simpleMode.Enabled = false;
        fullMode.Enabled = false;

        launchedLabel.Visible = true;
        tcs.SetResult(fullMode.Checked);
    });

    new LC.StackPanel(horizontal: false,
        new LC.FieldSet("Select mode",
            new LC.StackPanel(horizontal: false, simpleMode, fullMode)),
        new LC.StackPanel(horizontal: false, launch, launchedLabel))
    .Dump("Launcher");

    return tcs.Task;
}

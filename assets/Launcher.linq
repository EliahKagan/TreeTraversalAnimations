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

#load "./Styling.linq"

#nullable enable

Util.CreateSynchronizationContext();

internal static class Launcher {
    internal static Task<bool> RunAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        ShowLauncher(tcs);
        ShowLicenses();
        return tcs.Task;
    }

    private static void ShowLauncher(TaskCompletionSource<bool> tcs)
    {
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
                new LC.StackPanel(horizontal: false,
                    simpleMode,
                    fullMode)),
            new LC.StackPanel(horizontal: false,
                launch,
                launchedLabel))
        .StyledDump("Launcher");
    }

    private static void ShowLicenses()
    {
        var myLicense = CreateLicensePanel(
            summary: "This program is licensed under 0BSD.",
            workName: "TreeTraversalAnimations",
            licenseName: "0BSD",
            licenseText: @"
Copyright (C) 2020, 2021 Eliah Kagan <degeneracypressure@gmail.com>

Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted.

THE SOFTWARE IS PROVIDED ""AS IS"" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM
LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
PERFORMANCE OF THIS SOFTWARE.");

        var msaglLicense = CreateLicensePanel(
            summary: @"
This program uses the Microsoft Automatic Graph Layout
(MSAGL) library, which is licensed under the MIT license.",
            workName: "MSAGL",
            licenseName: "MIT license",
            licenseText: @"
Microsoft Automatic Graph Layout,MSAGL

Copyright (c) Microsoft Corporation

All rights reserved.

MIT License

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
""""Software""""), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.");

        new LC.StackPanel(horizontal: false, myLicense, msaglLicense)
            .StyledDump("Licenses");
    }

    private static LC.StackPanel CreateLicensePanel(string summary,
                                                    string workName,
                                                    string licenseName,
                                                    string licenseText)
    {
        var toggle = new LC.Button();

        var license = new LC.FieldSet($"{workName} - {licenseName}",
            new LC.Label(licenseText.Trim())) { Visible = false };

        UpdateToggleText();

        toggle.Click += delegate {
            license.Visible = !license.Visible;
            UpdateToggleText();
        };

        void UpdateToggleText()
        {
            var verb = (license.Visible ? "Hide" : "Show");
            toggle.Text = $"{verb} {licenseName}";
        }

        return new(horizontal: false,
            new LC.Label(summary.Trim()),
            toggle,
            license);
    }
}

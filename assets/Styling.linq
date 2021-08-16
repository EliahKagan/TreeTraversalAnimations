<Query Kind="Statements" />

// Styling.linq - Custom dump styling for TreeTraversalAnimations.
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

Util.RawHtml(@"<style>
    h1.headingpresenter {
        font-size: 1.35rem;
    }

    fieldset {
        display: inline-block;
    }
</style>").Dump();

internal static class Styling {
    internal static T StyledDump<T>(this T self, string? label = null)
    {
        Util.WithStyle(self, "font-size: 1.2em").Dump(label);
        return self;
    }
}

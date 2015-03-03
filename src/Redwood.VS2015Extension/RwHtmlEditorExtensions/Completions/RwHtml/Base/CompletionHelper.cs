﻿using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Parser.RwHtml.Tokenizer;

namespace Redwood.VS2015Extension.RwHtmlEditorExtensions.Completions.RwHtml.Base
{
    public static class CompletionHelper
    {
        public static bool IsWhiteSpaceTextToken(RwHtmlToken token)
        {
            return token.Type == RwHtmlTokenType.Text && string.IsNullOrWhiteSpace(token.Text);
        }
    }
}
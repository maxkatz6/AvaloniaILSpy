// Copyright (c) 2021 Tom Englert
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

namespace ICSharpCode.ILSpy.Themes
{
	public static class ResourceKeys
	{
		public static string BorderBrush { get; } = $"{nameof(ResourceKeys)}-{nameof(BorderBrush)}";
		public static string DisabledBrush { get; } = $"{nameof(ResourceKeys)}-{nameof(DisabledBrush)}";
		public static string TextBackgroundBrush { get; } = $"{nameof(ResourceKeys)}-{nameof(TextBackgroundBrush)}";
		public static string TextForegroundBrush { get; } = $"{nameof(ResourceKeys)}-{nameof(TextForegroundBrush)}";
		public static string TextMarkerBackgroundColor { get; } = $"{nameof(ResourceKeys)}-{nameof(TextMarkerBackgroundColor)}";
		public static string TextMarkerDefinitionBackgroundColor { get; } = $"{nameof(ResourceKeys)}-{nameof(TextMarkerDefinitionBackgroundColor)}";
		public static string SearchResultBackgroundBrush { get; } = $"{nameof(ResourceKeys)}-{nameof(SearchResultBackgroundBrush)}";
		public static string LinkTextForegroundBrush { get; } = $"{nameof(ResourceKeys)}-{nameof(LinkTextForegroundBrush)}";
		public static string BracketHighlightBackgroundBrush { get; } = $"{nameof(ResourceKeys)}-{nameof(BracketHighlightBackgroundBrush)}";
		public static string BracketHighlightBorderPen { get; } = $"{nameof(ResourceKeys)}-{nameof(BracketHighlightBorderPen)}";
		public static string LineNumbersForegroundBrush { get; } = $"{nameof(ResourceKeys)}-{nameof(LineNumbersForegroundBrush)}";
		public static string CurrentLineBackgroundBrush { get; } = $"{nameof(ResourceKeys)}-{nameof(CurrentLineBackgroundBrush)}";
		public static string CurrentLineBorderPen { get; } = $"{nameof(ResourceKeys)}-{nameof(CurrentLineBorderPen)}";
		public static string ThemeAwareButtonEffect { get; } = $"{nameof(ResourceKeys)}-{nameof(ThemeAwareButtonEffect)}";
		public static string ControlLightLightColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlLightLightColorKey)}";
		public static string ControlLightColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlLightColorKey)}";
		public static string ControlColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlColorKey)}";
		public static string ControlDarkColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlDarkColorKey)}";
		public static string ControlDarkDarkColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlDarkDarkColorKey)}";
		public static string ControlTextColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlTextColorKey)}";
		public static string GrayTextColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(GrayTextColorKey)}";
		public static string HighlightColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(HighlightColorKey)}";
		public static string HighlightTextColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(HighlightTextColorKey)}";
		public static string InfoTextColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(InfoTextColorKey)}";
		public static string InfoColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(InfoColorKey)}";
		public static string MenuColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(MenuColorKey)}";
		public static string MenuBarColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(MenuBarColorKey)}";
		public static string MenuTextColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(MenuTextColorKey)}";
		public static string WindowColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(WindowColorKey)}";
		public static string WindowTextColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(WindowTextColorKey)}";
		public static string ActiveCaptionColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ActiveCaptionColorKey)}";
		public static string ActiveBorderColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ActiveBorderColorKey)}";
		public static string ActiveCaptionTextColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ActiveCaptionTextColorKey)}";
		public static string InactiveCaptionColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(InactiveCaptionColorKey)}";
		public static string InactiveBorderColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(InactiveBorderColorKey)}";
		public static string InactiveCaptionTextColorKey { get; } = $"{nameof(ResourceKeys)}-{nameof(InactiveCaptionTextColorKey)}";
		public static string ControlLightLightBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlLightLightBrushKey)}";
		public static string ControlLightBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlLightBrushKey)}";
		public static string ControlBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlBrushKey)}";
		public static string ControlDarkBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlDarkBrushKey)}";
		public static string ControlDarkDarkBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlDarkDarkBrushKey)}";
		public static string ControlTextBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ControlTextBrushKey)}";
		public static string GrayTextBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(GrayTextBrushKey)}";
		public static string HighlightBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(HighlightBrushKey)}";
		public static string HighlightTextBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(HighlightTextBrushKey)}";
		public static string InfoTextBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(InfoTextBrushKey)}";
		public static string InfoBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(InfoBrushKey)}";
		public static string MenuBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(MenuBrushKey)}";
		public static string MenuBarBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(MenuBarBrushKey)}";
		public static string MenuTextBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(MenuTextBrushKey)}";
		public static string WindowBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(WindowBrushKey)}";
		public static string WindowTextBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(WindowTextBrushKey)}";
		public static string ActiveCaptionBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ActiveCaptionBrushKey)}";
		public static string ActiveBorderBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ActiveBorderBrushKey)}";
		public static string ActiveCaptionTextBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(ActiveCaptionTextBrushKey)}";
		public static string InactiveCaptionBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(InactiveCaptionBrushKey)}";
		public static string InactiveBorderBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(InactiveBorderBrushKey)}";
		public static string InactiveCaptionTextBrushKey { get; } = $"{nameof(ResourceKeys)}-{nameof(InactiveCaptionTextBrushKey)}";
	}
}

#nullable enable

using Avalonia.Media;

using AvaloniaEdit.Highlighting;

namespace ICSharpCode.ILSpy.Themes;

public class SyntaxColor
{
	public Color? Foreground { get; set; }
	public Color? Background { get; set; }
	public FontWeight? FontWeight { get; set; }
	public FontStyle? FontStyle { get; set; }

	public void ApplyTo(HighlightingColor color)
	{
		color.Foreground = Foreground is { } foreground ? new SimpleHighlightingBrush(foreground) : null;
		color.Background = Background is { } background ? new SimpleHighlightingBrush(background) : null;
		color.FontWeight = FontWeight ?? Avalonia.Media.FontWeight.Normal;
		color.FontStyle = FontStyle ?? Avalonia.Media.FontStyle.Normal;
	}

	public static void ResetColor(HighlightingColor color)
	{
		color.Foreground = null;
		color.Background = null;
		color.FontWeight = null;
		color.FontStyle = null;
	}
}

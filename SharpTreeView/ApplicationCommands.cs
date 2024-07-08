using System;

using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Labs.Input;

namespace ICSharpCode;

public static class ApplicationCommands
{
	public static RoutedCommand Cut { get; } = new RoutedCommand(nameof(Cut), new KeyGesture(Key.X, GetFromHotKeys(h => h.CommandModifiers)));
	public static RoutedCommand Paste { get; } = new RoutedCommand(nameof(Paste), new KeyGesture(Key.V, GetFromHotKeys(h => h.CommandModifiers)));
	public static RoutedCommand Delete { get; } = new RoutedCommand(nameof(Delete), new KeyGesture(Key.Delete));
	public static RoutedCommand Copy { get; } = new RoutedCommand(nameof(Copy), new KeyGesture(Key.C, GetFromHotKeys(h => h.CommandModifiers)));
	public static RoutedCommand Open { get; } = new RoutedCommand(nameof(Open), new KeyGesture(Key.O, GetFromHotKeys(h => h.CommandModifiers)));
	public static RoutedCommand Save { get; } = new RoutedCommand(nameof(Save), new KeyGesture(Key.S, GetFromHotKeys(h => h.CommandModifiers)));
	public static RoutedCommand Refresh { get; } = new RoutedCommand(nameof(Refresh), new KeyGesture(Key.R, GetFromHotKeys(h => h.CommandModifiers)));

	private static T GetFromHotKeys<T>(Func<PlatformHotkeyConfiguration, T> filter)
	{
		var hotkeys = Application.Current?.PlatformSettings!.HotkeyConfiguration
		              ?? throw new InvalidOperationException("HotkeyConfiguration was not initialized");
		return filter(hotkeys);
	}
}
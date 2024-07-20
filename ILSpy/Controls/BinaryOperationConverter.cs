using System.Linq;

using Avalonia.Data.Converters;

namespace ICSharpCode.ILSpy.Controls;

public static class BinaryOperationConverter
{
	public static IMultiValueConverter Equality { get; } =
		new FuncMultiValueConverter<object, bool>(items => items.Distinct().Count() == 1);
}
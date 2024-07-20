// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using System;
using System.Collections.Generic;
using System.IO;

using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ICSharpCode.ILSpy
{
	static class Images
	{
		static IImage Load(string icon)
		{
			var image = new DrawingImage(LoadDrawingGroup(null, "Images/" + icon));
			// TODO Avalonia: there should be something like .ToImmutable()
			// if (image.CanFreeze)
			// {
			// 	image.Freeze();
			// }

			return image;
		}

		public static readonly Bitmap ILSpyIcon = new Bitmap(AssetLoader.Open(new Uri("avares://ILSpy/Images/ILSpy.ico")));

		public static readonly IImage ViewCode = Load("ViewCode");
		public static readonly IImage Save = Load("Save");
		public static readonly IImage OK = Load("OK");

		public static readonly IImage Delete = Load("Delete");
		public static readonly IImage Search = Load("Search");

		public static readonly IImage Assembly = Load("Assembly");
		public static readonly IImage AssemblyWarning = Load("AssemblyWarning");
		public static readonly IImage FindAssembly = Load("FindAssembly");

		public static readonly IImage Library = Load("Library");
		public static readonly IImage Namespace = Load("Namespace");

		public static readonly IImage ReferenceFolder = Load("ReferenceFolder");
		public static readonly IImage NuGet = Load(null, "Images/NuGet.png");

		public static readonly IImage SubTypes = Load("SubTypes");
		public static readonly IImage SuperTypes = Load("SuperTypes");

		public static readonly IImage FolderOpen = Load("Folder.Open");
		public static readonly IImage FolderClosed = Load("Folder.Closed");

		public static readonly IImage Resource = Load("Resource");
		public static readonly IImage ResourceImage = Load("ResourceImage");
		public static readonly IImage ResourceResourcesFile = Load("ResourceResourcesFile");
		public static readonly IImage ResourceXml = Load("ResourceXml");
		public static readonly IImage ResourceXsd = Load("ResourceXslt");
		public static readonly IImage ResourceXslt = Load("ResourceXslt");

		public static readonly IImage Class = Load("Class");
		public static readonly IImage Struct = Load("Struct");
		public static readonly IImage Interface = Load("Interface");
		public static readonly IImage Delegate = Load("Delegate");
		public static readonly IImage Enum = Load("Enum");

		public static readonly IImage Field = Load("Field");
		public static readonly IImage FieldReadOnly = Load("FieldReadOnly");
		public static readonly IImage Literal = Load("Literal");
		public static readonly IImage EnumValue = Load("EnumValue");

		public static readonly IImage Method = Load("Method");
		public static readonly IImage Constructor = Load("Constructor");
		public static readonly IImage VirtualMethod = Load("VirtualMethod");
		public static readonly IImage Operator = Load("Operator");
		public static readonly IImage ExtensionMethod = Load("ExtensionMethod");
		public static readonly IImage PInvokeMethod = Load("PInvokeMethod");

		public static readonly IImage Property = Load("Property");
		public static readonly IImage Indexer = Load("Indexer");

		public static readonly IImage Event = Load("Event");

		private static readonly IImage OverlayProtected = Load("OverlayProtected");
		private static readonly IImage OverlayInternal = Load("OverlayInternal");
		private static readonly IImage OverlayProtectedInternal = Load("OverlayProtectedInternal");
		private static readonly IImage OverlayPrivate = Load("OverlayPrivate");
		private static readonly IImage OverlayPrivateProtected = Load("OverlayPrivateProtected");
		private static readonly IImage OverlayCompilerControlled = Load("OverlayCompilerControlled");

		private static readonly IImage OverlayStatic = Load("OverlayStatic");

		public static IImage Load(object part, string icon)
		{
			if (icon.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
				return LoadImage(part, icon);
			Uri uri = GetUri(part, icon + ".xaml");
			if (ResourceExists(uri))
			{
				var image = new DrawingImage(LoadDrawingGroup(part, icon));
				// if (image.CanFreeze)
				// {
				// 	image.Freeze();
				// }
				return image;
			}
			return LoadImage(part, icon + ".png");
		}

		static Bitmap LoadImage(object part, string icon)
		{
			Uri uri = GetUri(part, icon);
			Bitmap image = new Bitmap(AssetLoader.Open(uri));
			// if (image.CanFreeze)
			// {
			// 	image.Freeze();
			// }
			return image;
		}

		public static Drawing LoadDrawingGroup(object part, string icon)
		{
			// TODO IT'S NOT AOT FRIENDLY AT ALL, revisit.
			return (Drawing)AvaloniaXamlLoader.Load(GetUri(part, icon + ".xaml", absolute: false));
		}

		private static Uri GetUri(object part, string icon, bool absolute = true)
		{
			Uri uri;
			var assembly = part?.GetType().Assembly;
			string prefix;
			UriKind kind;
			if (absolute)
			{
				prefix = "pack://application:,,,/";
				kind = UriKind.Absolute;
			}
			else
			{
				prefix = "/";
				kind = UriKind.Relative;
			}
			if (part == null || assembly == typeof(Images).Assembly)
			{
				uri = new Uri(prefix + icon, kind);
			}
			else
			{
				var name = assembly.GetName();
				uri = new Uri(prefix + name.Name + ";v" + name.Version + ";component/" + icon, kind);
			}

			return uri;
		}

		private static bool ResourceExists(Uri uri)
		{
			try
			{
				return AssetLoader.Exists(uri);
			}
			catch (IOException)
			{
				return false;
			}
		}

		private static readonly TypeIconCache typeIconCache = new TypeIconCache();
		private static readonly MemberIconCache memberIconCache = new MemberIconCache();

		public static IImage GetIcon(TypeIcon icon, AccessOverlayIcon overlay, bool isStatic = false)
		{
			lock (typeIconCache)
				return typeIconCache.GetIcon(icon, overlay, isStatic);
		}

		public static IImage GetIcon(MemberIcon icon, AccessOverlayIcon overlay, bool isStatic)
		{
			lock (memberIconCache)
				return memberIconCache.GetIcon(icon, overlay, isStatic);
		}

		#region icon caches & overlay management

		private class TypeIconCache : IconCache<TypeIcon>
		{
			public TypeIconCache()
			{
				PreloadPublicIconToCache(TypeIcon.Class, Images.Class);
				PreloadPublicIconToCache(TypeIcon.Enum, Images.Enum);
				PreloadPublicIconToCache(TypeIcon.Struct, Images.Struct);
				PreloadPublicIconToCache(TypeIcon.Interface, Images.Interface);
				PreloadPublicIconToCache(TypeIcon.Delegate, Images.Delegate);
			}

			protected override IImage GetBaseImage(TypeIcon icon)
			{
				IImage baseImage;
				switch (icon)
				{
					case TypeIcon.Class:
						baseImage = Images.Class;
						break;
					case TypeIcon.Enum:
						baseImage = Images.Enum;
						break;
					case TypeIcon.Struct:
						baseImage = Images.Struct;
						break;
					case TypeIcon.Interface:
						baseImage = Images.Interface;
						break;
					case TypeIcon.Delegate:
						baseImage = Images.Delegate;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(icon), $"TypeIcon.{icon} is not supported!");
				}

				return baseImage;
			}
		}

		private class MemberIconCache : IconCache<MemberIcon>
		{
			public MemberIconCache()
			{
				PreloadPublicIconToCache(MemberIcon.Field, Images.Field);
				PreloadPublicIconToCache(MemberIcon.FieldReadOnly, Images.FieldReadOnly);
				PreloadPublicIconToCache(MemberIcon.Literal, Images.Literal);
				PreloadPublicIconToCache(MemberIcon.EnumValue, Images.EnumValue);
				PreloadPublicIconToCache(MemberIcon.Property, Images.Property);
				PreloadPublicIconToCache(MemberIcon.Indexer, Images.Indexer);
				PreloadPublicIconToCache(MemberIcon.Method, Images.Method);
				PreloadPublicIconToCache(MemberIcon.Constructor, Images.Constructor);
				PreloadPublicIconToCache(MemberIcon.VirtualMethod, Images.VirtualMethod);
				PreloadPublicIconToCache(MemberIcon.Operator, Images.Operator);
				PreloadPublicIconToCache(MemberIcon.ExtensionMethod, Images.ExtensionMethod);
				PreloadPublicIconToCache(MemberIcon.PInvokeMethod, Images.PInvokeMethod);
				PreloadPublicIconToCache(MemberIcon.Event, Images.Event);
			}

			protected override IImage GetBaseImage(MemberIcon icon)
			{
				IImage baseImage;
				switch (icon)
				{
					case MemberIcon.Field:
						baseImage = Images.Field;
						break;
					case MemberIcon.FieldReadOnly:
						baseImage = Images.FieldReadOnly;
						break;
					case MemberIcon.Literal:
						baseImage = Images.Literal;
						break;
					case MemberIcon.EnumValue:
						baseImage = Images.EnumValue;
						break;
					case MemberIcon.Property:
						baseImage = Images.Property;
						break;
					case MemberIcon.Indexer:
						baseImage = Images.Indexer;
						break;
					case MemberIcon.Method:
						baseImage = Images.Method;
						break;
					case MemberIcon.Constructor:
						baseImage = Images.Constructor;
						break;
					case MemberIcon.VirtualMethod:
						baseImage = Images.VirtualMethod;
						break;
					case MemberIcon.Operator:
						baseImage = Images.Operator;
						break;
					case MemberIcon.ExtensionMethod:
						baseImage = Images.ExtensionMethod;
						break;
					case MemberIcon.PInvokeMethod:
						baseImage = Images.PInvokeMethod;
						break;
					case MemberIcon.Event:
						baseImage = Images.Event;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(icon), $"MemberIcon.{icon} is not supported!");
				}

				return baseImage;
			}
		}

		private abstract class IconCache<T>
		{
			private readonly Dictionary<(T, AccessOverlayIcon, bool), IImage> cache = new Dictionary<(T, AccessOverlayIcon, bool), IImage>();

			protected void PreloadPublicIconToCache(T icon, IImage image)
			{
				var iconKey = (icon, AccessOverlayIcon.Public, false);
				cache.Add(iconKey, image);
			}

			public IImage GetIcon(T icon, AccessOverlayIcon overlay, bool isStatic)
			{
				var iconKey = (icon, overlay, isStatic);
				if (cache.ContainsKey(iconKey))
				{
					return cache[iconKey];
				}
				else
				{
					IImage result = BuildMemberIcon(icon, overlay, isStatic);
					cache.Add(iconKey, result);
					return result;
				}
			}

			private IImage BuildMemberIcon(T icon, AccessOverlayIcon overlay, bool isStatic)
			{
				IImage baseImage = GetBaseImage(icon);
				IImage overlayImage = GetOverlayImage(overlay);

				return CreateOverlayImage(baseImage, overlayImage, isStatic);
			}

			protected abstract IImage GetBaseImage(T icon);

			private static IImage GetOverlayImage(AccessOverlayIcon overlay)
			{
				IImage overlayImage;
				switch (overlay)
				{
					case AccessOverlayIcon.Public:
						overlayImage = null;
						break;
					case AccessOverlayIcon.Protected:
						overlayImage = Images.OverlayProtected;
						break;
					case AccessOverlayIcon.Internal:
						overlayImage = Images.OverlayInternal;
						break;
					case AccessOverlayIcon.ProtectedInternal:
						overlayImage = Images.OverlayProtectedInternal;
						break;
					case AccessOverlayIcon.Private:
						overlayImage = Images.OverlayPrivate;
						break;
					case AccessOverlayIcon.PrivateProtected:
						overlayImage = Images.OverlayPrivateProtected;
						break;
					case AccessOverlayIcon.CompilerControlled:
						overlayImage = Images.OverlayCompilerControlled;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(overlay), $"AccessOverlayIcon.{overlay} is not supported!");
				}
				return overlayImage;
			}

			private static readonly Rect iconRect = new Rect(0, 0, 16, 16);

			private static IImage CreateOverlayImage(IImage baseImage, IImage overlay, bool isStatic)
			{
				var group = new DrawingGroup();

				Drawing baseDrawing = new ImageDrawing {
					ImageSource = baseImage,
					Rect = iconRect
				};

				if (overlay != null)
				{
					var nestedGroup = new DrawingGroup { Transform = new ScaleTransform(0.8, 0.8) };
					nestedGroup.Children.Add(baseDrawing);
					group.Children.Add(nestedGroup);
					group.Children.Add(new ImageDrawing {
						ImageSource = overlay,
						Rect = iconRect
					});
				}
				else
				{
					group.Children.Add(baseDrawing);
				}

				if (isStatic)
				{
					group.Children.Add(new ImageDrawing {
						ImageSource = Images.OverlayStatic,
						Rect = iconRect
					});
				}

				var image = new DrawingImage(group);
				// TODO Avalonia, there should be something like .ToImmutable()
				// if (image.CanFreeze)
				// {
				// 	image.Freeze();
				// }
				return image;
			}
		}

		#endregion
	}
}

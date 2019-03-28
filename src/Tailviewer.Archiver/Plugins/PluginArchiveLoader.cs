﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using log4net;
using Tailviewer.BusinessLogic.Plugins;

namespace Tailviewer.Archiver.Plugins
{
	/// <summary>
	///     This class is responsible for loading plugin archives which have been created with <see cref="PluginPacker" />
	///     (or the cli equivalent, packer.exe).
	/// </summary>
	public sealed class PluginArchiveLoader
		: IPluginLoader
		, IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly Dictionary<IPluginDescription, IPluginArchive> _archivesByPlugin;
		private readonly Dictionary<PluginId, IPluginStatus> _pluginStati;

		/// <summary>
		/// </summary>
		/// <param name="filesystem"></param>
		/// <param name="path"></param>
		public PluginArchiveLoader(IFilesystem filesystem, string path)
		{
			_archivesByPlugin = new Dictionary<IPluginDescription, IPluginArchive>();
			_pluginStati = new Dictionary<PluginId, IPluginStatus>();

			try
			{
				// TODO: How would we make this truly async? Currently the app has to block until all plugins are loaded wich is sad
				var files = filesystem.EnumerateFiles(path, string.Format("*.{0}", PluginArchive.PluginExtension)).Result;
				foreach (var pluginPath in files)
					ReflectPlugin(pluginPath);
			}
			catch (DirectoryNotFoundException e)
			{
				Log.WarnFormat("Unable to find plugins in '{0}': {1}", path, e);
			}
			catch (Exception e)
			{
				Log.ErrorFormat("Unable to find plugins in '{0}': {1}", path, e);
			}
		}

		/// <inheritdoc />
		public IEnumerable<IPluginDescription> Plugins
		{
			get
			{
				var plugins = _archivesByPlugin.GroupBy(x => x.Key.Id).Select(FindUsablePlugin).Where(x => x != null).ToList();
				return plugins;
			}
		}

		[Pure]
		private static IPluginDescription FindUsablePlugin(IGrouping<PluginId, KeyValuePair<IPluginDescription, IPluginArchive>> grouping)
		{
			var highestUsable = grouping.Where(x => IsUsable(x.Value.Index)).MaxBy(x => x.Key.Version);
			return highestUsable.Key;
		}

		/// <summary>
		/// Tests if a plugin with the given index is actually usable (or if the plugin is too old, or too new).
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		[Pure]
		private static bool IsUsable(IPluginPackageIndex index)
		{
			if (index.PluginArchiveVersion < PluginArchive.MinimumSupportedPluginArchiveVersion)
				return false;

			if (index.PluginArchiveVersion > PluginArchive.CurrentPluginArchiveVersion)
				return false;

			// We might also want to discard plugins which are built against outdated APIs, but this
			// will probably have to be hard-coded, no?
			return true;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			foreach (var archive in _archivesByPlugin.Values)
				archive.Dispose();
			_archivesByPlugin.Clear();
		}

		public IPluginStatus GetStatus(IPluginDescription description)
		{
			var id = description?.Id;
			if (id != null && _pluginStati.TryGetValue(id, out var status))
				return status;

			status = new PluginStatus
			{
				IsInstalled = false
			};
			return status;
		}

		public IReadOnlyDictionary<string, Type> ResolveSerializableTypes()
		{
			var serializableTypes = new Dictionary<string, Type>();

			foreach (var pair in _archivesByPlugin)
			{
				var assembly = pair.Value.LoadPlugin();
				var types = pair.Key.SerializableTypes;
				foreach (var tmp in types)
				{
					if (TryResolveType(assembly, tmp.Value, out var type))
					{
						serializableTypes.Add(tmp.Key, type);
					}
				}
			}

			return serializableTypes;
		}

		public T Load<T>(IPluginDescription description) where T : class, IPlugin
		{
			if (!_archivesByPlugin.TryGetValue(description, out var archive))
				throw new ArgumentException();

			if (!description.Plugins.TryGetValue(typeof(T), out var interfaceImplementation))
				throw new NotImplementedException();

			var plugin = archive.LoadPlugin();
			var type = plugin.GetType(interfaceImplementation);
			var pluginObject = Activator.CreateInstance(type);
			return (T) pluginObject;
		}

		/// <inheritdoc />
		public IReadOnlyList<T> LoadAllOfType<T>() where T : class, IPlugin
		{
			var interfaceType = typeof(T);
			Log.InfoFormat("Loading plugins implementing '{0}'...", interfaceType.Name);

			var ret = new List<T>();
			foreach (var pluginDescription in Plugins)
			{
				if (pluginDescription.Plugins.ContainsKey(interfaceType))
				{
					try
					{
						var plugin = Load<T>(pluginDescription);
						ret.Add(plugin);
					}
					catch (Exception e)
					{
						Log.ErrorFormat("Unable to load plugin of interface '{0}' from '{1}': {2}",
							interfaceType,
							pluginDescription,
							e);
					}
				}
			}

			Log.InfoFormat("Loaded #{0} plugins", ret.Count);

			return ret;
		}

		public IPluginDescription ReflectPlugin(string pluginPath)
		{
			try
			{
				var archive = PluginArchive.OpenRead(pluginPath);
				return ReflectPlugin(archive);
			}
			catch (Exception e)
			{
				Log.ErrorFormat("Unable to load '{0}': {1}", pluginPath, e);

				ExtractIdAndVersion(pluginPath, out var id, out var version);
				var description = new PluginDescription
				{
					Id = id,
					Version = version,
					Error = string.Format("The plugin couldn't be loaded: {0}", e.Message),
					Plugins = new Dictionary<Type, string>(),
					FilePath = pluginPath
				};
				_archivesByPlugin.Add(description, new EmptyPluginArchive());
				return description;
			}
		}

		private bool TryResolveType(Assembly assembly, string typeName, out Type type)
		{
			try
			{
				type = assembly.GetType(typeName, throwOnError: true);
				if (type == null)
				{
					Log.ErrorFormat("Unable to resolve type '{0}'", typeName);
					return false;
				}

				return true;
			}
			catch (Exception e)
			{
				Log.ErrorFormat("Unable to resolve type '{0}': {1}", typeName, e);
				type = null;
				return false;
			}
		}

		private static void ExtractIdAndVersion(string pluginPath, out PluginId id, out Version version)
		{
			var fileName = Path.GetFileNameWithoutExtension(pluginPath);
			int idx = fileName.IndexOf(".");
			if (idx != -1)
			{
				id = new PluginId(fileName.Substring(0, idx));
				var tmp = fileName.Substring(idx + 1);
				if (!Version.TryParse(tmp, out version))
					version = new Version(0, 0, 0, 0);
			}
			else
			{
				id = new PluginId("Unknown");
				version = new Version(0, 0, 0, 0);
			}
		}

		public IPluginDescription ReflectPlugin(Stream stream, bool leaveOpen = false)
		{
			var archive = PluginArchive.OpenRead(stream, leaveOpen);
			return ReflectPlugin(archive);
		}

		private IPluginDescription ReflectPlugin(PluginArchive archive)
		{
			var description = CreateDescription(archive);
			_archivesByPlugin.Add(description, archive);

			if (!_pluginStati.ContainsKey(description.Id))
			{
				_pluginStati.Add(description.Id, new PluginStatus
				{
					IsInstalled = true
				});
			}
			return description;
		}

		[Pure]
		private static IPluginDescription CreateDescription(PluginArchive archive)
		{
			var archiveIndex = archive.Index;

			Uri.TryCreate(archiveIndex.Website, UriKind.Absolute, out var website);

			var plugins = new Dictionary<Type, string>();
			foreach (var pair in archiveIndex.ImplementedPluginInterfaces)
			{
				var pluginInterfaceType = typeof(IPlugin).Assembly.GetType(pair.InterfaceTypename);
				if (pluginInterfaceType != null)
					plugins.Add(pluginInterfaceType, pair.ImplementationTypename);
				else
					Log.WarnFormat("Plugin implements unknown interface '{0}', skipping it...", pair.InterfaceTypename);
			}
			var serializableTypes = new Dictionary<string, string>();
			foreach (var pair in archiveIndex.SerializableTypes)
			{
				serializableTypes.Add(pair.Name, pair.FullName);
			}

			var desc = new PluginDescription
			{
				Id = new PluginId(archiveIndex.Id),
				Name = archiveIndex.Name,
				Version = archiveIndex.Version,
				Icon = LoadIcon(archive.ReadIcon()),
				Author = archiveIndex.Author,
				Description = archiveIndex.Description,
				Website = website,
				Plugins = plugins,
				SerializableTypes = serializableTypes
			};

			return desc;
		}

		private static ImageSource LoadIcon(Stream icon)
		{
			if (icon == null)
				return null;

			var image = new BitmapImage();
			image.BeginInit();
			image.StreamSource = icon;
			image.EndInit();
			return image;
		}
	}
}
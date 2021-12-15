using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using SomethingNeedDoing.Managers;

namespace SomethingNeedDoing
{
    /// <summary>
    /// Dalamud and plugin services.
    /// </summary>
    internal class Service
    {
        /// <summary>
        /// Gets or sets the plugin configuration.
        /// </summary>
        internal static SomethingNeedDoingConfiguration Configuration { get; set; } = null!;

        /// <summary>
        /// Gets or sets the plugin address resolver.
        /// </summary>
        internal static PluginAddressResolver Address { get; set; } = null!;

        /// <summary>
        /// Gets or sets the plugin action manager.
        /// </summary>
        internal static ActionManager ActionManager { get; set; } = null!;

        /// <summary>
        /// Gets or sets the plugin chat manager.
        /// </summary>
        internal static ChatManager ChatManager { get; set; } = null!;

        /// <summary>
        /// Gets or sets the plugin event framework manager.
        /// </summary>
        internal static EventFrameworkManager EventFrameworkManager { get; set; } = null!;

        /// <summary>
        /// Gets or sets the plugin macro manager.
        /// </summary>
        internal static MacroManager MacroManager { get; set; } = null!;

        /// <summary>
        /// Gets the Dalamud plugin interface.
        /// </summary>
        [PluginService]
        internal static DalamudPluginInterface Interface { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud chat gui.
        /// </summary>
        [PluginService]
        internal static ChatGui ChatGui { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud client state.
        /// </summary>
        [PluginService]
        internal static ClientState ClientState { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud command manager.
        /// </summary>
        [PluginService]
        internal static CommandManager CommandManager { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud data manager.
        /// </summary>
        [PluginService]
        internal static DataManager DataManager { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud framework.
        /// </summary>
        [PluginService]
        internal static Framework Framework { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud game gui.
        /// </summary>
        [PluginService]
        internal static GameGui GameGui { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud object table.
        /// </summary>
        [PluginService]
        internal static ObjectTable ObjectTable { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud target manager.
        /// </summary>
        [PluginService]
        internal static TargetManager TargetManager { get; private set; } = null!;
    }
}

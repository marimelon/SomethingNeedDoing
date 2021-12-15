using System;
using System.Collections.Generic;
using System.Threading;

using Dalamud.Game;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;

namespace SomethingNeedDoing.Managers
{
    /// <summary>
    /// Manager that actions status.
    /// </summary>
    internal class ActionManager : IDisposable
    {
        private readonly ManualResetEvent canUseActionWaiter = new(true);
        private readonly Dictionary<(ClassJob, string), (uint, ActionType)> craftActions = new();
        private CraftAction? waitingAction = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionManager"/> class.
        /// </summary>
        public ActionManager()
        {
            var actions = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>();
            if (actions != null)
            {
                foreach (var row in actions)
                {
                    var job = row?.ClassJob?.Value?.ClassJobCategory?.Value;
                    if (row != null && job != null && IsCrafter(job))
                    {
                        var name = row.Name.RawString.Trim(new char[] { ' ', '"', '\'' }).ToLower();
                        if (name != null && row.ClassJob.Value != null)
                            this.craftActions[(row.ClassJob.Value, name)] = (row.RowId, ActionType.Spell);
                    }
                }
            }

            var craftActions = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.CraftAction>();
            if (craftActions != null)
            {
                foreach (var row in craftActions)
                {
                    var job = row?.ClassJob?.Value?.ClassJobCategory?.Value;
                    if (row != null && job != null && IsCrafter(job))
                    {
                        var name = row.Name.RawString.Trim(new char[] { ' ', '"', '\'' }).ToLower();
                        if (name != null && row.ClassJob.Value != null)
                            this.craftActions[(row.ClassJob.Value, name)] = (row.RowId, ActionType.CraftAction);
                    }
                }
            }

            Service.Framework.Update += this.Framework_OnUpdateEvent;
        }

        /// <summary>
        /// ActionType.
        /// </summary>
        /// <param name="job">ClassJobCategory.</param>
        /// <returns> Is Crafter Action. </returns>
        public static bool IsCrafter(ClassJobCategory job)
        {
            return job.CRP || job.BSM || job.ARM || job.GSM || job.LTW || job.WVR || job.ALC || job.CUL;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Service.Framework.Update -= this.Framework_OnUpdateEvent;

            this.canUseActionWaiter.Dispose();
        }

        /// <summary>
        /// Wait for the action to become available.
        /// </summary>
        /// <param name="action">CraftAction.</param>
        /// <param name="millisecondsTimeout">millisecondsTimeout.</param>
        public void WaitCanUseAction(CraftAction action, int millisecondsTimeout = 5000)
        {
            this.waitingAction = action;
            this.canUseActionWaiter.Reset();
            this.canUseActionWaiter.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Gets the CraftAction from action name and classJob.
        /// </summary>
        /// <param name="name">ActuibName.</param>
        /// <param name="classjob">ClassJob.</param>
        /// <returns>CraftAction.</returns>
        public CraftAction? GetCraftAction(string name, ClassJob? classjob = null)
        {
            var classjob1 = classjob ?? Service.ClientState.LocalPlayer?.ClassJob?.GameData;
            if (classjob1 == null)
                return null;

            (uint, ActionType) ret;
            if (this.craftActions.TryGetValue((classjob1, name), out ret))
                return new CraftAction { Id = ret.Item1, Name = name, Job = classjob1, Type = ret.Item2 };

            PluginLog.Debug($"Not found craft action. {name} {classjob1}");
            return null;
        }

        private void Framework_OnUpdateEvent(Framework framework)
        {
            try
            {
                if (this.waitingAction == null)
                    return;
                if (this.canUseActionWaiter.WaitOne(0))
                    return;

                if (this.CanUseAction(this.waitingAction.Value))
                    this.canUseActionWaiter.Set();
            }
            catch
            {
                PluginLog.Debug("ActionManager Error");
            }
        }

        private unsafe bool CanUseAction(CraftAction action)
        {
            var actionManager = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance();
            return actionManager->GetActionStatus(action.Type, action.Id, 0xE000_0000, 0, 1) == 0;
        }

        /// <summary>
        /// CraftAction.
        /// </summary>
        protected internal struct CraftAction
        {
            /// <summary>
            /// Action Id.
            /// </summary>
            public uint Id;

            /// <summary>
            /// Action Name.
            /// </summary>
            public string Name;

            /// <summary>
            /// Job.
            /// </summary>
            public ClassJob Job;

            /// <summary>
            /// ActionType.
            /// </summary>
            public ActionType Type;
        }
    }
}

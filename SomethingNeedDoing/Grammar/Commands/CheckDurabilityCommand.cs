using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Text;
using SomethingNeedDoing.Exceptions;
using SomethingNeedDoing.Grammar.Modifiers;

namespace SomethingNeedDoing.Grammar.Commands
{
    internal class CheckDurabilityCommand : MacroCommand
    {
        private static readonly Regex Regex = new(@"^/checkdurability(?: (?<minimum>\d{1,3}(\.\d{1,2})?))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private float minimum;

        private CheckDurabilityCommand(string text, float minimum, WaitModifier wait)
            : base(text, wait)
        {
            this.minimum = minimum;
        }

        public static CheckDurabilityCommand Parse(string text)
        {
            _ = WaitModifier.TryParse(ref text, out var waitModifier);

            var match = Regex.Match(text);
            if (!match.Success)
                throw new MacroSyntaxError(text);

            var minimum = 0f;
            var minimumMatch = match.Groups["minimum"];
            if (minimumMatch.Success && !float.TryParse(minimumMatch.Value, out minimum))
                throw new MacroSyntaxError(text);

            return new CheckDurabilityCommand(text, minimum, waitModifier);
        }

        public async override Task Execute(CancellationToken token)
        {
            var internalMinimumValue = this.minimum * 300;

            Func<uint, SeString?> getName = (uint itemid) => Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>()?.GetRow(itemid)?.Name ?? new SeString("Unkown");

            var dataWaiter = Service.EventFrameworkManager.DataAvailableWaiter;
            dataWaiter.Reset();

            unsafe
            {
                var inventoryMgr = InventoryManager.Instance();
                var container = inventoryMgr->GetInventoryContainer(InventoryType.EquippedItems);
                for (int i = 0; i < container->Size; i++)
                {
                    var item = container->Items[i];

#if DEBUG
                    PluginLog.Debug($"{i} : {getName(item.ItemID)}({item.ItemID}) {item.Condition}");
#endif

                    if (item.Condition == 0)
                    {
                        throw new MacroCommandError($"{getName(item.ItemID)} is broken");
                    }

                    if (item.Condition < internalMinimumValue)
                    {
                        throw new MacroCommandError($"{getName(item.ItemID)}'s durability is below {this.minimum}.");
                    }
                }
            }

            await this.PerformWait(token);
        }
    }
}

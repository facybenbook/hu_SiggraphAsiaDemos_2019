using System;
using System.Collections.Generic;

namespace Devdog.General
{
    public interface IPlayerTriggerHandler
    {
        BestTriggerSelectorBase selector { get; set; }

        TriggerBase selectedTrigger { get; }
        event Action<TriggerBase, TriggerBase> OnSelectedTriggerChanged;

        bool IsInRangeOfTrigger(TriggerBase trigger);
    }
}

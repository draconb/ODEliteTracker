using EliteJournalReader.Events;
using EliteJournalReader;
using ODJournalDatabase.JournalManagement;
using ODEliteTracker.Models.PowerPlay;
using ODMVVM.Helpers;

namespace ODEliteTracker.Stores
{
    public sealed class PowerPlayDataStore : LogProcessorBase
    {
        public PowerPlayDataStore(IManageJournalEvents journalManager)
        {
            this.journalManager = journalManager;
            this.journalManager.RegisterLogProcessor(this);

            previousCycle = EliteHelpers.PreviousThursday(1);
            currentCycle = EliteHelpers.PreviousThursday();
            nextCycle = EliteHelpers.NextThursday();
        }


        private readonly IManageJournalEvents journalManager;
        private DateTime previousCycle;
        private DateTime currentCycle;
        private DateTime nextCycle;

        private long CurrentSystemAddress;
        private bool odyssey;

        private Dictionary<long, PowerPlaySystem> systems = [];
        public override string StoreName => "PowerPlay";

        public override Dictionary<JournalTypeEnum, bool> EventsToParse
        {
            get => new()
            {
                { JournalTypeEnum.LoadGame,true},
                { JournalTypeEnum.Location,true},
                { JournalTypeEnum.FSDJump, true},
                { JournalTypeEnum.CarrierJump, true},
                { JournalTypeEnum.Powerplay,true},
                { JournalTypeEnum.PowerplayMerits,true },
                { JournalTypeEnum.PowerplayCollect,true },
                { JournalTypeEnum.PowerplayDeliver,true },
                { JournalTypeEnum.PowerplayRank,true },
            };
        }
        public DateTime PreviousCycle => previousCycle;
        public DateTime CurrentCycle => currentCycle;

        public TimeSpan CycleRemaining => nextCycle - DateTime.UtcNow;

        public IEnumerable<PowerPlaySystem> Systems => systems.Values;

        private PledgeData? pledgeData;
        public PledgeData? PledgeData
        {
            get { return pledgeData; }
            set 
            { 
                pledgeData = value; 
                if(IsLive)
                    PledgeDataUpdated?.Invoke(this, pledgeData);
            }
        }

        public EventHandler<PledgeData?>? PledgeDataUpdated;
        public EventHandler<PowerPlaySystem>? SystemUpdated;
        public EventHandler<PowerPlaySystem>? SystemAdded;
        public EventHandler<PowerPlaySystem>? SystemCycleUpdated;
        public EventHandler? CyclesUpdated;
        public override DateTime GetJournalAge(DateTime defaultAge)
        {
            return previousCycle;
        }

        public override void ParseJournalEvent(JournalEntry evt)
        {
            if (EventsToParse.ContainsKey(evt.EventType) == false)
                return;

            if (GetVisitedCycle(evt.TimeStamp, out DateTime cycle) == false)
                return;

            switch (evt.EventData)
            {
                case LoadGameEvent.LoadGameEventArgs load:
                    odyssey = load.Odyssey;
                    if (IsLive)
                    {
                        var currentCycle = EliteHelpers.PreviousThursday();
                        if (currentCycle == this.currentCycle)
                            break;

                        var systemsToRemove = new List<long>();
                        foreach (var powerSystem in systems)
                        {
                            if (powerSystem.Value.CycleData.ContainsKey(this.currentCycle) == false)
                            {
                                systemsToRemove.Add(powerSystem.Key);
                            }
                        }

                        foreach (var sys in systemsToRemove)
                        {
                            systems.Remove(sys);
                        }

                        previousCycle = EliteHelpers.PreviousThursday(1);
                        this.currentCycle = currentCycle;
                        nextCycle = EliteHelpers.NextThursday();

                        CyclesUpdated?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case LocationEvent.LocationEventArgs location:
                    if (location.Powers is null || location.Powers.Count == 0)
                    {
                        break;
                    }
                    var system = new PowerPlaySystem(location, cycle);
                    Add_UpdateSystem(system, cycle);
                    break;
                case FSDJumpEvent.FSDJumpEventArgs fsdJump:
                    if (fsdJump.Powers is null || fsdJump.Powers.Count == 0)
                    {
                        break;
                    }
                    var ppSystem = new PowerPlaySystem(fsdJump, cycle);
                    Add_UpdateSystem(ppSystem, cycle);
                    break;
                case CarrierJumpEvent.CarrierJumpEventArgs cJump:
                    if (cJump.Powers is null || cJump.Powers.Count == 0)
                    {
                        break;
                    }
                    var pSystem = new PowerPlaySystem(cJump, cycle);
                    Add_UpdateSystem(pSystem, cycle);
                    break;
                case PowerplayEvent.PowerplayEventArgs powerplay:
                    PledgeData = new(powerplay);
                    break;
                case PowerplayRankEvent.PowerplayRankEventArgs rank:
                    if (PledgeData == null)
                        break;
                    PledgeData.Rank = rank.Rank;
                    if (IsLive)
                        PledgeDataUpdated?.Invoke(this, PledgeData);
                    break;
                case PowerplayMeritsEvent.PowerplayMeritsEventArgs merits:
                    if(systems.TryGetValue(CurrentSystemAddress, out system))
                    {
                        UpdatePledgeData(merits);

                        if (system.CycleData.TryGetValue(cycle, out var ppData))
                        {
                            ppData.MeritsEarned += merits.MeritsGained;                            
                            UpdateSystemIfLive(system);
                            break;
                        }

                        var data = new PowerplayCycleData()
                        {
                            MeritsEarned = merits.MeritsGained
                        };

                        if(system.CycleData.TryAdd(cycle, data))
                        {
                            UpdateSystemIfLive(system);
                        }
                    }
                    break;
                case PowerplayCollectEvent.PowerplayCollectEventArgs powerplayCollect:
                    if (systems.TryGetValue(CurrentSystemAddress, out system))
                    {
                        var itemName = powerplayCollect.GetTypeCollected();

                        if (system.CycleData.TryGetValue(cycle, out var ppData))
                        {
                            if(ppData.GoodsCollected.ContainsKey(itemName))
                            {
                                ppData.GoodsCollected[itemName] += powerplayCollect.Count;
                                UpdateSystemIfLive(system);
                                break;
                            }

                            if (ppData.GoodsCollected.TryAdd(itemName, powerplayCollect.Count))
                            {
                                UpdateSystemIfLive(system);
                            }
                            break;
                        } 
                    }
                    break;
                case PowerplayDeliverEvent.PowerplayDeliverEventArgs powerplayDeliver:
                    if (systems.TryGetValue(CurrentSystemAddress, out system))
                    {
                        var itemName = powerplayDeliver.GetTypeCollected();

                        if (system.CycleData.TryGetValue(cycle, out var ppData))
                        {
                            if (ppData.GoodsDelivered.ContainsKey(itemName))
                            {
                                ppData.GoodsDelivered[itemName] += powerplayDeliver.Count;
                                UpdateSystemIfLive(system);
                                break;
                            }

                            if (ppData.GoodsDelivered.TryAdd(itemName, powerplayDeliver.Count))
                            {
                                UpdateSystemIfLive(system);
                            }
                            break;
                        }
                    }
                    break;

            }
        }

        private void UpdatePledgeData(PowerplayMeritsEvent.PowerplayMeritsEventArgs merits)
        {
            if (PledgeData != null)
            {
                PledgeData.Merits = merits.TotalMerits;
                if (IsLive)
                    PledgeDataUpdated?.Invoke(this, PledgeData);
            }
        }

        private void UpdateSystemIfLive(PowerPlaySystem system)
        {
            if (IsLive)
                SystemUpdated?.Invoke(this, system);
        }

        private bool GetVisitedCycle(DateTime timeStamp, out DateTime cycle)
        {
            if(timeStamp > previousCycle && timeStamp < currentCycle)
            {
                cycle = previousCycle;
                return true;
            }
            if (timeStamp > currentCycle)
            {
                cycle = currentCycle;
                return true;
            }
            cycle = DateTime.MinValue;
            return false;
        }

        private void Add_UpdateSystem(PowerPlaySystem newSystem, DateTime cycle)
        {
            CurrentSystemAddress = newSystem.Address;
            if (systems.TryGetValue(newSystem.Address, out var nSystem))
            {
                nSystem.Add_UpdateCycle(cycle, newSystem);
                if (IsLive)
                    SystemCycleUpdated?.Invoke(this, newSystem);
                return;
            }
            if (systems.TryAdd(newSystem.Address, newSystem))
            {
                if (IsLive)
                    SystemAdded?.Invoke(this, newSystem);
                return;
            }   
        }

        public override void ClearData()
        {
            IsLive = false;
            systems.Clear();
        }

        public override void Dispose()
        {
            this.journalManager.UnregisterLogProcessor(this);
        }
    }
}

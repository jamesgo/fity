using Fity.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Fity.Utils;
using System;
using System.Threading;

namespace Fity.Views
{
    internal class ActivityManager
    {
        private GpsDataManager dataManager;

        public ActivityManager(GpsDataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        private SemaphoreSlim activitiesSemaphore = new SemaphoreSlim(1);
        private IEnumerable<GprxExtended> activities = null;
        public async Task<IEnumerable<GprxExtended>> GetActivities()
        {
            if (this.activities == null)
            {
                try
                {
                    await activitiesSemaphore.WaitAsync();
                    if (this.activities == null)
                    {
                        await this.dataManager.LoadAllAsync();
                        this.activities = this.dataManager.GetAll().Select(l => new GprxExtended(l.Task.Result));
                    }
                }
                finally { activitiesSemaphore.Release(); }
            }

            return this.activities;
        }

        public async Task<GprxExtended> GetMerged()
        {
            var activityMerger = new ActivityMerger(await this.GetActivities());
            return new GprxExtended(activityMerger.GetMerged());
        }
    }
}
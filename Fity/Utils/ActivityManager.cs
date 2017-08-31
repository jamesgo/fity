using Fity.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Fity.Utils;
using System;
using System.Threading;
using Fity.Models;
using Fity.Data.TCX;

namespace Fity.Utils
{
    internal class ActivityManager
    {
        private GpsDataManager dataManager;

        public ActivityManager(GpsDataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        private SemaphoreSlim activitiesSemaphore = new SemaphoreSlim(1);
        private IEnumerable<Session> activities = null;
        public async Task<IEnumerable<Session>> GetSessions()
        {
            if (this.activities == null)
            {
                try
                {
                    await activitiesSemaphore.WaitAsync();
                    if (this.activities == null)
                    {
                        await this.dataManager.LoadAllAsync();
                        this.activities = this.dataManager.GetAll().Select(l => l.Task.Result.ToModel());
                    }
                }
                finally { activitiesSemaphore.Release(); }
            }

            return this.activities;
        }

        public async Task<Session> GetMerged()
        {
            var activityMerger = new ActivityMerger(await this.GetSessions());
            return activityMerger.GetMerged();
        }
    }
}
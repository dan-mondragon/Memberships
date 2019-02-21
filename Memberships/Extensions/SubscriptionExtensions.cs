using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity;
using Memberships.Entities;
using Memberships.Models;

namespace Memberships.Extensions
{
    public static class SubscriptionExtensions
    {
        public static async Task<int> GetSubscriptionIdByRegistrationCode(this IDbSet<Subscription> subscription, string code)
        {
            try
            {
                if (subscription == null || code == null || code.Equals(String.Empty))
                    return Int32.MinValue;

                var subscriptionId = await subscription.Where(s => s.RegistrationCode.Equals(code)).Select(s => s.Id).FirstOrDefaultAsync();
                return subscriptionId;
            }
            catch
            {
                return Int32.MinValue;
            }
        }

        public static async Task Register(this IDbSet<UserSubscription> userSubscription, int subscriptionId, string userId)
        {
            try
            {
                if (userSubscription == null || subscriptionId.Equals(Int32.MinValue) || userId.Equals(String.Empty))
                    return;

                var exists = await Task.Run(() => userSubscription.CountAsync(s => s.SubscriptionId.Equals(subscriptionId) && s.UserId.Equals(userId))) > 0;
                if (!exists)
                {
                    await Task.Run(() => userSubscription.Add(
                        new UserSubscription()
                        {
                            UserId = userId,
                            SubscriptionId = subscriptionId,
                            StartDate = DateTime.Now,
                            EndDate = DateTime.MaxValue
                        }));
                }
            }
            catch { }
        }

        public static async Task<bool> RegisterUserSubscriptionCode(string userId, string code)
        {
            try
            {
                var db = ApplicationDbContext.Create();

                var id = await db.Subscriptions.GetSubscriptionIdByRegistrationCode(code);
                if (id <= 0) return false;
                await db.UserSubscriptions.Register(id, userId);

                if (db.ChangeTracker.HasChanges())
                    await db.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }
    }
}
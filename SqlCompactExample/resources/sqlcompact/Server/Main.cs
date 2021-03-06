﻿using GTANetworkServer;
using SqlCompactExample.resources.sqlcompact.Server.Models;
using System.Data.Entity;
using System.Linq;

namespace SqlCompactExample.resources.sqlcompact.Server
{
    public class Main : Script
    {
        public Main()
        {
            API.onResourceStart += OnResourceStart;
            API.onPlayerConnected += OnPlayerConnected;
        }

        private void OnResourceStart()
        {
            var databaseFilePath = API.getResourceFolder() + "\\Database.sdf";
            ContextFactory.DatabaseFilePath = databaseFilePath;

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DefaultDbContext, MigrationConfiguration>());

            using (var database = ContextFactory.Instance)
            {
                var uniqueUsers = ContextFactory.Instance.UserProfiles.Count();
                API.consoleOutput("Unique players: " + uniqueUsers);
            }
        }

        private void OnPlayerConnected(Client player)
        {
            using (var database = ContextFactory.Instance)
            {
                var userProfile = database.UserProfiles.FirstOrDefault(up => up.SocialClubName == player.socialClubName);

                if (userProfile == null)
                {
                    API.consoleOutput("New player: " + player.socialClubName);

                    userProfile = new UserProfile { SocialClubName = player.socialClubName };
                    database.UserProfiles.Add(userProfile);
                }
                else
                {
                    API.consoleOutput($"Returning player: {player.socialClubName}. Last display name: {userProfile.LastDisplayName}, current display name: {player.name}");
                }

                userProfile.LastIp = player.address;
                userProfile.LastDisplayName = player.name;

                database.SaveChanges();
            }
        }
    }
}

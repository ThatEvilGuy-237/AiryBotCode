using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.database;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AiryBotCode.Application.Services.Database.GiveAway
{
    public class GiveAwayUserService : IGiveAwayUserService
    {
        private readonly IGiveAwayUserRepository _giveAwayUser;

        public GiveAwayUserService(IGiveAwayUserRepository giveAwayUser)
        {
            _giveAwayUser = giveAwayUser;
        }

        public async Task<List<GiveAwayUser>> GetRandomUsers(int ammount)
        {
            var allUsers = await _giveAwayUser.GetAllAsync();
            var rnd = new Random();
            var randomUsers = allUsers.OrderBy(x => rnd.Next()).Take(ammount).ToList();
            return randomUsers;
        }

        public async Task<List<GiveAwayUser>> GetAllUsers()
        {
            var users = await _giveAwayUser.GetAllAsync();
            return users.ToList();
        }

        public async Task CreateUser(GiveAwayUser user)
        {
            await _giveAwayUser.AddAsync(user);
            await _giveAwayUser.SaveChangesAsync();
        }
    }
}

using System.Threading.Tasks;
using NormalApi.Entities;
using NormalApi.Interfaces;

namespace NormalApi.Managers
{
    public class ShortlistManager : IShortlistManager
    {
        public Task AddShortlistNotes(ShortlistNoteRequest request)
        {
            return Task.CompletedTask;
        }

        public Task UpdateShortlist(ShortlistRequest request)
        {
            return Task.CompletedTask;
        }
    }
}
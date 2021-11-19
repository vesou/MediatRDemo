using System.Threading.Tasks;
using Mediator.Entities;
using Mediator.Interfaces;

namespace Mediator.Managers
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
using System.Threading.Tasks;
using Mediator.Entities;
using Mediator.Interfaces;

namespace Mediator.Managers
{
    public class ShortlistManager : IShortlistManager
    {
        public Task UpdateShortlist(ShortlistRequest request)
        {
            return null;
        }

        public Task AddShortlistNotes(ShortlistNoteRequest request)
        {
            return null;
        }
    }
}
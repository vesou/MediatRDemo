using System.Threading.Tasks;
using Mediator.Entities;

namespace Mediator.Interfaces
{
    public interface IShortlistManager
    {
        Task UpdateShortlist(ShortlistRequest request);
        Task AddShortlistNotes(ShortlistNoteRequest request);
    }
}
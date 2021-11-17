using System.Threading.Tasks;
using Mediator.Entities;

namespace Mediator.Interfaces
{
    public interface IShortlistManager
    {
        Task AddShortlistNotes(ShortlistNoteRequest request);
        Task UpdateShortlist(ShortlistRequest request);
    }
}
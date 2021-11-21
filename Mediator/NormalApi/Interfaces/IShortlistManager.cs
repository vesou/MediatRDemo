using System.Threading.Tasks;
using NormalApi.Entities;

namespace NormalApi.Interfaces
{
    public interface IShortlistManager
    {
        Task AddShortlistNotes(ShortlistNoteRequest request);
        Task UpdateShortlist(ShortlistRequest request);
    }
}
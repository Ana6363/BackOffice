using System.Threading.Tasks;
using Healthcare.Domain;

namespace Healthcare.Application.Repositories
{
    public interface ISurgeryRoomRepository
    {
        Task AddSurgeryRoomAsync(SurgeryRoom surgeryRoom);
    }
}

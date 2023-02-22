using shuttleasy.DAL.EFRepositories.ShuttleSessionSearch;
using shuttleasy.DAL.Models;
using shuttleasy.DAL.Models.dto;
using shuttleasy.DAL.Models.dto.ShuttleSessions.dto;
using static shuttleasy.DAL.EFRepositories.ShuttleSessionSearch.ShuttleSessionSearchRepository;

namespace shuttleasy.LOGIC.Logics.ShuttleSessionSearch
{
    public class ShuttleSessionSearchLogic
    {

        public List< ShuttleSessionSearchDto> InnerJoinTables(string lastPoint)
        {
            ShuttleSessionSearchRepository shuttleSessionSearchRepository = new ShuttleSessionSearchRepository();
            var list = shuttleSessionSearchRepository.InnerJoinTables(lastPoint);
            return list;
        }
    }
}

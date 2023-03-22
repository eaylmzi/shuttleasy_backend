
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static shuttleasy.LOGIC.Logics.JoinTables.JoinTableLogic;

namespace shuttleasy.LOGIC.Logics.JoinTables
{
    public interface IJoinTableLogic
    {
        public List<ShuttleDetailsGroupDto> ShuttleDetailsInnerJoinTables(string destinationName);
        public List<CommentDetailsDto> CommentDetailsInnerJoinTables(int companyId);
        public List<CompanyDetailGroupDto> CompanyDetailsInnerJoinTables(int companyId);
        public List<SessionGeoPointsDto> SessionGeoPointsInnerJoinTables(int? sessionId);
    }
}

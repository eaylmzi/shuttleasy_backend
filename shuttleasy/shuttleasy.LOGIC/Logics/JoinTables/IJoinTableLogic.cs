
using shuttleasy.DAL.Models.dto.JoinTables.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.JoinTables
{
    public interface IJoinTableLogic
    {
        public List<ShuttleDetailsDto> ShuttleDetailsInnerJoinTables(string destinationName);
        public List<CommentDetailsDto> CommentDetailsInnerJoinTables(int companyId);
    }
}

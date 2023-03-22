using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.DAL.Models.dto.JoinTables.dto
{
    public class CompanyDetailGroupDto
    {
        public Company Company { get; set; }
        public List<CommentDetailsDto> CommentDetails { get; set; }
    }
}

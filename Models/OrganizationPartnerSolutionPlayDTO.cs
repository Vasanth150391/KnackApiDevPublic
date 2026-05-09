﻿using Knack.DBEntities;

namespace Knack.API.Models
{
    public class OrganizationPartnerSolutionPlayDTO
    {
        public Guid? PartnerSolutionPlayId { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public Guid? OrgId { get; set; }
        public string? SolutionPlayName { get; set; }
        public string? SolutionAreaName { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public string? SolutionStatus { get; set; }
        public string? DisplayLabel { get; set; }
        public int? IsPublished { get; set; }
        public Guid? SolutionPlayId { get; set; }
        public string? SolutionPlayLabel { get; set; }
        public Guid? PartnerSolutionPlayByPlayId { get; set; }
    }
    public class OrganizationPartnerSolutionPlayFilterDTO
    {
        public Guid? PartnerSolutionPlayId { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public Guid? OrgId { get; set; }
        public string? OrganizationName { get; set; }
        public string? SolutionAreaName {get; set;}
        public string? SolutionPlayName { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public string? SolutionStatus { get; set; }
        public string? DisplayLabel { get; set; }
        public int? IsPublished { get; set; }
        public Guid? PartnerSolutionPlayByPlayId { get; set; }
        public Guid? SolutionPlayId { get; set; }
    }

}
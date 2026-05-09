﻿using Knack.DBEntities;

namespace Knack.API.Models
{
    public class OrganizationPartnerSolutionDTO
    {
        public Guid? PartnerSolutionId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? IndustryId { get; set; }
        public Guid? SubIndustryId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public string? SolutionName { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public string? SolutionStatus { get; set; }
        public string? DisplayLabel { get; set; }
        public int? IsPublished { get; set; }
        public Guid? PartnerSolutionByAreaId { get; set; }
        public string? PartnerSolutionSlug { get; set; }
        public string? PartnerSolutionDesc { get; set; }
        public string? LogoFileLink { get; set; }
        public string? ParentSolutionId { get; set; }
    }
    public class OrganizationPartnerSolutionFilterDTO
    {
        public Guid? PartnerSolutionId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? IndustryId { get; set; }
        public Guid? SubIndustryId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public string? SolutionName { get; set; }
        public string? SubIndustryName { get; set; }
        public string? IndustryName { get; set; }

        public Guid? SolutionStatusId { get; set; }
        public string? SolutionStatus { get; set; }
        public string? DisplayLabel { get; set; }
        public int? IsPublished { get; set; }
        public Guid? PartnerSolutionByAreaId { get; set; }
        public string? PartnerSolutionSlug { get; set; }
        public string? ParentSolutionId { get; set; }
    }

}
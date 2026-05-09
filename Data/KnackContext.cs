using System;
using System.Collections.Generic;
using Knack.DBEntities;
using Knack.Entities.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace Knack.API.Data;

public partial class KnackContext : DbContext
{
    public KnackContext()
    {
    }

    public KnackContext(DbContextOptions<KnackContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeadPartner>(entity =>
        {
            entity.HasKey(e => e.LeadPartnerId).HasName("pk_LeadPartnerId");

            entity.ToTable("LeadPartner", "lead");

            entity.Property(e => e.CompanyName).HasMaxLength(50);
            entity.Property(e => e.Createby).HasMaxLength(50);
            entity.Property(e => e.Createdon).HasColumnType("datetime");
            entity.Property(e => e.EmailAddress).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.FTPFolderName)
                .HasMaxLength(50)
                .HasColumnName("FTPFolderName");
            entity.Property(e => e.FtpuserName)
                .HasMaxLength(50)
                .HasColumnName("FTPUserName");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Modifiedby).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public virtual DbSet<LeadPartner> LeadPartners { get; set; }
    public virtual DbSet<Geo> Geos { get; set; }

    public virtual DbSet<Industry> Industries { get; set; }

    public virtual DbSet<IndustryTheme> IndustryTheme { get; set; }
    public virtual DbSet<ArchiveIndustryTheme> ArchiveIndustryTheme { get; set; }
    public virtual DbSet<Organization> Organization { get; set; }

    public virtual DbSet<PartnerSolution> PartnerSolution { get; set; }

    public virtual DbSet<PartnerSolutionByArea> PartnerSolutionByArea { get; set; }

    public virtual DbSet<PartnerSolutionResourceLink> PartnerSolutionResourceLink { get; set; }

    public virtual DbSet<PartnerUser> PartnerUser { get; set; }

    public virtual DbSet<ResourceLink> ResourceLinks { get; set; }

    public virtual DbSet<Solution> Solutions { get; set; }

    public virtual DbSet<SolutionArea> SolutionAreas { get; set; }

    public virtual DbSet<SolutionAvailableGeo> SolutionAvailableGeos { get; set; }

    public virtual DbSet<PartnerSolutionAvailableGeo> PartnerSolutionAvailableGeo { get; set; }

    public virtual DbSet<SubIndustry> SubIndustries { get; set; }
    public virtual DbSet<ArchiveSubIndustry> ArchiveSubIndustries { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<IndustryShowcasePartnerSolution> IndustryShowcasePartnerSolution { get; set; }

    public virtual DbSet<IndustryTargetCustomerProspect> IndustryTargetCustomerProspect { get; set; }

    public virtual DbSet<IndustryThemeBySolutionArea> IndustryThemeBySolutionArea { get; set; }

    public virtual DbSet<IndustryResourceLink> IndustryResourceLink { get; set; }

    public virtual DbSet<SolutionStatusType> SolutionStatusType { get; set; }

    public virtual DbSet<SpotLight> SpotLight { get; set; }

    public virtual DbSet<UserInvite> UserInvite { get; set; }

    public virtual DbSet<SolutionPlay> SolutionPlay { get; set; }

    public virtual DbSet<PartnerSolutionPlay> PartnerSolutionPlay { get; set; }

    public virtual DbSet<PartnerSolutionPlayAvailableGeo> PartnerSolutionPlayAvailableGeo { get; set; }

    public virtual DbSet<PartnerSolutionPlayByPlay> PartnerSolutionPlayByPlay { get; set; }

    public virtual DbSet<PartnerSolutionPlayResourceLink> PartnerSolutionPlayResourceLink { get; set; }

    public virtual DbSet<UserOtp> userOtps { get; set; }
    public virtual DbSet<UsecaseOrganization> UsecaseOrganization { get; set; }

    public virtual DbSet<TechnologyShowcasePartnerSolution> TechnologyShowcasePartnerSolution { get; set; }

}

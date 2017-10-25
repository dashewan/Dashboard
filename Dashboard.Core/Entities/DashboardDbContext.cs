using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using EFCodeFirstConventions;
using EFCodeFirstConventions.Conventions;
using Dashboard.Domain.BasicData;
using Dashboard.Domain.MasterData;
using Dashboard.Domain.SysManagement;
using Dashboard.Domain.Models.BasicData;

namespace Dashboard.Core.Entities
{
    public class DashboardDbContext : ExtendedDbContext
    {
        public DashboardDbContext()
            : base(ConfigurationManager.ConnectionStrings["DashboardEntities"].ConnectionString)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<HotSpot>()
            //.HasRequired(m => m.ProjectDetail)
            //.WithMany(t => t.HotSpot)
            //.HasForeignKey(m => m.ProjectDetailID)
            //.WillCascadeOnDelete(false);

            //modelBuilder.Entity<HotSpot>()
            //            .HasRequired(m => m.ProjectDetailIsLinked)
            //            .WithMany(t => t.HotSpotLinked)
            //            .HasForeignKey(m => m.LinkProjectDetailID)
            //            .WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        protected override void AddConventions()
        {
            AddConvention(new EtendedStringConvention());
            AddConvention(new GenericDecimalConvention());
            AddConvention(new PercentConvention());
            AddConvention(new ExtendedDecimalConvention());
        }

        public DbSet<SysException> SysException { get; set; }
        public DbSet<SysOrganization> SysOrganization { get; set; }
        public DbSet<SysRole> SysRole { get; set; }
        public DbSet<SysFunction> SysFunction { get; set; }
        public DbSet<SysRoleFunction> SysRoleFunction { get; set; }
        public DbSet<SysDataAuth> SysDataAuth { get; set; }
        public DbSet<SysUser> SysUser { get; set; }
        public DbSet<SysUserRole> SysUserRole { get; set; }

        public DbSet<CBasCity> BasCity { get; set; }
        public DbSet<CBasProvince> CBasProvince { get; set; }
        public DbSet<CBasCodeType> CBasCodeType { get; set; }
        public DbSet<CBasCodeDef> CBasCodeDef { get; set; }

        public DbSet<CBasAdjustmentTable> CBasAdjustmentTable { get; set; }
        public DbSet<CBasNormalSchedule> CBasNormalSchedule { get; set; }
        public DbSet<CBasNormalScheduleS> CBasNormalScheduleS { get; set; }
        public DbSet<CBasNormalScheduleVCt> CBasNormalScheduleVCt { get; set; }
        public DbSet<CBasNormalScheduleVCtDet> CBasNormalScheduleVCtDet { get; set; }
        public DbSet<CBasNormalScheduleVTm> CBasNormalScheduleVTm { get; set; }
        public DbSet<CBasNormalScheduleVTmDet> CBasNormalScheduleVTmDet { get; set; }
        public DbSet<CBasSpecialDay> CBasSpecialDay { get; set; }
        public DbSet<CBasSpecialSchedule> CBasSpecialSchedule { get; set; }
        public DbSet<CBasPdcSequence> CBasPdcSequence { get; set; }
        public DbSet<CBasAllDataFormConfig> CBasAllDataFormConfig { get; set; }
        public DbSet<CBasSpecialScheduleIndex> CBasSpecialScheduleIndex { get; set; }
        //public DbSet<CBasCR> CBasCR { get; set; }
    }
}

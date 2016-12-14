using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MyVote.Data.Entities
{
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
	public interface IEntitiesContext : IDisposable
	{
		DbSet<ActiveUsers> ActiveUsers { get; set; }
		DbSet<Mvcategory> Mvcategory { get; set; }
		DbSet<Mvdates> Mvdates { get; set; }
		DbSet<Mvgeography> Mvgeography { get; set; }
		DbSet<Mvpoll> Mvpoll { get; set; }
		DbSet<MvpollComment> MvpollComment { get; set; }
		DbSet<MvpollOption> MvpollOption { get; set; }
		DbSet<MvpollResponse> MvpollResponse { get; set; }
		DbSet<MvpollSubmission> MvpollSubmission { get; set; }
		DbSet<MvreportedPoll> MvreportedPoll { get; set; }
		DbSet<MvreportedPollStateLog> MvreportedPollStateLog { get; set; }
		DbSet<MvreportedPollStateOption> MvreportedPollStateOption { get; set; }
		DbSet<Mvuser> Mvuser { get; set; }
		DbSet<MvuserRole> MvuserRole { get; set; }
		DbSet<MyVotePhotos> MyVotePhotos { get; set; }
		EntityEntry Entry(object entity);
		void SetState(object entity, EntityState state);
		int SaveChanges();

		Guid Identity { get; }
	}

	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
	public partial class EntitiesContext : DbContext, IEntitiesContext
	{
		private readonly IConfigurationRoot root;

		public EntitiesContext(IConfigurationRoot root)
		{
			if(root == null)
			{
				throw new ArgumentNullException(nameof(root));
			}

			this.root = root;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ActiveUsers>(entity =>
			{
				entity.ToTable("ActiveUsers", "MyVote");

				entity.Property(e => e.Id).HasColumnName("id");

				entity.Property(e => e.ContainerName).HasColumnName("containerName");

				entity.Property(e => e.ResourceName).HasColumnName("resourceName");

				entity.Property(e => e.Sas).HasColumnName("sas");
			});

			modelBuilder.Entity<Mvcategory>(entity =>
			{
				entity.HasKey(e => e.CategoryId)
					 .HasName("PK__MVCatego__19093A2B53E06C8F");

				entity.ToTable("MVCategory");

				entity.Property(e => e.CategoryId)
					 .HasColumnName("CategoryID")
					 .ValueGeneratedNever();

				entity.Property(e => e.CategoryName)
					 .IsRequired()
					 .HasMaxLength(50);
			});

			modelBuilder.Entity<Mvdates>(entity =>
			{
				entity.HasKey(e => e.DateKey)
					 .HasName("pk_MVDates");

				entity.ToTable("MVDates");

				entity.Property(e => e.DateKey).ValueGeneratedNever();

				entity.Property(e => e.CalendarMonthName)
					 .IsRequired()
					 .HasColumnType("varchar(20)");

				entity.Property(e => e.CalendarMonthNod).HasColumnName("CalendarMonthNOD");

				entity.Property(e => e.CalendarMonthShort)
					 .IsRequired()
					 .HasColumnType("char(8)");

				entity.Property(e => e.CalendarQuarterName)
					 .IsRequired()
					 .HasColumnType("char(15)");

				entity.Property(e => e.CalendarQuarterNod).HasColumnName("CalendarQuarterNOD");

				entity.Property(e => e.CalendarQuarterShort)
					 .IsRequired()
					 .HasColumnType("char(7)");

				entity.Property(e => e.CalendarWeekName)
					 .IsRequired()
					 .HasColumnType("varchar(20)");

				entity.Property(e => e.CalendarWeekNod).HasColumnName("CalendarWeekNOD");

				entity.Property(e => e.CalendarWeekShort)
					 .IsRequired()
					 .HasColumnType("char(9)");

				entity.Property(e => e.CalendarYearNod).HasColumnName("CalendarYearNOD");

				entity.Property(e => e.DateId)
					 .HasColumnName("DateID")
					 .HasColumnType("date");

				entity.Property(e => e.DayNme)
					 .IsRequired()
					 .HasColumnType("varchar(50)");

				entity.Property(e => e.DayNod).HasColumnName("DayNOD");

				entity.Property(e => e.DayShort)
					 .IsRequired()
					 .HasColumnType("char(10)");
			});

			modelBuilder.Entity<Mvgeography>(entity =>
			{
				entity.HasKey(e => e.GeographyKey)
					 .HasName("pk_MVGeography");

				entity.ToTable("MVGeography");

				entity.Property(e => e.AreaCodes)
					 .HasColumnName("Area_Codes")
					 .HasColumnType("varchar(50)");

				entity.Property(e => e.County).HasColumnType("varchar(50)");

				entity.Property(e => e.EstimatedPopulation)
					 .HasColumnName("Estimated_Population")
					 .HasColumnType("varchar(50)");

				entity.Property(e => e.Latitude).HasColumnType("varchar(50)");

				entity.Property(e => e.Longitude).HasColumnType("varchar(50)");

				entity.Property(e => e.PrimaryCity)
					 .HasColumnName("Primary_City")
					 .HasColumnType("varchar(50)");

				entity.Property(e => e.State).HasColumnType("varchar(50)");

				entity.Property(e => e.TimeZone).HasColumnType("varchar(50)");

				entity.Property(e => e.Zip).HasColumnType("varchar(50)");
			});

			modelBuilder.Entity<Mvpoll>(entity =>
			{
				entity.HasKey(e => e.PollId)
					 .HasName("pk_MVPoll");

				entity.ToTable("MVPoll");

				entity.Property(e => e.PollId).HasColumnName("PollID");

				entity.Property(e => e.PollCategoryId).HasColumnName("PollCategoryID");

				entity.Property(e => e.PollImageLink).HasMaxLength(500);

				entity.Property(e => e.PollQuestion)
					 .IsRequired()
					 .HasMaxLength(1000);

				entity.Property(e => e.UserId).HasColumnName("UserID");

				entity.HasOne(d => d.PollCategory)
					 .WithMany(p => p.Mvpoll)
					 .HasForeignKey(d => d.PollCategoryId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("FK_MVPoll_MVCategory");

				entity.HasOne(d => d.User)
					 .WithMany(p => p.Mvpoll)
					 .HasForeignKey(d => d.UserId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVPoll_MVUser");
			});

			modelBuilder.Entity<MvpollComment>(entity =>
			{
				entity.HasKey(e => e.PollCommentId)
					 .HasName("pk_MVPollComment");

				entity.ToTable("MVPollComment");

				entity.Property(e => e.PollCommentId).HasColumnName("PollCommentID");

				entity.Property(e => e.CommentText).HasMaxLength(1000);

				entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");

				entity.Property(e => e.PollId).HasColumnName("PollID");

				entity.Property(e => e.UserId).HasColumnName("UserID");

				entity.HasOne(d => d.Poll)
					 .WithMany(p => p.MvpollComment)
					 .HasForeignKey(d => d.PollId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVPoll_MVPollComment");

				entity.HasOne(d => d.User)
					 .WithMany(p => p.MvpollComment)
					 .HasForeignKey(d => d.UserId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVUser_MVPollComment");
			});

			modelBuilder.Entity<MvpollOption>(entity =>
			{
				entity.HasKey(e => e.PollOptionId)
					 .HasName("pk_MVPollOption");

				entity.ToTable("MVPollOption");

				entity.Property(e => e.PollOptionId).HasColumnName("PollOptionID");

				entity.Property(e => e.OptionText)
					 .IsRequired()
					 .HasMaxLength(200);

				entity.Property(e => e.PollId).HasColumnName("PollID");

				entity.HasOne(d => d.Poll)
					 .WithMany(p => p.MvpollOption)
					 .HasForeignKey(d => d.PollId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVPoll_MVPollOption");
			});

			modelBuilder.Entity<MvpollResponse>(entity =>
			{
				entity.HasKey(e => e.PollResponseId)
					 .HasName("pk_MVPollResponse");

				entity.ToTable("MVPollResponse");

				entity.Property(e => e.PollResponseId).HasColumnName("PollResponseID");

				entity.Property(e => e.PollId).HasColumnName("PollID");

				entity.Property(e => e.PollOptionId).HasColumnName("PollOptionID");

				entity.Property(e => e.PollSubmissionId).HasColumnName("PollSubmissionID");

				entity.Property(e => e.UserId).HasColumnName("UserID");

				entity.HasOne(d => d.Poll)
					 .WithMany(p => p.MvpollResponse)
					 .HasForeignKey(d => d.PollId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVPoll_MVPollResponse");

				entity.HasOne(d => d.PollOption)
					 .WithMany(p => p.MvpollResponse)
					 .HasForeignKey(d => d.PollOptionId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVPollOption_MVPollResponse");

				entity.HasOne(d => d.PollSubmission)
					 .WithMany(p => p.MvpollResponse)
					 .HasForeignKey(d => d.PollSubmissionId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVPollSubmission_MVPollResponse");

				entity.HasOne(d => d.User)
					 .WithMany(p => p.MvpollResponse)
					 .HasForeignKey(d => d.UserId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVUser_MVPollResponse");
			});

			modelBuilder.Entity<MvpollSubmission>(entity =>
			{
				entity.HasKey(e => e.PollSubmissionId)
					 .HasName("pk_MVPollSubmission");

				entity.ToTable("MVPollSubmission");

				entity.Property(e => e.PollSubmissionId).HasColumnName("PollSubmissionID");

				entity.Property(e => e.PollId).HasColumnName("PollID");

				entity.Property(e => e.PollSubmissionComment).HasMaxLength(1000);

				entity.Property(e => e.UserId).HasColumnName("UserID");

				entity.HasOne(d => d.Poll)
					 .WithMany(p => p.MvpollSubmission)
					 .HasForeignKey(d => d.PollId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVPoll_MVPollSubmission");

				entity.HasOne(d => d.User)
					 .WithMany(p => p.MvpollSubmission)
					 .HasForeignKey(d => d.UserId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVUser_MVPollSubmission");
			});

			modelBuilder.Entity<MvreportedPoll>(entity =>
			{
				entity.HasKey(e => e.ReportedPollId)
					 .HasName("pk_MVReportedPoll");

				entity.ToTable("MVReportedPoll");

				entity.Property(e => e.ReportedPollId).HasColumnName("ReportedPollID");

				entity.Property(e => e.CurrentStateAdminUserId).HasColumnName("CurrentStateAdminUserID");

				entity.Property(e => e.DateReported).HasDefaultValueSql("getutcdate()");

				entity.Property(e => e.PollId).HasColumnName("PollID");

				entity.Property(e => e.ReportComments).HasMaxLength(500);

				entity.Property(e => e.ReportedByUserId).HasColumnName("ReportedByUserID");

				entity.Property(e => e.ReportedPollStateOptionId).HasColumnName("ReportedPollStateOptionID");

				entity.HasOne(d => d.CurrentStateAdminUser)
					 .WithMany(p => p.MvreportedPollCurrentStateAdminUser)
					 .HasForeignKey(d => d.CurrentStateAdminUserId)
					 .HasConstraintName("fk_CurrentStateAdminUser_MVReportedPoll");

				entity.HasOne(d => d.Poll)
					 .WithMany(p => p.MvreportedPoll)
					 .HasForeignKey(d => d.PollId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVPoll_MVReportedPoll");

				entity.HasOne(d => d.ReportedByUser)
					 .WithMany(p => p.MvreportedPollReportedByUser)
					 .HasForeignKey(d => d.ReportedByUserId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_ReportedByUser_MVReportedPoll");

				entity.HasOne(d => d.ReportedPollStateOption)
					 .WithMany(p => p.MvreportedPoll)
					 .HasForeignKey(d => d.ReportedPollStateOptionId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVReportedPollStateOption_MVReportedPoll");
			});

			modelBuilder.Entity<MvreportedPollStateLog>(entity =>
			{
				entity.HasKey(e => e.ReportedPollStateLogId)
					 .HasName("pk_MVReportedPollStateLog");

				entity.ToTable("MVReportedPollStateLog");

				entity.Property(e => e.ReportedPollStateLogId).HasColumnName("ReportedPollStateLogID");

				entity.Property(e => e.PollId).HasColumnName("PollID");

				entity.Property(e => e.ReportedPollId).HasColumnName("ReportedPollID");

				entity.Property(e => e.StateAdminUserId).HasColumnName("StateAdminUserID");

				entity.Property(e => e.StateChangeComments).HasMaxLength(500);

				entity.HasOne(d => d.Poll)
					 .WithMany(p => p.MvreportedPollStateLog)
					 .HasForeignKey(d => d.PollId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVPoll_MVReportedPollStateLog");

				entity.HasOne(d => d.ReportedPoll)
					 .WithMany(p => p.MvreportedPollStateLog)
					 .HasForeignKey(d => d.ReportedPollId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_MVReportedPoll_MVReportedPollStateLog");

				entity.HasOne(d => d.StateAdminUser)
					 .WithMany(p => p.MvreportedPollStateLog)
					 .HasForeignKey(d => d.StateAdminUserId)
					 .OnDelete(DeleteBehavior.Restrict)
					 .HasConstraintName("fk_StateAdminUser_MVReportedPollStateLog");
			});

			modelBuilder.Entity<MvreportedPollStateOption>(entity =>
			{
				entity.HasKey(e => e.ReportedPollStateOptionId)
					 .HasName("pk_MVReportedPollStateOption");

				entity.ToTable("MVReportedPollStateOption");

				entity.Property(e => e.ReportedPollStateOptionId)
					 .HasColumnName("ReportedPollStateOptionID")
					 .ValueGeneratedNever();

				entity.Property(e => e.ReportedPollStateComments).HasMaxLength(500);

				entity.Property(e => e.ReportedPollStateName)
					 .IsRequired()
					 .HasMaxLength(50);
			});

			modelBuilder.Entity<Mvuser>(entity =>
			{
				entity.HasKey(e => e.UserId)
					 .HasName("pk_MVUser");

				entity.ToTable("MVUser");

				entity.HasIndex(e => e.UserName)
					 .HasName("cuidx_MVUser_UserName")
					 .IsUnique();

				entity.Property(e => e.UserId).HasColumnName("UserID");

				entity.Property(e => e.AuditCreateDate).HasDefaultValueSql("getutcdate()");

				entity.Property(e => e.BirthDate).HasColumnType("date");

				entity.Property(e => e.EmailAddress)
					 .IsRequired()
					 .HasMaxLength(200);

				entity.Property(e => e.FirstName).HasMaxLength(100);

				entity.Property(e => e.Gender).HasColumnType("nchar(1)");

				entity.Property(e => e.LastName).HasMaxLength(100);

				entity.Property(e => e.PostalCode).HasMaxLength(20);

				entity.Property(e => e.ProfileAuthToken).HasMaxLength(2100);

				entity.Property(e => e.ProfileId)
					 .HasColumnName("ProfileID")
					 .HasMaxLength(200);

				entity.Property(e => e.UserName)
					 .IsRequired()
					 .HasMaxLength(200);

				entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");

				entity.HasOne(d => d.UserRole)
					 .WithMany(p => p.Mvuser)
					 .HasForeignKey(d => d.UserRoleId)
					 .HasConstraintName("fk_MVUser_MVUserRole");
			});

			modelBuilder.Entity<MvuserRole>(entity =>
			{
				entity.HasKey(e => e.UserRoleId)
					 .HasName("pk_MVRole");

				entity.ToTable("MVUserRole");

				entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");

				entity.Property(e => e.UserRoleName)
					 .IsRequired()
					 .HasMaxLength(100);
			});

			modelBuilder.Entity<MyVotePhotos>(entity =>
			{
				entity.ToTable("MyVotePhotos", "MyVote");

				entity.Property(e => e.Id).HasColumnName("id");
			});
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SQLCONNSTR_Entities"));
            base.OnConfiguring(optionsBuilder);
		}

		public virtual DbSet<ActiveUsers> ActiveUsers { get; set; }
		public virtual DbSet<Mvcategory> Mvcategory { get; set; }
		public virtual DbSet<Mvdates> Mvdates { get; set; }
		public virtual DbSet<Mvgeography> Mvgeography { get; set; }
		public virtual DbSet<Mvpoll> Mvpoll { get; set; }
		public virtual DbSet<MvpollComment> MvpollComment { get; set; }
		public virtual DbSet<MvpollOption> MvpollOption { get; set; }
		public virtual DbSet<MvpollResponse> MvpollResponse { get; set; }
		public virtual DbSet<MvpollSubmission> MvpollSubmission { get; set; }
		public virtual DbSet<MvreportedPoll> MvreportedPoll { get; set; }
		public virtual DbSet<MvreportedPollStateLog> MvreportedPollStateLog { get; set; }
		public virtual DbSet<MvreportedPollStateOption> MvreportedPollStateOption { get; set; }
		public virtual DbSet<Mvuser> Mvuser { get; set; }
		public virtual DbSet<MvuserRole> MvuserRole { get; set; }
		public virtual DbSet<MyVotePhotos> MyVotePhotos { get; set; }

		public void SetState(object entity, EntityState state)
		{
			this.Entry(entity).State = state;
		}

		public Guid Identity { get; private set; }
	}
}


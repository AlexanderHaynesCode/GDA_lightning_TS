using Microsoft.EntityFrameworkCore;

namespace GDA_lightning_TS.Models;

public partial class CoreDbContext : DbContext
{
    private readonly IConfiguration _config;
    private readonly string _AzureSQLConnString;
    public CoreDbContext()
    {
        _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        _AzureSQLConnString = _config.GetValue<string>("ConnectionStrings:AZURE_SQL_CONNECTION");
    }

    public CoreDbContext(DbContextOptions<CoreDbContext> options)
        : base(options)
    {
        
    }

    public virtual DbSet<User_GDA> User_GDA { get; set; }
    public virtual DbSet<UserLoggedIn_GDA> UserLoggedIn_GDA { get; set; } 
    public virtual DbSet<Conversation_GDA> Conversations_GDA { get; set; }
    public virtual DbSet<ConversationWithUsernames_GDA> ConversationWithUsernames_GDA { get; set; }
    public virtual DbSet<ConversationWithUnread_GDA> ConversationWithUnread_GDA { get; set; }
    public virtual DbSet<StringObj> StringObj { get; set; }
    public virtual DbSet<IntObject> IntObject { get; set; }
    public virtual DbSet<Message_GDA> Messages_GDA { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_AzureSQLConnString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User_GDA>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User_GDA__3214EC272242A497");

            entity.ToTable("User_GDA");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.LookingFor)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePicPath)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserLoggedIn_GDA>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserLogg__3214EC277605D10A");

            entity.ToTable("UserLoggedIn_GDA");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ExpiresOn).HasColumnType("datetime");
            entity.Property(e => e.User_ID).HasMaxLength(256);
            entity.Property(e => e.IsLoggedIn).HasColumnType("bit");
            entity.Property(e => e.IPAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Conversation_GDA>(entity =>
        {
            entity.HasKey(e => e.Conversation_ID).HasName("PK__Conversa__CAA577F354665BE2");

            entity.ToTable("Conversations_GDA");

            entity.Property(e => e.Conversation_ID).HasColumnName("Conversation_ID");
            entity.Property(e => e.Person1_ID).HasMaxLength(256);
            entity.Property(e => e.Person2_ID).HasMaxLength(256);
            entity.Property(e => e.Person1_InConvo).HasColumnType("bit");
            entity.Property(e => e.Person2_InConvo).HasColumnType("bit");
        }); 

        modelBuilder.Entity<ConversationWithUsernames_GDA>(entity =>
        {
            entity.HasKey(e => e.Conversation_ID).HasName("PK__Conversa__CAA577F354665BE2");

            entity.ToTable("ConversationWithUsernames_GDA");

            entity.Property(e => e.Conversation_ID).HasColumnName("Conversation_ID");
            entity.Property(e => e.Person1_ID).HasMaxLength(256);
            entity.Property(e => e.Person2_ID).HasMaxLength(256);
            entity.Property(e => e.Person1_InConvo).HasColumnType("bit");
            entity.Property(e => e.Person2_InConvo).HasColumnType("bit");
            entity.Property(e => e.Person1_Username).HasMaxLength(255);
            entity.Property(e => e.Person2_Username).HasMaxLength(255);
        });  

        modelBuilder.Entity<ConversationWithUnread_GDA>(entity =>
        {
            entity.HasKey(e => e.Conversation_ID).HasName("PK__Conversa__CAA577F354665BE2");

            entity.ToTable("ConversationWithUnread_GDA");

            entity.Property(e => e.Conversation_ID).HasColumnName("Conversation_ID");
            entity.Property(e => e.Person1_ID).HasMaxLength(256);
            entity.Property(e => e.Person2_ID).HasMaxLength(256);
            entity.Property(e => e.Person1_InConvo).HasColumnType("bit");
            entity.Property(e => e.Person2_InConvo).HasColumnType("bit");
            entity.Property(e => e.Person1_Username).HasMaxLength(255);
            entity.Property(e => e.Person2_Username).HasMaxLength(255);
            entity.Property(e => e.Has_Unread_Message).HasColumnType("bit");
        });

        modelBuilder.Entity<StringObj>(entity =>
        {
            entity.HasKey(e => e.Value).HasName("PK__Conversa__CAA577F354665BE2");

            entity.ToTable("BooleanObj");

            entity.Property(e => e.Value).HasMaxLength(5);
        });  

        modelBuilder.Entity<IntObject>(entity =>
        {
            entity.HasKey(e => e.Value).HasName("PK__Conversa__CAA577F354665BE2");

            entity.ToTable("IntObject");

            entity.Property(e => e.Value).HasMaxLength(256);
        });

        modelBuilder.Entity<Message_GDA>(entity =>
        {
            entity.HasKey(e => e.Message_ID).HasName("PK__Messages__F5A446E2D5B6E802");

            entity.ToTable("Messages_GDA");

            entity.Property(e => e.Conversation_ID).HasMaxLength(256);
            entity.Property(e => e.Message_ID).HasColumnName("Message_ID");
            entity.Property(e => e.From_ID).HasMaxLength(256);
            entity.Property(e => e.To_ID).HasMaxLength(256);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(5000);

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.Conversation_ID)
                .HasConstraintName("FK__Messages___Conve__36470DEF");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
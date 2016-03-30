using Nop.Core.Domain.Common;
namespace Nop.Data.Mapping.Common
{
    public partial class CommonDataMap : NopEntityTypeConfiguration<CustomData>
    {
        public CommonDataMap()
        {
            this.ToTable("CustomData");
            this.HasKey(ga => ga.Id);

            this.Property(ga => ga.KeyGroup).IsRequired().HasMaxLength(50);
            this.Property(ga => ga.Key).IsRequired().HasMaxLength(50);
            this.Property(ga => ga.Value).IsRequired();
        }
    }
}

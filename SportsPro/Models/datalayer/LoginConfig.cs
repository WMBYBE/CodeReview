using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace SportsPro.Models.datalayer
{
    public class LoginConfig : IEntityTypeConfiguration<Login>
    {
        public void Configure(EntityTypeBuilder<Login> entity)
        {
            new Login { Username = "admin", Password = "P@ssw0rd" };


        }
    }
}

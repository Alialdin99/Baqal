using Baqal.DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.DataAccess.ExtensionMethods
{
    public static class RepositoryCollectionExtension
    {
        public static IServiceCollection AddRepository(this IServiceCollection service)
            => service
                .AddScoped<IUnitOfWork, UnitOfWork>();
    }
}

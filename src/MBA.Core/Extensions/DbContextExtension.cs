
using Microsoft.EntityFrameworkCore;

namespace MBA.Core.Extensions;
public static class DbContextExtensions
{
    public static void AtualizarEstadoValueObject<T>(this DbContext context, T? antigo, T? novo)
        where T : class
    {
        if (antigo is not null)
            context.Entry(antigo).State = EntityState.Deleted;

        if (novo is not null)
            context.Entry(novo).State = EntityState.Added;
    }
}
using AutoMapper.Extensions.ExpressionMapping;
using BrainRing.Application.Interfaces.Repositories;
using BrainRing.DbAdapter.Interfaces.Mappers;
using BrainRing.DbAdapter.Mappers;
using BrainRing.DbAdapter.Mappers.Profiles;
using BrainRing.DbAdapter.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrainRing.DbAdapter.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDBService(this  IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<BrainRingDbContext>(options =>
                options.UseSqlite(connectionString));

            return services;
        }
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAnswersRepository, AnswersRepository>();
            services.AddScoped<IGameSessionsRepository, GameSessionsRepository>();
            services.AddScoped<IGameSessionsUsersRepository, GameSessionsUsersRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IQuestionsOptionsRepository, QuestionsOptionsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();

            return services;
        }
        public static IServiceCollection RegisterEntityMapper(this IServiceCollection services)
        {
            services.AddScoped<IEntityMapper, EntityMapper>();

            services.AddAutoMapper(cfg => {
                cfg.AddExpressionMapping();
            }, typeof(AnswerProfile).Assembly);

            return services;
        }
    }
}

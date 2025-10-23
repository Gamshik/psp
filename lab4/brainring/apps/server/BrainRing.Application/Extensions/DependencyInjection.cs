using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BrainRing.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IGameSessionService, GameSessionService>();
            services.AddScoped<IQuestionService, QuestionService>();

            return services;
        }
    }
}

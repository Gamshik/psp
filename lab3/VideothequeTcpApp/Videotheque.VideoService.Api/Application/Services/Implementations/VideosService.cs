using Videotheque.VideoService.Api.Application.Services.Interfaces;
using Videotheque.VideoService.Api.Infrastructure.Persistence;
using Videotheque.VideoService.Api.Models.Domain;
using Videotheque.VideoService.Api.Models.DTO;

namespace Videotheque.VideoService.Api.Application.Services.Implementations
{
    public class VideosService : IVideosService
    {
        private readonly AppDbContext _context;

        public VideosService(AppDbContext context)
        {
            _context = context;
        }

        public VideosService() : this(new AppDbContextFactory().CreateDbContext(Array.Empty<string>()))
        {
        }

        public IEnumerable<VideoDTO> GetAll()
        {
            return _context.Videos.Select(v => MapToDTO(v)).ToList();
        }

        public VideoDTO? GetById(Guid id)
        {
            var video = _context.Videos.FirstOrDefault(v => v.Id == id);
            return video != null ? MapToDTO(video) : null;
        }

        public VideoDTO Create(VideoDTO videoDTO)
        {
            var video = new Video
            {
                Id = Guid.NewGuid(),
                Title = videoDTO.Title,
                Genre = videoDTO.Genre,
                Year = videoDTO.Year,
                Director = videoDTO.Director,
                Price = videoDTO.Price,
                AvailableCopies = videoDTO.AvailableCopies
            };

            _context.Videos.Add(video);
            _context.SaveChanges();
            return MapToDTO(video);
        }

        public bool Update(VideoDTO videoDTO)
        {
            var existing = _context.Videos.FirstOrDefault(v => v.Id == videoDTO.Id);
            if (existing == null) return false;

            existing.Title = videoDTO.Title;
            existing.Genre = videoDTO.Genre;
            existing.Year = videoDTO.Year;
            existing.Director = videoDTO.Director;
            existing.Price = videoDTO.Price;
            existing.AvailableCopies = videoDTO.AvailableCopies;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var existing = _context.Videos.FirstOrDefault(v => v.Id == id);
            if (existing == null) return false;

            _context.Videos.Remove(existing);
            _context.SaveChanges();
            return true;
        }

        public static VideoDTO MapToDTO(Video entity)
        {
            return new VideoDTO
            {
                Id = entity.Id,
                Title = entity.Title,
                Genre = entity.Genre,
                Year = entity.Year,
                Director = entity.Director,
                Price = entity.Price,
                AvailableCopies = entity.AvailableCopies
            };
        }
    }
}
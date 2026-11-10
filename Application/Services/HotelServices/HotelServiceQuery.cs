using Application.Interfaces;
using Application.Models;
using Infrastructure.Context;
using Infrastructure.Models;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Services.HotelServices
{
    public class HotelServiceQuery(FlexiRoomsContext flexiRoomsContext,
                                   ILogger<HotelServiceQuery> logger) : IHotelService
    {
        private readonly FlexiRoomsContext flexiRoomsContext = flexiRoomsContext;

        public async Task<HotelDto> Create(HotelDto entity)
        {
            flexiRoomsContext.Hotels.Add(entity);

            await flexiRoomsContext.SaveChangesAsync();

            return entity;
        }

        public async Task Delete(int id)
        {
            var hotelToDelete = await flexiRoomsContext.Hotels.FindAsync(id);
            if (hotelToDelete is null)
                throw new ArgumentNullException(nameof(hotelToDelete));
            flexiRoomsContext.Hotels.Remove(hotelToDelete);
            await flexiRoomsContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<HotelDto>> GetAllById(int id)
        {
            logger.LogInformation("MethodName: {GetAllById} - Parameter: {entity}", nameof(GetAllById), id);

            var resultQuery = await flexiRoomsContext.Hotels.Where(x => x.Id == id).Include(x => x.AddressHotel).ToListAsync();

            logger.LogInformation("MethodName: {GetAllById} - result: {entity}", nameof(GetAllById), System.Text.Json.JsonSerializer.Serialize(resultQuery));

            List<HotelDto> resultList = new();

            foreach (var result in resultQuery)
            {
                resultList.Add(result);
            }
            return resultList;
        }

        public async Task<HotelDto> GetById(int id)
        {
            var _hotel = await flexiRoomsContext.Hotels.Where(x => x.Id == id).Include(x => x.AddressHotel).FirstOrDefaultAsync();

            if (_hotel is null)
                return await Task.FromResult<HotelDto>(result: new());

            return _hotel;
        }

        public async Task<IEnumerable<HotelDto>> GetMyHotels(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User id is required.", nameof(userId));

            List<Hotel> hotels = await flexiRoomsContext.Hotels
                .Where(x => x.IdentityNumber == userId)
                .Include(x => x.AddressHotel)
                .ToListAsync();

            List<HotelDto> hotelDtos = new();
            for (int i = 0;  i < hotels.Count; i++ )
            {
                hotelDtos.Add(hotels[i]);
            }

            return hotelDtos;
        }

        public async Task<IEnumerable<HotelDto>> GetHomeHotels(int pageNumber)
        {
            const int pageSize = 50;

            if (pageNumber <= 0)
                pageNumber = 1;

            List<Hotel> hotels = await flexiRoomsContext.Hotels
                .Where(x => x.ApprovalStatus == HotelApprovalStatus.Approved)
                .OrderBy(x => x.Id)
                .Include(x => x.AddressHotel)
                .Include(x => x.HotelPictures)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<HotelDto> hotelDtos = new();
            for (int i = 0; i < hotels.Count; i++)
            {
                hotelDtos.Add(hotels[i]);
            }

            return hotelDtos;
        }

        public async Task<IEnumerable<HotelDto>> SearchByKeyword(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                throw new ArgumentNullException(nameof(keyword));

            try
            {
                IEnumerable<Hotel> resultHotels = await flexiRoomsContext.Hotels
                    .Where(x => x.ApprovalStatus == HotelApprovalStatus.Approved)
                    .Where(x => EF.Functions.ILike(x.MetaDescription, $"%{keyword}%"))
                    .Include(x => x.AddressHotel)
                    .Include(x => x.HotelPictures)
                    .ToListAsync();

                IList<HotelDto> hoteles = new List<HotelDto>();

                foreach (var hotel in resultHotels)
                {
                    hoteles.Add(hotel);
                }

                return hoteles;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<HotelOwnership?> GetOwnership(int hotelId)
        {
            // AsNoTracking: the caller may Update() a detached entity with the same key afterwards.
            return await flexiRoomsContext.Hotels
                .AsNoTracking()
                .Where(hotel => hotel.Id == hotelId)
                .Select(hotel => new HotelOwnership(hotel.Id, hotel.IdentityNumber, hotel.ApprovalStatus))
                .FirstOrDefaultAsync();
        }

        public async Task Update(HotelDto entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var entry = flexiRoomsContext.Hotels.Update(entity);

            // Owner edits never change the review state: only ReviewHotel() does.
            entry.Property(hotel => hotel.ApprovalStatus).IsModified = false;
            entry.Property(hotel => hotel.RejectionReason).IsModified = false;
            entry.Property(hotel => hotel.SubmittedAtUtc).IsModified = false;
            entry.Property(hotel => hotel.ReviewedAtUtc).IsModified = false;
            entry.Property(hotel => hotel.ReviewedByUserId).IsModified = false;

            await flexiRoomsContext.SaveChangesAsync();
        }

        public async Task<HotelDto> CreateForOwner(HotelDto entity, string ownerUserId, bool approveImmediately)
        {
            ArgumentNullException.ThrowIfNull(entity);

            Hotel hotel = entity;
            hotel.IdentityNumber = ownerUserId;
            hotel.SubmittedAtUtc = DateTime.UtcNow;

            if (approveImmediately)
            {
                hotel.ApprovalStatus = HotelApprovalStatus.Approved;
                hotel.ReviewedAtUtc = hotel.SubmittedAtUtc;
                hotel.ReviewedByUserId = ownerUserId;
            }
            else
            {
                hotel.ApprovalStatus = HotelApprovalStatus.Pending;
            }

            flexiRoomsContext.Hotels.Add(hotel);
            await flexiRoomsContext.SaveChangesAsync();

            logger.LogInformation("Hotel {HotelId} created by {UserId} with status {Status}.", hotel.Id, ownerUserId, hotel.ApprovalStatus);
            return hotel;
        }

        public async Task<IReadOnlyList<PendingHotelDto>> GetPendingHotels()
        {
            return await flexiRoomsContext.Hotels
                .AsNoTracking()
                .Where(hotel => hotel.ApprovalStatus == HotelApprovalStatus.Pending)
                .OrderBy(hotel => hotel.SubmittedAtUtc ?? DateTime.MinValue)
                .Select(hotel => new PendingHotelDto(
                    hotel.Id,
                    hotel.Name,
                    hotel.Description,
                    hotel.Email,
                    hotel.CodeArea,
                    hotel.PhoneNumber,
                    hotel.AddressHotel != null ? hotel.AddressHotel.Street : null,
                    hotel.AddressHotel != null ? hotel.AddressHotel.Number : null,
                    hotel.Owner != null ? (hotel.Owner.Name + " " + hotel.Owner.LastName).Trim() : null,
                    hotel.Owner != null ? hotel.Owner.Email : null,
                    hotel.SubmittedAtUtc))
                .ToListAsync();
        }

        public async Task<bool> ReviewHotel(int hotelId, bool approve, string? rejectionReason, string reviewerUserId)
        {
            Hotel? hotel = await flexiRoomsContext.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
            if (hotel is null)
                return false;

            hotel.ApprovalStatus = approve ? HotelApprovalStatus.Approved : HotelApprovalStatus.Rejected;
            hotel.RejectionReason = approve ? null : rejectionReason?.Trim();
            hotel.ReviewedAtUtc = DateTime.UtcNow;
            hotel.ReviewedByUserId = reviewerUserId;

            await flexiRoomsContext.SaveChangesAsync();
            logger.LogInformation("Hotel {HotelId} reviewed by {UserId}: {Status}.", hotelId, reviewerUserId, hotel.ApprovalStatus);
            return true;
        }

        public async Task<bool> Resubmit(int hotelId)
        {
            // Straight to the database: right after Update() the tracked entity carries the
            // DTO's (unsaved) status, so reading it through the change tracker would be wrong.
            DateTime now = DateTime.UtcNow;
            int updated = await flexiRoomsContext.Hotels
                .Where(h => h.Id == hotelId && h.ApprovalStatus == HotelApprovalStatus.Rejected)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(h => h.ApprovalStatus, HotelApprovalStatus.Pending)
                    .SetProperty(h => h.SubmittedAtUtc, now));

            if (updated > 0)
                logger.LogInformation("Hotel {HotelId} resubmitted for review.", hotelId);

            return updated > 0;
        }
    }
}
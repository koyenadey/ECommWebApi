using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommWeb.Core.src.Common;
using ECommWeb.Core.src.Entity;
using ECommWeb.Core.src.RepoAbstract;
using ECommWeb.Infrastructure.src.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommWeb.Infrastructure.src.Repo;

public class ReviewRepo : IReviewRepo
{
    private readonly AppDbContext _context;

    public ReviewRepo(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Review> CreateReviewAsync(Review review)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                _context.Reviews.Add(review);
                _context.ReviewImages.AddRange(review.Images);

                await _context.SaveChangesAsync();
                var reviewsWithImages = await _context.Reviews
                                        .Include(review => review.Images)
                                        .Include(review => review.OrderedProduct)
                                            .ThenInclude(op => op.Product)
                                            .ThenInclude(p => p.Images)
                                        .Include(review => review.OrderedProduct)
                                            .ThenInclude(op => op.Product)
                                            .ThenInclude(p => p.Category)
                                        .Include(review => review.User)
                                        .FirstAsync(r => r.Id == review.Id);
                await transaction.CommitAsync();
                return reviewsWithImages;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public async Task<Review> DeleteReviewByIdAsync(Guid reviewId)
    {
        Review? reviewFound = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
        if (reviewFound != null)
        {
            _context.Reviews.Remove(reviewFound);
            await _context.SaveChangesAsync();
            return reviewFound;
        }
        return null;
    }

    public async Task<IEnumerable<Review>> GetAllReviewsAsync(QueryOptions options, Guid userId)
    {
        var pgNo = options.PageNo;
        var pgSize = options.PageSize;
        IQueryable<Review> query = _context.Reviews;
        var foundReviews = query
                            .OrderByDescending(r => r.ReviewDate)
                            .Include(review => review.Images)
                            .Include(review => review.OrderedProduct)
                                .ThenInclude(op => op.Product)
                                    .ThenInclude(p => p.Images)
                            .Include(review => review.OrderedProduct)
                                .ThenInclude(op => op.Product)
                                    .ThenInclude(p => p.Category)
                            .Include(review => review.User)
                            .Skip((pgNo - 1) * pgSize)
                            .Take(pgSize);
        return await foundReviews.ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetAllReviewsByProductIdAsync(QueryOptions options, Guid productId)
    {
        var pgNo = options.PageNo;
        var pgSize = options.PageSize;

        var orderedProductsIdList = await _context.OrderedProducts
                                    .Where(op => op.ProductId == productId)
                                    .Include(op => op.Product)
                                        .ThenInclude(p => p.Images)
                                    .Include(op => op.Product)
                                        .ThenInclude(p => p.Category)
                                    .Include(op => op.Order)
                                        .ThenInclude(o => o.User)
                                    .Select(op => op.Id)
                                    .Skip((pgNo - 1) * pgSize)
                                    .Take(pgSize)
                                    .ToListAsync();

        var filteredOrderedReviews = await _context.Reviews
                            .Where(r => orderedProductsIdList.Contains(r.OrderedProductId))
                            .ToListAsync();

        return filteredOrderedReviews;
    }

    public async Task<IEnumerable<Review>> GetAllReviewsByUserAsync(QueryOptions options, Guid userId)
    {
        return await _context.Reviews
                              .Include(review => review.OrderedProduct)
                                            .ThenInclude(op => op.Product)
                                            .ThenInclude(p => p.Images)
                            .Include(review => review.User)
                            .Where(r => r.UserId == userId).ToListAsync();
    }

    public async Task<Review> GetReviewByIdAsync(Guid reviewId)
    {
        var result = await _context.Reviews
                            .Include(review => review.OrderedProduct)
                                            .ThenInclude(op => op.Product)
                                            .ThenInclude(p => p.Images)
                            .Include(review => review.User)
                            .FirstOrDefaultAsync(r => r.Id == reviewId);
        return result!;
    }

    public async Task<Review> UpdateReviewByIdAsync(Guid reviewId, Review newReview)
    {
        Review? reviewFound = await _context.Reviews
                                    .FirstOrDefaultAsync(x => x.Id == reviewId);


        reviewFound.Comment = newReview.Comment;
        reviewFound.Rating = newReview.Rating;
        await _context.SaveChangesAsync();

        var updatedReview = await _context.Reviews
                                    .Where(x => x.Id == reviewId)
                                    .Include(review => review.OrderedProduct)
                                            .ThenInclude(op => op.Product)
                                            .ThenInclude(p => p.Images)
                                    .Include(review => review.OrderedProduct)
                                            .ThenInclude(op => op.Product)
                                                .ThenInclude(p => p.Category)
                                        .Include(review => review.User)
                                    .FirstOrDefaultAsync(x => x.Id == reviewId);
        return updatedReview!;
    }
}

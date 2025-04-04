using Microsoft.EntityFrameworkCore;
using Capstone.ECommerceApp.ShoppingCart.Data.Context;
using Capstone.ECommerceApp.ShoppingCart.Domain.Interfaces;
using Capstone.ECommerceApp.ShoppingCart.Domain.Models;

namespace Capstone.ECommerceApp.ShoppingCart.Data.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDbContext cartDbContext;
        public CartRepository(CartDbContext cartDbContext)
        {
            this.cartDbContext = cartDbContext;
        }

        public async Task<CartHeader> GetCartHeader(string userId)
        {
            return await cartDbContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public IEnumerable<CartDetails> GetCartDetails(int cartHeaderId)
        {
            return cartDbContext.CartDetails.Where(u => u.CartHeaderId == cartHeaderId);
        }

        public async Task CartUpsert(CartHeader cartHeader, IEnumerable<CartDetails> cartDetails)
        {
            var cardHeaderFromDb = await cartDbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartHeader.UserId);
            if (cardHeaderFromDb == null)
            {
                //Create cart header and cart details                
                cartDbContext.CartHeaders.Add(cartHeader);
                await cartDbContext.SaveChangesAsync();
                cartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                cartDbContext.CartDetails.Add(cartDetails.First());
                await cartDbContext.SaveChangesAsync();
            }
            else
            {
                //CartHeader exists for this user
                var cartDetailsFromDb = await cartDbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync
                    (u => u.ProductId == cartDetails.First().ProductId &&
                          u.CartHeaderId == cardHeaderFromDb.CartHeaderId
                    );

                if (cartDetailsFromDb == null)
                {
                    //Create new cart details
                    cartDetails.First().CartHeaderId = cardHeaderFromDb.CartHeaderId;
                    cartDbContext.CartDetails.Add(cartDetails.First());
                    await cartDbContext.SaveChangesAsync();
                }
                else
                {
                    //Update count in cart details
                    cartDetailsFromDb.Count += cartDetails.First().Count;
                    cartDbContext.CartDetails.Update(cartDetailsFromDb);
                    await cartDbContext.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveCart(int cartDetailId)
        {
            var cartDetailsFromDb = await cartDbContext.CartDetails.FirstAsync(u => u.CartDetailsId == cartDetailId);
            var totalCountOfCartItems = cartDbContext.CartDetails.Where(u => u.CartHeaderId == cartDetailsFromDb.CartHeaderId).Count();

            cartDbContext.CartDetails.Remove(cartDetailsFromDb); //Remove cart details

            if (totalCountOfCartItems == 1) //This is the last item in the cart
            {
                var cartHeaderToRemove = await cartDbContext.CartHeaders.FirstAsync(u => u.CartHeaderId == cartDetailsFromDb.CartHeaderId);
                cartDbContext.CartHeaders.Remove(cartHeaderToRemove);
            }

            await cartDbContext.SaveChangesAsync();
        }
    }
}

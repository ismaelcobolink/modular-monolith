
namespace Evently.Modules.Ticketing.Application.Carts;

public sealed class CartService()
{
    public async Task<Cart> GetAsync(Guid customerId)
    {
        var cart = Cart.CreateDefault(customerId);

        await Task.CompletedTask;

        return cart;
    }

    public async Task ClearAsync(Guid customerId)
    {
        Cart.CreateDefault(customerId);

        await Task.CompletedTask;
    }

    public async Task AddItemAsync(Guid customerId, CartItem cartItem)
    {
        Cart cart = await GetAsync(customerId);

        CartItem? existingCartItem = cart.Items.Find(c => c.TicketTypeId == cartItem.TicketTypeId);

        if (existingCartItem is null)
        {
            cart.Items.Add(cartItem);
        }
        else
        {
            existingCartItem.Quantity += cartItem.Quantity;
        }
    }

    public async Task RemoveItemAsync(Guid customerId, Guid ticketTypeId)
    {
        Cart cart = await GetAsync(customerId);

        CartItem? cartItem = cart.Items.Find(c => c.TicketTypeId == ticketTypeId);

        if (cartItem is null)
        {
            return;
        }

        cart.Items.Remove(cartItem);

    }

}

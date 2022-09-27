using Catalog.Models;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly InMemoryRepository repository;

        public ItemsController()
        {
            repository = new InMemoryRepository();
        }

        [HttpGet]
        public IEnumerable<Item> GetItems() 
            => repository.GetItems();

        [HttpGet("id")]
        public ActionResult<Item> GetItem(Guid id)
        {
            var item = repository.GetItem(id);

            if (item is null)
            {
                return NotFound();
            }

            return item;
        }

    }
}

using System;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Dtos;
using Catalog.Api.Models;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc; 
using Moq;
using Xunit;

namespace Catalog.UnitTests;

public class ItemsControllerTests
{
    private readonly Mock<IItemsRepository> repositoryStub = new Mock<IItemsRepository>();
    private readonly Random random = new Random();

    [Fact]
    public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
    {
        repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Item)null);

        var controller = new ItemsController(repositoryStub.Object);

        var result = await controller.GetItemAsync(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>(); 
    }

    [Fact]
    public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
    {
        var expectedItem = CreateRandomItem(); 

        repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedItem);

        var controller = new ItemsController(repositoryStub.Object);

        var result = await controller.GetItemAsync(Guid.NewGuid());

        result.Value.Should().BeEquivalentTo(
            expectedItem,
            options => options.ComparingByMembers<Item>()
        );
    }

    [Fact]
    public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
    {
        var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
        
        repositoryStub.Setup(repo => repo.GetItemsAsync())
            .ReturnsAsync(expectedItems);

        var controller = new ItemsController(repositoryStub.Object);

        var result = await controller.GetItemsAsync();

        result.Should().BeEquivalentTo(
            expectedItems,
            options => options.ComparingByMembers<Item>()
        );
    }

    [Fact]
    public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
    {
        var itemToCreate = new CreateItemDto()
        {
            Name = Guid.NewGuid().ToString(),
            Price = random.Next(1000)
        };
        
        var controller = new ItemsController(repositoryStub.Object);

        var result = await controller.CreateItemAsync(itemToCreate);

        var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
        itemToCreate.Should().BeEquivalentTo(
            createdItem,
            options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
        );
        createdItem.Id.Should().NotBeEmpty();
        createdItem.CreationDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0, 0, 1));
    }

    [Fact]
    public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
    {
        Item existingItem = CreateRandomItem();
        repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingItem);

        var itemId = existingItem.Id;
        var itemToUpdate = new UpdateItemDto()
        {
            Name = Guid.NewGuid().ToString(),
            Price = existingItem.Price - 1
        };
        
        var controller = new ItemsController(repositoryStub.Object);

        var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
    {
        Item existingItem = CreateRandomItem();
        repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingItem);
        
        var controller = new ItemsController(repositoryStub.Object);

        var result = await controller.DeleteItemAsync(existingItem.Id);

        result.Should().BeOfType<NoContentResult>();
    }

    private Item CreateRandomItem()
    {
        return new Item(){
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Price = random.Next(1000),
            CreationDate = DateTimeOffset.UtcNow
        };
    }
}
using AutoMapper;
using Interfaces.IManager;
using Moq;
using NUnit.Framework;
using Entity = DataAccess.Entities;
using Interfaces.IRepository;
using Models.DTOs;
using Managers;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace Testing;

[TestFixture]
public class Tests
{
    private Mock<IRepository<Entity.Category>> _categoryRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IInMemoryStore> _inMemoryStoreMock;
    private ICategoryManager _categoryManager;
    [SetUp]
    public void Setup()
    {
        _categoryRepositoryMock = new Mock<IRepository<Entity.Category>>();
        _mapperMock = new Mock<IMapper>();
        _inMemoryStoreMock = new Mock<IInMemoryStore>();
        _categoryManager = new CategoryManager(_categoryRepositoryMock.Object, _mapperMock.Object, _inMemoryStoreMock.Object);
    }

    [Test]
    public async Task GetAllCategoriesAsync_ReturnsMappedDtos_AndSuccess()
    {
        //Arrange
        var categories = new List<Entity.Category>
       {
           new Entity.Category{Id=1,CategoryName="Electronics"},
           new Entity.Category{Id=2,CategoryName="Books"}
       };
        _inMemoryStoreMock.Setup(store => store.TryGet(It.IsAny<string>(), out It.Ref<IEnumerable<Category>>.IsAny)).Returns(false);
        _categoryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);
        var categoryDtos = new List<Category>
       {
           new Category{Id=1,CategoryName="Electronics"},
           new Category{Id=2,CategoryName="Books"}
       };
        _mapperMock.Setup(m => m.Map<IEnumerable<Category>>(It.IsAny<IEnumerable<Entity.Category>>())).Returns(categoryDtos);
        //Act
        var result = await _categoryManager.GetAllCategoriesAsync();
        //Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Count(), Is.EqualTo(2));
        _categoryRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<Category>>(It.IsAny<IEnumerable<Entity.Category>>()), Times.Once);
    }
    [Test]
    public async Task GetCategoryByIdAsync_ReturnsMappedDto_AndSuccess()
    {
        //Arrange
        var categoryEntity = new Entity.Category { Id = 1, CategoryName = "Electronics" };
        _inMemoryStoreMock.Setup(s => s.TryGet(It.IsAny<string>(), out It.Ref<Category>.IsAny)).Returns(false);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(categoryEntity);
        var categoryDto = new Category { Id = 1, CategoryName = "Electronics" };
        _mapperMock.Setup(m => m.Map<Category>(It.IsAny<Entity.Category>())).Returns(categoryDto);
        //Act
        var result = await _categoryManager.GetCategoryByIdAsync(1);
        //Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Id, Is.EqualTo(1));
        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mapperMock.Verify(m => m.Map<Category>(It.IsAny<Entity.Category>()), Times.Once);
    }
    [Test]
    public async Task AddCategoryAsync_AddsCategory_AndReturnsSuccess()
    {
        //Arrange
        var categoryDto = new Category { Id = 1, CategoryName = "Electronics" };
        var categoryEntity = new Entity.Category { Id = 1, CategoryName = "Electronics" };
        _mapperMock.Setup(m => m.Map<Entity.Category>(It.IsAny<Category>())).Returns(categoryEntity);
        //Act
        var result = await _categoryManager.AddCategoryAsync(categoryDto);
        //Assert
        Assert.That(result.Success, Is.True);
        _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Entity.Category>()), Times.Once);
        _mapperMock.Verify(m => m.Map<Entity.Category>(It.IsAny<Category>()), Times.Once);

    }
    [Test]
    public async Task UpdateCategoryAsync_UpdatesCategory_AndReturnsSuccess()
    {
        //Arrange
        var categoryDto = new Category { Id = 1, CategoryName = "Electronics" };
        var existingEntity = new Entity.Category { Id = 1, CategoryName = "OldName" };
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingEntity);
        var updatedEntity = new Entity.Category { Id = 1, CategoryName = "Electronics" };
        _mapperMock.Setup(m => m.Map<Entity.Category>(It.IsAny<Category>())).Returns(updatedEntity);
        //Act
        var result = await _categoryManager.UpdateCategoryAsync(categoryDto);
        //Assert
        Assert.That(result.Success, Is.True);
        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _categoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Entity.Category>()), Times.Once);
    }
    [Test]
    public async Task DeleteCategoryAsync_DeletesCategory_AndReturnsSuccess()
    {
        //Arrange
        var existingEntity = new Entity.Category { Id = 1, CategoryName = "Electronics" };
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingEntity);
        //Act
        var result = await _categoryManager.DeleteCategoryAsync(1);
        //Assert
        Assert.That(result.Success, Is.True);
        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _categoryRepositoryMock.Verify(r => r.DeleteAsync(existingEntity), Times.Once);
    }
    [Test]
    public async Task GetAllCategoriesAsync_ReturnsFromCache_AndSuccess()
    {
        //Arrange
        IEnumerable<Category> cachedCategories = new List<Category>
        {
            new Category{Id=1,CategoryName="Electronics"},
            new Category{Id=2,CategoryName="Books"}
        };
        _inMemoryStoreMock.Setup(s => s.TryGet(It.IsAny<string>(), out cachedCategories)).Returns(true);
        //Act
        var result = await _categoryManager.GetAllCategoriesAsync();
        //Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Count(), Is.EqualTo(2));
        _categoryRepositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
        _mapperMock.Verify(m => m.Map<IEnumerable<Category>>(It.IsAny<IEnumerable<Entity.Category>>()), Times.Never);
    }
    [Test]
    public async Task GetAllCategoriesAsync_ReturnsDb_AndFailsWhenNotFound()
    {
        //Arrange
        _inMemoryStoreMock.Setup(s => s.TryGet(It.IsAny<string>(), out It.Ref<IEnumerable<Category>>.IsAny)).Returns(false);
        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Entity.Category>());
        //Act
        var result = await _categoryManager.GetAllCategoriesAsync();
        //Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Count(), Is.EqualTo(0));
        _categoryRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
    [Test]
    public async Task GetCategoryByIdAsync_ReturnsFromCache_AndSuccess()
    {
        var cachedCategory = new Category { Id = 1, CategoryName = "Electronics" };
        _inMemoryStoreMock.Setup(s => s.TryGet(It.IsAny<string>(), out cachedCategory)).Returns(true);
        //Act
        var result = await _categoryManager.GetCategoryByIdAsync(1);
        //Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Id, Is.EqualTo(1));
        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mapperMock.Verify(m => m.Map<Category>(It.IsAny<Entity.Category>()), Times.Never);
    }
    [Test]
    public async Task GetCategoryByIdAsync_ReturnsDb_AndFailsWhenNotFound()
    {
        //Arrange
        _inMemoryStoreMock.Setup(s => s.TryGet(It.IsAny<string>(), out It.Ref<Category>.IsAny)).Returns(false);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Entity.Category?)null);
        //Act
        var result = await _categoryManager.GetCategoryByIdAsync(1);
        //Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Data, Is.Null);
        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }
    [Test]
    public async Task UpdateCategoryAsync_FailsWhenNotFound()
    {
        //Arrange
        var categoryDto = new Category { Id = 1, CategoryName = "Electronics" };
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Entity.Category?)null);
        //Act
        var result = await _categoryManager.UpdateCategoryAsync(categoryDto);
        //Assert
        Assert.That(result.Success, Is.False);
        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _categoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Entity.Category>()), Times.Never);

    }
    [Test]
    public async Task DeleteCategoryAsync_FailsWhenNotFound()
    {
        //Arrange
        var existingEntity = new Entity.Category { Id = 1, CategoryName = "Electronics" };
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Entity.Category?)null);
        //Act
        var result = await _categoryManager.DeleteCategoryAsync(1);
        //Assert
        Assert.That(result.Success, Is.False);
        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _categoryRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Entity.Category>()), Times.Never);
    }
    [Test]
    public async Task AddCategoryAsync_HandlesException_AndReturnsFailure()
    {
        //Arrange
        var categoryDto = new Category { Id = 1, CategoryName = "Electronics" };
        var categoryEntity = new Entity.Category { Id = 1, CategoryName = "Electronics" };
        _mapperMock.Setup(m => m.Map<Entity.Category>(It.IsAny<Category>())).Returns(categoryEntity);
        _categoryRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Entity.Category>())).ThrowsAsync(new Exception("Db error"));
        //Act
        var result = await _categoryManager.AddCategoryAsync(categoryDto);
        //Assert
        Assert.That(result.Success, Is.False);
        _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Entity.Category>()), Times.Once);
        _mapperMock.Verify(m => m.Map<Entity.Category>(It.IsAny<Category>()), Times.Once);
    }
    [Test]
    public async Task UpdateCategoryAsync_HandlesException_AndReturnsFailure()
    {
        //Arrange
        var categoryDto = new Category { Id = 1, CategoryName = "Electronics" };
        var existingEntity = new Entity.Category { Id = 1, CategoryName = "OldName" };
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingEntity);
        _categoryRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Entity.Category>())).ThrowsAsync(new Exception("Db error"));
        //Act
        var result = await _categoryManager.UpdateCategoryAsync(categoryDto);
        //Assert
        Assert.That(result.Success, Is.False);
        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _categoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Entity.Category>()), Times.Once);
    }
    [Test]
    public async Task DeleteCategoryAsync_HandlesException_AndReturnsFailure()
    {
        //Arrange
        var categoryDto = new Category { Id = 1, CategoryName = "Electronics" };
        var existingEntity = new Entity.Category { Id = 1, CategoryName = "Electronics" };
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingEntity);
        _categoryRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Entity.Category>())).ThrowsAsync(new Exception("Db error"));
        //Act
        var result = await _categoryManager.DeleteCategoryAsync(1);
        //Assert
        Assert.That(result.Success, Is.False);
        _categoryRepositoryMock.Verify(r=>r.GetByIdAsync(1),Times.Once);
        _categoryRepositoryMock.Verify(r=>r.DeleteAsync(It.IsAny<Entity.Category>()),Times.Once);
    }
}

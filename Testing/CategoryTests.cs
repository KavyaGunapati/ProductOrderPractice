namespace Testing;
[TestFixture]
public class Tests
{
    private Mock<IRepository<Category>> _categoryRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private ICategoryManager _categoryManager;
    [SetUp]
    public void Setup()
    {
        _categoryRepositoryMock=new Mock<IRepository<Category>>();
        _mapperMock=new Mock<IMapper>();
        _categoryManager=new ICategoryManager(_categoryRepositoryMock.Object,_mapperMock.Object);
    }

    [Test]
    public async Task GetAllCategoriesAsync_ReturnsMappedDtos_AndSuccess()
    {
        // Arrange
        var categories=new List<Category>
        {
            new Category{Id=1,Name="Electronics"},
            new Category{Id=2,Name="Books"}
        };
        _categoryRepositoryMock.SetUp(repo=>repo.GetAllAsync()).ReturnsAsync(categories);
        var categoryDtos=new List<Category>
        {
            new Category{Id=1,Name="Electronics"},
            new Category{Id=2,Name="Books"}
        };
        _mapperMock.SetUp(m=>m.Map<IEnumerable<Category>>(It.IsAny<IEnumerable<Category>>())).ReturnsAsync(categoryDtos);
        // Act
        var result=await _categoryManager.GetAllCategoriesAsync();
        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2,result.Data.Count());
        _categoryRepositoryMock.Verify(repo=>repo.GetAllAsync(),Times.Once);
        _mapperMock.Verify(m=>m.Map<IEnumerable<Category>>(It.IsAny<IEnumerable<Category>>()),Times.Once);

    }
}

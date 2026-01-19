using AutoMapper;
using Moq;
using NUnit.Framework;
using Interfaces.IRepository;
using Interfaces.IManager;
using Managers;
using Models.DTOs;
using Entity=DataAccess.Entities;
namespace Testing
{
    [TestFixture]
    public class ProductTests
    {
        private ProductManager _productManager;
        //private Mock<IInMemoryStore> _mockStore;
        private Mock<IMapper> _mockMapper;
        private Mock<IRepository<Entity.Product>> _mockRepository;
        [SetUp]
        public void Setup()
        {
            //_mockStore=new Mock<IInMemoryStore>();
            _mockMapper=new Mock<IMapper>();
            _mockRepository=new Mock<IRepository<Entity.Product>>();
            _productManager=new ProductManager(/*_mockStore.Object,*/_mockRepository.Object,_mockMapper.Object);
        }
        [Test]
        public async Task GetProductById_ProductExists_ReturnsProduct()
        {
            //Arrange
            var productEntity=new Entity.Product{Id=1,ProductName="TestProduct"};
            _mockRepository.Setup(r=>r.GetByIdAsync(1)).ReturnsAsync(productEntity);
            var productDto=new Product{Id=1,ProductName="TestProduct"};
            _mockMapper.Setup(m=>m.Map<Product>(It.IsAny<Entity.Product>())).Returns(productDto);
            //Act
            var result=await _productManager.GetProductByIdAsync(1);
            //Assert
            Assert.That(result.Success,Is.True);
            Assert.That(result.Data,Is.EqualTo(productDto));
            _mockRepository.Verify(r=>r.GetByIdAsync(1),Times.Once);
            _mockMapper.Verify(m=>m.Map<Product>(It.IsAny<Entity.Product>()),Times.Once);

        }
        [Test]
        public async Task GetProductById_ProductDoesNotExist_ReturnsFailure()
        {
            //Arrange
            _mockRepository.Setup(r=>r.GetByIdAsync(1)).ReturnsAsync((Entity.Product?)null);
            //Act
            var result=await _productManager.GetProductByIdAsync(1);
            //Assert
            Assert.That(result.Success,Is.False);
            Assert.That(result.Data,Is.Null);
            _mockRepository.Verify(r=>r.GetByIdAsync(1),Times.Once);
            _mockMapper.Verify(m=>m.Map<Product>(It.IsAny<Entity.Product>()),Times.Never);

        }
        [Test]
        public  async Task GetAllProducts_ReturnsProductList()
        {
            //Arrange
            var productEntities=new List<Entity.Product>
            {
                new Entity.Product{Id=1,ProductName="Product1"},
                new Entity.Product{Id=2,ProductName="Product2"}
            };
            _mockRepository.Setup(r=>r.GetAllAsync()).ReturnsAsync(productEntities);
            var productDtos=new List<Product>
            {
                new Product{Id=1,ProductName="Product1"},
                new Product{Id=2,ProductName="Product2"}
            };
            _mockMapper.Setup(m=>m.Map<IEnumerable<Product>>(It.IsAny<IEnumerable<Entity.Product>>())).Returns(productDtos);
            //Act
            var result=await _productManager.GetAllProductsAsync();
            //Assert
            Assert.That(result.Success,Is.True);
            Assert.That(result.Data,Is.EqualTo(productDtos));
            _mockRepository.Verify(r=>r.GetAllAsync(),Times.Once);
            _mockMapper.Verify(m=>m.Map<IEnumerable<Product>>(It.IsAny<IEnumerable<Entity.Product>>()),Times.Once);
        }
        [Test]
        public async Task AddProduct_ValidProduct_ReturnsSuccess()
        {
            //Arrange
            var productDto=new Product{Id=1,ProductName="NewProduct"};
            var productEntity=new Entity.Product{Id=1,ProductName="NewProduct"};
            _mockMapper.Setup(m=>m.Map<Entity.Product>(It.IsAny<Product>())).Returns(productEntity);
            _mockRepository.Setup(r=>r.AddAsync(It.IsAny<Entity.Product>())).Returns(Task.CompletedTask);
            //Act
            var result=await _productManager.AddProductAsync(productDto);
            //Assert
            Assert.That(result.Success,Is.True);
            _mockMapper.Verify(m=>m.Map<Entity.Product>(It.IsAny<Product>()),Times.Once);
            _mockRepository.Verify(r=>r.AddAsync(It.IsAny<Entity.Product>()),Times.Once);
        }
    }
}
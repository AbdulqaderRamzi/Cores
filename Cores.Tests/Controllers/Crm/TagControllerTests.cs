using Cores.DataService.Repository.IRepository;
using Cores.Models.CRM;
using Cores.Web.Areas.CRM.Controllers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Cores.Tests.Controllers.Crm;

public class TagControllerTests
{
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly TagController _sut;

    public TagControllerTests()
    {
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        _sut = new TagController(_fakeUnitOfWork);
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithTags_WhenDataExists()
    {
        // Arrange
        var expectedTags = new List<Tag>
        {
            new() { Id = 1, Name = "Test Tag 1" },
            new() { Id = 2, Name = "Test Tag 2" }
        };

        A.CallTo(() => _fakeUnitOfWork.Tag.GetAll(null, null))
            .Returns(expectedTags);

        // Act
        var result = await _sut.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = (ViewResult)result;
        viewResult.Model.Should().BeAssignableTo<IEnumerable<Tag>>();
        viewResult.Model.Should().BeEquivalentTo(expectedTags);
        
        A.CallTo(() => _fakeUnitOfWork.Tag.GetAll(null, null))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Index_ShouldReturnEmptyView_WhenNoDataExists()
    {
        // Arrange
        A.CallTo(() => _fakeUnitOfWork.Tag.GetAll(null, null))
            .Returns(new List<Tag>()); // Empty Tag List

        // Act
        var result = await _sut.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = (ViewResult)result;
        viewResult.Model.Should().BeAssignableTo<IEnumerable<Tag>>();
        ((IEnumerable<Tag>)viewResult.Model!).Should().BeEmpty();
        
        A.CallTo(() => _fakeUnitOfWork.Tag.GetAll(null, null))
            .MustHaveHappenedOnceExactly();
    }
}
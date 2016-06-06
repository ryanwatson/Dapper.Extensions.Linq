using System;
using System.Linq;
using Dapper.Extensions.Linq.Core.Repositories;
using Dapper.Extensions.Linq.Test.Entities;
using NUnit.Framework;

namespace Dapper.Extensions.Linq.Test.IntegrationTests.Fixtures
{
    public abstract partial class FixturesBase
    {
        [Test]
        public void Skip_Returns()
        {
            // Arrange
            var personRepository = Container.Resolve<IRepository<Person>>();

            personRepository.Insert(new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "f", LastName = "f1", DateCreated = DateTime.UtcNow });

            // Act
            var result = personRepository.Query(person => true).OrderBy(person => person.Id).Skip(2).Take(2).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void Skip_Zero_ReturnsFirstRecord()
        {
            // Arrange
            var personRepository = Container.Resolve<IRepository<Person>>();

            personRepository.Insert(new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "f", LastName = "f1", DateCreated = DateTime.UtcNow });

            // Act
            var result = personRepository.Query(person => true).OrderBy(person => person.Id).Skip(0).Take(2).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First().FirstName, Is.EqualTo("a"));
        }

        [Test]
        public void Skip_WithoutSort_Throws()
        {
            // Arrange
            var personRepository = Container.Resolve<IRepository<Person>>();

            personRepository.Insert(new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "f", LastName = "f1", DateCreated = DateTime.UtcNow });

            // Act
            // Assert
            var ex = Assert.Throws<ArgumentNullException>(() => personRepository.Query(person => true).Skip(2).Take(2).ToList());
            Assert.That(ex.Message, Does.Contain("Sort cannot be null or empty"));
        }

        [Test]
        public void Skip_WithoutTake_Throws()
        {
            // Arrange
            var personRepository = Container.Resolve<IRepository<Person>>();

            personRepository.Insert(new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });
            personRepository.Insert(new Person { Active = false, FirstName = "f", LastName = "f1", DateCreated = DateTime.UtcNow });

            // Act
            // Assert
            var ex = Assert.Throws<ArgumentNullException>(() => personRepository.Query(person => true).OrderBy(person => person.Id).Skip(2).ToList());
            Assert.That(ex.Message, Does.Contain("Skip requires Take"));
        }
    }
}
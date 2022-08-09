using CleanArchitecture.Blazor.Application.Common.Exceptions;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using System.Collections.Generic;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Exceptions;

public class ValidationExceptionTests
{


    [Test]
    public void SingleValidationFailureCreatesASingleElementErrorDictionary()
    {
        var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Age", "'Age' must be over 18"),
            };

        var actual = new ValidationException(failures).ErrorMessages;

        actual.Should().BeEquivalentTo(new List<string>() { "'Age' must be over 18" });
        
    }

    [Test]
    public void MulitpleValidationFailureForMultiplePropertiesCreatesAMultipleElementErrorDictionaryEachWithMultipleValues()
    {
        var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Age", "must be 18 or older"),
                new ValidationFailure("Age", "must be 25 or younger"),
                new ValidationFailure("Password", "must contain at least 8 characters"),
                new ValidationFailure("Password", "must contain a digit"),
                new ValidationFailure("Password", "must contain upper case letter"),
                new ValidationFailure("Password", "must contain lower case letter"),
            };

        var actual = new ValidationException(failures).ErrorMessages;

        actual.Should().NotBeEmpty();

        

    }
}


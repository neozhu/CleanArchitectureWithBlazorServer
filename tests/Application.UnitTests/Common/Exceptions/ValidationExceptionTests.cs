using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Exceptions;

public class ValidationExceptionTests
{
    [Test]
    public void SingleValidationFailureCreatesASingleElementErrorDictionary()
    {
        var expectedErrorMessage = "'Age' must be over 18";

        var failures = new List<ValidationFailure>
        {
            new("Age", expectedErrorMessage)
        };
        var validationException = new ValidationException(failures);
        var actualErrorMessages = validationException.Errors.Select(x => x.ErrorMessage);
        actualErrorMessages.Should().BeEquivalentTo(new List<string> { expectedErrorMessage });
    }

    [Test]
    public void
        MultipleValidationFailureForMultiplePropertiesCreatesAMultipleElementErrorDictionaryEachWithMultipleValues()
    {
        var failures = new List<ValidationFailure>
        {
            new("Age", "must be 18 or older"),
            new("Age", "must be 25 or younger"),
            new("Password", "must contain at least 8 characters"),
            new("Password", "must contain a digit"),
            new("Password", "must contain upper case letter"),
            new("Password", "must contain lower case letter")
        };

        var actual = new ValidationException(failures).Errors;

        actual.Should().NotBeEmpty();
    }
}
using AutoMapper;
using BankCore.Application.DTOs.Responses;
using BankCore.Application.Profiles;
using BankCore.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;

namespace BankCore.Tests;

public class CustomerEmailOwnershipTests
{
    [Fact]
    public void Customer_does_not_expose_an_email_property()
    {
        Assert.Null(typeof(Customer).GetProperty("Email"));
    }

    [Fact]
    public void Customer_response_maps_email_from_related_user()
    {
        var (customer, user) = CreateCustomerWithUser();
        var mapper = new MapperConfiguration(
            configuration => configuration.AddProfile<CustomerProfile>(),
            NullLoggerFactory.Instance).CreateMapper();

        var response = mapper.Map<CustomerResponse>(customer);

        Assert.Equal(user.Email, response.Email);
    }

    [Fact]
    public void Updating_customer_details_does_not_change_related_user_email()
    {
        var (customer, user) = CreateCustomerWithUser();
        var originalEmail = user.Email;

        customer.UpdateDetails("Updated", "Customer");

        Assert.Equal(originalEmail, user.Email);
        Assert.Equal("Updated", customer.FirstName);
        Assert.Equal("Customer", customer.LastName);
    }

    private static (Customer Customer, User User) CreateCustomerWithUser()
    {
        var user = new User("Customer@Example.com");
        var customer = new Customer(
            1,
            "Test",
            "Customer",
            DateTime.UtcNow.Date.AddYears(-30));

        typeof(Customer)
            .GetProperty(nameof(Customer.User), BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(customer, user);

        return (customer, user);
    }
}

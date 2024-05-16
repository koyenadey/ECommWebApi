using ECommWeb.Core.src.Entity;

namespace ECommWeb.Business.src.DTO;

public class AddressReadDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string EmailAddress { get; set; }
    public string AddressLine { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Postcode { get; set; }
    public string PhoneNumber { get; set; }
    public string Landmark { get; set; }

}

public class AddressCreateDto
{
    public Guid UserId { get; set; }
    public string? AddressLine { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Postcode { get; set; }
    public string PhoneNumber { get; set; }
    public string? Landmark { get; set; }

}

public class AddressUpdateDto
{
    public string AddressLine { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Postcode { get; set; }
    public string PhoneNumber { get; set; }
    public string Landmark { get; set; }

}

using System.ComponentModel.DataAnnotations;

namespace CatalogService.Contracts;

// DTO 1
public record ClientDto(int Id, string FullName, string Email, string Phone, string City);

// DTO 2
public record ClientDtoV2(int Id, string LastName, string FirstName, string Email, string Phone, string City);

// create/upd
public record CreateClientRequest(
    [Required, StringLength(200)] string FullName,
    [Required, EmailAddress, StringLength(254)] string Email,
    [Required, Phone, StringLength(20)] string Phone,
    [Required, StringLength(100)] string City);

public record UpdateClientRequest(
    [Required, StringLength(200)] string FullName,
    [Required, EmailAddress, StringLength(254)] string Email,
    [Required, Phone, StringLength(20)] string Phone,
    [Required, StringLength(100)] string City);

public record PagedResult<T>(
    IReadOnlyList<T> Items, int Page, int PageSize, int TotalItems, int TotalPages);
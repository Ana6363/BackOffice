using BackOffice.Domain.Patients;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Persistence.Models;

public static class UserMapper
{
    public static UserDto ToDto(User domainModel)
    {
        return new UserDto
        {
            Id = domainModel.Id.AsString(),
            Role = domainModel.Role,
            Active = domainModel.Active,
            ActivationToken = domainModel.ActivationToken,
            TokenExpiration = domainModel.TokenExpiration,
            FirstName = domainModel.FirstName.NameValue,
            LastName = domainModel.LastName.NameValue,
            FullName = domainModel.FullName.NameValue,
            IsToBeDeleted = domainModel.IsToBeDeleted
        };
    }

    public static UserDataModel ToDataModel(User domainModel)
    {
        return new UserDataModel
        {
            Id = domainModel.Id.AsString(),
            Role = domainModel.Role,
            Active = domainModel.Active,
            ActivationToken = domainModel.ActivationToken,
            TokenExpiration = domainModel.TokenExpiration,
            FirstName = domainModel.FirstName.NameValue,
            LastName = domainModel.LastName.NameValue,
            FullName = domainModel.FullName.NameValue,
            IsToBeDeleted = domainModel.IsToBeDeleted
        };
    }

    public static User ToDomain(UserDataModel dataModel)
    {
        var user = new User(
            dataModel.Id,
            dataModel.Role,
            new Name(dataModel.FirstName), 
            new Name(dataModel.LastName),   
            new Name(dataModel.FullName)
        );

        if (dataModel.Active)
        {
            user.MarkAsActive();
        }
        else
        {
            user.MarkAsInactive();
        }

        user.ActivationToken = dataModel.ActivationToken;
        user.TokenExpiration = dataModel.TokenExpiration;

        if (dataModel.IsToBeDeleted)
        {
            user.MarkDeleteAsActive();
        }

        return user;
    }

    public static User ToDomain(UserDto dto)
    {
        var user = new User(
            dto.Id,
            dto.Role,
            new Name(dto.FirstName),
            new Name(dto.LastName),   
            new Name(dto.FullName)    
        )
        {
            Active = dto.Active,
            ActivationToken = dto.ActivationToken,
            TokenExpiration = dto.TokenExpiration
        };

        if (dto.IsToBeDeleted)
        {
            user.MarkDeleteAsActive();
        }

        return user;
    }
}

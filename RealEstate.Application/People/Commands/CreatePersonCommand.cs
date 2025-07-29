using MediatR;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.People.Commands
{
    public record CreatePersonCommand(
        
        string FullName, 
        string NationallD, 
        string ImageURL, 
        string Phone, 
        enGender Gender, 
        DateOnly DateOfBirth
        ) : IRequest<Guid>;

}

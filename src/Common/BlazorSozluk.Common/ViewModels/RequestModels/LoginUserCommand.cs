using System;
using BlazorSozluk.Common.ViewModels.Queries;
using MediatR;

namespace BlazorSozluk.Common.ViewModels.RequestModels
{
    public class LoginUserCommand : IRequest<LoginUserViewModel>
    {

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public LoginUserCommand(string emailAddress, string password)
        {
            EmailAddress = emailAddress;
            Password = password;
        }

        public LoginUserCommand()
        {

        }



    }
}


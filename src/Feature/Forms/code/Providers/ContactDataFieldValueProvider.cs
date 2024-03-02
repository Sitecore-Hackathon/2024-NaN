﻿using Sitecore.Demo.Platform.Foundation.Accounts.Models;
using Sitecore.Demo.Platform.Foundation.Accounts.Services;
using Sitecore.DependencyInjection;
using Sitecore.ExperienceForms.ValueProviders;

namespace Sitecore.Demo.Platform.Feature.Forms.Providers
{
    public class ContactDataFieldValueProvider : IFieldValueProvider
    {
        private readonly IContactFacetService _contactFacetService;
        public FieldValueProviderContext ValueProviderContext { get; set; }

        public ContactDataFieldValueProvider()
        {
            _contactFacetService = (IContactFacetService)ServiceLocator.ServiceProvider.GetService(typeof(IContactFacetService));
        }

        public object GetValue(string parameters)
        {
            ContactFacetData data = _contactFacetService.GetContactData();

            switch(parameters.ToLower())
            {
                case "email":
                    return data.EmailAddress;
                case "first name":
                    return data.FirstName;
                case "last name":
                    return data.LastName;
                case "phone":
                    return data.PhoneNumber;
                default:
                    return string.Empty;                    
            }
        }
    }
}